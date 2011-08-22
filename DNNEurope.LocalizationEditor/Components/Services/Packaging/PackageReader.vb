' 
' Copyright (c) 2004-2011 DNN-Europe, http://www.dnn-europe.net
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this 
' software and associated documentation files (the "Software"), to deal in the Software 
' without restriction, including without limitation the rights to use, copy, modify, merge, 
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
' to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or 
' substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
' ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
' 
Imports System.Xml
Imports System.IO
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Entities.Texts
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Helpers

Namespace Services.Packaging
 Public Class PackageReader

  ''' <summary>
  ''' A module definition to import
  ''' </summary>
  ''' <remarks></remarks>
  Private Class ManifestModuleInfo
   Public ObjectName As String
   Public FriendlyName As String = ""
   Public FolderName As String = ""
   Public Version As String = "0"
   Public PackageType As String = "Module"
   Public ResourceFiles As New SortedList
   Public DnnCoreVersion As String = ""
  End Class

  ''' <summary>
  ''' Import an object from package. The zip is extracted to a temporary directory and searched for resource files. The object is added to the database and its resource files are read.
  ''' </summary>
  ''' <param name="moduleContent">Package content as stream</param>
  ''' <param name="HomeDirectoryMapPath">HomeDirectoryMapPath of this portal (needed for the temp path)</param>
  ''' <param name="ModuleId">To be used for an installed module</param>
  ''' <remarks></remarks>
  Public Shared Sub ImportModulePackage(ByVal moduleContent As IO.Stream, ByVal HomeDirectoryMapPath As String, ByVal ModuleId As Integer, DnnCoreVersion As String, InstalledByDefault As Boolean)
   ' Create a temporary directory to unpack the package
   Dim tempDirectory As String = HomeDirectoryMapPath & "LocalizationEditor\~tmp" & Now.ToString("yyyyMMdd-hhmmss") & "-" & (CInt(Rnd() * 1000)).ToString

   ' Check if the temporary directory already exists
   If Not Directory.Exists(tempDirectory) Then
    Directory.CreateDirectory(tempDirectory)
   Else
    Throw New IOException("New directory " & tempDirectory & " already exists")
   End If
   Globals.CleanupTempDirs(HomeDirectoryMapPath & "LocalizationEditor")

   ' Unzip file contents
   ZipHelper.Unzip(moduleContent, tempDirectory)

   ' Find the DNN manifest file
   Dim dnnFiles As String() = Directory.GetFiles(tempDirectory, "*.dnn")
   If dnnFiles.Length = 0 Then
    If Directory.GetFiles(tempDirectory, "Default.aspx").Length = 0 Then
     Throw New FileNotFoundException("No DNN Manifest file found, nor a core distribution")
    End If
   End If
   ' Multiple manifests are allowed (.dnn and .dnn5)

   ' Now process what has been uploaded
   Dim manifestModules As New List(Of ManifestModuleInfo)

   If dnnFiles.Length = 0 Then ' we're processing the core

    Dim parsedVersion As String = Globals.GetAssemblyVersion(tempDirectory & "\bin\DotNetNuke.dll")

    Dim core As New ManifestModuleInfo
    With core
     .FriendlyName = Globals.glbCoreFriendlyName
     .ObjectName = Globals.glbCoreName
     .Version = parsedVersion
     .FolderName = ""
     .PackageType = Globals.glbCoreName
    End With
    ReadResourceFiles(core, "", tempDirectory)
    manifestModules.Add(core)

    ' we need to 'import' modules/providers etc.
    For Each d As DirectoryInfo In (New DirectoryInfo(tempDirectory & "\Install")).GetDirectories
     For Each obj As FileInfo In d.GetFiles("*.zip")
      Using objStream As New IO.FileStream(obj.FullName, FileMode.Open, FileAccess.Read)
       ImportModulePackage(objStream, HomeDirectoryMapPath, ModuleId, parsedVersion, True)
      End Using
     Next
    Next

    ' also 'import' non-installed modules/providers etc.
    For Each d As DirectoryInfo In (New DirectoryInfo(tempDirectory & "\Install")).GetDirectories
     For Each obj As FileInfo In d.GetFiles("*.resources")
      Using objStream As New IO.FileStream(obj.FullName, FileMode.Open, FileAccess.Read)
       Try
        ImportModulePackage(objStream, HomeDirectoryMapPath, ModuleId, parsedVersion, False)
       Catch ex As Exception
        ' ignore errors
       End Try
      End Using
     Next
    Next

   Else ' we're processing a package with a manifest

    Dim dnnManifestFile As String = dnnFiles(0)

    ' Parse DNN manifest file
    Dim manifest As New XmlDocument
    manifest.Load(dnnManifestFile)
    Dim manifestVersion As Integer = 0

    ' Determine version of manifest file
    Dim mainNodes As XmlNodeList
    mainNodes = manifest.SelectNodes("dotnetnuke/packages/package")
    If mainNodes.Count > 0 Then
     manifestVersion = 5
    Else
     mainNodes = manifest.SelectNodes("dotnetnuke/folders/folder")
     If mainNodes.Count > 0 Then
      manifestVersion = 3
     End If
    End If

    If manifestVersion = 5 Then
     ' Remark about DNN 5 manifest file: it is assumed that only one <desktopModule> node per <package> node exists.

     ' Create a module for each package
     For Each packageNode As XmlNode In mainNodes

      Dim manifestModule As New ManifestModuleInfo()
      manifestModule.DnnCoreVersion = DnnCoreVersion

      ' Determine the version
      If Not packageNode.SelectSingleNode("@version") Is Nothing Then
       manifestModule.Version = Globals.FormatVersion(packageNode.SelectSingleNode("@version").InnerText.Trim)
       If String.IsNullOrEmpty(manifestModule.Version) Then
        manifestModule.Version = "0"
        '2009-06-26 Janga:  Default version.
       End If
      Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
      End If

      ' Determine the object name
      If Not packageNode.SelectSingleNode("@name") Is Nothing Then
       manifestModule.ObjectName = packageNode.SelectSingleNode("@name").InnerText.Trim
      Else
       Throw New Exception("Could not retrieve package name in DNN Manifest file")
      End If

      ' Determine the friendly name
      If Not packageNode("friendlyName") Is Nothing Then
       manifestModule.FriendlyName = packageNode("friendlyName").InnerText
      Else
       manifestModule.FriendlyName = manifestModule.ObjectName
      End If

      manifestModule.PackageType = packageNode.SelectSingleNode("@type").InnerText

      Select Case manifestModule.PackageType.ToLower

       Case "module"

        ' Determine the desktop module
        Dim moduleNodes As XmlNodeList
        moduleNodes = packageNode.SelectNodes("components/component[@type='Module']/desktopModule")
        If moduleNodes.Count = 0 Then
         Throw New Exception("Could not retrieve desktop module information in DNN Manifest file")
        End If
        If moduleNodes.Count > 1 Then
         Throw New Exception("Multiple desktop modules found in DNN Manifest file")
        End If

        For Each dmNode As XmlNode In moduleNodes
         ' Determine the folder name
         If Not dmNode("foldername") Is Nothing Then
          manifestModule.FolderName = dmNode("foldername").InnerText.Replace("/"c, "\"c)
         Else
          Throw New Exception("Could not retrieve folder information in DNN Manifest file")
         End If
        Next

        ' Find the resource files using the manifest xml
        For Each fileGroupNode As XmlNode In packageNode.SelectNodes("components/component[@type='File']/files")
         Dim basePath As String = Path.Combine("DesktopModules", manifestModule.FolderName)
         If fileGroupNode("basePath") IsNot Nothing Then
          basePath = fileGroupNode("basePath").InnerText.Replace("/"c, "\"c).Trim("\"c)
         End If
         For Each node As XmlNode In fileGroupNode.SelectNodes("file")
          Dim resFile As String = ""
          Dim resDir As String = ""
          If Not node("name") Is Nothing Then resFile = node("name").InnerText
          If Not node("path") Is Nothing Then resDir = node("path").InnerText
          If resFile.ToLower.EndsWith(".resx") Then
           Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
           If Not node("sourceFileName") Is Nothing Then resPath = Path.Combine(Path.Combine(tempDirectory, resDir), node("sourceFileName").InnerText)
           Dim resKey As String = Path.Combine(Path.Combine(basePath, resDir), resFile)
           manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
          End If
         Next
        Next

       Case "provider"

        ' Find the resource files using the manifest xml
        For Each fileGroupNode As XmlNode In packageNode.SelectNodes("components/component[@type='File']/files")
         Dim resourceFiles As String = ""
         Dim basePath As String = ""
         If fileGroupNode("basePath") IsNot Nothing Then
          basePath = fileGroupNode("basePath").InnerText.Replace("/"c, "\"c).Trim("\"c)
         End If
         For Each node As XmlNode In fileGroupNode.SelectNodes("file")
          Dim resFile As String = ""
          Dim resDir As String = ""
          If Not node("name") Is Nothing Then resFile = node("name").InnerText
          If Not node("path") Is Nothing Then resDir = node("path").InnerText
          If resFile.ToLower.EndsWith(".resx") Then
           Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
           If Not node("sourceFileName") Is Nothing Then resPath = Path.Combine(Path.Combine(tempDirectory, resDir), node("sourceFileName").InnerText)
           Dim resKey As String = Path.Combine(Path.Combine(basePath, resDir), resFile)
           manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
          End If
         Next
        Next

       Case "skin"

        ' Find the resource files using the manifest xml
        For Each fileGroupNode As XmlNode In packageNode.SelectNodes("components/component[@type='Skin']/skinFiles")
         Dim basePath As String = ""
         If fileGroupNode("basePath") IsNot Nothing Then
          basePath = fileGroupNode("basePath").InnerText.Replace("/"c, "\"c).Trim("\"c)
         End If
         For Each node As XmlNode In fileGroupNode.SelectNodes("skinFile")
          Dim resFile As String = ""
          Dim resDir As String = ""
          If Not node("name") Is Nothing Then resFile = node("name").InnerText
          If Not node("path") Is Nothing Then resDir = node("path").InnerText
          If resFile.ToLower.EndsWith(".resx") Then
           Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
           If Not node("sourceFileName") Is Nothing Then resPath = Path.Combine(Path.Combine(tempDirectory, resDir), node("sourceFileName").InnerText)
           Dim resKey As String = Path.Combine(Path.Combine(basePath, resDir), resFile)
           manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
          End If
         Next
        Next

       Case "container"

        ' Find the resource files using the manifest xml
        For Each fileGroupNode As XmlNode In packageNode.SelectNodes("components/component[@type='Container']/containerFiles")
         Dim basePath As String = ""
         If fileGroupNode("basePath") IsNot Nothing Then
          basePath = fileGroupNode("basePath").InnerText.Replace("/"c, "\"c).Trim("\"c)
         End If
         For Each node As XmlNode In fileGroupNode.SelectNodes("containerFile")
          Dim resFile As String = ""
          Dim resDir As String = ""
          If Not node("name") Is Nothing Then resFile = node("name").InnerText
          If Not node("path") Is Nothing Then resDir = node("path").InnerText
          If resFile.ToLower.EndsWith(".resx") Then
           Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
           If Not node("sourceFileName") Is Nothing Then resPath = Path.Combine(Path.Combine(tempDirectory, resDir), node("sourceFileName").InnerText)
           Dim resKey As String = Path.Combine(Path.Combine(basePath, resDir), resFile)
           manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
          End If
         Next
        Next

      End Select

      ' Handle resource files
      Dim i As Integer = 0
      For Each resFileNode As XmlNode In packageNode.SelectNodes("components/component[@type='ResourceFile']")
       Dim basePath As String = resFileNode.SelectSingleNode("resourceFiles/basePath").InnerText.Replace("/"c, "\"c).Trim("\"c)
       Dim resFile As String = resFileNode.SelectSingleNode("resourceFiles/resourceFile/name").InnerText
       ZipHelper.Unzip(tempDirectory & "\" & resFile, tempDirectory & "\ResourceFiles" & i.ToString)
       ReadResourceFiles(manifestModule, basePath, tempDirectory & "\ResourceFiles" & i.ToString)
       i += 1
      Next

      ' Add the manifest module to the collection
      manifestModules.Add(manifestModule)
     Next

    ElseIf manifestVersion = 3 Then

     ' Create a module for each folder
     For Each folderNode As XmlNode In mainNodes

      Dim manifestModule As New ManifestModuleInfo()
      manifestModule.DnnCoreVersion = DnnCoreVersion

      ' Determine the module name
      If Not folderNode("modulename") Is Nothing Then
       manifestModule.ObjectName = folderNode("modulename").InnerText
      ElseIf Not folderNode("friendlyname") Is Nothing Then
       manifestModule.ObjectName = folderNode("friendlyname").InnerText
      ElseIf Not folderNode("name") Is Nothing Then
       manifestModule.ObjectName = folderNode("name").InnerText
      Else : Throw New Exception("Could not retrieve module name in DNN Manifest file")
      End If

      manifestModule.PackageType = manifest.SelectSingleNode("dotnetnuke/@type").InnerText

      ' Determine the friendly name
      If Not folderNode("friendlyname") Is Nothing Then
       manifestModule.FriendlyName = folderNode("friendlyname").InnerText
      Else : manifestModule.FriendlyName = manifestModule.ObjectName
      End If

      ' Determine the folder name
      If Not folderNode("foldername") Is Nothing Then
       manifestModule.FolderName = folderNode("foldername").InnerText.Replace("/"c, "\"c)
      Else : Throw New Exception("Could not retrieve folder information in DNN Manifest file")
      End If

      ' Determine the version
      If Not folderNode("version") Is Nothing Then
       manifestModule.Version = folderNode("version").InnerText.Trim
       If String.IsNullOrEmpty(manifestModule.Version) Then
        manifestModule.Version = "0"
        '2009-06-26 Janga:  Default version.
       End If
      Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
      End If

      ' Find the resource files using the manifest xml
      For Each node As XmlNode In folderNode.SelectNodes("files/file")
       Dim resFile As String = ""
       Dim resDir As String = ""
       If Not node("name") Is Nothing Then resFile = node("name").InnerText
       If Not node("path") Is Nothing Then resDir = node("path").InnerText

       'TODO Support resource files which are already localized
       If resFile.ToLower.EndsWith("ascx.resx") OrElse resFile.ToLower.EndsWith("aspx.resx") Then
        ' Determine the resource directory and key for the module
        Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
        Dim resKey As String = Path.Combine(Path.Combine(Path.Combine("DesktopModules", manifestModule.FolderName), resDir), resFile)
        manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
       End If
      Next

      ' Find the resource file
      If Not folderNode("resourcefile") Is Nothing Then
       Dim basePath As String = "DesktopModules\" & manifestModule.FolderName
       Dim resFile As String = folderNode("resourcefile").InnerText
       ZipHelper.Unzip(tempDirectory & "\" & resFile, tempDirectory & "\ResourceFiles")
       ReadResourceFiles(manifestModule, basePath, tempDirectory & "\ResourceFiles")
      End If

      ' Add the manifest module to the collection
      manifestModules.Add(manifestModule)
     Next
    Else : Throw New Exception("Could not determine version of manifest file")
    End If

   End If ' whether core or package

   ' TODO Use transactions
   ' DNN Manifest file or core parsed succesfully, now process each module from the manifest
   For Each manifestModule As ManifestModuleInfo In manifestModules
    If manifestModule.ObjectName IsNot Nothing Then
     If manifestModule.ResourceFiles.Count > 0 Then

      ' Check if the module is already imported
      Dim objObjectInfo As ObjectInfo = ObjectsController.GetObjectByObjectName(ModuleId, manifestModule.ObjectName)
      If objObjectInfo Is Nothing Then
       ' Create a new translate module
       objObjectInfo = New ObjectInfo(0, manifestModule.FriendlyName, ModuleId, manifestModule.FolderName, manifestModule.ObjectName, manifestModule.PackageType)
       objObjectInfo.ObjectId = ObjectsController.AddObject(objObjectInfo)
      End If
      If manifestModule.DnnCoreVersion <> "" Then
       ObjectCoreVersionsController.SetObjectCoreVersion(objObjectInfo.ObjectId, manifestModule.Version, manifestModule.DnnCoreVersion, InstalledByDefault)
      End If

      ' Import or update resource files for this module
      ProcessResourceFiles(manifestModule.ResourceFiles, tempDirectory, objObjectInfo, manifestModule.Version)

     End If
    End If
   Next

   ' Try to clean up
   Try
    IO.Directory.Delete(tempDirectory, True)
   Catch
   End Try

  End Sub

  Private Shared Sub ReadResourceFiles(ByRef manifestModule As ManifestModuleInfo, ByVal keyBasePath As String, ByVal path As String)

   If keyBasePath <> "" Then
    keyBasePath &= "\"
   End If

   For Each f As FileInfo In (New DirectoryInfo(path)).GetFiles("*.as?x.resx")
    If manifestModule.ResourceFiles(keyBasePath & f.Name) Is Nothing Then
     manifestModule.ResourceFiles.Add(keyBasePath & f.Name, f)
    End If
   Next
   For Each f As FileInfo In (New DirectoryInfo(path)).GetFiles("*Resources.resx")
    If manifestModule.ResourceFiles(keyBasePath & f.Name) Is Nothing Then
     manifestModule.ResourceFiles.Add(keyBasePath & f.Name, f)
    End If
   Next

   For Each d As String In Directory.GetDirectories(path)
    ReadResourceFiles(manifestModule, keyBasePath & Mid(d, d.LastIndexOf("\") + 2), d)
   Next

  End Sub

  Public Shared Sub ProcessResourceFiles(ByVal resourceFileList As SortedList, ByVal rootPath As String, ByVal objObjectInfo As ObjectInfo, ByVal version As String)

   Const pattern As String = ".resx"

   ' Add new stuff
   Dim currentVersionKeys As New List(Of String)
   For Each file As DictionaryEntry In resourceFileList
    Dim fileKey As String = file.Key.ToString.Replace(rootPath, "").Replace(pattern, ".resx")
    Dim resFile As New XmlDocument

    Dim fi As FileInfo = CType(file.Value, FileInfo)
    If Not IO.File.Exists(fi.FullName) Then
     ' This can happen in DNN 3 manifests as the location of the resx file can be in the root or the place it should be
     Dim testFile As String = Path.Combine(rootPath, fi.Name)
     If IO.File.Exists(testFile) Then
      fi = New FileInfo(testFile)
     End If
    End If

    Try
     resFile.Load(fi.FullName)
    Catch
     Try
      Dim fc As String = Globals.ReadFile(fi.FullName)
      resFile.LoadXml(fc)
     Catch ex As Exception
      Globals.SimpleLog("Original resource file '" & file.Key.ToString & "' is incorrect or could not be read.")
     End Try
    End Try

    For Each x As XmlNode In resFile.DocumentElement.SelectNodes("/root/data")
     Dim key As String = x.Attributes("name").InnerText
     Dim value As String = x.SelectSingleNode("value").InnerXml
     currentVersionKeys.Add(fileKey.ToLower & ";" & key.ToLower)
     Dim ti As Entities.Texts.TextInfo = TextsController.GetLatestText(objObjectInfo.ObjectId, fileKey, "", key)

     Try
      If ti Is Nothing Then
       ti = New Entities.Texts.TextInfo(-1, "", fileKey, objObjectInfo.ObjectId, value, key, version)
       TextsController.AddText(ti)
      ElseIf ti.OriginalValue <> value Then
       If ti.Version = version Then
        ti.OriginalValue = value
        TextsController.UpdateText(ti)
       Else ' new version
        ' deprecate the old one
        ti.DeprecatedIn = version
        TextsController.UpdateText(ti)
        ' add new one
        ti = New Entities.Texts.TextInfo(-1, "", fileKey, objObjectInfo.ObjectId, value, key, version)
        TextsController.AddText(ti)
       End If
      End If
     Catch ex As Exception
      Globals.SimpleLog("Error importing resources: " & ex.Message, ex.StackTrace, "Filekey: " & fileKey, "Key    : " & key, "Value  : " & value, "")
     End Try
    Next
   Next

   ' Deprecate old stuff
   For Each ti As Entities.Texts.TextInfo In TextsController.GetTextsByObject(objObjectInfo.ModuleId, objObjectInfo.ObjectId, "", version).Values
    If ti.Version <> version Then ' it's an old one
     If Not currentVersionKeys.Contains(ti.FilePath.ToLower & ";" & ti.TextKey.ToLower) Then
      ti.DeprecatedIn = version
      TextsController.UpdateText(ti)
     End If
    End If
   Next
  End Sub

 End Class
End Namespace

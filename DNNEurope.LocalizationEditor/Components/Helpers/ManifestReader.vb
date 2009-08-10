Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Business

Public Class ManifestReader

 ''' <summary>
 ''' A module definition to import
 ''' </summary>
 ''' <remarks></remarks>
 Private Class ManifestModuleInfo
  Public ModuleName As String
  Public FriendlyName As String
  Public FolderName As String
  Public Version As String
  Public ResourceFiles As New SortedList
 End Class

 ''' <summary>
 ''' Import an object from package. The zip is extracted to a temporary directory and searched for resource files. The object is added to the database and its resource files are read.
 ''' </summary>
 ''' <param name="modulePackagePath">Name of the uploaded package file</param>
 ''' <param name="moduleContent">Package content as stream</param>
 ''' <param name="HomeDirectoryMapPath">HomeDirectoryMapPath of this portal (needed for the temp path)</param>
 ''' <remarks></remarks>
 Public Shared Sub ImportModulePackage(ByVal modulePackagePath As String, ByVal moduleContent As IO.Stream, ByVal HomeDirectoryMapPath As String)
  '// Create a temporary directory to unpack the package
  Dim tempDirectory As String = HomeDirectoryMapPath & "LocalizationEditor\~tmp" & Now.ToString("yyyyMMdd-hhmmss") & "-" & (CInt(Rnd() * 1000)).ToString

  '// Check if the temporary directory already exists
  If Not Directory.Exists(tempDirectory) Then
   Directory.CreateDirectory(tempDirectory)
  Else
   Throw New IOException("New directory " & tempDirectory & " already exists")
  End If

  '// Unzip file contents
  ZipHelper.Unzip(moduleContent, tempDirectory)

  '// Find the DNN manifest file
  Dim dnnFiles As String() = Directory.GetFiles(tempDirectory, "*.dnn")
  If dnnFiles.Length = 0 Then
   If Directory.GetFiles(tempDirectory, "Default.aspx").Count = 0 Then
    Throw New FileNotFoundException("No DNN Manifest file found, nor a core distribution")
   End If
  End If
  If dnnFiles.Length > 1 Then Throw New IOException("Multiple DNN Manifest files found")

  ' Now process what has been uploaded
  Dim manifestModules As New List(Of ManifestModuleInfo)

  If dnnFiles.Length = 0 Then ' we're processing the core

   Dim core As New ManifestModuleInfo
   With core
    .FriendlyName = "DNN Core"
    .ModuleName = "Core"
    .Version = GetAssemblyVersion(tempDirectory & "\bin\DotNetNuke.dll")
   End With
   ReadResourceFiles(core, "", tempDirectory)
   manifestModules.Add(core)

  Else ' we're processing a package with a manifest

   Dim dnnManifestFile As String = dnnFiles(0)

   '// Parse DNN manifest file
   Dim manifest As New XmlDocument
   manifest.Load(dnnManifestFile)
   Dim manifestVersion As Integer = 0

   '// Determine version of manifest file
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
    '// Remark about DNN 5 manifest file: it is assumed that only one <desktopModule> node per <package> node exists.

    '// Create a module for each package
    For Each packageNode As XmlNode In mainNodes
     Dim manifestModule As New ManifestModuleInfo()

     '// Determine the version
     If Not packageNode.SelectSingleNode("@version") Is Nothing Then
      manifestModule.Version = packageNode.SelectSingleNode("@version").InnerText.Trim
      If String.IsNullOrEmpty(manifestModule.Version) Then
       manifestModule.Version = "0"
       '2009-06-26 Janga:  Default version.
      End If
     Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
     End If

     '// Determine the desktop module
     Dim moduleNodes As XmlNodeList
     moduleNodes = packageNode.SelectNodes("components/component/desktopModule")
     If moduleNodes.Count = 0 Then
      Throw New Exception("Could not retrieve desktop module information in DNN Manifest file")
     End If
     If moduleNodes.Count > 1 Then
      Throw New Exception("Multiple desktop modules found in DNN Manifest file")
     End If

     For Each dmNode As XmlNode In moduleNodes
      '// Determine the folder name
      If Not dmNode("foldername") Is Nothing Then
       manifestModule.FolderName = dmNode("foldername").InnerText.Replace("/"c, "\"c)
      Else : Throw New Exception("Could not retrieve folder information in DNN Manifest file")
      End If

      '// Determine the module name
      If Not dmNode("moduleName") Is Nothing Then
       manifestModule.ModuleName = dmNode("moduleName").InnerText.Replace("/"c, "\"c)
      Else : Throw New Exception("Could not retrieve module name in DNN Manifest file")
      End If
     Next

     '// Determine the friendly name
     If Not packageNode("friendlyName") Is Nothing Then
      manifestModule.FriendlyName = packageNode("friendlyName").InnerText
     Else : manifestModule.FriendlyName = manifestModule.ModuleName
     End If

     '// Find the resource files using the manifest xml
     Dim resourceFiles As String = ""
     For Each node As XmlNode In packageNode.SelectNodes("components/component/files/file")
      Dim resFile As String = ""
      Dim resDir As String = ""
      If Not node("name") Is Nothing Then resFile = node("name").InnerText
      If Not node("path") Is Nothing Then resDir = node("path").InnerText

      'TODO Support resource files which are already localized
      If resFile.ToLower.EndsWith("ascx.resx") OrElse resFile.ToLower.EndsWith("aspx.resx") Then
       '// Determine the resource directory and key for the module
       Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
       Dim resKey As String = Path.Combine(Path.Combine(Path.Combine("DesktopModules", manifestModule.FolderName), resDir), resFile)

       manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
      End If
     Next

     '// Find the resource file
     Dim resFileNode As XmlNode = packageNode.SelectSingleNode("components/component[type='ResourceFile']")
     If resFileNode IsNot Nothing Then
      Dim basePath As String = resFileNode.SelectSingleNode("resourceFiles/basePath").InnerText
      Dim resFile As String = resFileNode.SelectSingleNode("resourceFiles/resourceFile/name").InnerText
      ZipHelper.Unzip(tempDirectory & "\" & resFile, tempDirectory & "\ResourceFiles")
      ReadResourceFiles(manifestModule, basePath, tempDirectory & "\ResourceFiles")
     End If

     '// Add the manifest module to the collection
     manifestModules.Add(manifestModule)
    Next

   ElseIf manifestVersion = 3 Then

    '// Create a module for each folder
    For Each folderNode As XmlNode In mainNodes
     Dim manifestModule As New ManifestModuleInfo()

     '// Determine the module name
     If Not folderNode("modulename") Is Nothing Then
      manifestModule.ModuleName = folderNode("modulename").InnerText
     ElseIf Not folderNode("friendlyname") Is Nothing Then
      manifestModule.ModuleName = folderNode("friendlyname").InnerText
     ElseIf Not folderNode("name") Is Nothing Then
      manifestModule.ModuleName = folderNode("name").InnerText
     Else : Throw New Exception("Could not retrieve module name in DNN Manifest file")
     End If

     '// Determine the friendly name
     If Not folderNode("friendlyname") Is Nothing Then
      manifestModule.FriendlyName = folderNode("friendlyname").InnerText
     Else : manifestModule.FriendlyName = manifestModule.ModuleName
     End If

     '// Determine the folder name
     If Not folderNode("foldername") Is Nothing Then
      manifestModule.FolderName = folderNode("foldername").InnerText.Replace("/"c, "\"c)
     Else : Throw New Exception("Could not retrieve folder information in DNN Manifest file")
     End If

     '// Determine the version
     If Not folderNode("version") Is Nothing Then
      manifestModule.Version = folderNode("version").InnerText.Trim
      If String.IsNullOrEmpty(manifestModule.Version) Then
       manifestModule.Version = "0"
       '2009-06-26 Janga:  Default version.
      End If
     Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
     End If

     '// Find the resource files using the manifest xml
     Dim resourceFiles As String = ""
     For Each node As XmlNode In folderNode.SelectNodes("files/file")
      Dim resFile As String = ""
      Dim resDir As String = ""
      If Not node("name") Is Nothing Then resFile = node("name").InnerText
      If Not node("path") Is Nothing Then resDir = node("path").InnerText

      'TODO Support resource files which are already localized
      If resFile.ToLower.EndsWith("ascx.resx") OrElse resFile.ToLower.EndsWith("aspx.resx") Then
       '// Determine the resource directory and key for the module
       Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
       Dim resKey As String = Path.Combine(Path.Combine(Path.Combine("DesktopModules", manifestModule.FolderName), resDir), resFile)

       manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
      End If
     Next

     '// Find the resource file
     Dim resFileNode As XmlNode = folderNode.SelectSingleNode("resourcefile")
     If Not folderNode("resourcefile") Is Nothing Then
      Dim basePath As String = "DesktopModules\" & manifestModule.FolderName
      Dim resFile As String = folderNode("resourcefile").InnerText
      ZipHelper.Unzip(tempDirectory & "\" & resFile, tempDirectory & "\ResourceFiles")
      ReadResourceFiles(manifestModule, basePath, tempDirectory & "\ResourceFiles")
     End If

     '// Add the manifest module to the collection
     manifestModules.Add(manifestModule)
    Next
   Else : Throw New Exception("Could not determine version of manifest file")
   End If

  End If ' whether core or package

  'TODO Use transactions
  '// DNN Manifest file or core parsed succesfully, now process each module from the manifest
  For Each manifestModule As ManifestModuleInfo In manifestModules

   '// Check if the module is already imported
   Dim objObjectInfo As ObjectInfo = ObjectController.GetObjectByObjectName(manifestModule.ModuleName)
   If objObjectInfo Is Nothing Then
    '// Create a new translate module
    objObjectInfo = New ObjectInfo(0, manifestModule.ModuleName, manifestModule.FriendlyName)
    objObjectInfo.ObjectId = ObjectController.AddObject(objObjectInfo)
   End If

   '// Import or update resource files for this module
   LocalizationController.ProcessResourceFiles(manifestModule.ResourceFiles, tempDirectory, objObjectInfo, manifestModule.Version)
  Next

  '// Try to clean up
  Try
   IO.Directory.Delete(tempDirectory, True)
  Catch
  End Try

  'TODO
  ' Read module package manifest and add TranslationModule object
  ' Find the resource files in the package and add them to the DB....only add module package if it contains resource files?

  'Dim tm As New ObjectInfo(0, "Testje", "Lalala")
  'ObjectController.AddObject(tm)
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

  For Each d As String In Directory.GetDirectories(path)
   ReadResourceFiles(manifestModule, keyBasePath & Mid(d, d.LastIndexOf("\") + 2), d)
  Next

 End Sub

 Private Shared Function GetAssemblyVersion(ByVal path As String) As String

  Try
   Dim v As String = System.Diagnostics.FileVersionInfo.GetVersionInfo(path).FileVersion()
   Dim m As Match = Regex.Match(v, "(\d+)\.(\d+)\.(\d+)\.?(\d*)")
   If m.Success Then
    v = CInt(m.Groups(1).Value).ToString("00")
    v &= "." & CInt(m.Groups(2).Value).ToString("00")
    v &= "." & CInt(m.Groups(3).Value).ToString("00")
   End If
   Return v
  Catch ex As Exception
   Return "0"
  End Try

 End Function

End Class

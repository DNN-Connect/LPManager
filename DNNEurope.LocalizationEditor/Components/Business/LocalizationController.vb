' 
' Copyright (c) 2004-2009 DNN-Europe, http://www.dnn-europe.net
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
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Services.Localization
Imports System.IO
Imports DotNetNuke.Entities.Modules
Imports System.Collections.Generic
Imports System.Globalization
Imports ICSharpCode.SharpZipLib.Zip

Namespace Business
 Public Class LocalizationController

#Region " Object Reading "
  Public Shared Sub ProcessResourceFiles(ByVal resourceFileList As SortedList, ByVal rootPath As String, ByVal objObjectInfo As ObjectInfo, ByVal version As String)
   Dim pattern As String = ".resx"

   ' Add new stuff
   Dim currentVersionKeys As New List(Of String)
   For Each file As DictionaryEntry In resourceFileList
    Dim fileKey As String = file.Key.ToString.Replace(rootPath, "").Replace(pattern, ".resx")
    'Dim dsDef As New DataSet
    'Dim dtDef As DataTable = Nothing
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
     'dsDef.ReadXml(fi.FullName)
     'dtDef = dsDef.Tables("data")
    Catch
     Try
      Dim fc As String = Globals.ReadFile(fi.FullName)
      resFile.LoadXml(fc)
     Catch ex As Exception
      Globals.SimpleLog("Original resource file '" & file.Key.ToString & "' is incorrect or could not be read.")
     End Try
    End Try

    'If Not (dtDef Is Nothing) Then
    'Try
    ' dtDef.PrimaryKey = New DataColumn() {dtDef.Columns("name")}
    'Catch ex As Exception
    ' If Not Directory.Exists(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors") Then
    '  Directory.CreateDirectory(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors")
    ' End If
    ' dtDef.WriteXml(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors\LastError.xml")
    ' Globals.SimpleLog("Original resource file '" & file.Key.ToString & "' has doubles in keys.")
    'End Try

    'For Each dr As DataRow In dtDef.Rows
    For Each x As XmlNode In resFile.DocumentElement.SelectNodes("/root/data")
     'Dim key As String = CStr(dr.Item("name"))
     'Dim value As String = CStr(dr.Item("value"))
     Dim key As String = x.Attributes("name").InnerText
     Dim value As String = x.SelectSingleNode("value").InnerXml
     currentVersionKeys.Add(fileKey.ToLower & ";" & key.ToLower)
     Dim ti As TextInfo = TextsController.GetLatestText(objObjectInfo.ObjectId, fileKey, "", key)

     Try
      If ti Is Nothing Then
       ti = New TextInfo(-1, "", fileKey, objObjectInfo.ObjectId, value, key, version)
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
        ti = New TextInfo(-1, "", fileKey, objObjectInfo.ObjectId, value, key, version)
        TextsController.AddText(ti)
       End If
      End If
     Catch ex As Exception
      Globals.SimpleLog("Error importing resources: " & ex.Message, ex.StackTrace, "Filekey: " & fileKey, "Key    : " & key, "Value  : " & value, "")
     End Try
    Next
    'End If
   Next

   ' Deprecate old stuff
   For Each ti As TextInfo In TextsController.GetTextsByObject(objObjectInfo.ModuleId, objObjectInfo.ObjectId, "", version).Values
    If ti.Version <> version Then ' it's an old one
     If Not currentVersionKeys.Contains(ti.FilePath.ToLower & ";" & ti.TextKey.ToLower) Then
      ti.DeprecatedIn = version
      TextsController.UpdateText(ti)
     End If
    End If
   Next
  End Sub
#End Region

#Region " Other "
  Public Shared Function GetObjectMetrics(ByVal ObjectId As Integer, ByVal Locale As String) As ObjectMetrics
   Dim res As New ObjectMetrics
   With res
    .CurrentVersion = TextsController.CurrentVersion(ObjectId, Locale)
    .NrOfFiles = TextsController.NrOfFiles(ObjectId, .CurrentVersion)
    .NrOfItems = TextsController.NrOfItems(ObjectId, .CurrentVersion)
    .NrOfChangedTexts = TextsController.NrOfChangedTexts(ObjectId, .CurrentVersion)
    If Locale = "" Then
     .NrOfMissingTranslations = -1
    Else
     .NrOfMissingTranslations = TextsController.NrOfMissingTranslations(ObjectId, Locale, .CurrentVersion)
    End If
    If .NrOfItems > 0 Then .PercentageComplete = CInt(Math.Round(((.NrOfItems - .NrOfMissingTranslations) * 100.0) / .NrOfItems))
   End With
   Return res
  End Function

  Public Structure ObjectMetrics
   Public NrOfFiles As Integer
   Public NrOfItems As Integer
   Public CurrentVersion As String
   Public NrOfChangedTexts As Integer
   Public NrOfMissingTranslations As Integer
   Public PercentageComplete As Integer
  End Structure

  Public Shared Function GetFrameworkVersion() As String
   Dim res As String = "01.00.00"
   Using ir As IDataReader = DataProvider.Instance.GetFrameworkVersion
    Do While ir.Read
     res = String.Format("{0:00}.{1:00}.{2:00}", ir.Item("Major"), ir.Item("Minor"), ir.Item("Build"))
     Exit Do
    Loop
   End Using
   Return res
  End Function

  Public Shared Function GetCorrectPath(ByVal manifestPath As String, ByVal localResourceDirectory As String) As String
   Dim m As Match = Regex.Match(manifestPath, "(?i)GlobalResource\\(.*)\.\w\w-\w\w.resx(?-i)")
   If m.Success Then
    ' Global Resource
    Return Localization.ApplicationResourceDirectory.Replace("~/", "") & "\" & m.Groups(1).Value & ".resx"
   Else
    ' Local Resource
    m = Regex.Match(manifestPath, "(?i)([^\\]+)\\(.*)\.\w\w-\w\w.resx(?-i)")
    If m.Success Then
     Dim sRes As String = ""
     Select Case m.Groups(1).Value.ToLower
      Case "localresource"
       sRes = "DesktopModules\"
      Case "controlresource"
       sRes = "controls\"
      Case "adminresource"
       sRes = "admin\"
      Case "installresource"
       sRes = "Install\"
      Case "providerresource"
       sRes = "Providers\"
      Case Else
       Return manifestPath
     End Select
     Return sRes & Regex.Replace(m.Groups(2).Value, "([^\\]+)$", localResourceDirectory & "\$1.resx")
    End If
    Return manifestPath
   End If
  End Function

  Private Shared Function GetObjectBasePath(ByVal objObject As ObjectInfo) As String
   If objObject.InstallPath = "" Then
    Return ""
   End If
   Select Case objObject.PackageType.ToLower
    Case "Provider"
     Return "Providers\" & objObject.InstallPath.Replace("/", "\")
    Case Else
     Return "DesktopModules\" & objObject.InstallPath.Replace("/", "\")
   End Select
  End Function

  Public Shared Function GetLastEditTime(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As DateTime
   Dim res As DateTime = Now
   Using ir As IDataReader = Data.DataProvider.Instance().GetLastEditTime(ObjectId, Locale, Version)
    If ir.Read Then
     res = CDate(ir.Item(0))
    End If
   End Using
   Return res
  End Function

  Public Shared Function CleanName(ByVal name As String) As String
   Return name.Replace("\", "_").Replace("/", "_")
  End Function
#End Region

#Region " Pack Creation "
  Public Enum PackType
   V3
   V5
   Hybrid
  End Enum

  Public Shared Function CreateResourcePack(ByVal objObject As ObjectInfo, ByVal Version As String, ByVal Locale As String, isFullPack As Boolean) As String

   Dim CompressionLevel As Integer = 9
   Dim pattern As String = ".resx"
   If Locale <> "" Then
    pattern = "." & Locale & pattern
   End If
   Dim fileName As String = ""

   Dim Type As PackType = PackType.Hybrid
   If objObject.IsCore Then
    If Version > "04.99.99" Then
     Type = PackType.V5
    Else
     Type = PackType.V3
    End If
   End If

   Dim ObjectsToPack As New List(Of ObjectInfo)
   ObjectsToPack.Add(objObject)
   ' If it's a Core pack we include default objects
   If objObject.IsCore Then
    For Each o As ObjectInfo In ObjectCoreVersionsController.GetCoreObjects(Version, isFullPack)
     ObjectsToPack.Add(o)
    Next
   End If

   Dim strmZipFile As FileStream = Nothing

   Try

    Dim packName As String = objObject.ObjectName
    fileName = "ResourcePack." & CleanName(packName)
    If objObject.IsCore Then
     If isFullPack Then
      fileName &= ".Full"
     End If
    End If
    fileName &= "." & Version & "." & Locale & ".zip"

    Dim packPath As String = DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\LocalizationEditor\Cache\" & objObject.ModuleId.ToString & "\"
    If Not Directory.Exists(packPath) Then
     Directory.CreateDirectory(packPath)
    End If

    ' check for caching
    If objObject.Module.CachePacks AndAlso IO.File.Exists(packPath & fileName) Then
     Dim f As New FileInfo(packPath & fileName)
     Dim lastPackWriteTime As DateTime = f.LastWriteTime
     Dim isCached As Boolean = True
     For Each o As ObjectInfo In ObjectsToPack
      Dim lastEditTime As DateTime = GetLastEditTime(o.ObjectId, Locale, Version)
      If lastEditTime > lastPackWriteTime Then
       isCached = False
      End If
     Next
     If isCached Then
      Return fileName
     End If
    End If

    strmZipFile = File.Create(packPath & fileName)
    Dim strmZipStream As ZipOutputStream = Nothing
    Try
     strmZipStream = New ZipOutputStream(strmZipFile)

     Dim myZipEntry As ZipEntry

     If Type = PackType.V5 Or Type = PackType.Hybrid Then
      ' Add DNN 5+ content
      Dim loc As New CultureInfo(Locale)
      Dim manifestName As String = packName & "_" & loc.Name & ".dnn"
      manifestName = manifestName.Replace("/", "_").Replace("\", "_")
      myZipEntry = New ZipEntry(manifestName)
      strmZipStream.PutNextEntry(myZipEntry)
      strmZipStream.SetLevel(CompressionLevel)
      Dim manifestV5 As XmlDocument = GetLanguagePackManifestV5(ObjectsToPack, Version, Locale)
      Dim quirkPack As Boolean = CBool(ObjectsToPack(0).IsCore And Version < "06.00.00")
      Dim FileData As Byte() = Encoding.UTF8.GetBytes(manifestV5.OuterXml)
      strmZipStream.Write(FileData, 0, FileData.Length)

      For Each o As ObjectInfo In ObjectsToPack
       Dim basePath As String = ""
       If Not quirkPack Then basePath = GetObjectBasePath(o)
       For Each filePath As String In TextsController.GetFileList(o.ObjectId, Version)
        Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
        resFileName = resFileName.Replace(".resx", pattern)
        Dim targetPath As String = GetResourceZipPathV5(filePath, basePath)
        Dim texts As IDictionary(Of Integer, TextInfo) = TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, Locale, Version, False)
        If texts.Count > 0 Then ' do not write an empty file
         Dim resDoc As New XmlDocument
         resDoc.Load(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\DNNEurope\LocalizationEditor\App_LocalResources\Template.resx")
         Dim root As XmlNode = resDoc.DocumentElement
         For Each ti As TextInfo In texts.Values
          Globals.AddResourceText(root, ti.TextKey, ti.TextValue)
         Next
         myZipEntry = New ZipEntry(targetPath & "\" & resFileName)
         strmZipStream.PutNextEntry(myZipEntry)
         Using w As New MemoryStream
          Using xw As New XmlTextWriter(w, Encoding.UTF8)
           xw.Formatting = Formatting.Indented
           resDoc.WriteContentTo(xw)
          End Using
          FileData = w.ToArray()
         End Using
         strmZipStream.Write(FileData, 0, FileData.Length)
        End If
       Next
      Next
     End If

     If Type = PackType.V3 Or Type = PackType.Hybrid Then
      ' Add DNN 3/4 content
      myZipEntry = New ZipEntry("Manifest.xml")
      strmZipStream.PutNextEntry(myZipEntry)
      strmZipStream.SetLevel(CompressionLevel)
      Dim manifestV3 As XmlDocument = GetLanguagePackManifestV3(ObjectsToPack, Version, Locale)
      Dim FileData As Byte() = Encoding.UTF8.GetBytes(manifestV3.OuterXml)
      strmZipStream.Write(FileData, 0, FileData.Length)

      For Each o As ObjectInfo In ObjectsToPack
       For Each filePath As String In TextsController.GetFileList(o.ObjectId, Version)
        Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
        resFileName = resFileName.Replace(".resx", pattern)
        Dim targetPath As String = GetResourceZipPathV3(filePath)
        Dim texts As IDictionary(Of Integer, TextInfo) = TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, Locale, Version, False)
        If texts.Count > 0 Then ' do not write an empty file
         Dim resDoc As New XmlDocument
         resDoc.Load(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\DNNEurope\LocalizationEditor\App_LocalResources\Template.resx")
         Dim root As XmlNode = resDoc.DocumentElement
         For Each ti As TextInfo In texts.Values
          Globals.AddResourceText(root, ti.TextKey, ti.TextValue)
         Next
         myZipEntry = New ZipEntry(targetPath & "\" & resFileName)
         strmZipStream.PutNextEntry(myZipEntry)
         Using w As New MemoryStream
          Using xw As New XmlTextWriter(w, Encoding.UTF8)
           xw.Formatting = Formatting.Indented
           resDoc.WriteContentTo(xw)
          End Using
          FileData = w.ToArray()
         End Using
         strmZipStream.Write(FileData, 0, FileData.Length)
        End If
       Next
      Next
     End If

    Catch ex As Exception

    Finally
     If Not strmZipStream Is Nothing Then
      strmZipStream.Flush()
      strmZipStream.Finish()
      strmZipStream.Close()
     End If
    End Try
   Catch ex As Exception

   Finally
    If Not strmZipFile Is Nothing Then
     strmZipFile.Close()
    End If
   End Try

   Return fileName

  End Function
#End Region

#Region " Pack Creation V3 "
  Private Shared Function GetLanguagePackManifestV3(ByVal objObjects As List(Of ObjectInfo), ByVal Version As String, ByVal Locale As String) As XmlDocument
   Dim pattern As String = ".resx"
   If Locale <> "" Then
    pattern = "." & Locale & pattern
   End If
   Dim manifest As New XmlDocument
   manifest.AppendChild(manifest.CreateXmlDeclaration("1.0", Nothing, Nothing))
   Dim root As XmlNode = manifest.CreateElement("LanguagePack")
   manifest.AppendChild(root)
   Globals.AddAttribute(root, "Version", "3.0")
   Dim loc As New CultureInfo(Locale)
   Globals.AddElement(root, "Culture", "", "Code=" & Locale, "DisplayName=" & loc.NativeName, "Fallback=en-US")
   Dim files As XmlNode = manifest.CreateElement("Files")
   root.AppendChild(files)
   For Each objObject As ObjectInfo In objObjects
    For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, Version)
     If TextsController.GetTextsByObjectAndFile(-1, objObject.ObjectId, filePath, Locale, Version, False).Count > 0 Then
      AddPackResourcePathToManifestV3(files, filePath, Locale)
     End If
    Next
   Next
   Return manifest
  End Function

  Private Shared Sub AddPackResourcePathToManifestV3(ByRef filesNode As XmlNode, ByVal filePath As String, ByVal locale As String)
   Try
    Dim fileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
    fileName = fileName.Replace(".resx", "." & locale & ".resx")
    Dim fullPath As String = Left(filePath, filePath.LastIndexOf("\"))
    Dim part As String = fullPath.ToLower
    If fullPath.IndexOf("\") > -1 Then
     part = Left(fullPath, fullPath.IndexOf("\")).ToLower
     fullPath = Mid(fullPath, fullPath.IndexOf("\") + 2)
    End If
    If fullPath.ToLower.EndsWith("\app_localresources") Then
     fullPath = Left(fullPath, fullPath.Length - 19)
    End If
    Select Case part
     Case "admin"
      'fullPath = "AdminResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=AdminResource", "ModuleName=" & fullPath)
     Case "controls"
      'fullPath = "ControlResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=ControlResource")
     Case "app_globalresources"
      'fullPath = "GlobalResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=GlobalResource")
     Case "install"
      'fullPath = "InstallResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=InstallResource")
     Case "desktopmodules"
      'fullPath = "LocalResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=LocalResource", "ModuleName=" & fullPath)
     Case "providers"
      'fullPath = "ProviderResource\" & fullPath
      Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=ProviderResource", "ModuleName=" & fullPath)
    End Select

   Catch ex As Exception

   End Try
  End Sub

  Private Shared Function GetResourceZipPathV3(ByVal fullPath As String) As String
   Try
    fullPath = Regex.Replace(fullPath, "(?i)^(desktopmodules|admin|controls|app_globalresources|install|providers)(?-i)", AddressOf ReplaceRootPathV3)
    fullPath = Regex.Replace(fullPath, "(?i)\\([^\\]+)\\([^\\]+)$(?-i)", "")
    fullPath = Regex.Replace(fullPath, "(?i)\\(Global|Shared)Resources.resx(?-i)", "")
   Catch ex As Exception
   End Try
   Return fullPath
  End Function

  Private Shared Function ReplaceRootPathV3(ByVal m As Match) As String
   Select Case m.Groups(1).Value.ToLower
    Case "admin"
     Return "AdminResource"
    Case "controls"
     Return "ControlResource"
    Case "app_globalresources"
     Return "GlobalResource"
    Case "install"
     Return "InstallResource"
    Case "desktopmodules"
     Return "LocalResource"
    Case "providers"
     Return "ProviderResource"
    Case Else
     Return m.Groups(1).Value
   End Select
  End Function
#End Region

#Region " Pack Creation V5 DNN 5 "
  Private Shared Sub AddCoreV5ManifestDNN5(ByRef packagesNode As XmlNode, ByVal objObjects As List(Of ObjectInfo), ByVal Version As String, ByVal loc As CultureInfo)
   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", Globals.glbCoreName & " " & loc.NativeName) ' Our package name. The convention is Objectname + verbose language
   Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Globals.AddAttribute(package, "version", Version)
   Globals.AddElement(package, "friendlyName", Globals.glbCoreFriendlyName & " " & loc.NativeName) ' little to add here to name
   Globals.AddElement(package, "description", String.Format(Localization.GetString("ManifestDescription", Globals.glbSharedResources, loc.Name), loc.NativeName, Globals.glbCoreFriendlyName))
   Dim owner As XmlNode = Globals.AddElement(package, "owner")
   Globals.AddElement(owner, "name", objObjects(0).Module.OwnerName)
   Globals.AddElement(owner, "organization", objObjects(0).Module.OwnerOrganization)
   Globals.AddElement(owner, "url", objObjects(0).Module.OwnerUrl)
   Globals.AddElement(owner, "email", objObjects(0).Module.OwnerEmail)
   Globals.AddElement(package, "license", Globals.GetLicense(DotNetNuke.Common.ApplicationMapPath & "\" & objObjects(0).Module.HomeDirectory & "\", objObjects(0).Module.ModuleId))
   Globals.AddElement(package, "releaseNotes", "")
   Dim component As XmlNode = Globals.AddElement(package, "components")
   component = Globals.AddElement(component, "component")
   Globals.AddAttribute(component, "type", "CoreLanguage")
   Dim files As XmlNode = Globals.AddElement(component, "languageFiles")
   Globals.AddElement(files, "code", loc.Name)
   Globals.AddElement(files, "displayName", loc.NativeName)
   Dim basePath As String = GetObjectBasePath(objObjects(0))
   Globals.AddElement(files, "basePath", basePath) ' basepath needs to be added to object
   For Each o As ObjectInfo In objObjects
    For Each filePath As String In TextsController.GetFileList(o.ObjectId, o.Version)
     If TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, loc.Name, Version, False).Count > 0 Then
      AddPackResourcePathToManifestV5(files, filePath, loc.Name, basePath)
     End If
    Next
   Next
  End Sub
#End Region

#Region " Pack Creation V5 DNN 6+ "
  Private Shared Function GetLanguagePackManifestV5(ByVal objObjects As List(Of ObjectInfo), ByVal Version As String, ByVal Locale As String) As XmlDocument
   Dim pattern As String = ".resx"
   If Locale <> "" Then
    pattern = "." & Locale & pattern
   End If
   Dim loc As New CultureInfo(Locale)
   Dim manifest As New XmlDocument
   manifest.AppendChild(manifest.CreateXmlDeclaration("1.0", Nothing, Nothing))
   Dim root As XmlNode = manifest.CreateElement("dotnetnuke")
   manifest.AppendChild(root)
   Globals.AddAttribute(root, "type", "Package")
   Globals.AddAttribute(root, "version", "5.0")
   Dim package As XmlNode = Globals.AddElement(root, "packages")
   If objObjects(0).IsCore And Version < "06.00.00" Then
    ' Special provision as DNN 5 installer fails if a dependant package is not installed
    AddCoreV5ManifestDNN5(package, objObjects, Version, loc)
   Else
    For Each objObject As ObjectInfo In objObjects
     AddObjectToV5Manifest(package, objObject, Version, loc)
    Next
   End If
   Return manifest
  End Function

  Private Shared Sub AddObjectToV5Manifest(ByRef packagesNode As XmlNode, ByVal objObject As ObjectInfo, ByVal Version As String, ByVal loc As CultureInfo)

   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", objObject.ObjectName & " " & loc.NativeName) ' Our package name. The convention is Objectname + verbose language
   If objObject.IsCore Then
    Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Else
    Globals.AddAttribute(package, "type", "ExtensionLanguagePack")
   End If
   Globals.AddAttribute(package, "version", Version)
   Globals.AddElement(package, "friendlyName", objObject.ObjectName & " " & loc.NativeName) ' little to add here to name
   Globals.AddElement(package, "description", String.Format(Localization.GetString("ManifestDescription", Globals.glbSharedResources, loc.Name), loc.NativeName, objObject.ObjectName))
   Dim owner As XmlNode = Globals.AddElement(package, "owner")
   Globals.AddElement(owner, "name", objObject.Module.OwnerName)
   Globals.AddElement(owner, "organization", objObject.Module.OwnerOrganization)
   Globals.AddElement(owner, "url", objObject.Module.OwnerUrl)
   Globals.AddElement(owner, "email", objObject.Module.OwnerEmail)
   Globals.AddElement(package, "license", Globals.GetLicense(DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\", objObject.Module.ModuleId))
   Globals.AddElement(package, "releaseNotes", "")
   Dim component As XmlNode = Globals.AddElement(package, "components")
   component = Globals.AddElement(component, "component")
   If objObject.IsCore Then
    Globals.AddAttribute(component, "type", "CoreLanguage")
   Else
    Globals.AddAttribute(component, "type", "ExtensionLanguage")
   End If
   Dim files As XmlNode = Globals.AddElement(component, "languageFiles")
   Globals.AddElement(files, "code", loc.Name)
   Globals.AddElement(files, "displayName", loc.NativeName)
   If Not objObject.IsCore Then
    Dim dnnPackage As New DotNetNuke.Services.Installer.Packages.PackageInfo
    With dnnPackage
     .PackageType = objObject.PackageType
     .Name = objObject.ObjectName
     .FriendlyName = objObject.FriendlyName
    End With
    DotNetNuke.Services.Installer.LegacyUtil.ParsePackageName(dnnPackage)
    Globals.AddElement(files, "package", dnnPackage.Name) ' this creates the dependency between lang pack and object (ignored for Core)
   End If
   Dim basePath As String = GetObjectBasePath(objObject)
   Globals.AddElement(files, "basePath", basePath) ' basepath needs to be added to object
   For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, Version)
    If TextsController.GetTextsByObjectAndFile(objObject.ModuleId, objObject.ObjectId, filePath, loc.Name, Version, False).Count > 0 Then
     AddPackResourcePathToManifestV5(files, filePath, loc.Name, basePath)
    End If
   Next

  End Sub

  Private Shared Sub AddPackResourcePathToManifestV5(ByRef filesNode As XmlNode, ByVal filePath As String, ByVal locale As String, ByVal basePath As String)
   Try
    filePath = Mid(filePath, basePath.Length + 1)
    If filePath.StartsWith("\") Then
     filePath = Mid(filePath, 2)
    End If
    Dim fileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
    fileName = fileName.Replace(".resx", "." & locale & ".resx")
    filePath = Left(filePath, filePath.LastIndexOf("\"))
    Dim file As XmlNode = Globals.AddElement(filesNode, "languageFile")
    Globals.AddElement(file, "path", filePath.ToLower)
    Globals.AddElement(file, "name", fileName)
   Catch ex As Exception
   End Try
  End Sub

  Private Shared Function GetResourceZipPathV5(ByVal fullPath As String, ByVal basePath As String) As String
   fullPath = Mid(fullPath, basePath.Length + 1)
   If fullPath.StartsWith("\") Then
    fullPath = Mid(fullPath, 2)
   End If
   fullPath = Left(fullPath, fullPath.LastIndexOf("\"))
   Return fullPath
  End Function

#End Region

 End Class
End Namespace

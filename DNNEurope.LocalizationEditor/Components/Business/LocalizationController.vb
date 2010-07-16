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
  ''' <summary>
  ''' Retrieves the latest resource for installed modules and updates the values in the localization editor data model
  ''' </summary>
  ''' <param name="InstallationMapPath"></param>
  ''' <param name="PortalId"></param>
  ''' <param name="objObjectInfo"></param>
  ''' <param name="UserId"></param>
  ''' <remarks></remarks>
  Public Shared Sub ReadResourceFiles(ByVal InstallationMapPath As String, ByVal PortalId As Integer, ByVal objObjectInfo As ObjectInfo, ByVal UserId As Integer)

   Dim resourceFileList As New SortedList
   Dim version As String = "01.00.00"
   Dim pattern As String = ".resx"
   If objObjectInfo.ObjectName = Globals.glbCoreName Then
    Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "admin", "")
    Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "controls", "")
    Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "providers", "")
    Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "install", "")
    resourceFileList.Add(Localization.GlobalResourceFile.Replace("~/", InstallationMapPath).Replace(".resx", pattern), New FileInfo(Localization.GlobalResourceFile.Replace("~/", InstallationMapPath).Replace(".resx", pattern)))
    resourceFileList.Add(Localization.SharedResourceFile.Replace("~/", InstallationMapPath).Replace(".resx", pattern), New FileInfo(Localization.SharedResourceFile.Replace("~/", InstallationMapPath).Replace(".resx", pattern)))
    version = GetFrameworkVersion()
   Else
    Dim _module As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(objObjectInfo.ObjectName, PortalId)
    If _module Is Nothing Then
     '// If the module is not found, then it's not installed in DotNetNuke. Skip the processing
     Return
    End If
    version = _module.Version.Trim
    If String.IsNullOrEmpty(version) Then
     version = "0"
     '2009-06-26 Janga:  Default version.
    End If
    Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "DesktopModules\" & _module.FolderName, "")
    ' remove the other files if part of another module that is nested
    For Each m As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(PortalId).Values
     If (Not m.ModuleName = objObjectInfo.ObjectName) AndAlso (m.FolderName <> _module.FolderName) AndAlso (m.FolderName.StartsWith(_module.FolderName)) Then
      Globals.RemoveResourceFiles(resourceFileList, InstallationMapPath & "DesktopModules\" & m.FolderName, "")
     End If
    Next
   End If

   ProcessResourceFiles(resourceFileList, InstallationMapPath, objObjectInfo, version)
  End Sub

  Public Shared Sub ProcessResourceFiles(ByVal resourceFileList As SortedList, ByVal rootPath As String, ByVal objObjectInfo As ObjectInfo, ByVal version As String)
   Dim pattern As String = ".resx"

   ' Add new stuff
   Dim currentVersionKeys As New List(Of String)
   For Each file As DictionaryEntry In resourceFileList
    Dim fileKey As String = file.Key.ToString.Replace(rootPath, "").Replace(pattern, ".resx")
    Dim dsDef As New DataSet
    Dim dtDef As DataTable = Nothing

    Try
     Dim fi As FileInfo = CType(file.Value, FileInfo)
     dsDef.ReadXml(fi.FullName)
     dtDef = dsDef.Tables("data")
    Catch
     Globals.SimpleLog("Original resource file '" & file.Key.ToString & "' is incorrect or could not be read.")
    End Try

    If Not (dtDef Is Nothing) Then
     Try
      dtDef.PrimaryKey = New DataColumn() {dtDef.Columns("name")}
     Catch ex As Exception
      If Not Directory.Exists(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors") Then
       Directory.CreateDirectory(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors")
      End If
      dtDef.WriteXml(DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Errors\LastError.xml")
      Globals.SimpleLog("Original resource file '" & file.Key.ToString & "' has doubles in keys.")
     End Try

     For Each dr As DataRow In dtDef.Rows
      Dim key As String = CStr(dr.Item("name"))
      Dim value As String = CStr(dr.Item("value"))
      currentVersionKeys.Add(fileKey & ";" & key)
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
    End If
   Next

   ' Deprecate old stuff
   For Each ti As TextInfo In TextsController.GetTextsByObject(objObjectInfo.ModuleId, objObjectInfo.ObjectId, "", version).Values
    If ti.Version <> version Then ' it's an old one
     If Not currentVersionKeys.Contains(ti.FilePath & ";" & ti.TextKey) Then
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
   Return "DesktopModules\" & objObject.InstallPath.Replace("/", "\")
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

  Public Shared Function CreateResourcePack(ByVal objObject As ObjectInfo, ByVal Version As String, ByVal Locale As String, ByVal type As PackType) As String

   Dim CompressionLevel As Integer = 9
   Dim pattern As String = ".resx"
   If Locale <> "" Then
    pattern = "." & Locale & pattern
   End If
   Dim fileName As String = ""

   Dim strmZipFile As FileStream = Nothing

   Try

    fileName = "ResourcePack." & CleanName(objObject.ObjectName) & "." & Version & "." & Locale & ".zip"

    Dim packPath As String = DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\LocalizationEditor\Cache\" & objObject.ModuleId.ToString & "\"
    If Not Directory.Exists(packPath) Then
     Directory.CreateDirectory(packPath)
    End If

    ' check for caching
    If objObject.Module.CachePacks AndAlso IO.File.Exists(packPath & fileName) Then
     Dim f As New FileInfo(packPath & fileName)
     Dim lastPackWriteTime As DateTime = f.LastWriteTime
     Dim lastEditTime As DateTime = GetLastEditTime(objObject.ObjectId, Locale, Version)
     If lastEditTime <= lastPackWriteTime Then
      Return fileName
     End If
    End If

    strmZipFile = File.Create(packPath & fileName)
    Dim strmZipStream As ZipOutputStream = Nothing
    Try
     strmZipStream = New ZipOutputStream(strmZipFile)

     Dim myZipEntry As ZipEntry

     If type = PackType.V5 Or type = PackType.Hybrid Then
      ' Add DNN 5+ content
      Dim loc As New CultureInfo(Locale)
      Dim manifestName As String = objObject.ObjectName & " " & loc.NativeName & ".dnn"
      manifestName = manifestName.Replace("/", "_").Replace("\", "_")
      myZipEntry = New ZipEntry(manifestName)
      strmZipStream.PutNextEntry(myZipEntry)
      strmZipStream.SetLevel(CompressionLevel)
      Dim manifestV5 As XmlDocument = GetLanguagePackManifestV5(objObject, Version, Locale)
      Dim FileData As Byte() = Encoding.UTF8.GetBytes(manifestV5.OuterXml)
      strmZipStream.Write(FileData, 0, FileData.Length)

      For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, Version)
       Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
       resFileName = resFileName.Replace(".resx", pattern)
       Dim targetPath As String = GetResourceZipPathV5(filePath, GetObjectBasePath(objObject))
       Dim texts As IDictionary(Of Integer, TextInfo) = TextsController.GetTextsByObjectAndFile(objObject.ModuleId, objObject.ObjectId, filePath, Locale, Version, False)
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
     End If

     If type = PackType.V3 Or type = PackType.Hybrid Then
      ' Add DNN 3/4 content
      myZipEntry = New ZipEntry("Manifest.xml")
      strmZipStream.PutNextEntry(myZipEntry)
      strmZipStream.SetLevel(CompressionLevel)
      Dim manifestV3 As XmlDocument = GetLanguagePackManifestV3(objObject.ObjectId, Version, Locale)
      Dim FileData As Byte() = Encoding.UTF8.GetBytes(manifestV3.OuterXml)
      strmZipStream.Write(FileData, 0, FileData.Length)

      For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, Version)
       Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
       resFileName = resFileName.Replace(".resx", pattern)
       Dim targetPath As String = GetResourceZipPathV3(filePath)
       Dim texts As IDictionary(Of Integer, TextInfo) = TextsController.GetTextsByObjectAndFile(objObject.ModuleId, objObject.ObjectId, filePath, Locale, Version, False)
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
  Private Shared Function GetLanguagePackManifestV3(ByVal ObjectId As Integer, ByVal Version As String, ByVal Locale As String) As XmlDocument
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
   For Each filePath As String In TextsController.GetFileList(ObjectId, Version)
    If TextsController.GetTextsByObjectAndFile(-1, ObjectId, filePath, Locale, Version, False).Count > 0 Then
     AddPackResourcePathToManifestV3(files, filePath, Locale)
    End If
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

#Region " Pack Creation V5 "
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
   For Each objObject As ObjectInfo In objObjects
    AddObjectToV5Manifest(package, objObject, Version, loc)
   Next
   Return manifest
  End Function

  Private Shared Function GetLanguagePackManifestV5(ByVal objObject As ObjectInfo, ByVal Version As String, ByVal Locale As String) As XmlDocument
   Dim objs As New List(Of ObjectInfo)
   If objObject.ObjectName = Globals.glbCoreName Then
    For Each obj As ObjectInfo In ObjectController.GetObjectList(objObject.ModuleId)
     If obj.IsCoreObject Then
      objs.Add(obj)
     End If
    Next
   Else
    objs.Add(objObject)
   End If
   Return GetLanguagePackManifestV5(objs, Version, Locale)
  End Function

  Private Shared Sub AddObjectToV5Manifest(ByRef packagesNode As XmlNode, ByVal objObject As ObjectInfo, ByVal Version As String, ByVal loc As CultureInfo)

   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", objObject.ObjectName & " " & Loc.NativeName) ' Our package name. The convention is Objectname + verbose language
   If objObject.ObjectName = Globals.glbCoreName Then
    Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Else
    Globals.AddAttribute(package, "type", "ExtensionLanguagePack")
   End If
   Globals.AddAttribute(package, "version", Version)
   Globals.AddElement(package, "friendlyName", objObject.ObjectName & " " & Loc.NativeName) ' little to add here to name
   Globals.AddElement(package, "description", "") ' and even less - leave empty
   Dim owner As XmlNode = Globals.AddElement(package, "owner")
   Globals.AddElement(owner, "name", objObject.Module.OwnerName)
   Globals.AddElement(owner, "organization", objObject.Module.OwnerOrganization)
   Globals.AddElement(owner, "url", objObject.Module.OwnerUrl)
   Globals.AddElement(owner, "email", objObject.Module.OwnerEmail)
   Globals.AddElement(package, "license", Globals.GetLicense(DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\", objObject.Module.ModuleId))
   Globals.AddElement(package, "releaseNotes", "")
   Dim component As XmlNode = Globals.AddElement(package, "components")
   component = Globals.AddElement(component, "component")
   If objObject.ObjectName = Globals.glbCoreName Then
    Globals.AddAttribute(component, "type", "CoreLanguage")
   Else
    Globals.AddAttribute(component, "type", "ExtensionLanguage")
   End If
   Dim files As XmlNode = Globals.AddElement(component, "languageFiles")
   Globals.AddElement(files, "code", loc.Name)
   Globals.AddElement(files, "displayName", Loc.NativeName)
   If objObject.ObjectName <> Globals.glbCoreName Then
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

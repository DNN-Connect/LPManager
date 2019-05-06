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
Imports System.Globalization
Imports ICSharpCode.SharpZipLib.Zip
Imports DNNEurope.Modules.LocalizationEditor.Entities.Texts
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Entities.Packages
Imports System.IO.Compression

Namespace Services.Packaging
 Public Class PackageWriter

#Region " Pack Creation "
  Public Shared Function CreateResourcePack(ByVal objObject As ObjectInfo, ByVal version As String, ByVal locale As String, isFullPack As Boolean) As String

   Const compressionLevel As Integer = 9
   Dim pattern As String = ".resx"
   If locale <> "" Then
    pattern = "." & locale & pattern
   End If
   Dim fileName As String = ""

   Dim objectsToPack As List(Of ObjectInfo) = ObjectsController.GetObjectPackList(objObject.ObjectId, version)

   Dim strmZipFile As FileStream = Nothing

   Try

    Dim packName As String = objObject.ObjectName
    fileName = "ResourcePack." & CleanName(packName)
    fileName &= "." & version & "." & locale & ".zip"

    Dim packPath As String = DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\LocalizationEditor\Cache\" & objObject.ModuleId.ToString & "\"
    If Not Directory.Exists(packPath) Then
     Directory.CreateDirectory(packPath)
    End If

    ' check for caching
    If objObject.Module.CachePacks AndAlso IO.File.Exists(packPath & fileName) Then
     Dim f As New FileInfo(packPath & fileName)
     Dim lastPackWriteTime As DateTime = f.LastWriteTime
     Dim isCached As Boolean = True
     For Each o As ObjectInfo In objectsToPack
      Dim lastEditTime As DateTime = GetLastEditTime(o.ObjectId, locale, version)
      If lastEditTime > lastPackWriteTime Then
       isCached = False
      End If
     Next
     If isCached Then
      Return fileName
     End If
    End If

    strmZipFile = File.Create(packPath & fileName)
    Dim strmZipStream As ZipArchive = Nothing

    Try

     strmZipStream = New ZipArchive(strmZipFile, ZipArchiveMode.Create)
     Dim myZipEntry As ZipArchiveEntry
     Dim loc As New CultureInfo(locale)
     Dim fileData As Byte()
     Dim packedObjects As New List(Of ObjectInfo)
     Dim packedFiles As New List(Of String)

     For Each o As ObjectInfo In objectsToPack
      Dim hasTexts As Boolean = False
      For Each filePath As String In TextsController.GetFileList(o.ObjectId, version)
       Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
       resFileName = resFileName.Replace(".resx", pattern)
       Dim targetPath As String = GetResourceZipPathV5(filePath, "")
       Dim onlyNonEmptyKeys As Boolean = False
       If Regex.Match(filePath, "\.template\.resx").Success Then
        onlyNonEmptyKeys = True
       End If
       Dim texts As Dictionary(Of String, Entities.Texts.TextInfo) = TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, locale, version, onlyNonEmptyKeys)
       If texts.Count > 0 AndAlso Not packedFiles.Contains(filePath) Then ' do not write an empty file or overwrite a previous file
        packedFiles.Add(filePath)
        Dim resDoc As New XmlDocument
        resDoc.Load(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\DNNEurope\LocalizationEditor\App_LocalResources\Template.resx")
        Dim root As XmlNode = resDoc.DocumentElement
        For Each ti As Entities.Texts.TextInfo In texts.Values.OrderBy(Function(t) t.TextKey)
         Try
          Globals.AddResourceText(root, ti.TextKey, ti.TextValue)
         Catch ex As Exception
          ' ignore errors
         End Try
        Next
        myZipEntry = strmZipStream.CreateEntry(targetPath & "\" & resFileName)
        fileData = Globals.XmlToFormattedByteArray(resDoc)
        Using zipStream As Stream = myZipEntry.Open()
         Try
          zipStream.Write(fileData, 0, fileData.Length)
         Catch ex As Exception
         End Try
        End Using
        hasTexts = True
       End If
      Next
      If hasTexts Then
       If o.IsCore Then
        packedObjects.Insert(0, o)
       Else
        packedObjects.Add(o)
       End If
      End If
     Next

     Dim manifestName As String = packName & "_" & loc.Name & ".dnn"
     manifestName = manifestName.Replace("/", "_").Replace("\", "_")
     myZipEntry = strmZipStream.CreateEntry(manifestName)
     Dim manifestV5 As XmlDocument = GetLanguagePackManifestV5(packedObjects, locale)
     fileData = Globals.XmlToFormattedByteArray(manifestV5)
     Using zipStream As Stream = myZipEntry.Open()
      Try
       zipStream.Write(fileData, 0, fileData.Length)
      Catch ex As Exception
      End Try
     End Using

    Catch ex As Exception

    Finally
     strmZipStream.Dispose()
    End Try
   Catch ex As Exception

   Finally
    If Not strmZipFile Is Nothing Then
     strmZipFile.Close()
     strmZipFile.Dispose()
    End If
   End Try

   Return fileName

  End Function
#End Region

#Region " Pack Creation V5 DNN 6+ "
  Private Shared Function GetLanguagePackManifestV5(ByVal objObjects As List(Of ObjectInfo), ByVal locale As String) As XmlDocument
   Dim loc As New CultureInfo(locale)
   Dim manifest As New XmlDocument
   manifest.AppendChild(manifest.CreateXmlDeclaration("1.0", Nothing, Nothing))
   Dim root As XmlNode = manifest.CreateElement("dotnetnuke")
   manifest.AppendChild(root)
   Globals.AddAttribute(root, "type", "Package")
   Globals.AddAttribute(root, "version", "5.0")
   Dim package As XmlNode = Globals.AddElement(root, "packages")
   For Each objObject As ObjectInfo In objObjects
    AddObjectToV5Manifest(package, objObject, loc)
   Next
   Return manifest
  End Function

  Private Shared Sub AddObjectToV5Manifest(ByRef packagesNode As XmlNode, ByVal objObject As ObjectInfo, ByVal loc As CultureInfo)

   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", objObject.ObjectName & "_" & loc.Name) ' Our package name.
   If objObject.IsCore Then
    Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Else
    Globals.AddAttribute(package, "type", "ExtensionLanguagePack")
   End If
   Globals.AddAttribute(package, "version", objObject.Version)
   Globals.AddElement(package, "friendlyName", objObject.FriendlyName & " " & loc.NativeName) ' little to add here to name
   Dim attributionText As String = ""
   If objObject.Module.Attribution.Trim <> "" Then
    For Each u As DotNetNuke.Entities.Users.UserInfo In ObjectsController.GetContributorList(objObject.ObjectId, objObject.Version, loc.Name)
     Dim tr As New TokenReplace(objObject, u, loc.Name)
     attributionText &= "<br />" & tr.ReplaceEnvironmentTokens(objObject.Module.Attribution)
    Next
   End If
   Dim description As String = DotNetNuke.Services.Localization.Localization.GetString("ManifestDescription", Globals.glbSharedResources, loc.Name)
   If description = DotNetNuke.Services.Localization.Localization.GetString("ManifestDescription", Globals.glbSharedResources, "en-US") Then
    description = String.Format(description, loc.EnglishName, objObject.FriendlyName)
   Else
    description = String.Format(description, loc.NativeName, objObject.FriendlyName)
   End If
   Globals.AddElement(package, "description", description & attributionText, True)
   Dim owner As XmlNode = Globals.AddElement(package, "owner")
   Globals.AddElement(owner, "name", objObject.Module.OwnerName)
   Globals.AddElement(owner, "organization", objObject.Module.OwnerOrganization)
   Globals.AddElement(owner, "url", objObject.Module.OwnerUrl)
   Globals.AddElement(owner, "email", objObject.Module.OwnerEmail)
   Globals.AddElement(package, "license", Globals.GetLicense(DotNetNuke.Common.ApplicationMapPath & "\" & objObject.Module.HomeDirectory & "\", objObject.Module.ModuleId), True)
   Globals.AddElement(package, "releaseNotes", attributionText, True)
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
     If Right(objObject.ObjectName, 4) = "_PRO" AndAlso Globals.glbProPackages.Contains(objObject.ObjectName.Replace("_PRO", "")) Then
      .Name = objObject.ObjectName.Replace("_PRO", "")
     Else
      .Name = objObject.ObjectName
     End If
     .FriendlyName = objObject.FriendlyName
    End With
    DotNetNuke.Services.Installer.LegacyUtil.ParsePackageName(dnnPackage)
    Globals.AddElement(files, "package", dnnPackage.Name) ' this creates the dependency between lang pack and object (ignored for Core)
   End If
   Dim basePath As String = ""
   Globals.AddElement(files, "basePath", basePath) ' basepath needs to be added to object
   For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, objObject.Version)
    If TextsController.GetTextsByObjectAndFile(objObject.ModuleId, objObject.ObjectId, filePath, loc.Name, objObject.Version, False).Count > 0 Then
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

  Public Shared Function GetObjectBasePath(ByVal objObject As ObjectInfo) As String
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

  Public Shared Function GetLastEditTime(ByVal objectId As Integer, ByVal locale As String, ByVal version As String) As DateTime
   Dim res As DateTime = Now
   Using ir As IDataReader = Data.DataProvider.Instance().GetLastEditTime(objectId, locale, version)
    If ir.Read Then
     res = CDate(ir.Item(0))
    End If
   End Using
   Return res
  End Function

  Public Shared Function CleanName(ByVal name As String) As String
   Return name.Replace("\", "_").Replace("/", "_")
  End Function

 End Class
End Namespace

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

   Dim objectsToPack As New List(Of ObjectInfo)
   objectsToPack.Add(objObject)
   ' If it's a Core pack we include default objects
   If objObject.IsCore Then
    For Each o As ObjectInfo In ObjectCoreVersionsController.GetCoreObjects(version, isFullPack)
     objectsToPack.Add(o)
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
    Dim strmZipStream As ZipOutputStream = Nothing
    Try
     strmZipStream = New ZipOutputStream(strmZipFile)

     Dim myZipEntry As ZipEntry

     Dim loc As New CultureInfo(locale)
     Dim manifestName As String = packName & "_" & loc.Name & ".dnn"
     manifestName = manifestName.Replace("/", "_").Replace("\", "_")
     myZipEntry = New ZipEntry(manifestName)
     strmZipStream.PutNextEntry(myZipEntry)
     strmZipStream.SetLevel(compressionLevel)
     Dim manifestV5 As XmlDocument = GetLanguagePackManifestV5(objectsToPack, version, locale)
     Dim quirkPack As Boolean = CBool(objectsToPack(0).IsCore And version < "06.00.00")
     Dim fileData As Byte() = Encoding.UTF8.GetBytes(manifestV5.OuterXml)
     strmZipStream.Write(fileData, 0, fileData.Length)

     For Each o As ObjectInfo In objectsToPack
      Dim basePath As String = ""
      If Not quirkPack Then basePath = GetObjectBasePath(o)
      For Each filePath As String In TextsController.GetFileList(o.ObjectId, version)
       Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
       resFileName = resFileName.Replace(".resx", pattern)
       Dim targetPath As String = GetResourceZipPathV5(filePath, basePath)
       Dim texts As IDictionary(Of Integer, Entities.Texts.TextInfo) = TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, locale, version, False)
       If texts.Count > 0 Then ' do not write an empty file
        Dim resDoc As New XmlDocument
        resDoc.Load(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\DNNEurope\LocalizationEditor\App_LocalResources\Template.resx")
        Dim root As XmlNode = resDoc.DocumentElement
        For Each ti As Entities.Texts.TextInfo In texts.Values
         Globals.AddResourceText(root, ti.TextKey, ti.TextValue)
        Next
        myZipEntry = New ZipEntry(targetPath & "\" & resFileName)
        strmZipStream.PutNextEntry(myZipEntry)
        Using w As New MemoryStream
         Using xw As New XmlTextWriter(w, Encoding.UTF8)
          xw.Formatting = Formatting.Indented
          resDoc.WriteContentTo(xw)
         End Using
         fileData = w.ToArray()
        End Using
        strmZipStream.Write(fileData, 0, fileData.Length)
       End If
      Next
     Next

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

#Region " Pack Creation V5 DNN 5 "
  Private Shared Sub AddCoreV5ManifestDnn5(ByRef packagesNode As XmlNode, ByVal objObjects As List(Of ObjectInfo), ByVal version As String, ByVal loc As CultureInfo)
   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", Globals.glbCoreName & " " & loc.NativeName) ' Our package name. The convention is Objectname + verbose language
   Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Globals.AddAttribute(package, "version", version)
   Globals.AddElement(package, "friendlyName", Globals.glbCoreFriendlyName & " " & loc.NativeName) ' little to add here to name
   Globals.AddElement(package, "description", String.Format(DotNetNuke.Services.Localization.Localization.GetString("ManifestDescription", Globals.glbSharedResources, loc.Name), loc.NativeName, Globals.glbCoreFriendlyName))
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
     If TextsController.GetTextsByObjectAndFile(o.ModuleId, o.ObjectId, filePath, loc.Name, version, False).Count > 0 Then
      AddPackResourcePathToManifestV5(files, filePath, loc.Name, basePath)
     End If
    Next
   Next
  End Sub
#End Region

#Region " Pack Creation V5 DNN 6+ "
  Private Shared Function GetLanguagePackManifestV5(ByVal objObjects As List(Of ObjectInfo), ByVal version As String, ByVal locale As String) As XmlDocument
   Dim loc As New CultureInfo(locale)
   Dim manifest As New XmlDocument
   manifest.AppendChild(manifest.CreateXmlDeclaration("1.0", Nothing, Nothing))
   Dim root As XmlNode = manifest.CreateElement("dotnetnuke")
   manifest.AppendChild(root)
   Globals.AddAttribute(root, "type", "Package")
   Globals.AddAttribute(root, "version", "5.0")
   Dim package As XmlNode = Globals.AddElement(root, "packages")
   If objObjects(0).IsCore And version < "06.00.00" Then
    ' Special provision as DNN 5 installer fails if a dependant package is not installed
    AddCoreV5ManifestDnn5(package, objObjects, version, loc)
   Else
    For Each objObject As ObjectInfo In objObjects
     AddObjectToV5Manifest(package, objObject, version, loc)
    Next
   End If
   Return manifest
  End Function

  Private Shared Sub AddObjectToV5Manifest(ByRef packagesNode As XmlNode, ByVal objObject As ObjectInfo, ByVal version As String, ByVal loc As CultureInfo)

   Dim package As XmlNode = Globals.AddElement(packagesNode, "package")
   Globals.AddAttribute(package, "name", objObject.ObjectName & " " & loc.NativeName) ' Our package name. The convention is Objectname + verbose language
   If objObject.IsCore Then
    Globals.AddAttribute(package, "type", "CoreLanguagePack")
   Else
    Globals.AddAttribute(package, "type", "ExtensionLanguagePack")
   End If
   Globals.AddAttribute(package, "version", version)
   Globals.AddElement(package, "friendlyName", objObject.ObjectName & " " & loc.NativeName) ' little to add here to name
   Globals.AddElement(package, "description", String.Format(DotNetNuke.Services.Localization.Localization.GetString("ManifestDescription", Globals.glbSharedResources, loc.Name), loc.NativeName, objObject.ObjectName))
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
   For Each filePath As String In TextsController.GetFileList(objObject.ObjectId, version)
    If TextsController.GetTextsByObjectAndFile(objObject.ModuleId, objObject.ObjectId, filePath, loc.Name, version, False).Count > 0 Then
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

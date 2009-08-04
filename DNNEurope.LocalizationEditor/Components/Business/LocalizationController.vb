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

Namespace DNNEurope.Modules.LocalizationEditor.Business
    Public Class LocalizationController
        ''' <summary>
        ''' Retrieves the latest resource for installed modules and updates the values in the localization editor data model
        ''' </summary>
        ''' <param name="InstallationMapPath"></param>
        ''' <param name="PortalId"></param>
        ''' <param name="objObjectInfo"></param>
        ''' <param name="UserId"></param>
        ''' <remarks></remarks>
        Public Shared Sub ReadResourceFiles(ByVal InstallationMapPath As String, ByVal PortalId As Integer, _
                                             ByVal objObjectInfo As ObjectInfo, ByVal UserId As Integer)

            Dim resourceFileList As New SortedList
            Dim version As String = "01.00.00"
            Dim pattern As String = ".resx"
            If objObjectInfo.ObjectName = "DNN Core" Then
                Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "admin", "")
                Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "controls", "")
                Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "providers", "")
                Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "install", "")
                resourceFileList.Add( _
                                      Localization.GlobalResourceFile.Replace("~/", InstallationMapPath).Replace( _
                                                                                                                   ".resx", _
                                                                                                                   pattern), _
                                      New FileInfo( _
                                                    Localization.GlobalResourceFile.Replace("~/", InstallationMapPath). _
                                                       Replace(".resx", pattern)))
                resourceFileList.Add( _
                                      Localization.SharedResourceFile.Replace("~/", InstallationMapPath).Replace( _
                                                                                                                   ".resx", _
                                                                                                                   pattern), _
                                      New FileInfo( _
                                                    Localization.SharedResourceFile.Replace("~/", InstallationMapPath). _
                                                       Replace(".resx", pattern)))
                version = GetFrameworkVersion()
            Else
                Dim _
                    _module As DesktopModuleInfo = _
                        DesktopModuleController.GetDesktopModuleByModuleName(objObjectInfo.ObjectName, PortalId)
                If _module Is Nothing Then
                    '// If the module is not found, then it's not installed in DotNetNuke. Skip the processing
                    Return
                End If
                version = _module.Version.Trim
                If String.IsNullOrEmpty(version) Then
                    version = "0"
                    '2009-06-26 Janga:  Default version.
                End If
                Globals.GetResourceFiles(resourceFileList, InstallationMapPath & "DesktopModules\" & _module.FolderName, _
                                          "")
                ' remove the other files if part of another module that is nested
                For Each m As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(PortalId).Values
                    If _
                        (Not m.ModuleName = objObjectInfo.ObjectName) AndAlso (m.FolderName <> _module.FolderName) AndAlso _
                        (m.FolderName.StartsWith(_module.FolderName)) Then
                        Globals.RemoveResourceFiles(resourceFileList, _
                                                     InstallationMapPath & "DesktopModules\" & m.FolderName, "")
                    End If
                Next
            End If

            ProcessResourceFiles(resourceFileList, InstallationMapPath, objObjectInfo, version)
        End Sub

        Public Shared Sub ProcessResourceFiles(ByVal resourceFileList As SortedList, ByVal rootPath As String, _
                                                ByVal objObjectInfo As ObjectInfo, ByVal version As String)
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
                    Globals.SimpleLog( _
                                       "Original resource file '" & file.Key.ToString & _
                                       "' is incorrect or could not be read.")
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
                            Globals.SimpleLog("Error importing resources: " & ex.Message, ex.StackTrace, _
                                               "Filekey: " & fileKey, "Key    : " & key, "Value  : " & value, "")
                        End Try
                    Next
                End If
            Next

            ' Deprecate old stuff
            For Each ti As TextInfo In TextsController.GetTextsByObject(objObjectInfo.ObjectId, "", version).Values
                If ti.Version <> version Then ' it's an old one
                    If Not currentVersionKeys.Contains(ti.FilePath & ";" & ti.TextKey) Then
                        ti.DeprecatedIn = version
                        TextsController.UpdateText(ti)
                    End If
                End If
            Next
        End Sub

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
                    .NrOfMissingTranslations = _
                        TextsController.NrOfMissingTranslations(ObjectId, Locale, .CurrentVersion)
                End If
                If .NrOfItems > 0 Then _
                    .PercentageComplete = CInt(Math.Round(((.NrOfItems - .NrOfMissingTranslations) * 100.0) / .NrOfItems))
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

        Public Shared Function CreateResourcePack(ByVal ObjectId As Integer, ByVal ObjectName As String, _
                                                   ByVal Version As String, ByVal Locale As String) As String

            Dim CompressionLevel As Integer = 9
            Dim pattern As String = ".resx"
            If Locale <> "" Then
                pattern = "." & Locale & pattern
            End If
            Dim packPath As String = DotNetNuke.Common.HostMapPath & "LocalizationEditor\"
            If Not Directory.Exists(packPath) Then
                Directory.CreateDirectory(packPath)
            End If
            Dim fileName As String = "ResourcePack." & ObjectName & "." & Version & "." & Locale & ".zip"

            Dim strmZipFile As FileStream = Nothing
            Try
                strmZipFile = File.Create(packPath & fileName)
                Dim strmZipStream As ZipOutputStream = Nothing
                Try
                    strmZipStream = New ZipOutputStream(strmZipFile)

                    Dim myZipEntry As ZipEntry
                    myZipEntry = New ZipEntry("Manifest.xml")

                    strmZipStream.PutNextEntry(myZipEntry)
                    strmZipStream.SetLevel(CompressionLevel)

                    Dim manifest As XmlDocument = GetLanguagePackManifest(ObjectId, Version, Locale)
                    Dim FileData As Byte() = Encoding.UTF8.GetBytes(manifest.OuterXml)
                    strmZipStream.Write(FileData, 0, FileData.Length)

                    For Each filePath As String In TextsController.GetFileList(ObjectId, Version)
                        Dim resFileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
                        resFileName = resFileName.Replace(".resx", pattern)
                        Dim targetPath As String = GetResourceZipPath(filePath)
                        Dim _
                            texts As IDictionary(Of Integer, TextInfo) = _
                                TextsController.GetTextsByObjectAndFile(ObjectId, filePath, Locale, Version, False)
                        If texts.Count > 0 Then ' do not write an empty file
                            Dim resDoc As New XmlDocument
                            resDoc.Load( _
                                         DotNetNuke.Common.ApplicationMapPath & _
                                         "\DesktopModules\DNNEurope\LocalizationEditor\App_LocalResources\Template.resx")
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

        Private Shared Function GetLanguagePackManifest(ByVal ObjectId As Integer, ByVal Version As String, _
                                                         ByVal Locale As String) As XmlDocument
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
                If TextsController.GetTextsByObjectAndFile(ObjectId, filePath, Locale, Version, False).Count > 0 Then
                    AddPackResourcePathToManifest(files, filePath, Locale)
                End If
            Next
            Return manifest
        End Function

        Private Shared Function GetResourceZipPath(ByVal fullPath As String) As String
            Try
                fullPath = _
                    Regex.Replace(fullPath, _
                                   "(?i)^(desktopmodules|admin|controls|app_globalresources|install|providers)(?-i)", _
                                   AddressOf ReplaceRootPath)
                fullPath = Regex.Replace(fullPath, "(?i)\\([^\\]+)\\([^\\]+)$(?-i)", "")
            Catch ex As Exception
            End Try
            Return fullPath
        End Function

        Private Shared Function ReplaceRootPath(ByVal m As Match) As String
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

        Private Shared Sub AddPackResourcePathToManifest(ByRef filesNode As XmlNode, ByVal filePath As String, _
                                                          ByVal locale As String)
            Try
                Dim fileName As String = Mid(filePath, filePath.LastIndexOf("\") + 2)
                fileName = fileName.Replace(".resx", "." & locale & ".resx")
                Dim fullPath As String = Left(filePath, filePath.LastIndexOf("\"))
                Dim part As String = Left(fullPath, fullPath.IndexOf("\")).ToLower
                fullPath = Mid(fullPath, fullPath.IndexOf("\") + 2)
                If fullPath.ToLower.EndsWith("\app_localresources") Then
                    fullPath = Left(fullPath, fullPath.Length - 19)
                End If
                Select Case part
                    Case "admin"
                        'fullPath = "AdminResource\" & fullPath
                        Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=AdminResource", _
                                            "ModuleName=" & fullPath)
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
                        Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=LocalResource", _
                                            "ModuleName=" & fullPath)
                    Case "providers"
                        'fullPath = "ProviderResource\" & fullPath
                        Globals.AddElement(filesNode, "File", "", "FileName=" & fileName, "FileType=ProviderResource", _
                                            "ModuleName=" & fullPath)
                End Select

            Catch ex As Exception

            End Try
        End Sub

        Public Shared Function GetFrameworkVersion() As String
            Dim res As String = "01.00.00"
            Using ir As IDataReader = DataProvider.Instance.GetFrameworkVersion
                Do While ir.Read
                    res = _
                        String.Format("{0:00}.{1:00}.{2:00}", ir.Item("Major"), ir.Item("Minor"), ir.Item("Build"))
                    Exit Do
                Loop
            End Using
            Return res
        End Function

        Public Shared Function GetCorrectPath(ByVal manifestPath As String, ByVal localResourceDirectory As String) _
            As String
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
    End Class
End Namespace

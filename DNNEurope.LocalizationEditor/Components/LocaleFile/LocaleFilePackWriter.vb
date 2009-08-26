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


Imports DotNetNuke.Services.Localization
Imports System.IO
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Exceptions
Imports System.Xml.Serialization
Imports ICSharpCode.SharpZipLib.Zip

Namespace LocaleFile
 Public Class LocaleFilePackWriter
  Private _ProgressLog As New Logger

  Public ReadOnly Property ProgressLog() As Logger
   Get
    Return _ProgressLog
   End Get
  End Property

  Private Function GetServerPath(ByVal RelativePath As String) As String
   Return HttpContext.Current.Server.MapPath(RelativePath)
  End Function

  Private Sub ProcessCoreFiles(ByVal ResPack As LocaleFilePack)
   ' Global files
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Global")))
   GetGlobalResourceFiles(ResPack.Files, ResPack.LocalePackCulture.Code)
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Global")))

   ' Controls files
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Control")))
   GetResourceFiles(ResPack.Files, GetServerPath("~/Controls"), GetServerPath("~/Controls"), ResPack.LocalePackCulture.Code, LocaleType.ControlResource)
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Control")))

   ' Admin files
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Admin")))
   GetResourceFiles(ResPack.Files, GetServerPath("~/Admin"), GetServerPath("~/Admin"), ResPack.LocalePackCulture.Code, LocaleType.AdminResource)
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Admin")))
  End Sub

  Private Sub ProcessModuleFiles(ByVal ResPack As LocaleFilePack)
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Module")))
   GetResourceFiles(ResPack.Files, GetServerPath("~/desktopmodules"), GetServerPath("~/desktopmodules"), ResPack.LocalePackCulture.Code, LocaleType.LocalResource)
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Module")))
  End Sub

  Private Sub ProcessModuleFiles(ByVal ResPack As LocaleFilePack, ByVal basefolders As ArrayList)
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Module")))
   For Each f As String In basefolders
    GetResourceFiles(ResPack.Files, GetServerPath("~/desktopmodules/") & f, GetServerPath("~/desktopmodules"), ResPack.LocalePackCulture.Code, LocaleType.LocalResource)
   Next
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Module")))
  End Sub

  Private Sub ProcessProviderFiles(ByVal ResPack As LocaleFilePack)
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Provider")))
   GetResourceFiles(ResPack.Files, GetServerPath("~/providers"), GetServerPath("~/providers"), ResPack.LocalePackCulture.Code, LocaleType.ProviderResource)
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Provider")))
  End Sub

  Private Sub ProcessProviderFiles(ByVal ResPack As LocaleFilePack, ByVal basefolders As ArrayList)
   ProgressLog.StartJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Provider")))
   For Each f As String In basefolders
    GetResourceFiles(ResPack.Files, GetServerPath("~/providers/HtmlEditorProviders/") & f, GetServerPath("~/providers"), ResPack.LocalePackCulture.Code, LocaleType.ProviderResource)
   Next
   ProgressLog.EndJob(String.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Provider")))
  End Sub

  Public Function SaveLanguagePack(ByVal SourceCulture As Locale, ByVal TargetCulture As Locale, ByVal fileList As SortedList, ByVal FileName As String) As String

   Dim resFileList As New LocaleFileCollection
   Dim basedir As String = DotNetNuke.Common.ApplicationMapPath
   For Each file As DictionaryEntry In fileList
    Dim localizationFile As String = Globals.ResourceFile(file.Key.ToString, SourceCulture.Code)
    If IO.File.Exists(localizationFile) Then
     Dim fi As New FileInfo(localizationFile)
     If InStr(localizationFile, "\DesktopModules\", CompareMethod.Text) > 1 Then
      resFileList.Add(GetLocaleFile(basedir & "\DesktopModules\", LocaleType.LocalResource, fi))
     ElseIf InStr(localizationFile, "\Admin\", CompareMethod.Text) > 1 Then
      resFileList.Add(GetLocaleFile(basedir & "\Admin\", LocaleType.LocalResource, fi))
     ElseIf InStr(localizationFile, "\Controls\", CompareMethod.Text) > 1 Then
      resFileList.Add(GetLocaleFile(basedir & "\Controls\", LocaleType.LocalResource, fi))
     Else
     End If
    End If
   Next

   Dim Result As String
   Dim ResPack As New LocaleFilePack
   ResPack.Version = "3.0"
   ResPack.LocalePackCulture = TargetCulture
   ResPack.Files = resFileList

   ProgressLog.StartJob(Localization.GetString("LOG.LangPack.ArchiveFiles"))
   Result = CreateResourcePack(ResPack, FileName, SourceCulture.Code, TargetCulture.Code)
   ProgressLog.EndJob(Localization.GetString("LOG.LangPack.ArchiveFiles"))

   ' log installation event
   Try
    Dim objEventLogInfo As New LogInfo
    objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString
    objEventLogInfo.LogProperties.Add(New LogDetailInfo("Install Language Pack:", FileName))
    Dim objLogEntry As LogEntry
    For Each objLogEntry In ProgressLog.Logs
     objEventLogInfo.LogProperties.Add(New LogDetailInfo("Info:", objLogEntry.Description))
    Next
    Dim objEventLog As New EventLogController
    objEventLog.AddLog(objEventLogInfo)
   Catch ex As Exception
    ' error
   End Try

   Return Result

  End Function

  Public Function SaveLanguagePack(ByVal SourceCulture As Locale, ByVal TargetCulture As Locale, ByVal packtype As LanguagePackType, ByVal basefolders As ArrayList, ByVal FileName As String) As String
   Dim Result As String
   Dim ResPack As New LocaleFilePack
   ResPack.Version = "3.0"
   ResPack.LocalePackCulture = TargetCulture

   Select Case packtype
    Case LanguagePackType.Core
     ProcessCoreFiles(ResPack)

    Case LanguagePackType.Module
     ProcessModuleFiles(ResPack, basefolders)

    Case LanguagePackType.Provider
     ProcessProviderFiles(ResPack, basefolders)

    Case LanguagePackType.Full
     ProcessCoreFiles(ResPack)
     ProcessModuleFiles(ResPack)
     ProcessProviderFiles(ResPack)

   End Select

   ProgressLog.StartJob(Localization.GetString("LOG.LangPack.ArchiveFiles"))
   Result = CreateResourcePack(ResPack, FileName, SourceCulture.Code, TargetCulture.Code)
   ProgressLog.EndJob(Localization.GetString("LOG.LangPack.ArchiveFiles"))

   ' log installation event
   Try
    Dim objEventLogInfo As New LogInfo
    objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString
    objEventLogInfo.LogProperties.Add(New LogDetailInfo("Install Language Pack:", FileName))
    Dim objLogEntry As LogEntry
    For Each objLogEntry In ProgressLog.Logs
     objEventLogInfo.LogProperties.Add(New LogDetailInfo("Info:", objLogEntry.Description))
    Next
    Dim objEventLog As New EventLogController
    objEventLog.AddLog(objEventLogInfo)
   Catch ex As Exception
    ' error
   End Try

   Return Result
  End Function

  Private Sub GetResourceFiles(ByVal ResFileList As LocaleFileCollection, ByVal BasePath As String, ByVal RootPath As String, ByVal LocaleCode As String, ByVal ResType As LocaleType)
   Dim folders As String()
   Try
    folders = Directory.GetDirectories(BasePath)
   Catch
    ' in case a folder does not exist in DesktopModules (base admin modules)
    ' just exit
    Exit Sub
   End Try

   Dim folder As String
   Dim objFolder As DirectoryInfo

   For Each folder In folders
    objFolder = New DirectoryInfo(folder)

    If objFolder.Name = Localization.LocalResourceDirectory Then
     ' found local resource folder, add resources
     GetLocalResourceFiles(ResFileList, RootPath, LocaleCode, ResType, objFolder)
     GetLocalSharedResourceFile(ResFileList, RootPath, LocaleCode, ResType, objFolder)
    Else
     GetResourceFiles(ResFileList, folder, RootPath, LocaleCode, ResType)
    End If
   Next

  End Sub

  Private Sub GetLocalResourceFiles(ByVal ResFileList As LocaleFileCollection, ByVal RootPath As String, ByVal LocaleCode As String, ByVal ResType As LocaleType, ByVal objFolder As DirectoryInfo)
   Dim objFile As FileInfo
   Dim ascxFiles As FileInfo()
   Dim aspxFiles As FileInfo()

   If LocaleCode = Localization.SystemLocale Then
    ' This is the case for en-US which is the default locale
    ascxFiles = objFolder.GetFiles("*.ascx.resx")
    aspxFiles = objFolder.GetFiles("*.aspx.resx")
   Else
    ascxFiles = objFolder.GetFiles("*.ascx." & LocaleCode & ".resx")
    aspxFiles = objFolder.GetFiles("*.aspx." & LocaleCode & ".resx")
   End If

   For Each objFile In ascxFiles
    ResFileList.Add(GetLocaleFile(RootPath, ResType, objFile))
    ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name))
   Next
   For Each objFile In aspxFiles
    ResFileList.Add(GetLocaleFile(RootPath, ResType, objFile))
    ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name))
   Next

  End Sub

  Private Sub GetLocalSharedResourceFile(ByVal ResFileList As LocaleFileCollection, ByVal RootPath As String, ByVal LocaleCode As String, ByVal ResType As LocaleType, ByVal objFolder As DirectoryInfo)
   Dim SharedFile As FileInfo
   Dim strFilePath As String
   If LocaleCode = Localization.SystemLocale Then
    ' This is the case for en-US which is the default locale
    strFilePath = Path.Combine(objFolder.FullName, "SharedResources.resx")
   Else
    strFilePath = Path.Combine(objFolder.FullName, "SharedResources." & LocaleCode & ".resx")
   End If
   If File.Exists(strFilePath) Then

    SharedFile = New FileInfo(strFilePath)

    ResFileList.Add(GetLocaleFile(RootPath, ResType, SharedFile))
    ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.LoadFileName"), SharedFile.Name))
   End If

  End Sub

  Private Function GetLocaleFile(ByRef RootPath As String, ByVal ResType As LocaleType, ByVal objFile As FileInfo) As LocaleFileInfo
   Dim LocaleFile As New LocaleFileInfo
   LocaleFile.LocaleFileName = objFile.Name
   LocaleFile.LocalePath = StripCommonDirectory(RootPath, objFile.DirectoryName)
   LocaleFile.LocaleModule = GetModuleName(LocaleFile.LocalePath)
   LocaleFile.LocalePath = StripModuleName(LocaleFile.LocalePath, LocaleFile.LocaleModule)
   LocaleFile.LocaleFileType = ResType
   LocaleFile.Buffer = GetFileAsByteArray(objFile)

   Return LocaleFile
  End Function

  Private Sub GetGlobalResourceFiles(ByVal ResFileList As LocaleFileCollection, ByVal LocaleCode As String)
   Dim objFile As FileInfo
   Dim objFolder As New DirectoryInfo(GetServerPath(Localization.ApplicationResourceDirectory))

   Dim Files As FileInfo()
   Dim TimeZoneFile As FileInfo
   If LocaleCode = Localization.SystemLocale Then
    ' This is the case for en-US which is the default locale
    Files = objFolder.GetFiles("*.resx")
    Dim LastIndex As Integer = Files.Length
    ReDim Preserve Files(LastIndex)
    TimeZoneFile = New FileInfo(Path.Combine(objFolder.FullName, "TimeZones.xml"))
    Files(LastIndex) = TimeZoneFile
   Else
    Files = objFolder.GetFiles("*." & LocaleCode & ".resx")
    Dim LastIndex As Integer = Files.Length
    ReDim Preserve Files(LastIndex)
    TimeZoneFile = New FileInfo(Path.Combine(objFolder.FullName, "TimeZones." & LocaleCode & ".xml"))
    Files(LastIndex) = TimeZoneFile
   End If

   For Each objFile In Files
    If (Not objFile.Name.StartsWith("Template")) AndAlso (LocaleCode <> Localization.SystemLocale OrElse (LocaleCode = Localization.SystemLocale AndAlso objFile.Name.IndexOf("."c) = objFile.Name.LastIndexOf("."c))) Then

     Dim LocaleFile As New LocaleFileInfo
     LocaleFile.LocaleFileName = objFile.Name
     'Since paths are relative and all global resources exist in a known directory,
     ' we don't need a path.
     LocaleFile.LocalePath = Nothing
     LocaleFile.LocaleFileType = LocaleType.GlobalResource
     LocaleFile.Buffer = GetFileAsByteArray(objFile)

     ResFileList.Add(LocaleFile)
     ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name))
    End If
   Next
  End Sub

  Public Function StripCommonDirectory(ByVal RootPath As String, ByVal FullPath As String) As String
   Dim NewPath As String = FullPath
   '.Replace("/", "\")
   NewPath = NewPath.Replace(RootPath, "")
   NewPath = NewPath.Replace(Localization.LocalResourceDirectory, "")
   NewPath = NewPath.Trim("\"c, "/"c)

   If NewPath.Length = 0 Then
    NewPath = Nothing
   End If
   Return NewPath
  End Function

  Public Function GetModuleName(ByVal FullPath As String) As String
   Dim ModuleName As String = FullPath
   If Not ModuleName Is Nothing Then
    Dim Paths As String() = ModuleName.Split("/"c)
    ModuleName = Paths(0)
   End If
   Return ModuleName
  End Function

  Public Function StripModuleName(ByVal BasePath As String, ByVal ModuleName As String) As String
   Dim NewPath As String
   If BasePath Is Nothing Or ModuleName Is Nothing Then
    Return Nothing
   Else
    NewPath = BasePath.Replace(ModuleName, "").Trim("/"c)
   End If

   If NewPath.Length = 0 Then
    NewPath = Nothing
   End If
   Return NewPath
  End Function

  Private Function GetFileAsByteArray(ByVal FileObject As FileInfo) As Byte()
   Dim Buffer(CType(FileObject.Length - 1, Integer)) As Byte
   Dim strmFile As FileStream = Nothing
   Try
    strmFile = FileObject.OpenRead()
    strmFile.Read(Buffer, 0, Buffer.Length)
   Catch ex As Exception
    LogException(ex)
    ProgressLog.AddFailure(String.Format(Localization.GetString("Log.ERROR.CreatingByteArray"), FileObject.Name, ex))
   Finally
    If Not strmFile Is Nothing Then
     strmFile.Close()
    End If
   End Try
   Return Buffer
  End Function

  Private Function GetLanguagePackManifest(ByVal ResourcePack As LocaleFilePack) As Byte()
   Dim Manifest() As Byte = Nothing
   Dim ManifestSerializer As New XmlSerializer(GetType(LocaleFilePack))
   Dim ms As New MemoryStream
   Try
    ManifestSerializer.Serialize(ms, ResourcePack)
    ReDim Manifest(CType(ms.Length - 1, Integer))
    ms.Position = 0
    ms.Read(Manifest, 0, CType(ms.Length, Integer))
    ProgressLog.AddInfo(Localization.GetString("LOG.LangPack.SerializeManifest"))
   Catch ex As Exception
    LogException(ex)
    ProgressLog.AddFailure(String.Format(Localization.GetString("LOG.LangPack.ERROR.ManifestFile"), ex))
   Finally
    ms.Close()
   End Try

   Return Manifest
  End Function

  Private Function CreateResourcePack(ByVal ResourcePack As LocaleFilePack, ByVal FileName As String, ByVal SourceLocale As String, ByVal TargetLocale As String) As String

   Dim CompressionLevel As Integer = 9
   Dim BlockSize As Integer = 4096
   Dim TargetEqualsSource As Boolean = (SourceLocale = TargetLocale)

   Dim strmZipFile As FileStream = Nothing
   Try
    strmZipFile = File.Create(FileName)
    Dim strmZipStream As ZipOutputStream = Nothing
    Try
     strmZipStream = New ZipOutputStream(strmZipFile)

     Dim myZipEntry As ZipEntry
     myZipEntry = New ZipEntry("Manifest.xml")

     strmZipStream.PutNextEntry(myZipEntry)
     strmZipStream.SetLevel(CompressionLevel)

     Dim FileData As Byte() = GetLanguagePackManifest(ResourcePack)

     If TargetEqualsSource Then
      strmZipStream.Write(FileData, 0, FileData.Length)
     Else
      Dim tmp As String = Encoding.UTF8.GetString(FileData)
      tmp = Globals.TranslateFilenames(tmp, TargetLocale)
      FileData = Encoding.UTF8.GetBytes(tmp)
      strmZipStream.Write(FileData, 0, FileData.Length)
     End If
     ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.SavedFile"), "Manifest.xml"))

     For Each LocaleFile As LocaleFileInfo In ResourcePack.Files
      If TargetEqualsSource Then
       myZipEntry = New ZipEntry(LocaleFileUtil.GetFullFileName(LocaleFile))
      Else
       myZipEntry = New ZipEntry(Globals.TranslateFilenames(LocaleFileUtil.GetFullFileName(LocaleFile), TargetLocale))
      End If
      strmZipStream.PutNextEntry(myZipEntry)
      strmZipStream.Write(LocaleFile.Buffer, 0, LocaleFile.Buffer.Length)
      ProgressLog.AddInfo(String.Format(Localization.GetString("LOG.LangPack.SavedFile"), LocaleFile.LocaleFileName))
     Next
    Catch ex As Exception
     LogException(ex)
     ProgressLog.AddFailure(String.Format(Localization.GetString("LOG.LangPack.ERROR.SavingFile"), ex))
    Finally
     If Not strmZipStream Is Nothing Then
      strmZipStream.Finish()
      strmZipStream.Close()
     End If
    End Try
   Catch ex As Exception
    LogException(ex)
    ProgressLog.AddFailure(String.Format(Localization.GetString("LOG.LangPack.ERROR.SavingFile"), ex))
   Finally
    If Not strmZipFile Is Nothing Then
     strmZipFile.Close()
    End If
   End Try

   Return FileName

  End Function
 End Class
End Namespace
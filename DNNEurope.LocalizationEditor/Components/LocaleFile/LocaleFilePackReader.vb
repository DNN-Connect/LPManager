'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports ICSharpCode.SharpZipLib.Zip
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization


Namespace DNNEurope.Modules.LocalizationEditor.LocaleFile
	Public Class LocaleFilePackReader

		Protected EXCEPTION_FileRead As String = GetLocalizedString("EXCEPTION_FileRead")

		Protected EXCEPTION_LangPack_Install As String = GetLocalizedString("EXCEPTION.LangPack.Install")	'There was an error installing the Language Pack. Error: {0}
		Protected EXCEPTION_LangPack_ManifestZip As String = GetLocalizedString("EXCEPTION.LangPack.ManifestZip")	 'There was an error reading the Language Pack manifest from the zip file. Error: {0}
		Protected EXCEPTION_LangPack_ManifestLoad As String = GetLocalizedString("EXCEPTION.LangPack.ManifestLoad")	   'There was an error loading the Language Pack manifest. Error: {0}
		Protected EXCEPTION_LangPack_FileMissing As String = GetLocalizedString("EXCEPTION.LangPack.FileMissing")	 'File {0} missing in Language Pack manifest.
		Protected EXCEPTION_LangPack_ResourceLoad As String = GetLocalizedString("EXCEPTION.LangPack.Install")	  'There was an error loading a Language Pack resource file. Error: {0}

		Protected LOG_LangPack_Job_LoadManifest As String = GetLocalizedString("LOG.LangPack.Job.LoadManifest")	   'Load manifest file.
		Protected LOG_LangPack_Job_DeserializeManifest As String = GetLocalizedString("LOG.LangPack.Job.DeserializeManifest")	 'Deserializing manifest file.
		Protected LOG_LangPack_Job_LoadFiles As String = GetLocalizedString("LOG.LangPack.Job.LoadFiles")	 'Unzipping language files
		Protected LOG_LangPack_Job_ImportFiles As String = GetLocalizedString("LOG.LangPack.Job.SaveFiles")	   'Importing language files
		Protected LOG_LangPack_Job_CreateLocale As String = GetLocalizedString("LOG.LangPack.Job.CreateLocale")	   'Creating/Updating the Locale


		Protected LOG_LangPack_LoadFiles As String = GetLocalizedString("LOG.LangPackReader.LoadFiles")	   'Unzipping language file: {0}
		Protected LOG_LangPack_ImportFiles As String = GetLocalizedString("LOG.LangPackReader.UnzipFiles")	  '"Import file: {0}"
		Protected LOG_LangPack_CreateLocale As String = GetLocalizedString("LOG.LangPackReader.CreateLocale")	 'Creating Locale: {0}
		Protected LOG_LangPack_ModuleWarning As String = GetLocalizedString("LOG.LangPackReader.ModuleWarning")	   '"Resource file {0} was not saved because the module {1} is not installed"

		Private _ProgressLog As New Logger

		Public ReadOnly Property ProgressLog() As Logger
			Get
				Return _ProgressLog
			End Get
		End Property



		Private Function GetLocalizedString(ByVal key As String) As String

			Return Localization.GetString(key, PortalSettings.Current)

		End Function

		Public Function Install(ByVal FileName As String) As Logger
			Dim strm As New FileStream(FileName, FileMode.Open)
			Try
				Install(strm)
			Finally
				strm.Close()
			End Try

			Return ProgressLog
		End Function

		Public Function Install(ByVal FileStrm As Stream) As Logger
			Dim Manifest As String
			Try
				FileStrm.Position = 0

				ProgressLog.StartJob(LOG_LangPack_Job_LoadManifest)
				Manifest = System.Text.Encoding.UTF8.GetString(GetLanguagePackManifest(FileStrm))
				ProgressLog.EndJob(LOG_LangPack_Job_LoadManifest)

				ProgressLog.StartJob(LOG_LangPack_Job_DeserializeManifest)
				Dim LanguagePack As LocaleFilePack = GetLanguagePack(Manifest)
				ProgressLog.EndJob(LOG_LangPack_Job_DeserializeManifest)

				ProgressLog.StartJob(LOG_LangPack_Job_LoadFiles)
				LoadLocaleFilesFromZip(LanguagePack, FileStrm)
				ProgressLog.EndJob(LOG_LangPack_Job_LoadFiles)

				ProgressLog.StartJob(LOG_LangPack_Job_ImportFiles)
				SaveLocaleFiles(LanguagePack)
				ProgressLog.EndJob(LOG_LangPack_Job_ImportFiles)

				ProgressLog.StartJob(LOG_LangPack_Job_CreateLocale)
				CreateLocale(LanguagePack.LocalePackCulture)
				ProgressLog.EndJob(LOG_LangPack_Job_CreateLocale)

			Catch ex As Exception
				LogException(ex)
				ProgressLog.AddFailure(String.Format(EXCEPTION_LangPack_Install, ex.Message))
			End Try

			Return ProgressLog

		End Function

		Protected Function GetLanguagePackManifest(ByVal LangPackStream As Stream) As Byte()
			Dim unzip As New ZipInputStream(LangPackStream)
			Dim Buffer As Byte() = Nothing
			Try
				Dim entry As ZipEntry = unzip.GetNextEntry()

				While Not (entry Is Nothing)
					If Not entry.IsDirectory AndAlso Path.GetFileName(entry.Name).ToLower = "manifest.xml" Then
						Exit While
					End If
					entry = unzip.GetNextEntry
				End While

				Dim size As Integer = 0
				If Not entry Is Nothing Then
					ReDim Buffer(Convert.ToInt32(entry.Size) - 1)
					While size < Buffer.Length
						size += unzip.Read(Buffer, size, Buffer.Length - size)
					End While
					If size <> Buffer.Length Then
						Throw New Exception(EXCEPTION_FileRead & Buffer.Length & "/" & size)
					End If
				Else
					Throw New Exception(EXCEPTION_FileRead & Buffer.Length & "/" & size)
				End If
			Catch ex As Exception
				LogException(ex)
				ProgressLog.AddFailure(String.Format(EXCEPTION_LangPack_ManifestZip, ex.Message))
			Finally

			End Try

			Return Buffer

		End Function

		Protected Function GetLanguagePack(ByVal Manifest As String) As LocaleFilePack

			Dim ManifestSerializer As New XmlSerializer(GetType(LocaleFilePack))
			Dim LanguagePack As LocaleFilePack = Nothing
			Try
				Dim ManifestXML As New StringReader(Manifest)
				Dim XMLText As New XmlTextReader(ManifestXML)
				LanguagePack = (CType(ManifestSerializer.Deserialize(XMLText), LocaleFilePack))
			Catch ex As Exception
				LogException(ex)
				ProgressLog.AddFailure(String.Format(EXCEPTION_LangPack_ManifestLoad, ex.Message))
			Finally

			End Try

			Return LanguagePack
		End Function

		Protected Sub LoadLocaleFilesFromZip(ByVal LangPack As LocaleFilePack, ByVal LangPackStream As Stream)
			LangPackStream.Position = 0
			Dim unzip As New ZipInputStream(LangPackStream)
			Try
				Dim entry As ZipEntry = unzip.GetNextEntry()

				While Not (entry Is Nothing)
					If entry.Name.ToLower <> "manifest.xml" AndAlso (Not entry.IsDirectory) Then

						Dim LocaleFile As LocaleFileInfo = LangPack.Files.LocaleFile(entry.Name)
						If Not LocaleFile Is Nothing Then
							ProgressLog.AddInfo(String.Format(LOG_LangPack_LoadFiles, LocaleFile.LocaleFileName))
							ReDim LocaleFile.Buffer(Convert.ToInt32(entry.Size) - 1)
							Dim size As Integer = 0
							While size < LocaleFile.Buffer.Length
								size += unzip.Read(LocaleFile.Buffer, size, LocaleFile.Buffer.Length - size)
							End While
							If size <> LocaleFile.Buffer.Length Then
								Throw New Exception(EXCEPTION_FileRead & LocaleFile.Buffer.Length & "/" & size)
							End If
						Else
							ProgressLog.AddInfo(String.Format(EXCEPTION_LangPack_FileMissing, entry.Name))
						End If
					End If
					entry = unzip.GetNextEntry
				End While
			Catch ex As Exception
				LogException(ex)
				ProgressLog.AddFailure(String.Format(EXCEPTION_LangPack_ResourceLoad, ex.Message))
			End Try

		End Sub

		Protected Sub SaveLocaleFiles(ByVal LangPack As LocaleFilePack)
			Dim GlobalResourceDirectory As String = HttpContext.Current.Server.MapPath(Localization.ApplicationResourceDirectory)
			Dim ControlResourceDirectory As String = HttpContext.Current.Server.MapPath("~/controls/" & Localization.LocalResourceDirectory)
			Dim ProviderDirectory As String = HttpContext.Current.Server.MapPath("~/providers/")
			Dim InstallDirectory As String = HttpContext.Current.Server.MapPath("~/Install/")

			Dim AdminResourceRootDirectory As String = HttpContext.Current.Server.MapPath("~/Admin")
			Dim LocalResourceRootDirectory As String = HttpContext.Current.Server.MapPath("~/DesktopModules")

			For Each LocaleFile As LocaleFileInfo In LangPack.Files
				ProgressLog.AddInfo(String.Format(LOG_LangPack_ImportFiles, LocaleFile.LocaleFileName))
				Select Case LocaleFile.LocaleFileType
					Case LocaleType.ControlResource
						FileSystemUtils.SaveFile(Path.Combine(ControlResourceDirectory, LocaleFile.LocaleFileName), LocaleFile.Buffer)

					Case LocaleType.GlobalResource
						FileSystemUtils.SaveFile(Path.Combine(GlobalResourceDirectory, LocaleFile.LocaleFileName), LocaleFile.Buffer)

					Case LocaleType.AdminResource
						FileSystemUtils.SaveFile(GetFullLocaleFileName(AdminResourceRootDirectory, LocaleFile), LocaleFile.Buffer)

					Case LocaleType.LocalResource
						Try
							FileSystemUtils.SaveFile(GetFullLocaleFileName(LocalResourceRootDirectory, LocaleFile), LocaleFile.Buffer)
						Catch ex As Exception
							ProgressLog.AddInfo(String.Format(LOG_LangPack_ModuleWarning, LocaleFile.LocaleFileName, LocaleFile.LocaleModule))
						End Try
					Case LocaleType.ProviderResource
						Try
							FileSystemUtils.SaveFile(GetFullLocaleFileName(ProviderDirectory, LocaleFile), LocaleFile.Buffer)
						Catch ex As Exception
							ProgressLog.AddInfo(String.Format(LOG_LangPack_ModuleWarning, LocaleFile.LocaleFileName, LocaleFile.LocaleModule))
						End Try
					Case LocaleType.InstallResource
						Try
							FileSystemUtils.SaveFile(GetFullLocaleFileName(InstallDirectory, LocaleFile), LocaleFile.Buffer)
						Catch ex As Exception
							ProgressLog.AddInfo(String.Format(LOG_LangPack_ModuleWarning, LocaleFile.LocaleFileName, LocaleFile.LocaleModule))
						End Try
				End Select
			Next
		End Sub

		Protected Function GetFullLocaleFileName(ByVal RootPath As String, ByVal LocaleFile As LocaleFileInfo) As String
			Dim Result As String = RootPath & "/"
			If Not LocaleFile.LocaleModule Is Nothing Then
				Result &= LocaleFile.LocaleModule & "/"
			End If
			If Not LocaleFile.LocalePath Is Nothing Then
				Result &= LocaleFile.LocalePath & "/"
			End If
			Result &= Localization.LocalResourceDirectory & "/" & LocaleFile.LocaleFileName
			Return Result

		End Function

		Protected Sub CreateLocale(ByVal LocaleCulture As DotNetNuke.Services.Localization.Locale)
			ProgressLog.AddInfo(String.Format(LOG_LangPack_CreateLocale, LocaleCulture.Text))

			Localization.SaveLanguage(LocaleCulture)

			'Select Case Localization.AddLocale(LocaleCulture.Code, LocaleCulture.Text)
			'	Case "Duplicate.ErrorMessage"
			'		ProgressLog.AddWarning(Localization.GetString("Duplicate.ErrorMessage.Text"))
			'	Case "Save.ErrorMessage"
			'		ProgressLog.AddWarning(Localization.GetString("Save.ErrorMessage.Text"))
			'End Select
		End Sub

		Public Sub New()

		End Sub
	End Class
End Namespace


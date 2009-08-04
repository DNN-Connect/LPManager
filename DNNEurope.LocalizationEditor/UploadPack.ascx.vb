Imports System.Xml

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Localization.Localization

Imports DNNEurope.Modules.LocalizationEditor.Business

Namespace DNNEurope.Modules.LocalizationEditor
	Partial Public Class Import
		Inherits ModuleBase

#Region " Private Members "
        Private _ObjectId As Integer = -2
        Private _locale As String = ""
        Private _moduleFriendlyName As String = ""
        Private _tempDirectory As String = ""
#End Region

#Region " Properties "
        Public Property ModuleFriendlyName() As String
            Get
                Return _moduleFriendlyName
            End Get
            Set(ByVal value As String)
                _moduleFriendlyName = value
            End Set
        End Property

        Public Property Locale() As String
            Get
                Return _locale
            End Get
            Set(ByVal value As String)
                _locale = value
            End Set
        End Property

        Public Property ObjectId() As Integer
            Get
                Return _ObjectId
            End Get
            Set(ByVal value As Integer)
                _ObjectId = value
            End Set
        End Property

        Public Property TempDirectory() As String
            Get
                Return _tempDirectory
            End Get
            Set(ByVal value As String)
                _tempDirectory = value
            End Set
        End Property
#End Region

#Region " Event Handlers "
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
            Globals.ReadQuerystringValue(Me.Request.Params, "Locale", Locale)

            Dim objObjectInfo As ObjectInfo = ObjectController.GetObject(ObjectId)
            If objObjectInfo Is Nothing Then Return
            ModuleFriendlyName = objObjectInfo.FriendlyName
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            '// Force full postback for wizard
            DotNetNuke.Framework.AJAX.RegisterPostBackControl(wzdImport)

            wzdImport.CancelButtonText = "<img src=""" + ApplicationPath + "/images/cancel.gif"" border=""0"" /> " + GetString("Cancel", Me.LocalResourceFile)
            wzdImport.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + GetString("Next", Me.LocalResourceFile)
            wzdImport.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + GetString("Next", Me.LocalResourceFile)
            wzdImport.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + GetString("Previous", Me.LocalResourceFile)
            wzdImport.FinishPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + GetString("Previous", Me.LocalResourceFile)
            wzdImport.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/save.gif"" border=""0"" /> " + GetString("Finish", Me.LocalResourceFile)

            If Not Me.IsPostBack Then

                ' Permission check here
                If Locale = "" Or ObjectId = -2 Then
                    Throw New Exception("Access denied")
                End If
                If Not PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ModuleId, ObjectId, Locale) Then
                    Throw New Exception("Access denied")
                End If

                ddVersion.DataSource = Data.DataProvider.Instance.GetVersions(Me.ObjectId)
                ddVersion.DataBind()

            End If


        End Sub

        Private Sub wzdImport_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wzdImport.NextButtonClick

            Select Case e.CurrentStepIndex
                Case 0 'Upload
                    'Before we leave Page 1, the user must have uploaded a valid resource file
                    If Not ctlUpload.HasFile Then
                        lblUploadError.Text = GetString("NoFile", LocalResourceFile)
                        e.Cancel = True
                        Exit Sub
                    End If
                    TempDirectory = PortalSettings.HomeDirectoryMapPath & "LocalizationEditor\~tmp" & Now.ToString("yyyyMMdd-hhmmss") & "-" & (CInt(Rnd() * 1000)).ToString
                    Dim rep As New StringBuilder
                    If Not UnpackUploadedFile(rep) Then
                        lblUploadError.Text = GetString("WrongFile", LocalResourceFile)
                        lblUploadReport.Text = rep.ToString.Replace(vbCrLf, "<br />")
                        e.Cancel = True
                        Exit Sub
                    Else
                        txtResult.Text = rep.ToString
                    End If
                Case 1 ' Parameters
                    txtAnalysis.Text = AnalyzePack()
            End Select

        End Sub

        Private Sub wzdImport_CancelButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles wzdImport.CancelButtonClick
            Try
                If TempDirectory <> "" Then
                    IO.Directory.Delete(TempDirectory)
                End If
            Catch
            End Try
            Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
        End Sub

        Private Sub wzdImport_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wzdImport.FinishButtonClick
            ImportPack()
            Try
                If TempDirectory <> "" Then
                    IO.Directory.Delete(TempDirectory)
                End If
            Catch
            End Try
            Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
        End Sub
#End Region

#Region " Private Methods "
        Private Function UnpackUploadedFile(ByRef report As StringBuilder) As Boolean
            If Not IO.Directory.Exists(TempDirectory) Then
                IO.Directory.CreateDirectory(TempDirectory)
            Else
                Throw New Exception("New directory " & TempDirectory & " already exists")
            End If
            Try
                Dim resFilename As String = ctlUpload.FileName
                report.AppendLine("Uploaded " & resFilename)
                report.AppendLine("Unpacking to " & TempDirectory)
                Dim objZipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry
                Using objZipInputStream As New ICSharpCode.SharpZipLib.Zip.ZipInputStream(ctlUpload.FileContent)
                    objZipEntry = objZipInputStream.GetNextEntry
                    While Not objZipEntry Is Nothing
                        Dim strFileName As String = objZipEntry.Name.Replace("/", "\")
                        report.AppendLine("Unpacking " & strFileName)
                        If strFileName <> "" And Not objZipEntry.IsDirectory Then
                            Dim sFile As String = strFileName
                            Dim sPath As String = TempDirectory & "\"
                            If strFileName.IndexOf("\"c) > 0 Then
                                sFile = Mid(strFileName, strFileName.LastIndexOf("\"c) + 2)
                                sPath = sPath & Left(strFileName, strFileName.LastIndexOf("\"c))
                                If Not IO.Directory.Exists(sPath) Then
                                    IO.Directory.CreateDirectory(sPath)
                                End If
                                sPath &= "\"
                            End If
                            Using objFileStream As IO.FileStream = IO.File.Create(sPath & sFile)
                                Dim intSize As Integer = 2048
                                Dim arrData(2048) As Byte
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objFileStream.Write(arrData, 0, intSize)
                                    intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                End While
                            End Using
                        End If
                        objZipEntry = objZipInputStream.GetNextEntry
                    End While
                End Using
                Dim manifest As New XmlDocument
                manifest.Load(TempDirectory & "\Manifest.Xml")
                For Each xNode As XmlNode In manifest.SelectNodes("LanguagePack/Files/File")
                    Dim sFileName As String = xNode.Attributes("FileName").InnerText
                    Dim sFileType As String = xNode.Attributes("FileType").InnerText
                Next
                Dim m As Match = Regex.Match(resFilename, "\d\d\.\d\d\.\d\d")
                Try
                    ddVersion.ClearSelection()
                    If m.Success Then
                        Dim parsedVersion As String = m.Value
                        ddVersion.Items.FindByText(parsedVersion).Selected = True
                    Else
                        ddVersion.Items(ddVersion.Items.Count - 1).Selected = True
                    End If
                Catch
                End Try
            Catch ex As Exception
                report.AppendLine("Error Occurred: " & ex.Message)
                report.AppendLine(ex.StackTrace)
                Return False
            End Try
            Return True
        End Function

        Private Function AnalyzePack() As String
            Dim report As New StringBuilder
            Dim manifest As New XmlDocument
            manifest.Load(TempDirectory & "\Manifest.Xml")
            report.AppendLine("Loaded Manifest")
            For Each xNode As XmlNode In manifest.SelectNodes("LanguagePack/Files/File")
                Dim sFileName As String = xNode.Attributes("FileName").InnerText
                Dim sFileType As String = xNode.Attributes("FileType").InnerText
                Dim sModuleName As String = ""
                If xNode.Attributes("ModuleName") IsNot Nothing Then
                    sModuleName = xNode.Attributes("ModuleName").InnerText & "\"
                End If
                Dim sPath As String = sFileType & "\" & sModuleName & sFileName
                AnalyzeFile(report, sPath)
            Next
            Return report.ToString
        End Function

        Private Sub AnalyzeFile(ByRef report As StringBuilder, ByVal resFile As String)
            report.AppendLine("Analyzing " & resFile)
            Dim res As New XmlDocument
            res.Load(TempDirectory & "\" & resFile)
            Dim locPath As String = LocalizationController.GetCorrectPath(resFile, LocalResourceDirectory)
            report.AppendLine("Mapped to " & locPath)
            Using ir As IDataReader = Data.DataProvider.Instance.GetTextsByObjectAndFile(ObjectId, locPath, Locale, ddVersion.SelectedValue, True)
                Do While ir.Read
                    Dim textKey As String = CStr(ir.Item("TextKey"))
                    Dim hasValue As Boolean = False
                    If ir.Item("TextValue") IsNot DBNull.Value Then
                        hasValue = True
                    End If
                    Try
                        Dim xNode As XmlNode = res.SelectSingleNode("root/data[@name='" & textKey & "']")
                        If xNode Is Nothing Then
                            report.AppendLine("Nothing for " & textKey)
                        Else
                            If hasValue Then
                                report.AppendLine("Overwrite " & textKey)
                            Else
                                report.AppendLine("Add " & textKey)
                            End If
                        End If
                    Catch ex As XPath.XPathException
                        report.AppendLine("!!!! Invalid token in attribute value: " & textKey)
                    End Try
                Loop
            End Using
        End Sub

        Private Sub ImportPack()
            Dim manifest As New XmlDocument
            manifest.Load(TempDirectory & "\Manifest.Xml")
            For Each xNode As XmlNode In manifest.SelectNodes("LanguagePack/Files/File")
                Dim sFileName As String = xNode.Attributes("FileName").InnerText
                Dim sFileType As String = xNode.Attributes("FileType").InnerText
                Dim sModuleName As String = ""
                If xNode.Attributes("ModuleName") IsNot Nothing Then
                    sModuleName = xNode.Attributes("ModuleName").InnerText & "\"
                End If
                Dim sPath As String = sFileType & "\" & sModuleName & sFileName
                ImportFile(sPath)
            Next
        End Sub

        Private Sub ImportFile(ByVal resFile As String)
            Dim res As New XmlDocument
            res.Load(TempDirectory & "\" & resFile)
            Dim locPath As String = LocalizationController.GetCorrectPath(resFile, LocalResourceDirectory)

            'Fix slashes (from / to \ )
            locPath = locPath.Replace("/"c, "\"c)

            Using ir As IDataReader = Data.DataProvider.Instance.GetTextsByObjectAndFile(ObjectId, locPath, Locale, ddVersion.SelectedValue, True)
                Do While ir.Read
                    Dim textKey As String = CStr(ir.Item("TextKey"))
                    Dim textId As Integer = CInt(ir.Item("TextId"))
                    Dim hasValue As Boolean = False
                    If ir.Item("TextValue") IsNot DBNull.Value Then
                        hasValue = True
                    End If
                    Try
                        Dim xNode As XmlNode = res.SelectSingleNode("root/data[@name='" & textKey & "']")
                        If xNode IsNot Nothing Then
                            Try
                                Dim transValue As String = xNode.SelectSingleNode("value").InnerText
                                If hasValue Then
                                    Dim tr As TranslationInfo = TranslationsController.GetTranslation(textId, Locale)
                                    If tr.TextValue <> transValue Then
                                        With tr
                                            .LastModified = Now
                                            .LastModifiedUserId = UserId
                                            .TextValue = transValue
                                        End With
                                        TranslationsController.UpdateTranslation(tr)
                                    End If
                                Else
                                    Dim tr As New TranslationInfo
                                    With tr
                                        .TextId = textId
                                        .Locale = Locale
                                        .LastModified = Now
                                        .LastModifiedUserId = UserId
                                        .TextValue = transValue
                                    End With
                                    TranslationsController.AddTranslation(tr)
                                End If
                            Catch
                                ' ignore errors
                            End Try
                        End If
                    Catch ex As XPath.XPathException
                        ' ignore XPath errors
                    End Try
                Loop
            End Using
        End Sub
#End Region

#Region " ViewState Handling "
		Protected Overrides Sub LoadViewState(ByVal savedState As Object)

			If Not (savedState Is Nothing) Then
				Dim myState As Object() = CType(savedState, Object())
				If Not (myState(0) Is Nothing) Then
					MyBase.LoadViewState(myState(0))
				End If
				If Not (myState(1) Is Nothing) Then
					_tempDirectory = CType(myState(1), String)
				End If
			End If

		End Sub

		Protected Overrides Function SaveViewState() As Object

			Dim allStates(1) As Object
			allStates(0) = MyBase.SaveViewState()
			allStates(1) = _tempDirectory
			Return allStates

		End Function
#End Region

	End Class
End Namespace
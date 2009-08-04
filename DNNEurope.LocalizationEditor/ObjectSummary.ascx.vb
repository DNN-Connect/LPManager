Imports DotNetNuke.Services.Localization.Localization

Imports DNNEurope.Modules.LocalizationEditor.Business

Namespace DNNEurope.Modules.LocalizationEditor
	Partial Public Class ObjectSummary
		Inherits ModuleBase

#Region " Private Members "
        Private _ObjectId As Integer
        Private _locale As String
        Private _moduleFriendlyName As String
        Private _original As LocalizationController.ObjectMetrics
        Private _target As LocalizationController.ObjectMetrics
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

        Public Property Target() As LocalizationController.ObjectMetrics
            Get
                Return _target
            End Get
            Set(ByVal value As LocalizationController.ObjectMetrics)
                _target = value
            End Set
        End Property

        Public Property Original() As LocalizationController.ObjectMetrics
            Get
                Return _original
            End Get
            Set(ByVal value As LocalizationController.ObjectMetrics)
                _original = value
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

            If Not Me.IsPostBack Then
                LocalizationController.ReadResourceFiles(Server.MapPath("~/"), PortalId, objObjectInfo, UserId)
            End If
            Original = LocalizationController.GetObjectMetrics(ObjectId, "")
            Target = LocalizationController.GetObjectMetrics(ObjectId, Locale)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Me.IsPostBack Then

                ' Permission check here
                If Not PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ModuleId, ObjectId, Locale) Then
                    Throw New Exception("Access denied")
                End If

                ddSourceLocale.DataSource = Data.DataProvider.Instance.GetLocalesForUserObject(ObjectId, PortalSettings.AdministratorId, PortalId, ModuleId)
                ddSourceLocale.DataBind()
                ddSourceLocale.Items.Insert(0, New ListItem(GetString("NoSource", Me.LocalResourceFile), ""))

                ddVersion.DataSource = Data.DataProvider.Instance.GetVersions(ObjectId)
                ddVersion.DataBind()
                Try
                    ddVersion.Items.FindByValue(Original.CurrentVersion).Selected = True
                Catch
                End Try

                ddSelection.DataSource = Data.DataProvider.Instance().GetFiles(ObjectId, Original.CurrentVersion)
                ddSelection.DataBind()
                ddSelection.Items.Insert(0, New ListItem(GetString("All", Me.LocalResourceFile), "All"))
                ddSelection.Items.Insert(0, New ListItem(GetString("New", Me.LocalResourceFile), "New"))
                ddSelection.Items.Insert(0, New ListItem(GetString("Untranslated", Me.LocalResourceFile), "Untranslated"))

                cmdDownload.NavigateUrl = EditUrl("ObjectId", ObjectId.ToString, "DownloadPack")
                cmdUpload.NavigateUrl = EditUrl("ObjectId", ObjectId.ToString, "UploadPack", "Locale=" & Locale)
                cmdReturn.NavigateUrl = DotNetNuke.Common.NavigateURL

            End If

        End Sub

        Private Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
            Dim url As String = ""
            If ddSourceLocale.SelectedValue = "" Then
                url = EditUrl("ObjectId", ObjectId.ToString, "Edit", "Locale=" & Locale.ToString, "Version=" & ddVersion.SelectedValue, "Selection=" & ddSelection.SelectedValue) ', "AutoTranslate=" & IIf(chkAutoTranslate.Checked, "1", "0").ToString()
            Else
                url = EditUrl("ObjectId", ObjectId.ToString, "Edit", "Locale=" & Locale.ToString, "Version=" & ddVersion.SelectedValue, "SourceLocale=" & ddSourceLocale.SelectedValue, "Selection=" & ddSelection.SelectedValue) ', "AutoTranslate=" & IIf(chkAutoTranslate.Checked, "1", "0").ToString()
            End If
            Me.Response.Redirect(url, False)
        End Sub
#End Region

	End Class
End Namespace
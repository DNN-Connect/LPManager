Imports System.Globalization

Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.UI.Utilities

Imports DNNEurope.Modules.LocalizationEditor.Business

Namespace DNNEurope.Modules.LocalizationEditor
	<ControlMethodClass("DNNEurope.Modules.LocalizationEditor.Users")> _
 Partial Public Class Users
		Inherits ModuleBase

#Region " Event Handlers "
		Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
			ClientAPI.HandleClientAPICallbackEvent(Me.Page)
			ClientAPI.RegisterControlMethods(Me)
		End Sub

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			If Not Me.IsPostBack Then
				Globals.DisablePostbackOnEnter(txtUsername)

				'// Register scripts
				Page.ClientScript.RegisterClientScriptInclude("LocalizationEditorGetUsernames", Me.ModulePath + "/Users.ascx.js")
				Page.ClientScript.RegisterClientScriptInclude("LocalizationEditorAutoSuggest", Me.ModulePath + "/AutoSuggest.js")

				'// Set clientid of username textbox
				ClientAPI.RegisterClientVariable(Me.Page, "UsernameInput", txtUsername.ClientID, True)

				ddObjects.DataSource = Data.DataProvider.Instance.GetAllObjects
				ddObjects.DataBind()
				BindData()
			End If
		End Sub

		Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
			Dim user As UserInfo = UserController.GetUserByName(PortalId, txtUsername.Text.Trim)
			If user Is Nothing Then
				lblError.Text = String.Format(GetString("UserNotFound", Me.LocalResourceFile), txtUsername.Text.Trim)
				Exit Sub
			End If
            Dim locale As String = txtLocale.Text.Trim

			Dim uperm As PermissionInfo = PermissionsController.GetPermission(CInt(ddObjects.SelectedValue), user.UserID, locale, ModuleId)
			If uperm Is Nothing Then
				uperm = New PermissionInfo(CInt(ddObjects.SelectedValue), user.UserID, locale, ModuleId)
				PermissionsController.AddPermission(uperm)
			End If

			'// Clear input fields
			txtUsername.Text = ""
			txtLocale.Text = ""
			BindData()
		End Sub

		Private Sub dlUserPermissions_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlUserPermissions.DeleteCommand
			Dim permissionId As Integer = CInt(dlUserPermissions.DataKeys(e.Item.ItemIndex))
			PermissionsController.DeletePermission(permissionId)
			BindData()
		End Sub

		Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
			Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
		End Sub
#End Region

#Region " Private Methods "
		Private Sub BindData()
			dlUserPermissions.DataSource = PermissionsController.GetPermissions(ModuleId)
			dlUserPermissions.DataBind()
		End Sub

		<ControlMethod()> _
		Public Function GetUsernamesByFilter(ByVal filter As String) As String
			If filter Is Nothing Then Return String.Empty

			'// Get a list of users filtered by the given filter string
			Dim filteredUsers As System.Data.DataSet = UsersController.GetUsersFiltered(filter)

			'// Store each username in a combined string
			Dim sb As New StringBuilder()
			For Each row As System.Data.DataRow In filteredUsers.Tables(0).Rows
                Dim username As String = CStr(row("Username"))
                Dim email As String = CStr(row("Email"))

                '// Html encode data
                username = HttpUtility.HtmlEncode(username)
                email = HttpUtility.HtmlEncode(email)

                sb.Append(username)
                sb.Append(",")

                '// Add highlight indicators
                username = Globals.StringReplace(username, filter, "[[" & filter & "]]", True)
                email = Globals.StringReplace(email, filter, "[[" & filter & "]]", True)

                sb.Append(username)
                sb.Append(",")
                sb.Append(email)
                sb.Append("/")
            Next

			'// Return the matched usernames
			Return sb.ToString().TrimEnd("/"c)
		End Function
#End Region

	End Class
End Namespace
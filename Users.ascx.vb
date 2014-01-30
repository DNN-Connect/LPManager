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
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Common
Imports DNNEurope.Modules.LocalizationEditor.Entities.Permissions

<ControlMethodClass("DNNEurope.Modules.LocalizationEditor.Users")> Partial Public Class Users
 Inherits ModuleBase

#Region " Event Handlers "

 Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  ClientAPI.HandleClientAPICallbackEvent(Me.Page)
  ClientAPI.RegisterControlMethods(Me)
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
  If Not IsAdmin Then
   Response.Redirect(AccessDeniedURL())
  End If

  If Not Me.IsPostBack Then
   Globals.DisablePostbackOnEnter(txtUsername)

   ' Register scripts
   Page.ClientScript.RegisterClientScriptInclude("LocalizationEditorGetUsernames", Me.ControlPath + "/js/Users.ascx.js")
   Page.ClientScript.RegisterClientScriptInclude("LocalizationEditorAutoSuggest", Me.ControlPath + "/js/AutoSuggest.js")

   ' Set clientid of username textbox
   ClientAPI.RegisterClientVariable(Me.Page, "UsernameInput", txtUsername.ClientID, True)

   BindData()
  End If
 End Sub

 Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click
  Dim user As UserInfo = UserController.GetUserByName(PortalId, txtUsername.Text.Trim)
  If user Is Nothing Then
   lblError.Text = String.Format(Localization.GetString("UserNotFound", Me.LocalResourceFile), txtUsername.Text.Trim)
   Exit Sub
  End If
  Dim locale As String = txtLocale.Text.Trim

  Dim uperm As Entities.Permissions.PermissionInfo = PermissionsController.GetPermission(user.UserID, locale, ModuleId)
  If uperm Is Nothing Then
   uperm = New Entities.Permissions.PermissionInfo(-1, ModuleId, locale, user.UserID)
   PermissionsController.AddPermission(uperm)
  End If

  'Rebind with same user and locale.
  BindData()
 End Sub

 Private Sub dlUserPermissions_DeleteCommand(ByVal source As Object, ByVal e As DataListCommandEventArgs) Handles dlUserPermissions.DeleteCommand
  Dim permissionId As Integer = CInt(dlUserPermissions.DataKeys(e.Item.ItemIndex))
  PermissionsController.DeletePermission(permissionId)
  BindData()
 End Sub

 Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReturn.Click
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
 End Sub

#End Region

#Region " Private Methods "

 Private Sub BindData()
  dlUserPermissions.DataSource = PermissionsController.GetPermissions(ModuleId)
  dlUserPermissions.DataBind()
 End Sub

 <ControlMethod()> Public Function GetUsernamesByFilter(ByVal filter As String) As String
  If filter Is Nothing Then Return String.Empty

  ' Get a list of users filtered by the given filter string
  Dim filteredUsers As DataSet = PermissionsController.GetUsersFiltered(filter)

  ' Store each username in a combined string
  Dim sb As New StringBuilder()
  For Each row As DataRow In filteredUsers.Tables(0).Rows
   Dim username As String = CStr(row("Username"))
   Dim email As String = CStr(row("Email"))

   ' Html encode data
   username = HttpUtility.HtmlEncode(username)
   email = HttpUtility.HtmlEncode(email)

   sb.Append(username)
   sb.Append(",")

   ' Add highlight indicators
   username = Globals.StringReplace(username, filter, "[[" & filter & "]]", True)
   email = Globals.StringReplace(email, filter, "[[" & filter & "]]", True)

   sb.Append(username)
   sb.Append(",")
   sb.Append(email)
   sb.Append("/")
  Next

  ' Return the matched usernames
  Return sb.ToString().TrimEnd("/"c)
 End Function

#End Region

End Class

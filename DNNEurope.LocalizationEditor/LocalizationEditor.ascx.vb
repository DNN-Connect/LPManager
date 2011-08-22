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
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Entities.Modules.Actions

Partial Public Class LocalizationEditor
 Inherits ModuleBase
 Implements DotNetNuke.Entities.Modules.IActionable

#Region " Private Members "

 Private _userId As Integer = UserId
 Private _userLocales As List(Of String)
 Private _allLocales As List(Of String)

#End Region

#Region " Properties "
 Public Property UserLocales As List(Of String)
  Get
   If _userLocales Is Nothing Then
    _userLocales = New List(Of String)
    Dim uid As Integer = UserId
    If UserInfo.IsSuperUser Then
     uid = PortalSettings.AdministratorId
    End If
    Using ir As IDataReader = DataProvider.Instance.GetLocalesForUser(uid, PortalId, ModuleId)
     Do While ir.Read
      _userLocales.Add(CStr(ir.Item("Locale")))
     Loop
    End Using
   End If
   Return _userLocales
  End Get
  Set(value As List(Of String))
   _userLocales = value
  End Set
 End Property

 Public Property AllLocales As List(Of String)
  Get
   If _allLocales Is Nothing Then
    _allLocales = Entities.Translations.TranslationsController.GetLocales(ModuleId)
   End If
   Return _allLocales
  End Get
  Set(ByVal value As List(Of String))
   _allLocales = value
  End Set
 End Property
#End Region

#Region " Event Handlers "

 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  If UserInfo.IsSuperUser Then
   _userId = PortalSettings.AdministratorId
  End If
 End Sub

 Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
  Try
   ' Show functions for authorized users
   cmdManagePermissions.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdManageObjects.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdManagePartners.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdClearCaches.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") And Me.Settings.CachePacks

   If Not Me.IsPostBack Then

    If Settings.AllowDataExtract AndAlso ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") Then
     hlCube.NavigateUrl = ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/GetCube.ashx") & "?pid=" & PortalId.ToString & "&mid=" & ModuleId.ToString
     hlCube.ToolTip = GetString("hlCube", LocalResourceFile)
     hlCube.Visible = True
    End If

    Me.DataBind()

   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdUploadPack_Click(sender As Object, e As System.EventArgs) Handles cmdUploadPack.Click
  Response.Redirect(EditUrl("UploadPack"))
 End Sub

 Protected Sub cmdManagePermissions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdManagePermissions.Click
  Response.Redirect(EditUrl("Users"))
 End Sub

 Private Sub cmdManagePartners_Click(sender As Object, e As System.EventArgs) Handles cmdManagePartners.Click
  Response.Redirect(EditUrl("Partners"))
 End Sub

 Protected Sub cmdManageObjects_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdManageObjects.Click
  Response.Redirect(EditUrl("ManageObjects"))
 End Sub

 Private Sub cmdClearCaches_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdClearCaches.Click
  Dim packPath As String = PortalSettings.HomeDirectoryMapPath & "\LocalizationEditor\Cache\" & ModuleId.ToString & "\"
  If IO.Directory.Exists(packPath) Then
   Dim zipFiles() As String = IO.Directory.GetFiles(packPath, "*.zip")
   For Each f As String In zipFiles
    Try
     IO.File.Delete(f)
    Catch
    End Try
   Next
  End If
 End Sub
#End Region

#Region " Public Methods "
 Public Function Localeheaders() As String
  Dim res As String = ""
  For Each Loc As String In AllLocales
   res &= String.Format("<th>{0}</th>", Loc)
  Next
  Return res
 End Function

 Public Function GetObjectLocales(ByVal r As Object) As String
  Dim record As System.Data.Common.DbDataRecord = CType(CType(r, DataListItem).DataItem, System.Data.Common.DbDataRecord)
  Dim res As New StringBuilder
  For Each l As String In AllLocales
   res.Append("<td>")
   If UserLocales.Contains(l) Then
    res.AppendFormat("<a href=""{0}"" class=""CommandButton"">", EditUrl("ObjectId", CStr(record.Item("ObjectId")), "ObjectSummary", "Locale=" & l))
   End If
   Dim perct As Double = CInt(record.Item(l.Replace("-", ""))) * 100 / CInt(record.Item("LastVersionTextCount"))
   'Dim pperct As Double = Globals.GetADouble(record.Item("PartnerComplete"))
   res.AppendFormat("{0} %", Math.Round(perct))
   'If perct < pperct Then
   ' res.AppendFormat(" ({0} %)", Math.Round(pperct))
   'End If
   If UserLocales.Contains(l) Then
    res.Append("</a>")
   End If
   res.AppendLine("</td>")
  Next
  Return res.ToString
 End Function

 Public Function GetObjectUrl(ByVal objectId As Integer) As String
  Return EditUrl("ObjectId", objectId.ToString, "DownloadPack")
 End Function

#End Region

#Region " Overrides "

 Public Overrides Sub DataBind()

  dlObjects.DataSource = DataProvider.Instance.GetObjectsWithStatus(ModuleId)
  dlObjects.DataBind()
  If dlObjects.Items.Count = 0 Then
   pnlEdit.Visible = False
  End If
  cmdUploadPack.Visible = CBool(UserLocales.Count > 0)

 End Sub

#End Region

#Region " IActionable "
 Public ReadOnly Property ModuleActions As ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
  Get
   Dim modActions As New DotNetNuke.Entities.Modules.Actions.ModuleActionCollection
   modActions.Add(GetNextActionID, GetString("lbManageObjects", Me.LocalResourceFile), ModuleActionType.ContentOptions, "", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/images/file_16.png"), EditUrl("ManageObjects"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   modActions.Add(GetNextActionID, GetString("lbManagePermissions", Me.LocalResourceFile), ModuleActionType.ContentOptions, "", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/images/user_16.png"), EditUrl("Users"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   modActions.Add(GetNextActionID, GetString("lbManagePartners", Me.LocalResourceFile), ModuleActionType.ContentOptions, "", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/images/clients_16.png"), EditUrl("Partners"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
   If UserLocales.Count > 0 Then
    modActions.Add(GetNextActionID, GetString("lbUploadPack", Me.LocalResourceFile), ModuleActionType.ContentOptions, "", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/images/up_16.png"), EditUrl("UploadPack"), False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   End If
   If Settings.AllowDataExtract Then
    modActions.Add(GetNextActionID, GetString("hlCube", Me.LocalResourceFile), ModuleActionType.ContentOptions, "", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/images/network_connector_16.png"), ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/GetCube.ashx") & "?pid=" & PortalId.ToString & "&mid=" & ModuleId.ToString, False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   End If
   Return modActions
  End Get
 End Property
#End Region

End Class

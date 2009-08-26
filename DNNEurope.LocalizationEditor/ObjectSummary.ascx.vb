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
Imports DNNEurope.Modules.LocalizationEditor.Business
Imports DotNetNuke.Services.Localization


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
 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

  Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
  Globals.ReadQuerystringValue(Me.Request.Params, "Locale", Locale)

  Dim objObjectInfo As ObjectInfo = ObjectController.GetObject(ObjectId)
  If objObjectInfo Is Nothing Then Return
  ModuleFriendlyName = objObjectInfo.FriendlyName

  ' we no longer automatically read the latest version from the installed versions!
  'If Not Me.IsPostBack Then
  ' LocalizationController.ReadResourceFiles(Server.MapPath("~/"), PortalId, objObjectInfo, UserId)
  'End If
  Original = LocalizationController.GetObjectMetrics(ObjectId, "")
  Target = LocalizationController.GetObjectMetrics(ObjectId, Locale)

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then

   ' Permission check here
   If Not PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ModuleId, ObjectId, Locale) Then
    Throw New Exception("Access denied")
   End If

   ddSourceLocale.DataSource = DataProvider.Instance.GetLocalesForUserObject(ObjectId, PortalSettings.AdministratorId, PortalId, ModuleId)
   ddSourceLocale.DataBind()
   ddSourceLocale.Items.Insert(0, New ListItem(Localization.GetString("NoSource", Me.LocalResourceFile), ""))

   ddVersion.DataSource = DataProvider.Instance.GetVersions(ObjectId)
   ddVersion.DataBind()
   Try
    ddVersion.Items.FindByValue(Original.CurrentVersion).Selected = True
   Catch
   End Try

   ddSelection.DataSource = DataProvider.Instance().GetFiles(ObjectId, Original.CurrentVersion)
   ddSelection.DataBind()
   ddSelection.Items.Insert(0, New ListItem(Localization.GetString("All", Me.LocalResourceFile), "All"))
   ddSelection.Items.Insert(0, New ListItem(Localization.GetString("New", Me.LocalResourceFile), "New"))
   ddSelection.Items.Insert(0, New ListItem(Localization.GetString("Untranslated", Me.LocalResourceFile), "Untranslated"))

   cmdDownload.NavigateUrl = EditUrl("ObjectId", ObjectId.ToString, "DownloadPack")
   cmdUpload.NavigateUrl = EditUrl("ObjectId", ObjectId.ToString, "UploadPack", "Locale=" & Locale)
   cmdReturn.NavigateUrl = DotNetNuke.Common.NavigateURL

  End If

 End Sub

 Private Sub cmdEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdEdit.Click
  Dim url As String = ""
  If ddSourceLocale.SelectedValue = "" Then
   url = EditUrl("ObjectId", ObjectId.ToString, "Edit", "Locale=" & Locale.ToString, "Version=" & ddVersion.SelectedValue, "Selection=" & ddSelection.SelectedValue)
  Else
   url = EditUrl("ObjectId", ObjectId.ToString, "Edit", "Locale=" & Locale.ToString, "Version=" & ddVersion.SelectedValue, "SourceLocale=" & ddSourceLocale.SelectedValue, "Selection=" & ddSelection.SelectedValue)
  End If
  Me.Response.Redirect(url, False)
 End Sub

#End Region

End Class

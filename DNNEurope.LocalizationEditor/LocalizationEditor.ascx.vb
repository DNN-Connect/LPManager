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
 Public Property Locale As String = ""
 Public Property IsEditorSpecificLocale As Boolean = False
 Public Property IsEditorGenericLocale As Boolean = False

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
  Globals.ReadValue(Me.Request.Params, "Locale", Locale)
  If UserInfo.IsSuperUser Then
   _userId = PortalSettings.AdministratorId
  End If
  IsEditorSpecificLocale = UserLocales.Contains(Locale)
  IsEditorGenericLocale = UserLocales.Contains(Left(Locale, 2))
 End Sub

 Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
  Try
   ' Show functions for authorized users
   cmdManagePermissions.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdManageObjects.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdManagePartners.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")
   cmdClearCaches.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") And Me.Settings.CachePacks
   cmdUploadPack.Visible = ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT")

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
 Public Function GetObjectLocalePerctComplete(ByVal r As Object) As String
  'Dim record As System.Data.Common.DbDataRecord = CType(CType(r, DataListItem).DataItem, System.Data.Common.DbDataRecord)
  Dim record As DataRowView = CType(r, DataRowView)
  Try
   Dim perct As Double = CInt(record.Item("TextCount")) * 100
   If CInt(record.Item("LastVersionTextCount")) = 0 Then Return ""
   perct = perct / CInt(record.Item("LastVersionTextCount"))
   Return String.Format("{0} %", Math.Round(perct))
  Catch ex As Exception
  End Try
  Return ""
 End Function

 Public Function GetEditColumn(ByVal r As Object) As String
  Dim record As DataRowView = CType(r, DataRowView)
  Dim res As String = ""
  If IsEditorSpecificLocale Then
   res = String.Format("<a href=""{0}"" class=""CommandButton"" title=""{2}""><img src=""{1}"" border=""0"" alt=""{2}"" /></a>", EditUrl("ObjectId", CStr(record.Item("ObjectId")), "ObjectSummary", "Locale=" & Locale, "Version=" & CStr(record.Item("LastVersion"))), ResolveUrl("~/images/edit_pen.gif"), String.Format(GetString("Edit", LocalResourceFile), Locale))
  End If
  If IsEditorGenericLocale Then
   res &= String.Format("<a href=""{0}"" class=""CommandButton"" title=""{2}""><img src=""{1}"" border=""0"" alt=""{2}"" /></a>", EditUrl("ObjectId", CStr(record.Item("ObjectId")), "ObjectSummary", "Locale=" & Left(Locale, 2), "Version=" & CStr(record.Item("LastVersion"))), ResolveUrl("~/images/edit_pen.gif"), String.Format(GetString("Edit", LocalResourceFile), Left(Locale, 2)))
  End If
  Return res
 End Function

 Public Function GetObjectUrl(ByVal objectId As Integer) As String
  Return EditUrl("ObjectId", objectId.ToString, "DownloadPack", "Locale=" & Locale)
 End Function

#End Region

#Region " Private Methods "
 Private Function ListCacheKey() As String
  Return String.Format("LocList{0}", ModuleId)
 End Function
#End Region

#Region " Overrides "

 Public Overrides Sub DataBind()

  If _locale = "" Then
   Dim locList As String = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(ListCacheKey), String)
   If locList Is Nothing Then
    Dim llb As New StringBuilder
    Dim ourUrl As String = DotNetNuke.Common.NavigateURL(TabId)
    If ourUrl.Contains("?") Then
     ourUrl &= "&"
    Else
     ourUrl &= "?"
    End If
    ourUrl &= "locale="
    ' top level of generic cultures
    Dim gLocs As New List(Of String)
    For Each l As String In AllLocales
     If Not gLocs.Contains(Left(l, 2)) Then
      gLocs.Add(Left(l, 2))
     End If
    Next
    Dim genericLocales As New SortedDictionary(Of String, Globalization.CultureInfo)
    For Each c As Globalization.CultureInfo In Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.NeutralCultures)
     If gLocs.Contains(c.Name) Then
      genericLocales.Add(c.NativeName, c)
     End If
    Next
    Dim handledLocales As New List(Of String)
    For Each gl As String In genericLocales.Keys
     handledLocales.Add(genericLocales(gl).Name)
     llb.AppendFormat("<div class=""genericLocale"">{0} ({1})</div>", gl, genericLocales(gl).EnglishName)
     If AllLocales.Contains(genericLocales(gl).Name) Then ' we have the generic variant in our system
      For Each c As Globalization.CultureInfo In Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.SpecificCultures)
       If Left(c.Name, 2) = genericLocales(gl).Name Then
        llb.AppendFormat("<div class=""specificLocale""><a href=""{0}{1}"">{2}</a></div>", ourUrl, c.Name, c.NativeName)
        handledLocales.Add(c.Name)
       End If
      Next
     Else ' we just have a specific locale here
      For Each l As String In AllLocales
       If Left(l, 2) = genericLocales(gl).Name Then
        Dim c As New Globalization.CultureInfo(l)
        llb.AppendFormat("<div class=""specificLocale""><a href=""{0}{1}"">{2}</a></div>", ourUrl, c.Name, c.NativeName)
        handledLocales.Add(l)
       End If
      Next
     End If
    Next
    ' Now for any overlooked specific locales
    Dim overlookedLocales As New List(Of String)
    For Each l As String In AllLocales
     If Not handledLocales.Contains(l) Then
      overlookedLocales.Add(l)
     End If
    Next
    If overlookedLocales.Count > 0 Then
     llb.AppendFormat("<div class=""genericLocale"">{0}</div>", GetString("Other", LocalResourceFile))
     For Each l As String In overlookedLocales
      Dim c As New Globalization.CultureInfo(l)
      llb.AppendFormat("<div class=""specificLocale""><a href=""{0}{1}"">{2}</a></div>", ourUrl, c.Name, c.NativeName)
     Next
    End If
    locList = llb.ToString
   End If
   DotNetNuke.Common.Utilities.DataCache.SetCache(ListCacheKey, locList, Now.AddMinutes(30))
   plhLocales.Controls.Add(New LiteralControl(locList))
   pnlLocaleRequest.Visible = False
  Else
   pnlLocaleRequest.Visible = True
   Dim oList As DataTable = DotNetNuke.Common.ConvertDataReaderToDataTable(DataProvider.Instance.GetObjectsWithStatus(ModuleId, Locale))
   If IsEditorGenericLocale Or IsEditorSpecificLocale Then
    dlObjects.DataSource = New DataView(oList, "PackageType<>'Pack'", "FriendlyName", DataViewRowState.CurrentRows)
   Else
    dlObjects.DataSource = New DataView(oList, "PackageType<>'Pack' And TextCount>0", "FriendlyName", DataViewRowState.CurrentRows)
   End If
   dlObjects.DataBind()
   dlPackages.DataSource = New DataView(oList, "PackageType='Pack' And ChildCount>0", "FriendlyName", DataViewRowState.CurrentRows)
   dlPackages.DataBind()
   If dlObjects.Items.Count = 0 Then
    pnlLocaleRequest.Visible = False
   End If
  End If

  cmdUploadPack.Visible = cmdUploadPack.Visible Or CBool(UserLocales.Count > 0)

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

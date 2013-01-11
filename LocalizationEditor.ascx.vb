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

#Region " Private Members "

 Private _userId As Integer = UserId
 Private _userLocales As List(Of String)
 Private _allLocales As List(Of String)

#End Region

#Region " Properties "
 Public Property Locale As String = ""
 Public Property IsEditorSpecificLocale As Boolean = False
 Public Property IsEditorGenericLocale As Boolean = False
 Public Property IsEditor As Boolean = False
 Public Property IsAdmin As Boolean = False

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
    For Each l As String In UserLocales
     If Not _allLocales.Contains(l) Then
      _allLocales.Add(l)
     End If
    Next
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
   IsAdmin = True
  End If
  If ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") Then
   IsAdmin = True
  End If
  IsEditorSpecificLocale = UserLocales.Contains(Locale)
  IsEditorGenericLocale = UserLocales.Contains(Left(Locale, 2))
  IsEditor = CBool(UserLocales.Count > 0)

  cmdUploadPack.ToolTip = LocalizeString("lbUploadPack")
  cmdManageObjects.ToolTip = LocalizeString("lbManageObjects")
  cmdManagePermissions.ToolTip = LocalizeString("lbManagePermissions")
  cmdManagePartners.ToolTip = LocalizeString("lbManagePartners")
  cmdClearCaches.ToolTip = LocalizeString("lbClearCaches")
  cmdCube.ToolTip = LocalizeString("lbCube")
  cmdService.ToolTip = LocalizeString("lbService")
 End Sub

 Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
  Try
   ' Show functions for authorized users
   cmdManagePermissions.Visible = IsAdmin
   cmdManageObjects.Visible = IsAdmin
   cmdManagePartners.Visible = IsAdmin
   cmdClearCaches.Visible = IsAdmin And Me.Settings.CachePacks
   cmdUploadPack.Visible = IsAdmin Or IsEditor

   If Not Me.IsPostBack Then

    Me.DataBind()

   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdCube_Click(sender As Object, e As System.EventArgs) Handles cmdCube.Click
  Response.Redirect(ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/GetCube.ashx") & "?pid=" & PortalId.ToString & "&mid=" & ModuleId.ToString, False)
 End Sub

 Private Sub cmdService_Click(sender As Object, e As System.EventArgs) Handles cmdService.Click
  Response.Redirect(ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/API") & "?tabid=" & TabId.ToString & "&moduleid=" & ModuleId.ToString, False)
 End Sub

 Private Sub cmdUploadPack_Click(sender As Object, e As System.EventArgs) Handles cmdUploadPack.Click
  Response.Redirect(EditUrl("UploadPack"), False)
 End Sub

 Protected Sub cmdManagePermissions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdManagePermissions.Click
  Response.Redirect(EditUrl("Users"), False)
 End Sub

 Private Sub cmdManagePartners_Click(sender As Object, e As System.EventArgs) Handles cmdManagePartners.Click
  Response.Redirect(EditUrl("Partners"), False)
 End Sub

 Protected Sub cmdManageObjects_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdManageObjects.Click
  Response.Redirect(EditUrl("ManageObjects"), False)
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
  If IsAdmin Or IsEditorSpecificLocale Then
   res = String.Format("<a href=""{0}"" title=""{2}"" class=""iconLink""><span class=""entypoIcon icon16"" title=""{2}"">{1}</span></a>", EditUrl("ObjectId", CStr(record.Item("ObjectId")), "ObjectSummary", "Locale=" & Locale, "Version=" & CStr(record.Item("LastVersion"))), "&#9998;", String.Format(GetString("Edit", LocalResourceFile), Locale))
  End If
  If IsAdmin Or IsEditorGenericLocale Then
   res &= String.Format("<a href=""{0}"" title=""{2}""class=""iconLink""><span class=""entypoIcon icon16"" title=""{2}"">{1}</span></a>", EditUrl("ObjectId", CStr(record.Item("ObjectId")), "ObjectSummary", "Locale=" & Left(Locale, 2), "Version=" & CStr(record.Item("LastVersion"))), "&#9998;", String.Format(GetString("Edit", LocalResourceFile), Left(Locale, 2)))
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

  If _Locale = "" Then
   Dim locList As String = Nothing
   If Not IsEditor Then
    locList = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(ListCacheKey), String)
   End If
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
     llb.AppendFormat("<div class=""genericLocale"">{0} ({1})</div>", CultureTextInfo.ToTitleCase(gl), CultureTextInfo.ToTitleCase(genericLocales(gl).EnglishName))
     If AllLocales.Contains(genericLocales(gl).Name) Then ' we have the generic variant in our system
      For Each c As Globalization.CultureInfo In Globalization.CultureInfo.GetCultures(Globalization.CultureTypes.SpecificCultures)
       If Left(c.Name, 2) = genericLocales(gl).Name Then
        llb.AppendFormat("<div class=""specificLocale""><a href=""{0}{1}"">{2}</a></div>", ourUrl, c.Name, CultureTextInfo.ToTitleCase(c.NativeName))
        handledLocales.Add(c.Name)
       End If
      Next
     Else ' we just have a specific locale here
      For Each l As String In AllLocales
       If Left(l, 2) = genericLocales(gl).Name Then
        Dim c As New Globalization.CultureInfo(l)
        llb.AppendFormat("<div class=""specificLocale""><a href=""{0}{1}"">{2}</a></div>", ourUrl, c.Name, CultureTextInfo.ToTitleCase(c.NativeName))
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
   If Not IsEditor Then
    DotNetNuke.Common.Utilities.DataCache.SetCache(ListCacheKey, locList)
   End If
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
   dlCorePackages.DataSource = New DataView(oList, "PackageType='Pack' And ChildCount>0 And PercentComplete>0 And (ObjectName='DNNCE' Or ObjectName='DNNPE' Or ObjectName='DNNEE')", "FriendlyName", DataViewRowState.CurrentRows)
   dlCorePackages.DataBind()
   If dlCorePackages.Items.Count < 1 Then pnlCorePackages.Visible = False
   dlPackages.DataSource = New DataView(oList, "PackageType='Pack' And ChildCount>0 And PercentComplete>0 And Not (ObjectName='DNNCE' Or ObjectName='DNNPE' Or ObjectName='DNNEE')", "FriendlyName", DataViewRowState.CurrentRows)
   dlPackages.DataBind()
   If dlPackages.Items.Count < 1 Then pnlPackages.Visible = False
   If dlObjects.Items.Count = 0 Then
    pnlLocaleRequest.Visible = False
   End If
  End If

 End Sub

#End Region

End Class

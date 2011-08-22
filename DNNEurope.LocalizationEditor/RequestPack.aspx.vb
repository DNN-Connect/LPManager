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
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Entities.Permissions
Imports DNNEurope.Modules.LocalizationEditor.Entities.Texts
Imports System.Globalization


Partial Public Class RequestPack
 Inherits PageBase

#Region " Private Members "
#End Region

#Region " Properties "
 Public Property ModuleKey As String = ""
 Public Property Objectname As String = ""
 Public Property Version As String = ""
 Public Property ObjectId As Integer = -1
 Public Property FriendlyName As String = ""
 Public Property TotalItems As Integer = -1
 Public Property HasPartnerPacks As Boolean = False
#End Region

#Region " Event Handlers "
 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

  pnlDisambiguate.Visible = False
  Globals.ReadValue(Me.Request.Params, "Key", ModuleKey)
  Globals.ReadValue(Me.Request.Params, "Object", Objectname)
  Globals.ReadValue(Me.Request.Params, "Version", Version)
  If Objectname = "" Then
   Globals.ReadValue(Me.Request.Params, "ObjectId", ObjectId)
   Dim tm As ObjectInfo = ObjectsController.GetObject(ObjectId)
   Objectname = tm.ObjectName
   FriendlyName = tm.FriendlyName
  Else
   If ModuleKey = "" Then
    Dim foundObjects As List(Of ObjectInfo) = ObjectsController.GetObjectsByObjectName(Objectname)
    If foundObjects.Count = 1 Then
     Dim tm As ObjectInfo = foundObjects(0)
     ObjectId = tm.ObjectId
     Objectname = tm.ObjectName
     FriendlyName = tm.FriendlyName
    ElseIf foundObjects.Count = 0 Then
     ' Didn't find any objects by that name
     pnlMain.Visible = False
     lblError.Text = Localization.GetString("ObjectNotFound.Error", LocalResourceFile)
    Else
     ' Found more than one objects by that name
     pnlMain.Visible = False
     pnlDisambiguate.Visible = True
     For Each o As ObjectInfo In foundObjects
      FriendlyName = o.FriendlyName
      Dim l As String = ""
      For Each p As PermissionInfo In PermissionsController.GetPermissions(o.ModuleId)
       l &= p.Locale & ", "
      Next
      l = l.Trim.TrimEnd(","c)
      plhDisambiguate.Controls.Add(New LiteralControl(String.Format("<p><a href=""{0}&ObjectId={1}&Version={2}"">{3}</a></p>", ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/RequestPack.aspx"), o.ObjectId, Version, l)))
     Next
    End If
   Else
    Dim o As ObjectInfo = ObjectsController.GetObjectByObjectNameAndModuleKey(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, Objectname, ModuleKey)
    If o Is Nothing Then
     lblError.Text = "NOT FOUND"
     Exit Sub
    End If
    ObjectId = o.ObjectId
    Objectname = o.ObjectName
    FriendlyName = o.FriendlyName
   End If
  End If
  If Not Me.IsPostBack Then
   ddVersion.DataSource = DataProvider.Instance.GetVersions(Me.ObjectId)
   ddVersion.DataBind()
  End If
  Try
   If String.IsNullOrEmpty(Version) Then
    Version = ddVersion.Items(ddVersion.Items.Count - 1).Text
   End If
   If ddVersion.Items.FindByText(Version) Is Nothing Then
    Dim v As String = ddVersion.Items(ddVersion.Items.Count - 1).Text
    For Each itm As ListItem In ddVersion.Items
     If itm.Text > Version Then Exit For
     v = itm.Text
    Next
    ddVersion.Items.FindByText(v).Selected = True
   Else
    ddVersion.Items.FindByText(Version).Selected = True
   End If
  Catch
  End Try
  TotalItems = TextsController.NrOfItems(ObjectId, Version)
  Localization.LocalizeDataGrid(dgLocales, Me.LocalResourceFile)

  ' Hide all if there are not items and show message to user
  If TotalItems = 0 Then
   pnlTranslations.Visible = False
   lblNoResourceFiles.Visible = True
  End If
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   Dim dt As DataTable = DotNetNuke.Common.ConvertDataReaderToDataTable(DataProvider.Instance.GetLanguagePacks(ObjectId, Version))
   dgLocales.DataSource = dt
   dgLocales.DataBind()
  End If

 End Sub

 Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
  dgLocales.Columns(2).Visible = HasPartnerPacks
 End Sub

 Private Sub ddVersion_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddVersion.SelectedIndexChanged
  Me.Response.Redirect(ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/RequestPack.aspx") & "?ObjectId=" & ObjectId.ToString & "&Version=" & ddVersion.SelectedValue, False)
 End Sub

#End Region

#Region " Public Methods "
 Public Function DownloadPackList(ByVal packUrl As String, ByVal objectId As Integer, ByVal remoteObjectId As Integer, ByVal locale As String, ByVal version As String) As String
  Dim o As ObjectInfo = ObjectsController.GetObject(objectId)
  If packUrl = "" Then
   packUrl = Globals.PackUrl
  Else
   objectId = remoteObjectId
   HasPartnerPacks = True
  End If
  If o.IsCore Then
   Dim res1 As String = "Core: "
   Dim res2 As String = "Full: "
   Dim i As Integer = 1
   For Each drv As DataRowView In GetLocalesByCode(locale)
    res1 &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}&Type=Core"">{2}</a>&nbsp;", packUrl, objectId, drv.Item("Locale"), version)
    res2 &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}&Type=Full"">{2}</a>&nbsp;", packUrl, objectId, drv.Item("Locale"), version)
    If i Mod 4 = 0 Then
     res1 &= "<br />"
     res2 &= "<br />"
    End If
    i += 1
   Next
   Return String.Format("{0}<br />{1}", res1, res2)
  Else
   Dim res As String = ""
   Dim i As Integer = 1
   For Each drv As DataRowView In GetLocalesByCode(locale)
    res &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}"">{2}</a>&nbsp;", packUrl, objectId, drv.Item("Locale"), version)
    If i Mod 4 = 0 Then
     res &= "<br />"
    End If
    i += 1
   Next
   Return res
  End If
 End Function
#End Region

#Region " Private Methods "
 Private _locales As DataTable
 Private ReadOnly Property Locales() As DataTable
  Get
   If _locales Is Nothing Then
    _locales = New DataTable("Locales")
    _locales.Columns.Add("Locale", GetType(String))
    For Each c As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
     Dim dr As DataRow = _locales.NewRow
     dr.Item("Locale") = c.Name
     _locales.Rows.Add(dr)
    Next
   End If
   Return _locales
  End Get
 End Property

 Private Function GetLocalesByCode(ByVal code As String) As DataView
  Return New DataView(Locales, "Locale LIKE '" & code & "*'", "Locale", DataViewRowState.CurrentRows)
 End Function
#End Region

End Class

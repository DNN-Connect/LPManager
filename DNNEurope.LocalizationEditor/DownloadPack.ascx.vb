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
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports System.Globalization

Partial Public Class DownloadPack
 Inherits ModuleBase

#Region " Private Members "

 Private _ObjectId As Integer = -1
 Private _objectname As String = ""
 Private _version As String = ""
 Private _friendlyName As String = ""
 Private _totalItems As Integer = -1

#End Region

#Region " Properties "

 Public Property Objectname() As String
  Get
   Return _objectname
  End Get
  Set(ByVal value As String)
   _objectname = value
  End Set
 End Property

 Public Property Version() As String
  Get
   Return _version
  End Get
  Set(ByVal value As String)
   _version = value
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

 Public Property FriendlyName() As String
  Get
   Return _friendlyName
  End Get
  Set(ByVal value As String)
   _friendlyName = value
  End Set
 End Property

 Public Property TotalItems() As Integer
  Get
   Return _totalItems
  End Get
  Set(ByVal value As Integer)
   _totalItems = value
  End Set
 End Property

#End Region

#Region " Event Handlers "

 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  DNNClientAPI.AddBodyOnloadEventHandler(Me.Page, "")

  Globals.ReadQuerystringValue(Me.Request.Params, "Object", Objectname)
  Globals.ReadQuerystringValue(Me.Request.Params, "Version", Version)
  If Objectname = "" Then
   Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
   Dim tm As ObjectInfo = ObjectController.GetObject(ObjectId)
   Objectname = tm.ObjectName
   FriendlyName = tm.FriendlyName
  Else
   Dim tm As ObjectInfo = ObjectController.GetObjectByObjectName(ModuleId, Objectname)
   Objectname = tm.ObjectName
   FriendlyName = tm.FriendlyName
  End If
  If Not Me.IsPostBack Then
   ddVersion.DataSource = DataProvider.Instance.GetVersions(Me.ObjectId)
   ddVersion.DataBind()
  End If
  Try
   If String.IsNullOrEmpty(Version) Then
    Version = ddVersion.Items(ddVersion.Items.Count - 1).Text
   End If
   ddVersion.Items.FindByText(Version).Selected = True
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
   dt.Columns.Add(New DataColumn("MissingTranslations", GetType(Integer)))
   dt.Columns.Add(New DataColumn("PercentComplete", GetType(Double)))
   For Each dr As DataRow In dt.Rows
    Dim missing As Integer = TextsController.NrOfMissingTranslations(ObjectId, CStr(dr.Item("Locale")), CStr(dr.Item("Version")))
    dr.Item("MissingTranslations") = missing
    dr.Item("PercentComplete") = ((TotalItems - missing) * 100) / TotalItems
   Next
   Dim dv As New DataView(dt)
   dv.Sort = "Locale"
   dgLocales.DataSource = dv
   dgLocales.DataBind()

   cmdReturn.NavigateUrl = DotNetNuke.Common.NavigateURL
  End If

 End Sub

 Private Sub ddVersion_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddVersion.SelectedIndexChanged
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "DownloadPack", "&Version=" & ddVersion.SelectedValue))
 End Sub

 Private Sub cmdDownload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDownload.Click

  Dim locale As String = txtLocale.Text
  If locale.Length > 2 Then
   locale = LCase(Left(locale, 2)) & "-" & UCase(Mid(locale, 4))
  Else
   locale = LCase(Left(locale, 2))
  End If
  Dim url As String = ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")
  url &= "?ObjectId=" & ObjectId
  url &= "&Locale=" & locale
  url &= "&Version=" & ddVersion.SelectedValue
  Me.Response.Redirect(url, False)

 End Sub

#End Region

#Region " Public Methods "
 Public Function DownloadPackList(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As String
  Dim o As ObjectInfo = ObjectController.GetObject(ObjectId)
  Dim packPath As String = ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")
  If o.IsCore Then
   Dim res1 As String = "Core: "
   Dim res2 As String = "Full: "
   For Each drv As DataRowView In GetLocalesByCode(Locale)
    res1 &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}&Type=Core"">{2}</a>&nbsp;", packPath, ObjectId, drv.Item("Locale"), Version)
    res2 &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}&Type=Full"">{2}</a>&nbsp;", packPath, ObjectId, drv.Item("Locale"), Version)
   Next
   Return String.Format("{0}<br />{1}", res1, res2)
  Else
   Dim res As String = ""
   For Each drv As DataRowView In GetLocalesByCode(Locale)
    res &= String.Format("<a href=""{0}?ObjectId={1}&Locale={2}&Version={3}"">{2}</a>&nbsp;", packPath, ObjectId, drv.Item("Locale"), Version)
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
    For Each culture As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
     Dim dr As DataRow = _locales.NewRow
     dr.Item("Locale") = culture.Name
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

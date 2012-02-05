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
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Entities.Texts

Partial Public Class DownloadPack
 Inherits ModuleBase

#Region " Private Members "
#End Region

#Region " Properties "
 Public Property Objectname As String = ""
 Public Property Version As String = ""
 Public Property ObjectId As Integer = -1
 Public Property FriendlyName As String = ""
 Public Property TotalItems As Integer = 0
 Public Property HasPartnerPacks As Boolean = False
 Public Property Component As ObjectInfo = Nothing
 Public Property Locale As String = ""
#End Region

#Region " Event Handlers "
 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

  Globals.ReadValue(Me.Request.Params, "Object", Objectname)
  Globals.ReadValue(Me.Request.Params, "Version", Version)
  Globals.ReadValue(Me.Request.Params, "Locale", Locale)
  If Objectname = "" Then
   Globals.ReadValue(Me.Request.Params, "ObjectId", ObjectId)
   Component = ObjectsController.GetObject(ObjectId)
   Objectname = Component.ObjectName
   FriendlyName = Component.FriendlyName
  Else
   Component = ObjectsController.GetObjectByObjectName(ModuleId, Objectname)
   Objectname = Component.ObjectName
   FriendlyName = Component.FriendlyName
   ObjectId = Component.ObjectId
  End If

  If Locale = "" Then
   pnlVersions.Visible = False
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
  Else ' we already have a locale
   pnlTranslations.Visible = False
   pnlVersions.Visible = True
   Localization.LocalizeDataGrid(dgVersions, Me.LocalResourceFile)
  End If

 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

  If Not Me.IsPostBack Then
   If Locale = "" Then
    Dim dt As DataTable = DotNetNuke.Common.ConvertDataReaderToDataTable(DataProvider.Instance.GetLanguagePacks(ObjectId, Version))
    dgLocales.DataSource = dt
    dgLocales.DataBind()
    cmdReturn.NavigateUrl = DotNetNuke.Common.NavigateURL
   Else
    dgVersions.DataSource = DataProvider.Instance.GetObjectVersionList(ObjectId, Locale)
    dgVersions.DataBind()
    cmdReturn.NavigateUrl = DotNetNuke.Common.NavigateURL("", "Locale=" & Locale)
   End If
  End If

 End Sub

 Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
  dgLocales.Columns(2).Visible = HasPartnerPacks
 End Sub

 Private Sub ddVersion_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddVersion.SelectedIndexChanged
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "DownloadPack", "&Version=" & ddVersion.SelectedValue))
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

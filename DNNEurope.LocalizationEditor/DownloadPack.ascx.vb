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

Namespace DNNEurope.Modules.LocalizationEditor
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
                Dim tm As ObjectInfo = ObjectController.GetObjectByObjectName(Objectname)
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

            '// Hide all if there are not items and show message to user
            If TotalItems = 0 Then
                pnlTranslations.Visible = False
                lblNoResourceFiles.Visible = True
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            If Not Me.IsPostBack Then
                Dim _
                    dt As DataTable = _
                        DotNetNuke.Common.ConvertDataReaderToDataTable( _
                                                                        DataProvider.Instance.GetLanguagePacks(ObjectId, _
                                                                                                                Version))
                dt.Columns.Add(New DataColumn("MissingTranslations", GetType(Integer)))
                dt.Columns.Add(New DataColumn("PercentComplete", GetType(Double)))
                For Each dr As DataRow In dt.Rows
                    Dim _
                        missing As Integer = _
                            TextsController.NrOfMissingTranslations(ObjectId, CStr(dr.Item("Locale")), _
                                                                     CStr(dr.Item("Version")))
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

        Private Sub ddVersion_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles ddVersion.SelectedIndexChanged
            Me.Response.Redirect( _
                                  EditUrl("ObjectId", ObjectId.ToString, "DownloadPack", _
                                           "&Version=" & ddVersion.SelectedValue))
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
    End Class
End Namespace
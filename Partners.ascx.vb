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
Public Class Partners
 Inherits ModuleBase

#Region " Event Handlers "
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  If Not Me.IsPostBack Then

   Me.ViewState("SortExpression") = "PartnerName"
   Me.ViewState("PageIndex") = 0

   cmdAdd.NavigateUrl = EditUrl("EditPartner")
   cmdBack.NavigateUrl = DotNetNuke.Common.NavigateURL
   BindList()

  End If

 End Sub

 Public Sub PartnerGridSort(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs)

  If Convert.ToString(Me.ViewState("SortExpression")) = e.SortExpression Then
   Me.ViewState("SortExpression") = e.SortExpression & " DESC"
  Else
   Me.ViewState("SortExpression") = e.SortExpression
  End If

  dgPartners.Controls.Clear()
  BindList()

 End Sub

 Public Sub PartnerGridPageCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs)

  Me.ViewState("PageIndex") = e.NewPageIndex
  dgPartners.Controls.Clear()
  BindList()

 End Sub
#End Region

#Region " Optional Interfaces "
#End Region

#Region " Private Methods "
 Private Sub BindList()
  DotNetNuke.Services.Localization.Localization.LocalizeDataGrid(dgPartners, Me.LocalResourceFile)

  Dim dv As New DataView(DotNetNuke.Common.ConvertDataReaderToDataTable(Data.DataProvider.Instance().GetPartnersByModule(ModuleId, 0, 1, CStr(Me.ViewState("SortExpression")))))

  With dgPartners
   .CurrentPageIndex = Convert.ToInt32(Me.ViewState("PageIndex"))
   .DataSource = dv
   .DataBind()
   If .AllowPaging Then
    If dv.Count <= .PageSize Then
     .AllowPaging = False
    Else
     .AllowPaging = True
    End If
   End If
  End With

 End Sub
#End Region

#Region " Public Methods "
#End Region

End Class
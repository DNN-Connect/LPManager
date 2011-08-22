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

Imports System
Imports System.Web.UI
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common
Imports DNNEurope.Modules.LocalizationEditor.Entities.Partners
Imports DNNEurope.Modules.LocalizationEditor.Services.DataExchange

Public Class EditPartner
 Inherits ModuleBase

#Region " Controls "
#End Region

#Region " Variables "
 Dim _partnerId As Int32 = -1
 Dim _partner As PartnerInfo = Nothing
 Dim _dl As Boolean = False
 Dim _override As Boolean = False
#End Region

#Region " Event Handlers "
 Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
  Globals.ReadValue(Request.Params, "PartnerId", _partnerId)
  Globals.ReadValue(Request.Params, "dl", _dl)
  Globals.ReadValue(Request.Params, "o", _override)
  If _partnerId <> -1 Then
   _partner = PartnersController.GetPartner(_partnerId)
   If _partner IsNot Nothing AndAlso _partner.ModuleId <> ModuleId Then
    _partner = Nothing
    Response.Redirect(AccessDeniedURL(), True)
   End If
  End If
  If _partnerId = -1 Then cmdUpdate.Enabled = False
 End Sub

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  If Not Page.IsPostBack Then
   DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, DotNetNuke.Services.Localization.Localization.GetString("Delete.Confirm", Me.LocalResourceFile))
   If _partnerId <> -1 Then
    pnlDetails.Visible = True
    tblCubeUrl.Visible = False
    If Not _partner Is Nothing Then
     'set screen
     With _partner
      chkAllowDirectDownload.Checked = .AllowDirectDownload
      chkAllowRedistribute.Checked = .AllowRedistribute
      txtCubeUrl.Text = .CubeUrl
      chkDownloadAffiliates.Checked = .DownloadAffiliates
      txtPackUrl.Text = .PackUrl
      txtPartnerName.Text = .PartnerName
      txtPartnerUrl.Text = .PartnerUrl
      txtPartnerUsername.Text = .PartnerUsername
     End With
    Else ' security violation attempt to access item not related to this Module
     Response.Redirect(AccessDeniedURL(), True)
    End If
   Else
    'Initialize New Record
    pnlDetails.Visible = False
    tblCubeUrl.Visible = True
   End If
  Else
   'its a postback
  End If

 End Sub

 Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
  Try
   ' Only Update if the Entered Data is Valid
   If Page.IsValid = True Then
    'bind text values to object
    With _partner
     'set values
     .AllowRedistribute = chkAllowRedistribute.Checked
     .DownloadAffiliates = chkDownloadAffiliates.Checked
     .PartnerUsername = txtPartnerUsername.Text
    End With
    PartnersController.UpdatePartner(_partner)
    Response.Redirect(EditUrl("Partners"), False)
   End If
  Catch exc As Exception 'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
  Try
   Response.Redirect(EditUrl("Partners"), False)
  Catch exc As Exception 'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
  Try
   If Not _partnerId = -1 Then
    PartnersController.DeletePartner(_partnerId)
   End If
   Response.Redirect(EditUrl("Partners"), False)
  Catch exc As Exception 'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Private Sub cmdDownload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDownload.Click

  If _partner Is Nothing Then
   _partner = New PartnerInfo
   With _partner
    .PartnerName = "New Partner"
    .CubeUrl = txtCubeUrl.Text
    .ModuleId = ModuleId
    _partnerId = PartnersController.AddPartner(_partner)
    .PartnerId = _partnerId
   End With
   cmdDownload.Enabled = False
   txtCubeUrl.Enabled = False
   cmdUpdate.Enabled = True
  End If

  EditUrl("PartnerId", _partnerId.ToString, "EditPartner")

  With _partner
   'set values
   .AllowRedistribute = chkAllowRedistribute.Checked
   .DownloadAffiliates = chkDownloadAffiliates.Checked
   .PartnerUsername = txtPartnerUsername.Text
  End With
  Dim fImport As New FeedImporter(_partner)
  fImport.ImportFeed(chkImportOverride.Checked)
  _partner = PartnersController.GetPartner(_partnerId)
  txtPartnerName.Text = _partner.PartnerName
  txtPartnerUrl.Text = _partner.PartnerUrl
  txtPackUrl.Text = _partner.PartnerUrl
  plhDownload.Controls.Add(New LiteralControl(fImport.Log.ToString))

 End Sub
#End Region

End Class
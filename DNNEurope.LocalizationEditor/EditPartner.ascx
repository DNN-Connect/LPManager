<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditPartner.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.EditPartner" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellSpacing="0" cellPadding="2" border="0" runat="server" id="tblCubeUrl">
 <tr>
  <td class="SubHead">
   <dnn:label id="plCubeUrl" runat="server" controlname="txtCubeUrl" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtCubeUrl" width="300" MaxLength="255" CssClass="NormalTextBox" runat="server" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plImportOverride" runat="server" controlname="chkImportOverride" suffix=":" />
  </td>
  <td>
   <asp:CheckBox ID="chkImportOverride" runat="server" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plDownload" runat="server" controlname="cmdDownload" suffix=":" />
  </td>
  <td>
   <asp:LinkButton id="cmdDownload" runat="server" CssClass="CommandButton" BorderStyle="none" Text="Download" ResourceKey="cmdDownload" />
  </td>
 </tr>
</table>

<asp:Panel runat="server" ID="pnlDetails">
<table cellSpacing="0" cellPadding="2" border="0">
 <tr>
  <td class="SubHead">
   <dnn:label id="plPartnerName" runat="server" controlname="txtPartnerName" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtPartnerName" width="300" MaxLength="100" CssClass="NormalTextBox" runat="server" Enabled="false" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plPartnerUrl" runat="server" controlname="txtPartnerUrl" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtPartnerUrl" width="300" MaxLength="300" CssClass="NormalTextBox" runat="server" Enabled="false" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plPackUrl" runat="server" controlname="txtPackUrl" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtPackUrl" width="300" MaxLength="255" CssClass="NormalTextBox" runat="server" Enabled="false" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plAllowDirectDownload" runat="server" controlname="chkAllowDirectDownload" suffix=":" />
  </td>
  <td>
   <asp:checkbox id="chkAllowDirectDownload" Runat="server" Enabled="false" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plPartnerUsername" runat="server" controlname="txtPartnerUsername" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtPartnerUsername" width="300" MaxLength="50" CssClass="NormalTextBox" runat="server" />
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plAllowRedistribute" runat="server" controlname="chkAllowRedistribute" suffix=":" />
  </td>
  <td>
<asp:checkbox id="chkAllowRedistribute" Runat="server" Text="Yes"></asp:checkbox>
  </td>
 </tr>
 <tr>
  <td class="SubHead">
   <dnn:label id="plDownloadAffiliates" runat="server" controlname="chkDownloadAffiliates" suffix=":" />
  </td>
  <td>
   <asp:checkbox id="chkDownloadAffiliates" Runat="server" Text="Yes"></asp:checkbox>
  </td>
 </tr>
</table>
</asp:Panel>

<p style="padding-top:30px;">
 <asp:linkbutton id="cmdUpdate" CssClass="CommandButton" runat="server" Text="Update" BorderStyle="none" ResourceKey="cmdUpdate" />&nbsp;
 <asp:linkbutton id="cmdCancel" CssClass="CommandButton" runat="server" Text="Cancel" BorderStyle="none" CausesValidation="False" ResourceKey="cmdCancel" />&nbsp;
 <asp:LinkButton id="cmdDelete" runat="server" CssClass="CommandButton" CausesValidation="False" BorderStyle="none" Text="Delete" ResourceKey="cmdDelete" />
</p>

<pre><asp:PlaceHolder runat="server" ID="plhDownload" /></pre>

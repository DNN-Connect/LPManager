<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0">
 <tr>
  <td colspan="2" class="NormalRed">
   <asp:Label runat="server" id="lblOwner" resourcekey="lblOwner" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plOwnerName" runat="server" controlname="txtOwnerName" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtOwnerName" Width="250" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plOwnerOrganization" runat="server" controlname="txtOwnerOrganization" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtOwnerOrganization" Width="250" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plOwnerEmail" runat="server" controlname="txtOwnerEmail" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtOwnerEmail" Width="250" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plOwnerUrl" runat="server" controlname="txtOwnerUrl" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtOwnerUrl" Width="250" />
  </td>
 </tr>
 <tr>
  <td colspan="2" class="NormalRed">
   <asp:Label runat="server" id="lblLicense" resourcekey="lblLicense" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plLicense" runat="server" controlname="txtLicense" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtLicense" Width="250" Height="300" TextMode="MultiLine" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plAttribution" runat="server" controlname="txtAttribution" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtAttribution" Width="250" Height="100" TextMode="MultiLine" />
  </td>
 </tr>
 <tr>
  <td colspan="2" class="NormalRed">
   <asp:Label runat="server" id="lblOther" resourcekey="lblOther" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plCachePacks" runat="server" controlname="chkCachePacks" suffix=":" />
  </td>
  <td>
   <asp:CheckBox runat="server" ID="chkCachePacks" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plAllowDirectDownload" runat="server" controlname="chkAllowDirectDownload" suffix=":" />
  </td>
  <td>
   <asp:CheckBox runat="server" ID="chkAllowDirectDownload" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plAllowDataExtract" runat="server" controlname="chkAllowDataExtract" suffix=":" />
  </td>
  <td>
   <asp:CheckBox runat="server" ID="chkAllowDataExtract" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plKeepStatistics" runat="server" controlname="chkKeepStatistics" suffix=":" />
  </td>
  <td>
   <asp:CheckBox runat="server" ID="chkKeepStatistics" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plModuleKey" runat="server" controlname="txtModuleKey" suffix=":" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtModuleKey" Width="250" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plAutoImportObjects" runat="server" controlname="chkAutoImportObjects" suffix=":" />
  </td>
  <td>
   <asp:CheckBox runat="server" ID="chkAutoImportObjects" />
  </td>
 </tr>
</table>

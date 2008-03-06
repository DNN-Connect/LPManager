<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LocalizationEditor.ascx.vb" Inherits="Bring2mind.DNN.Modules.LocalizationEditor.LocalizationEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0">
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plObjects" runat="server" controlname="ddObjects" suffix=":" />
  </td>
  <td>
   <asp:DropDownList runat="server" ID="ddObjects" DataTextField="Key" DataValueField="Value" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plSourceLocale" runat="server" controlname="ddSourceLocale" suffix=":" />
  </td>
  <td>
			<asp:dropdownlist id="ddSourceLocale" runat="server" DataTextField="Text" DataValueField="Code" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plLocales" runat="server" controlname="ddLocales" suffix=":" />
  </td>
  <td>
			<asp:dropdownlist id="ddLocales" runat="server" DataTextField="Text" DataValueField="Code" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plEditorTypes" runat="server" controlname="ddEditorTypes" suffix=":" />
  </td>
  <td>
   <asp:dropdownlist runat="server" ID="ddEditorTypes" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plCopyToEmpty" runat="server" controlname="chkCopyToEmpty" suffix=":" />
  </td>
  <td>
   <asp:checkbox Runat="server" ID="chkCopyToEmpty" />
  </td>
 </tr>
 <tr>
  <td colspan="2">&nbsp;</td>
 </tr>
 <tr>
  <td colspan="2">
   <asp:LinkButton runat="server" ID="cmdEdit" resourcekey="cmdEdit" CssClass="CommandButton" />&nbsp;
   <asp:LinkButton runat="server" ID="cmdVerify" resourcekey="cmdVerify" CssClass="CommandButton" />&nbsp;
   <asp:LinkButton runat="server" ID="cmdLangPack" resourcekey="cmdLangPack" CssClass="CommandButton" />
  </td>
 </tr>
</table>


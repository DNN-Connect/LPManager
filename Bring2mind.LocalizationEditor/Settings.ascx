<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="Bring2mind.DNN.Modules.LocalizationEditor.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0">
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plLocales" runat="server" controlname="chkLocales" suffix=":" />
  </td>
  <td>
   <asp:checkboxlist Runat="server" ID="chkLocales" RepeatColumns="2" DataTextField="Text" DataValueField="Code" CssClass="Normal" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plObjects" runat="server" controlname="chkObjects" suffix=":" />
  </td>
  <td>
   <asp:checkboxlist Runat="server" ID="chkObjects" RepeatColumns="2" DataTextField="FriendlyName" DataValueField="DesktopModuleID" CssClass="Normal" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plRemoveBlanksFromFile" runat="server" controlname="chkRemoveBlanksFromFile" suffix=":" />
  </td>
  <td>
   <asp:checkbox Runat="server" ID="chkRemoveBlanksFromFile" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plAddNewEntriesAsBlank" runat="server" controlname="chkAddNewEntriesAsBlank" suffix=":" />
  </td>
  <td>
   <asp:checkbox Runat="server" ID="chkAddNewEntriesAsBlank" />
  </td>
 </tr>
</table>

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Editor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.Controls.Editor" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<asp:Panel runat="server" ID="pnlEditor">
  <table cellpadding="0" cellspacing="2">
    <tr>
      <td>
        <asp:Panel runat="server" ID="pnlTextbox">
          <asp:TextBox runat="server" ID="txtValue" Width="400" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlHtmlValue">
          <dnn:TextEditor runat="server" ID="teValue" Width="500" Height="300" />
        </asp:Panel>
      </td>
      <td>
        <asp:LinkButton runat="server" ID="cmdSwitch" Text="Switch" />
      </td>
    </tr>
  </table>
</asp:Panel>

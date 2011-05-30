<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ObjectSummary.ascx.vb"
  Inherits="DNNEurope.Modules.LocalizationEditor.ObjectSummary" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="width: 100%; text-align: left;">
  <h2>
    <%=ModuleFriendlyName%>
    -
    <%=Locale%></h2>
  <table cellpadding="4" cellspacing="1" border="0">
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblCurrentVersion" resourcekey="lblCurrentVersion" />
      </td>
      <td>
        <%=Original.CurrentVersion%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblCurrentVersionLoc" resourcekey="lblCurrentVersionLoc" />
        <%=Locale%>
      </td>
      <td>
        <%=Target.CurrentVersion%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblNrOfFiles" resourcekey="lblNrOfFiles" />
      </td>
      <td>
        <%=Original.NrOfFiles%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblNrOfItems" resourcekey="lblNrOfItems" />
      </td>
      <td>
        <%=Original.NrOfItems%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblNrOfChangedTexts" resourcekey="lblNrOfChangedTexts" />
      </td>
      <td>
        <%=Original.NrOfChangedTexts%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblNrOfMissingTranslations" resourcekey="lblNrOfMissingTranslations" />
      </td>
      <td>
        <%=Target.NrOfMissingTranslations%>
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <asp:Label runat="server" ID="lblPercentageComplete" resourcekey="lblPercentageComplete" />
      </td>
      <td>
        <%=Target.PercentageComplete%>%
      </td>
    </tr>
  </table>
  <h2>
    <asp:Label runat="server" ID="lblEdit" resourcekey="lblEdit" /></h2>
  <table cellspacing="0" cellpadding="2" border="0">
    <tr>
      <td class="SubHead" width="165">
        <dnn:Label ID="plSourceLocale" runat="server" ControlName="ddSourceLocale" Suffix=":" />
      </td>
      <td>
        <asp:DropDownList runat="server" ID="ddSourceLocale" DataValueField="Locale" DataTextField="Locale" />
      </td>
    </tr>
    <tr>
      <td class="SubHead" width="165">
        <dnn:Label ID="plVersion" runat="server" ControlName="ddVersion" Suffix=":" />
      </td>
      <td>
        <asp:DropDownList runat="server" ID="ddVersion" DataValueField="Version" DataTextField="Version" />
      </td>
    </tr>
    <tr>
      <td class="SubHead" width="165">
        <dnn:Label ID="plSelection" runat="server" ControlName="ddSelection" Suffix=":" />
      </td>
      <td>
        <asp:DropDownList runat="server" ID="ddSelection" DataValueField="FilePath" DataTextField="FilePath" />
      </td>
    </tr>
  </table>
</div>
<p style="margin-top: 20px;">
  <asp:LinkButton runat="server" ID="cmdEdit" resourcekey="cmdEdit" Text="Edit" CssClass="CommandButton" />&nbsp;&nbsp;
  <asp:HyperLink runat="server" ID="cmdDownload" resourcekey="cmdDownload" Text="Download"
    CssClass="CommandButton" />&nbsp;&nbsp;
  <asp:HyperLink runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return"
    CssClass="CommandButton" />
</p>

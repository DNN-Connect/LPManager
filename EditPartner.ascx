﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditPartner.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.EditPartner" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" runat="server" id="tblCubeUrl">
    <tr>
        <td class="SubHead">
            <dnn:label id="plCubeUrl" runat="server" controlname="txtCubeUrl" suffix=":" />
        </td>
        <td>
            <asp:TextBox ID="txtCubeUrl" Width="300" MaxLength="255" CssClass="NormalTextBox" runat="server" />
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
            <ul class="dnnActions dnnClear">
                 
            <li>
                <asp:LinkButton ID="cmdDownload" runat="server" CssClass="dnnSecondaryAction" BorderStyle="none" Text="Download" ResourceKey="cmdDownload" /></li>
            </ul>
        </td>
    </tr>
</table>

<asp:Panel runat="server" ID="pnlDetails">
    <table cellspacing="0" cellpadding="2" border="0">
        <tr>
            <td class="SubHead">
                <dnn:label id="plPartnerName" runat="server" controlname="txtPartnerName" suffix=":" />
            </td>
            <td>
                <asp:TextBox ID="txtPartnerName" Width="300" MaxLength="100" CssClass="NormalTextBox" runat="server" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plPartnerUrl" runat="server" controlname="txtPartnerUrl" suffix=":" />
            </td>
            <td>
                <asp:TextBox ID="txtPartnerUrl" Width="300" MaxLength="300" CssClass="NormalTextBox" runat="server" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plPackUrl" runat="server" controlname="txtPackUrl" suffix=":" />
            </td>
            <td>
                <asp:TextBox ID="txtPackUrl" Width="300" MaxLength="255" CssClass="NormalTextBox" runat="server" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plAllowDirectDownload" runat="server" controlname="chkAllowDirectDownload" suffix=":" />
            </td>
            <td>
                <asp:CheckBox ID="chkAllowDirectDownload" runat="server" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plPartnerUsername" runat="server" controlname="txtPartnerUsername" suffix=":" />
            </td>
            <td>
                <asp:TextBox ID="txtPartnerUsername" Width="300" MaxLength="50" CssClass="NormalTextBox" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plAllowRedistribute" runat="server" controlname="chkAllowRedistribute" suffix=":" />
            </td>
            <td>
                <asp:CheckBox ID="chkAllowRedistribute" runat="server" Text="Yes"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="SubHead">
                <dnn:label id="plDownloadAffiliates" runat="server" controlname="chkDownloadAffiliates" suffix=":" />
            </td>
            <td>
                <asp:CheckBox ID="chkDownloadAffiliates" runat="server" Text="Yes"></asp:CheckBox>
            </td>
        </tr>
    </table>
</asp:Panel>
<ul class="dnnActions dnnClear">
    <li>
        <asp:LinkButton ID="cmdUpdate" CssClass="dnnPrimaryAction" runat="server" Text="Update" BorderStyle="none" ResourceKey="cmdUpdate" /></li>
    <li>
        <asp:LinkButton ID="cmdCancel" CssClass="dnnSecondaryAction" runat="server" Text="Cancel" BorderStyle="none" CausesValidation="False" ResourceKey="cmdCancel" /></li>
    <li>
        <asp:LinkButton ID="cmdDelete" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" BorderStyle="none" Text="Delete" ResourceKey="cmdDelete" /></li>
</ul>

<pre><asp:PlaceHolder runat="server" ID="plhDownload" /></pre>

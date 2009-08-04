﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManageObjects.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.ManageObjects" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<h2><asp:Label ID="lblImportObject" runat="server" resourcekey="lblImportObject" /></h2>

<table cellpadding="2" cellspacing="1">
    <tr>
        <td>
            <dnn:Label ID="lblPackageFile" runat="server" resourcekey="lblPackageFile" suffix=":" />
        </td>
        <td>
            <asp:FileUpload runat="server" ID="ctlUpload" />
        </td>
        <td>
            <asp:LinkButton ID="lbImportPackage" runat="server" resourcekey="lbImportPackage" Text="Import Package" CssClass="CommandButton" />
        </td>
    </tr>
    <tr>
        <td>
            <dnn:Label ID="lblInstalledObject" runat="server" resourcekey="lblInstalledObject" suffix=":" />
        </td>
        <td>
            <asp:DropDownList ID="ddlInstalledObjects" runat="server" />
        </td>
        <td>
            <asp:LinkButton ID="lbImportInstalledObject" runat="server" resourcekey="lbImportInstalledObject" Text="Import Installed Object" CssClass="CommandButton" />
        </td>
    </tr>
</table>

<p><asp:Label runat="server" ID="lblUploadError" CssClass="NormalRed" /></p>

<p style="margin-bottom:20px;">
    <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return" CssClass="CommandButton" CausesValidation="false" />
</p>

<h2><asp:Label ID="lblCurrentObjects" runat="server" resourcekey="lblCurrentObjects" /></h2>

<asp:DataList runat="server" ID="dlTranslateObjects" DataKeyField="ObjectId">
    <HeaderTemplate>
    <table cellpadding="2" cellspacing="1">
        <tr>
            <th></th>
            <th><%#DotNetNuke.Services.Localization.Localization.GetString("FriendlyName.Header", LocalResourceFile)%></th>
            <th></th>
            <th><%#DotNetNuke.Services.Localization.Localization.GetString("ObjectName.Header", LocalResourceFile)%></th>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <asp:imagebutton id="cmdDeleteTranslateObject" runat="server" causesvalidation="false" commandname="Delete"	imageurl="~/images/delete.gif" 
                    OnClientClick='<%# String.Concat("return confirm(""", DotNetNuke.Services.Localization.Localization.GetString("DeleteItem.Text", LocalResourceFile), """);") %>' 
                    alternatetext="Delete" resourcekey="cmdDelete" />
            </td>
            <td><%#DataBinder.Eval(Container.DataItem, "FriendlyName")%></td>
            <td>&nbsp;</td>
            <td><%#DataBinder.Eval(Container.DataItem, "ObjectName")%></td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
    </table>
    </FooterTemplate>
</asp:DataList>

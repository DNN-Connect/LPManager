<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LocalizationEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.LocalizationEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<asp:Panel runat="server" ID="pnlEdit">
<asp:DataList runat="server" ID="dlObjects">
    <HeaderTemplate>
    <table cellpadding="4" cellspacing="1">
        <tr align="left">
            <th><asp:label runat="server" ID="lblObjects" resourcekey="lblObjects" /></th>
            <th><asp:label runat="server" ID="lblLocales" resourcekey="lblLocales" /></th>
        </tr>
        <tr align="left">
            <td><asp:label runat="server" ID="lblObjectAction" resourcekey="lblObjectAction" /></td>
            <td><asp:label runat="server" ID="lblLocaleAction" resourcekey="lblLocaleAction" /></td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr align="left">
            <td><a href="<%#GetObjectURL(DataBinder.Eval(Container.DataItem, "ObjectId"))%>" class="CommandButton"><%#DataBinder.Eval(Container.DataItem, "ModuleFriendlyName")%></a></td>           
            <td><%#GetObjectLocales(DataBinder.Eval(Container.DataItem, "ObjectId"))%></td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
    </table>
    </FooterTemplate>
</asp:DataList>
</asp:Panel>

<p style="margin-top:20px;">
    <asp:LinkButton ID="lbManageObjects" runat="server" resourcekey="lbManageObjects" Text="Manage modules" CssClass="CommandButton" />&nbsp;&nbsp;
    <asp:LinkButton ID="lbManagePermissions" runat="server" resourcekey="lbManagePermissions" Text="Manage Permissions" CssClass="CommandButton" />
</p>

﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LocalizationEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.LocalizationEditor" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<script language="javascript">
    function confirmSend() {
        return confirm('<%=LocalizeString("SendConfirm") %>');
 }
</script>
<asp:PlaceHolder ID="plhLocales" runat="server" />
<asp:Panel ID="pnlLocaleRequest" runat="server">
    <div class="genericLocale"><%=(New System.Globalization.CultureInfo(Locale)).NativeName %></div>
    <ul class="dnnActions dnnClear">

        <li><a href="<%=DotNetNuke.Common.NavigateUrl() %>" class="CommandButton"><%=LocalizeString("Back")%></a></li>
    </ul>
    <asp:Panel runat="server" ID="pnlCorePackages">
        <h3><%=LocalizeString("CorePackages")%></h3>
        <asp:DataList runat="server" ID="dlCorePackages">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="1">
                    <tr align="left">
                        <th>
                            <asp:Label runat="server" ID="lblPackageCP" resourcekey="lblPackage" />
                        </th>
                        <th>
                            <asp:Label runat="server" ID="lblLastVersion2CP" resourcekey="lblLastVersion" />
                        </th>
                        <th colspan="2">&nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr align="left">
                    <td>
                        <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
                    </td>
                    <td>
                        <%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>
                    </td>
                    <td>
                        <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>&Locale=<%=Locale%>"
                            title="<%=LocalizeString("Download") %>"
                            style="display: <%#IIF(DataBinder.Eval(Container.DataItem, "PercentComplete")>0,"block","none")%>" class="iconLink">
                            <span class="entypoIcon icon16" title="<%=LocalizeString("Download") %>">&#128190;</span>
                        </a>
                    </td>
                    <td>
                        <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>"
                            title="<%=LocalizeString("OtherVersions") %>" class="iconLink">
                            <span class="entypoIcon icon16" title="<%=LocalizeString("OtherVersions") %>">&#59249;</span>
                        </a>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:DataList>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlPackages">
        <h3><%=LocalizeString("Packages")%></h3>
        <asp:DataList runat="server" ID="dlPackages">
            <HeaderTemplate>
                <table cellpadding="4" cellspacing="1">
                    <tr align="left">
                        <th>
                            <asp:Label runat="server" ID="lblPackage" resourcekey="lblPackage" />
                        </th>
                        <th  align="center">
                            <asp:Label runat="server" ID="lblLastVersion2" resourcekey="lblLastVersion" />
                        </th>
                        <th colspan="2">&nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr align="left">
                    <td>
                        <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
                    </td>
                    <td  align="center">
                        <%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>
                    </td>
                    <td>
                        <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>&Locale=<%=Locale%>"
                            title="<%=LocalizeString("Download") %>"
                            style="display: <%#IIF(DataBinder.Eval(Container.DataItem, "PercentComplete")>0,"block","none")%>" class="iconLink">
                            <span class="entypoIcon icon16" title="<%=LocalizeString("Download") %>">&#128190;</span>
                        </a>
                    </td>
                    <td>
                        <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>"
                            title="<%=LocalizeString("OtherVersions") %>" class="iconLink">
                            <span class="entypoIcon icon16" title="<%=LocalizeString("OtherVersions") %>">&#59249;</span>
                        </a>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:DataList>
    </asp:Panel>

    <h3><%=LocalizeString("Components")%></h3>
    <asp:DataList runat="server" ID="dlObjects">
        <HeaderTemplate>
            <table cellpadding="4" cellspacing="1">
                <tr align="left">
                    <th>
                        <asp:Label runat="server" ID="lblObjects" resourcekey="lblObjects" />
                    </th>
                    <th align="center">
                        <asp:Label runat="server" ID="lblLastVersion" resourcekey="lblLastVersion" />
                    </th>
                    <th  align="center" style="display: <%#IIF(IsEditor Or IsAdmin,"block","none")%>">
                        <asp:Label runat="server" ID="lblNrKeys" resourcekey="lblNrKeys" />
                    </th>
                    <th>
                        <asp:Label runat="server" ID="lblLocaleAction" resourcekey="lblLocaleAction" />
                    </th>
                    <th colspan="3">&nbsp;</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr align="left">
                <td>
                    <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
                </td>
                <td align="center">
                    <%#DataBinder.Eval(Container.DataItem, "LastVersion")%>
                </td>
                <%If IsEditor Or IsAdmin Then%>
                <td align="center">
                    <%#DataBinder.Eval(Container.DataItem, "LastVersionTextCount")%>
                </td>
                <%End If%>
                <td align="center">
                    <%#GetObjectLocalePerctComplete(Container.DataItem)%>
                </td>
                <td>
                    <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "LastVersion")%>&Locale=<%=Locale%>"
                        title="<%=LocalizeString("Download") %>" style="display: <%#IIF(DataBinder.Eval(Container.DataItem, "TextCount")>0,"block","none")%>" class="iconLink">
                        <span class="entypoIcon icon16" title="<%=LocalizeString("Download") %>">&#128190;</span>
                    </a>
                </td>
                <td>
                    <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>"
                        title="<%=LocalizeString("OtherVersions") %>" class="iconLink">
                        <span class="entypoIcon icon16" title="<%=LocalizeString("OtherVersions") %>">&#59249;</span>
                    </a>
                </td>
                <td>
                    <%#GetEditColumn(Container.DataItem)%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:DataList>
</asp:Panel>

<p style="margin-top: 20px;">
    <asp:LinkButton ID="cmdUploadPack" runat="server" CausesValidation="false" Visible="false" Text="&#128228;" CssClass="entypoButton" />
    <asp:LinkButton ID="cmdManageObjects" runat="server" CausesValidation="false" Visible="false" Text="&#59213;" CssClass="entypoButton" />
    <asp:LinkButton ID="cmdManagePermissions" runat="server" CausesValidation="false" Visible="false" Text="&#128101;" CssClass="entypoButton" />
    <asp:LinkButton ID="cmdManagePartners" runat="server" CausesValidation="false" Visible="false" Text="&#128362;" CssClass="entypoButton" />
    <asp:LinkButton ID="cmdClearCaches" runat="server" CausesValidation="false" Visible="false" Text="&#59249;" CssClass="entypoButton" />
    <asp:LinkButton ID="cmdCube" runat="server" CausesValidation="false" Visible="false" Text="&#59157;" CssClass="entypoButton" />
    <a href="#" id="cmdService" runat="server" class="entypoButton" title='<%= LocalizeString("lbService") %>'>&#128363;</a>
</p>

<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
<% If UserPermissions.Count > 0 Then %>
     var $dialogConnections = $('<div class="dnnDialog"></div>')
           .html('<div class="dialogLine"><span><%= LocalizeJSString("ServiceUrl") %></span><input type="text" value="http://<%= Request.Url.Host & DotNetNuke.Common.Globals.ApplicationPath & String.Format("/DesktopModules/DNNEurope/LocalizationEditor/API?tabid={0}&moduleid={1}", TabId, ModuleId) %>" /></div><h3><%= LocalizeJSString("AccessKeys") %></h3><% For Each up As DNNEurope.Modules.LocalizationEditor.Entities.Permissions.PermissionInfo In UserPermissions %><div class="dialogLine"><span><%= up.Locale %></span><input type="text" value="<%= up.PermissionId %>-<%= up.AccessKey %>" /></div><% Next %>')
		.dialog({
		    autoOpen: false,
		    resizable: false,
		    dialogClass: 'dnnFormPopup dnnClear',
		    title: '<%= LocalizeJSString("TranslatorAccess") %>',
		 width: 500
		});
     $('#<%=cmdService.ClientId %>').click(function () {
         $dialogConnections.dialog('open');
         return false;
     });
<% End If %>
 });
}(jQuery, window.Sys));
</script>

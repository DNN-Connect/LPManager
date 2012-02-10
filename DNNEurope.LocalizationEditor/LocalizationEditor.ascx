<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LocalizationEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.LocalizationEditor" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:PlaceHolder ID="plhLocales" runat="server" />
<asp:Panel ID="pnlLocaleRequest" runat="server">
 <div class="genericLocale"><%=(New Globalization.CultureInfo(Locale)).NativeName %></div>
 <p>
  <a href="<%=DotNetNuke.Common.NavigateUrl() %>" class="CommandButton"><%=DotNetNuke.Services.Localization.Localization.GetString("Back",LocalResourceFile) %></a>
 </p>
 <h3><%=DotNetNuke.Services.Localization.Localization.GetString("Packages",LocalResourceFile) %></h3>
  <asp:DataList runat="server" ID="dlPackages">
  <HeaderTemplate>
   <table cellpadding="4" cellspacing="1">
    <tr align="left">
     <th>
      <asp:Label runat="server" ID="lblPackage" resourcekey="lblPackage" />
     </th>
     <th>
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
    <td>
     <%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>
    </td>
    <td>
     <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "LastPackVersion")%>&Locale=<%=Locale%>"
       title="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>"
       style="display:<%#IIF(DataBinder.Eval(Container.DataItem, "PercentComplete")>0,"block","none")%>">
      <img src="<%=ResolveUrl("~/images/eip_save.gif")%>" border="0" alt="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>" />
     </a>
    </td>
    <td>
     <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>"
       title="<%=DotNetNuke.Services.Localization.Localization.GetString("OtherVersions",LocalResourceFile) %>">
      <img src="<%=ResolveUrl("~/images/rt.gif")%>" border="0" alt="<%=DotNetNuke.Services.Localization.Localization.GetString("OtherVersions",LocalResourceFile) %>" />
     </a>
    </td>     
   </tr>
  </ItemTemplate>
  <FooterTemplate>
   </table>
  </FooterTemplate>
 </asp:DataList>

 <h3><%=DotNetNuke.Services.Localization.Localization.GetString("Components",LocalResourceFile) %></h3>
 <asp:DataList runat="server" ID="dlObjects">
  <HeaderTemplate>
   <table cellpadding="4" cellspacing="1">
    <tr align="left">
     <th>
      <asp:Label runat="server" ID="lblObjects" resourcekey="lblObjects" />
     </th>
     <th>
      <asp:Label runat="server" ID="lblLastVersion" resourcekey="lblLastVersion" />
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
    <td>
     <%#DataBinder.Eval(Container.DataItem, "LastVersion")%>
    </td>
    <td>
     <%#GetObjectLocalePerctComplete(Container.DataItem)%>
    </td>
    <td>
     <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "LastVersion")%>&Locale=<%=Locale%>"
       title="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>" style="display:<%#IIF(DataBinder.Eval(Container.DataItem, "TextCount")>0,"block","none")%>">
      <img src="<%=ResolveUrl("~/images/eip_save.gif")%>" border="0" alt="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>" />
     </a>
    </td>
    <td>
     <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>"
       title="<%=DotNetNuke.Services.Localization.Localization.GetString("OtherVersions",LocalResourceFile) %>">
      <img src="<%=ResolveUrl("~/images/rt.gif")%>" border="0" alt="<%=DotNetNuke.Services.Localization.Localization.GetString("OtherVersions",LocalResourceFile) %>" />
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
 <dnn:CommandButton ID="cmdUploadPack" ResourceKey="lbUploadPack" runat="server" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/up_32.png" DisplayLink="False" CausesValidation="false" Visible="false" />
 <dnn:CommandButton ID="cmdManageObjects" ResourceKey="lbManageObjects" runat="server" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/file_32.png" DisplayLink="False" CausesValidation="false" Visible="false" />
 <dnn:CommandButton ID="cmdManagePermissions" ResourceKey="lbManagePermissions" runat="server" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/user_32.png" DisplayLink="False" CausesValidation="false" Visible="false" />
 <dnn:CommandButton ID="cmdManagePartners" ResourceKey="lbManagePartners" runat="server" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/clients_32.png" DisplayLink="False" CausesValidation="false" Visible="false" />
 <dnn:CommandButton ID="cmdClearCaches" ResourceKey="lbClearCaches" runat="server" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/recycle_bin_32.png" DisplayLink="False" CausesValidation="false" Visible="false" />
 <asp:HyperLink runat="server" ID="hlCube" Visible="false">
  <asp:Image runat="server" ID="imgCube" ImageUrl="~/DesktopModules/DNNEurope/LocalizationEditor/images/network_connector_32.png" BorderWidth="0" />
 </asp:HyperLink>
</p>

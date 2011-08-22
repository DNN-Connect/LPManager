<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LocalizationEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.LocalizationEditor" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlEdit">
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
      <asp:Label runat="server" ID="lblNrtexts" resourcekey="lblNrtexts" />
     </th>
     <th colspan="<%=AllLocales.Count%>">
      <asp:Label runat="server" ID="lblLocaleAction" resourcekey="lblLocaleAction" />
     </th>
    </tr>
    <tr align="left">
     <td>
      <asp:Label runat="server" ID="lblObjectAction" resourcekey="lblObjectAction" />
     </td>
     <td colspan="2" />
     <%=Localeheaders%>
    </tr>
  </HeaderTemplate>
  <ItemTemplate>
   <tr align="left">
    <td>
     <a href="<%#GetObjectUrl(DataBinder.Eval(Container.DataItem, "ObjectId"))%>" class="CommandButton">
      <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%></a>
    </td>
    <td>
     <%#DataBinder.Eval(Container.DataItem, "LastVersion")%>
    </td>
    <td>
     <%#DataBinder.Eval(Container.DataItem, "LastVersionTextCount")%>
    </td>
     <%#GetObjectLocales(Container)%>
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

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DownloadPack.ascx.vb"
 Inherits="DNNEurope.Modules.LocalizationEditor.DownloadPack" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlHeader">
 <h1>
  <%=FriendlyName%>&nbsp;<%=Version%></h1>
</asp:Panel>

<asp:Panel ID="pnlTranslations" runat="server">
 <table>
  <tr>
   <td>
    <dnn:Label runat="server" ID="lblVersion" ControlName="ddVersion" Suffix=":" CssClass="SubHead" />
   </td>
   <td>
    <asp:DropDownList runat="server" ID="ddVersion" DataValueField="Version" DataTextField="Version" AutoPostBack="true" />
   </td>
  </tr>
  <tr>
   <td>
    <dnn:Label runat="server" ID="lblTotalTranslations" Suffix=":" CssClass="SubHead" />
   </td>
   <td>
    <%=TotalItems%>
   </td>
  </tr>
 </table>
 <p style="margin-top: 20px;">
  <asp:DataGrid ID="dgLocales" runat="server" GridLines="Horizontal" BorderWidth="1px" CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" CssClass="le_tbl" HeaderStyle-CssClass="SubHead" ItemStyle-CssClass="Normal">
   <Columns>
    <asp:BoundColumn DataField="Locale" HeaderText="Locale" />
    <asp:BoundColumn DataField="PercentComplete" HeaderText="PercentComplete" DataFormatString="{0:F0}" />
    <asp:TemplateColumn HeaderText="PartnerName">
     <ItemTemplate>
      <a href="<%#DataBinder.Eval(Container.DataItem, "PartnerUrl")%>" target="_blank"><%#DataBinder.Eval(Container.DataItem, "PartnerName")%></a>
     </ItemTemplate>
    </asp:TemplateColumn>
    <asp:BoundColumn DataField="LastModified" HeaderText="LastModified" DataFormatString="{0:d}" />
    <asp:TemplateColumn Visible="true" HeaderText="AvailablePacks">
     <ItemTemplate>
      <%#DownloadPackList(DataBinder.Eval(Container.DataItem, "PackUrl"), ObjectId, DataBinder.Eval(Container.DataItem, "RemoteObjectId"), DataBinder.Eval(Container.DataItem, "Locale"), Version)%>
     </ItemTemplate>
    </asp:TemplateColumn>
   </Columns>
  </asp:DataGrid>
 </p>
</asp:Panel>
<asp:Panel runat="server" ID="pnlVersions">
  <h2><%=(New Globalization.CultureInfo(Locale)).NativeName %></h2>
  <asp:DataGrid ID="dgVersions" runat="server" GridLines="Horizontal" BorderWidth="1px" CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" CssClass="le_tbl" HeaderStyle-CssClass="SubHead" ItemStyle-CssClass="Normal">
   <Columns>
    <asp:BoundColumn DataField="Version" HeaderText="Version" />
    <asp:BoundColumn DataField="PercentComplete" HeaderText="PercentComplete" DataFormatString="{0:F0}" />
    <asp:TemplateColumn HeaderText="Download">
     <ItemTemplate>
     <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>&Version=<%#DataBinder.Eval(Container.DataItem, "Version")%>&Locale=<%=Locale%>"
       title="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>">
      <img src="<%=ResolveUrl("~/images/eip_save.gif")%>" border="0" alt="<%=DotNetNuke.Services.Localization.Localization.GetString("Download",LocalResourceFile) %>" />
     </a>
     </ItemTemplate>
    </asp:TemplateColumn>
   </Columns>
  </asp:DataGrid>
</asp:Panel>
<p style="margin-top: 20px;">
 <asp:HyperLink runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return"  CssClass="CommandButton" />
</p>
<p style="margin-top: 20px;">
 <asp:Label runat="server" ID="lblNoResourceFiles" resourcekey="lblNoResourceFiles" Visible="false" />
</p>

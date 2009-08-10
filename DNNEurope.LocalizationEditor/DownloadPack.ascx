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
        <asp:DropDownList runat="server" ID="ddVersion" DataValueField="Version" DataTextField="Version"
          AutoPostBack="true" />
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
  <p>
    <asp:DataGrid ID="dgLocales" runat="server" GridLines="None" BorderWidth="0px" CellPadding="2"
      CellSpacing="4" AutoGenerateColumns="False" HeaderStyle-CssClass="SubHead" ItemStyle-CssClass="Normal">
      <Columns>
        <asp:BoundColumn DataField="Locale" HeaderText="Locale" />
        <asp:BoundColumn DataField="MissingTranslations" HeaderText="MissingTranslations" />
        <asp:BoundColumn DataField="PercentComplete" HeaderText="PercentComplete" DataFormatString="{0:F0}" />
        <asp:TemplateColumn Visible="true">
          <ItemTemplate>
            <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%=ObjectId%>&Locale=<%#DataBinder.Eval(Container.DataItem, "Locale")%>&Version=<%=Version%>"
              style="display: <%#IIF(CStr(DataBinder.Eval(Container.DataItem, "Locale")).Length > 2,"block","none")%>">
              <%=DotNetNuke.Services.Localization.Localization.GetString("cmdDownload", LocalResourceFile)%></a>
          </ItemTemplate>
        </asp:TemplateColumn>
      </Columns>
    </asp:DataGrid>
  </p>
</asp:Panel>
<div style="width: 500px; margin-top: 20px;">
  <asp:Label runat="server" ID="lblHelp" resourcekey="lblHelp" />
</div>
<p>
  <asp:HyperLink runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return"
    CssClass="CommandButton" /></p>
</asp:Panel>
<p style="margin-top: 20px;">
  <asp:Label runat="server" ID="lblNoResourceFiles" resourcekey="lblNoResourceFiles"
    Visible="false" />
</p>

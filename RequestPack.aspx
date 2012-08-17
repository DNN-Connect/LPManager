<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RequestPack.aspx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.RequestPack" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGO" Src="~/Admin/Skins/Logo.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <title><%=DotNetNuke.Services.Localization.Localization.GetString("PageTitle", LocalResourceFile)%></title>
 <style>
 
Body, A
{
   font-family: Verdana, Arial, Helvetica, Sans Serif;
   font-size: 8pt;
   font-weight: normal;
   color: black;
}

Body
{
    background-color: white;
    background-image: url(<%=DotNetNuke.Common.Globals.ResolveUrl("~/Install/installbg.gif")%>);
    background-repeat: repeat-x;
    margin:25px 25px 0px 25px;
}

.Normal, tr td
{
   font-family: Verdana, Arial, Helvetica, Sans Serif;
   font-size: 8pt;
   font-weight: normal;
   color: black;
}

table.le_tbl tbody tr td
{
 vertical-align: top;
}

table.le_tbl tbody tr.le_thdr td
{
 font-weight: bold;
}

H1
{
	font-size: 1.5em;
	font-weight: bold;
	color: #75808A;
	text-decoration: underline;
}

A:link  {
    font-size:  1.0em;
    font-weight:    bold;
    text-decoration:    none;
    color:  #75808A;
}

A:visited   {
    font-size:  1.0em;
    font-weight:    bold;
    text-decoration:    none;
    color:  #75808A;
}

A:active    {
    font-size:  1.0em;
    font-weight:    bold;
    text-decoration:    none;
    color:  #75808A;
}

A:hover {
    font-size:  1.0em;
    font-weight:    bold;
    text-decoration: underline;
    color:  #cc0000;
}

HR {
    color: #ededed;
    height:1pt;
    text-align:left
}
 </style>
</head>
<body>
 <form id="form1" runat="server">
 
 <dnn:LOGO runat="server" ID="dnnLOGO" />

<asp:Panel runat="server" ID="pnlMain">

<asp:Panel runat="server" ID="pnlHeader">
 <h1><%=FriendlyName%>&nbsp;<%=Version%></h1>
</asp:Panel>

<asp:Panel ID="pnlTranslations" runat="server">
<p>
 <asp:Label runat="server" ID="lblVersion" resourcekey="lblVersion" />&nbsp;&nbsp;
 <asp:DropDownList runat="server" ID="ddVersion" DataValueField="Version" DataTextField="Version" AutoPostBack="true" />
</p>
 
<p>
 <asp:Label runat="server" ID="lblTotalTranslations" resourcekey="lblTotalTranslations" />&nbsp;&nbsp;
 <%=TotalItems%>
</p>

<p>
  <asp:DataGrid ID="dgLocales" runat="server" GridLines="Horizontal" BorderWidth="1px" CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" CssClass="le_tbl" HeaderStyle-CssClass="le_thdr" ItemStyle-CssClass="Normal">
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

<div style="width:400px">
 <asp:Label runat="server" ID="lblHelp" resourcekey="lblHelp" />
</div>

</asp:Panel> 

<p>
 <asp:Label runat="server" ID="lblNoResourceFiles" resourcekey="lblNoResourceFiles" Visible="false" />
</p>

</asp:Panel>

<asp:Panel runat="server" ID="pnlDisambiguate">
 <h1><%=FriendlyName%></h1>
 <asp:PlaceHolder runat="server" ID="plhDisambiguate" />
</asp:Panel>

<asp:Label runat="server" ID="lblError" CssClass="NormalRed" />

 </form>
</body>
</html>

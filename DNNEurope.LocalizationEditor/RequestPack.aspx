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

.Normal, tr.TableItem td
{
   font-family: Verdana, Arial, Helvetica, Sans Serif;
   font-size: 8pt;
   font-weight: normal;
   color: black;
}

tr.TableHeader td
{
   font-family: Verdana, Arial, Helvetica, Sans Serif;
   font-size: 8pt;
   font-weight: bold;
   color: #75808A;
   border-bottom: solid 1px #75808A;
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
 <asp:DataGrid 
  id="dgLocales" runat="server"
  GridLines="None" 
  CellPadding="2" CellSpacing="4"
  BorderWidth="0px" AutoGenerateColumns="False" HeaderStyle-CssClass="TableHeader" ItemStyle-CssClass="TableItem">
  <Columns>
   <asp:BoundColumn DataField="Locale" HeaderText="Locale" />
   <asp:BoundColumn DataField="MissingTranslations" HeaderText="MissingTranslations" />
   <asp:BoundColumn DataField="PercentComplete" HeaderText="PercentComplete" DataFormatString="{0:F0}" />
   <asp:TemplateColumn>
    <ItemTemplate>
     <a href="<%=ResolveUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")%>?ObjectId=<%=ObjectId%>&Locale=<%#DataBinder.Eval(Container.DataItem, "Locale")%>&Version=<%=Version%>" style="display:<%#IIF(CStr(DataBinder.Eval(Container.DataItem, "Locale")).Length > 2,"block","none")%>"><%=DotNetNuke.Services.Localization.Localization.GetString("cmdDownload", LocalResourceFile)%></a>
    </ItemTemplate>
   </asp:TemplateColumn>
  </Columns>
 </asp:DataGrid>
</p>

<div style="width:400px">
 <asp:Label runat="server" ID="lblHelp" resourcekey="lblHelp" />
</div>

<p><table>
 <tr>
  <td>
   <dnn:label id="plLocale" runat="server" controlname="txtLocale" suffix=":" CssClass="SubHead" />
  </td>
  <td>
   <asp:TextBox runat="server" ID="txtLocale" Width="50" />&nbsp;
  </td>
  <td>
   <asp:Button runat="server" ID="cmdDownload" resourcekey="cmdDownload" />
  </td>
 </tr>
 <tr>
  <td />
  <td colspan="2">
   <asp:RegularExpressionValidator runat="server" ID="regLocale" ControlToValidate="txtLocale" Display="Dynamic" CssClass="NormalRed" resourcekey="InvalidLocale.Error" ValidationExpression="\w\w-\w\w" />
  </td>
 </tr>
</table></p>
</asp:Panel> 

<p>
 <asp:Label runat="server" ID="lblNoResourceFiles" resourcekey="lblNoResourceFiles" Visible="false" />
</p> 
 </form>
</body>
</html>

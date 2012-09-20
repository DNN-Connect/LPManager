<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Partners.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.Partners" %>

<asp:DataGrid 
 id="dgPartners" runat="server" 
 GridLines="Horizontal" 
 CellPadding="6"
 BorderWidth="1px" 
 AutoGenerateColumns="False" CssClass="le_tbl" HeaderStyle-CssClass="le_thdr">
 <Columns>
  <asp:TemplateColumn HeaderText="Edit" SortExpression="">
   <ItemTemplate>
    <asp:HyperLink runat="server" Text="Edit" NavigateURL='<%# EditURL("PartnerId", DataBinder.Eval(Container.DataItem, "PartnerId"), "EditPartner") %>' id="hlEdit" resourceKey="cmdEdit" />
   </ItemTemplate>
  </asp:TemplateColumn>
  <asp:BoundColumn DataField="PartnerName" HeaderText="PartnerName" SortExpression="PartnerName" />
  <asp:BoundColumn DataField="PartnerUrl" HeaderText="PartnerUrl" SortExpression="PartnerUrl" />
 </Columns>
</asp:DataGrid>

<p style="padding-top: 30px;">
<asp:hyperlink runat="server" id="cmdAdd" resourcekey="cmdAdd" text="Add" cssclass="CommandButton" />&nbsp;
<asp:hyperlink runat="server" id="cmdBack" resourcekey="cmdBack" text="Back" cssclass="CommandButton" />
</p>

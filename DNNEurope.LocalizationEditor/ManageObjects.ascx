<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManageObjects.ascx.vb"
 Inherits="DNNEurope.Modules.LocalizationEditor.ManageObjects" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<h2>
 <asp:Label ID="lblImportObject" runat="server" resourcekey="lblImportObject" /></h2>
<table cellpadding="2" cellspacing="1">
 <tr>
  <td>
   <dnn:Label ID="lblPackageFile" runat="server" ResourceKey="lblPackageFile" Suffix=":" />
  </td>
  <td>
   <asp:FileUpload runat="server" ID="ctlUpload" />
  </td>
  <td>
   <asp:LinkButton ID="lbImportPackage" runat="server" resourcekey="lbImportPackage"
    Text="Import Package" CssClass="CommandButton" />
  </td>
 </tr>
</table>
<p>
 <asp:Label runat="server" ID="lblUploadError" CssClass="NormalRed" /></p>
<p style="margin-bottom: 20px;">
 <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return"
  CssClass="CommandButton" CausesValidation="false" />
</p>
<h2>
 <asp:Label ID="lblCurrentObjects" runat="server" resourcekey="lblCurrentObjects" /></h2>
<asp:DataList runat="server" ID="dlTranslateObjects" DataKeyField="ObjectId">
 <HeaderTemplate>
  <table cellpadding="2" cellspacing="1">
   <tr align="left">
    <th>
    </th>
    <th>
     <%#DotNetNuke.Services.Localization.Localization.GetString("FriendlyName.Header", LocalResourceFile)%>
    </th>
    <th>
     <%#DotNetNuke.Services.Localization.Localization.GetString("LastVersion.Header", LocalResourceFile)%>
    </th>
    <th>
    </th>
    <th>
     <%#DotNetNuke.Services.Localization.Localization.GetString("ObjectName.Header", LocalResourceFile)%>
    </th>
    <th>
     <%#DotNetNuke.Services.Localization.Localization.GetString("IsCoreObject.Header", LocalResourceFile)%>
    </th>
   </tr>
 </HeaderTemplate>
 <ItemTemplate>
  <tr align="left">
   <td>
    <asp:ImageButton ID="cmdDeleteTranslateObject" runat="server" CausesValidation="false"
     CommandName="Delete" ImageUrl="~/images/delete.gif" OnClientClick='<%# String.Concat("return confirm(""", DotNetNuke.Services.Localization.Localization.GetString("DeleteItem.Text", LocalResourceFile), """);") %>'
     AlternateText="Delete" resourcekey="cmdDelete" />
   </td>
   <td>
    <%#DataBinder.Eval(Container.DataItem, "FriendlyName")%>
   </td>
   <td>
    <%#DataBinder.Eval(Container.DataItem, "LastVersion")%>
   </td>
   <td>
    &nbsp;
   </td>
   <td>
    <%#DataBinder.Eval(Container.DataItem, "ObjectName")%>
   </td>
   <td>
    <asp:Image runat="server" ID="Image1" ImageUrl="~/images/checked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"IsCoreObject")="true" %>' />
    <asp:Image runat="server" ID="Image2" ImageUrl="~/images/unchecked.gif" Visible='<%# DataBinder.Eval(Container.DataItem,"IsCoreObject")="false" %>' />
   </td>
  </tr>
 </ItemTemplate>
 <FooterTemplate>
  </table>
 </FooterTemplate>
</asp:DataList>

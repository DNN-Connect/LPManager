<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Users.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.Users" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellpadding="2" cellspacing="0">
  <tr class="Normal" valign="top">
    <td>
      <dnn:Label ID="lblUsername" runat="server" ControlName="lblUsername" ResourceKey="lblUsername" Suffix=":" />
    </td>
    <td>
      <asp:TextBox runat="server" ID="txtUsername" Width="200" MaxLength="256" />
      <asp:RequiredFieldValidator runat="server" ID="reqVal1" ControlToValidate="txtUsername"
        resourcekey="Required.Error" Display="Dynamic" />
    </td>
  </tr>
  <tr class="Normal" valign="top">
    <td>
      <dnn:Label ID="lblObjects" runat="server" ControlName="lblObjects" ResourceKey="lblObjects" Suffix=":" />
    </td>
    <td>
      <asp:DropDownList runat="server" ID="ddObjects" DataTextField="FriendlyName" DataValueField="ObjectId" />
    </td>
  </tr>
  <tr class="Normal" valign="top">
    <td>
      <dnn:Label ID="lblLocale" runat="server" ControlName="lblLocale" ResourceKey="lblLocale" Suffix=":" />
    </td>
    <td>
      <asp:TextBox runat="server" ID="txtLocale" Width="50" />
      <asp:RequiredFieldValidator runat="server" ID="reqVal2" ControlToValidate="txtLocale"
        resourcekey="Required.Error" Display="Dynamic" />
      <asp:RegularExpressionValidator runat="server" ID="regVal1" ControlToValidate="txtLocale"
        ValidationExpression="^[a-z][a-z](-[A-Z][A-Z])?$" resourcekey="InvalidLocale.Error"
        Display="Dynamic" />
    </td>
  </tr>
</table>
<p>
  <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" Text="Add" CssClass="CommandButton" />&nbsp;&nbsp;
  <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" Text="Return"
    CssClass="CommandButton" CausesValidation="false" />
</p>
<p>
  <asp:Label runat="server" ID="lblError" CssClass="NormalRed" /></p>
<asp:DataList runat="server" ID="dlUserPermissions" DataKeyField="PermissionId">
  <HeaderTemplate>
    <table cellpadding="2" cellspacing="1">
      <tr>
        <th>
        </th>
        <th>
          <%#DotNetNuke.Services.Localization.Localization.GetString("User.Header", LocalResourceFile)%>
        </th>
        <th>
        </th>
        <th>
          <%#DotNetNuke.Services.Localization.Localization.GetString("Object.Header", LocalResourceFile)%>
        </th>
        <th>
        </th>
        <th>
          <%#DotNetNuke.Services.Localization.Localization.GetString("Locale.Header", LocalResourceFile)%>
        </th>
      </tr>
  </HeaderTemplate>
  <ItemTemplate>
    <tr>
      <td>
        <asp:ImageButton ID="cmdDeleteUserPermission" runat="server" CausesValidation="false"
          CommandName="Delete" ImageUrl="~/images/delete.gif" OnClientClick='<%# String.Concat("return confirm(""", DotNetNuke.Services.Localization.Localization.GetString("DeleteItem.Text"), """);") %>'
          AlternateText="Delete" resourcekey="cmdDelete" />
      </td>
      <td>
        <%#DataBinder.Eval(Container.DataItem, "DisplayName")%>
        (<%#DataBinder.Eval(Container.DataItem, "Username")%>)
      </td>
      <td>
        &nbsp;
      </td>
      <td>
        <%#DataBinder.Eval(Container.DataItem, "ObjectName")%>
      </td>
      <td>
        &nbsp;
      </td>
      <td>
        <%#DataBinder.Eval(Container.DataItem, "Locale")%>
      </td>
    </tr>
  </ItemTemplate>
  <FooterTemplate>
    </table>
  </FooterTemplate>
</asp:DataList>

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ResourceVerifier.ascx.vb" Inherits="Bring2mind.DNN.Modules.LocalizationEditor.ResourceVerifier" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<P><asp:placeholder id="PlaceHolder1" runat="server"></asp:placeholder></P>

<p>
 <asp:LinkButton runat="server" ID="cmdFix" resourcekey="cmdFix" CssClass="CommandButton" />&nbsp;
 <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" CssClass="CommandButton" />
</p>

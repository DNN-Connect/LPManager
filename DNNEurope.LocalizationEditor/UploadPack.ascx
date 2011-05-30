<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UploadPack.ascx.vb"
 Inherits="DNNEurope.Modules.LocalizationEditor.Import" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Wizard ID="wzdImport" runat="server" DisplayCancelButton="True" CellPadding="5"
 CellSpacing="5" CssClass="Wizard" StartNextButtonType="Link" StepNextButtonType="Link"
 StepPreviousButtonType="Link" FinishPreviousButtonType="Link" FinishCompleteButtonType="Link"
 CancelButtonType="Link" DisplaySideBar="False">
 <StepStyle VerticalAlign="Top" />
 <NavigationButtonStyle CssClass="CommandButton" BorderStyle="None" BackColor="Transparent" />
 <HeaderTemplate>
  <asp:Label ID="lblTitle" CssClass="Head" runat="server"><%=DotNetNuke.Services.Localization.Localization.GetString(wzdImport.ActiveStep.Title + ".Title", Me.LocalResourceFile)%></asp:Label><br />
  <br />
  <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><%=DotNetNuke.Services.Localization.Localization.GetString(wzdImport.ActiveStep.Title + ".Help", Me.LocalResourceFile)%></asp:Label>
 </HeaderTemplate>
 <WizardSteps>
  <asp:WizardStep ID="WizardStep1" runat="server" Title="StepUpload" StepType="Start"
   AllowReturn="False">
   <asp:FileUpload runat="server" ID="ctlUpload" />
   <p>
    <asp:Label runat="server" ID="lblUploadError" CssClass="NormalRed" /></p>
   <p>
    <asp:Label runat="server" ID="lblUploadReport" CssClass="Normal" /></p>
  </asp:WizardStep>
  <asp:WizardStep ID="WizardStep2" runat="server" Title="StepParameters">
   <table cellspacing="0" cellpadding="2" border="0">
    <tr>
     <td class="SubHead" width="165" valign="top">
      <dnn:Label ID="plResult" runat="server" ControlName="txtResult" Suffix=":" />
     </td>
     <td>
      <asp:TextBox runat="server" ID="txtResult" Width="400" Height="200" TextMode="MultiLine"
       Wrap="False" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" width="165">
      <dnn:Label ID="plLocale" runat="server" Suffix=":" />
     </td>
     <td>
      <asp:DropDownList runat="server" ID="ddLocale" DataTextField="Locale" DataValueField="Locale" />
     </td>
    </tr>
    <tr runat="server" id="trObject">
      <td class="SubHead" width="165">
        <dnn:Label ID="plObject" runat="server" ControlName="ddObject" Suffix=":" />
      </td>
      <td>
        <asp:DropDownList ID="ddObject" runat="server" DataValueField="ObjectId" DataTextField="ModuleFriendlyName" AutoPostBack="true" />
      </td>
    </tr>
    <tr runat="server" id="trVersion">
      <td class="SubHead" width="165">
        <dnn:Label ID="plVersion" runat="server" ControlName="ddVersion" Suffix=":" />
      </td>
      <td>
        <asp:DropDownList ID="ddVersion" runat="server" DataValueField="Version" DataTextField="Version" />
      </td>
    </tr>
   </table>
  </asp:WizardStep>
  <asp:WizardStep ID="WizardStep3" runat="server" Title="StepConfirm" StepType="Finish">
   <p>
    <dnn:Label ID="plAnalysis" runat="server" ControlName="txtAnalysis" Suffix=":" />
   </p>
   <p>
    <asp:TextBox runat="server" ID="txtAnalysis" Width="550" Height="400" TextMode="MultiLine"
     Wrap="False" /></p>
  </asp:WizardStep>
 </WizardSteps>
</asp:Wizard>

Imports System
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization

Partial Public Class Settings
 Inherits Entities.Modules.ModuleSettingsBase

#Region " Base Method Implementations "

 Public Overrides Sub LoadSettings()
  Try
   If Not Page.IsPostBack Then
    Dim Settings As New ModuleSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
    With Settings
     txtLicense.Text = .License
     txtOwnerEmail.Text = .OwnerEmail
     txtOwnerName.Text = .OwnerName
     txtOwnerOrganization.Text = .OwnerOrganization
     txtOwnerUrl.Text = .OwnerUrl
     chkCachePacks.Checked = .CachePacks
    End With
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try

   Dim Settings As New ModuleSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
   With Settings
    .CachePacks = chkCachePacks.Checked
    .License = txtLicense.Text
    .OwnerEmail = txtOwnerEmail.Text
    .OwnerName = txtOwnerName.Text
    .OwnerOrganization = txtOwnerOrganization.Text
    .OwnerUrl = txtOwnerUrl.Text
    .SaveSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
   End With

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

End Class
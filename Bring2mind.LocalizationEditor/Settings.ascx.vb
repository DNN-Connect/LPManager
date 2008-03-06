Imports System
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions

Partial Public Class Settings
 Inherits Entities.Modules.ModuleSettingsBase

#Region " Base Method Implementations "
 Public Overrides Sub LoadSettings()
  Try
   If Not Page.IsPostBack Then

    Dim locList As LocaleCollection = DotNetNuke.Services.Localization.Localization.GetSupportedLocales
    Try
     locList.Remove("en-US")
    Catch
    End Try
    With chkLocales
     .DataSource = locList.AllValues
     .DataBind()
    End With
    Dim Settings As New ModuleSettings(Me.ModuleId)
    With Settings
     chkAddNewEntriesAsBlank.Checked = .AddNewEntriesAsBlank
     chkRemoveBlanksFromFile.Checked = .RemoveBlanksFromFile
    End With
    Dim locales As String = ";" & Settings.Locales
    For Each itm As ListItem In chkLocales.Items
     If locales.IndexOf(";" & itm.Value & ";") > -1 Then
      itm.Selected = True
     End If
    Next

    'Dim rc As New RoleController
    'Dim dtmc As New DotNetNuke.Entities.Modules.DesktopModuleController
    'Dim desktopModules As ArrayList = dtmc.GetDesktopModules
    Dim dv As New DataView(DotNetNuke.Common.ConvertDataReaderToDataTable(DotNetNuke.Data.DataProvider.Instance().GetDesktopModules()))
    With dv
     .RowFilter = "Not IsAdmin"
     .Sort = "FriendlyName"
    End With

    With chkObjects
     .DataSource = dv
     .DataBind()
     .Items.Insert(0, New ListItem(Localization.GetString("Core", Me.LocalResourceFile), "-1"))
    End With
    Dim objects As String = ";" & Settings.Objects
    For Each itm As ListItem In chkObjects.Items
     If objects.IndexOf(";" & itm.Value & ";") > -1 Then
      itm.Selected = True
     End If
    Next

   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try

   Dim Settings As New ModuleSettings(Me.ModuleId)

   Dim locales As String = ""
   For Each itm As ListItem In chkLocales.Items
    If itm.Selected Then
     locales &= itm.Value & ";"
    End If
   Next

   Dim objects As String = ""
   For Each itm As ListItem In chkObjects.Items
    If itm.Selected Then
     objects &= itm.Value & ";"
    End If
   Next

   With Settings
    .Locales = locales
    .Objects = objects
    .AddNewEntriesAsBlank = chkAddNewEntriesAsBlank.Checked
    .RemoveBlanksFromFile = chkRemoveBlanksFromFile.Checked
    .SaveSettings(Me.ModuleId)
   End With
   Me.Cache.Remove("Settings4Module" & Me.ModuleId & "inPortal" & Me.PortalId.ToString)
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

#Region " Web Form Designer Generated Code "

 'This call is required by the Web Form Designer.
 <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

 End Sub

 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  'CODEGEN: This method call is required by the Web Form Designer
  'Do not modify it using the code editor.
  InitializeComponent()
 End Sub

#End Region

End Class
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Exceptions

Partial Public Class LocalizationEditor
 Inherits ModuleBase

#Region " Private Members "
 Private _desktopModules As Hashtable
#End Region

#Region " Event Handlers "
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try

   Dim objList As String = ";" & Settings.Objects
   Dim objs As New SortedList
   _desktopModules = New Hashtable
   Dim dtmc As New DotNetNuke.Entities.Modules.DesktopModuleController
   For Each dtm As DotNetNuke.Entities.Modules.DesktopModuleInfo In dtmc.GetDesktopModules
    Try
     _desktopModules.Add(dtm.FolderName, dtm.FriendlyName)
     If objList.IndexOf(";" & dtm.DesktopModuleID & ";") > -1 Then
      objs.Add(dtm.FriendlyName, dtm.DesktopModuleID)
     End If
    Catch ex As Exception
    End Try
   Next
   If objList.IndexOf(";-1;") > -1 Then
    objs.Add("Core", "-1")
   End If

   If Not Me.IsPostBack Then

    Dim locales As New LocaleCollection
    locales.Remove(Localization.SystemLocale)
    For Each loc As Locale In GetSupportedLocales().AllValues
     If Settings.Locales.Contains(loc.Code) Then
      locales.Add(loc.Code, loc)
     End If
    Next
    ddLocales.DataSource = locales.AllValues
    ddLocales.DataBind()
    ddSourceLocale.DataSource = GetSupportedLocales().AllValues
    ddSourceLocale.DataBind()
    Try
     'ddSourceLocale.Items.Insert(0, New ListItem(GetString("defaultLocale", Me.LocalResourceFile), "en-US"))
     ddSourceLocale.Items.FindByValue("en-US").Selected = True
    Catch ex As Exception
    End Try
    ddObjects.DataSource = objs
    ddObjects.DataBind()
    Dim canEdit As Boolean = Me.IsEditable And ddLocales.Items.Count > 0 And ddObjects.Items.Count > 0
    cmdEdit.Visible = canEdit
    cmdVerify.Visible = canEdit
    'cmdLangPack.Visible = canEdit

    With ddEditorTypes
     .Items.Clear()
     .Items.Add(New ListItem(GetString("optTextbox", Me.LocalResourceFile), Globals.EditorType.Textbox.ToString))
     .Items.Add(New ListItem(GetString("optDNNLabel", Me.LocalResourceFile), Globals.EditorType.DNNLabel.ToString))
    End With

   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try

 End Sub

 Private Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click

  Dim locale As String = ddLocales.SelectedValue
  Dim obj As String = ddObjects.SelectedValue
  Me.Response.Redirect(EditUrl("Locale", locale, "Edit", "Module=" & obj, "Editor=" & ddEditorTypes.SelectedValue, "SourceLocale=" & ddSourceLocale.SelectedValue, "CopyEmpty=" & chkCopyToEmpty.Checked.ToString))

 End Sub

 Private Sub cmdVerify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdVerify.Click

  Dim locale As String = ddLocales.SelectedValue
  Dim obj As String = ddObjects.SelectedValue
  Me.Response.Redirect(EditUrl("Locale", locale, "Verify", "Module=" & obj))

 End Sub

 Private Sub cmdLangPack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLangPack.Click

  Dim srcLocale As String = ddSourceLocale.SelectedValue
  Dim locale As String = ddLocales.SelectedValue
  Dim obj As String = ddObjects.SelectedValue
  Dim lc As LocaleCollection = Localization.GetSupportedLocales
  Dim PackDir As String = PortalSettings.HomeDirectoryMapPath & "LanguagePacks\"
  If Not IO.Directory.Exists(PackDir) Then
   IO.Directory.CreateDirectory(PackDir)
  End If

  Dim LangPackWriter As New LocaleFilePackWriter
  Dim LocaleCulture As Locale = lc(locale)

  Dim _filelist As New SortedList
  Dim Packname As String = "ResourcePack."
  If obj = "-1" Then
   Globals.GetResourceFiles(_filelist, Server.MapPath("~\admin"))
   Globals.GetResourceFiles(_filelist, Server.MapPath("~\controls"))
   _filelist.Add(Server.MapPath(Localization.GlobalResourceFile), New IO.FileInfo(Server.MapPath(Localization.GlobalResourceFile)))
   _filelist.Add(Server.MapPath(Localization.SharedResourceFile), New IO.FileInfo(Server.MapPath(Localization.SharedResourceFile)))
   Packname &= "Core." & DotNetNuke.Common.glbAppVersion & "."
  Else
   Dim dmc As New DesktopModuleController
   Dim _module As DesktopModuleInfo = dmc.GetDesktopModule(Integer.Parse(obj))
   If _module Is Nothing Then
    Throw New Exception("Module " & obj & " not found")
    Exit Sub
   End If
   Globals.GetResourceFiles(_filelist, Server.MapPath("~\DesktopModules\" & _module.FolderName))
   ' remove the other files if part of another module that is nested
   For Each m As DesktopModuleInfo In dmc.GetDesktopModules
    If (Not m.DesktopModuleID.ToString = obj) AndAlso (m.FolderName.StartsWith(_module.FolderName)) Then
     Globals.RemoveResourceFiles(_filelist, Server.MapPath("~\DesktopModules\" & m.FolderName))
    End If
   Next
   Packname &= _module.FolderName.Replace("\", "_") & "." & _module.Version & "."
  End If
  Packname &= "zip"
  Dim LangPackName As String = LangPackWriter.SaveLanguagePack(LocaleCulture, _filelist, PackDir & Packname)
  Me.Response.Redirect(PortalSettings.HomeDirectory & "LanguagePacks/" & Packname, False)

 End Sub
#End Region

End Class
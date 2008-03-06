Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions

Partial Public Class ResourceVerifier
 Inherits ModuleBase

#Region " Private Members "
 Private _targetLocale As String = ""
 Private _objectToEdit As String = ""
 Private _module As DesktopModuleInfo = Nothing
 Private _filelist As SortedList = Nothing
#End Region

#Region " Page Event Handlers "
 Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  _targetLocale = Me.Request.Params("Locale")
  _objectToEdit = Me.Request.Params("Module")

  ' Verify permission
  If Not Me.IsEditable Then
   Throw New Exception("No permission to edit")
   Exit Sub
  End If
  If Not (";" & Settings.Locales).IndexOf(";" & _targetLocale & ";") > -1 Then
   Throw New Exception("No permission to edit locale " & _targetLocale)
   Exit Sub
  End If
  If Not (";" & Settings.Objects).IndexOf(";" & _objectToEdit & ";") > -1 Then
   Throw New Exception("No permission to edit object " & _objectToEdit)
   Exit Sub
  End If

  Try

   Dim locales As LocaleCollection = Localization.GetSupportedLocales()
   Dim shc, shcTop As UI.UserControls.SectionHeadControl

   _filelist = New SortedList
   If _objectToEdit = "-1" Then
    Globals.GetResourceFiles(_filelist, Server.MapPath("~\admin"))
    Globals.GetResourceFiles(_filelist, Server.MapPath("~\controls"))
    _filelist.Add(Server.MapPath(Localization.GlobalResourceFile), New IO.FileInfo(Server.MapPath(Localization.GlobalResourceFile)))
    _filelist.Add(Server.MapPath(Localization.SharedResourceFile), New IO.FileInfo(Server.MapPath(Localization.SharedResourceFile)))
   Else
    Dim dmc As New DesktopModuleController
    _module = dmc.GetDesktopModule(Integer.Parse(_objectToEdit))
    If _module Is Nothing Then
     Throw New Exception("Module " & _objectToEdit & " not found")
     Exit Sub
    End If
    Globals.GetResourceFiles(_filelist, Server.MapPath("~\DesktopModules\" & _module.FolderName))
    ' remove the other files if part of another module that is nested
    For Each m As DesktopModuleInfo In dmc.GetDesktopModules
     If (Not m.DesktopModuleID.ToString = _objectToEdit) AndAlso (m.FolderName.StartsWith(_module.FolderName)) Then
      Globals.RemoveResourceFiles(_filelist, Server.MapPath("~\DesktopModules\" & m.FolderName))
     End If
    Next
   End If

   ' SectionHead for _targetLocale
   shcTop = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
   shcTop.Section = _targetLocale
   shcTop.IncludeRule = True
   shcTop.IsExpanded = True
   shcTop.CssClass = "Head"
   shcTop.Text = Localization.GetString("locale", Me.LocalResourceFile) & locales(_targetLocale).Text & " (" & _targetLocale & ")"

   Dim tableTop As New HtmlTable
   tableTop.ID = _targetLocale
   Dim rowTop As New HtmlTableRow
   Dim cellTop As New HtmlTableCell

   Dim tableMissing As New HtmlTable
   tableMissing.ID = "Missing" & _targetLocale
   Dim tableEntries As New HtmlTable
   tableEntries.ID = "Entry" & _targetLocale
   Dim tableObsolete As New HtmlTable
   tableObsolete.ID = "Obsolete" & _targetLocale
   Dim tableOld As New HtmlTable
   tableOld.ID = "Old" & _targetLocale
   Dim tableDuplicate As New HtmlTable
   tableDuplicate.ID = "Duplicate" & _targetLocale
   Dim tableError As New HtmlTable
   tableError.ID = "Error" & _targetLocale


   For Each file As DictionaryEntry In _filelist
    ' check for existance
    If Not IO.File.Exists(Globals.ResourceFile(file.Key.ToString, _targetLocale)) Then
     Dim row As New HtmlTableRow
     Dim cell As New HtmlTableCell
     cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
     cell.Attributes.Item("Class") = "Normal"
     row.Cells.Add(cell)
     tableMissing.Rows.Add(row)
    Else
     Dim dsDef As New DataSet
     Dim dsRes As New DataSet
     Dim dtDef, dtRes As DataTable

     Try
      dsDef.ReadXml(file.Key.ToString)
     Catch
      Dim row As New HtmlTableRow
      Dim cell As New HtmlTableCell
      cell.InnerText = file.Key.ToString.Replace(Server.MapPath("~"), "")
      cell.Attributes.Item("Class") = "Normal"
      row.Cells.Add(cell)
      tableError.Rows.Add(row)
     End Try
     Try
      dsRes.ReadXml(Globals.ResourceFile(file.Key.ToString, _targetLocale))
     Catch
      If _targetLocale <> Localization.SystemLocale Then
       Dim row As New HtmlTableRow
       Dim cell As New HtmlTableCell
       cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
       cell.Attributes.Item("Class") = "Normal"
       row.Cells.Add(cell)
       tableError.Rows.Add(row)
      End If
     End Try

     If Not dsRes Is Nothing And Not dsDef Is Nothing Then
      dtDef = dsDef.Tables("data")
      dtDef.TableName = "default"
      dtRes = dsRes.Tables("data").Copy
      dtRes.TableName = "localized"
      dsDef.Tables.Add(dtRes)

      ' Check for duplicate entries in localized file
      Try
       ' if this fails-> file contains duplicates
       Dim c As New UniqueConstraint("uniqueness", dtRes.Columns("name"))
       dtRes.Constraints.Add(c)
       dtRes.Constraints.Remove("uniqueness")
      Catch
       Dim row As New HtmlTableRow
       Dim cell As New HtmlTableCell
       cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
       cell.Attributes.Item("Class") = "Normal"
       row.Cells.Add(cell)
       tableDuplicate.Rows.Add(row)
      End Try

      ' Check for missing entries in localized file
      Try
       ' if this fails-> some entries in System default file are not found in Resource file
       dsDef.Relations.Add("missing", dtRes.Columns("name"), dtDef.Columns("name"))
      Catch
       Dim row As New HtmlTableRow
       Dim cell As New HtmlTableCell
       cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
       cell.Attributes.Item("Class") = "Normal"
       row.Cells.Add(cell)
       tableEntries.Rows.Add(row)
      Finally
       dsDef.Relations.Remove("missing")
      End Try

      ' Check for obsolete entries in localized file
      Try
       ' if this fails-> some entries in Resource File are not found in System default
       dsDef.Relations.Add("obsolete", dtDef.Columns("name"), dtRes.Columns("name"))
      Catch
       Dim row As New HtmlTableRow
       Dim cell As New HtmlTableCell
       cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
       cell.Attributes.Item("Class") = "Normal"
       row.Cells.Add(cell)
       tableObsolete.Rows.Add(row)
      Finally
       dsDef.Relations.Remove("obsolete")
      End Try

      ' Check older files
      Dim resFile As New IO.FileInfo(Globals.ResourceFile(file.Key.ToString, _targetLocale))
      If CType(file.Value, IO.FileInfo).LastWriteTime > resFile.LastWriteTime Then
       Dim row As New HtmlTableRow
       Dim cell As New HtmlTableCell
       cell.InnerText = Globals.ResourceFile(file.Key.ToString, _targetLocale).Replace(Server.MapPath("~"), "")
       cell.Attributes.Item("Class") = "Normal"
       row.Cells.Add(cell)
       tableOld.Rows.Add(row)
      End If
     End If
    End If

   Next

   If tableMissing.Rows.Count > 0 Then
    ' ------- Missing files
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Missing" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("MissingFiles", Me.LocalResourceFile) & tableMissing.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableMissing)
   End If

   If tableDuplicate.Rows.Count > 0 Then
    ' ------- Duplicate keys
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Duplicate" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("DuplicateEntries", Me.LocalResourceFile) & tableDuplicate.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableDuplicate)
   End If

   If tableEntries.Rows.Count > 0 Then
    ' ------- Missing entries
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Entry" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("MissingEntries", Me.LocalResourceFile) & tableEntries.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableEntries)
   End If

   If tableObsolete.Rows.Count > 0 Then
    ' ------- Missing entries
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Obsolete" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("ObsoleteEntries", Me.LocalResourceFile) & tableObsolete.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableObsolete)
   End If

   If tableOld.Rows.Count > 0 Then
    ' ------- Old files
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Old" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("OldFiles", Me.LocalResourceFile) & tableOld.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableOld)
   End If

   If tableError.Rows.Count > 0 Then
    ' ------- Error files
    shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
    shc.Section = "Error" & _targetLocale
    shc.IncludeRule = False
    shc.IsExpanded = False
    shc.CssClass = "SubHead"
    shc.Text = Localization.GetString("ErrorFiles", Me.LocalResourceFile) & tableError.Rows.Count.ToString
    cellTop.Controls.Add(shc)
    cellTop.Controls.Add(tableError)
   End If

   rowTop.Cells.Add(cellTop)
   tableTop.Rows.Add(rowTop)
   PlaceHolder1.Controls.Add(shcTop)
   PlaceHolder1.Controls.Add(tableTop)
   PlaceHolder1.Controls.Add(New LiteralControl("<br>"))


  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try

 End Sub
#End Region

#Region " Other Event Handlers "
 Private Sub cmdFix_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdFix.Click

  Dim destinationUrl As String = ResolveUrl("~/DesktopModules/Bring2mind/LocalizationEditor/Fix.aspx")
  destinationUrl &= "?tabid=" & TabId.ToString & "&moduleid=" & ModuleId
  destinationUrl &= "&Locale=" & _targetLocale & "&Module=" & _objectToEdit
  Me.Response.Redirect(destinationUrl, False)

 End Sub

 Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
 End Sub
#End Region

End Class
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization

Partial Public Class ResourceEditor
 Inherits ModuleBase

#Region " Private Members "
 Private _targetLocale As String = ""
 Private _objectToEdit As String = ""
 Private _sourceLocale As String = "en-US"
 Private _useSecondLocalization As Boolean = False
 Private _copyEmpty As Boolean = False
 Private _module As DesktopModuleInfo = Nothing
 Private _filelist As SortedList = Nothing
 Private _editor As Globals.EditorType = Globals.EditorType.Textbox
#End Region

#Region " Event Handlers "
 Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
  If Not Me.Request.Params("Editor") Is Nothing Then
   _editor = CType(System.Enum.Parse(GetType(Globals.EditorType), Me.Request.Params("Editor")), Globals.EditorType)
  End If
  _targetLocale = Me.Request.Params("Locale")
  _sourceLocale = Me.Request.Params("SourceLocale")
  _useSecondLocalization = (_sourceLocale <> "en-US")
  _objectToEdit = Me.Request.Params("Module")
  If Me.Request.Params("CopyEmpty") IsNot Nothing Then
   _copyEmpty = CBool(Me.Request.Params("CopyEmpty"))
  End If
  BindData()
  Select Case _editor
   Case Globals.EditorType.DNNLabel
    cmdReturn.Text = GetString("cmdReturn", Me.LocalResourceFile)
    cmdCancel.Visible = False
   Case Globals.EditorType.Textbox
    cmdReturn.Text = GetString("cmdSave", Me.LocalResourceFile)
    cmdCancel.Visible = True
  End Select
 End Sub

 Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
  UpdateData()
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
 End Sub

 Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
 End Sub
#End Region

#Region " Private Methods "
 Private Sub BindData()

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

  For Each file As DictionaryEntry In _filelist

   Dim shc As UI.UserControls.SectionHeadControl = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
   shc.Section = file.Key.ToString.Replace(Server.MapPath("~"), "").Replace(".", "_").Replace("\", "_")
   shc.IncludeRule = True
   shc.IsExpanded = False
   shc.CssClass = "Head"
   shc.Text = file.Key.ToString.Replace(Server.MapPath("~"), "")

   Dim table As New HtmlTable
   table.ID = file.Key.ToString.Replace(Server.MapPath("~"), "").Replace(".", "_").Replace("\", "_")
   table.Attributes.Add("class", "fileTable")
   Dim localizationFile As String = Globals.ResourceFile(file.Key.ToString, _targetLocale)
   Dim localizationFileExists As Boolean = IO.File.Exists(localizationFile)
   Dim sourceLocalizationFile As String = Globals.ResourceFile(file.Key.ToString, _sourceLocale)
   Dim sourceLocalizationFileExists As Boolean = IO.File.Exists(sourceLocalizationFile)
   Dim originalHasDoubles As Boolean = False
   Dim sourceHasDoubles As Boolean = False
   Dim destinationHasDoubles As Boolean = False

   Dim dsDef As New DataSet
   Dim dsRes As New DataSet
   Dim dtDef As DataTable = Nothing
   Dim dtSrc As DataTable = Nothing
   Dim dtRes As DataTable = Nothing

   Try
    dsDef.ReadXml(file.Key.ToString)
    dtDef = dsDef.Tables("data")
    dtDef.TableName = "default"
   Catch
    Throw New Exception("Original resource file '" & file.Key.ToString & "' is incorrect")
    Exit Sub
   End Try
   Try
    dtDef.PrimaryKey = New DataColumn() {dtDef.Columns("name")}
   Catch ex As Exception
    originalHasDoubles = True
   End Try
   If localizationFileExists Then
    Try
     dsRes.ReadXml(localizationFile)
     dtRes = dsRes.Tables("data").Copy
     dtRes.TableName = "localized"
     dsDef.Tables.Add(dtRes)
    Catch
     localizationFileExists = False
    Finally
     Try
      dtRes.PrimaryKey = New DataColumn() {dtRes.Columns("name")}
     Catch ex As Exception
      destinationHasDoubles = True
     End Try
    End Try
   End If
   If sourceLocalizationFileExists And _useSecondLocalization Then
    Try
     Dim dsSrc As New DataSet
     dsSrc.ReadXml(sourceLocalizationFile)
     dtSrc = dsSrc.Tables("data").Copy
     dtSrc.TableName = "source"
     dsDef.Tables.Add(dtSrc)
     dsSrc.Dispose()
    Catch
     sourceLocalizationFileExists = False
    Finally
     Try
      dtSrc.PrimaryKey = New DataColumn() {dtSrc.Columns("name")}
     Catch ex As Exception
      sourceHasDoubles = True
     End Try
    End Try
   End If

   Dim row As New HtmlTableRow
   Dim cell As New HtmlTableCell
   cell.Controls.Add(New LiteralControl(GetString("Key.Header", Me.LocalResourceFile)))
   cell.Attributes.Add("class", "fileTable_Header")
   row.Cells.Add(cell)
   cell = New HtmlTableCell
   cell.Controls.Add(New LiteralControl(GetString("Default.Header", Me.LocalResourceFile)))
   cell.Attributes.Add("class", "fileTable_Header")
   row.Cells.Add(cell)
   If _useSecondLocalization Then
    cell = New HtmlTableCell
    cell.Controls.Add(New LiteralControl(_sourceLocale))
    cell.Attributes.Add("class", "fileTable_Header")
    row.Cells.Add(cell)
   End If
   cell = New HtmlTableCell
   cell.Controls.Add(New LiteralControl(_targetLocale))
   cell.Attributes.Add("class", "fileTable_Header")
   row.Cells.Add(cell)
   table.Rows.Add(row)

   If Not (originalHasDoubles Or destinationHasDoubles Or sourceHasDoubles) Then
    Dim dv As New DataView(dtDef, "", "name", DataViewRowState.CurrentRows)
    For Each drv As DataRowView In dv
     Dim source As String = ""
     Dim trans As String = Nothing
     If localizationFileExists Then
      Dim dr As DataRow = dtRes.Rows.Find(drv("name"))
      If Not dr Is Nothing Then
       trans = CStr(dr.Item("value"))
      End If
     End If
     If sourceLocalizationFileExists And _useSecondLocalization Then
      Dim dr As DataRow = dtSrc.Rows.Find(drv("name"))
      If Not dr Is Nothing Then
       source = CStr(dr.Item("value"))
      End If
     End If
     table.Rows.Add(MakeRow(localizationFile, CStr(drv("name")), CStr(drv("value")), source, trans))
    Next
    If localizationFileExists Then
     dv = New DataView(dtRes, "", "name", DataViewRowState.CurrentRows)
     For Each drv As DataRowView In dv
      Dim dr As DataRow = dtDef.Rows.Find(drv("name"))
      If dr Is Nothing Then
       Dim source As String = ""
       If dtSrc IsNot Nothing Then
        Dim drSrc As DataRow = dtSrc.Rows.Find(drv("name"))
        If drSrc IsNot Nothing Then
         source = CStr(drSrc.Item("value"))
        End If
       End If
       table.Rows.Add(MakeRow(localizationFile, CStr(drv("name")), Nothing, source, CStr(drv("value"))))
      End If
     Next
    End If
   Else
   End If

   PlaceHolder1.Controls.Add(shc)
   PlaceHolder1.Controls.Add(table)
   PlaceHolder1.Controls.Add(New LiteralControl("<br>"))

  Next

 End Sub

 Private Function MakeRow(ByVal TranslatedFile As String, ByVal key As String, ByVal original As String, ByVal source As String, ByVal translation As String) As HtmlTableRow

  Dim row As New HtmlTableRow
  Dim cell As New HtmlTableCell
  cell.Controls.Add(New LiteralControl(key))
  cell.Attributes.Add("class", "fileTable_Key")
  row.Cells.Add(cell)
  cell = New HtmlTableCell
  If original IsNot Nothing Then
   cell.Controls.Add(New LiteralControl(original))
  End If
  cell.Attributes.Add("class", "fileTable_Original")
  row.Cells.Add(cell)
  If _useSecondLocalization Then
   cell = New HtmlTableCell
   If source IsNot Nothing Then
    cell.Controls.Add(New LiteralControl(source))
   End If
   cell.Attributes.Add("class", "fileTable_Original")
   row.Cells.Add(cell)
  End If
  If _copyEmpty AndAlso translation = "" Then
   If source = "" Then
    translation = original
   Else
    translation = source
   End If
  End If
  cell = New HtmlTableCell
  Select Case _editor
   Case Globals.EditorType.DNNLabel
    Dim editor As Editor = CType(Me.LoadControl("Controls/Editor.ascx"), Editor)
    With editor
     .ResourceFile = TranslatedFile
     .ResourceKey = key
     If original IsNot Nothing Then
      If original.Contains("<") Then
       .IsHtml = True
      End If
     End If
     If translation IsNot Nothing Then
      .OriginalValue = translation
     End If
    End With
    cell.Attributes.Add("class", "fileTable_Translated")
    cell.Controls.Add(editor)
   Case Globals.EditorType.Textbox
    Dim editor As TBEditor = CType(Me.LoadControl("Controls/TBEditor.ascx"), TBEditor)
    With editor
     .ResourceFile = TranslatedFile
     .ResourceKey = key
     If translation IsNot Nothing Then
      .Value = translation
     End If
     If original IsNot Nothing Then
      .OriginalLength = original.Length
     End If
    End With
    cell.Attributes.Add("class", "fileTable_Translated")
    cell.Controls.Add(editor)
  End Select
  row.Cells.Add(cell)
  cell = New HtmlTableCell
  Return row

 End Function

 Private Sub UpdateData()

  Select Case _editor
   Case Globals.EditorType.DNNLabel
    ' updates are handled in ajax
   Case Globals.EditorType.Textbox
    For Each x As Control In PlaceHolder1.Controls
     If TypeOf x Is HtmlTable Then
      Dim table As HtmlTable = CType(x, HtmlTable)
      For Each row As HtmlTableRow In table.Rows
       Dim cell As HtmlTableCell = row.Cells(row.Cells.Count - 1)
       If TypeOf cell.Controls(0) Is TBEditor Then
        Dim tbEdit As TBEditor = CType(cell.Controls(0), TBEditor)
        tbEdit.UpdateValue()
       End If
      Next
     End If
    Next
  End Select

 End Sub
#End Region

End Class
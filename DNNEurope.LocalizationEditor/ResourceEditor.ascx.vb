' 
' Copyright (c) 2004-2011 DNN-Europe, http://www.dnn-europe.net
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this 
' software and associated documentation files (the "Software"), to deal in the Software 
' without restriction, including without limitation the rights to use, copy, modify, merge, 
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
' to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or 
' substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
' ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
' 
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports Google.API
Imports DNNEurope.Modules.LocalizationEditor.Controls
Imports DotNetNuke.Framework
Imports Google.API.Translate
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.UserControls
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Entities.Permissions
Imports DNNEurope.Modules.LocalizationEditor.Entities.Translations
Imports DNNEurope.Modules.LocalizationEditor.Entities.Statistics

Partial Public Class ResourceEditor
 Inherits ModuleBase

#Region " Private Members "

 Private _ObjectId As Integer = -1
 Private _locale As String = ""
 Private _moduleFriendlyName As String = ""
 Private _version As String = ""
 Private _sourceLocale As String = ""
 Private _selection As String = "all"
 Private _editor As String = ""
 Private _useSecondLocalization As Boolean = False
 Private _autoTranslate As Boolean

#End Region

#Region " Properties "

 Public Property ModuleFriendlyName() As String
  Get
   Return _moduleFriendlyName
  End Get
  Set(ByVal value As String)
   _moduleFriendlyName = value
  End Set
 End Property

 Public Property Locale() As String
  Get
   Return _locale
  End Get
  Set(ByVal value As String)
   _locale = value
  End Set
 End Property

 Public Property SourceLocale() As String
  Get
   Return _sourceLocale
  End Get
  Set(ByVal value As String)
   _sourceLocale = value
  End Set
 End Property

 Public Property ObjectId() As Integer
  Get
   Return _ObjectId
  End Get
  Set(ByVal value As Integer)
   _ObjectId = value
  End Set
 End Property

 Public Property Version() As String
  Get
   Return _version
  End Get
  Set(ByVal value As String)
   _version = value
  End Set
 End Property

 Public Property Selection() As String
  Get
   Return _selection
  End Get
  Set(ByVal value As String)
   _selection = value
  End Set
 End Property

 Public Property Editor() As String
  Get
   Return _editor
  End Get
  Set(ByVal value As String)
   _editor = value
  End Set
 End Property

 Public Property UseSecondLocalization() As Boolean
  Get
   Return _useSecondLocalization
  End Get
  Set(ByVal value As Boolean)
   _useSecondLocalization = value
  End Set
 End Property

 Public Property AutoTranslate() As Boolean
  Get
   Return _autoTranslate
  End Get
  Set(ByVal value As Boolean)
   _autoTranslate = value
  End Set
 End Property

#End Region

#Region " Event Handlers "

 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  Globals.ReadValue(Me.Request.Params, "ObjectId", ObjectId)
  Globals.ReadValue(Me.Request.Params, "Locale", Locale)
  Globals.ReadValue(Me.Request.Params, "SourceLocale", SourceLocale)
  Globals.ReadValue(Me.Request.Params, "Version", Version)
  Globals.ReadValue(Me.Request.Params, "Selection", Selection)
  Globals.ReadValue(Me.Request.Params, "Editor", Editor)
  If Me.Request.Params("AutoTranslate") = "1" Then AutoTranslate = True

  Dim tm As ObjectInfo = ObjectsController.GetObject(ObjectId)
  If tm Is Nothing Then Throw New ArgumentException(String.Format("ObjectId with value {0} is not valid.", ObjectId))
  ModuleFriendlyName = tm.FriendlyName

  UseSecondLocalization = (SourceLocale <> "")
  AJAX.RegisterScriptManager()
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
  AJAX.RegisterPostBackControl(cmdUpdate)
  AJAX.RegisterPostBackControl(cmdSave)

  If Not Me.IsPostBack Then
   If Not PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ModuleId, Locale) Then
    Throw New Exception("Access denied")
   End If
  End If
  DataBind()
 End Sub

 Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
 End Sub

 Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
  Update()
 End Sub

 Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
  Update()
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
 End Sub

 'Protected Sub AjaxTimerTick(ByVal sender As Object, ByVal e As EventArgs)
 '    lblTimeCheck.Text = Now.ToString
 'End Sub

#End Region

#Region " Private Methods "

 Private Sub Update()
  For Each ctlTable As Control In PlaceHolder1.Controls
   If TypeOf (ctlTable) Is HtmlTable Then
    Dim IsFirstOrSecond As Integer = 0
    For Each row As HtmlTableRow In CType(ctlTable, HtmlTable).Rows
     If IsFirstOrSecond < 2 Then
      'Skip the first 2 rows, for they are headers.
      IsFirstOrSecond += 1
     Else
      Dim cell As HtmlTableCell = Nothing
      If UseSecondLocalization Then
       cell = row.Cells.Item(5)
      Else
       cell = row.Cells.Item(3)
      End If
      Dim txtBox As Editor = CType(cell.Controls(0), Editor)
      If txtBox.Value <> "" Then
       Dim trans As TranslationInfo = TranslationsController.GetTranslation(txtBox.TextId, Locale)
       Dim stat As Integer = txtBox.Value.Length
       If trans Is Nothing Then
        trans = New TranslationInfo(txtBox.TextId, Locale, Now, UserId, txtBox.Value)
        TranslationsController.SetTranslation(trans)
       ElseIf trans.TextValue <> txtBox.Value Then ' editor has changed the text
        If Settings.KeepStatistics Then
         If txtBox.Value.Length > 200 Then
          stat = Math.Abs(txtBox.Value.Length - trans.TextValue.Length)
         Else
          stat = Globals.LevenshteinDistance(trans.TextValue, txtBox.Value)
         End If
        End If
        trans.TextValue = txtBox.Value
        trans.LastModified = Now
        trans.LastModifiedUserId = UserId
        TranslationsController.SetTranslation(trans)
       End If
       If stat > 0 And Settings.KeepStatistics Then
        StatisticsController.RecordStatistic(txtBox.TextId, Locale, UserId, stat)
       End If
      Else
       TranslationsController.DeleteTranslation(txtBox.TextId, Locale)
      End If
     End If
    Next
   End If
  Next
 End Sub

#End Region

#Region " Overrides "

 Public Overrides Sub DataBind()
  'Determine the source and destination languages from the locales.
  Dim defaultLanguage As Language = Language.English
  'TODO: Make default language configurable?
  Dim fromLanguage As Language = defaultLanguage
  'Google.API.Translate.LanguageUtility.GetLanguageFromLocale(SourceLocale)
  Dim toLanguage As Language = LanguageUtility.GetLanguageFromLocale(Locale)
  If fromLanguage = Language.Unknown Or Not LanguageUtility.IsTranslatable(fromLanguage) Then
   fromLanguage = defaultLanguage
   'TODO: Message when not translatable.
  End If
  If toLanguage = Language.Unknown And Locale.Length = 2 Then
   toLanguage = LanguageUtility.GetLanguageFromLocale(Locale & "-" & Locale.ToUpper)
  End If
  If toLanguage = Language.Unknown Or Not LanguageUtility.IsTranslatable(toLanguage) Then
   toLanguage = defaultLanguage
   'TODO: Message when not translatable.
  End If

  Dim originalText As String = Localization.GetString("Default.Header", Me.LocalResourceFile)
  Dim transferText As String = Localization.GetString("Transfer", Me.LocalResourceFile)
  Dim transferImage1 As String = ResolveUrl("~/images/rt.gif")
  Dim transferImage2 As String = ResolveUrl("~/images/rt.gif")
  Dim TransferAllText As String = Localization.GetString("TransferAll", Me.LocalResourceFile)
  Dim translateText As String = Localization.GetString("Translate", Me.LocalResourceFile)
  Dim translateImage As String = ResolveUrl("~/images/icon_wizard_16px.gif")
  Dim TranslateAllText As String = Localization.GetString("TranslateAll", Me.LocalResourceFile)
  Dim Translator As String = Localization.GetString("Translator", Me.LocalResourceFile)
  Dim ClearAllText As String = Localization.GetString("ClearAll", Me.LocalResourceFile)

  Dim crtFile As String = ""
  Dim table As HtmlTable = Nothing
  Dim row As HtmlTableRow = Nothing
  Dim cell As HtmlTableCell = Nothing
  Dim PanelID As Integer = 0
  Dim btn As New HtmlImage
  Dim hyp As New HyperLink

  Dim originalValue As String
  Dim sourceValue As String
  Dim textValue As String

  '2009-06-24 Janga:  Add the button bar at the top of the page.
  'Button bar of page.
  table = New HtmlTable
  'table.ID = "janga"
  table.CellSpacing = 8
  PlaceHolder1.Controls.Add(table)

  row = New HtmlTableRow

  'Transfer all (original locale).
  cell = New HtmlTableCell
  'Cell #1.
  hyp = New HyperLink
  hyp.Text = TransferAllText & " (" & originalText & ")"
  hyp.CssClass = "CommandButton"
  hyp.Attributes.Add("style", "cursor:pointer;")
  hyp.Attributes.Add("onclick", "Javascript:transferAllTextValues(" & PanelID.ToString & ", " & "'original'" & ")" & "; return false;")
  hyp.NavigateUrl = "#_self"
  cell.Controls.Add(hyp)
  row.Cells.Add(cell)
  'Cell #1.

  If UseSecondLocalization Then
   'Transfer all (source locale).
   cell = New HtmlTableCell
   'Cell #2.
   hyp = New HyperLink
   hyp.Text = TransferAllText & " (" & SourceLocale & ")"
   hyp.CssClass = "CommandButton"
   hyp.Attributes.Add("style", "cursor:pointer;")
   hyp.Attributes.Add("onclick", "Javascript:transferAllTextValues(" & PanelID.ToString & ", " & "'source'" & ")" & "; return false;")
   hyp.NavigateUrl = "#_self"
   cell.Controls.Add(hyp)
   row.Cells.Add(cell)
   'Cell #2.
  End If

  'Clear all
  cell = New HtmlTableCell
  'Cell #3.
  hyp = New HyperLink
  hyp.Text = ClearAllText
  hyp.CssClass = "CommandButton"
  hyp.Attributes.Add("style", "cursor:pointer;")
  hyp.Attributes.Add("onclick", "Javascript:clearAllTextValues(" & PanelID.ToString & ")" & "; return false;")
  cell.Controls.Add(hyp)
  hyp.NavigateUrl = "#_self"
  row.Cells.Add(cell)
  'Cell #3.

  'Translate all
  cell = New HtmlTableCell
  'Cell #4.
  hyp = New HyperLink
  hyp.Text = TranslateAllText
  hyp.CssClass = "CommandButton"
  hyp.Attributes.Add("style", "cursor:pointer;")
  hyp.Attributes.Add("onclick", "Javascript:translateAllTextValues(" & PanelID.ToString & ", " & "'destination'" & ")" & "; return false;")
  hyp.NavigateUrl = "#_self"
  cell.Controls.Add(hyp)
  row.Cells.Add(cell)
  'Cell #4.

  table.Rows.Add(row)
  'End Button bar of page.

  'Show panels.
  Dim dt As DataTable = DotNetNuke.Common.ConvertDataReaderToDataTable(DataProvider.Instance().GetTranslationList(ObjectId, Locale, SourceLocale, Version))
  Dim dv As New DataView(dt)
  Select Case Selection.ToLower
   Case "all"
   Case "new"
    dv.RowFilter = "Version='" & Version & "'"
   Case "untranslated"
    dv.RowFilter = "TextValue IS NULL"
   Case Else
    dv.RowFilter = "FilePath='" & Selection & "'"
  End Select
  dv.Sort = "FilePath, TextKey"

  Dim firstResourceFile As Boolean = True
  For Each drv As DataRowView In dv
   Dim filePath As String = CStr(drv.Item("FilePath"))

   If filePath <> crtFile Then
    PlaceHolder1.Controls.Add(New LiteralControl("<br />"))
    PanelID += 1
    Dim shc As SectionHeadControl = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), SectionHeadControl)
    shc.Section = filePath.Replace(".", "_").Replace("\", "_")
    shc.IncludeRule = True
    shc.IsExpanded = False
    shc.CssClass = "Head"
    shc.Text = filePath
    If firstResourceFile Then
     shc.IsExpanded = True
     firstResourceFile = False
    End If
    PlaceHolder1.Controls.Add(shc)

    table = New HtmlTable
    table.ID = filePath.Replace(".", "_").Replace("\", "_")
    table.Attributes.Add("class", "fileTable")
    PlaceHolder1.Controls.Add(table)

    '2009-06-18 Janga:  Add the button bar at the top of the panel.
    'Button bar per panel.
    row = New HtmlTableRow

    'Empty header for spacing.
    cell = New HtmlTableCell
    'Cell #1.
    row.Cells.Add(cell)
    'Cell #1.

    'Transfer all (original locale).
    cell = New HtmlTableCell
    'Cell #2.
    hyp = New HyperLink
    hyp.Text = TransferAllText
    hyp.CssClass = "CommandButton"
    hyp.Attributes.Add("style", "cursor:pointer;")
    hyp.Attributes.Add("onclick", "Javascript:transferAllTextValues(" & PanelID.ToString & ", " & "'original'" & ")" & "; return false;")
    cell.Controls.Add(hyp)
    hyp.NavigateUrl = "#_self"
    row.Cells.Add(cell)
    'Cell #2.

    'Empty header for transfer button.
    cell = New HtmlTableCell
    'Cell #3.
    row.Cells.Add(cell)
    'Cell #3.

    If UseSecondLocalization Then
     'Transfer all (source locale).
     cell = New HtmlTableCell
     'Cell #4.
     hyp = New HyperLink
     hyp.Text = TransferAllText
     hyp.CssClass = "CommandButton"
     hyp.Attributes.Add("style", "cursor:pointer;")
     hyp.Attributes.Add("onclick", "Javascript:transferAllTextValues(" & PanelID.ToString & ", " & "'source'" & ")" & "; return false;")
     hyp.NavigateUrl = "#_self"
     cell.Controls.Add(hyp)
     row.Cells.Add(cell)
     'Cell #4.

     'Empty header for transfer button.
     cell = New HtmlTableCell
     'Cell #5.
     row.Cells.Add(cell)
     'Cell #5.
    End If

    'Clear all
    cell = New HtmlTableCell
    'Cell #6.
    hyp = New HyperLink
    hyp.Text = ClearAllText
    hyp.CssClass = "CommandButton"
    hyp.Attributes.Add("style", "cursor:pointer;")
    hyp.Attributes.Add("onclick", "Javascript:clearAllTextValues(" & PanelID.ToString & ")" & "; return false;")
    hyp.NavigateUrl = "#_self"
    cell.Controls.Add(hyp)
    row.Cells.Add(cell)
    'Cell #6.

    'Translate all
    cell = New HtmlTableCell
    'Cell #7.
    hyp = New HyperLink
    hyp.Text = TranslateAllText
    hyp.CssClass = "CommandButton"
    hyp.Attributes.Add("style", "cursor:pointer;")
    hyp.Attributes.Add("onclick", "Javascript:translateAllTextValues(" & PanelID.ToString & ", " & "'destination'" & ")" & "; return false;")
    hyp.NavigateUrl = "#_self"
    cell.Controls.Add(hyp)
    row.Cells.Add(cell)
    'Cell #7.

    table.Rows.Add(row)
    'End Button bar per panel.

    row = New HtmlTableRow

    cell = New HtmlTableCell
    'Cell #1.
    cell.Controls.Add(New LiteralControl(Localization.GetString("Key.Header", Me.LocalResourceFile)))
    cell.Attributes.Add("class", "fileTable_Header")
    row.Cells.Add(cell)
    'Cell #1.

    cell = New HtmlTableCell
    'Cell #2.
    cell.Controls.Add(New LiteralControl(originalText))
    cell.Attributes.Add("class", "fileTable_Header")
    row.Cells.Add(cell)
    'Cell #2.

    ' Empty header for transfer button.
    cell = New HtmlTableCell
    'Cell #3.
    row.Cells.Add(cell)
    'Cell #3.

    If UseSecondLocalization Then
     cell = New HtmlTableCell
     'Cell #4.
     cell.Controls.Add(New LiteralControl(SourceLocale))
     cell.Attributes.Add("class", "fileTable_Header")
     row.Cells.Add(cell)
     'Cell #4.

     ' Empty header for transfer button.
     cell = New HtmlTableCell
     'Cell #5.
     row.Cells.Add(cell)
     'Cell #5.
    End If

    cell = New HtmlTableCell
    'Cell #6.
    cell.Controls.Add(New LiteralControl(Locale))
    cell.Attributes.Add("class", "fileTable_Header")
    row.Cells.Add(cell)
    'Cell #6.

    ' Empty header for translate button.
    cell = New HtmlTableCell
    'Cell #7.
    row.Cells.Add(cell)
    'Cell #7.

    table.Rows.Add(row)
    crtFile = filePath
   End If

   row = New HtmlTableRow
   table.Rows.Add(row)

   Dim key As String = CStr(drv.Item("TextKey"))
   Dim TextId As Integer = Globals.GetAnInteger(drv.Item("TextId"))
   originalValue = Globals.GetAString(drv.Item("OriginalValue"))
   sourceValue = Globals.GetAString(drv.Item("SourceValue"))
   textValue = Globals.GetAString(drv.Item("TextValue"))

   Dim OriginalLength As Integer = originalValue.Length
   Dim OriginalHtmlAny As Boolean = Globals.IsAnyHtml(originalValue)
   Dim height As Integer = 0
   Dim width As Integer = 0
   If OriginalLength > 300 Then
    height = 200
    width = 300
   ElseIf OriginalLength > 100 Then
    height = 100
    width = 300
   ElseIf OriginalLength > 50 Or OriginalHtmlAny Then
    height = 50
    width = 200
   End If

   cell = New HtmlTableCell
   'Cell #1.
   cell.Controls.Add(New LiteralControl(key))
   cell.Attributes.Add("class", "fileTable_Key")
   row.Cells.Add(cell)
   'Cell #1.

   cell = New HtmlTableCell
   'Cell #2.
   If height > 50 Or OriginalHtmlAny Then
    Dim tb As New TextBox
    With tb
     .Width = Unit.Pixel(width)
     .Height = Unit.Pixel(height)
     .TextMode = TextBoxMode.MultiLine
     .Text = originalValue
     .ReadOnly = True
    End With
    cell.Controls.Add(tb)
   Else
    cell.Controls.Add(New LiteralControl(originalValue))
    If height > 0 Then
     cell.Width = width.ToString
    End If
   End If
   cell.Attributes.Add("class", "fileTable_Original")
   row.Cells.Add(cell)
   'Cell #2.

   cell = New HtmlTableCell
   'Cell #3.
   btn = New HtmlImage
   With btn
    .Alt = String.Format(transferText, originalText, Locale)
    .Src = transferImage1
    .Border = 0
    .Attributes.Add("title", String.Format(transferText, originalText, Locale))
    .Attributes.Add("style", "cursor:pointer;")
    .Attributes.Add("onclick", "Javascript:setTextValue('" & Me.ClientID & "', " & PanelID.ToString & ", " & TextId.ToString & ", '" & originalValue.Replace("'", "\'").Replace(vbCrLf, "\r\n") & "');")
    .Attributes.Add("panelID", PanelID.ToString)
    .Attributes.Add("typeoflocale", "original")
   End With
   cell.Controls.Add(btn)
   row.Cells.Add(cell)
   'Cell #3.

   If UseSecondLocalization Then
    cell = New HtmlTableCell
    'Cell #4.
    If height > 50 Or OriginalHtmlAny Then
     Dim tb As New TextBox
     With tb
      .Width = Unit.Pixel(width)
      .Height = Unit.Pixel(height)
      .TextMode = TextBoxMode.MultiLine
      .Text = sourceValue
      .ReadOnly = True
     End With
     cell.Controls.Add(tb)
    Else
     cell.Controls.Add(New LiteralControl(sourceValue))
     If height > 0 Then
      cell.Width = width.ToString
     End If
    End If
    cell.Attributes.Add("class", "fileTable_Original")
    row.Cells.Add(cell)
    'Cell #4.

    cell = New HtmlTableCell
    'Cell #5.
    btn = New HtmlImage
    With btn
     .Alt = String.Format(transferText, SourceLocale, Locale)
     .Src = transferImage2
     .Border = 0
     .Attributes.Add("title", String.Format(transferText, SourceLocale, Locale))
     .Attributes.Add("style", "cursor:pointer;")
     .Attributes.Add("onclick", "Javascript:setTextValue('" & Me.ClientID & "', " & PanelID.ToString & ", " & TextId.ToString & ", '" & sourceValue.Replace("'", "\'").Replace(vbCrLf, "\r\n") & "');")
     .Attributes.Add("panelID", PanelID.ToString)
     .Attributes.Add("typeoflocale", "source")
    End With
    cell.Controls.Add(btn)
    row.Cells.Add(cell)
    'Cell #5.
   End If

   cell = New HtmlTableCell
   'Cell #6.
   Dim editor As Editor = CType(Me.LoadControl("Controls/Editor.ascx"), Editor)
   editor.ID = "panel" & PanelID.ToString & "edit" & TextId.ToString
   editor.TextId = TextId
   editor.Value = textValue
   editor.OriginalValue = originalValue
   editor.SourceValue = sourceValue
   editor.FromLocale = SourceLocale
   editor.ToLocale = Locale
   editor.AutoTranslate = AutoTranslate
   editor.PanelID = PanelID
   cell.Controls.Add(editor)
   row.Cells.Add(cell)
   'Cell #6.

   cell = New HtmlTableCell
   'Cell #7.
   btn = New HtmlImage
   With btn
    .Alt = String.Format(translateText, Translator, originalText, Locale)
    .Src = translateImage
    .Border = 0
    .Attributes.Add("title", String.Format(translateText, Translator, originalText, Locale))
    .Attributes.Add("style", "cursor:pointer;")
    .Attributes.Add("onclick", "Javascript:translateTextValue('" & Me.ClientID & "', " & PanelID.ToString & ", " & TextId.ToString & ", '" & originalValue.Replace("'", "\'").Replace(vbCrLf, "\r\n") & "', '" & fromLanguage.ToString & "', '" & toLanguage.ToString & "', '" & defaultLanguage.ToString & "');")
    .Attributes.Add("panelID", PanelID.ToString)
    .Attributes.Add("typeoflocale", "destination")
   End With
   cell.Controls.Add(btn)
   row.Cells.Add(cell)
   'Cell #7.
  Next
 End Sub

#End Region

End Class

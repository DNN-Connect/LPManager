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
Imports DotNetNuke.Common
Imports DNNEurope.Modules.LocalizationEditor.Business
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Generic


Partial Public Class Import
 Inherits ModuleBase

#Region " Private Members "

 Private _ObjectId As Integer = -2
 Private _locale As String = ""
 Private _moduleFriendlyName As String = ""
 Private _tempDirectory As String = ""

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

 Public Property ObjectId() As Integer
  Get
   Return _ObjectId
  End Get
  Set(ByVal value As Integer)
   _ObjectId = value
  End Set
 End Property

 Public Property TempDirectory() As String
  Get
   Return _tempDirectory
  End Get
  Set(ByVal value As String)
   _tempDirectory = value
  End Set
 End Property

#End Region

#Region " Event Handlers "

 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

  Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
  Globals.ReadQuerystringValue(Me.Request.Params, "Locale", Locale)

  Dim objObjectInfo As ObjectInfo = ObjectController.GetObject(ObjectId)
  If objObjectInfo Is Nothing Then Return
  ModuleFriendlyName = objObjectInfo.FriendlyName
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

  '// Force full postback for wizard
  AJAX.RegisterPostBackControl(wzdImport)

  wzdImport.CancelButtonText = "<img src=""" + ApplicationPath + "/images/cancel.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)
  wzdImport.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
  wzdImport.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
  wzdImport.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
  wzdImport.FinishPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
  wzdImport.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/save.gif"" border=""0"" /> " + Localization.GetString("Finish", Me.LocalResourceFile)

  If Not Me.IsPostBack Then

   ' Permission check here
   If Locale = "" Or ObjectId = -2 Then
    Throw New Exception("Access denied")
   End If
   If Not PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ModuleId, Locale) Then
    Throw New Exception("Access denied")
   End If

   ddVersion.DataSource = DataProvider.Instance.GetVersions(Me.ObjectId)
   ddVersion.DataBind()

  End If


 End Sub

 Private Sub wzdImport_NextButtonClick(ByVal sender As Object, ByVal e As WizardNavigationEventArgs) Handles wzdImport.NextButtonClick

  Select Case e.CurrentStepIndex
   Case 0 'Upload
    'Before we leave Page 1, the user must have uploaded a valid resource file
    If Not ctlUpload.HasFile Then
     lblUploadError.Text = Localization.GetString("NoFile", LocalResourceFile)
     e.Cancel = True
     Exit Sub
    End If
    TempDirectory = PortalSettings.HomeDirectoryMapPath & "LocalizationEditor\~tmp" & Now.ToString("yyyyMMdd-hhmmss") & "-" & (CInt(Rnd() * 1000)).ToString
    Dim rep As New StringBuilder
    If Not UnpackUploadedFile(rep) Then
     lblUploadError.Text = Localization.GetString("WrongFile", LocalResourceFile)
     lblUploadReport.Text = rep.ToString.Replace(vbCrLf, "<br />")
     e.Cancel = True
     Exit Sub
    Else
     txtResult.Text = rep.ToString
    End If
   Case 1 ' Parameters
    txtAnalysis.Text = AnalyzePack()
  End Select

 End Sub

 Private Sub wzdImport_CancelButtonClick(ByVal sender As Object, ByVal e As EventArgs) Handles wzdImport.CancelButtonClick
  Try
   If TempDirectory <> "" Then
    Directory.Delete(TempDirectory)
   End If
  Catch
  End Try
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
 End Sub

 Private Sub wzdImport_FinishButtonClick(ByVal sender As Object, ByVal e As WizardNavigationEventArgs) Handles wzdImport.FinishButtonClick
  ImportPack()
  Try
   If TempDirectory <> "" Then
    Directory.Delete(TempDirectory)
   End If
  Catch
  End Try
  Me.Response.Redirect(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & Locale), False)
 End Sub

#End Region

#Region " Private Methods "

 Private Function UnpackUploadedFile(ByRef report As StringBuilder) As Boolean
  If Not Directory.Exists(TempDirectory) Then
   Directory.CreateDirectory(TempDirectory)
  Else
   Throw New Exception("New directory " & TempDirectory & " already exists")
  End If
  Try
   Dim resFilename As String = ctlUpload.FileName
   report.AppendLine("Uploaded " & resFilename)
   report.AppendLine("Unpacking to " & TempDirectory)
   Dim objZipEntry As ZipEntry
   Using objZipInputStream As New ZipInputStream(ctlUpload.FileContent)
    objZipEntry = objZipInputStream.GetNextEntry
    While Not objZipEntry Is Nothing
     Dim strFileName As String = objZipEntry.Name.Replace("/", "\")
     report.AppendLine("Unpacking " & strFileName)
     If strFileName <> "" And Not objZipEntry.IsDirectory Then
      Dim sFile As String = strFileName
      Dim sPath As String = TempDirectory & "\"
      If strFileName.IndexOf("\"c) > 0 Then
       sFile = Mid(strFileName, strFileName.LastIndexOf("\"c) + 2)
       sPath = sPath & Left(strFileName, strFileName.LastIndexOf("\"c))
       If Not Directory.Exists(sPath) Then
        Directory.CreateDirectory(sPath)
       End If
       sPath &= "\"
      End If
      Using objFileStream As FileStream = File.Create(sPath & sFile)
       Dim intSize As Integer = 2048
       Dim arrData(2048) As Byte
       intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
       While intSize > 0
        objFileStream.Write(arrData, 0, intSize)
        intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
       End While
      End Using
     End If
     objZipEntry = objZipInputStream.GetNextEntry
    End While
   End Using
   Dim m As Match = Regex.Match(resFilename, "\d\d\.\d\d\.\d\d")
   Try
    ddVersion.ClearSelection()
    If m.Success Then
     Dim parsedVersion As String = m.Value
     ddVersion.Items.FindByText(parsedVersion).Selected = True
    Else
     ddVersion.Items(ddVersion.Items.Count - 1).Selected = True
    End If
   Catch
   End Try
  Catch ex As Exception
   report.AppendLine("Error Occurred: " & ex.Message)
   report.AppendLine(ex.StackTrace)
   Return False
  End Try
  Return True
 End Function

#Region " Analysis "
 Private Function AnalyzePack() As String
  Dim files As String() = IO.Directory.GetFiles(TempDirectory, "*.dnn")
  If files.Length > 0 Then
   Return AnalyzePackV5(files(0))
  ElseIf IO.File.Exists(TempDirectory & "\Manifest.Xml") Then
   Return AnalyzePackV3()
  Else
   Return "No manifest found"
  End If
 End Function

 Private Function AnalyzePackV3() As String
  Dim report As New StringBuilder
  Dim manifest As New XmlDocument
  manifest.Load(TempDirectory & "\Manifest.Xml")
  report.AppendLine("Loaded Manifest")
  For Each xNode As XmlNode In manifest.SelectNodes("LanguagePack/Files/File")
   Dim fileName As String = xNode.Attributes("FileName").InnerText
   Dim fileType As String = xNode.Attributes("FileType").InnerText
   Dim moduleName As String = ""
   If xNode.Attributes("ModuleName") IsNot Nothing Then
    moduleName = xNode.Attributes("ModuleName").InnerText & "\"
   End If
   Dim fileKey As String = fileType & "\" & moduleName & fileName
   AnalyzeFileV3(report, fileKey)
  Next
  Return report.ToString
 End Function

 Private Sub AnalyzeFileV3(ByRef report As StringBuilder, ByVal resFile As String)
  report.AppendLine("Analyzing " & resFile)
  Dim res As New XmlDocument
  res.Load(TempDirectory & "\" & resFile)
  Dim resFileKey As String = LocalizationController.GetCorrectPath(resFile, Localization.LocalResourceDirectory)
  report.AppendLine("Mapped to " & resFileKey)
  AnalyzeFile(report, res, resFileKey)
 End Sub

 Private Function AnalyzePackV5(ByVal manifestFile As String) As String
  Dim report As New StringBuilder
  Dim manifest As New XmlDocument
  manifest.Load(manifestFile)
  report.AppendLine("Loaded Manifest")
  Dim basePath As String = ""
  If manifest.SelectSingleNode("dotnetnuke/packages/package/components/component/languageFiles/basePath") IsNot Nothing Then
   basePath = manifest.SelectSingleNode("dotnetnuke/packages/package/components/component/languageFiles/basePath").InnerText
  End If
  If basePath <> "" And Not basePath.EndsWith("\") Then
   basePath &= "\"
  End If
  For Each xNode As XmlNode In manifest.SelectNodes("dotnetnuke/packages/package/components/component/languageFiles/languageFile")
   Dim filePath As String = xNode.SelectSingleNode("path").InnerText
   If filePath <> "" And Not filePath.EndsWith("\") Then
    filePath &= "\"
   End If
   Dim fileName As String = xNode.SelectSingleNode("name").InnerText
   Dim fileKey As String = basePath & filePath & Regex.Replace(fileName, "(?i)\.\w{2}(-\w+)?\.resx$(?-i)", ".resx")
   AnalyzeFileV5(report, filePath & fileName, fileKey)
  Next
  Return report.ToString
 End Function

 Private Sub AnalyzeFileV5(ByRef report As StringBuilder, ByVal tempResFile As String, ByVal resFileKey As String)
  report.AppendLine("Analyzing " & tempResFile)
  Dim res As New XmlDocument
  res.Load(TempDirectory & "\" & tempResFile)
  report.AppendLine("Mapped to " & resFileKey)
  AnalyzeFile(report, res, resFileKey)
 End Sub

 Private Sub AnalyzeFile(ByRef report As StringBuilder, ByVal resFile As XmlDocument, ByVal resFileKey As String)
  Using ir As IDataReader = DataProvider.Instance.GetTextsByObjectAndFile(ModuleId, ObjectId, resFileKey, Locale, ddVersion.SelectedValue, True)
   Do While ir.Read
    Dim textKey As String = CStr(ir.Item("TextKey"))
    Dim hasValue As Boolean = False
    If ir.Item("TextValue") IsNot DBNull.Value Then
     hasValue = True
    End If
    Try
     Dim xNode As XmlNode = resFile.SelectSingleNode("root/data[@name='" & textKey & "']")
     If xNode Is Nothing Then
      report.AppendLine("Nothing for " & textKey)
     Else
      If hasValue Then
       report.AppendLine("Overwrite " & textKey)
      Else
       report.AppendLine("Add " & textKey)
      End If
     End If
    Catch ex As XPathException
     report.AppendLine("!!!! Invalid token in attribute value: " & textKey)
    End Try
   Loop
  End Using
 End Sub
#End Region

#Region " Import "
 Private Sub ImportPack()
  Dim files As String() = IO.Directory.GetFiles(TempDirectory, "*.dnn")
  If files.Length > 0 Then
   ImportPackV5(files(0))
  ElseIf IO.File.Exists(TempDirectory & "\Manifest.Xml") Then
   ImportPackV3()
  End If
 End Sub

 Private Sub ImportPackV3()
  Dim manifest As New XmlDocument
  manifest.Load(TempDirectory & "\Manifest.Xml")
  For Each xNode As XmlNode In manifest.SelectNodes("LanguagePack/Files/File")
   Dim sFileName As String = xNode.Attributes("FileName").InnerText
   Dim sFileType As String = xNode.Attributes("FileType").InnerText
   Dim sModuleName As String = ""
   If xNode.Attributes("ModuleName") IsNot Nothing Then
    sModuleName = xNode.Attributes("ModuleName").InnerText & "\"
   End If
   Dim sPath As String = sFileType & "\" & sModuleName & sFileName
   ImportFileV3(sPath)
  Next
 End Sub

 Private Sub ImportFileV3(ByVal resFile As String)
  Dim res As New XmlDocument
  res.Load(TempDirectory & "\" & resFile)
  Dim resFileKey As String = LocalizationController.GetCorrectPath(resFile, Localization.LocalResourceDirectory)
  'Fix slashes (from / to \ )
  resFileKey = resFileKey.Replace("/"c, "\"c)
  ImportFile(res, resFileKey)
 End Sub

 Private Sub ImportPackV5(ByVal manifestFile As String)
  Dim manifest As New XmlDocument
  manifest.Load(manifestFile)
  Dim basePath As String = ""
  If manifest.SelectSingleNode("dotnetnuke/packages/package/components/component/languageFiles/basePath") IsNot Nothing Then
   basePath = manifest.SelectSingleNode("dotnetnuke/packages/package/components/component/languageFiles/basePath").InnerText
  End If
  If basePath <> "" And Not basePath.EndsWith("\") Then
   basePath &= "\"
  End If
  For Each xNode As XmlNode In manifest.SelectNodes("dotnetnuke/packages/package/components/component/languageFiles/languageFile")
   Dim filePath As String = xNode.SelectSingleNode("path").InnerText
   If filePath <> "" And Not filePath.EndsWith("\") Then
    filePath &= "\"
   End If
   Dim fileName As String = xNode.SelectSingleNode("name").InnerText
   Dim fileKey As String = basePath & filePath & Regex.Replace(fileName, "(?i)\.\w{2}(-\w+)?\.resx$(?-i)", ".resx")
   ImportFileV5(filePath & fileName, fileKey)
  Next
 End Sub

 Private Sub ImportFileV5(ByVal tempResFile As String, ByVal resFileKey As String)
  Dim res As New XmlDocument
  res.Load(TempDirectory & "\" & tempResFile)
  'Fix slashes (from / to \ )
  resFileKey = resFileKey.Replace("/"c, "\"c)
  ImportFile(res, resFileKey)
 End Sub

 Private Sub ImportFile(ByVal resFile As XmlDocument, ByVal resFileKey As String)
  Dim updateList As New List(Of TranslationInfo)
  Dim addList As New List(Of TranslationInfo)
  Dim addStatisticsList As New Dictionary(Of Integer, Integer)
  Using ir As IDataReader = DataProvider.Instance.GetTextsByObjectAndFile(ModuleId, ObjectId, resFileKey, Locale, ddVersion.SelectedValue, True)
   Do While ir.Read
    Dim textKey As String = CStr(ir.Item("TextKey"))
    Dim textId As Integer = CInt(ir.Item("TextId"))
    Dim hasValue As Boolean = False
    If ir.Item("TranslationId") IsNot DBNull.Value Then
     hasValue = True
    End If
    Try
     Dim xNode As XmlNode = resFile.SelectSingleNode("root/data[@name='" & textKey & "']")
     If xNode IsNot Nothing Then
      Try
       Dim transValue As String = xNode.SelectSingleNode("value").InnerText
       If hasValue Then
        Dim tr As TranslationInfo = TranslationsController.GetTranslation(textId, Locale)
        If tr.TextValue <> transValue Then
         Dim stat As Integer = 0
         If Settings.KeepStatistics Then
          If transValue.Length > 200 Then
           stat = Math.Abs(transValue.Length - tr.TextValue.Length)
          Else
           stat = Globals.LevenshteinDistance(tr.TextValue, transValue)
          End If
         End If
         With tr
          .LastModified = Now
          .LastModifiedUserId = UserId
          .TextValue = transValue
         End With
         updateList.Add(tr)
         If Settings.KeepStatistics Then
          StatisticsController.RecordStatistic(UserId, tr.TranslationId, stat)
         End If
        End If
       Else
        Dim tr As New TranslationInfo
        With tr
         .TextId = textId
         .Locale = Locale
         .LastModified = Now
         .LastModifiedUserId = UserId
         .TextValue = transValue
        End With
        addList.Add(tr)
        addStatisticsList.Add(textId, transValue.Length)
       End If
      Catch
       ' ignore errors
      End Try
     End If
    Catch ex As XPathException
     ' ignore XPath errors
    End Try
   Loop
  End Using

  For Each tr As TranslationInfo In updateList
   TranslationsController.UpdateTranslation(tr)
  Next
  For Each tr As TranslationInfo In addList
   tr.TranslationId = TranslationsController.AddTranslation(tr)
  Next

  If Settings.KeepStatistics Then
   For Each tr As TranslationInfo In addList
    StatisticsController.RecordStatistic(UserId, tr.TranslationId, addStatisticsList(tr.TextId))
   Next
  End If

 End Sub
#End Region

#End Region

#Region " ViewState Handling "

 Protected Overrides Sub LoadViewState(ByVal savedState As Object)

  If Not (savedState Is Nothing) Then
   Dim myState As Object() = CType(savedState, Object())
   If Not (myState(0) Is Nothing) Then
    MyBase.LoadViewState(myState(0))
   End If
   If Not (myState(1) Is Nothing) Then
    _tempDirectory = CType(myState(1), String)
   End If
  End If

 End Sub

 Protected Overrides Function SaveViewState() As Object

  Dim allStates(1) As Object
  allStates(0) = MyBase.SaveViewState()
  allStates(1) = _tempDirectory
  Return allStates

 End Function

#End Region

End Class

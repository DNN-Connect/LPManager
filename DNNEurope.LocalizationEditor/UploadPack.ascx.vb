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
Imports DotNetNuke.Common
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Entities.Translations
Imports DNNEurope.Modules.LocalizationEditor.Entities.Statistics
Imports DNNEurope.Modules.LocalizationEditor.Services.Packaging


Partial Public Class Import
 Inherits ModuleBase

#Region " Private Members "
 Private _tempDirectory As String = ""
 Private _manifestFile As String = ""
#End Region

#Region " Properties "
 Public Property TempDirectory() As String
  Get
   Return _tempDirectory
  End Get
  Set(ByVal value As String)
   _tempDirectory = value
  End Set
 End Property

 Public Property ManifestFile() As String
  Get
   Return _manifestFile
  End Get
  Set(ByVal value As String)
   _manifestFile = value
  End Set
 End Property

 Public ReadOnly Property IsDnn5Manifest As Boolean
  Get
   Return Not CBool(ManifestFile.ToLower.EndsWith("manifest.xml"))
  End Get
 End Property
#End Region

#Region " Event Handlers "

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

  ' Force full postback for wizard
  AJAX.RegisterPostBackControl(wzdImport)

  wzdImport.CancelButtonText = "<img src=""" + ApplicationPath + "/images/cancel.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)
  wzdImport.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
  wzdImport.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
  wzdImport.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
  wzdImport.FinishPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
  wzdImport.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/save.gif"" border=""0"" /> " + Localization.GetString("Finish", Me.LocalResourceFile)

  If Not Me.IsPostBack Then

   If DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") Then
    ddLocale.Visible = False
    txtLocale.Visible = True
   Else
    ddLocale.Visible = True
    txtLocale.Visible = False
    ddLocale.DataSource = DataProvider.Instance().GetLocalesForUser(UserId, PortalId, ModuleId)
    ddLocale.DataBind()
   End If

   ddObject.DataSource = DataProvider.Instance().GetObjects(ModuleId)
   ddObject.DataBind()
   Try
    ddObject.Items(0).Selected = True
    ddVersion.DataSource = DataProvider.Instance.GetVersions(Integer.Parse(ddObject.SelectedValue))
    ddVersion.DataBind()
   Catch ex As Exception
   End Try

   trVersion.Visible = False
   trObject.Visible = False

   txtUsername.Text = UserInfo.Username

  End If

 End Sub

 Private Sub ddObject_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddObject.SelectedIndexChanged

  ddVersion.Items.Clear()
  ddVersion.DataSource = DataProvider.Instance.GetVersions(Integer.Parse(ddObject.SelectedValue))
  ddVersion.DataBind()

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
  Me.Response.Redirect(NavigateURL(), False)
 End Sub

 Private Sub wzdImport_FinishButtonClick(ByVal sender As Object, ByVal e As WizardNavigationEventArgs) Handles wzdImport.FinishButtonClick
  ImportPack()
  Try
   If TempDirectory <> "" Then
    Directory.Delete(TempDirectory)
   End If
  Catch
  End Try
  DotNetNuke.Common.Utilities.DataCache.RemoveCache(String.Format("LocList{0}", ModuleId))
  Me.Response.Redirect(NavigateURL(), False)
 End Sub

#End Region

#Region " Private Methods "

 Private Function UnpackUploadedFile(ByRef report As StringBuilder) As Boolean
  If Not Directory.Exists(TempDirectory) Then
   Directory.CreateDirectory(TempDirectory)
  Else
   Throw New Exception("New directory " & TempDirectory & " already exists")
  End If
  Globals.CleanupTempDirs(PortalSettings.HomeDirectoryMapPath & "LocalizationEditor")
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
   Dim files As String() = IO.Directory.GetFiles(TempDirectory, "*.dnn")
   If files.Length > 0 Then
    ManifestFile = files(0)
    ' try and parse out the locale already
    Dim manifest As New XmlDocument
    manifest.Load(ManifestFile)
    If manifest.SelectSingleNode("dotnetnuke/packages/package[position()=1]/components/component/languageFiles/code") IsNot Nothing Then
     Dim l As String = manifest.SelectSingleNode("dotnetnuke/packages/package[position()=1]/components/component/languageFiles/code").InnerText
     Try
      ddLocale.ClearSelection()
      ddLocale.Items.FindByValue(l).Selected = True
     Catch ex As Exception
     End Try
     txtLocale.Text = l
    End If
   ElseIf IO.File.Exists(TempDirectory & "\Manifest.Xml") Then
    trObject.Visible = True
    trVersion.Visible = True
    ManifestFile = TempDirectory & "\Manifest.Xml"
   Else
    report.AppendLine("No valid manifest found!!")
    Return False
   End If
  Catch ex As Exception
   report.AppendLine("Error Occurred: " & ex.Message)
   report.AppendLine(ex.StackTrace)
   Return False
  End Try
  Return True
 End Function

#Region " Analysis "
 Private Function AnalyzePack() As String
  If IsDnn5Manifest Then
   Return AnalyzePackV5()
  Else
   Return AnalyzePackV3()
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
  Dim resFileKey As String = Globals.GetCorrectPath(resFile, Localization.LocalResourceDirectory)
  report.AppendLine("Mapped to " & resFileKey)
  AnalyzeFile(report, ObjectsController.GetObject(Integer.Parse(ddObject.SelectedValue)), ddVersion.SelectedValue, resFile, resFileKey)
 End Sub

 Private Function AnalyzePackV5() As String
  Dim packLocale As String = ""
  Dim report As New StringBuilder
  Dim manifest As New XmlDocument
  manifest.Load(ManifestFile)
  report.AppendLine("Loaded Manifest")
  For Each p As XmlNode In manifest.SelectNodes("dotnetnuke/packages/package")
   Try
    Dim packType As String = ""
    packType = p.SelectSingleNode("components/component/@type").InnerText.ToLower
    Dim basePath As String = ""
    If p.SelectSingleNode("components/component/languageFiles/basePath") IsNot Nothing Then
     basePath = p.SelectSingleNode("components/component/languageFiles/basePath").InnerText
    End If
    If basePath <> "" And Not basePath.EndsWith("\") Then
     basePath &= "\"
    End If
    Dim depObject As ObjectInfo = Nothing
    Dim dependentPackage As String = ""
    If packType = "corelanguage" Then
     dependentPackage = Globals.glbCoreName
    End If
    If p.SelectSingleNode("components/component/languageFiles/package") IsNot Nothing Then
     dependentPackage = p.SelectSingleNode("components/component/languageFiles/package").InnerText
    End If
    Dim depVersion As String = Globals.FormatVersion(p.SelectSingleNode("@version").InnerText)
    depObject = ObjectsController.GetObjectByObjectName(ModuleId, dependentPackage)
    If depObject Is Nothing AndAlso dependentPackage.ToLower.StartsWith("dotnetnuke.") Then
     dependentPackage = Mid(dependentPackage, 12)
     depObject = ObjectsController.GetObjectByObjectName(ModuleId, dependentPackage)
    End If
    If depObject Is Nothing Then
     report.AppendLine(String.Format("Could not find dependent component {0}", dependentPackage))
    Else
     packLocale = p.SelectSingleNode("components/component/languageFiles/code").InnerText
     report.AppendLine(String.Format("Uploading package for {0} {1}", depObject.ObjectName, depVersion))
     For Each xNode As XmlNode In p.SelectNodes("components/component/languageFiles/languageFile")
      Dim filePath As String = xNode.SelectSingleNode("path").InnerText
      If filePath <> "" And Not filePath.EndsWith("\") Then
       filePath &= "\"
      End If
      Dim fileName As String = xNode.SelectSingleNode("name").InnerText
      Dim fileKey As String = basePath & filePath & Regex.Replace(fileName, "(?i)\.\w{2}(-\w+)?\.resx$(?-i)", ".resx")
      AnalyzeFile(report, depObject, depVersion, filePath & fileName, fileKey)
     Next
    End If
   Catch ex As Exception
    report.AppendLine("Error Occurred: " & ex.Message)
    report.AppendLine(ex.StackTrace)
   End Try
  Next
  Return report.ToString
 End Function

 Private Sub AnalyzeFile(ByRef report As StringBuilder, obj As ObjectInfo, version As String, ByVal tempResFile As String, ByVal resFileKey As String)
  report.AppendLine("Analyzing " & tempResFile)
  Dim resFile As New XmlDocument
  resFile.Load(TempDirectory & "\" & tempResFile)
  report.AppendLine("Mapped to " & resFileKey)
  Dim hits As Integer = 0
  Dim Locale As String = txtLocale.Text
  If ddLocale.Visible = True Then
   Locale = ddLocale.SelectedValue
  End If
  Using ir As IDataReader = DataProvider.Instance.GetTextsByObjectAndFile(ModuleId, obj.ObjectId, resFileKey, Locale, version, True)
   Do While ir.Read
    hits += 1
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
  If hits = 0 And obj.IsCore Then ' maybe this core pack includes resources from dependent objects
   Using ir As IDataReader = DataProvider.Instance.GetDependentTextsForObject(ModuleId, obj.ObjectId, resFileKey, Locale, version, True)
    Do While ir.Read
     hits += 1
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
  End If
  If hits = 0 Then
   report.AppendLine("This file was not found in the original data or it was empty")
  End If
 End Sub
#End Region

#Region " Import "
 Private Sub ImportPack()
  If IsDnn5Manifest Then
   ImportPackV5()
  Else
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
  Dim resFileKey As String = Globals.GetCorrectPath(resFile, Localization.LocalResourceDirectory)
  'Fix slashes (from / to \ )
  resFileKey = resFileKey.Replace("/"c, "\"c)
  ImportFile(ObjectsController.GetObject(Integer.Parse(ddObject.SelectedValue)), ddVersion.SelectedValue, resFile, resFileKey)
 End Sub

 Private Sub ImportPackV5()
  Dim manifest As New XmlDocument
  manifest.Load(ManifestFile)
  For Each p As XmlNode In manifest.SelectNodes("dotnetnuke/packages/package")
   Try
    Dim packType As String = ""
    packType = p.SelectSingleNode("components/component/@type").InnerText.ToLower
    Dim basePath As String = ""
    If p.SelectSingleNode("components/component/languageFiles/basePath") IsNot Nothing Then
     basePath = p.SelectSingleNode("components/component/languageFiles/basePath").InnerText
    End If
    If basePath <> "" And Not basePath.EndsWith("\") Then
     basePath &= "\"
    End If
    Dim depObject As ObjectInfo = Nothing
    Dim dependentPackage As String = ""
    If packType = "corelanguage" Then
     dependentPackage = Globals.glbCoreName
    End If
    If p.SelectSingleNode("components/component/languageFiles/package") IsNot Nothing Then
     dependentPackage = p.SelectSingleNode("components/component/languageFiles/package").InnerText
    End If
    depObject = ObjectsController.GetObjectByObjectName(ModuleId, dependentPackage)
    If depObject Is Nothing AndAlso dependentPackage.ToLower.StartsWith("dotnetnuke.") Then
     dependentPackage = Mid(dependentPackage, 12)
     depObject = ObjectsController.GetObjectByObjectName(ModuleId, dependentPackage)
    End If
    Dim depVersion As String = Globals.FormatVersion(p.SelectSingleNode("@version").InnerText)
    For Each xNode As XmlNode In p.SelectNodes("components/component/languageFiles/languageFile")
     Dim filePath As String = xNode.SelectSingleNode("path").InnerText
     If filePath <> "" And Not filePath.EndsWith("\") Then
      filePath &= "\"
     End If
     Dim fileName As String = xNode.SelectSingleNode("name").InnerText
     Dim fileKey As String = basePath & filePath & Regex.Replace(fileName, "(?i)\.\w{2}(-\w+)?\.resx$(?-i)", ".resx")
     ImportFile(depObject, depVersion, filePath & fileName, fileKey)
    Next
   Catch ex As Exception
   End Try
  Next
 End Sub

 Private Sub ImportFile(obj As ObjectInfo, version As String, ByVal tempResFile As String, ByVal resFileKey As String)

  Dim resFile As New XmlDocument
  resFile.Load(TempDirectory & "\" & tempResFile)
  'Fix slashes (from / to \ )
  resFileKey = resFileKey.Replace("/"c, "\"c)

  Dim authorUserId As Integer = UserId
  If txtUsername.Text.Trim <> "" Then
   Dim u As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetUserByName(PortalId, txtUsername.Text.Trim)
   If u IsNot Nothing Then
    authorUserId = u.UserID
   End If
  End If

  Dim updateList As New List(Of TranslationInfo)
  Dim addList As New List(Of TranslationInfo)
  Dim addStatisticsList As New Dictionary(Of Integer, Integer)
  Dim Locale As String = txtLocale.Text
  If ddLocale.Visible = True Then
   Locale = ddLocale.SelectedValue
  End If
  Dim hits As Integer = 0
  Using ir As IDataReader = DataProvider.Instance.GetTextsByObjectAndFile(ModuleId, obj.ObjectId, resFileKey, Locale, version, True)
   Do While ir.Read
    hits += 1
    Dim textKey As String = CStr(ir.Item("TextKey"))
    Dim textId As Integer = CInt(ir.Item("TextId"))
    Dim hasValue As Boolean = False
    If ir.Item("TextValue") IsNot DBNull.Value Then
     hasValue = True
    End If
    Try
     Dim xNode As XmlNode = resFile.SelectSingleNode("root/data[@name='" & textKey & "']")
     If xNode IsNot Nothing Then
      Try
       Dim transValue As String = xNode.SelectSingleNode("value").InnerXml
       If hasValue Then
        Dim tr As TranslationInfo = TranslationsController.GetTranslation(textId, CStr(ir.Item("Locale")))
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
          .LastModifiedUserId = authorUserId
          .TextValue = transValue
         End With
         updateList.Add(tr)
         If Settings.KeepStatistics Then
          StatisticsController.RecordStatistic(tr.TextId, Locale, authorUserId, stat)
         End If
        End If
       Else
        Dim tr As New TranslationInfo
        With tr
         .TextId = textId
         .Locale = Locale
         .LastModified = Now
         .LastModifiedUserId = authorUserId
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
  If hits = 0 And obj.IsCore Then ' maybe this core pack includes resources from dependent objects
   Using ir As IDataReader = DataProvider.Instance.GetDependentTextsForObject(ModuleId, obj.ObjectId, resFileKey, Locale, version, True)
    Do While ir.Read
     Dim textKey As String = CStr(ir.Item("TextKey"))
     Dim textId As Integer = CInt(ir.Item("TextId"))
     Dim hasValue As Boolean = False
     If ir.Item("TextValue") IsNot DBNull.Value Then
      hasValue = True
     End If
     Try
      Dim xNode As XmlNode = resFile.SelectSingleNode("root/data[@name='" & textKey & "']")
      If xNode IsNot Nothing Then
       Try
        Dim transValue As String = xNode.SelectSingleNode("value").InnerXml
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
           .LastModifiedUserId = authorUserId
           .TextValue = transValue
          End With
          updateList.Add(tr)
          If Settings.KeepStatistics Then
           StatisticsController.RecordStatistic(textId, Locale, authorUserId, stat)
          End If
         End If
        Else
         Dim tr As New TranslationInfo
         With tr
          .TextId = textId
          .Locale = Locale
          .LastModified = Now
          .LastModifiedUserId = authorUserId
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
  End If

  For Each tr As TranslationInfo In updateList
   TranslationsController.SetTranslation(tr)
  Next
  For Each tr As TranslationInfo In addList
   TranslationsController.SetTranslation(tr)
  Next

  If Settings.KeepStatistics Then
   For Each tr As TranslationInfo In addList
    StatisticsController.RecordStatistic(tr.TextId, tr.Locale, authorUserId, addStatisticsList(tr.TextId))
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
   If Not (myState(2) Is Nothing) Then
    _manifestFile = CType(myState(2), String)
   End If
  End If

 End Sub

 Protected Overrides Function SaveViewState() As Object

  Dim allStates(2) As Object
  allStates(0) = MyBase.SaveViewState()
  allStates(1) = _tempDirectory
  allStates(2) = _manifestFile
  Return allStates

 End Function

#End Region

End Class

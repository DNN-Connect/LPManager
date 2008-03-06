Imports System.Xml
Imports DotNetNuke
'Imports DotNetNuke.Common.Utilities.HtmlUtils
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization

Partial Public Class Fix
 Inherits DotNetNuke.Framework.PageBase

#Region " Private Members "
 Private _settings As ModuleSettings
 Private _targetLocale As String = ""
 Private _objectToEdit As String = ""
 Private _module As DesktopModuleInfo = Nothing
 Private _filelist As SortedList = Nothing
#End Region

#Region " Page Events "
 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

  DotNetNuke.Services.Upgrade.Upgrade.StartTimer()

  'Get current Script time-out
  Dim scriptTimeOut As Integer = Server.ScriptTimeout

  'Disable Client side caching
  Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)

  'Set Script timeout to MAX value
  Server.ScriptTimeout = Integer.MaxValue

  Dim objStreamReader As IO.StreamReader
  objStreamReader = IO.File.OpenText(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm"))
  Dim strHTML As String = objStreamReader.ReadToEnd
  strHTML = strHTML.Replace("href=""", "href=""" & ResolveUrl("~/Install/"))
  strHTML = strHTML.Replace("src=""", "src=""" & ResolveUrl("~/Install/"))
  objStreamReader.Close()
  Response.Write(strHTML)

  Response.Write("<h1>Fixing Localization Files</h1>")

  WriteFeedback(0, "Retrieving Import Settings")

  Dim TabId As Integer = -1
  Try
   WriteFeedback(1, "TabId: ", False)
   TabId = Integer.Parse(Me.Request.Params("TabId"))
   WriteSuccessError(True)
  Catch ex As Exception
   WriteSuccessError(False)
  End Try

  Dim ModuleId As Integer = -1
  Try
   WriteFeedback(1, "ModuleId: ", False)
   ModuleId = Integer.Parse(Me.Request.Params("ModuleId"))
   WriteSuccessError(True)
  Catch ex As Exception
   WriteSuccessError(False)
  End Try

  _targetLocale = Me.Request.Params("Locale")
  _objectToEdit = Me.Request.Params("Module")
  _settings = New ModuleSettings(ModuleId)

  ' Verify permission
  'If Not Me.IsEditable Then
  ' Throw New Exception("No permission to edit")
  ' Exit Sub
  'End If
  If Not (";" & _settings.Locales).IndexOf(";" & _targetLocale & ";") > -1 Then
   Throw New Exception("No permission to edit locale " & _targetLocale)
   Exit Sub
  End If
  If Not (";" & _settings.Objects).IndexOf(";" & _objectToEdit & ";") > -1 Then
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

   Dim sourceFile As String = file.Key.ToString
   Dim destinationFile As String = Globals.ResourceFile(file.Key.ToString, _targetLocale)

   WriteFeedback(1, "<b>" & destinationFile & "</b>")

   ' check for existance
   If Not IO.File.Exists(destinationFile) Then

    ' Does not exist, so create empty resource file
    Try
     WriteFeedback(2, "Creating Resourcefile: ", False)
     Dim resDoc As New XmlDocument
     resDoc.Load(Server.MapPath("~/DesktopModules/Bring2mind/LocalizationEditor/Template.resx"))
     resDoc.Save(destinationFile)
     resDoc = Nothing
     WriteSuccessError(True)
    Catch ex As Exception
     WriteSuccessError(False)
     WriteFeedback(2, "Error: " & ex.Message & "<br />" & ex.StackTrace)
    End Try

   Else

    Dim dsDef As New DataSet
    Dim dsRes As New DataSet
    Dim dtDef, dtRes As DataTable
    Dim resDoc As New XmlDocument
    Dim defDoc As New XmlDocument
    Dim attr As XmlAttribute
    Dim node, nodeData, parent As XmlNode
    resDoc.Load(destinationFile)
    defDoc.Load(sourceFile)

    Try
     WriteFeedback(2, "Reading source file: ", False)
     dsDef.ReadXml(sourceFile)
     WriteSuccessError(True)
    Catch ex As Exception
     WriteSuccessError(False)
     WriteFeedback(2, "Error: " & ex.Message & "<br />" & ex.StackTrace)
    End Try
    Try
     WriteFeedback(2, "Reading destination file: ", False)
     dsRes.ReadXml(destinationFile)
     WriteSuccessError(True)
    Catch ex As Exception
     WriteSuccessError(False)
     WriteFeedback(2, "Error: " & ex.Message & "<br />" & ex.StackTrace)
    End Try

    If Not dsRes Is Nothing And Not dsDef Is Nothing Then

     dtDef = dsDef.Tables("data")
     dtDef.TableName = "default"
     dtRes = dsRes.Tables("data").Copy
     dtRes.TableName = "localized"
     dsDef.Tables.Add(dtRes)

     ' Check for duplicate entries in localized file
     Try
      Dim c As New UniqueConstraint("uniqueness", dtRes.Columns("name"))
      dtRes.Constraints.Add(c)
      dtRes.Constraints.Remove("uniqueness")
     Catch
      WriteFeedback(2, "Duplicate entries in destination file. Attempting fix.")
      ' destination file contains duplicates
      For Each node In resDoc.SelectNodes("//root/data")
       If resDoc.SelectNodes("//root/data[@name='" + node.Attributes("name").Value + "']").Count > 1 Then
        WriteFeedback(3, "Removing duplicate entries for " & node.Attributes("name").Value & ": ", False)
        Try
         parent = node.ParentNode
         parent.RemoveChild(node)
         WriteSuccessError(True)
        Catch ex As Exception
         WriteSuccessError(False)
         WriteFeedback(3, "Error: " & ex.Message & "<br />" & ex.StackTrace)
        End Try
       End If
      Next
      resDoc.Save(destinationFile)
     End Try

     ' Check for missing entries in localized file
     Try
      dsDef.Relations.Add("missing", dtRes.Columns("name"), dtDef.Columns("name"))
     Catch
      ' some entries in System default file are not found in Resource file
      WriteFeedback(2, "Some entries are not found in Resource file. Attempting fix.")
      For Each node In defDoc.SelectNodes("//root/data")
       If resDoc.SelectSingleNode("//root/data[@name='" + node.Attributes("name").Value + "']") Is Nothing Then
        WriteFeedback(3, "Adding new entry " & node.Attributes("name").Value & ": ", False)
        Try
         nodeData = resDoc.CreateElement("data")
         attr = resDoc.CreateAttribute("name")
         nodeData.Attributes.Append(DotNetNuke.Common.Utilities.XmlUtils.CreateAttribute(resDoc, "name", node.Attributes("name").Value))
         nodeData.AppendChild(resDoc.CreateElement("value"))
         If Not _settings.AddNewEntriesAsBlank Then
          nodeData.SelectSingleNode("value").InnerXml = node.SelectSingleNode("value").InnerXml
         End If
         resDoc.SelectSingleNode("//root").AppendChild(nodeData)
         WriteSuccessError(True)
        Catch ex As Exception
         WriteSuccessError(False)
         WriteFeedback(3, "Error: " & ex.Message & "<br />" & ex.StackTrace)
        End Try
       End If
      Next
      resDoc.Save(destinationFile)
     Finally
      dsDef.Relations.Remove("missing")
     End Try

     ' Check for obsolete entries in localized file
     Try
      dsDef.Relations.Add("obsolete", dtDef.Columns("name"), dtRes.Columns("name"))
     Catch
      ' some entries in Resource File are not found in System default
      WriteFeedback(2, "Some entries in Resource file are not found in System default. Attempting fix.")
      For Each node In resDoc.SelectNodes("//root/data")
       If defDoc.SelectSingleNode("//root/data[@name='" + node.Attributes("name").Value + "']") Is Nothing Then
        WriteFeedback(3, "Removing obsolete entry " & node.Attributes("name").Value & ": ", False)
        Try
         parent = node.ParentNode
         parent.RemoveChild(node)
         WriteSuccessError(True)
        Catch ex As Exception
         WriteSuccessError(False)
         WriteFeedback(3, "Error: " & ex.Message & "<br />" & ex.StackTrace)
        End Try
       End If
      Next
      resDoc.Save(destinationFile)
     Finally
      dsDef.Relations.Remove("obsolete")
     End Try

     If _settings.RemoveBlanksFromFile Then
      WriteFeedback(2, "Removing any blank entries: ", False)
      For Each node In resDoc.SelectNodes("//root/data")
       If node.SelectSingleNode("value").InnerXml = "" Then
        WriteFeedback(3, "Removing blank entry " & node.Attributes("name").Value & ": ", False)
        Try
         parent = node.ParentNode
         parent.RemoveChild(node)
         WriteSuccessError(True)
        Catch ex As Exception
         WriteSuccessError(False)
         WriteFeedback(3, "Error: " & ex.Message & "<br />" & ex.StackTrace)
        End Try
       End If
      Next
     End If

    End If
   End If

   WriteFeedback(1, "Finished with " & destinationFile & ".")
   WriteFeedback(1, "")

  Next

  WriteFeedback(0, "Finished Fixing Resource Files")

  Dim returnurl As String = ""
  Try
   returnurl = Me.Request.UrlReferrer.PathAndQuery
  Catch ex As Exception
   returnurl = ResolveUrl("~/Default.aspx?tabid=" & TabId)
  End Try
  Response.Write("<br><br><h2><a href='" & returnurl & "'>Click Here To Return</a></h2><br><br>")

  'Restore Script timeout
  Server.ScriptTimeout = scriptTimeOut

 End Sub
#End Region

#Region " Private Methods "
 Private Sub WriteFeedback(ByVal indent As Int32, ByVal message As String)

  WriteFeedback(indent, message, True, True)

 End Sub

 Private Sub WriteFeedback(ByVal indent As Int32, ByVal message As String, ByVal newline As Boolean)

  WriteFeedback(indent, message, newline, True)

 End Sub

 Private Sub WriteFeedback(ByVal indent As Int32, ByVal message As String, ByVal newline As Boolean, ByVal showtime As Boolean)

  Dim timeElapsed As TimeSpan = DotNetNuke.Services.Upgrade.Upgrade.RunTime

  Dim strMessage As String = ""
  If showtime = True Then
   strMessage += timeElapsed.ToString.Substring(0, timeElapsed.ToString.LastIndexOf(".") + 4) & " -"
  End If

  For i As Integer = 0 To indent
   strMessage += "&nbsp;"
  Next
  strMessage += message
  If newline Then
   strMessage += "<br />"
  End If
  Response.Write(strMessage)
  Response.Flush()

 End Sub

 Private Sub WriteSuccessError(ByVal bSuccess As Boolean)
  If bSuccess Then
   WriteFeedback(0, "<font color='green'>Success</font><br>", False)
  Else
   WriteFeedback(0, "<font color='red'>Error!</font><br>", False)
  End If
 End Sub
#End Region

End Class
Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports System.Text.RegularExpressions
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.Localization.Localization

Partial Public Class Editor
 Inherits ModuleBase

#Region " Private Members "
 Public ResourceFile As String = ""
 Public ResourceKey As String = ""
 Public OriginalValue As String = ""
 Public IsHtml As Boolean = False
#End Region

#Region "Private Methods"

 'Local tag cache to avoid multiple generations of the same tag
 Private tagCache As Hashtable

 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Replaces all smart Tags in content with their appropriate values.
 ''' </summary>
 ''' <param name="content"></param>
 ''' <returns>Content with replaced Tags</returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' </history>
 ''' -----------------------------------------------------------------------------

 Private Function ProcessTags(ByVal content As String) As String
  Dim sb As New System.Text.StringBuilder
  Dim retVal As String = Null.NullString

  Dim position As Integer = 0

  'Find all tags used in text
  'The regular expression matches the tag including the square brackets:
  'in "aaa[bbb[ccc]ddd]eee" it matches only "[ccc]"
  If Not content Is Nothing Then
   For Each _match As Match In Regex.Matches(content, "\[([^\[]*?)\]")
    'Append the text before the match to the result
    sb.Append(content.Substring(position, _match.Index - position))

    'Process the tag and append the output to the result
    sb.Append(ProcessTag(_match.Value))

    'Set the starting point for the next match
    position = _match.Index + _match.Value.Length
   Next

   'Append the rest of the text to the result
   sb.Append(content.Substring(position))

   retVal = sb.ToString
  End If

  Return retVal
 End Function

 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Processes all occurrences of the given tag by the given value in given content
 ''' </summary>
 ''' <param name="tag"></param>
 ''' <returns>Content with replaced Tag</returns>
 ''' <remarks>
 ''' A hashtable is used to make sure each tag will only be processed once each load.
 ''' </remarks>
 ''' <history>
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Private Function ProcessTag(ByVal tag As String) As String

  'Store the tag in the result to keep the text if its not a smarttag
  Dim retval As String = tag
  Dim tagitems As String() = tag.Substring(1, tag.Length - 2).Trim().Split(" ".ToCharArray())
  'Ensure case independency
  tagitems(0) = tagitems(0).ToUpper()
  'Initialize cache if not created
  If tagCache Is Nothing Then tagCache = New Hashtable
  'If we have tag in cache, simply return the cached content
  If tagCache.ContainsKey(tag) Then
   retval = tagCache(tag).ToString()
  Else
   'Build tag content value if its a known tag
   Select Case tagitems(0)
    Case "PORTAL.NAME"
     retval = PortalSettings.PortalName
     tagCache(tag) = retval
    Case "DATE"
     If tagitems.Length = 2 Then
      Try
       retval = DateTime.Now.ToString(tagitems(1))
      Catch
       retval = DateTime.Now.ToShortDateString()
      End Try
     Else
      retval = DateTime.Now.ToShortDateString()
     End If
     tagCache(tag) = retval
    Case "TIME"
     If tagitems.Length = 2 Then
      Try
       retval = DateTime.Now.ToString(tagitems(1))
      Catch
       retval = DateTime.Now.ToShortTimeString()
      End Try
     Else
      retval = DateTime.Now.ToShortTimeString()
     End If
     tagCache(tag) = retval
   End Select
  End If

  Return retval
 End Function

#End Region

#Region "Event Handlers"
 Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
  MyBase.OnInit(e)
  'If DotNetNuke.Entities.Host.HostSettings.GetHostSetting("UseFriendlyUrls") <> "Y" Then
  '    'allow for relative urls
  '    lblContent.UrlFormat = UI.WebControls.UrlFormatType.Relative
  'End If
  'Me.LocalResourceFile = "Admin/"
 End Sub

 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Page_Load runs when the control is loaded
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try

   lblContent.RichTextEnabled = IsHtml

   'localize toolbar
   For Each objButton As DotNetNuke.UI.WebControls.DNNToolBarButton In Me.tbEIPHTML.Buttons
    objButton.ToolTip = Services.Localization.Localization.GetString("cmd" & objButton.ToolTip, LocalResourceFile)
   Next

   ' handle Smart Tags that might have been used
   OriginalValue = ProcessTags(OriginalValue)

   'add content to module
   lblContent.Controls.Add(New LiteralControl(OriginalValue))

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' lblContent_UpdateLabel allows for inline editing of content
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Private Sub lblContent_UpdateLabel(ByVal source As Object, ByVal e As UI.WebControls.DNNLabelEditEventArgs) Handles lblContent.UpdateLabel

  Dim returnText As String = e.Text
  If returnText <> OriginalValue Then

   Dim resDoc As New XmlDocument, node As XmlNode
   If Not IO.File.Exists(ResourceFile) Then
    resDoc.Load(Server.MapPath("~/DesktopModules/Bring2mind/LocalizationEditor/Template.resx"))
   Else
    resDoc.Load(ResourceFile)
   End If

   ' Add if not present
   node = resDoc.SelectSingleNode("//root/data[@name='" + ResourceKey + "']")
   If node Is Nothing Then
    node = resDoc.CreateElement("data")
    node.Attributes.Append(XmlUtils.CreateAttribute(resDoc, "name", ResourceKey))
    node.AppendChild(resDoc.CreateElement("value"))
    resDoc.SelectSingleNode("//root").AppendChild(node)
   End If
   node = resDoc.SelectSingleNode("//root/data[@name='" + ResourceKey + "']/value")
   node.InnerXml = Server.HtmlEncode(returnText)

   resDoc.Save(ResourceFile)

  End If

  SynchronizeModule()

 End Sub
#End Region

End Class
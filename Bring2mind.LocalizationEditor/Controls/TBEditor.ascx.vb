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

Partial Public Class TBEditor
 Inherits ModuleBase

#Region " Private Members "
 Public ResourceFile As String = ""
 Public ResourceKey As String = ""
 Public Value As String = ""
 Public OriginalLength As Integer = 0
#End Region

#Region "Private Methods"
#End Region

#Region "Event Handlers"
 Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
  MyBase.OnInit(e)
 End Sub

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try

   If Not Me.IsPostBack Then
    Dim height As Integer = 40
    If Value.Length > 300 Or OriginalLength > 300 Then
     height = 200
    ElseIf Value.Length > 100 Or OriginalLength > 100 Then
     height = 100
    ElseIf Value.Length < 50 Or OriginalLength < 50 Then
     height = 0
    End If
    If height > 0 Then
     txtValue.Text = Value
     txtValue.Height = Unit.Pixel(100)
    Else
     txtValue.TextMode = TextBoxMode.MultiLine
    End If
   End If

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Sub UpdateValue()

  Dim returnText As String = txtValue.Text
  If returnText <> Value Then

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

#Region " ViewState Handling "
 Protected Overrides Sub LoadViewState(ByVal savedState As Object)

  If Not (savedState Is Nothing) Then
   Dim myState As Object() = CType(savedState, Object())
   If Not (myState(0) Is Nothing) Then
    MyBase.LoadViewState(myState(0))
   End If
   If Not (myState(1) Is Nothing) Then
    ResourceFile = CStr(myState(1))
   End If
   If Not (myState(2) Is Nothing) Then
    ResourceKey = CStr(myState(2))
   End If
   If Not (myState(3) Is Nothing) Then
    Value = CStr(myState(3))
   End If
  End If

 End Sub

 Protected Overrides Function SaveViewState() As Object

  Dim allStates(3) As Object
  allStates(0) = MyBase.SaveViewState()
  allStates(1) = ResourceFile
  allStates(2) = ResourceKey
  allStates(3) = Value
  Return allStates

 End Function
#End Region

End Class
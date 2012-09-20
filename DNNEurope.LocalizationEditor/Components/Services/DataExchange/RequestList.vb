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
Imports System.Xml
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects

Namespace Services.DataExchange
 Public Class RequestList

  Private _locale As String = ""
  Private _portalId As Integer = -1
  Private _moduleKey As String = ""
  Private _objects As New Dictionary(Of String, String)

#Region " Constructors "
  Public Sub New(ByVal portalId As Integer, ByVal moduleKey As String, ByVal locale As String, ByVal objectList As String)

   _portalId = portalId
   _locale = locale
   _moduleKey = moduleKey
   Dim ol As New XmlDocument
   ol.LoadXml(objectList)
   For Each xn As XmlNode In ol.DocumentElement.SelectNodes("Objects")
    Dim oName As String = xn.SelectSingleNode("Name").InnerText
    Dim oVersion As String = xn.SelectSingleNode("Version").InnerText
    Dim m As Match = Regex.Match(oVersion, "^(\d+)\.(\d+)\.(\d+)")
    If m.Success Then
     oVersion = String.Format("{0:00}.{1:00}.{2:00}", CInt(m.Groups(1).Value), CInt(m.Groups(2).Value), CInt(m.Groups(3).Value))
    End If
    _objects.Add(oName, oVersion)
   Next

  End Sub
#End Region

#Region " Public Methods "
  Public Sub WriteData(ByVal output As XmlWriter)

   output.WriteStartElement("LanguagePacks")
   output.WriteAttributeString("locale", _locale)
   For Each oName As String In _objects.Keys
    Dim o As ObjectInfo = ObjectsController.GetObjectByObjectNameAndModuleKey(_portalId, oName, _moduleKey)
    If o IsNot Nothing Then
     output.WriteStartElement("Object")
     output.WriteAttributeString("name", oName)
     output.WriteAttributeString("version", _objects(oName))
     output.WriteAttributeString("friendlyName", o.FriendlyName)
     Using ir As IDataReader = Data.DataProvider.Instance().GetLanguagePacksByObjectVersionLocale(o.ObjectId, _objects(oName), _locale)
      Do While ir.Read
       If CDbl(ir.Item("PercentComplete")) > 0 Then
        Dim partnerId As Integer = CInt(ir.Item("PartnerId"))
        Dim name As String = CStr(ir.Item("PartnerName"))
        Dim url As String = Globals.GetAString(ir.Item("PartnerUrl"))
        Dim packUrl As String = CStr(ir.Item("PackUrl"))
        Dim remoteObjectId As Integer = CInt(ir.Item("RemoteObjectId"))
        Dim percComplete As Double = CDbl(ir.Item("PercentComplete"))
        If partnerId = -1 Then
         name = Settings(o.ModuleId).OwnerOrganization
         If name = "" Then name = Settings(o.ModuleId).OwnerName
         url = Settings(o.ModuleId).OwnerUrl
         packUrl = Globals.PackUrl
         remoteObjectId = o.ObjectId
        End If
        output.WriteStartElement("Pack")
        output.WriteAttributeString("orgName", name)
        output.WriteAttributeString("orgUrl", url)
        output.WriteAttributeString("perc", percComplete.ToString("0"))
        output.WriteAttributeString("url", String.Format("{0}?ObjectId={1}&Locale={2}&Version={3}", packUrl, remoteObjectId, _locale, _objects(oName)))
        If Not ir.Item("LastModified") Is DBNull.Value Then
         output.WriteAttributeString("lastmodified", CDate(ir.Item("LastModified")).ToString("yyyy-MM-dd"))
        End If
        output.WriteEndElement() ' Object
       End If
      Loop
     End Using
     output.WriteEndElement() ' Object
    End If
   Next
   output.WriteEndElement() ' LanguagePacks

  End Sub
#End Region

#Region " Private Methods "
  Private _settings As ModuleSettings
  Private Function Settings(ByVal moduleId As Integer) As ModuleSettings
   If _settings Is Nothing Then
    _settings = ModuleSettings.GetSettings("", moduleId)
   End If
   Return _settings
  End Function
#End Region

 End Class
End Namespace

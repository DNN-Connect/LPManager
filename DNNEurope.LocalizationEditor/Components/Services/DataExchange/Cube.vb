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
Imports DNNEurope.Modules.LocalizationEditor.Entities.Statistics
Imports DNNEurope.Modules.LocalizationEditor.Entities.Partners

Namespace Services.DataExchange
 Public Class Cube

  Private _moduleId As Integer = -1
  Private _portalId As Integer = -1

#Region " Constructors "
  Public Sub New(ByVal PortalId As Integer, ByVal ModuleId As Integer)
   _portalId = PortalId
   _moduleId = ModuleId
  End Sub
#End Region

#Region " Public Methods "
  Public Sub WriteCube(ByVal output As XmlWriter)

   Dim pi As DotNetNuke.Entities.Portals.PortalInfo = (New DotNetNuke.Entities.Portals.PortalController).GetPortal(_portalId)
   Dim portalHomeDirMapPath As String = IO.Path.Combine(DotNetNuke.Common.ApplicationMapPath, pi.HomeDirectory).Replace("/", "\") & "\"
   Dim ms As New ModuleSettings(portalHomeDirMapPath, _moduleId)
   If Not ms.AllowDataExtract Then Exit Sub
   output.WriteStartElement("cube")
   output.WriteAttributeString("ownerName", ms.OwnerName)
   output.WriteAttributeString("ownerEmail", ms.OwnerEmail)
   output.WriteAttributeString("ownerOrganization", ms.OwnerOrganization)
   output.WriteAttributeString("ownerUrl", ms.OwnerUrl)
   output.WriteAttributeString("packUrl", Globals.PackUrl)
   output.WriteAttributeString("allowDirectDownload", ms.AllowDirectDownload.ToString)
   output.WriteElementString("license", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(ms.License))
   Using ir As IDataReader = Data.DataProvider.Instance().GetCube(_moduleId)
    Dim lastObjectId As Integer = -1
    Dim lastVersion As String = ""
    Do While ir.Read
     Dim objectId As Integer = CInt(ir.Item("ObjectId"))
     Dim version As String = CStr(ir.Item("Version"))
     If objectId <> lastObjectId Then
      ' start new object
      If lastObjectId > -1 Then
       ' close last object
       output.WriteEndElement() ' version
       output.WriteEndElement() ' object
      End If
      lastObjectId = objectId
      lastVersion = ""
      output.WriteStartElement("object")
      output.WriteAttributeString("id", objectId.ToString)
      output.WriteAttributeString("name", CStr(ir.Item("ObjectName")))
      output.WriteAttributeString("friendlyName", CStr(ir.Item("FriendlyName")))
      output.WriteAttributeString("type", CStr(ir.Item("PackageType")))
     End If
     If version <> lastVersion Then
      ' start new version
      If lastVersion <> "" Then
       ' close last version
       output.WriteEndElement() ' version
      End If
      ' start version
      output.WriteStartElement("version")
      output.WriteAttributeString("name", version)
      output.WriteAttributeString("newTexts", CStr(ir.Item("NewTexts")))
      output.WriteAttributeString("totalTexts", CStr(ir.Item("TotalTexts")))
      lastVersion = version
     End If
     Dim nrTranslations As Integer = CInt(ir.Item("TotalTranslated"))
     If nrTranslations > 0 Then
      output.WriteStartElement("locale")
      output.WriteAttributeString("name", CStr(ir.Item("Locale")))
      output.WriteAttributeString("totalTexts", nrTranslations.ToString)
      output.WriteAttributeString("lastModified", CDate(ir.Item("LastModified")).ToUniversalTime().ToString("u"))
      ' Include statistics if applicable
      If ms.KeepStatistics Then
       output.WriteStartElement("statistics")
       For Each s As Statistic In StatisticsController.GetPackStatistics(objectId, version, CStr(ir.Item("Locale")))
        output.WriteStartElement("user")
        output.WriteAttributeString("name", s.UserName)
        output.WriteAttributeString("total", s.TotalScore.ToString)
        output.WriteEndElement() ' user
       Next
       output.WriteEndElement() ' statistics
      End If
      output.WriteEndElement() ' locale
     End If
    Loop
    If lastObjectId > -1 Then
     ' close last object
     output.WriteEndElement() ' version
     output.WriteEndElement() ' object
    End If
   End Using
   output.WriteStartElement("partners")
   For Each p As PartnerInfo In PartnersController.GetPartnersByModule(_moduleId)
    If p.AllowRedistribute AndAlso Not String.IsNullOrEmpty(p.LastCube) Then
     output.WriteRaw(p.LastCube)
    End If
   Next
   output.WriteEndElement() ' partners
   output.WriteEndElement() ' cube

  End Sub
#End Region

 End Class
End Namespace

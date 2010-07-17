Imports System.Xml

Namespace Business
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
   output.WriteAttributeString("packUrl", DotNetNuke.Common.AddHTTP(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.PortalAlias.HTTPAlias & "/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx"))
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
   output.WriteEndElement() ' cube

  End Sub
#End Region

 End Class
End Namespace

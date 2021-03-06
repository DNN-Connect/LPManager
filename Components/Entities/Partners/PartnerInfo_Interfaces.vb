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
Imports System
Imports System.Data
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace Entities.Partners
  <Serializable(), XmlRoot("Partner"), DataContract()>
  Partial Public Class PartnerInfo
   Implements IHydratable
   Implements IPropertyAccess
   Implements IXmlSerializable

#Region " IHydratable Implementation "
 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Fill hydrates the object from a Datareader
 ''' </summary>
 ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
 ''' rather than using the more expensive Refection  methods.</remarks>
 ''' <history>
 ''' 	[pdonker]	08/14/2011  Created
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

  AllowDirectDownload = Convert.ToBoolean(Null.SetNull(dr.Item("AllowDirectDownload"), AllowDirectDownload))
  ModuleId = Convert.ToInt32(Null.SetNull(dr.Item("ModuleId"), ModuleId))
  AllowRedistribute = Convert.ToBoolean(Null.SetNull(dr.Item("AllowRedistribute"), AllowRedistribute))
  CubeUrl = Convert.ToString(Null.SetNull(dr.Item("CubeUrl"), CubeUrl))
  DownloadAffiliates = Convert.ToBoolean(Null.SetNull(dr.Item("DownloadAffiliates"), DownloadAffiliates))
  LastCube = Convert.ToString(Null.SetNull(dr.Item("LastCube"), LastCube))
  PackUrl = Convert.ToString(Null.SetNull(dr.Item("PackUrl"), PackUrl))
  PartnerId = Convert.ToInt32(Null.SetNull(dr.Item("PartnerId"), PartnerId))
  PartnerName = Convert.ToString(Null.SetNull(dr.Item("PartnerName"), PartnerName))
  PartnerUrl = Convert.ToString(Null.SetNull(dr.Item("PartnerUrl"), PartnerUrl))
  PartnerUsername = Convert.ToString(Null.SetNull(dr.Item("PartnerUsername"), PartnerUsername))

 End Sub
 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Gets and sets the Key ID
 ''' </summary>
 ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
 ''' as the key property when creating a Dictionary</remarks>
 ''' <history>
 ''' 	[pdonker]	08/14/2011  Created
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Public Property KeyID() As Integer Implements IHydratable.KeyID
  Get
   Return PartnerId
  End Get
  Set(ByVal value As Integer)
   PartnerId = value
  End Set
 End Property
#End Region

#Region " IPropertyAccess Implementation "
 Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
  Dim OutputFormat As String = String.Empty
  Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
  If strFormat = String.Empty Then
   OutputFormat = "D"
  Else
   OutputFormat = strFormat
  End If
  Select Case strPropertyName.ToLower
   Case "allowdirectdownload"
    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowDirectDownload, formatProvider)
   Case "moduleid"
    Return (Me.ModuleId.ToString(OutputFormat, formatProvider))
   Case "allowredistribute"
    Return PropertyAccess.Boolean2LocalizedYesNo(Me.AllowRedistribute, formatProvider)
   Case "cubeurl"
    Return PropertyAccess.FormatString(Me.CubeUrl, strFormat)
   Case "downloadaffiliates"
    Return PropertyAccess.Boolean2LocalizedYesNo(Me.DownloadAffiliates, formatProvider)
   Case "lastcube"
    Return PropertyAccess.FormatString(Me.LastCube, strFormat)
   Case "packurl"
    Return PropertyAccess.FormatString(Me.PackUrl, strFormat)
   Case "partnerid"
    Return (Me.PartnerId.ToString(OutputFormat, formatProvider))
   Case "partnername"
    Return PropertyAccess.FormatString(Me.PartnerName, strFormat)
   Case "partnerurl"
    Return PropertyAccess.FormatString(Me.PartnerUrl, strFormat)
   Case "partnerusername"
    Return PropertyAccess.FormatString(Me.PartnerUsername, strFormat)
   Case Else
    PropertyNotFound = True
  End Select

  Return Null.NullString
 End Function

 Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
  Get
   Return CacheLevel.fullyCacheable
  End Get
 End Property
#End Region

#Region " IXmlSerializable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	08/14/2011  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(ByVal reader As XmlReader, ByVal ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	08/14/2011  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

   Boolean.TryParse(readElement(reader, "AllowDirectDownload"), AllowDirectDownload)
    If Not Int32.TryParse(readElement(reader, "ModuleId"), ModuleId) Then
     ModuleId = Null.NullInteger
    End If
   Boolean.TryParse(readElement(reader, "AllowRedistribute"), AllowRedistribute)
    CubeUrl = readElement(reader, "CubeUrl")
   Boolean.TryParse(readElement(reader, "DownloadAffiliates"), DownloadAffiliates)
    LastCube = readElement(reader, "LastCube")
    PackUrl = readElement(reader, "PackUrl")
    PartnerName = readElement(reader, "PartnerName")
    PartnerUrl = readElement(reader, "PartnerUrl")
    PartnerUsername = readElement(reader, "PartnerUsername")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Partner", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	08/14/2011  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Partner")
   writer.WriteElementString("PartnerId",  PartnerId.ToString())
   writer.WriteElementString("AllowDirectDownload",  AllowDirectDownload.ToString())
   writer.WriteElementString("ModuleId",  ModuleId.ToString())
   writer.WriteElementString("AllowRedistribute",  AllowRedistribute.ToString())
   writer.WriteElementString("CubeUrl",  CubeUrl)
   writer.WriteElementString("DownloadAffiliates",  DownloadAffiliates.ToString())
   writer.WriteElementString("LastCube",  LastCube)
   writer.WriteElementString("PackUrl",  PackUrl)
   writer.WriteElementString("PartnerName",  PartnerName)
   writer.WriteElementString("PartnerUrl",  PartnerUrl)
   writer.WriteElementString("PartnerUsername",  PartnerUsername.ToString())
   writer.WriteEndElement()
  End Sub
#End Region

#Region " ToXml Methods "
  Public Function ToXml() As String
   Return ToXml("Partner")
  End Function

  Public Function ToXml(ByVal elementName As String) As String
   Dim xml As New StringBuilder
   xml.Append("<")
   xml.Append(elementName)
   AddAttribute(xml, "PartnerId", PartnerId.ToString())
   AddAttribute(xml, "AllowDirectDownload", AllowDirectDownload.ToString())
   AddAttribute(xml, "ModuleId", ModuleId.ToString())
   AddAttribute(xml, "AllowRedistribute", AllowRedistribute.ToString())
   AddAttribute(xml, "CubeUrl", CubeUrl)
   AddAttribute(xml, "DownloadAffiliates", DownloadAffiliates.ToString())
   AddAttribute(xml, "LastCube", LastCube)
   AddAttribute(xml, "PackUrl", PackUrl)
   AddAttribute(xml, "PartnerName", PartnerName)
   AddAttribute(xml, "PartnerUrl", PartnerUrl)
   AddAttribute(xml, "PartnerUsername", PartnerUsername.ToString())
   xml.Append(" />")
   Return xml.ToString
  End Function

  Private Sub AddAttribute(ByRef xml As StringBuilder, ByVal attributeName As String, ByVal attributeValue As String)
   xml.Append(" " & attributeName)
   xml.Append("=""" & attributeValue & """")
  End Sub
#End Region

#Region " JSON Serialization "
  Public Sub WriteJSON(ByRef s As Stream)
   Dim ser As New DataContractJsonSerializer(GetType(PartnerInfo))
   ser.WriteObject(s, Me)
  End Sub
#End Region

 End Class
End Namespace



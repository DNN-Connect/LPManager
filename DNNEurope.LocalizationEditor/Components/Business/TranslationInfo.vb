' 
' Copyright (c) 2004-2009 DNN-Europe, http://www.dnn-europe.net
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
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports System.Xml.Schema
Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Namespace Business

#Region " TranslationInfo "

 <Serializable(), XmlRoot("Translation")> Public Class TranslationInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

  ' local property declarations
  Private _TranslationId As Integer
  Private _TextId As Integer
  Private _Locale As String
  Private _LastModified As Date
  Private _LastModifiedUserId As Integer
  Private _TextValue As String

#Region " Constructors "

  Public Sub New()
  End Sub

  Public Sub New(ByVal TextId As Integer, ByVal Locale As String, ByVal LastModified As Date, ByVal LastModifiedUserId As Integer, ByVal TextValue As String)
   Me.LastModified = LastModified
   Me.LastModifiedUserId = LastModifiedUserId
   Me.Locale = Locale
   Me.TextId = TextId
   Me.TextValue = TextValue
  End Sub

#End Region

#Region " Public Properties "

  Public Property TranslationId() As Integer
   Get
    Return _TranslationId
   End Get
   Set(ByVal Value As Integer)
    _TranslationId = Value
   End Set
  End Property

  Public Property TextId() As Integer
   Get
    Return _TextId
   End Get
   Set(ByVal Value As Integer)
    _TextId = Value
   End Set
  End Property

  Public Property Locale() As String
   Get
    Return _Locale
   End Get
   Set(ByVal Value As String)
    _Locale = Value
   End Set
  End Property

  Public Property LastModified() As Date
   Get
    Return _LastModified
   End Get
   Set(ByVal Value As Date)
    _LastModified = Value
   End Set
  End Property

  Public Property LastModifiedUserId() As Integer
   Get
    Return _LastModifiedUserId
   End Get
   Set(ByVal Value As Integer)
    _LastModifiedUserId = Value
   End Set
  End Property

  Public Property TextValue() As String
   Get
    Return _TextValue
   End Get
   Set(ByVal Value As String)
    _TextValue = Value
   End Set
  End Property

#End Region

#Region " IHydratable Implementation "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   TranslationId = Convert.ToInt32(Null.SetNull(dr.Item("TranslationId"), TranslationId))
   LastModified = Convert.ToDateTime(Null.SetNull(dr.Item("LastModified"), LastModified))
   LastModifiedUserId = Convert.ToInt32(Null.SetNull(dr.Item("LastModifiedUserId"), LastModifiedUserId))
   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   TextId = Convert.ToInt32(Null.SetNull(dr.Item("TextId"), TextId))
   TextValue = Convert.ToString(Null.SetNull(dr.Item("TextValue"), TextValue))

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return TranslationId
   End Get
   Set(ByVal value As Integer)
    TranslationId = value
   End Set
  End Property

#End Region

#Region " IPropertyAccess Implementation "

  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As CultureInfo, ByVal AccessingUser As UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "translationid"
     Return (Me.TranslationId.ToString(OutputFormat, formatProvider))
    Case "lastmodified"
     Return (Me.LastModified.ToString(OutputFormat, formatProvider))
    Case "lastmodifieduserid"
     Return (Me.LastModifiedUserId.ToString(OutputFormat, formatProvider))
    Case "locale"
     Return PropertyAccess.FormatString(Me.Locale, strFormat)
    Case "textid"
     Return (Me.TextId.ToString(OutputFormat, formatProvider))
    Case "textvalue"
     Return PropertyAccess.FormatString(Me.TextValue, strFormat)
    Case Else
     PropertyNotFound = True
   End Select

   Return Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As CacheLevel Implements IPropertyAccess.Cacheability
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
  ''' 	[]	06/16/2008  Created
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
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    If Not DateTime.TryParse(readElement(reader, "LastModified"), LastModified) Then
     LastModified = DateTime.MinValue
    End If
    If Not Int32.TryParse(readElement(reader, "LastModifiedUserId"), LastModifiedUserId) Then
     LastModifiedUserId = Null.NullInteger
    End If
    TextValue = readElement(reader, "TextValue")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Translation", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Translation")
   writer.WriteElementString("TranslationId", TranslationId.ToString())
   writer.WriteElementString("TextId", TextId.ToString())
   writer.WriteElementString("Locale", Locale)
   writer.WriteElementString("LastModified", LastModified.ToString())
   writer.WriteElementString("LastModifiedUserId", LastModifiedUserId.ToString())
   writer.WriteElementString("TextValue", TextValue)
   writer.WriteEndElement()
  End Sub

#End Region
 End Class

#End Region

End Namespace

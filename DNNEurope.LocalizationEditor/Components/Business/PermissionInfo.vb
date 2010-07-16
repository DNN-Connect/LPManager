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

 <Serializable(), XmlRoot("Permission")> _
 Public Class PermissionInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

  ' local property declarations
  Private _PermissionId As Integer
  Private _ModuleId As Integer
  Private _UserId As Integer
  Private _Locale As String

  Private _displayName As String
  Private _userName As String
  Private _objectName As String

#Region " Constructors "

  Public Sub New()
  End Sub

  Public Sub New(ByVal PermissionId As Int32, ByVal Locale As String, ByVal ModuleId As Int32, ByVal UserId As Int32)
   Me.Locale = Locale
   Me.ModuleId = ModuleId
   Me.PermissionId = PermissionId
   Me.UserId = UserId
  End Sub

#End Region

#Region " Public Properties "

  Public Property PermissionId() As Integer
   Get
    Return _PermissionId
   End Get
   Set(ByVal Value As Integer)
    _PermissionId = Value
   End Set
  End Property

  Public Property ModuleId() As Integer
   Get
    Return _ModuleId
   End Get
   Set(ByVal Value As Integer)
    _ModuleId = Value
   End Set
  End Property

  Public Property UserId() As Integer
   Get
    Return _UserId
   End Get
   Set(ByVal Value As Integer)
    _UserId = Value
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

  Public Property ObjectName() As String
   Get
    Return _objectName
   End Get
   Set(ByVal value As String)
    _objectName = value
   End Set
  End Property

  Public Property UserName() As String
   Get
    Return _userName
   End Get
   Set(ByVal value As String)
    _userName = value
   End Set
  End Property

  Public Property DisplayName() As String
   Get
    Return _displayName
   End Get
   Set(ByVal value As String)
    _displayName = value
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
  ''' 	[]	06/05/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   PermissionId = Convert.ToInt32(Null.SetNull(dr.Item("PermissionId"), PermissionId))
   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   ModuleId = Convert.ToInt32(Null.SetNull(dr.Item("ModuleId"), ModuleId))
   UserId = Convert.ToInt32(Null.SetNull(dr.Item("UserId"), UserId))
   Try
    DisplayName = Convert.ToString(Null.SetNull(dr.Item("DisplayName"), DisplayName))
    UserName = Convert.ToString(Null.SetNull(dr.Item("UserName"), UserName))
    ObjectName = Convert.ToString(Null.SetNull(dr.Item("ObjectName"), DisplayName))
   Catch ex As Exception
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	06/05/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return PermissionId
   End Get
   Set(ByVal value As Integer)
    PermissionId = value
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
    Case "permissionid"
     Return (Me.PermissionId.ToString(OutputFormat, formatProvider))
    Case "locale"
     Return PropertyAccess.FormatString(Me.Locale, strFormat)
    Case "moduleid"
     Return (Me.ModuleId.ToString(OutputFormat, formatProvider))
    Case "userid"
     Return (Me.UserId.ToString(OutputFormat, formatProvider))
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
  ''' 	[]	06/05/2008  Created
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
  ''' 	[]	06/05/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    If Not Int32.TryParse(readElement(reader, "ModuleId"), ModuleId) Then
     ModuleId = Null.NullInteger
    End If
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Permission", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[]	06/05/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Permission")
   writer.WriteElementString("PermissionId", PermissionId.ToString())
   writer.WriteElementString("UserId", UserId.ToString())
   writer.WriteElementString("Locale", Locale)
   writer.WriteElementString("ModuleId", ModuleId.ToString())
   writer.WriteEndElement()
  End Sub
#End Region

 End Class


End Namespace

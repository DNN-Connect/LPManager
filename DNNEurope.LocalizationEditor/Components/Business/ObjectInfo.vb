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

 <Serializable(), XmlRoot("Object")> Public Class ObjectInfo
  Implements IHydratable
  Implements IPropertyAccess
  Implements IXmlSerializable

  ' local property declarations
  Private _ObjectId As Integer
  Private _ObjectName As String
  Private _FriendlyName As String
  Private _installPath As String
  Private _moduleId As Integer
  Private _packageType As String = "Module"
  Private _module As ModuleInfo
  Private _lastVersion As String = "00.00.00"
  Private _version As String = "00.00.00"

#Region " Constructors "

  Public Sub New()
  End Sub

  Public Sub New(ByVal ObjectId As Integer, ByVal Objectname As String, ByVal FriendlyName As String, ByVal InstallPath As String, ByVal ModuleId As Integer, ByVal PackageType As String)
   Me.ObjectId = ObjectId
   Me.ObjectName = Objectname
   Me.FriendlyName = FriendlyName
   Me.InstallPath = InstallPath
   Me.ModuleId = ModuleId
   Me.PackageType = PackageType
  End Sub

#End Region

#Region " Public Properties "
  Public Property ObjectId() As Integer
   Get
    Return _ObjectId
   End Get
   Set(ByVal Value As Integer)
    _ObjectId = Value
   End Set
  End Property

  Public Property ObjectName() As String
   Get
    Return _ObjectName
   End Get
   Set(ByVal Value As String)
    _ObjectName = Value
   End Set
  End Property

  Public Property FriendlyName() As String
   Get
    Return _FriendlyName
   End Get
   Set(ByVal Value As String)
    _FriendlyName = Value
   End Set
  End Property

  Public Property InstallPath() As String
   Get
    Return _installPath
   End Get
   Set(ByVal value As String)
    _installPath = value
   End Set
  End Property

  Public Property ModuleId() As Integer
   Get
    Return _moduleId
   End Get
   Set(ByVal value As Integer)
    _moduleId = value
   End Set
  End Property

  Public Property PackageType() As String
   Get
    Return _packageType
   End Get
   Set(ByVal value As String)
    _packageType = value
   End Set
  End Property

  Public Property [Module]() As ModuleInfo
   Get
    If _module Is Nothing Then
     _module = New ModuleInfo(ObjectId)
    End If
    Return _module
   End Get
   Set(ByVal value As ModuleInfo)
    _module = value
   End Set
  End Property

  Public Property LastVersion() As String
   Get
    Return _lastVersion
   End Get
   Set(ByVal value As String)
    _lastVersion = value
   End Set
  End Property

  Public Property Version() As String
   Get
    Return _version
   End Get
   Set(ByVal value As String)
    _version = value
   End Set
  End Property

  Public ReadOnly Property IsCore As Boolean
   Get
    Return CBool(ObjectName = Globals.glbCoreName)
   End Get
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

   ObjectId = Convert.ToInt32(Null.SetNull(dr.Item("ObjectId"), ObjectId))
   ObjectName = Convert.ToString(Null.SetNull(dr.Item("ObjectName"), ObjectName))
   FriendlyName = Convert.ToString(Null.SetNull(dr.Item("FriendlyName"), FriendlyName))
   InstallPath = Convert.ToString(Null.SetNull(dr.Item("InstallPath"), InstallPath))
   ModuleId = Convert.ToInt32(Null.SetNull(dr.Item("ModuleId"), ModuleId))
   PackageType = Convert.ToString(Null.SetNull(dr.Item("PackageType"), PackageType))
   Try
    LastVersion = Convert.ToString(Null.SetNull(dr.Item("LastVersion"), LastVersion))
   Catch
   End Try
   Try
    Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))
   Catch
   End Try

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
    Return ObjectId
   End Get
   Set(ByVal value As Integer)
    ObjectId = value
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
    Case "objectid"
     Return (Me.ObjectId.ToString(OutputFormat, formatProvider))
    Case "objectname"
     Return PropertyAccess.FormatString(Me.ObjectName, strFormat)
    Case "friendlyname"
     Return PropertyAccess.FormatString(Me.FriendlyName, strFormat)
    Case "installpath"
     Return PropertyAccess.FormatString(Me.InstallPath, strFormat)
    Case "moduleid"
     Return (Me.ModuleId.ToString(OutputFormat, formatProvider))
    Case "packagetype"
     Return PropertyAccess.FormatString(Me.PackageType, strFormat)
    Case "lastversion"
     Return PropertyAccess.FormatString(Me.LastVersion, strFormat)
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
    If Not Int32.TryParse(readElement(reader, "ObjectId"), ObjectId) Then
     ObjectId = Null.NullInteger
    End If
    ObjectName = readElement(reader, "ObjectName")
    FriendlyName = readElement(reader, "FriendlyName")
    InstallPath = readElement(reader, "InstallPath")
    If Not Int32.TryParse(readElement(reader, "ModuleId"), ModuleId) Then
     ModuleId = Null.NullInteger
    End If
    PackageType = readElement(reader, "PackageType")
   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of an Text", ex)
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
   writer.WriteStartElement("Object")
   writer.WriteElementString("ObjectId", ObjectId.ToString())
   writer.WriteElementString("ObjectName", ObjectName)
   writer.WriteElementString("FriendlyName", FriendlyName)
   writer.WriteElementString("InstallPath", InstallPath)
   writer.WriteElementString("ModuleId", ModuleId.ToString())
   writer.WriteElementString("PackageType", PackageType)
   writer.WriteEndElement()
  End Sub

#End Region

 End Class

End Namespace

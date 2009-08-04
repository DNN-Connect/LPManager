Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " PermissionInfo "
	<Serializable(), XmlRoot("Permission")> _
	Public Class PermissionInfo
		Implements IHydratable
		Implements IPropertyAccess
		Implements IXmlSerializable

		' local property declarations
		Private _PermissionId As Integer
		Private _ModuleId As Integer
		Private _UserId As Integer
        Private _ObjectId As Integer
        Private _Locale As String

        Private _displayName As String
        Private _userName As String
        Private _objectName As String

#Region " Constructors "
        Public Sub New()
        End Sub

        Public Sub New(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer)
            Me.Locale = Locale
            Me.ModuleId = ModuleId
            Me.ObjectId = ObjectId
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

        Public Property ObjectId() As Integer
            Get
                Return _ObjectId
            End Get
            Set(ByVal Value As Integer)
                _ObjectId = Value
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
            ObjectId = Convert.ToInt32(Null.SetNull(dr.Item("ObjectId"), ObjectId))
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
        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
            Dim OutputFormat As String = String.Empty
            Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
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
                Case "Objectid"
                    Return (Me.ObjectId.ToString(OutputFormat, formatProvider))
                Case "userid"
                    Return (Me.UserId.ToString(OutputFormat, formatProvider))
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
            writer.WriteElementString("ObjectId", ObjectId.ToString())
            writer.WriteElementString("UserId", UserId.ToString())
            writer.WriteElementString("Locale", Locale)
            writer.WriteElementString("ModuleId", ModuleId.ToString())
            writer.WriteEndElement()
        End Sub
#End Region

	End Class
#End Region

End Namespace

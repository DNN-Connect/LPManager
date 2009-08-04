Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " TextInfo "
	<Serializable(), XmlRoot("Text")> _
	Public Class TextInfo
		Implements IHydratable
		Implements IPropertyAccess
		Implements IXmlSerializable

		' local property declarations
		Private _TextId As Integer
		Private _DeprecatedIn As String
		Private _FilePath As String
        Private _ObjectId As Integer
        Private _OriginalValue As String
        Private _TextKey As String
        Private _Version As String
        Private _textValue As String = ""
        Private _locale As String = ""

#Region " Constructors "
        Public Sub New()
        End Sub

        Public Sub New(ByVal TextId As Integer, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)
            Me.DeprecatedIn = DeprecatedIn
            Me.FilePath = FilePath
            Me.ObjectId = ObjectId
            Me.OriginalValue = OriginalValue
            Me.TextId = TextId
            Me.TextKey = TextKey
            Me.Version = Version
        End Sub
#End Region

#Region " Public Properties "
        Public Property TextValue() As String
            Get
                If Locale = "" Then
                    Return _OriginalValue
                Else
                    Return _textValue
                End If
            End Get
            Set(ByVal value As String)
                If Locale = "" Then
                    _OriginalValue = value
                Else
                    _textValue = value
                End If
            End Set
        End Property

        Public Property Locale() As String
            Get
                Return _locale
            End Get
            Set(ByVal value As String)
                _locale = value
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

        Public Property DeprecatedIn() As String
            Get
                Return _DeprecatedIn
            End Get
            Set(ByVal Value As String)
                _DeprecatedIn = Value
            End Set
        End Property

        Public Property FilePath() As String
            Get
                Return _FilePath
            End Get
            Set(ByVal Value As String)
                _FilePath = Value
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

        Public Property OriginalValue() As String
            Get
                Return _OriginalValue
            End Get
            Set(ByVal Value As String)
                _OriginalValue = Value
            End Set
        End Property

        Public Property TextKey() As String
            Get
                Return _TextKey
            End Get
            Set(ByVal Value As String)
                _TextKey = Value.Replace("/", "\")
            End Set
        End Property

        Public Property Version() As String
            Get
                Return _Version
            End Get
            Set(ByVal Value As String)
                _Version = Value
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

            DeprecatedIn = Convert.ToString(Null.SetNull(dr.Item("DeprecatedIn"), DeprecatedIn))
            FilePath = Convert.ToString(Null.SetNull(dr.Item("FilePath"), FilePath))
            ObjectId = Convert.ToInt32(Null.SetNull(dr.Item("ObjectId"), ObjectId))
            OriginalValue = Convert.ToString(Null.SetNull(dr.Item("OriginalValue"), OriginalValue))
            TextId = Convert.ToInt32(Null.SetNull(dr.Item("TextId"), TextId))
            TextKey = Convert.ToString(Null.SetNull(dr.Item("TextKey"), TextKey))
            Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))
            Try
                Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
                If Locale <> "" Then
                    TextValue = Convert.ToString(Null.SetNull(dr.Item("TextValue"), TextValue))
                End If
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
                Return TextId
            End Get
            Set(ByVal value As Integer)
                TextId = value
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
                Case "deprecatedin"
                    Return PropertyAccess.FormatString(Me.DeprecatedIn, strFormat)
                Case "filepath"
                    Return PropertyAccess.FormatString(Me.FilePath, strFormat)
                Case "Objectid"
                    Return (Me.ObjectId.ToString(OutputFormat, formatProvider))
                Case "originalvalue"
                    Return PropertyAccess.FormatString(Me.OriginalValue, strFormat)
                Case "textid"
                    Return (Me.TextId.ToString(OutputFormat, formatProvider))
                Case "textkey"
                    Return PropertyAccess.FormatString(Me.TextKey, strFormat)
                Case "textvalue"
                    Return PropertyAccess.FormatString(Me.TextValue, strFormat)
                Case "locale"
                    Return PropertyAccess.FormatString(Me.Locale, strFormat)
                Case "version"
                    Return PropertyAccess.FormatString(Me.Version, strFormat)
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

                DeprecatedIn = readElement(reader, "DeprecatedIn")
                FilePath = readElement(reader, "FilePath")
                If Not Int32.TryParse(readElement(reader, "ObjectId"), ObjectId) Then
                    ObjectId = Null.NullInteger
                End If
                OriginalValue = readElement(reader, "OriginalValue")
                TextKey = readElement(reader, "TextKey")
                Version = readElement(reader, "Version")
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
            writer.WriteStartElement("Text")
            writer.WriteElementString("TextId", TextId.ToString())
            writer.WriteElementString("DeprecatedIn", DeprecatedIn)
            writer.WriteElementString("FilePath", FilePath)
            writer.WriteElementString("ObjectId", ObjectId.ToString())
            writer.WriteElementString("OriginalValue", OriginalValue)
            writer.WriteElementString("TextKey", TextKey)
            writer.WriteElementString("Version", Version)
            writer.WriteEndElement()
        End Sub
#End Region


	End Class
#End Region

End Namespace

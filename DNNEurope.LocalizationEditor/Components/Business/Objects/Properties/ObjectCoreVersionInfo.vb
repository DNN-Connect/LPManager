
Imports System
Imports System.Data
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Imports DNNEurope.Modules.LocalizationEditor.Data

Namespace Business
 
  Partial Public Class ObjectCoreVersionInfo

#Region " Private Members "
   Private _ObjectId as Int32
   Private _Version as String
   Private _CoreVersion as String
#End Region
	
#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(ByVal ObjectId As Int32, ByVal Version As String, ByVal CoreVersion As String)
   Me.ObjectId = ObjectId
   Me.Version = Version
   Me.CoreVersion = CoreVersion
  End Sub
#End Region
	
#Region " Public Properties "
  Public Property ObjectId() as Int32
   Get
    Return _ObjectId
   End Get
   Set(ByVal Value as Int32)
    _ObjectId = Value
   End Set
  End Property

  Public Property Version() as String
   Get
    Return _Version
   End Get
   Set(ByVal Value as String)
    _Version = Value
   End Set
  End Property

  Public Property CoreVersion() as String 
   Get
    Return _CoreVersion
   End Get
   Set(ByVal Value as String)
    _CoreVersion = Value
   End Set
  End Property

#End Region

 End Class
End Namespace



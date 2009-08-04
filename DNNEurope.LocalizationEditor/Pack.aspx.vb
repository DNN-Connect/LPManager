Imports DNNEurope.Modules.LocalizationEditor.Business

Namespace DNNEurope.Modules.LocalizationEditor
	Partial Public Class Pack
		Inherits DotNetNuke.Framework.PageBase

#Region " Private Members "
        Private _ObjectId As Integer = -1
        Private _locale As String = ""
        Private _moduleName As String = ""
        Private _version As String = ""
#End Region

#Region " Properties "
        Public Property ModuleName() As String
            Get
                Return _moduleName
            End Get
            Set(ByVal value As String)
                _moduleName = value
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

        Public Property ObjectId() As Integer
            Get
                Return _ObjectId
            End Get
            Set(ByVal value As Integer)
                _ObjectId = value
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
#End Region

#Region " Event Handlers "
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
            Globals.ReadQuerystringValue(Me.Request.Params, "Locale", Locale)
            Globals.ReadQuerystringValue(Me.Request.Params, "Version", Version)

            Dim tm As ObjectInfo = ObjectController.GetObject(ObjectId)
            If tm Is Nothing Then Throw New ArgumentException(String.Format("ObjectId with value {0} is not valid.", ObjectId))
            ModuleName = tm.ObjectName.Replace("\", "_").Replace("/", "_")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim fn As String = LocalizationController.CreateResourcePack(ObjectId, ModuleName, Version, Locale)
            Me.Response.Redirect(DotNetNuke.Common.HostPath & "/LocalizationEditor/" & fn, False)
        End Sub
#End Region

	End Class
End Namespace
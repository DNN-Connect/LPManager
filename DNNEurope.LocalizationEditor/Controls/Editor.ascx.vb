Imports DNNEurope.Modules.LocalizationEditor.Helpers

Namespace DNNEurope.Modules.LocalizationEditor.Controls
	Partial Public Class Editor
		Inherits ModuleBase

#Region " Unrecognized Controls "
		Protected WithEvents teValue As DotNetNuke.UI.UserControls.TextEditor
#End Region

#Region " Private Members "
		Private _translationId As Integer
		Private _textId As Integer
		Private _value As String
		Private _originalValue As String
        Private _sourceValue As String
        Private _showHtml As Boolean = False
        Private _fromLocale As String
        Private _toLocale As String
        Private _autoTranslate As Boolean
        Private _panelID As Integer
#End Region

#Region " Properties "
        Public Property TranslationId() As Integer
            Get
                Return _translationId
            End Get
            Set(ByVal value As Integer)
                _translationId = value
            End Set
        End Property

        Public Property SourceValue() As String
            Get
                Return _sourceValue
            End Get
            Set(ByVal value As String)
                _sourceValue = value
            End Set
        End Property

        Public Property OriginalValue() As String
            Get
                Return _originalValue
            End Get
            Set(ByVal value As String)
                _originalValue = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

        Public Property TextId() As Integer
            Get
                Return _textId
            End Get
            Set(ByVal value As Integer)
                _textId = value
            End Set
        End Property

        Public Property ShowHtml() As Boolean
            Get
                Return _showHtml
            End Get
            Set(ByVal value As Boolean)
                _showHtml = value
            End Set
        End Property

        Public Property FromLocale() As String
            Get
                Return _fromLocale
            End Get
            Set(ByVal value As String)
                _fromLocale = value
            End Set
        End Property

        Public Property ToLocale() As String
            Get
                Return _toLocale
            End Get
            Set(ByVal value As String)
                _toLocale = value
            End Set
        End Property

        Public Property AutoTranslate() As Boolean
            Get
                Return _autoTranslate
            End Get
            Set(ByVal value As Boolean)
                _autoTranslate = value
            End Set
        End Property

        Public Property PanelID() As Integer
            Get
                Return _panelID
            End Get
            Set(ByVal value As Integer)
                _panelID = value
            End Set
        End Property
#End Region

#Region " Events "
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            cmdSwitch.Visible = False

            If Not Me.IsPostBack Then
                Dim OriginalLength As Integer = OriginalValue.Length
                Dim height As Integer = 0
                If OriginalLength > 300 Then
                    height = 200
                ElseIf OriginalLength > 100 Then
                    height = 100
                ElseIf OriginalLength > 50 Then
                    height = 50
                End If
                If height > 0 Then
                    txtValue.Height = Unit.Pixel(height)
                    txtValue.TextMode = TextBoxMode.MultiLine
                End If
                If Globals.IsGoodHtml(OriginalValue) Then
                    'ShowHtml = True
                End If
                txtValue.Attributes.Add("panelId", PanelID.ToString)
                teValue.Attributes.Add("panelId", PanelID.ToString)
                Me.DataBind()
            Else
                Dim paramName As String = ""
                If ShowHtml Then
                    paramName = teValue.ClientID & "_teValue"
                Else
                    paramName = txtValue.ClientID
                End If
                If Me.Request.Params(paramName) Is Nothing Then
                    paramName = paramName.Replace("_", "$")
                    If Me.Request.Params(paramName) IsNot Nothing Then
                        Value = Me.Request.Params(paramName)
                    End If
                Else
                    Value = Me.Request.Params(paramName)
                End If
            End If
        End Sub

#End Region

#Region " Overrides "
        Public Overrides Sub DataBind()
            pnlHtmlValue.Visible = ShowHtml
            pnlTextbox.Visible = Not ShowHtml
            txtValue.Text = Value
            teValue.Text = Value

            'Apply translation -- NOT!
            'Translate()
        End Sub

        ''' <summary>
        ''' Applies translation to the current value
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Translate()
            Try
                '// Add default google translation value if empty
                If AutoTranslate AndAlso String.IsNullOrEmpty(Value) Then
                    '// Determine from and to languages
                    Dim fromLanguage As Google.API.Language = Google.API.Translate.LanguageUtility.GetLanguageFromLocale(FromLocale)
                    Dim toLanguage As Google.API.Language = Google.API.Translate.LanguageUtility.GetLanguageFromLocale(ToLocale)

                    If fromLanguage = Google.API.Language.Unknown Then
                        fromLanguage = Google.API.Language.English 'TODO Make default configurable?
                    End If

                    '// Skip translation if a target language is unknown
                    If toLanguage = Google.API.Language.Unknown Then Return

                    '// Perform translation and set to textbox
                    txtValue.Text = TranslateHelper.Translate(OriginalValue, fromLanguage, toLanguage)
                End If
            Catch ex As Google.API.GoogleAPIException
                DotNetNuke.Services.Exceptions.LogException(ex)

                'TODO inform user that translation has failed
                txtValue.Text = "?"
            End Try
        End Sub
#End Region

#Region " ViewState Handling "
        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If Not (savedState Is Nothing) Then
                Dim myState As Object() = CType(savedState, Object())
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If
                If Not (myState(1) Is Nothing) Then
                    ShowHtml = CBool(myState(1))
                End If
                If Not (myState(2) Is Nothing) Then
                    TextId = CInt(myState(2))
                End If
                If Not (myState(3) Is Nothing) Then
                    Value = CStr(myState(3))
                End If
                If Not (myState(4) Is Nothing) Then
                    TranslationId = CInt(myState(4))
                End If
            End If
        End Sub

        Protected Overrides Function SaveViewState() As Object
            Dim allStates(4) As Object
            allStates(0) = MyBase.SaveViewState()
            allStates(1) = ShowHtml
            allStates(2) = TextId
            allStates(3) = Value
            allStates(4) = TranslationId
            Return allStates
        End Function
#End Region

    End Class
End Namespace
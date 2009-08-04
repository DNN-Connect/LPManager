'
' Copyright (c) 2004-2005
' by Peter Donker ( info@bring2mind.net )
'
' Permission is hereby granted to use and/or modify this software for a single server. It is 
' NOT permitted to publish, distribute, sublicense, and/or sell copies of this software in its
' original or modified form. You are not allowed to remove this copyright notice.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
' BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
' NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
' DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OF THE SOFTWARE.
'

Namespace DNNEurope.Modules.LocalizationEditor
	Public Class ModuleBase
		Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region " Variables "
		Private _settings As ModuleSettings
#End Region

#Region " Properties "
		Public Shadows Property Settings() As ModuleSettings
			Get

				If _settings Is Nothing Then
					Dim CacheKey As String = "Settings4Module" & Me.ModuleId & "inPortal" & Me.PortalId.ToString
					Try
						_settings = CType(Me.Cache.Item(CacheKey), ModuleSettings)
					Catch ex As Exception
					End Try
					If _settings Is Nothing Then
						_settings = New ModuleSettings(ModuleId)
						Me.Cache.Add(CacheKey, _settings, Nothing, Date.MaxValue, New System.TimeSpan(1, 0, 0), Caching.CacheItemPriority.Low, Nothing)
					End If
				End If

				Return _settings

			End Get
			Set(ByVal Value As ModuleSettings)
				_settings = Value
			End Set
		End Property

#End Region

	End Class

End Namespace
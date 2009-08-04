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
Imports System.Web.Caching
Imports DotNetNuke.Entities.Modules

Namespace DNNEurope.Modules.LocalizationEditor
    Public Class ModuleBase
        Inherits PortalModuleBase

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
                        Me.Cache.Add(CacheKey, _settings, Nothing, Date.MaxValue, New TimeSpan(1, 0, 0), _
                                      CacheItemPriority.Low, Nothing)
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
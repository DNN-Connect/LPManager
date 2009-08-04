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
Imports DotNetNuke.Entities.Modules

Namespace DNNEurope.Modules.LocalizationEditor
    Public Class ModuleSettings

#Region " Private Members "

        Private _Locales As String = ""
        Private _objects As String = String.Empty
        Private _removeBlanksFromFile As Boolean = False
        Private _addNewEntriesAsBlank As Boolean = False

#End Region

#Region " Constructors "

        Public Sub New(ByVal ModuleId As Integer)

            Dim mc As New ModuleController
            Dim Settings As Hashtable = mc.GetModuleSettings(ModuleId)

            If Not Settings.Item("Locales") Is Nothing Then
                Locales = CType(Settings.Item("Locales"), String)
            End If

            If Not Settings.Item("Objects") Is Nothing Then
                Objects = CType(Settings.Item("Objects"), String)
            End If

            If Not Settings.Item("RemoveBlanksFromFile") Is Nothing Then
                RemoveBlanksFromFile = CType(Settings.Item("RemoveBlanksFromFile"), Boolean)
            End If

            If Not Settings.Item("AddNewEntriesAsBlank") Is Nothing Then
                AddNewEntriesAsBlank = CType(Settings.Item("AddNewEntriesAsBlank"), Boolean)
            End If

        End Sub

#End Region

#Region " Public Members "

        Public Sub SaveSettings(ByVal ModuleId As Integer)

            Dim objModules As New ModuleController
            objModules.UpdateModuleSetting(ModuleId, "Locales", Me.Locales.ToString)
            objModules.UpdateModuleSetting(ModuleId, "Objects", Me.Objects.ToString)
            objModules.UpdateModuleSetting(ModuleId, "RemoveBlanksFromFile", Me.RemoveBlanksFromFile.ToString)
            objModules.UpdateModuleSetting(ModuleId, "AddNewEntriesAsBlank", Me.AddNewEntriesAsBlank.ToString)

        End Sub

#End Region

#Region " Properties "

        Public Property Locales() As String
            Get
                Return _Locales
            End Get
            Set(ByVal Value As String)
                _Locales = Value
            End Set
        End Property

        Public Property Objects() As String
            Get
                Return _objects
            End Get
            Set(ByVal value As String)
                _objects = value
            End Set
        End Property

        Public Property RemoveBlanksFromFile() As Boolean
            Get
                Return _removeBlanksFromFile
            End Get
            Set(ByVal value As Boolean)
                _removeBlanksFromFile = value
            End Set
        End Property

        Public Property AddNewEntriesAsBlank() As Boolean
            Get
                Return _addNewEntriesAsBlank
            End Get
            Set(ByVal value As Boolean)
                _addNewEntriesAsBlank = value
            End Set
        End Property

#End Region
    End Class
End Namespace
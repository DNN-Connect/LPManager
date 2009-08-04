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

Namespace DNNEurope.Modules.LocalizationEditor.LocaleFile
    Public Class LocaleFileInfo
        Private _LocaleFileName As String
        Private _LocaleFileType As LocaleType
        Private _LocalePath As String
        Private _LocaleModule As String
        Private _Buffer As Byte()

        Public Sub New()
            MyBase.new()
        End Sub

        Public Sub New(ByVal FileName As String, ByVal FileType As LocaleType, ByVal ModuleName As String, _
                        ByVal Path As String)
            MyBase.new()

            _LocaleFileName = FileName
            _LocaleFileType = FileType
            _LocaleModule = ModuleName
            _LocalePath = Path

        End Sub

        <XmlIgnore()> _
        Public Property Buffer() As Byte()
            Get
                Return _Buffer
            End Get
            Set(ByVal Value As Byte())
                _Buffer = Value
            End Set
        End Property

        <XmlAttributeAttribute("FileName")> _
        Public Property LocaleFileName() As String
            Get
                Return _LocaleFileName
            End Get
            Set(ByVal Value As String)
                _LocaleFileName = Value
            End Set
        End Property

        <XmlAttributeAttribute("FileType")> _
        Public Property LocaleFileType() As LocaleType
            Get
                Return _LocaleFileType
            End Get
            Set(ByVal Value As LocaleType)
                _LocaleFileType = Value
            End Set
        End Property

        <XmlAttributeAttribute("ModuleName")> _
        Public Property LocaleModule() As String
            Get
                Return _LocaleModule
            End Get
            Set(ByVal Value As String)
                _LocaleModule = Value
            End Set
        End Property

        <XmlAttributeAttribute("FilePath")> _
        Public Property LocalePath() As String
            Get
                Return _LocalePath
            End Get
            Set(ByVal Value As String)
                _LocalePath = Value
            End Set
        End Property
    End Class
End Namespace

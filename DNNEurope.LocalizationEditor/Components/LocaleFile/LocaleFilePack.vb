'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System.Xml.Serialization
Imports DotNetNuke.Services.Localization

Namespace DNNEurope.Modules.LocalizationEditor.LocaleFile
	<XmlRoot("LanguagePack")> _
	Public Class LocaleFilePack
		Private _LocalePackCulture As New Locale
		Private _Version As String
		Private _Files As New LocaleFileCollection

		<XmlAttributeAttribute()> _
		Public Property Version() As String
			Get
				Return _Version
			End Get
			Set(ByVal Value As String)
				_Version = Value
			End Set
		End Property

		<XmlElement("Culture")> _
		Public Property LocalePackCulture() As Locale
			Get
				Return _LocalePackCulture
			End Get
			Set(ByVal Value As Locale)
				_LocalePackCulture = Value
			End Set
		End Property

		<XmlArrayItem("File", GetType(LocaleFileInfo))> _
		Public Property Files() As LocaleFileCollection
			Get
				Return _Files
			End Get
			Set(ByVal Value As LocaleFileCollection)
				_Files = Value
			End Set
		End Property

	End Class
End Namespace
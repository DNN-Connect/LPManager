' 
' Copyright (c) 2004-2011 DNN-Europe, http://www.dnn-europe.net
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
Imports System.Text.RegularExpressions

Namespace Services.DataExchange
 ' Simple class to handle http response and cluster processing
 Public Class WebPage

#Region " Private Members "
  Private _BodyContents As String
  Private _Raw As String
#End Region

#Region " Constructors "
  Public Sub New(ByVal RawContent As String)
   _Raw = RawContent
   _BodyContents = Regex.Match(_Raw, "(?si)<body.*body>(?-si)").ToString
   _BodyContents = Regex.Replace(_BodyContents, "(?si)(<body[^>]*>)|(</body>)(?-si)", "")
   _BodyContents = Regex.Replace(_BodyContents, "(?si)<form.*?</form>(?-si)", "")
  End Sub
#End Region

#Region " Properties "
  Public Property BodyContents() As String
   Get
    Return _BodyContents
   End Get
   Set(ByVal Value As String)
    _BodyContents = Value
   End Set
  End Property

  Public Property Raw() As String
   Get
    Return _Raw
   End Get
   Set(ByVal Value As String)
    _Raw = Value
   End Set
  End Property
#End Region

 End Class
End Namespace

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
Imports System
Imports System.Runtime.Serialization

Namespace Entities.Texts
  Partial Public Class TextInfo

#Region " Private Members "
#End Region
	
#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(ByVal TextId As Int32, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Int32, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)
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
  <DataMember()>
  Public Property DeprecatedIn As String = ""
  <DataMember()>
  Public Property FilePath As String = ""
  <DataMember()>
  Public Property ObjectId As Int32 = -1
  <DataMember()>
  Public Property OriginalValue As String = ""
  <DataMember()>
  Public Property TextId As Int32 = -1
  <DataMember()>
  Public Property TextKey As String = ""
  <DataMember()>
  Public Property Version As String = ""
#End Region

 End Class
End Namespace



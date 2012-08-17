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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Utilities
Imports System.Runtime.Serialization

Namespace Entities.Texts

 Partial Public Class TextInfo
  Implements IHydratable

  ' local property declarations
  Private _textValue As String = ""
  Private _locale As String = ""

#Region " Public Properties "

  <DataMember()>
  Public Property TextValue() As String
   Get
    If Locale = "" Then
     Return _OriginalValue
    Else
     Return _textValue
    End If
   End Get
   Set(ByVal value As String)
    If Locale = "" Then
     _OriginalValue = value
    Else
     _textValue = value
    End If
   End Set
  End Property

  <DataMember()>
  Public Property Locale() As String
   Get
    Return _locale
   End Get
   Set(ByVal value As String)
    _locale = value
   End Set
  End Property

  <DataMember()>
  Public Property Translation As String = ""
  <DataMember()>
  Public Property LastModified As Date = Date.MinValue
  <DataMember()>
  Public Property LastModifiedUserId As Integer = -1
#End Region

#Region " IHydratable Implementation "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   DeprecatedIn = Convert.ToString(Null.SetNull(dr.Item("DeprecatedIn"), DeprecatedIn))
   FilePath = Convert.ToString(Null.SetNull(dr.Item("FilePath"), FilePath))
   ObjectId = Convert.ToInt32(Null.SetNull(dr.Item("ObjectId"), ObjectId))
   OriginalValue = Convert.ToString(Null.SetNull(dr.Item("OriginalValue"), OriginalValue))
   TextId = Convert.ToInt32(Null.SetNull(dr.Item("TextId"), TextId))
   TextKey = Convert.ToString(Null.SetNull(dr.Item("TextKey"), TextKey))
   Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))
   Try
    Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
    If Locale <> "" Then
     TextValue = Convert.ToString(Null.SetNull(dr.Item("TextValue"), TextValue))
    End If
    Translation = Convert.ToString(Null.SetNull(dr.Item("TextValue"), Translation))
    LastModified = Convert.ToDateTime(Null.SetNull(dr.Item("LastModified"), LastModified))
    LastModifiedUserId = Convert.ToInt32(Null.SetNull(dr.Item("LastModifiedUserId"), LastModifiedUserId))
   Catch
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return TextId
   End Get
   Set(ByVal value As Integer)
    TextId = value
   End Set
  End Property

#End Region

 End Class

End Namespace

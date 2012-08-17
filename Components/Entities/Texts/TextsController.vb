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
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Common.Utilities

Namespace Entities.Texts

#Region " TextsController "

 Public Class TextsController

  Public Shared Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As TextInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetLatestText(ObjectId, FilePath, Locale, TextKey), GetType(TextInfo)), TextInfo)
  End Function

  Public Shared Function GetTextByVersion(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal TextKey As String, ByVal Version As String) As TextInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetTextByVersion(ObjectId, FilePath, TextKey, Version), GetType(TextInfo)), TextInfo)
  End Function

  Public Shared Function GetOldestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal TextKey As String) As TextInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetOldestText(ObjectId, FilePath, TextKey), GetType(TextInfo)), TextInfo)
  End Function

  Public Shared Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As TextInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetText(ObjectId, FilePath, Locale, Version, TextKey), GetType(TextInfo)), TextInfo)
  End Function

  Public Shared Function GetTextsByObject(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDictionary(Of Integer, TextInfo)
   Return CBO.FillDictionary(Of TextInfo)(DataProvider.Instance.GetTextsByObject(ModuleId, ObjectId, Locale, Version))
  End Function

  Public Shared Function GetTextsByObjectAndFile(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As Dictionary(Of String, TextInfo)
   Dim res As New Dictionary(Of String, TextInfo)
   Using ir As IDataReader = DataProvider.Instance.GetTextsByObjectAndFile(ModuleId, ObjectId, FilePath, Locale, Version, IncludeNonTranslated)
    Do While ir.Read
     Dim ti As New TextInfo
     ti.Fill(ir)
     res.Add(ti.TextKey, ti)
    Loop
   End Using
   Return res
  End Function

  Public Shared Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As String
   Using ir As IDataReader = DataProvider.Instance.CurrentVersion(ObjectId, Locale)
    If ir.Read Then
     Return Globals.GetAString(ir.Item(0))
    End If
   End Using
   Return "00.00.00"
  End Function

  Public Shared Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As Integer
   Using ir As IDataReader = DataProvider.Instance.NrOfChangedTexts(ObjectId, Version)
    If ir.Read Then
     Return Globals.GetAnInteger(ir.Item(0))
    End If
   End Using
   Return -1
  End Function

  Public Shared Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As Integer
   Using ir As IDataReader = DataProvider.Instance.NrOfFiles(ObjectId, Version)
    If ir.Read Then
     Return Globals.GetAnInteger(ir.Item(0))
    End If
   End Using
   Return -1
  End Function

  Public Shared Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As Integer
   Return DataProvider.Instance.NrOfItems(ObjectId, Version)
   'Using ir As IDataReader = DataProvider.Instance.NrOfItems(ObjectId, Version)
   ' If ir.Read Then
   '  Return Globals.GetAnInteger(ir.Item(0))
   ' End If
   'End Using
   'Return -1
  End Function

  Public Shared Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As Integer
   Using ir As IDataReader = DataProvider.Instance.NrOfMissingTranslations(ObjectId, Locale, Version)
    If ir.Read Then
     Return Globals.GetAnInteger(ir.Item(0))
    End If
   End Using
   Return -1
  End Function

  Public Shared Function GetVersions(ByVal ObjectId As Integer) As List(Of String)
   Dim res As New List(Of String)
   Using ir As IDataReader = DataProvider.Instance.GetVersions(ObjectId)
    Do While ir.Read
     res.Add(Globals.GetAString(ir.Item(0)))
    Loop
   End Using
   Return res
  End Function

  Public Shared Function GetFileList(ByVal ObjectId As Integer, ByVal Version As String) As List(Of String)
   Dim res As New List(Of String)
   Using ir As IDataReader = DataProvider.Instance.GetFiles(ObjectId, Version)
    Do While ir.Read
     res.Add(Globals.GetAString(ir.Item(0)))
    Loop
   End Using
   Return res
  End Function
 End Class

#End Region

End Namespace

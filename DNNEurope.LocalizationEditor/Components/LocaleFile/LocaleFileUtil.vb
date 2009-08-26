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


Namespace LocaleFile
 Public Class LocaleFileUtil
  Public Shared Function GetFullFileName(ByVal LocaleFile As LocaleFileInfo) As String
   Dim Result As String = LocaleFile.LocaleFileType.ToString & "\"
   If Not LocaleFile.LocaleModule Is Nothing Then
    Result &= LocaleFile.LocaleModule & "\"
   End If
   If Not LocaleFile.LocalePath Is Nothing Then
    Result &= LocaleFile.LocalePath & "\"
   End If
   Result &= LocaleFile.LocaleFileName
   Return Result
  End Function
 End Class
End Namespace

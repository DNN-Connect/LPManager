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
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip

Namespace Helpers
 Public Class ZipHelper

  Public Shared Sub Unzip(ByVal fileStream As Stream, ByVal tempDirectory As String)
   Dim objZipEntry As ZipEntry
   Using objZipInputStream As New ZipInputStream(fileStream)
    objZipEntry = objZipInputStream.GetNextEntry
    While Not objZipEntry Is Nothing
     Dim strFileName As String = objZipEntry.Name.Replace("/", "\")
     If strFileName <> "" And Not objZipEntry.IsDirectory Then
      Dim sFile As String = strFileName
      Dim sPath As String = tempDirectory & "\"
      If strFileName.IndexOf("\"c) > 0 Then
       sFile = Mid(strFileName, strFileName.LastIndexOf("\"c) + 2)
       sPath = sPath & Left(strFileName, strFileName.LastIndexOf("\"c))
       If Not Directory.Exists(sPath) Then
        Directory.CreateDirectory(sPath)
       End If
       sPath &= "\"
      End If
      If Not IO.Directory.Exists(IO.Path.GetDirectoryName(sPath & sFile)) Then
       IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(sPath & sFile))
      End If
      Using objFileStream As FileStream = File.Create(sPath & sFile)
       Dim intSize As Integer = 2048
       Dim arrData(2048) As Byte
       intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
       While intSize > 0
        objFileStream.Write(arrData, 0, intSize)
        intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
       End While
      End Using
     End If
     objZipEntry = objZipInputStream.GetNextEntry
    End While
   End Using
  End Sub

  Public Shared Sub Unzip(ByVal filePath As String, ByVal tempDirectory As String)
   Using fileStrm As IO.FileStream = File.Open(filePath, FileMode.Open, FileAccess.Read)
    Unzip(fileStrm, tempDirectory)
   End Using
  End Sub

 End Class
End Namespace

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
Imports Google.API
Imports Google.API.Translate

Namespace Helpers
 Public Class TranslateHelper
  Private Const MaxLength As Integer = 2048 - 100
  '// A maximum of 2048 characters in the URL is allowed, subtract some characters for the Google service URL

  ''' <summary>
  ''' Translates a given text from one to another language
  ''' </summary>
  ''' <param name="originalValue">The text to be translated</param>
  ''' <param name="fromLanguage">The source language</param>
  ''' <param name="toLanguage">The destination language</param>
  ''' <returns>The translated text</returns>
  Public Shared Function Translate(ByVal originalValue As String, ByVal fromLanguage As Language, ByVal toLanguage As Language) As String
   '// Do no translate null or empty text
   If String.IsNullOrEmpty(originalValue) Then Return String.Empty

   '// Split the original text if it exceeds the maximum length
   'TODO Make splitting intelligent by splitting at whitespace, comma, dot or other special characters
   If HttpUtility.HtmlEncode(originalValue).Length > MaxLength Then
    Dim sb As New StringBuilder
    Dim i As Integer = 0
    While (i < originalValue.Length)
     Dim substringLength As Integer = MaxLength
     Dim textPart As String = String.Empty
     While True
      textPart = originalValue.Substring(i, Math.Min(substringLength, originalValue.Length - i))

      '// Check if the text part exceeds the maximum length when URL encoded 
      If HttpUtility.HtmlEncode(textPart).Length <= MaxLength Then
       Exit While
      Else
       '// Decrease the substring length until it's encoded value length is less then the maximum
       substringLength -= 100
      End If

      If substringLength <= 0 Then Throw New ArgumentException("Given text to translate could not be processed.")
     End While

     sb.Append(Translator.Translate(textPart, fromLanguage, toLanguage, TranslateFormat.text))
     i += textPart.Length
    End While
    Return sb.ToString()
   End If

   Return Translator.Translate(originalValue, fromLanguage, toLanguage, TranslateFormat.text)
  End Function
 End Class
End Namespace
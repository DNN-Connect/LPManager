Namespace DNNEurope.Modules.LocalizationEditor.Helpers
	Public Class TranslateHelper
		Private Const MaxLength As Integer = 2048 - 100	'// A maximum of 2048 characters in the URL is allowed, subtract some characters for the Google service URL

		''' <summary>
		''' Translates a given text from one to another language
		''' </summary>
		''' <param name="originalValue">The text to be translated</param>
		''' <param name="fromLanguage">The source language</param>
		''' <param name="toLanguage">The destination language</param>
		''' <returns>The translated text</returns>
		Public Shared Function Translate(ByVal originalValue As String, ByVal fromLanguage As Google.API.Language, ByVal toLanguage As Google.API.Language) As String
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

					sb.Append(Google.API.Translate.Translator.Translate(textPart, fromLanguage, toLanguage, Google.API.Translate.TranslateFormat.text))
					i += textPart.Length
				End While
				Return sb.ToString()
			End If

			Return Google.API.Translate.Translator.Translate(originalValue, fromLanguage, toLanguage, Google.API.Translate.TranslateFormat.text)
		End Function
	End Class
End Namespace
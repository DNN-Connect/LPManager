Imports System.IO
Imports System.Xml
Imports DotNetNuke.Services.Localization

Namespace DNNEurope.Modules.LocalizationEditor
	Public Class Globals

		Public Shared Function ResourceFile(ByVal filename As String, ByVal language As String) As String
			Dim resourcefilename As String = filename

			If language <> Localization.SystemLocale Then
				resourcefilename = resourcefilename.Replace(".resx", "." + language + ".resx")
			End If

			Return resourcefilename

		End Function

		Public Shared Sub GetResourceFiles(ByRef fileList As SortedList, ByVal _path As String)
			GetResourceFiles(fileList, _path, "")
		End Sub

		Public Shared Sub GetResourceFiles(ByRef fileList As SortedList, ByVal _path As String, ByVal locale As String)
			Dim folders As String() = Directory.GetDirectories(_path)
			Dim folder As String
			Dim objFile As IO.FileInfo
			Dim objFolder As DirectoryInfo
			Dim pattern As String = ".resx"
			If locale <> "" Then
				pattern = "." & locale & pattern
			End If

			For Each folder In folders
				objFolder = New System.IO.DirectoryInfo(folder)

				If objFolder.Name = Localization.LocalResourceDirectory Then
					' found local resource folder, add resources
					For Each objFile In objFolder.GetFiles("*.ascx" & pattern)
						fileList.Add(objFile.FullName, objFile)
					Next
					For Each objFile In objFolder.GetFiles("*.aspx" & pattern)
						fileList.Add(objFile.FullName, objFile)
					Next
					' add LocalSharedResources if found
					If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
						fileList.Add(Path.Combine(folder, Localization.LocalSharedResourceFile), New System.IO.FileInfo(Path.Combine(folder, Localization.LocalSharedResourceFile)))
					End If
				Else
					GetResourceFiles(fileList, folder, locale)
				End If
			Next
		End Sub

		Public Shared Sub RemoveResourceFiles(ByRef fileList As SortedList, ByVal _path As String)
			RemoveResourceFiles(fileList, _path, "")
		End Sub

		Public Shared Sub RemoveResourceFiles(ByRef fileList As SortedList, ByVal _path As String, ByVal locale As String)
			Dim folders As String() = Directory.GetDirectories(_path)
			Dim folder As String
			Dim objFile As IO.FileInfo
			Dim objFolder As DirectoryInfo
			Dim pattern As String = ".resx"
			If locale <> "" Then
				pattern = "." & locale & pattern
			End If

			For Each folder In folders
				objFolder = New System.IO.DirectoryInfo(folder)

				If objFolder.Name = Localization.LocalResourceDirectory Then
					' found local resource folder, add resources
					For Each objFile In objFolder.GetFiles("*.ascx" & pattern)
						fileList.Remove(objFile.FullName)
					Next
					For Each objFile In objFolder.GetFiles("*.aspx" & pattern)
						fileList.Remove(objFile.FullName)
					Next
					' add LocalSharedResources if found
					If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
						fileList.Remove(Path.Combine(folder, Localization.LocalSharedResourceFile))
					End If
				Else
					RemoveResourceFiles(fileList, folder, locale)
				End If
			Next

		End Sub

		Public Shared Sub EnsureUTF8Encoding(ByVal Filename As String)

			Dim IsUTF8 As Boolean = False
			Dim sr As New IO.StreamReader(Filename, True)
			Dim buffer As String = sr.ReadToEnd
			sr.Close()
			Dim sw As New IO.StreamWriter(Filename, False, System.Text.Encoding.UTF8)
			sw.Write(buffer)
			sw.Close()
			sr.Close()

		End Sub

		Public Enum EditorType
			Textbox
			DNNLabel
		End Enum

		Public Shared Function TranslateFilenames(ByVal sourceText As String, ByVal targetLocale As String) As String
			If targetLocale.ToLower = "en-us" Then
				Return System.Text.RegularExpressions.Regex.Replace(sourceText, "(\.as\wx\.)(\w\w-\w\w\.|)(resx)", "$1$3")
			Else
				Return System.Text.RegularExpressions.Regex.Replace(sourceText, "(\.as\wx\.)(\w\w-\w\w\.|)(resx)", "$1" & targetLocale & ".$3")
			End If
		End Function

		Public Shared Sub ReadQuerystringValue(ByVal parameters As NameValueCollection, ByVal parameterName As String, ByRef value As String)
			If parameters(parameterName) IsNot Nothing Then
				value = parameters(parameterName)
			End If
		End Sub

		Public Shared Sub ReadQuerystringValue(ByVal parameters As NameValueCollection, ByVal parameterName As String, ByRef value As Integer)
			If parameters(parameterName) IsNot Nothing Then
				value = CInt(parameters(parameterName))
			End If
		End Sub

		Public Shared Function IsGoodHtml(ByVal text As String) As Boolean
			Return Regex.Match(text, "<\w[^>]*>[^<]*</\w[^>]*>").Success
		End Function

		Public Shared Function IsAnyHtml(ByVal text As String) As Boolean
			Return Regex.Match(text, "<(/)?\w").Success
		End Function

		Public Shared Function GetAString(ByVal Value As Object) As String
			If Value Is Nothing Then
				Return ""
			Else
				If Value Is DBNull.Value Then
					Return ""
				Else
					Return CStr(Value)
				End If
			End If
		End Function

		Public Shared Function GetABoolean(ByVal x As Object) As Boolean
			If x Is Nothing Then
				Return False
			Else
				If x Is DBNull.Value Then
					Return False
				Else
					Try
						Return CType(x, Boolean)
					Catch ex As Exception
						If TypeOf x Is String Then
							If CType(x, String).ToLower = "on" Then
								Return True
							ElseIf CType(x, String).ToLower = "yes" Then
								Return True
							ElseIf CType(x, String).ToLower = "no" Then
								Return False
							ElseIf CType(x, String).ToLower = "off" Then
								Return False
							End If
						End If
						Return False
					End Try
				End If
			End If
		End Function

		Public Shared Function GetAnInteger(ByVal Value As Object) As Integer

			If Value Is Nothing Then
				Return 0
			Else
				If Value Is DBNull.Value Then
					Return 0
				Else
					Return CType(Value, Integer)
				End If
			End If

		End Function

		Public Shared Sub AddAttribute(ByRef node As XmlNode, ByVal propName As String, ByVal propValue As String)
			Dim xAtt As XmlAttribute = node.OwnerDocument.CreateAttribute(propName)
			xAtt.InnerText = propValue
			node.Attributes.Append(xAtt)
		End Sub

		Public Shared Sub AddElement(ByRef node As XmlNode, ByVal elementName As String, ByVal elementValue As String, ByVal ParamArray attributes() As String)
			Dim newNode As XmlNode = node.OwnerDocument.CreateElement(elementName)
			newNode.InnerText = elementValue
			node.AppendChild(newNode)
			For Each xAttribute As String In attributes
				Dim x As String() = xAttribute.Split("="c)
				AddAttribute(newNode, x(0), x(1))
			Next
		End Sub

		Public Shared Sub AddResourceText(ByRef resourceRoot As XmlNode, ByVal textKey As String, ByVal textValue As String)
			Dim newNode As XmlNode = resourceRoot.OwnerDocument.CreateElement("data")
			resourceRoot.AppendChild(newNode)
			AddAttribute(newNode, "name", textKey)
			AddElement(newNode, "value", textValue)
		End Sub

		Public Shared Sub SimpleLog(ByVal ParamArray Messages As String())

			Dim LogPath As String = DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Log\"
			If Not IO.Directory.Exists(LogPath) Then
				IO.Directory.CreateDirectory(LogPath)
			End If
			Dim File As String = LogPath & "Log-" & Now.Year.ToString & "-" & Now.Month.ToString & ".txt"
			Try
				Using sw As New IO.StreamWriter(File, True)
					Dim Message As String
					For Each Message In Messages
						sw.Write(Format(Now, "dd HH:mm:ss.ff") & " ")
						sw.WriteLine(Message)
					Next
					sw.Flush()
				End Using
			Catch ex As Exception
			End Try
		End Sub

		Public Shared Function StringReplace(ByVal expr As String, ByVal find As String, ByVal repl As String, ByVal bIgnoreCase As Boolean) As String
			'// Get input string length
			Dim exprLen As Integer = expr.Length
			Dim findLen As Integer = find.Length

			'// Check inputs
			If (0 = exprLen OrElse 0 = findLen OrElse findLen > exprLen) Then Return expr

			'// Use the original method if the case is required
			If Not bIgnoreCase Then Return expr.Replace(find, repl)

			Dim sbRet As StringBuilder = New StringBuilder(exprLen)
			Dim pos As Integer = 0

			While (pos + findLen <= exprLen)
				If (0 = String.Compare(expr, pos, find, 0, findLen, bIgnoreCase)) Then
					'// Add the replaced string
					sbRet.Append(repl)
					pos += findLen
					Continue While
				End If
				'// Advance one character
				sbRet.Append(expr, pos, 1)
				pos += 1
			End While

			'// Append remaining characters
			sbRet.Append(expr, pos, exprLen - pos)

			'// Return string
			Return sbRet.ToString()
		End Function

		''' <summary>
		''' Disable postback when hitting enter
		''' </summary>
		Public Shared Sub DisablePostbackOnEnter(ByVal control As System.Web.UI.WebControls.WebControl)
			control.Attributes.Add("onkeydown", "if(event.which || event.keyCode) { if ((event.which == 13) || (event.keyCode == 13))  { return false; } } return true; ")
		End Sub

	End Class
End Namespace
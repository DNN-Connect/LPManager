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
Imports System.Xml
Imports DotNetNuke.Services.Localization
Imports System.IO

Public Class Globals

 Public Const glbCoreName As String = "Core"
 Public Const glbCoreFriendlyName As String = "DNN Core"
 Public Const glbSharedResources As String = "DesktopModules/DNNEurope/LocalizationEditor/App_LocalResources/SharedResources"

 Public Shared Function PackUrl() As String
  Return CorrectUrl("~/DesktopModules/DNNEurope/LocalizationEditor/Pack.aspx")
 End Function
 Public Shared Function CorrectUrl(ByVal url As String) As String
  url = DotNetNuke.Common.ResolveUrl(url)
  If url.StartsWith("/") Then
   url = HttpContext.Current.Request.Url.Host & url
   url = DotNetNuke.Common.AddHTTP(url)
  End If
  Return url
 End Function

 Public Shared Function CorrectHttp(ByVal url As String) As String
  If url.ToLower.StartsWith("http") Then Return url
  Return "http://" & url
 End Function

 ''' <summary>
 ''' Returns correct resource file filename based on the base filename and the requested language
 ''' </summary>
 ''' <param name="filename">The base file name</param>
 ''' <param name="language">The requested languages</param>
 ''' <returns>Correct language specific resource filename</returns>
 ''' <remarks></remarks>
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

 Public Shared Function GetCorrectPath(ByVal manifestPath As String, ByVal localResourceDirectory As String) As String
  Dim m As Match = Regex.Match(manifestPath, "(?i)GlobalResource\\(.*)\.\w\w-\w\w.resx(?-i)")
  If m.Success Then
   ' Global Resource
   Return DotNetNuke.Services.Localization.Localization.ApplicationResourceDirectory.Replace("~/", "") & "\" & m.Groups(1).Value & ".resx"
  Else
   ' Local Resource
   m = Regex.Match(manifestPath, "(?i)([^\\]+)\\(.*)\.\w\w-\w\w.resx(?-i)")
   If m.Success Then
    Dim sRes As String = ""
    Select Case m.Groups(1).Value.ToLower
     Case "localresource"
      sRes = "DesktopModules\"
     Case "controlresource"
      sRes = "controls\"
     Case "adminresource"
      sRes = "admin\"
     Case "installresource"
      sRes = "Install\"
     Case "providerresource"
      sRes = "Providers\"
     Case Else
      Return manifestPath
    End Select
    Return sRes & Regex.Replace(m.Groups(2).Value, "([^\\]+)$", localResourceDirectory & "\$1.resx")
   End If
   Return manifestPath
  End If
 End Function

 ''' <summary>
 ''' Recursively gets all resource (*.resx) files from a folder (and all child folders). Returns a <see cref="System.Collections.SortedList" />
 ''' </summary>
 ''' <param name="fileList">SortedList with all found resource files</param>
 ''' <param name="_path">Path from where to start the search</param>
 ''' <param name="locale">Locale for which to find the resource files</param>
 ''' <remarks></remarks>
 Public Shared Sub GetResourceFiles(ByRef fileList As SortedList, ByVal _path As String, ByVal locale As String)
  Dim folders As String() = Directory.GetDirectories(_path)
  Dim folder As String
  Dim objFile As FileInfo
  Dim objFolder As DirectoryInfo
  Dim pattern As String = ".resx"
  If locale <> "" Then
   pattern = "." & locale & pattern
  End If

  For Each folder In folders
   objFolder = New DirectoryInfo(folder)

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
     fileList.Add(Path.Combine(folder, Localization.LocalSharedResourceFile), New FileInfo(Path.Combine(folder, Localization.LocalSharedResourceFile)))
    End If
   Else
    GetResourceFiles(fileList, folder, locale)
   End If
  Next
 End Sub

 Public Shared Sub RemoveResourceFiles(ByRef fileList As SortedList, ByVal _path As String)
  RemoveResourceFiles(fileList, _path, "")
 End Sub

 ''' <summary>
 ''' Removes resource files from filelist
 ''' </summary>
 ''' <param name="fileList"></param>
 ''' <param name="_path"></param>
 ''' <param name="locale"></param>
 ''' <remarks></remarks>
 Public Shared Sub RemoveResourceFiles(ByRef fileList As SortedList, ByVal _path As String, ByVal locale As String)
  Dim folders As String() = Directory.GetDirectories(_path)
  Dim folder As String
  Dim objFile As FileInfo
  Dim objFolder As DirectoryInfo
  Dim pattern As String = ".resx"
  If locale <> "" Then
   pattern = "." & locale & pattern
  End If

  For Each folder In folders
   objFolder = New DirectoryInfo(folder)

   If objFolder.Name = Localization.LocalResourceDirectory Then
    ' found local resource folder, remove resources
    For Each objFile In objFolder.GetFiles("*.ascx" & pattern)
     fileList.Remove(objFile.FullName)
    Next
    For Each objFile In objFolder.GetFiles("*.aspx" & pattern)
     fileList.Remove(objFile.FullName)
    Next
    ' remove LocalSharedResources if found
    If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
     fileList.Remove(Path.Combine(folder, Localization.LocalSharedResourceFile))
    End If
   Else
    RemoveResourceFiles(fileList, folder, locale)
   End If
  Next

 End Sub

 ''' <summary>
 ''' Forces UTF8 Encoding on a file.
 ''' </summary>
 ''' <param name="Filename"></param>
 ''' <remarks></remarks>
 Public Shared Sub EnsureUTF8Encoding(ByVal Filename As String)

  Dim IsUTF8 As Boolean = False
  Dim sr As New StreamReader(Filename, True)
  Dim buffer As String = sr.ReadToEnd
  sr.Close()
  Dim sw As New StreamWriter(Filename, False, Encoding.UTF8)
  sw.Write(buffer)
  sw.Close()
  sr.Close()

 End Sub

 Public Enum EditorType
  Textbox
  DNNLabel
 End Enum

 ''' <summary>
 ''' Translates source filename into target filename (taking into account the target locale)
 ''' </summary>
 ''' <param name="sourceText">Source file name</param>
 ''' <param name="targetLocale">Target language</param>
 ''' <returns>Translated file name</returns>
 ''' <remarks></remarks>
 Public Shared Function TranslateFilenames(ByVal sourceText As String, ByVal targetLocale As String) As String
  If targetLocale.ToLower = "en-us" Then
   Return Regex.Replace(sourceText, "(\.as\wx\.)(\w\w-\w\w\.|)(resx)", "$1$3")
  Else
   Return Regex.Replace(sourceText, "(\.as\wx\.)(\w\w-\w\w\.|)(resx)", "$1" & targetLocale & ".$3")
  End If
 End Function

 ''' <summary>
 ''' Tests whether text is correct html
 ''' </summary>
 ''' <param name="text"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
 Public Shared Function IsGoodHtml(ByVal text As String) As Boolean
  Return Regex.Match(text, "<\w[^>]*>[^<]*</\w[^>]*>").Success
 End Function

 ''' <summary>
 ''' tests whether text contains html
 ''' </summary>
 ''' <param name="text"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
 Public Shared Function IsAnyHtml(ByVal text As String) As Boolean
  Return Regex.Match(text, "<(/)?\w").Success
 End Function

 ''' <summary>
 ''' converts an object into a string
 ''' </summary>
 ''' <param name="Value"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
 Public Shared Function GetAString(ByVal Value As Object) As String
  If Value Is Nothing Then
   Return ""
  Else
   If Value Is DBNull.Value Then
    Return ""
   Else
    'TODO: should use Convert.ToString
    Return CStr(Value)
   End If
  End If
 End Function

 ''' <summary>
 ''' Converts an object into a boolean
 ''' </summary>
 ''' <param name="x"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
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

 ''' <summary>
 ''' Converts an object into an integer
 ''' </summary>
 ''' <param name="Value"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
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

 ''' <summary>
 ''' Converts an object into a double
 ''' </summary>
 ''' <param name="value"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
 Public Shared Function GetADouble(ByVal value As Object) As Double

  If value Is Nothing Then
   Return 0
  Else
   If value Is DBNull.Value Then
    Return 0
   ElseIf Not IsNumeric(value) Then
    Return 0
   Else
    Return CType(value, Double)
   End If
  End If

 End Function

 ''' <summary>
 ''' Adds an xml attribute to an element
 ''' </summary>
 ''' <param name="node"></param>
 ''' <param name="propName"></param>
 ''' <param name="propValue"></param>
 ''' <remarks></remarks>
 Public Shared Sub AddAttribute(ByRef node As XmlNode, ByVal propName As String, ByVal propValue As String)
  Dim xAtt As XmlAttribute = node.OwnerDocument.CreateAttribute(propName)
  xAtt.InnerText = propValue
  node.Attributes.Append(xAtt)
 End Sub

 ''' <summary>
 ''' Adds an XML element 
 ''' </summary>
 ''' <param name="node"></param>
 ''' <param name="elementName"></param>
 ''' <param name="elementValue"></param>
 ''' <param name="attributes"></param>
 ''' <remarks></remarks>
 Public Shared Sub AddElement(ByRef node As XmlNode, ByVal elementName As String, ByVal elementValue As String, ByVal ParamArray attributes() As String)
  Dim newNode As XmlNode = node.OwnerDocument.CreateElement(elementName)
  newNode.InnerText = elementValue
  node.AppendChild(newNode)
  For Each xAttribute As String In attributes
   Dim x As String() = xAttribute.Split("="c)
   AddAttribute(newNode, x(0), x(1))
  Next
 End Sub

 ''' <summary>
 ''' Adds an XML element and returns it 
 ''' </summary>
 ''' <param name="node"></param>
 ''' <param name="elementName"></param>
 ''' <remarks></remarks>
 Public Shared Function AddElement(ByRef node As XmlNode, ByVal elementName As String) As XmlNode
  Dim newNode As XmlNode = node.OwnerDocument.CreateElement(elementName)
  Return node.AppendChild(newNode)
 End Function

 ''' <summary>
 ''' Adds a resource key/value pair to an xml file
 ''' </summary>
 ''' <param name="resourceRoot"></param>
 ''' <param name="textKey"></param>
 ''' <param name="textValue"></param>
 ''' <remarks></remarks>
 Public Shared Sub AddResourceText(ByRef resourceRoot As XmlNode, ByVal textKey As String, ByVal textValue As String)
  Dim newNode As XmlNode = resourceRoot.OwnerDocument.CreateElement("data")
  resourceRoot.AppendChild(newNode)
  AddAttribute(newNode, "name", textKey)
  AddElement(newNode, "value", textValue)
 End Sub

 ''' <summary>
 ''' logs info to a log file
 ''' </summary>
 ''' <param name="Messages"></param>
 ''' <remarks></remarks>
 Public Shared Sub SimpleLog(ByVal ParamArray Messages As String())
  'TODO: is this thread safe? what happens if multiple people are editing in the module?
  Dim LogPath As String = DotNetNuke.Common.HostMapPath & "\LocalizationEditor\Log\"
  If Not Directory.Exists(LogPath) Then
   Directory.CreateDirectory(LogPath)
  End If
  Dim File As String = LogPath & "Log-" & Now.Year.ToString & "-" & Now.Month.ToString & ".txt"
  Try
   Using sw As New StreamWriter(File, True)
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

 ''' <summary>
 ''' custom String Replace function
 ''' </summary>
 ''' <param name="expr"></param>
 ''' <param name="find"></param>
 ''' <param name="repl"></param>
 ''' <param name="bIgnoreCase"></param>
 ''' <returns></returns>
 ''' <remarks></remarks>
 Public Shared Function StringReplace(ByVal expr As String, ByVal find As String, ByVal repl As String, ByVal bIgnoreCase As Boolean) As String
  ' Get input string length
  Dim exprLen As Integer = expr.Length
  Dim findLen As Integer = find.Length

  ' Check inputs
  If (0 = exprLen OrElse 0 = findLen OrElse findLen > exprLen) Then Return expr

  ' Use the original method if the case is required
  If Not bIgnoreCase Then Return expr.Replace(find, repl)

  Dim sbRet As StringBuilder = New StringBuilder(exprLen)
  Dim pos As Integer = 0

  While (pos + findLen <= exprLen)
   If (0 = String.Compare(expr, pos, find, 0, findLen, bIgnoreCase)) Then
    ' Add the replaced string
    sbRet.Append(repl)
    pos += findLen
    Continue While
   End If
   ' Advance one character
   sbRet.Append(expr, pos, 1)
   pos += 1
  End While

  ' Append remaining characters
  sbRet.Append(expr, pos, exprLen - pos)

  ' Return string
  Return sbRet.ToString()
 End Function

 ''' <summary>
 ''' Disable postback when hitting enter
 ''' </summary>
 ''' <param name="control"></param>
 ''' <remarks></remarks>
 Public Shared Sub DisablePostbackOnEnter(ByVal control As WebControl)
  control.Attributes.Add("onkeydown", "if(event.which || event.keyCode) { if ((event.which == 13) || (event.keyCode == 13))  { return false; } } return true; ")
 End Sub

 ''' <summary>
 ''' Licenses are stored on disk
 ''' </summary>
 ''' <param name="portalHomeDirMapPath">Portal Home directory</param>
 ''' <param name="moduleID">Module ID</param>
 ''' <returns>Filename of the license</returns>
 ''' <remarks></remarks>
 Public Shared Function GetLicenseFilename(ByVal portalHomeDirMapPath As String, ByVal moduleID As Integer) As String
  Dim licDir As String = portalHomeDirMapPath & "LocalizationEditor\Licenses\"
  If Not IO.Directory.Exists(licDir) Then
   IO.Directory.CreateDirectory(licDir)
  End If
  Return licDir & moduleID.ToString & ".resources"
 End Function

 ''' <summary>
 ''' Get the contents of a license. Returns empty string if not found.
 ''' </summary>
 ''' <param name="portalHomeDirMapPath">Portal Home directory</param>
 ''' <param name="moduleID">Module ID</param>
 ''' <returns>Contents of the license</returns>
 ''' <remarks></remarks>
 Public Shared Function GetLicense(ByVal portalHomeDirMapPath As String, ByVal moduleID As Integer) As String
  Dim licFile As String = GetLicenseFilename(portalHomeDirMapPath, moduleID)
  If Not IO.File.Exists(licFile) Then
   Return ""
  End If
  Dim res As String = ""
  Using sr As New StreamReader(licFile)
   res = sr.ReadToEnd
  End Using
  Return res
 End Function

 ''' <summary>
 ''' Writes the license to disk.
 ''' </summary>
 ''' <param name="portalHomeDirMapPath">Portal Home directory</param>
 ''' <param name="moduleID">Module ID</param>
 ''' <param name="license">Contents of the license</param>
 ''' <remarks></remarks>
 Public Shared Sub WriteLicense(ByVal portalHomeDirMapPath As String, ByVal moduleID As Integer, ByVal license As String)
  Dim licFile As String = GetLicenseFilename(portalHomeDirMapPath, moduleID)
  Using sw As New StreamWriter(licFile, False)
   sw.Write(license)
   sw.Flush()
  End Using
 End Sub

 ''' <summary>
 ''' Calculate the Levenshtein distance between two strings
 ''' </summary>
 ''' <param name="a">first string</param>
 ''' <param name="b">second string</param>
 ''' <returns>integer value indicating the distance between the two strings</returns>
 ''' <remarks>Taken from http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance</remarks>
 Public Shared Function LevenshteinDistance(ByVal a As [String], ByVal b As [String]) As Int32

  If String.IsNullOrEmpty(a) Then
   If Not String.IsNullOrEmpty(b) Then
    Return b.Length
   End If
   Return 0
  End If
  If String.IsNullOrEmpty(b) Then
   If Not String.IsNullOrEmpty(a) Then
    Return a.Length
   End If
   Return 0
  End If

  Dim cost As Int32
  Dim d As Int32(,) = New Integer(a.Length, b.Length) {}
  Dim min1 As Int32
  Dim min2 As Int32
  Dim min3 As Int32

  For i As Int32 = 0 To d.GetUpperBound(0)
   d(i, 0) = i
  Next
  For i As Int32 = 0 To d.GetUpperBound(1)
   d(0, i) = i
  Next
  For i As Int32 = 1 To d.GetUpperBound(0)
   For j As Int32 = 1 To d.GetUpperBound(1)
    cost = Convert.ToInt32(Not (a(i - 1) = b(j - 1)))
    min1 = d(i - 1, j) + 1
    min2 = d(i, j - 1) + 1
    min3 = d(i - 1, j - 1) + cost
    d(i, j) = Math.Min(Math.Min(min1, min2), min3)
   Next
  Next

  Return d(d.GetUpperBound(0), d.GetUpperBound(1))

 End Function

 Public Shared Function GetAssemblyVersion(ByVal path As String) As String

  Try
   Dim v As String = System.Diagnostics.FileVersionInfo.GetVersionInfo(path).FileVersion()
   Return FormatVersion(v)
  Catch ex As Exception
   Return "0"
  End Try

 End Function

 Public Shared Function FormatVersion(ByVal version As String) As String

  Dim m As Match = Regex.Match(version, "(\d+)\.(\d+)\.(\d+)\.?(\d*)")
  If m.Success Then
   version = CInt(m.Groups(1).Value).ToString("00")
   version &= "." & CInt(m.Groups(2).Value).ToString("00")
   version &= "." & CInt(m.Groups(3).Value).ToString("00")
  End If
  Return version

 End Function

 Public Shared Function ReadFile(ByVal fileName As String) As String
  If Not IO.File.Exists(fileName) Then Return ""
  Using sr As New IO.StreamReader(fileName)
   Return sr.ReadToEnd
  End Using
 End Function

 Public Shared Sub CleanupTempDirs(ByVal parentPath As String)
  Dim pd As New IO.DirectoryInfo(parentPath)
  For Each d As IO.DirectoryInfo In pd.GetDirectories("~tmp*")
   Dim dt As String = Mid(d.Name, 5, 8)
   If dt < Now.ToString("yyyyMMdd") Then
    Try
     d.Delete(True)
    Catch ex As Exception
    End Try
   End If
  Next
 End Sub

 Public Shared Function GetFrameworkVersion() As String
  Dim res As String = "01.00.00"
  Using ir As IDataReader = Data.DataProvider.Instance.GetFrameworkVersion
   Do While ir.Read
    res = String.Format("{0:00}.{1:00}.{2:00}", ir.Item("Major"), ir.Item("Minor"), ir.Item("Build"))
    Exit Do
   Loop
  End Using
  Return res
 End Function

#Region " Value Deserialization "
 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
    Select Case ValueTable.Item(ValueName).ToLower
     Case "on", "yes"
      Variable = True
     Case Else
      Variable = False
    End Select
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Integer)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As String)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Boolean)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Date)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 Public Shared Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As TimeSpan)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub
#End Region

End Class

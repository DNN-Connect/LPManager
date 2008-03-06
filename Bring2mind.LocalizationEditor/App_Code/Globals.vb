Imports System.IO
Imports System.Xml
Imports DotNetNuke.Services.Localization

Public Class Globals

 Public Shared Function ResourceFile(ByVal filename As String, ByVal language As String) As String
  Dim resourcefilename As String = filename

  If language <> Localization.SystemLocale Then
   resourcefilename = resourcefilename.Replace(".resx", "." + language + ".resx")
  End If

  Return resourcefilename

 End Function

 Public Shared Sub GetResourceFiles(ByRef fileList As SortedList, ByVal _path As String)
  Dim folders As String() = Directory.GetDirectories(_path)
  Dim folder As String
  Dim objFile As IO.FileInfo
  Dim objFolder As DirectoryInfo

  For Each folder In folders
   objFolder = New System.IO.DirectoryInfo(folder)

   If objFolder.Name = Localization.LocalResourceDirectory Then
    ' found local resource folder, add resources
    For Each objFile In objFolder.GetFiles("*.ascx.resx")
     fileList.Add(objFile.FullName, objFile)
    Next
    For Each objFile In objFolder.GetFiles("*.aspx.resx")
     fileList.Add(objFile.FullName, objFile)
    Next
    ' add LocalSharedResources if found
    If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
     fileList.Add(Path.Combine(folder, Localization.LocalSharedResourceFile), New System.IO.FileInfo(Path.Combine(folder, Localization.LocalSharedResourceFile)))
    End If
   Else
    GetResourceFiles(fileList, folder)
   End If
  Next
 End Sub

 Public Shared Sub RemoveResourceFiles(ByRef fileList As SortedList, ByVal _path As String)
  Dim folders As String() = Directory.GetDirectories(_path)
  Dim folder As String
  Dim objFile As IO.FileInfo
  Dim objFolder As DirectoryInfo

  For Each folder In folders
   objFolder = New System.IO.DirectoryInfo(folder)

   If objFolder.Name = Localization.LocalResourceDirectory Then
    ' found local resource folder, add resources
    For Each objFile In objFolder.GetFiles("*.ascx.resx")
     fileList.Remove(objFile.FullName)
    Next
    For Each objFile In objFolder.GetFiles("*.aspx.resx")
     fileList.Remove(objFile.FullName)
    Next
    ' add LocalSharedResources if found
    If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
     fileList.Remove(Path.Combine(folder, Localization.LocalSharedResourceFile))
    End If
   Else
    RemoveResourceFiles(fileList, folder)
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
End Class

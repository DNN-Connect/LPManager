Public Class ZipHelper
	Public Shared Sub Unzip(ByVal fileStream As IO.Stream, ByVal tempDirectory As String)
		Dim objZipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry
		Using objZipInputStream As New ICSharpCode.SharpZipLib.Zip.ZipInputStream(fileStream)
			objZipEntry = objZipInputStream.GetNextEntry
			While Not objZipEntry Is Nothing
				Dim strFileName As String = objZipEntry.Name.Replace("/", "\")
				If strFileName <> "" And Not objZipEntry.IsDirectory Then
					Dim sFile As String = strFileName
					Dim sPath As String = tempDirectory & "\"
					If strFileName.IndexOf("\"c) > 0 Then
						sFile = Mid(strFileName, strFileName.LastIndexOf("\"c) + 2)
						sPath = sPath & Left(strFileName, strFileName.LastIndexOf("\"c))
						If Not IO.Directory.Exists(sPath) Then
							IO.Directory.CreateDirectory(sPath)
						End If
						sPath &= "\"
					End If
					Using objFileStream As IO.FileStream = IO.File.Create(sPath & sFile)
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
End Class

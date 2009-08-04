'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System
Imports System.Collections

Namespace DNNEurope.Modules.LocalizationEditor.LocaleFile
	''' <summary>
	''' Represents a collection of <see cref="LocaleFileInfo">LocaleFileInfo</see> objects.
	''' </summary>
	Public Class LocaleFileCollection
		Inherits CollectionBase

		''' <summary>
		''' Initializes a new instance of the <see cref="LocaleFileCollection">LocaleFileCollection</see> class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="LocaleFileCollection">LocaleFileCollection</see> class containing the elements of the specified source collection.
		''' </summary>
		''' <param name="value">A <see cref="LocaleFileCollection">LocaleFileCollection</see> with which to initialize the collection.</param>
		Public Sub New(ByVal value As LocaleFileCollection)
			MyBase.New()
			AddRange(value)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="LocaleFileCollection">LocaleFileCollection</see> class containing the specified array of <see cref="LocaleFileInfo">LocaleFileInfo</see> objects.
		''' </summary>
		''' <param name="value">An array of <see cref="LocaleFileInfo">LocaleFileInfo</see> objects with which to initialize the collection. </param>
		Public Sub New(ByVal value As LocaleFileInfo())
			MyBase.New()
			AddRange(value)
		End Sub

		''' <summary>
		''' Gets the <see cref="LocaleFileCollection">LocaleFileCollection</see> at the specified index in the collection.
		''' <para>
		''' In VB.Net, this property is the indexer for the <see cref="LocaleFileCollection">LocaleFileCollection</see> class.
		''' </para>
		''' </summary>
		Default Public Property Item(ByVal index As Integer) As LocaleFileInfo
			Get
				Return CType(List(index), LocaleFileInfo)
			End Get
			Set(ByVal Value As LocaleFileInfo)
				List(index) = Value
			End Set
		End Property

		Public Property LocaleFile(ByVal FullFileName As String) As LocaleFileInfo
			Get
				Return GetLocaleFile(FullFileName)
			End Get
			Set(ByVal Value As LocaleFileInfo)
				Dim LocaleFile As LocaleFileInfo = GetLocaleFile(FullFileName)
				If Not LocaleFile Is Nothing Then
					LocaleFile = Value
				Else
					Throw New ArgumentOutOfRangeException(FullFileName, FullFileName & " does not exist in this collection.")
				End If
			End Set
		End Property

		''' <summary>
		''' Add an element of the specified <see cref="LocaleFileInfo">LocaleFileInfo</see> to the end of the collection.
		''' </summary>
		''' <param name="value">An object of type <see cref="LocaleFileInfo">LocaleFileInfo</see> to add to the collection.</param>
		Public Function Add(ByVal value As LocaleFileInfo) As Integer
			Return List.Add(value)
		End Function		  'Add

		''' <summary>
		''' Gets the index in the collection of the specified <see cref="LocaleFileCollection">LocaleFileCollection</see>, if it exists in the collection.
		''' </summary>
		''' <param name="value">The <see cref="LocaleFileCollection">LocaleFileCollection</see> to locate in the collection.</param>
		''' <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
		Public Function IndexOf(ByVal value As LocaleFileInfo) As Integer
			Return List.IndexOf(value)
		End Function		  'IndexOf

		''' <summary>
		''' Add an element of the specified <see cref="LocaleFileInfo">LocaleFileInfo</see> to the collection at the designated index.
		''' </summary>
		''' <param name="index">An <see cref="system.int32">Integer</see> to indicate the location to add the object to the collection.</param>
		''' <param name="value">An object of type <see cref="LocaleFileInfo">LocaleFileInfo</see> to add to the collection.</param>
		Public Sub Insert(ByVal index As Integer, ByVal value As LocaleFileInfo)
			List.Insert(index, value)
		End Sub		  'Insert

		''' <summary>
		''' Remove the specified object of type <see cref="LocaleFileInfo">LocaleFileInfo</see> from the collection.
		''' </summary>
		''' <param name="value">An object of type <see cref="LocaleFileInfo">LocaleFileInfo</see> to remove to the collection.</param>
		Public Sub Remove(ByVal value As LocaleFileInfo)
			List.Remove(value)
		End Sub		  'Remove

		''' <summary>
		''' Gets a value indicating whether the collection contains the specified <see cref="LocaleFileCollection">LocaleFileCollection</see>.
		''' </summary>
		''' <param name="value">The <see cref="LocaleFileCollection">LocaleFileCollection</see> to search for in the collection.</param>
		''' <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
		Public Function Contains(ByVal value As LocaleFileInfo) As Boolean
			' If value is not of type LocaleFileInfo, this will return false.
			Return List.Contains(value)
		End Function		  'Contains

		''' <summary>
		''' Copies the elements of the specified <see cref="LocaleFileInfo">LocaleFileInfo</see> array to the end of the collection.
		''' </summary>
		''' <param name="value">An array of type <see cref="LocaleFileInfo">LocaleFileInfo</see> containing the objects to add to the collection.</param>
		Public Sub AddRange(ByVal value As LocaleFileInfo())
			For i As Integer = 0 To value.Length - 1
				Add(value(i))
			Next i
		End Sub

		''' <summary>
		''' Adds the contents of another <see cref="LocaleFileCollection">LocaleFileCollection</see> to the end of the collection.
		''' </summary>
		''' <param name="value">A <see cref="LocaleFileCollection">LocaleFileCollection</see> containing the objects to add to the collection. </param>
		Public Sub AddRange(ByVal value As LocaleFileCollection)
			For i As Integer = 0 To value.Count - 1
				Add(CType(value.List(i), LocaleFileInfo))
			Next i
		End Sub

		''' <summary>
		''' Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
		''' </summary>
		''' <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
		''' <param name="index">The index of the array at which to begin inserting.</param>
		Public Sub CopyTo(ByVal array As LocaleFileInfo(), ByVal index As Integer)
			List.CopyTo(array, index)
		End Sub

		''' <summary>
		''' Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
		''' </summary>
		''' <returns>Array of type LocaleFileInfo</returns>
		Public Function ToArray() As LocaleFileInfo()
			Dim arr As LocaleFileInfo()
			ReDim Preserve arr(Count - 1)
			CopyTo(arr, 0)

			Return arr
		End Function

		Private Function GetLocaleFile(ByVal FullFileName As String) As LocaleFileInfo
			For Each LocaleFile As LocaleFileInfo In List
				If LocaleFileUtil.GetFullFileName(LocaleFile).Replace("\", "/") = FullFileName.Replace("\", "/") Then
					Return LocaleFile
				End If
			Next

			Return Nothing
		End Function

	End Class
End Namespace


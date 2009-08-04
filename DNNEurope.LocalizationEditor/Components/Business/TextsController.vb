Imports DotNetNuke.Common.Utilities
Imports System.Collections.Generic

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " TextsController "
	Public Class TextsController

		Public Shared Function GetText(ByVal TextId As Integer) As TextInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetText(TextId), GetType(TextInfo)), TextInfo)
        End Function

		Public Shared Function AddText(ByVal objText As TextInfo) As Integer
            Return CType(Data.DataProvider.Instance().AddText(objText.DeprecatedIn, objText.FilePath, objText.ObjectId, objText.OriginalValue, objText.TextKey, objText.Version), Integer)
        End Function

        Public Shared Sub UpdateText(ByVal objText As TextInfo)
            Data.DataProvider.Instance().UpdateText(objText.TextId, objText.DeprecatedIn, objText.FilePath, objText.ObjectId, objText.OriginalValue, objText.TextKey, objText.Version)
        End Sub

        Public Shared Sub DeleteText(ByVal TextId As Integer)
            Data.DataProvider.Instance().DeleteText(TextId)
        End Sub

        Public Shared Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As TextInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetLatestText(ObjectId, FilePath, Locale, TextKey), GetType(TextInfo)), TextInfo)
        End Function

        Public Shared Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As TextInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetText(ObjectId, FilePath, Locale, Version, TextKey), GetType(TextInfo)), TextInfo)
        End Function

        Public Shared Function GetTextsByObject(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDictionary(Of Integer, TextInfo)
            Return CBO.FillDictionary(Of TextInfo)(Data.DataProvider.Instance.GetTextsByObject(ObjectId, Locale, Version))
        End Function

        Public Shared Function GetTextsByObjectAndFile(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDictionary(Of Integer, TextInfo)
            Return CBO.FillDictionary(Of TextInfo)(Data.DataProvider.Instance.GetTextsByObjectAndFile(ObjectId, FilePath, Locale, Version, IncludeNonTranslated))
        End Function

        Public Shared Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As String
            Using ir As IDataReader = Data.DataProvider.Instance.CurrentVersion(ObjectId, Locale)
                If ir.Read Then
                    Return Globals.GetAString(ir.Item(0))
                End If
            End Using
            Return "00.00.00"
        End Function

        Public Shared Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As Integer
            Using ir As IDataReader = Data.DataProvider.Instance.NrOfChangedTexts(ObjectId, Version)
                If ir.Read Then
                    Return Globals.GetAnInteger(ir.Item(0))
                End If
            End Using
            Return -1
        End Function

        Public Shared Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As Integer
            Using ir As IDataReader = Data.DataProvider.Instance.NrOfFiles(ObjectId, Version)
                If ir.Read Then
                    Return Globals.GetAnInteger(ir.Item(0))
                End If
            End Using
            Return -1
        End Function

        Public Shared Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As Integer
            Using ir As IDataReader = Data.DataProvider.Instance.NrOfItems(ObjectId, Version)
                If ir.Read Then
                    Return Globals.GetAnInteger(ir.Item(0))
                End If
            End Using
            Return -1
        End Function

        Public Shared Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As Integer
            Using ir As IDataReader = Data.DataProvider.Instance.NrOfMissingTranslations(ObjectId, Locale, Version)
                If ir.Read Then
                    Return Globals.GetAnInteger(ir.Item(0))
                End If
            End Using
            Return -1
        End Function

        Public Shared Function GetVersions(ByVal ObjectId As Integer) As List(Of String)
            Dim res As New List(Of String)
            Using ir As IDataReader = Data.DataProvider.Instance.GetVersions(ObjectId)
                Do While ir.Read
                    res.Add(Globals.GetAString(ir.Item(0)))
                Loop
            End Using
            Return res
        End Function

        Public Shared Function GetFileList(ByVal ObjectId As Integer, ByVal Version As String) As List(Of String)
            Dim res As New List(Of String)
            Using ir As IDataReader = Data.DataProvider.Instance.GetFiles(ObjectId, Version)
                Do While ir.Read
                    res.Add(Globals.GetAString(ir.Item(0)))
                Loop
            End Using
            Return res
        End Function
	End Class
#End Region

End Namespace

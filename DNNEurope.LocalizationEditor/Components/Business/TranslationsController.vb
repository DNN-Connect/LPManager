Imports DotNetNuke.Common.Utilities

Imports DNNEurope.Modules.LocalizationEditor.Data

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " TranslationsController "
	Public Class TranslationsController

		Public Shared Function GetTranslation(ByVal TranslationId As Integer) As TranslationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetTranslation(TranslationId), GetType(TranslationInfo)), TranslationInfo)
        End Function

		Public Shared Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As TranslationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetTranslation(TextId, Locale), GetType(TranslationInfo)), TranslationInfo)
        End Function

		Public Shared Sub AddTranslation(ByVal objTranslation As TranslationInfo)
            Try
                DataProvider.Instance().AddTranslation(objTranslation.TextId, objTranslation.Locale, objTranslation.LastModified, objTranslation.LastModifiedUserId, objTranslation.TextValue)
            Catch ex As Exception
            End Try
        End Sub

		Public Shared Sub UpdateTranslation(ByVal objTranslation As TranslationInfo)
            DataProvider.Instance().UpdateTranslation(objTranslation.TranslationId, objTranslation.TextId, objTranslation.Locale, objTranslation.LastModified, objTranslation.LastModifiedUserId, objTranslation.TextValue)
        End Sub

		Public Shared Sub DeleteTranslation(ByVal TranslationId As Integer)
            DataProvider.Instance().DeleteTranslation(TranslationId)
        End Sub

		Public Shared Function GetTranslationsByText(ByVal TextId As Integer) As ArrayList
            Return DotNetNuke.Common.Utilities.CBO.FillCollection(DataProvider.Instance().GetTranslationsByText(TextId), GetType(TranslationInfo))
        End Function

	End Class
#End Region

End Namespace

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
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Common.Utilities

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " TranslationsController "

    Public Class TranslationsController
        Public Shared Function GetTranslation(ByVal TranslationId As Integer) As TranslationInfo
            Return _
                CType( _
                    CBO.FillObject(DataProvider.Instance().GetTranslation(TranslationId), GetType(TranslationInfo)),  _
                    TranslationInfo)
        End Function

        Public Shared Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As TranslationInfo
            Return _
                CType( _
                    CBO.FillObject(DataProvider.Instance().GetTranslation(TextId, Locale), GetType(TranslationInfo)),  _
                    TranslationInfo)
        End Function

        Public Shared Sub AddTranslation(ByVal objTranslation As TranslationInfo)
            Try
                DataProvider.Instance().AddTranslation(objTranslation.TextId, objTranslation.Locale, _
                                                        objTranslation.LastModified, objTranslation.LastModifiedUserId, _
                                                        objTranslation.TextValue)
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Sub UpdateTranslation(ByVal objTranslation As TranslationInfo)
            DataProvider.Instance().UpdateTranslation(objTranslation.TranslationId, objTranslation.TextId, _
                                                       objTranslation.Locale, objTranslation.LastModified, _
                                                       objTranslation.LastModifiedUserId, objTranslation.TextValue)
        End Sub

        Public Shared Sub DeleteTranslation(ByVal TranslationId As Integer)
            DataProvider.Instance().DeleteTranslation(TranslationId)
        End Sub

        Public Shared Function GetTranslationsByText(ByVal TextId As Integer) As ArrayList
            Return _
                CBO.FillCollection(DataProvider.Instance().GetTranslationsByText(TextId), GetType(TranslationInfo))
        End Function
    End Class

#End Region
End Namespace

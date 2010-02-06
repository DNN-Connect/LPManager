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
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Common.Utilities
Imports System.Collections.Generic

Namespace Business

 Public Structure Statistic
  Public UserName As String
  Public TotalScore As Integer
 End Structure

 Public Class StatisticsController

  Public Shared Sub RecordStatistic(ByVal UserId As Integer, ByVal TranslationId As Integer, ByVal Statistic As Integer)

   Dim st As StatisticInfo = StatisticsController.GetStatistic(UserId, TranslationId)
   If st Is Nothing Then
    st = New StatisticInfo
    st.UserId = UserId
    st.TranslationId = TranslationId
    st.Total = Statistic
    StatisticsController.AddStatistic(st)
   Else
    st.Total += Statistic
    StatisticsController.UpdateStatistic(st)
   End If

  End Sub

  Public Shared Function GetPackStatistics(ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As List(Of Statistic)

   Dim res As New List(Of Statistic)
   Using ir As IDataReader = Data.DataProvider.Instance().GetPackStatistics(ObjectId, Version, Locale)
    Do While ir.Read
     Dim s As New Statistic
     s.UserName = CStr(ir.Item("UserName"))
     s.TotalScore = CInt(ir.Item("Total"))
     res.Add(s)
    Loop
   End Using
   Return res

  End Function

  Public Shared Function GetStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32) As StatisticInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetStatistic(UserId, TranslationId), GetType(StatisticInfo)), StatisticInfo)

  End Function

  Public Shared Sub AddStatistic(ByVal objStatistic As StatisticInfo)

   DataProvider.Instance().AddStatistic(objStatistic.UserId, objStatistic.TranslationId, objStatistic.Total)

  End Sub

  Public Shared Sub UpdateStatistic(ByVal objStatistic As StatisticInfo)

   DataProvider.Instance().UpdateStatistic(objStatistic.UserId, objStatistic.TranslationId, objStatistic.Total)

  End Sub

  Public Shared Sub DeleteStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32)

   DataProvider.Instance().DeleteStatistic(UserId, TranslationId)

  End Sub

  Public Shared Function GetStatisticsByTranslation(ByVal TranslationId As Int32) As ArrayList

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(DataProvider.Instance().GetStatisticsByTranslation(TranslationId), GetType(StatisticInfo))

  End Function

  Public Shared Function GetStatisticsByUser(ByVal UserID As Int32) As ArrayList

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(DataProvider.Instance().GetStatisticsByUser(UserID), GetType(StatisticInfo))

  End Function

 End Class
End Namespace


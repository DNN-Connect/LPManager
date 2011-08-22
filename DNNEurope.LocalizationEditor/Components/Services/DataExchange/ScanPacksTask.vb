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
Imports DotNetNuke.Services.Exceptions
Imports DNNEurope.Modules.LocalizationEditor.Entities.Partners

Namespace Services.DataExchange
 Public Class ScanPartnerPacksTask
  Inherits DotNetNuke.Services.Scheduling.SchedulerClient

  Private Log As New StringBuilder

  Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
   MyBase.new()
   Me.ScheduleHistoryItem = objScheduleHistoryItem
  End Sub

  Public Overrides Sub DoWork()

   Try

    Me.Progressing() ' Start

    For Each Partner As PartnerInfo In PartnersController.GetAllPartners

     Dim fImport As New FeedImporter(Partner)
     fImport.Log = Log
     fImport.ImportFeed()

    Next

    Me.ScheduleHistoryItem.AddLogNote(Log.ToString.Replace(vbCrLf, "<br />"))
    Me.ScheduleHistoryItem.Succeeded = True

   Catch ex As Exception
    Me.ScheduleHistoryItem.AddLogNote(String.Format("{0}<br />Scanning PartnerPacks failed: {1}<br />{2}", Log.ToString.Replace(vbCrLf, "<br />"), ex.Message, ex.StackTrace))
    Me.ScheduleHistoryItem.Succeeded = False
    Me.Errored(ex)
    LogException(ex)
   End Try


  End Sub

 End Class
End Namespace

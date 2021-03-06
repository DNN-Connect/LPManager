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

Namespace Services.Logging
 Public Class Logger
  Private m_Logs As ArrayList
  Private m_Valid As Boolean
  Private _NormalClass As String
  Private _HighlightClass As String
  Private _ErrorClass As String

  Public Sub New()
   m_Logs = New ArrayList
   m_Valid = True
  End Sub

  Public Sub AddInfo(ByVal info As String)
   m_Logs.Add(New LogEntry(LogType.Info, info))
  End Sub

  Public Sub AddWarning(ByVal warning As String)
   m_Logs.Add(New LogEntry(LogType.Warning, warning))
  End Sub

  Public Sub AddFailure(ByVal failure As String)
   m_Logs.Add(New LogEntry(LogType.Failure, failure))
   m_Valid = False
  End Sub

  Public Sub Add(ByVal ex As Exception)
   AddFailure(("EXCEPTION" + ex.ToString()))
  End Sub

  Public Sub StartJob(ByVal job As String)
   m_Logs.Add(New LogEntry(LogType.StartJob, job))
  End Sub

  Public Sub EndJob(ByVal job As String)
   m_Logs.Add(New LogEntry(LogType.EndJob, job))
  End Sub

  Public ReadOnly Property Logs() As ArrayList
   Get
    Return m_Logs
   End Get
  End Property

  Public ReadOnly Property Valid() As Boolean
   Get
    Return m_Valid
   End Get
  End Property

  Public Property ErrorClass() As String
   Get
    If _ErrorClass = "" Then
     _ErrorClass = "NormalRed"
    End If
    Return _ErrorClass
   End Get
   Set(ByVal Value As String)
    _ErrorClass = Value
   End Set
  End Property

  Public Property HighlightClass() As String
   Get
    If _HighlightClass = "" Then
     _HighlightClass = "NormalBold"
    End If
    Return _HighlightClass
   End Get
   Set(ByVal Value As String)
    _HighlightClass = Value
   End Set
  End Property

  Public Property NormalClass() As String
   Get
    If _NormalClass = "" Then
     _NormalClass = "Normal"
    End If
    Return _NormalClass
   End Get
   Set(ByVal Value As String)
    _NormalClass = Value
   End Set
  End Property

  Public Function GetLogsTable() As HtmlTable
   Dim arrayTable As New HtmlTable

   Dim LogEntry As LogEntry

   For Each LogEntry In Logs
    Dim tr As New HtmlTableRow
    Dim tdType As New HtmlTableCell
    tdType.InnerText = DotNetNuke.Services.Localization.Localization.GetString("LOG.PALogger." & LogEntry.Type.ToString)
    Dim tdDescription As New HtmlTableCell
    tdDescription.InnerText = LogEntry.Description
    tr.Cells.Add(tdType)
    tr.Cells.Add(tdDescription)
    Select Case LogEntry.Type
     Case LogType.Failure, LogType.Warning
      tr.Attributes.Add("class", ErrorClass)
     Case LogType.StartJob, LogType.EndJob
      tr.Attributes.Add("class", HighlightClass)
     Case LogType.Info
      tr.Attributes.Add("class", NormalClass)
    End Select
    arrayTable.Rows.Add(tr)
    If LogEntry.Type = LogType.EndJob Then
     Dim SpaceTR As New HtmlTableRow
     Dim SpaceTD As New HtmlTableCell
     SpaceTD.ColSpan = 2
     SpaceTD.InnerHtml = "&nbsp;"
     SpaceTR.Cells.Add(SpaceTD)
     arrayTable.Rows.Add(SpaceTR)
    End If
   Next

   Return arrayTable
  End Function
 End Class
End Namespace

Imports DotNetNuke.Services.Localization.Localization

Namespace DNNEurope.Modules.LocalizationEditor
	Public Class LogEntry
		Private m_Type As LogType
		Private m_Description As [String]

		Public Sub New(ByVal type As LogType, ByVal description As [String])
			m_Type = type
			m_Description = description
		End Sub	   'New

		Public ReadOnly Property Type() As LogType
			Get
				Return m_Type
			End Get
		End Property

		Public ReadOnly Property Description() As [String]
			Get
				If m_Description Is Nothing Then
					Return "..."
				Else
					Return m_Description
				End If
			End Get
		End Property

	End Class


	Public Class Logger
		Private m_Logs As ArrayList
		Private m_Valid As Boolean
		Private _NormalClass As String
		Private _HighlightClass As String
		Private _ErrorClass As String

		Public Sub New()
			m_Logs = New ArrayList
			m_Valid = True
		End Sub	   'New

		Public Sub AddInfo(ByVal info As String)
			m_Logs.Add(New LogEntry(LogType.Info, info))
		End Sub	   'AddInfo

		Public Sub AddWarning(ByVal warning As String)
			m_Logs.Add(New LogEntry(LogType.Warning, warning))
		End Sub	   'AddWarning

		Public Sub AddFailure(ByVal failure As String)
			m_Logs.Add(New LogEntry(LogType.Failure, failure))
			m_Valid = False
		End Sub	   'AddFailure

		Public Sub Add(ByVal ex As Exception)
			AddFailure(("EXCEPTION" + ex.ToString()))
		End Sub	   'Add

		Public Sub StartJob(ByVal job As String)
			m_Logs.Add(New LogEntry(LogType.StartJob, job))
		End Sub	   'StartJob

		Public Sub EndJob(ByVal job As String)
			m_Logs.Add(New LogEntry(LogType.EndJob, job))
		End Sub	   'EndJob

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
				tdType.InnerText = GetString("LOG.PALogger." & LogEntry.Type.ToString)
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

	Public Enum LogType
		Info
		Warning
		Failure
		StartJob
		EndJob
	End Enum 'LogType
End Namespace
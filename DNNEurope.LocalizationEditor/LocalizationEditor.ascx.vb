Imports System

Imports DotNetNuke.Services.Exceptions

Namespace DNNEurope.Modules.LocalizationEditor
	Partial Public Class LocalizationEditor
		Inherits ModuleBase

#Region " Private Members "
		Private _desktopModules As Hashtable
		Private _userId As Integer = UserId
#End Region

#Region " Event Handlers "
		Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
			If UserInfo.IsSuperUser Then
				_userId = PortalSettings.AdministratorId
			End If
		End Sub

		Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				'// Show functions for authorized users
				lbManagePermissions.Visible = Me.HasModulePermission("ManagePermissions")
                lbManageObjects.Visible = Me.HasModulePermission("ManageObjects")

				If Not Me.IsPostBack Then
					Me.DataBind()
				End If

			Catch exc As Exception
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub lbManagePermissions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lbManagePermissions.Click
			Response.Redirect(EditUrl("Users"))
		End Sub

        Protected Sub lbManageObjects_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lbManageObjects.Click
            Response.Redirect(EditUrl("ManageObjects"))
        End Sub
#End Region

#Region " Public Methods "
        Public Function GetObjectLocales(ByVal ObjectId As Integer) As String
            Dim uid As Integer = UserId
            If UserInfo.IsSuperUser Then
                uid = PortalSettings.AdministratorId
            End If
            Dim res As New StringBuilder
            Using ir As IDataReader = Data.DataProvider.Instance.GetLocalesForUserObject(ObjectId, uid, PortalId, ModuleId)
                Do While ir.Read
                    res.Append("<a href=""")
                    res.Append(EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", "Locale=" & CStr(ir.Item("Locale"))))
                    res.Append(""" class=""CommandButton"">")
                    res.Append(CStr(ir.Item("Locale")))
                    res.AppendFormat("</a> ({0}%) | ", ir.Item("Progress"))
                Loop
            End Using
            Return res.ToString.Trim.TrimEnd(CChar("|"))
        End Function

        Public Function GetObjectURL(ByVal ObjectId As Integer) As String
            Return EditUrl("ObjectId", ObjectId.ToString, "DownloadPack")
        End Function
#End Region

#Region " Overrides "
		Public Overrides Sub DataBind()

			dlObjects.DataSource = Data.DataProvider.Instance.GetObjectsForUser(_userId, PortalId, ModuleId)
			dlObjects.DataBind()
			If dlObjects.Items.Count = 0 Then
				pnlEdit.Visible = False
			End If

		End Sub
#End Region

	End Class
End Namespace
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
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Exceptions

Namespace DNNEurope.Modules.LocalizationEditor
    Partial Public Class LocalizationEditor
        Inherits ModuleBase

#Region " Private Members "

        Private _desktopModules As Hashtable
        Private _userId As Integer = UserId

#End Region

#Region " Event Handlers "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            If UserInfo.IsSuperUser Then
                _userId = PortalSettings.AdministratorId
            End If
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                '// Show functions for authorized users
                lbManagePermissions.Visible = _
                    ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, _
                                                                    "ManagePermissions")
                lbManageObjects.Visible = _
                    ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, _
                                                                    "ManageObjects")

                If Not Me.IsPostBack Then
                    Me.DataBind()
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub lbManagePermissions_Click(ByVal sender As Object, ByVal e As EventArgs) _
            Handles lbManagePermissions.Click
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
            Using ir As IDataReader = DataProvider.Instance.GetLocalesForUserObject(ObjectId, uid, PortalId, ModuleId)
                Do While ir.Read
                    res.Append("<a href=""")
                    res.Append( _
                                EditUrl("ObjectId", ObjectId.ToString, "ObjectSummary", _
                                         "Locale=" & CStr(ir.Item("Locale"))))
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

            dlObjects.DataSource = DataProvider.Instance.GetObjectsForUser(_userId, PortalId, ModuleId)
            dlObjects.DataBind()
            If dlObjects.Items.Count = 0 Then
                pnlEdit.Visible = False
            End If

        End Sub

#End Region
    End Class
End Namespace
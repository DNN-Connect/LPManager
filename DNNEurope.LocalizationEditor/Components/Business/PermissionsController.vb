Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " PermissionsController "
	Public Class PermissionsController

        Public Shared Function GetPermission(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As PermissionInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetPermission(ObjectId, UserId, Locale, ModuleId), GetType(PermissionInfo)), PermissionInfo)
        End Function

        Public Shared Function GetPermissions(ByVal ModuleId As Integer) As ArrayList
            Return CBO.FillCollection(Data.DataProvider.Instance().GetPermissions(ModuleId), GetType(PermissionInfo))
        End Function

        'Public Shared Function GetPermissionsByUserObject(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As ArrayList
        ' Return CBO.FillCollection(Data.DataProvider.Instance().GetPermissionsByUserObject(ObjectId, UserId, PortalId, ModuleId), GetType(PermissionInfo))
        'End Function

        Public Shared Sub AddPermission(ByVal objPermission As PermissionInfo)
            Data.DataProvider.Instance().AddPermission(objPermission.ObjectId, objPermission.UserId, objPermission.Locale, objPermission.ModuleId)
        End Sub

        Public Shared Sub UpdatePermission(ByVal objPermission As PermissionInfo)
            Data.DataProvider.Instance().UpdatePermission(objPermission.PermissionId, objPermission.ObjectId, objPermission.UserId, objPermission.Locale, objPermission.ModuleId)
        End Sub

        Public Shared Sub DeletePermission(ByVal PermissionId As Integer)
            Data.DataProvider.Instance().DeletePermission(PermissionId)
        End Sub

        Public Shared Function HasAccess(ByVal user As UserInfo, ByVal AdminRole As String, ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String) As Boolean
            If user.IsSuperUser Or user.IsInRole(AdminRole) Then Return True
            Using ir As IDataReader = Data.DataProvider.Instance().GetPermission(ObjectId, user.UserID, Locale, ModuleId)
                If ir.Read Then
                    Return True
                End If
            End Using
            Return False
        End Function

	End Class
#End Region

End Namespace

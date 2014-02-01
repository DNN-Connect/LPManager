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
Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Common.Utilities

Namespace Entities.Permissions

 Public Class PermissionsController

  Public Shared Function GetPermission(ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As PermissionInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetPermission(UserId, Locale, ModuleId), GetType(PermissionInfo)), PermissionInfo)
  End Function

  Public Shared Function GetPermissions(ByVal ModuleId As Integer) As List(Of PermissionInfo)
   Return GetPermissions(ModuleId, False)
  End Function

  Public Shared Function GetPermissions(ByVal ModuleId As Integer, refreshCache As Boolean) As List(Of PermissionInfo)
   Dim cacheKey As String = "LEPermissions" & ModuleId.ToString
   Dim res As List(Of PermissionInfo) = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(cacheKey), List(Of PermissionInfo))
   If res Is Nothing Or refreshCache Then
    res = CBO.FillCollection(Of PermissionInfo)(DataProvider.Instance().GetPermissions(ModuleId))
    DotNetNuke.Common.Utilities.DataCache.SetCache(cacheKey, res)
   End If
   Return res
  End Function

  Public Shared Function GetPermissionById(permissionId As Integer) As PermissionInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetPermissionById(permissionId), GetType(PermissionInfo)), PermissionInfo)
  End Function

  Public Shared Function HasAccess(ByVal user As UserInfo, ByVal AdminRole As String, ByVal ModuleId As Integer, ByVal Locale As String) As Boolean
   If user.IsSuperUser Or user.IsInRole(AdminRole) Then Return True
   Using ir As IDataReader = DataProvider.Instance().GetPermission(user.UserID, Locale, ModuleId)
    If ir.Read Then
     Return True
    End If
   End Using
   Return False
  End Function

  Public Shared Function GetUsersFiltered(ByVal filter As String) As DataSet
   Return DataProvider.Instance().GetUsersFiltered(filter)
  End Function
 End Class

End Namespace

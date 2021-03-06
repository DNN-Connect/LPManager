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
Imports System
Imports DNNEurope.Modules.LocalizationEditor.Data

Namespace Entities.Permissions

 Partial Public Class PermissionsController

  Public Shared Function AddPermission(ByVal objPermission As PermissionInfo) As Integer

   DotNetNuke.Common.Utilities.DataCache.SetCache("LEPermissions" & objPermission.ModuleId.ToString, Nothing)
   Dim newPermissionId As Integer = CType(DataProvider.Instance().AddPermission(objPermission.ModuleId, objPermission.Locale, objPermission.UserId), Integer)
   GetPermissions(objPermission.ModuleId, True)
   Return newPermissionId

  End Function

  Public Shared Sub DeletePermission(ByVal PermissionId As Int32)

   Dim p As PermissionInfo = GetPermissionById(PermissionId)
   If p IsNot Nothing Then GetPermissions(p.ModuleId, True)
   DataProvider.Instance().DeletePermission(PermissionId)

  End Sub

 End Class
End Namespace


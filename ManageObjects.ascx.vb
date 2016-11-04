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
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Common
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports DNNEurope.Modules.LocalizationEditor.Services.Packaging

Partial Public Class ManageObjects
 Inherits ModuleBase

#Region " Properties "
 Public Property CanDelete As Boolean = False
#End Region

#Region " Event Handlers "
 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
  If Not (IsAdmin Or ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, Globals.glbObjectMgrPermission)) Then
   Response.Redirect(AccessDeniedURL())
  End If
  CanDelete = Settings.ManagersCanDelete Or IsAdmin
  ' Force full postback when using upload control
  AJAX.RegisterPostBackControl(lbImportPackage)

  ' Check to see if there are any packages ready to import
  Dim importPath As String = PortalSettings.HomeDirectoryMapPath & "LocalizationEditor\ImportModule"
  If Not IO.Directory.Exists(importPath) Then
   IO.Directory.CreateDirectory(importPath)
  End If

  For Each zipFile As String In IO.Directory.GetFiles(importPath, "*.zip")
   Using fs As New IO.FileStream(zipFile, IO.FileMode.Open, IO.FileAccess.Read)
    PackageReader.ImportModulePackage(fs, PortalSettings.HomeDirectoryMapPath, ModuleId, Nothing)
   End Using
   IO.File.Delete(zipFile)
  Next

  If Not Me.IsPostBack Then
   BindData()
  End If
 End Sub

 ''' <summary>
 ''' Import a module using a module install package into the localization editor
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Private Sub lbImportPackage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lbImportPackage.Click
  ' Check if a file is given
  If Not ctlUpload.HasFile Then
   lblUploadError.Text = Localization.GetString("NoFile", LocalResourceFile)
   Return
  End If

   PackageReader.ImportModulePackage(ctlUpload.FileContent, PortalSettings.HomeDirectoryMapPath, ModuleId, Nothing)

  ' Reload data
  BindData()
 End Sub

 Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReturn.Click
  DotNetNuke.Common.Utilities.DataCache.RemoveCache(String.Format("LocList{0}", ModuleId))
  Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
 End Sub

 Private Sub dlTranslateObjects_DeleteCommand(ByVal source As Object, ByVal e As DataListCommandEventArgs) Handles dlTranslateObjects.DeleteCommand
  If CanDelete Then
   Dim ObjectId As Integer = CInt(dlTranslateObjects.DataKeys(e.Item.ItemIndex))
   ObjectsController.DeleteObject(ObjectId)
  End If
  BindData()
 End Sub
#End Region

#Region " Private Methods "
 Private Sub BindData()
  ' Load all imported modules
  Dim translatedModules As List(Of ObjectInfo) = ObjectsController.GetObjects(ModuleId)
  dlTranslateObjects.DataSource = translatedModules
  dlTranslateObjects.DataBind()
 End Sub
#End Region

End Class

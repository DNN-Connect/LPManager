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
Imports DNNEurope.Modules.LocalizationEditor.Business
Imports DotNetNuke.Framework
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules
Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Common

Namespace DNNEurope.Modules.LocalizationEditor
 Partial Public Class ManageObjects
  Inherits ModuleBase

#Region " Event Handlers "

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
   If Not ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, _
                                                           "ManageObjects") Then
    Response.Redirect(AccessDeniedURL())
   End If
   '// Force full postback when using upload control
   AJAX.RegisterPostBackControl(lbImportPackage)

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
   '// Check if a file is given
   If Not ctlUpload.HasFile Then
    lblUploadError.Text = Localization.GetString("NoFile", LocalResourceFile)
    Return
   End If

   ManifestReader.ImportModulePackage(ctlUpload.FileName, ctlUpload.FileContent, PortalSettings.HomeDirectoryMapPath)

   '// Reload data
   BindData()
  End Sub

  ''' <summary>
  ''' Import an installed module into the localization editor
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  ''' <remarks></remarks>
  Private Sub lbImportInstalledModule_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles lbImportInstalledObject.Click
   '// Get the id of the installed module
   Dim desktopModuleId As Integer
   If Not Integer.TryParse(ddlInstalledObjects.SelectedValue, desktopModuleId) Then
    Return
   End If

   '// Get the desktopmodule information 
   Dim dm As DesktopModuleInfo = DesktopModuleController.GetDesktopModule(desktopModuleId, PortalId)

   '// Add the module into the localization editor
   Dim tm As New ObjectInfo(0, dm.ModuleName, dm.FriendlyName)
   tm.ObjectId = ObjectController.AddObject(tm)

   '// Process the resources for the module
   LocalizationController.ReadResourceFiles(Server.MapPath("~/"), PortalId, tm, UserId)

   '// Reload data
   BindData()
  End Sub

  Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReturn.Click
   Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
  End Sub

  Private Sub dlTranslateObjects_DeleteCommand(ByVal source As Object, ByVal e As DataListCommandEventArgs) _
      Handles dlTranslateObjects.DeleteCommand
   Dim ObjectId As Integer = CInt(dlTranslateObjects.DataKeys(e.Item.ItemIndex))
   ObjectController.DeleteObject(ObjectId)
   BindData()
  End Sub

#End Region

#Region " Private Methods "

  Private Sub BindData()
   '// Load all imported modules
   Dim translatedModules As ArrayList = ObjectController.GetObjectList()
   dlTranslateObjects.DataSource = translatedModules
   dlTranslateObjects.DataBind()

   '// Load the installed modules
   ddlInstalledObjects.Items.Clear()
   For Each dm As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(PortalId).Values
    '// If the module is not already imported add it to the list
    Dim isImported As Boolean = False
    For Each tm As ObjectInfo In translatedModules
     If tm.ObjectName = dm.ModuleName Then
      isImported = True
      Exit For
     End If
    Next
    If Not isImported Then
     ddlInstalledObjects.Items.Add(New ListItem(dm.FriendlyName, dm.DesktopModuleID.ToString))
    End If
   Next
  End Sub

#End Region

 End Class
End Namespace
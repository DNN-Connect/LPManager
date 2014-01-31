﻿' 
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
Imports DotNetNuke.Services.Exceptions

Partial Public Class Settings
 Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region " Base Method Implementations "

 Public Overrides Sub LoadSettings()
  Try
   If Not Page.IsPostBack Then
    Dim Settings As New ModuleSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
    With Settings
     txtLicense.Text = .License
     txtOwnerEmail.Text = .OwnerEmail
     txtOwnerName.Text = .OwnerName
     txtOwnerOrganization.Text = .OwnerOrganization
     txtOwnerUrl.Text = .OwnerUrl
     chkCachePacks.Checked = .CachePacks
     chkAllowDirectDownload.Checked = .AllowDirectDownload
     chkAllowDataExtract.Checked = .AllowDataExtract
     chkKeepStatistics.Checked = .KeepStatistics
     chkAutoImportObjects.Checked = .AutoImportObjects
     txtModuleKey.Text = .ModuleKey
     txtAttribution.Text = .Attribution
     chkWhiteSpaceSignificant.Checked = .WhiteSpaceSignificant
     chkManagersCanDelete.Checked = .ManagersCanDelete
    End With
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 Public Overrides Sub UpdateSettings()
  Try

   Dim Settings As New ModuleSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
   With Settings
    .CachePacks = chkCachePacks.Checked
    .License = txtLicense.Text.Trim
    .OwnerEmail = txtOwnerEmail.Text.Trim
    .OwnerName = txtOwnerName.Text.Trim
    .OwnerOrganization = txtOwnerOrganization.Text.Trim
    .OwnerUrl = txtOwnerUrl.Text.Trim
    .AllowDirectDownload = chkAllowDirectDownload.Checked
    .AllowDataExtract = chkAllowDataExtract.Checked
    .KeepStatistics = chkKeepStatistics.Checked
    .AutoImportObjects = chkAutoImportObjects.Checked
    .ModuleKey = txtModuleKey.Text.Trim
    .Attribution = txtAttribution.Text
    .WhiteSpaceSignificant = chkWhiteSpaceSignificant.Checked
    .ManagersCanDelete = chkManagersCanDelete.Checked
    .SaveSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
   End With

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

#End Region

End Class
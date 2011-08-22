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

Imports DotNetNuke.Entities.Modules
Imports DNNEurope.Modules.LocalizationEditor.Globals

Public Class ModuleSettings

#Region " Properties "
 Public Property OwnerName() As String = ""
 Public Property OwnerEmail() As String = ""
 Public Property OwnerUrl() As String = ""
 Public Property OwnerOrganization() As String = ""
 Public Property License() As String = ""
 Public Property CachePacks() As Boolean = True
 Public Property AllowDirectDownload() As Boolean = True
 Public Property AllowDataExtract() As Boolean = True
 Public Property KeepStatistics() As Boolean = False
 Public Property AutoImportObjects() As Boolean = False
 Public Property ModuleKey As String = ""
#End Region

#Region " Constructors "

 Public Sub New(ByVal portalHomeDirMapPath As String, ByVal moduleId As Integer)

  Dim mc As New ModuleController
  Dim settings As Hashtable = mc.GetModuleSettings(moduleId)
  ModuleKey = moduleId.ToString

  ReadValue(settings, "OwnerName", OwnerName)
  ReadValue(settings, "OwnerEmail", OwnerEmail)
  ReadValue(settings, "OwnerUrl", OwnerUrl)
  ReadValue(settings, "OwnerOrganization", OwnerOrganization)
  ReadValue(settings, "CachePacks", CachePacks)
  ReadValue(settings, "AllowDirectDownload", AllowDirectDownload)
  ReadValue(settings, "AllowDataExtract", AllowDataExtract)
  ReadValue(settings, "KeepStatistics", KeepStatistics)
  ReadValue(settings, "AutoImportObjects", AutoImportObjects)
  ReadValue(settings, "ModuleKey", ModuleKey)

  If portalHomeDirMapPath <> "" Then
   License = Globals.GetLicense(portalHomeDirMapPath, moduleId)
  End If

 End Sub

#End Region

#Region " Public Members "
 Public Sub SaveSettings(ByVal portalHomeDirMapPath As String, ByVal moduleId As Integer)

  Dim objModules As New ModuleController
  objModules.UpdateModuleSetting(ModuleId, "OwnerName", Me.OwnerName.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerEmail", Me.OwnerEmail.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerUrl", Me.OwnerUrl.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerOrganization", Me.OwnerOrganization.ToString)
  objModules.UpdateModuleSetting(ModuleId, "CachePacks", Me.CachePacks.ToString)
  objModules.UpdateModuleSetting(ModuleId, "AllowDirectDownload", Me.AllowDirectDownload.ToString)
  objModules.UpdateModuleSetting(ModuleId, "AllowDataExtract", Me.AllowDataExtract.ToString)
  objModules.UpdateModuleSetting(moduleId, "KeepStatistics", Me.KeepStatistics.ToString)
  objModules.UpdateModuleSetting(moduleId, "ModuleKey", Me.ModuleKey)
  DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(moduleId), Me)
  Globals.WriteLicense(portalHomeDirMapPath, ModuleId, License)

 End Sub

 Public Shared Function GetSettings(ByVal portalHomeDirMapPath As String, ByVal moduleId As Integer) As ModuleSettings

  Dim res As ModuleSettings = Nothing
  Try
   res = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey(moduleId)), ModuleSettings)
  Catch ex As Exception
  End Try
  If res Is Nothing Then
   res = New ModuleSettings(portalHomeDirMapPath, moduleId)
   DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(moduleId), res)
  End If
  Return res

 End Function

 Public Shared Function CacheKey(moduleId As Integer) As String
  Return String.Format("SettingsModule{0}", moduleId)
 End Function
#End Region

End Class

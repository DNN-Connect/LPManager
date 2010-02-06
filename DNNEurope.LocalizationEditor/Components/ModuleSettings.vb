' 
' Copyright (c) 2004-2009 DNN-Europe, http://www.dnn-europe.net
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


Public Class ModuleSettings

#Region " Private Members "

 Private _ownerName As String = ""
 Private _ownerEmail As String = ""
 Private _ownerUrl As String = ""
 Private _ownerOrganization As String = ""
 Private _license As String = ""
 Private _cachePacks As Boolean = True
 Private _allowDirectDownload As Boolean = True
 Private _allowDataExtract As Boolean = True
 Private _keepStatistics As Boolean = False

#End Region

#Region " Constructors "

 Public Sub New(ByVal PortalHomeDirMapPath As String, ByVal ModuleId As Integer)

  Dim mc As New ModuleController
  Dim Settings As Hashtable = mc.GetModuleSettings(ModuleId)

  If Not Settings.Item("OwnerName") Is Nothing Then
   OwnerName = CType(Settings.Item("OwnerName"), String)
  End If

  If Not Settings.Item("OwnerEmail") Is Nothing Then
   OwnerEmail = CType(Settings.Item("OwnerEmail"), String)
  End If

  If Not Settings.Item("OwnerUrl") Is Nothing Then
   OwnerUrl = CType(Settings.Item("OwnerUrl"), String)
  End If

  If Not Settings.Item("OwnerOrganization") Is Nothing Then
   OwnerOrganization = CType(Settings.Item("OwnerOrganization"), String)
  End If

  License = Globals.GetLicense(PortalHomeDirMapPath, ModuleId)

  If Not Settings.Item("CachePacks") Is Nothing Then
   CachePacks = CType(Settings.Item("CachePacks"), Boolean)
  End If

  If Not Settings.Item("AllowDirectDownload") Is Nothing Then
   AllowDirectDownload = CType(Settings.Item("AllowDirectDownload"), Boolean)
  End If

  If Not Settings.Item("AllowDataExtract") Is Nothing Then
   AllowDataExtract = CType(Settings.Item("AllowDataExtract"), Boolean)
  End If

  If Not Settings.Item("KeepStatistics") Is Nothing Then
   KeepStatistics = CType(Settings.Item("KeepStatistics"), Boolean)
  End If

 End Sub

#End Region

#Region " Public Members "

 Public Sub SaveSettings(ByVal PortalHomeDirMapPath As String, ByVal ModuleId As Integer)

  Dim objModules As New ModuleController
  objModules.UpdateModuleSetting(ModuleId, "OwnerName", Me.OwnerName.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerEmail", Me.OwnerEmail.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerUrl", Me.OwnerUrl.ToString)
  objModules.UpdateModuleSetting(ModuleId, "OwnerOrganization", Me.OwnerOrganization.ToString)
  objModules.UpdateModuleSetting(ModuleId, "CachePacks", Me.CachePacks.ToString)
  objModules.UpdateModuleSetting(ModuleId, "AllowDirectDownload", Me.AllowDirectDownload.ToString)
  objModules.UpdateModuleSetting(ModuleId, "AllowDataExtract", Me.AllowDataExtract.ToString)
  objModules.UpdateModuleSetting(ModuleId, "KeepStatistics", Me.KeepStatistics.ToString)
  Dim CacheKey As String = "Settings4Module" & ModuleId.ToString
  DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, Me)
  Globals.WriteLicense(PortalHomeDirMapPath, ModuleId, License)

 End Sub

#End Region

#Region " Properties "

 Public Property OwnerName() As String
  Get
   Return _ownerName
  End Get
  Set(ByVal Value As String)
   _ownerName = Value
  End Set
 End Property

 Public Property OwnerEmail() As String
  Get
   Return _ownerEmail
  End Get
  Set(ByVal value As String)
   _ownerEmail = value
  End Set
 End Property

 Public Property OwnerUrl() As String
  Get
   Return _ownerUrl
  End Get
  Set(ByVal value As String)
   _ownerUrl = value
  End Set
 End Property

 Public Property OwnerOrganization() As String
  Get
   Return _ownerOrganization
  End Get
  Set(ByVal value As String)
   _ownerOrganization = value
  End Set
 End Property

 Public Property License() As String
  Get
   Return _license
  End Get
  Set(ByVal value As String)
   _license = value
  End Set
 End Property

 Public Property CachePacks() As Boolean
  Get
   Return _cachePacks
  End Get
  Set(ByVal value As Boolean)
   _cachePacks = value
  End Set
 End Property

 Public Property AllowDirectDownload() As Boolean
  Get
   Return _allowDirectDownload
  End Get
  Set(ByVal value As Boolean)
   _allowDirectDownload = value
  End Set
 End Property

 Public Property AllowDataExtract() As Boolean
  Get
   Return _allowDataExtract
  End Get
  Set(ByVal value As Boolean)
   _allowDataExtract = value
  End Set
 End Property

 Public Property KeepStatistics() As Boolean
  Get
   Return _keepStatistics
  End Get
  Set(ByVal value As Boolean)
   _keepStatistics = value
  End Set
 End Property

#End Region

End Class

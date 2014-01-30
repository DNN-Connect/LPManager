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
Imports DotNetNuke.Security.Permissions

Public Class ModuleBase
 Inherits PortalModuleBase

#Region " Variables "

 Private _settings As ModuleSettings

#End Region

#Region " Properties "
 Public Property IsAdmin As Boolean = False

 Public ReadOnly Property CultureTextInfo As Globalization.TextInfo
  Get
   Return Threading.Thread.CurrentThread.CurrentCulture.TextInfo
  End Get
 End Property

 Public Shadows Property Settings() As ModuleSettings
  Get

   If _settings Is Nothing Then
    _settings = ModuleSettings.GetSettings(PortalSettings.HomeDirectoryMapPath, ModuleId)
   End If
   Return _settings

  End Get
  Set(ByVal Value As ModuleSettings)
   _settings = Value
  End Set
 End Property
#End Region

#Region " Page Events "
 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  If UserInfo.IsSuperUser Then
   IsAdmin = True
  End If
  If ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "EDIT") Then
   IsAdmin = True
  End If
 End Sub
#End Region

End Class

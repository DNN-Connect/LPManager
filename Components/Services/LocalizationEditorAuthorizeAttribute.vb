Imports System.Web

Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users

Imports DNNEurope.Modules.LocalizationEditor.Entities.Permissions
Imports System.Threading
Imports DotNetNuke.Web.Api
Imports DotNetNuke.Common
Imports System.Security.Cryptography

Namespace Services
 Public Class LocalizationEditorAuthorizeAttribute
  Inherits AuthorizeAttributeBase
  Implements IOverrideDefaultAuthLevel

  Public Sub New()
   AccessLevel = SecurityAccessLevel.Admin
  End Sub
  Public Sub New(accessLevel As SecurityAccessLevel)
   Me.AccessLevel = accessLevel
  End Sub

  Public Property AccessLevel() As SecurityAccessLevel
   Get
    Return m_AccessLevel
   End Get
   Set(value As SecurityAccessLevel)
    m_AccessLevel = value
   End Set
  End Property
  Private m_AccessLevel As SecurityAccessLevel

  Public Property UserInfo() As UserInfo
  Public Property AccessKey As Integer = -1
  Public Property AccessHash As String = ""
  Public Property AccessSalt As String = ""

  Public Overrides Function IsAuthorized(context As DotNetNuke.Web.Api.AuthFilterContext) As Boolean

   If AccessLevel = SecurityAccessLevel.Anonymous Then Return True

   Dim activeModule As ModuleInfo = context.ActionContext.Request.FindModuleInfo()

   Dim values As IEnumerable(Of String) = Nothing
   If context.ActionContext.Request.Headers.TryGetValues("AccessKey", values) Then
    Integer.TryParse(values.FirstOrDefault, AccessKey)
   End If

   If context.ActionContext.Request.Headers.TryGetValues("AccessHash", values) Then
    AccessHash = values.FirstOrDefault
   End If

   If context.ActionContext.Request.Headers.TryGetValues("AccessSalt", values) Then
    AccessSalt = values.FirstOrDefault
   End If

   If AccessKey <> -1 Then
    Dim permission As PermissionInfo = PermissionsController.GetPermissionById(AccessKey)
    If permission IsNot Nothing Then
     ' check the hash
     Dim textToEncode As String = permission.AccessKey.ToString & AccessSalt
     If Not String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form("body")) Then
      textToEncode &= System.Web.HttpContext.Current.Request.Form("body")
     End If
     Dim hash As New SHA256Managed
     Dim result As String = Convert.ToBase64String(hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(textToEncode)))
     If result = AccessHash Then
      Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
      _UserInfo = UserController.GetUserById(portalSettings.PortalId, permission.UserId)
     End If
    End If
   End If

   If _UserInfo Is Nothing Then
    _UserInfo = New UserInfo
   End If

   If activeModule IsNot Nothing Then
    Return HasModuleAccess(AccessLevel, "", activeModule)
   End If

   Return False
  End Function

  Public Function HasModuleAccess(accessLevel As SecurityAccessLevel, permissionKey As String, moduleConfiguration As ModuleInfo) As Boolean
   Dim isAuthorized As Boolean = False

   If UserInfo IsNot Nothing AndAlso UserInfo.IsSuperUser Then
    isAuthorized = True
   Else
    Select Case accessLevel
     Case SecurityAccessLevel.Anonymous
      isAuthorized = True
      Exit Select
     Case SecurityAccessLevel.View
      If DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(moduleConfiguration) Then
       isAuthorized = True
      End If
      Exit Select
     Case SecurityAccessLevel.Edit
      If DotNetNuke.Security.Permissions.TabPermissionController.CanAddContentToPage() Then
       isAuthorized = True
      Else
       If String.IsNullOrEmpty(permissionKey) Then
        permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE"
       End If
       If moduleConfiguration IsNot Nothing AndAlso DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(moduleConfiguration) AndAlso (DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(moduleConfiguration.ModulePermissions, permissionKey) OrElse DotNetNuke.Security.Permissions.ModulePermissionController.HasModulePermission(moduleConfiguration.ModulePermissions, "EDIT")) Then
        isAuthorized = True
       End If
      End If
      Exit Select
     Case SecurityAccessLevel.Admin
      isAuthorized = DotNetNuke.Security.Permissions.TabPermissionController.CanAddContentToPage()
      Exit Select
     Case SecurityAccessLevel.Translator
      For Each p As PermissionInfo In PermissionsController.GetPermissions(moduleConfiguration.ModuleID)
       If p.UserId = UserInfo.UserID Then
        isAuthorized = True
        Exit Select
       End If
      Next
      isAuthorized = False
      Exit Select
    End Select
   End If
   Return isAuthorized
  End Function

 End Class

 Public Enum SecurityAccessLevel As Integer
  Anonymous = 0
  Admin = 1
  View = 2
  Edit = 3
  Translator = 4
 End Enum
End Namespace

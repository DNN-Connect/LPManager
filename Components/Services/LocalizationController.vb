Imports System.Web.Mvc
Imports DotNetNuke.Web.Services
Imports DotNetNuke.Security.Permissions
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports System.Globalization
Imports System.Runtime.Serialization.Json

Namespace Services
 Public Class LocalizationController
  Inherits DnnController
  Implements IServiceRouteMapper

  Public Sub RegisterRoutes(mapRouteManager As DotNetNuke.Web.Services.IMapRoute) Implements DotNetNuke.Web.Services.IServiceRouteMapper.RegisterRoutes
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "", New With {.Controller = "Localization", .Action = "PublicModules"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}", New With {.Controller = "Localization", .Action = "ListObjects"}, New With {.tabId = "\d*", .moduleId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}/Objects", New With {.Controller = "Localization", .Action = "ListObjects"}, New With {.tabId = "\d*", .moduleId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}/EditLocales", New With {.Controller = "Localization", .Action = "GetEditLocales"}, New With {.tabId = "\d*", .moduleId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}/{objectName}/{objectVersion}/Resources", New With {.Controller = "Localization", .Action = "GetResources"}, New With {.tabId = "\d*", .moduleId = "\d*", .objectVersion = "\d+\.\d+\.\d+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}/UpdateResources", New With {.Controller = "Localization", .Action = "UpdateResources"}, New With {.tabId = "\d*", .moduleId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapRoute("DNNEurope/LocalizationEditor", "{tabid}/{moduleId}/{objectName}/{objectVersion}/File/{*fileKey}", New With {.Controller = "Localization", .Action = "GetResourceFile"}, New With {.tabId = "\d*", .moduleId = "\d*", .objectVersion = "\d+\.\d+\.\d+", .fileKey = "[^?]*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
  End Sub

  <DnnAuthorize(AllowAnonymous:=True)>
  Public Function PublicModules() As ActionResult
   Dim mc As New DotNetNuke.Entities.Modules.ModuleController()
   Dim modules As New Dictionary(Of String, String)()
   For Each m As DotNetNuke.Entities.Modules.ModuleInfo In GetLEModules(PortalSettings.PortalId)
    If ModulePermissionController.HasModuleAccess(DotNetNuke.Security.SecurityAccessLevel.View, "", m) Then
     modules.Add(String.Format("{0}/{1}", m.TabID, m.ModuleID), m.ModuleTitle)
    End If
   Next
   Return Json(modules, JsonRequestBehavior.AllowGet)
  End Function

  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.View)>
  Public Function GetLEModules(portalID As Integer) As List(Of DotNetNuke.Entities.Modules.ModuleInfo)
   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of DotNetNuke.Entities.Modules.ModuleInfo)(DotNetNuke.Data.DataProvider.Instance().GetModuleByDefinition(portalID, "Localization Editor"))
  End Function

  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function ListObjects(tabId As Integer, moduleId As Integer) As ActionResult
   Dim translatedModules As List(Of ObjectInfo) = ObjectsController.GetObjects(moduleId)
   Return Json(translatedModules, JsonRequestBehavior.AllowGet)
  End Function

  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetEditLocales(tabId As Integer, moduleId As Integer) As ActionResult
   Dim res As New List(Of String)
   For Each p As Entities.Permissions.PermissionInfo In Entities.Permissions.PermissionsController.GetPermissions(moduleId)
    If p.UserId = UserInfo.UserID Then
     res.Add(p.Locale)
    End If
   Next
   Return Json(res, JsonRequestBehavior.AllowGet)
  End Function

  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetResources(tabId As Integer, moduleId As Integer, objectName As String, objectVersion As String) As ActionResult
   Dim obj As ObjectInfo = ObjectsController.GetObjectByObjectName(moduleId, objectName)
   If obj Is Nothing Then Return Json(New List(Of Entities.Texts.TextInfo), JsonRequestBehavior.AllowGet)
   Dim locale As String = ""
   If HttpContext.Request.Params("locale") IsNot Nothing Then
    locale = HttpContext.Request.Params("locale")
   End If
   Dim res As New List(Of Entities.Texts.TextInfo)
   For Each o As ObjectInfo In ObjectsController.GetObjectPackList(obj.ObjectId, objectVersion)
    For Each ti As Entities.Texts.TextInfo In Entities.Texts.TextsController.GetTextsByObject(moduleId, o.ObjectId, locale, objectVersion).Values
     res.Add(ti)
    Next
   Next
   Return New LargeJsonResult() With {.Data = res, .MaxJsonLength = Int32.MaxValue, .JsonRequestBehavior = JsonRequestBehavior.AllowGet}
  End Function

  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetResourceFile(tabId As Integer, moduleId As Integer, objectName As String, objectVersion As String, fileKey As String) As ActionResult
   fileKey = fileKey.Replace("=", "_").Replace("-", ".").Replace("/", "\")
   Dim obj As ObjectInfo = ObjectsController.GetObjectByObjectName(moduleId, objectName)
   If obj Is Nothing Then Return Json(New List(Of Entities.Texts.TextInfo), JsonRequestBehavior.AllowGet)
   Dim locale As String = ""
   If HttpContext.Request.Params("locale") IsNot Nothing Then
    locale = HttpContext.Request.Params("locale")
   End If
   Dim res As New List(Of Entities.Texts.TextInfo)
   For Each ti As Entities.Texts.TextInfo In Entities.Texts.TextsController.GetTextsByObjectAndFile(moduleId, obj.ObjectId, fileKey, locale, objectVersion, True).Values
    res.Add(ti)
   Next
   Return New LargeJsonResult() With {.Data = res, .MaxJsonLength = Int32.MaxValue, .JsonRequestBehavior = JsonRequestBehavior.AllowGet}
  End Function


  <DnnAuthorize(AllowAnonymous:=True)>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function UpdateResources(tabId As Integer, moduleId As Integer) As ActionResult

   Dim keepStatistics As Boolean = ModuleSettings.GetSettings(PortalSettings.HomeDirectoryMapPath, moduleId).KeepStatistics

   Dim serializer As New DataContractJsonSerializer(GetType(List(Of Entities.Texts.TextInfo)))
   Dim textList As New List(Of Entities.Texts.TextInfo)
   Try
    Using s As IO.Stream = System.Web.HttpContext.Current.Request.InputStream
     textList = CType(serializer.ReadObject(s), Global.System.Collections.Generic.List(Of Entities.Texts.TextInfo))
    End Using
   Catch ex As Exception
   End Try
   If textList.Count = 0 Then Return Json("OK", JsonRequestBehavior.AllowGet)

   ' all texts should be for the same locale - check the user's status on this
   Dim targetLocale As String = textList(0).Locale
   If Not Entities.Permissions.PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, moduleId, targetLocale) Then
    Return Json("Access Denied", JsonRequestBehavior.AllowGet)
   End If

   ' begin but keep checking object and text ids
   Dim okObjects As New List(Of Integer)
   For Each o As ObjectInfo In ObjectsController.GetObjects(moduleId)
    okObjects.Add(o.ObjectId)
   Next
   For Each t As Entities.Texts.TextInfo In textList
    If okObjects.Contains(t.ObjectId) Then
     Dim check As Entities.Texts.TextInfo = Entities.Texts.TextsController.GetTextByVersion(t.ObjectId, t.FilePath, t.TextKey, t.Version)
     If check IsNot Nothing Then
      Dim trans As Entities.Translations.TranslationInfo = Entities.Translations.TranslationsController.GetTranslation(check.TextId, targetLocale)
      Dim stat As Integer = t.Translation.Length
      If trans Is Nothing Then
       trans = New Entities.Translations.TranslationInfo With {.LastModified = Now, .LastModifiedUserId = UserInfo.UserID, .Locale = targetLocale, .TextId = check.TextId, .TextValue = t.Translation}
      ElseIf trans.TextValue <> t.Translation Then
       If keepStatistics Then
        If t.Translation.Length > 200 Then
         stat = Math.Abs(t.Translation.Length - trans.TextValue.Length)
        Else
         stat = Globals.LevenshteinDistance(trans.TextValue, t.Translation)
        End If
       End If
       trans.LastModified = Now
       trans.LastModifiedUserId = UserInfo.UserID
       trans.TextValue = t.Translation
       If stat > 0 And keepStatistics Then
        Entities.Statistics.StatisticsController.RecordStatistic(check.TextId, targetLocale, UserInfo.UserID, stat)
       End If
      End If
      Entities.Translations.TranslationsController.SetTranslation(trans)
     End If
    End If
   Next

   Return Json("OK", JsonRequestBehavior.AllowGet)

  End Function

  Public Structure Resource
   Public FileKey As String
   Public ResourceKey As String
   Public OriginalValue As String
   Public Translation As String
  End Structure

 End Class
End Namespace
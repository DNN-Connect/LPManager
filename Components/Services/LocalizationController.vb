Imports DotNetNuke.Security.Permissions
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects
Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Web.Api
Imports DNNEurope.Modules.LocalizationEditor.Entities.Packages

Namespace Services
 Public Class LocalizationController
  Inherits DnnApiController
  Implements IServiceRouteMapper

  Public Sub RegisterRoutes(mapRouteManager As DotNetNuke.Web.Api.IMapRoute) Implements DotNetNuke.Web.Api.IServiceRouteMapper.RegisterRoutes
   ' Public Routes
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "Default", "", New With {.Controller = "Localization", .Action = "ListObjects"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListObjects", "Objects", New With {.Controller = "Localization", .Action = "ListObjects"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListAllPacks1", "Object/{objectId}", New With {.Controller = "Localization", .Action = "ListAllPacks"}, New With {.objectId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListAllPacks2", "Object/{objectId}/Packs", New With {.Controller = "Localization", .Action = "ListAllPacks"}, New With {.objectId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListPacksByLocale1", "Object/{objectId}/Locale/{locale}", New With {.Controller = "Localization", .Action = "ListPacksByLocale"}, New With {.objectId = "\d*", .locale = "\w+-\w+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListPacksByLocale2", "Object/{objectId}/Locale/{locale}/Packs", New With {.Controller = "Localization", .Action = "ListPacksByLocale"}, New With {.objectId = "\d*", .locale = "\w+-\w+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListPacksByVersion1", "Object/{objectId}/Version/{version}", New With {.Controller = "Localization", .Action = "ListPacksByVersion"}, New With {.objectId = "\d*", .version = "\d+\.\d+\.\d+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "ListPacksByVersion2", "Object/{objectId}/Version/{version}/Packs", New With {.Controller = "Localization", .Action = "ListPacksByVersion"}, New With {.objectId = "\d*", .version = "\d+\.\d+\.\d+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   ' Translator Routes
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "GetEditLocales", "EditLocales", New With {.Controller = "Localization", .Action = "GetEditLocales"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "GetResources", "Object/{objectName}/Version/{objectVersion}/Resources", New With {.Controller = "Localization", .Action = "GetResources"}, New With {.objectVersion = "\d+\.\d+\.\d+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "GetResourceFile", "Object/{objectName}/Version/{objectVersion}/File/{*fileKey}", New With {.Controller = "Localization", .Action = "GetResourceFile"}, New With {.objectVersion = "\d+\.\d+\.\d+", .fileKey = "[^?]*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "UpdateResources", "UpdateResources", New With {.Controller = "Localization", .Action = "UpdateResources"}, New With {.tabId = "\d*", .moduleId = "\d*"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   ' Helpers
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "AFT", "aft", New With {.controller = "Localization", .action = "Aft"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

   ' Updater routes
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "GetUpdateStatus", "Status", New With {.Controller = "Localization", .Action = "GetUpdateStatus"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})
   mapRouteManager.MapHttpRoute("DNNEurope/LocalizationEditor", "GetObjectPack", "ObjectId/{objectId}/Version/{objectVersion}/Locale/{locale}/Pack", New With {.Controller = "Localization", .Action = "GetObjectPack"}, New With {.objectId = "\d+", .locale = "\w+-\w+"}, New String() {"DNNEurope.Modules.LocalizationEditor.Services"})

  End Sub

#Region " Public Part "
  <HttpGet()>
  <AllowAnonymous()>
  Public Function ListObjects() As HttpResponseMessage
   Dim translatedModules As List(Of ObjectInfo) = ObjectsController.GetObjects(ActiveModule.ModuleID)
   Return Request.CreateResponse(HttpStatusCode.OK, translatedModules)
  End Function

  <HttpGet()>
  <AllowAnonymous()>
  Public Function ListAllPacks(objectId As Integer) As HttpResponseMessage
   Return Request.CreateResponse(HttpStatusCode.OK, PackagesController.GetPackages(ActiveModule.ModuleID, objectId))
  End Function

  <HttpGet()>
  <AllowAnonymous()>
  Public Function ListPacksByLocale(objectId As Integer, locale As String) As HttpResponseMessage
   Return Request.CreateResponse(HttpStatusCode.OK, PackagesController.GetPackagesByLocale(ActiveModule.ModuleID, objectId, locale))
  End Function

  <HttpGet()>
  <AllowAnonymous()>
  Public Function ListPacksByVersion(objectId As Integer, version As String) As HttpResponseMessage
   Return Request.CreateResponse(HttpStatusCode.OK, PackagesController.GetPackagesByVersion(ActiveModule.ModuleID, objectId, version))
  End Function
#End Region

#Region " Translator Part "
  <HttpGet()>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetEditLocales(tabId As Integer, moduleId As Integer) As HttpResponseMessage
   Dim res As New List(Of String)
   For Each p As Entities.Permissions.PermissionInfo In Entities.Permissions.PermissionsController.GetPermissions(moduleId)
    If p.UserId = UserInfo.UserID Then
     res.Add(p.Locale)
    End If
   Next
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function

  <HttpGet()>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetResources(objectName As String, objectVersion As String) As HttpResponseMessage
   Dim obj As ObjectInfo = ObjectsController.GetObjectByObjectName(ActiveModule.ModuleID, objectName)
   If obj Is Nothing Then Return Request.CreateResponse(HttpStatusCode.OK, New List(Of Entities.Texts.TextInfo))
   Dim locale As String = ""
   Dim queryString As NameValueCollection = HttpUtility.ParseQueryString(Me.Request.RequestUri.Query)
   If queryString("locale") IsNot Nothing Then
    locale = queryString("locale")
   End If
   Dim res As New List(Of Entities.Texts.TextInfo)
   For Each o As ObjectInfo In ObjectsController.GetObjectPackList(obj.ObjectId, objectVersion)
    For Each ti As Entities.Texts.TextInfo In Entities.Texts.TextsController.GetTextsByObject(ActiveModule.ModuleID, o.ObjectId, locale, objectVersion).Values
     res.Add(ti)
    Next
   Next
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function

  <HttpGet()>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function GetResourceFile(objectName As String, objectVersion As String, fileKey As String) As HttpResponseMessage
   fileKey = fileKey.Replace("=", "_").Replace("-", ".").Replace("/", "\")
   Dim obj As ObjectInfo = ObjectsController.GetObjectByObjectName(ActiveModule.ModuleID, objectName)
   If obj Is Nothing Then Return Request.CreateResponse(HttpStatusCode.OK, New List(Of Entities.Texts.TextInfo))
   Dim locale As String = ""
   Dim queryString As NameValueCollection = HttpUtility.ParseQueryString(Me.Request.RequestUri.Query)
   If queryString("locale") IsNot Nothing Then
    locale = queryString("locale")
   End If
   Dim res As New List(Of Entities.Texts.TextInfo)
   For Each ti As Entities.Texts.TextInfo In Entities.Texts.TextsController.GetTextsByObjectAndFile(ActiveModule.ModuleID, obj.ObjectId, fileKey, locale, objectVersion, True).Values
    res.Add(ti)
   Next
   Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Function

  <HttpPost()>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  <ValidateAntiForgeryToken()>
  Public Function UpdateResources() As HttpResponseMessage

   Dim settings As ModuleSettings = ModuleSettings.GetSettings(PortalSettings.HomeDirectoryMapPath, ActiveModule.ModuleID)
   Dim keepStatistics As Boolean = settings.KeepStatistics

   Dim serializer As New DataContractJsonSerializer(GetType(List(Of Entities.Texts.TextInfo)))
   Dim textList As New List(Of Entities.Texts.TextInfo)
   Dim requestBody As String = System.Web.HttpContext.Current.Request.Form("body")
   Try
    Using memStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody))
     memStream.Seek(0, IO.SeekOrigin.Begin)
     textList = CType(serializer.ReadObject(memStream), Global.System.Collections.Generic.List(Of Entities.Texts.TextInfo))
    End Using
   Catch ex As Exception
   End Try
   If textList.Count = 0 Then
    Return Request.CreateResponse(HttpStatusCode.OK, "OK")
   End If

   ' all texts should be for the same locale - check the user's status on this
   Dim targetLocale As String = textList(0).Locale
   If Not Entities.Permissions.PermissionsController.HasAccess(UserInfo, PortalSettings.AdministratorRoleName, ActiveModule.ModuleID, targetLocale) Then
    Return Request.CreateResponse(HttpStatusCode.Unauthorized, "Access Denied")
   End If

   ' begin but keep checking object and text ids
   Dim okObjects As New List(Of Integer)
   For Each o As ObjectInfo In ObjectsController.GetObjects(ActiveModule.ModuleID)
    okObjects.Add(o.ObjectId)
   Next
   For Each t As Entities.Texts.TextInfo In textList
    If Not settings.WhiteSpaceSignificant Then
     t.Translation = t.Translation.Trim
    End If
    Dim check As Entities.Texts.TextInfo = Entities.Texts.TextsController.GetTextByVersion(t.ObjectId, t.FilePath, t.TextKey, t.Version)
    If check IsNot Nothing Then
     If t.ObjectId < 0 Then ' what if we receive a text that has not been bound to an object yet?
      t.ObjectId = check.ObjectId
     End If
     If okObjects.Contains(t.ObjectId) Then
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
      End If
      If stat > 0 And keepStatistics Then
       Entities.Statistics.StatisticsController.RecordStatistic(check.TextId, targetLocale, UserInfo.UserID, stat)
      End If
      Entities.Translations.TranslationsController.SetTranslation(trans)
     End If
    End If
   Next

   Return Request.CreateResponse(HttpStatusCode.OK, "OK")

  End Function
#End Region

#Region " Update Service "
  <HttpPost()>
  <AllowAnonymous()>
  Public Function GetUpdateStatus() As HttpResponseMessage

   Dim serializer As New DataContractJsonSerializer(GetType(List(Of UpdateService.DnnPackage)))
   Dim requestBody As String = System.Web.HttpContext.Current.Request.Form("body")
   Dim packageList As New List(Of UpdateService.DnnPackage)
   Try
    Using memStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody))
     memStream.Seek(0, IO.SeekOrigin.Begin)
     packageList = CType(serializer.ReadObject(memStream), Global.System.Collections.Generic.List(Of UpdateService.DnnPackage))
    End Using
   Catch ex As Exception
   End Try
   If packageList.Count = 0 Then
    Return Request.CreateResponse(HttpStatusCode.OK, "OK")
   End If

   Dim res As New List(Of UpdateService.DnnPackage)
   For Each package As UpdateService.DnnPackage In packageList
    Using ir As IDataReader = Data.DataProvider.Instance().GetTranslationStatusByObject(PortalSettings.PortalId, package.PackageName, package.Version, package.TargetLocale)
     If ir.Read Then
      package.ObjectId = Convert.ToInt32(Null.SetNull(ir.Item("ObjectId"), package.ObjectId))
      'package.TextCount = Convert.ToInt32(Null.SetNull(ir.Item("TextCount"), package.TextCount))
      package.Available = Convert.ToInt32(Null.SetNull(ir.Item("Translated"), package.Available))
      package.LastChange = CDate(Null.SetNull(ir.Item("LastModified"), package.LastChange))
      res.Add(package.Clone)
     End If
    End Using
   Next

   Return Request.CreateResponse(HttpStatusCode.OK, res)

  End Function

  <HttpGet()>
  <AllowAnonymous()>
  Public Function GetObjectPack(objectId As Integer, objectVersion As String, locale As String) As HttpResponseMessage

   Dim response As HttpResponse = HttpContext.Current.Response
   Dim fn As String = ""
   Dim _requestedObject As ObjectInfo = ObjectsController.GetObject(objectId)
   fn = Services.Packaging.PackageWriter.CreateResourcePack(_requestedObject, objectVersion, locale, False)
   response.Clear()
   response.AppendHeader("Content-Disposition", "attachment; filename=""" & fn & """")
   response.AppendHeader("Content-Encoding", "identity")
   Using fileIn As New IO.FileStream(DotNetNuke.Common.ApplicationMapPath & "\" & _requestedObject.Module.HomeDirectory & "\LocalizationEditor\Cache\" & _requestedObject.ModuleId.ToString & "\" & fn, IO.FileMode.Open, IO.FileAccess.Read)
    Dim bBuffer(25000) As Byte
    Dim iLengthOfReadChunk As Int32
    Do
     iLengthOfReadChunk = fileIn.Read(bBuffer, 0, 25000)
     response.OutputStream.Write(bBuffer, 0, iLengthOfReadChunk)
     If iLengthOfReadChunk = 0 Then Exit Do
    Loop
   End Using

   Return Request.CreateResponse(HttpStatusCode.OK, "")

  End Function
#End Region

#Region " Helpers "
  <HttpGet()>
  <LocalizationEditorAuthorizeAttribute(Services.SecurityAccessLevel.Translator)>
  Public Function Aft() As HttpResponseMessage
   Dim afhtml As String = System.Web.Helpers.AntiForgery.GetHtml.ToString
   afhtml = System.Text.RegularExpressions.Regex.Match(afhtml, "value=""([^""]*)""").Groups(1).Value
   If System.Web.HttpContext.Current.Response.Cookies(System.Web.Helpers.AntiForgeryConfig.CookieName) Is Nothing Then
    System.Web.HttpContext.Current.Response.Cookies.Add(New System.Web.HttpCookie(System.Web.Helpers.AntiForgeryConfig.CookieName, afhtml))
   End If
   Return Request.CreateResponse(HttpStatusCode.OK, afhtml)
  End Function

  Public Structure Resource
   Public FileKey As String
   Public ResourceKey As String
   Public OriginalValue As String
   Public Translation As String
  End Structure
#End Region

 End Class
End Namespace
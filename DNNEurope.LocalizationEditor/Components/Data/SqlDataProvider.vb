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
Imports Microsoft.ApplicationBlocks.Data

Namespace Data
 Public Class SqlDataProvider
  Inherits DataProvider

#Region " ObjectCoreVersion Methods "

  Public Overrides Sub SetObjectCoreVersion(ObjectId As Int32, Version As String, DnnVersion As String, InstalledByDefault As Boolean)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetObjectCoreVersion", ObjectId, Version, DnnVersion, InstalledByDefault)
  End Sub

  Public Overrides Function GetCoreObjects(ByVal DnnVersion As String, GetAllObjects As Boolean) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetCoreObjects", DnnVersion, GetAllObjects), IDataReader)
  End Function

#End Region

#Region " Objects Methods "

  Public Overrides Function GetObjects(ByVal moduleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjects", ModuleId), IDataReader)
  End Function

  Public Overrides Function GetObjectByObjectNameAndModuleKey(ByVal portalId As Integer, ByVal objectName As String, ByVal moduleKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectByObjectNameAndModuleKey", portalId, objectName, moduleKey), IDataReader)
  End Function

  Public Overrides Function GetObjectByObjectName(moduleId As Integer, ByVal objectName As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectByObjectName", ModuleId, ObjectName), IDataReader)
  End Function

  Public Overrides Function GetObjectsByObjectName(ByVal objectName As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectsByObjectName", ObjectName), IDataReader)
  End Function

  Public Overrides Function GetObjectsWithStatus(ByVal moduleId As Integer) As IDataReader

   Dim locales As List(Of String) = Entities.Translations.TranslationsController.GetLocales(ModuleId)
   Dim sql As New StringBuilder
   'sql.Append("SELECT o.ObjectId, o.ObjectName, o.FriendlyName, o.InstallPath, o.ModuleId, o.PackageType, o.LastVersion, COUNT(t.textId) NrTexts, MAX(ISNULL(pp.PercentComplete,0)) PartnerComplete")
   sql.Append("SELECT o.ObjectId, o.ObjectName, o.FriendlyName, o.InstallPath, o.ModuleId, o.PackageType, o.LastVersion, o.LastVersionTextCount")
   Dim i As Integer = 1
   For Each Loc As String In locales
    sql.AppendFormat(",COUNT(t{0}.TextId) {1}", i, Loc.Replace("-", ""))
    i += 1
   Next
   sql.AppendLine()
   sql.AppendFormat("FROM {0}{1}vw_LocalizationEditor_Objects o", DatabaseOwner, ObjectQualifier)
   sql.AppendLine()
   sql.AppendFormat(" LEFT JOIN {0}{1}LocalizationEditor_Texts t ON o.ObjectId=t.ObjectId AND (t.DeprecatedIn IS NULL) AND NOT (t.OriginalValue IS NULL OR t.OriginalValue='')", DatabaseOwner, ObjectQualifier)
   sql.AppendLine()
   i = 1
   For Each Loc As String In locales
    If Loc.Length > 2 Then
     sql.AppendFormat(" LEFT JOIN {2}{3}LocalizationEditor_Translations t{0} ON t{0}.TextId=t.TextId AND (t{0}.Locale='{1}' OR t{0}.Locale='{4}')", i, Loc, DatabaseOwner, ObjectQualifier, Left(Loc, 2))
    Else
     sql.AppendFormat(" LEFT JOIN {2}{3}LocalizationEditor_Translations t{0} ON t{0}.TextId=t.TextId AND t{0}.Locale='{1}'", i, Loc, DatabaseOwner, ObjectQualifier)
    End If
    i += 1
   Next
   sql.AppendLine()
   'sql.AppendFormat(" LEFT JOIN {0}{1}LocalizationEditor_PartnerPacks pp ON pp.ObjectId=o.ObjectId AND pp.Version=o.LastVersion", DatabaseOwner, ObjectQualifier)
   'sql.AppendLine()
   sql.AppendLine("GROUP BY o.ObjectId, o.ObjectName, o.FriendlyName, o.InstallPath, o.ModuleId, o.PackageType, o.LastVersion, o.LastVersionTextCount")
   sql.AppendFormat("HAVING o.ModuleId={0}", ModuleId)
   sql.AppendLine()
   sql.AppendLine("ORDER BY o.FriendlyName")
   Return CType(SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sql.ToString), IDataReader)

  End Function

#End Region

#Region " Permissions Methods "

  Public Overrides Function GetPermission(ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPermission", UserId, Locale, ModuleId), IDataReader)
  End Function

  Public Overrides Function GetPermissions(ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPermissions", ModuleId), IDataReader)
  End Function

#End Region

#Region " Statistics Methods "

  Public Overrides Sub SetStatistic(ByVal Locale As String, ByVal TextId As Int32, ByVal Total As Int32, ByVal UserId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetStatistic", Locale, TextId, Total, UserId)
  End Sub

  Public Overrides Function GetPackStatistics(ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPackStatistics", ObjectId, Version, Locale), IDataReader)
  End Function

#End Region

#Region " Texts Methods "

  Public Overrides Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "FindText", ObjectId, FilePath, Locale, Version, TextKey), IDataReader)
  End Function

  Public Overrides Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLatestText", ObjectId, FilePath, Locale, TextKey), IDataReader)
  End Function

  Public Overrides Function GetTextsByObject(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTextsByObject", ModuleId, ObjectId, Locale, Version), IDataReader)
  End Function

  Public Overrides Function GetTextsByObjectAndFile(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTextsByObjectAndFile", ModuleId, ObjectId, FilePath, Locale, Version, IncludeNonTranslated), IDataReader)
  End Function

  Public Overrides Function GetAdjacentTextsForCore(ByVal ModuleId As Integer, ByVal CoreObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetAdjacentTextsForCore", ModuleId, CoreObjectId, FilePath, Locale, Version, IncludeNonTranslated), IDataReader)
  End Function

  Public Overrides Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "CurrentVersion", ObjectId, Locale), IDataReader)
  End Function

  Public Overrides Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "NrOfChangedTexts", ObjectId, Version), IDataReader)
  End Function

  Public Overrides Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "NrOfFiles", ObjectId, Version), IDataReader)
  End Function

  Public Overrides Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "NrOfItems", ObjectId, Version), IDataReader)
  End Function

  Public Overrides Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "NrOfMissingTranslations", ObjectId, Locale, Version), IDataReader)
  End Function

  Public Overrides Function GetFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetFiles", ObjectId, Version), IDataReader)
  End Function

  Public Overrides Function GetVersions(ByVal ObjectId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetVersions", ObjectId), IDataReader)
  End Function

  Public Overrides Function GetTranslationList(ByVal ObjectId As Integer, ByVal Locale As String, ByVal SourceLocale As String, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslationList", ObjectId, Locale, SourceLocale, Version), IDataReader)
  End Function

#End Region

#Region " Translations Methods "

  Public Overrides Function SetTranslation(ByVal lastModified As Date, ByVal lastModifiedUserId As Int32, ByVal locale As String, ByVal textId As Int32, ByVal textValue As String) As Integer
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetTranslation", GetNull(LastModified), GetNull(LastModifiedUserId), Locale, TextId, GetNull(TextValue))
  End Function

#End Region

#Region " Partner Methods "
  Public Overrides Function GetPartnerByName(ByVal moduleId As Integer, ByVal name As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartnerByName", moduleId, name), IDataReader)
  End Function

  Public Overrides Function GetAllPartners() As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetAllPartners"), IDataReader)
  End Function
#End Region

#Region " Other Procedures "

  Public Overrides Function GetUsersFiltered(ByVal filter As String) As DataSet
   Return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUsersFiltered", filter)
  End Function

  Public Overrides Function GetLocales(ByVal moduleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLocales", ModuleId), IDataReader)
  End Function

  Public Overrides Function GetLocalesForUser(ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLocalesForUser", UserId, PortalId, ModuleId), IDataReader)
  End Function

  Public Overrides Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLanguagePacks", ObjectId, Version), IDataReader)
  End Function

  Public Overrides Function GetLanguagePacksByObjectVersionLocale(ByVal objectId As Integer, ByVal version As String, ByVal locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLanguagePacksByObjectVersionLocale", objectId, version, locale), IDataReader)
  End Function

  Public Overrides Function GetFrameworkVersion() As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetFrameworkVersion"), IDataReader)
  End Function

  Public Overrides Function GetModuleForObject(ByVal ObjectId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetModuleForObject", ObjectId), IDataReader)
  End Function

  Public Overrides Function GetLastEditTime(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As System.Data.IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLastEditTime", ObjectId, Locale, Version), IDataReader)
  End Function

  Public Overrides Function GetCube(ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetCube", ModuleId), IDataReader)
  End Function

#End Region

 End Class
End Namespace

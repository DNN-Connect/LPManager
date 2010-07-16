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
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Common.Utilities

Namespace Data
 Public Class SqlDataProvider
  Inherits DataProvider

#Region " Private Members "

  Private Const ProviderType As String = "data"
  Private Const ModuleQualifier As String = "LocalizationEditor_"

  Private _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType)

  Private _connectionString As String
  Private _providerPath As String
  Private _objectQualifier As String
  Private _databaseOwner As String

#End Region

#Region " Constructors "

  Public Sub New()

   ' Read the configuration specific information for this provider
   Dim objProvider As Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

   ' This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists. 
   ' Get Connection string from web.config
   _connectionString = Config.GetConnectionString()

   ' If above funtion does not return anything then connectionString must be set in the dataprovider section.
   If _connectionString = "" Then
    ' Use connection string specified in provider
    _connectionString = objProvider.Attributes("connectionString")
   End If

   _providerPath = objProvider.Attributes("providerPath")

   _objectQualifier = objProvider.Attributes("objectQualifier")
   If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
    _objectQualifier += "_"
   End If

   _databaseOwner = objProvider.Attributes("databaseOwner")
   If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
    _databaseOwner += "."
   End If

  End Sub

#End Region

#Region " Properties "

  Public ReadOnly Property ConnectionString() As String
   Get
    Return _connectionString
   End Get
  End Property

  Public ReadOnly Property ProviderPath() As String
   Get
    Return _providerPath
   End Get
  End Property

  Public ReadOnly Property ObjectQualifier() As String
   Get
    Return _objectQualifier
   End Get
  End Property

  Public ReadOnly Property DatabaseOwner() As String
   Get
    Return _databaseOwner
   End Get
  End Property

#End Region

#Region " General Methods "

  Public Overrides Function GetNull(ByVal Field As Object) As Object
   Return Null.GetNull(Field, DBNull.Value)
  End Function

#End Region

#Region " Permissions Methods "

  Public Overrides Function GetPermission(ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPermission", UserId, Locale, ModuleId), IDataReader)
  End Function

  Public Overrides Function GetPermissions(ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPermissions", ModuleId), IDataReader)
  End Function

  Public Overrides Function AddPermission(ByVal Locale As String, ByVal ModuleId As Int32, ByVal UserId As Int32) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddPermission", Locale, ModuleId, UserId), Integer)
  End Function

  Public Overrides Sub UpdatePermission(ByVal PermissionId As Int32, ByVal Locale As String, ByVal ModuleId As Int32, ByVal UserId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdatePermission", PermissionId, Locale, ModuleId, UserId)
  End Sub

  Public Overrides Sub DeletePermission(ByVal PermissionId As Integer)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeletePermission", PermissionId)
  End Sub

#End Region

#Region " Objects Methods "

  Public Overrides Function GetObject(ByVal ObjectId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObject", ObjectId), IDataReader)
  End Function

  Public Overrides Function GetObjectByObjectName(ByVal ObjectName As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectByObjectName", ObjectName), IDataReader)
  End Function

  Public Overrides Function GetObjectList(ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectList", ModuleId), IDataReader)
  End Function

  Public Overrides Function AddObject(ByVal ObjectName As String, ByVal FriendlyName As String, ByVal InstallPath As String, ByVal ModuleId As Integer, ByVal PackageType As String, ByVal IsCoreObject As Boolean) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddObject", ObjectName, FriendlyName, InstallPath, ModuleId, PackageType, IsCoreObject), Integer)
  End Function

  Public Overrides Sub DeleteObject(ByVal ObjectId As Integer)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteObject", ObjectId)
  End Sub

#End Region

#Region " Statistics Methods "

  Public Overrides Function GetStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetStatistic", UserId, TranslationId), IDataReader)
  End Function

  Public Overrides Sub AddStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32, ByVal Total As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddStatistic", UserId, TranslationId, Total)
  End Sub

  Public Overrides Sub UpdateStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32, ByVal Total As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateStatistic", UserId, TranslationId, Total)
  End Sub

  Public Overrides Sub DeleteStatistic(ByVal UserId As Int32, ByVal TranslationId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteStatistic", UserId, TranslationId)
  End Sub

  Public Overrides Function GetStatisticsByTranslation(ByVal TranslationId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetStatisticsByTranslation", TranslationId), IDataReader)
  End Function

  Public Overrides Function GetStatisticsByUser(ByVal UserID As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetStatisticsByUser", UserID), IDataReader)
  End Function

  Public Overrides Function GetPackStatistics(ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPackStatistics", ObjectId, Version, Locale), IDataReader)
  End Function
#End Region

#Region " Texts Methods "

  Public Overrides Function GetText(ByVal TextId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetText", TextId), IDataReader)
  End Function

  Public Overrides Function AddText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddText", GetNull(DeprecatedIn), GetNull(FilePath), ObjectId, GetNull(OriginalValue), GetNull(TextKey), GetNull(Version)), Integer)
  End Function

  Public Overrides Sub UpdateText(ByVal TextId As Integer, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateText", TextId, GetNull(DeprecatedIn), GetNull(FilePath), ObjectId, GetNull(OriginalValue), GetNull(TextKey), GetNull(Version))
  End Sub

  Public Overrides Sub DeleteText(ByVal TextId As Integer)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteText", TextId)
  End Sub

  Public Overrides Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "FindText", ObjectId, FilePath, Locale, Version, TextKey), IDataReader)
  End Function

  Public Overrides Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLatestText", ObjectId, FilePath, Locale, TextKey), IDataReader)
  End Function

  Public Overrides Function GetTextsByObject(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTextsByObject", ObjectId, Locale, Version), IDataReader)
  End Function

  Public Overrides Function GetTextsByObjectAndFile(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTextsByObjectAndFile", ObjectId, FilePath, Locale, Version, IncludeNonTranslated), IDataReader)
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

  Public Overrides Function GetTranslation(ByVal TranslationId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslationById", TranslationId), IDataReader)
  End Function

  Public Overrides Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslation", TextId, Locale), IDataReader)
  End Function

  Public Overrides Function AddTranslation(ByVal LastModified As Date, ByVal LastModifiedUserId As Int32, ByVal Locale As String, ByVal TextId As Int32, ByVal TextValue As String) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddTranslation", GetNull(LastModified), GetNull(LastModifiedUserId), Locale, TextId, GetNull(TextValue)), Integer)
  End Function

  Public Overrides Sub UpdateTranslation(ByVal TranslationId As Int32, ByVal LastModified As Date, ByVal LastModifiedUserId As Int32, ByVal Locale As String, ByVal TextId As Int32, ByVal TextValue As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateTranslation", TranslationId, GetNull(LastModified), GetNull(LastModifiedUserId), Locale, TextId, GetNull(TextValue))
  End Sub

  Public Overrides Sub DeleteTranslation(ByVal TranslationId As Integer)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteTranslation", TranslationId)
  End Sub

  Public Overrides Function GetTranslationsByText(ByVal TextId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslationsByText", TextId), IDataReader)
  End Function

#End Region

#Region " User Methods "

  Public Overrides Function GetUsersFiltered(ByVal filter As String) As DataSet
   Return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUsersFiltered", filter)
  End Function

#End Region

#Region " Other Procedures "

  Public Overrides Function GetAllObjects() As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetAllObjects"), IDataReader)
  End Function

  Public Overrides Function GetUsedObjects() As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUsedObjects"), IDataReader)
  End Function

  Public Overrides Function GetObjectsForUser(ByVal UserID As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObjectsForUser", UserID, PortalId, ModuleId), IDataReader)
  End Function

  Public Overrides Function GetLocalesForUserObject(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLocalesForUserObject", ObjectId, UserId, PortalId, ModuleId), IDataReader)
  End Function

  Public Overrides Function GetAvailableLanguagePacks() As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetAvailableLanguagePacks"), IDataReader)
  End Function

  Public Overrides Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetLanguagePacks", ObjectId, Version), IDataReader)
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

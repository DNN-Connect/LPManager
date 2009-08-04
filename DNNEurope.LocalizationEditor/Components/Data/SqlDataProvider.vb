Imports System
Imports System.Data
Imports DotNetNuke.Common.Utilities
Imports Microsoft.ApplicationBlocks.Data

Namespace DNNEurope.Modules.LocalizationEditor.Data

	Public Class SqlDataProvider
		Inherits DataProvider

#Region " Private Members "
        Private Const ProviderType As String = "data"

        Private _providerConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String
#End Region

#Region " Constructors "
        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)

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
			Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
		End Function
#End Region

#Region " Permissions Methods "
        Public Overrides Function GetPermission(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetPermission", ObjectId, UserId, Locale, ModuleId), IDataReader)
        End Function

        Public Overrides Function GetPermissions(ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetPermissions", ModuleId), IDataReader)
        End Function

        Public Overrides Sub AddPermission(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_AddPermission", ObjectId, UserId, Locale, GetNull(ModuleId))
        End Sub

        Public Overrides Sub UpdatePermission(ByVal PermissionId As Integer, ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_UpdatePermission", PermissionId, ObjectId, UserId, Locale, GetNull(ModuleId))
        End Sub

        Public Overrides Sub DeletePermission(ByVal PermissionId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_DeletePermission", PermissionId)
        End Sub
#End Region

#Region " Objects Methods "
        Public Overrides Function GetObject(ByVal ObjectId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetObject", ObjectId), IDataReader)
        End Function

        Public Overrides Function GetObjectByObjectName(ByVal ObjectName As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetObjectByObjectName", ObjectName), IDataReader)
        End Function

        Public Overrides Function GetObjectList() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetObjectList"), IDataReader)
        End Function

        Public Overrides Function AddObject(ByVal ObjectName As String, ByVal FriendlyName As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_AddObject", ObjectName, FriendlyName), Integer)
        End Function

        Public Overrides Sub DeleteObject(ByVal ObjectId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_DeleteObject", ObjectId)
        End Sub
#End Region

#Region " Texts Methods "
        Public Overrides Function GetText(ByVal TextId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetText", TextId), IDataReader)
        End Function

        Public Overrides Function AddText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_AddText", GetNull(DeprecatedIn), GetNull(FilePath), ObjectId, GetNull(OriginalValue), GetNull(TextKey), GetNull(Version)), Integer)
        End Function

        Public Overrides Sub UpdateText(ByVal TextId As Integer, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_UpdateText", TextId, GetNull(DeprecatedIn), GetNull(FilePath), ObjectId, GetNull(OriginalValue), GetNull(TextKey), GetNull(Version))
        End Sub

        Public Overrides Sub DeleteText(ByVal TextId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_DeleteText", TextId)
        End Sub

        Public Overrides Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_FindText", ObjectId, FilePath, Locale, Version, TextKey), IDataReader)
        End Function

        Public Overrides Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetLatestText", ObjectId, FilePath, Locale, TextKey), IDataReader)
        End Function

        Public Overrides Function GetTextsByObject(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTextsByObject", ObjectId, Locale, Version), IDataReader)
        End Function

        Public Overrides Function GetTextsByObjectAndFile(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTextsByObjectAndFile", ObjectId, FilePath, Locale, Version, IncludeNonTranslated), IDataReader)
        End Function

        Public Overrides Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_CurrentVersion", ObjectId, Locale), IDataReader)
        End Function

        Public Overrides Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_NrOfChangedTexts", ObjectId, Version), IDataReader)
        End Function

        Public Overrides Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_NrOfFiles", ObjectId, Version), IDataReader)
        End Function

        Public Overrides Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_NrOfItems", ObjectId, Version), IDataReader)
        End Function

        Public Overrides Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_NrOfMissingTranslations", ObjectId, Locale, Version), IDataReader)
        End Function

        Public Overrides Function GetFiles(ByVal ObjectId As Integer, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetFiles", ObjectId, Version), IDataReader)
        End Function

        Public Overrides Function GetVersions(ByVal ObjectId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetVersions", ObjectId), IDataReader)
        End Function

        Public Overrides Function GetTranslationList(ByVal ObjectId As Integer, ByVal Locale As String, ByVal SourceLocale As String, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTranslationList", ObjectId, Locale, SourceLocale, Version), IDataReader)
        End Function
#End Region

#Region " Translations Methods "
        Public Overrides Function GetTranslation(ByVal TranslationId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTranslationById", TranslationId), IDataReader)
        End Function

        Public Overrides Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTranslation", TextId, Locale), IDataReader)
        End Function

        Public Overrides Sub AddTranslation(ByVal TextId As Integer, ByVal Locale As String, ByVal LastModified As Date, ByVal LastModifiedUserId As Integer, ByVal TextValue As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_AddTranslation", TextId, Locale, GetNull(LastModified), GetNull(LastModifiedUserId), GetNull(TextValue))
        End Sub

        Public Overrides Sub UpdateTranslation(ByVal TranslationId As Integer, ByVal TextId As Integer, ByVal Locale As String, ByVal LastModified As Date, ByVal LastModifiedUserId As Integer, ByVal TextValue As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_UpdateTranslation", TranslationId, TextId, Locale, GetNull(LastModified), GetNull(LastModifiedUserId), GetNull(TextValue))
        End Sub

        Public Overrides Sub DeleteTranslation(ByVal TranslationId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_DeleteTranslation", TranslationId)
        End Sub

        Public Overrides Function GetTranslationsByText(ByVal TextId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetTranslationsByText", TextId), IDataReader)
        End Function
#End Region

#Region " User Methods "
        Public Overrides Function GetUsersFiltered(ByVal filter As String) As DataSet
            Return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetUsersFiltered", filter)
        End Function
#End Region

#Region " Other Procedures "
        Public Overrides Function GetAllObjects() As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetAllObjects"), IDataReader)
        End Function

        Public Overrides Function GetUsedObjects() As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetUsedObjects"), IDataReader)
        End Function

        Public Overrides Function GetObjectsForUser(ByVal UserID As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetObjectsForUser", UserID, PortalId, ModuleId), IDataReader)
        End Function

        Public Overrides Function GetLocalesForUserObject(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetLocalesForUserObject", ObjectId, UserId, PortalId, ModuleId), IDataReader)
        End Function

        Public Overrides Function GetAvailableLanguagePacks() As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetAvailableLanguagePacks"), IDataReader)
        End Function

        Public Overrides Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetLanguagePacks", ObjectId, Version), IDataReader)
        End Function

        Public Overrides Function GetFrameworkVersion() As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "LocalizationEditor_GetFrameworkVersion"), IDataReader)
        End Function
#End Region

    End Class

End Namespace

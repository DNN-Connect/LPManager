Imports DotNetNuke

Namespace DNNEurope.Modules.LocalizationEditor.Data

	Public MustInherit Class DataProvider

#Region " Shared/Static Methods "
        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DNNEurope.Modules.LocalizationEditor.Data", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function
#End Region

#Region " General Methods "
		Public MustOverride Function GetNull(ByVal Field As Object) As Object
#End Region

#Region " Permission Methods "
        Public MustOverride Function GetPermission(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function GetPermissions(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Sub AddPermission(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer)
        Public MustOverride Sub UpdatePermission(ByVal PermissionId As Integer, ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer)
        Public MustOverride Sub DeletePermission(ByVal PermissionId As Integer)
#End Region

#Region " Translate Module Methods "
        Public MustOverride Function GetObject(ByVal ObjectId As Integer) As IDataReader
        Public MustOverride Function GetObjectByObjectName(ByVal ObjectName As String) As IDataReader
        Public MustOverride Function GetObjectList() As IDataReader
        Public MustOverride Function AddObject(ByVal ObjectName As String, ByVal FriendlyName As String) As Integer
        Public MustOverride Sub DeleteObject(ByVal ObjectId As Integer)
#End Region

#Region " Text Methods "
        Public MustOverride Function GetText(ByVal TextId As Integer) As IDataReader
        Public MustOverride Function AddText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String) As Integer
        Public MustOverride Sub UpdateText(ByVal TextId As Integer, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)
        Public MustOverride Sub DeleteText(ByVal TextId As Integer)

        Public MustOverride Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader
        Public MustOverride Function GetTextsByObject(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
        Public MustOverride Function GetTextsByObjectAndFile(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
        Public MustOverride Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As IDataReader
        Public MustOverride Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
        Public MustOverride Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
        Public MustOverride Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As IDataReader
        Public MustOverride Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
        Public MustOverride Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
        Public MustOverride Function GetVersions(ByVal ObjectId As Integer) As IDataReader
        Public MustOverride Function GetFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
        Public MustOverride Function GetTranslationList(ByVal ObjectId As Integer, ByVal Locale As String, ByVal SourceLocale As String, ByVal Version As String) As IDataReader
#End Region

#Region " Translation Methods "
        Public MustOverride Function GetTranslation(ByVal TranslationId As Integer) As IDataReader
        Public MustOverride Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As IDataReader
        Public MustOverride Sub AddTranslation(ByVal TextId As Integer, ByVal Locale As String, ByVal LastModified As Date, ByVal LastModifiedUserId As Integer, ByVal TextValue As String)
        Public MustOverride Sub UpdateTranslation(ByVal TranslationId As Integer, ByVal TextId As Integer, ByVal Locale As String, ByVal LastModified As Date, ByVal LastModifiedUserId As Integer, ByVal TextValue As String)
        Public MustOverride Sub DeleteTranslation(ByVal TranslationId As Integer)
        Public MustOverride Function GetTranslationsByText(ByVal TextId As Integer) As IDataReader
#End Region

#Region " User Methods "
        Public MustOverride Function GetUsersFiltered(ByVal filter As String) As DataSet
#End Region

#Region " Other Procedures "
        Public MustOverride Function GetAllObjects() As IDataReader
        Public MustOverride Function GetUsedObjects() As IDataReader
        Public MustOverride Function GetObjectsForUser(ByVal UserID As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function GetLocalesForUserObject(ByVal ObjectId As Integer, ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function GetAvailableLanguagePacks() As IDataReader
        Public MustOverride Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
        Public MustOverride Function GetFrameworkVersion() As IDataReader
#End Region

	End Class

End Namespace

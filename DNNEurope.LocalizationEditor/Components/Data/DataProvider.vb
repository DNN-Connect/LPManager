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

Imports DotNetNuke.Framework

Namespace Data
 Partial Public MustInherit Class DataProvider

#Region " Shared/Static Methods "

  ' singleton reference to the instantiated object 
  Private Shared objProvider As DataProvider = Nothing

  ' constructor
  Shared Sub New()
   CreateProvider()
  End Sub

  ' dynamically create provider
  Private Shared Sub CreateProvider()
   objProvider = CType(DotNetNuke.Framework.Reflection.CreateObject("data", "DNNEurope.Modules.LocalizationEditor.Data", ""), DataProvider)
  End Sub

  ' return the provider
  Public Shared Shadows Function Instance() As DataProvider
   Return objProvider
  End Function

#End Region

#Region " General Methods "
  Public MustOverride Function GetNull(ByVal Field As Object) As Object
#End Region

#Region " ObjectCoreVersion Methods "
  Public MustOverride Sub SetObjectCoreVersion(ObjectId As Int32, Version As String, DnnVersion As String, InstalledByDefault As Boolean)
  Public MustOverride Function GetCoreObjects(ByVal DnnVersion As String, GetAllObjects As Boolean) As IDataReader
#End Region


#Region " Permission Methods "

  Public MustOverride Function GetPermission(ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader

  Public MustOverride Function GetPermissions(ByVal ModuleId As Integer) As IDataReader

  Public MustOverride Function AddPermission(ByVal Locale As String, ByVal ModuleId As Int32, ByVal UserId As Int32) As Integer

  Public MustOverride Sub UpdatePermission(ByVal PermissionId As Int32, ByVal Locale As String, ByVal ModuleId As Int32, ByVal UserId As Int32)

  Public MustOverride Sub DeletePermission(ByVal PermissionId As Integer)

#End Region

#Region " Object Methods "

  Public MustOverride Function GetObject(ByVal ObjectId As Integer) As IDataReader
  Public MustOverride Function GetObjectByObjectName(ModuleId As Integer, ByVal ObjectName As String) As IDataReader
  Public MustOverride Function GetObjectsByObjectName(ByVal ObjectName As String) As IDataReader
  Public MustOverride Function GetObjectList(ByVal ModuleId As Integer) As IDataReader
  Public MustOverride Function AddObject(ByVal ObjectName As String, ByVal FriendlyName As String, ByVal InstallPath As String, ByVal ModuleId As Integer, ByVal PackageType As String) As Integer
  Public MustOverride Sub DeleteObject(ByVal ObjectId As Integer)

#End Region

#Region " Statistic Methods "

  Public MustOverride Function GetStatistic(ByVal TextId As Int32, ByVal Locale As String, ByVal UserId As Int32) As IDataReader
  Public MustOverride Sub SetStatistic(ByVal Locale As String, ByVal TextId As Int32, ByVal Total As Int32, ByVal UserId As Int32)
  Public MustOverride Sub DeleteStatistic(ByVal TextId As Int32, ByVal Locale As String, ByVal UserId As Int32)
  Public MustOverride Function GetStatisticsByUser(ByVal UserID As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader

  Public MustOverride Function GetPackStatistics(ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As IDataReader

#End Region

#Region " Text Methods "

  Public MustOverride Function GetText(ByVal TextId As Integer) As IDataReader

  Public MustOverride Function AddText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String) As Integer

  Public MustOverride Sub UpdateText(ByVal TextId As Integer, ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Integer, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String)

  Public MustOverride Sub DeleteText(ByVal TextId As Integer)

  Public MustOverride Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader

  Public MustOverride Function GetTextsByObject(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader

  Public MustOverride Function GetTextsByObjectAndFile(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader

  Public MustOverride Function GetAdjacentTextsForCore(ByVal ModuleId As Integer, ByVal CoreObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader

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

  Public MustOverride Function GetTranslation(ByVal TextId As Integer, ByVal Locale As String) As IDataReader

  Public MustOverride Function SetTranslation(ByVal LastModified As Date, ByVal LastModifiedUserId As Int32, ByVal Locale As String, ByVal TextId As Int32, ByVal TextValue As String) As Integer

  Public MustOverride Sub DeleteTranslation(ByVal TextId As Int32, ByVal Locale As String)
  Public MustOverride Function GetTranslationsByText(ByVal TextId As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader

#End Region

#Region " User Methods "

  Public MustOverride Function GetUsersFiltered(ByVal filter As String) As DataSet

#End Region

#Region " Other Procedures "

  Public MustOverride Function GetAllObjects() As IDataReader
  Public MustOverride Function GetUsedObjects() As IDataReader

  Public MustOverride Function GetObjects(ByVal ModuleId As Integer) As IDataReader

  Public MustOverride Function GetLocalesForUser(ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader

  Public MustOverride Function GetAvailableLanguagePacks() As IDataReader

  Public MustOverride Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader

  Public MustOverride Function GetFrameworkVersion() As IDataReader

  Public MustOverride Function GetModuleForObject(ByVal ObjectId As Integer) As IDataReader

  Public MustOverride Function GetLastEditTime(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader

  Public MustOverride Function GetCube(ByVal ModuleId As Integer) As IDataReader

#End Region

 End Class
End Namespace

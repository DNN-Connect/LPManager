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

Namespace Data
 Partial Public MustInherit Class DataProvider

#Region " Object Methods "

  Public MustOverride Function GetObjects(ByVal moduleId As Integer) As IDataReader
  Public MustOverride Function GetObjectByObjectNameAndModuleKey(ByVal portalId As Integer, ByVal objectName As String, ByVal moduleKey As String) As IDataReader
  Public MustOverride Function GetObjectByObjectName(moduleId As Integer, ByVal objectName As String) As IDataReader
  Public MustOverride Function GetObjectsByObjectName(ByVal objectName As String) As IDataReader
  Public MustOverride Function GetObjectsWithStatus(ByVal moduleId As Integer, locale As String) As IDataReader

#End Region

#Region " Packages Methods "

  Public MustOverride Function GetObjectsByPackage(ByVal packageObjectId As Integer, ByVal version As String) As IDataReader
  Public MustOverride Sub RegisterPackageItem(ByVal ParentObjectId As Integer, ByVal ParentVersion As String, ByVal ChildObjectId As Integer, ByVal ChildVersion As String)
  Public MustOverride Function GetParentObjects(ByVal objectId As Integer, ByVal version As String) As IDataReader

#End Region

#Region " Permission Methods "
  Public MustOverride Function GetPermission(ByVal UserId As Integer, ByVal Locale As String, ByVal ModuleId As Integer) As IDataReader
  Public MustOverride Function GetPermissions(ByVal ModuleId As Integer) As IDataReader
#End Region

#Region " Statistic Methods "
  Public MustOverride Sub SetStatistic(ByVal Locale As String, ByVal TextId As Int32, ByVal Total As Int32, ByVal UserId As Int32)
  Public MustOverride Function GetPackStatistics(ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As IDataReader
#End Region

#Region " Text Methods "
  Public MustOverride Function GetText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal TextKey As String) As IDataReader
  Public MustOverride Function GetTextsByObject(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
  Public MustOverride Function GetTextsByObjectAndFile(ByVal ModuleId As Integer, ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
  Public MustOverride Function GetDependentTextsForObject(ByVal ModuleId As Integer, ByVal ParentObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal Version As String, ByVal IncludeNonTranslated As Boolean) As IDataReader
  Public MustOverride Function GetLatestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal Locale As String, ByVal TextKey As String) As IDataReader
  Public MustOverride Function NrOfFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
  Public MustOverride Function NrOfItems(ByVal ObjectId As Integer, ByVal Version As String) As Integer
  Public MustOverride Function CurrentVersion(ByVal ObjectId As Integer, ByVal Locale As String) As IDataReader
  Public MustOverride Function NrOfChangedTexts(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
  Public MustOverride Function NrOfMissingTranslations(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
  Public MustOverride Function GetVersions(ByVal ObjectId As Integer) As IDataReader
  Public MustOverride Function GetFiles(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
  Public MustOverride Function GetTranslationList(ByVal ObjectId As Integer, ByVal Locale As String, ByVal SourceLocale As String, ByVal Version As String) As IDataReader
  Public MustOverride Function GetTextByVersion(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal TextKey As String, ByVal Version As String) As IDataReader
  Public MustOverride Function GetOldestText(ByVal ObjectId As Integer, ByVal FilePath As String, ByVal TextKey As String) As IDataReader
#End Region

#Region " Translation Methods "
  Public MustOverride Function SetTranslation(ByVal lastModified As Date, ByVal lastModifiedUserId As Int32, ByVal locale As String, ByVal textId As Int32, ByVal textValue As String) As Integer
#End Region

#Region " Partner Methods "
  Public MustOverride Function GetPartnerByName(ByVal moduleId As Integer, ByVal name As String) As IDataReader
  Public MustOverride Function GetAllPartners() As IDataReader
#End Region

#Region " Other Procedures "
  Public MustOverride Function GetUsersFiltered(ByVal filter As String) As DataSet
  Public MustOverride Function GetLocales(ByVal moduleId As Integer) As IDataReader
  Public MustOverride Function GetLocalesForUser(ByVal UserId As Integer, ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
  Public MustOverride Function GetLanguagePacks(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
  Public MustOverride Function GetLanguagePacksByObjectVersionLocale(ByVal objectId As Integer, ByVal version As String, ByVal locale As String) As IDataReader
  Public MustOverride Function GetFrameworkVersion() As IDataReader
  Public MustOverride Function GetModuleForObject(ByVal ObjectId As Integer) As IDataReader
  Public MustOverride Function GetLastEditTime(ByVal ObjectId As Integer, ByVal Locale As String, ByVal Version As String) As IDataReader
  Public MustOverride Function GetCube(ByVal ModuleId As Integer) As IDataReader
  Public MustOverride Function GetObjectVersionList(ByVal ObjectId As Integer, ByVal Locale As String) As IDataReader
  Public MustOverride Function GetObjectPackList(ByVal ObjectId As Integer, ByVal Version As String) As IDataReader
  Public MustOverride Function GetContributorList(ByVal ObjectId As Integer, ByVal Version As String, locale As String) As IDataReader
#End Region

 End Class
End Namespace

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
Imports System
Imports System.Data
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities

Namespace Data

 Partial Public Class SqlDataProvider
  Inherits DataProvider

#Region " Private Members "

  Private Const ProviderType As String = "data"
  Private Const ModuleQualifier As String = "LocalizationEditor_"

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
   'Get Connection string from web.config
   _connectionString = Config.GetConnectionString()
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
  Public Overrides Function GetNull(ByVal field As Object) As Object
   Return DotNetNuke.Common.Utilities.Null.GetNull(field, DBNull.Value)
  End Function
#End Region



#Region " ObjectCoreVersion Methods "
#End Region


#Region " Object Methods "

  Public Overrides Function GetObject(ByVal ObjectId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetObject", ObjectId), IDataReader)
  End Function

  Public Overrides Function AddObject(ByVal FriendlyName As String, ByVal ModuleId As Int32, ByVal InstallPath As String, ByVal ObjectName As String, ByVal PackageType As String) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddObject", FriendlyName, ModuleId, InstallPath, ObjectName, PackageType), Integer)
  End Function

  Public Overrides Sub DeleteObject(ByVal ObjectId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteObject", ObjectId)
  End Sub

#End Region


#Region " PartnerPack Methods "

  Public Overrides Function GetPartnerPack(ByVal PartnerId As Int32, ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartnerPack", PartnerId, ObjectId, Version, Locale), IDataReader)
  End Function

  Public Overrides Sub AddPartnerPack(ByVal LastModified As Date, ByVal Locale As String, ByVal ObjectId As Int32, ByVal PartnerId As Int32, ByVal PercentComplete As Single, ByVal RemoteObjectId As Int32, ByVal Version As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddPartnerPack", LastModified, Locale, ObjectId, PartnerId, PercentComplete, RemoteObjectId, Version)
  End Sub

  Public Overrides Sub UpdatePartnerPack(ByVal LastModified As Date, ByVal Locale As String, ByVal ObjectId As Int32, ByVal PartnerId As Int32, ByVal PercentComplete As Single, ByVal RemoteObjectId As Int32, ByVal Version As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdatePartnerPack", LastModified, Locale, ObjectId, PartnerId, PercentComplete, RemoteObjectId, Version)
  End Sub

  Public Overrides Sub DeletePartnerPack(ByVal PartnerId As Int32, ByVal ObjectId As Int32, ByVal Version As String, ByVal Locale As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeletePartnerPack", PartnerId, ObjectId, Version, Locale)
  End Sub

#End Region


#Region " Partner Methods "

  Public Overrides Function GetPartner(ByVal PartnerId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartner", PartnerId), IDataReader)
  End Function

	Public Overrides Function AddPartner(ByVal AllowDirectDownload As Boolean, ByVal ModuleId As Int32, ByVal AllowRedistribute As Boolean, ByVal CubeUrl As String, ByVal DownloadAffiliates As Boolean, ByVal LastCube As String, ByVal PackUrl As String, ByVal PartnerName As String, ByVal PartnerUrl As String, ByVal PartnerUsername As String) As Integer
		Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddPartner", AllowDirectDownload, ModuleId, AllowRedistribute, CubeUrl, DownloadAffiliates, GetNull(LastCube), GetNull(PackUrl), PartnerName, GetNull(PartnerUrl), GetNull(PartnerUsername)), Integer)
  End Function

	Public Overrides Sub UpdatePartner(ByVal AllowDirectDownload As Boolean, ByVal ModuleId As Int32, ByVal AllowRedistribute As Boolean, ByVal CubeUrl As String, ByVal DownloadAffiliates As Boolean, ByVal LastCube As String, ByVal PackUrl As String, ByVal PartnerId As Int32, ByVal PartnerName As String, ByVal PartnerUrl As String, ByVal PartnerUsername As String)
		SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdatePartner", AllowDirectDownload, ModuleId, AllowRedistribute, CubeUrl, DownloadAffiliates, GetNull(LastCube), GetNull(PackUrl), PartnerId, PartnerName, GetNull(PartnerUrl), GetNull(PartnerUsername))
  End Sub

  Public Overrides Sub DeletePartner(ByVal PartnerId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeletePartner", PartnerId)
  End Sub

#End Region


#Region " Permission Methods "

  Public Overrides Function AddPermission(ByVal ModuleId As Int32, ByVal Locale As String, ByVal UserId As Int32) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddPermission", ModuleId, Locale, UserId), Integer)
  End Function

  Public Overrides Sub DeletePermission(ByVal PermissionId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeletePermission", PermissionId)
  End Sub

#End Region


#Region " Statistic Methods "

  Public Overrides Function GetStatistic(ByVal TextId As Int32, ByVal Locale As String, ByVal UserId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetStatistic", TextId, Locale, UserId), IDataReader)
  End Function

  Public Overrides Sub DeleteStatistic(ByVal TextId As Int32, ByVal Locale As String, ByVal UserId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteStatistic", TextId, Locale, UserId)
  End Sub

#End Region


#Region " Text Methods "

  Public Overrides Function GetText(ByVal TextId As Int32) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetText", TextId), IDataReader)
  End Function

  Public Overrides Function AddText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Int32, ByVal OriginalValue As String, ByVal TextKey As String, ByVal Version As String) As Integer
   Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddText", GetNull(DeprecatedIn), FilePath, ObjectId, GetNull(OriginalValue), TextKey, Version), Integer)
  End Function

  Public Overrides Sub UpdateText(ByVal DeprecatedIn As String, ByVal FilePath As String, ByVal ObjectId As Int32, ByVal OriginalValue As String, ByVal TextId As Int32, ByVal TextKey As String, ByVal Version As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateText", GetNull(DeprecatedIn), FilePath, ObjectId, GetNull(OriginalValue), TextId, TextKey, Version)
  End Sub

  Public Overrides Sub DeleteText(ByVal TextId As Int32)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteText", TextId)
  End Sub

#End Region


#Region " Translation Methods "

  Public Overrides Function GetTranslation(ByVal TextId As Int32, ByVal Locale As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslation", TextId, Locale), IDataReader)
  End Function

  Public Overrides Sub DeleteTranslation(ByVal TextId As Int32, ByVal Locale As String)
   SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteTranslation", TextId, Locale)
  End Sub

#End Region


 End Class

End Namespace

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

Namespace Data

 Partial Public Class SqlDataProvider

#Region " ObjectCoreVersion Methods "
#End Region

#Region " Object Methods "
#End Region

#Region " PartnerPack Methods "
	Public Overrides Function GetPartnerPacksByObject(ByVal ObjectId As Int32 , StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
		Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartnerPacksByObject", ObjectId, StartRowIndex, MaximumRows, OrderBy.ToUpper), IDataReader)
	End Function

	Public Overrides Function GetPartnerPacksByPartner(ByVal PartnerId As Int32 , StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
		Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartnerPacksByPartner", PartnerId, StartRowIndex, MaximumRows, OrderBy.ToUpper), IDataReader)
	End Function

#End Region

#Region " Partner Methods "
	Public Overrides Function GetPartnersByModule(ByVal ModuleID As Int32 , StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
		Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetPartnersByModule", ModuleID, StartRowIndex, MaximumRows, OrderBy.ToUpper), IDataReader)
	End Function

#End Region

#Region " Permission Methods "
#End Region

#Region " Statistic Methods "

  Public Overrides Function GetStatisticsByUser(ByVal UserID As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetStatisticsByUser", UserID, StartRowIndex, MaximumRows, OrderBy.ToUpper), IDataReader)
  End Function

#End Region

#Region " Text Methods "
#End Region

#Region " Translation Methods "

  Public Overrides Function GetTranslationsByText(ByVal TextId As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As IDataReader
   Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetTranslationsByText", TextId, StartRowIndex, MaximumRows, OrderBy.ToUpper), IDataReader)
  End Function

#End Region

 End Class

End Namespace

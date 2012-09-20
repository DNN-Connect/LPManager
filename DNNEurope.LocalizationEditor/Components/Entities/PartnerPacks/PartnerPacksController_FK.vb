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
Imports DNNEurope.Modules.LocalizationEditor.Data

Namespace Entities.PartnerPacks

 Partial Public Class PartnerPacksController

  Public Shared Function GetPartnerPacksByObject(ByVal ObjectId As Int32) As List(Of PartnerPackInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PartnerPackInfo)(DataProvider.Instance().GetPartnerPacksByObject(ObjectId, 0, 1, ""))

  End Function

  Public Shared Function GetPartnerPacksByObject(ByVal ObjectId As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As List(Of PartnerPackInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PartnerPackInfo)(DataProvider.Instance().GetPartnerPacksByObject(ObjectId, StartRowIndex, MaximumRows, OrderBy))

  End Function

  Public Shared Function GetPartnerPacksByPartner(ByVal PartnerId As Int32) As List(Of PartnerPackInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PartnerPackInfo)(DataProvider.Instance().GetPartnerPacksByPartner(PartnerId, 0, 1, ""))

  End Function

  Public Shared Function GetPartnerPacksByPartner(ByVal PartnerId As Int32, StartRowIndex As Integer, MaximumRows As Integer, OrderBy As String) As List(Of PartnerPackInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of PartnerPackInfo)(DataProvider.Instance().GetPartnerPacksByPartner(PartnerId, StartRowIndex, MaximumRows, OrderBy))

  End Function


 End Class
End Namespace


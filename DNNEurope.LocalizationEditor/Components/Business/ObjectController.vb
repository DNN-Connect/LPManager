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

Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Common.Utilities
Imports System.Collections.Generic

Namespace Business

 Public Class ObjectController
  Public Shared Function GetObject(ByVal ObjectId As Integer) As ObjectInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetObject(ObjectId), GetType(ObjectInfo)), ObjectInfo)
  End Function

  Public Shared Function GetObjectByObjectName(ModuleId As Integer, ByVal ObjectName As String) As ObjectInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetObjectByObjectName(ModuleId, ObjectName), GetType(ObjectInfo)), ObjectInfo)
  End Function

  Public Shared Function GetObjectsByObjectName(ByVal ObjectName As String) As List(Of ObjectInfo)
   Return CBO.FillCollection(Of ObjectInfo)(DataProvider.Instance().GetObjectsByObjectName(ObjectName))
  End Function

  Public Shared Function GetObjectList(ByVal ModuleId As Integer) As ArrayList
   Return CBO.FillCollection(DataProvider.Instance().GetObjectList(ModuleId), GetType(ObjectInfo))
  End Function

  Public Shared Function AddObject(ByVal objObject As ObjectInfo) As Integer
   Return CType(DataProvider.Instance().AddObject(objObject.ObjectName, objObject.FriendlyName, objObject.InstallPath, objObject.ModuleId, objObject.PackageType), Integer)
  End Function

  Public Shared Sub DeleteObject(ByVal ObjectId As Integer)
   DataProvider.Instance().DeleteObject(ObjectId)
  End Sub
 End Class

End Namespace

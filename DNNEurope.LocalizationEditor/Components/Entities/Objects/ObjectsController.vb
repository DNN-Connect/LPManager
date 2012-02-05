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

Imports DNNEurope.Modules.LocalizationEditor.Data
Imports DotNetNuke.Common.Utilities
Imports System.Collections.Generic
Imports DNNEurope.Modules.LocalizationEditor.Entities.Texts

Namespace Entities.Objects

 Public Class ObjectsController
  Public Shared Function GetObjectByObjectNameAndModuleKey(ByVal portalId As Integer, ByVal objectName As String, ByVal moduleKey As String) As ObjectInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetObjectByObjectNameAndModuleKey(portalId, objectName, moduleKey), GetType(ObjectInfo)), ObjectInfo)
  End Function

  Public Shared Function GetObjectByObjectName(ModuleId As Integer, ByVal ObjectName As String) As ObjectInfo
   Return CType(CBO.FillObject(DataProvider.Instance().GetObjectByObjectName(ModuleId, ObjectName), GetType(ObjectInfo)), ObjectInfo)
  End Function

  Public Shared Function GetObjectsByObjectName(ByVal ObjectName As String) As List(Of ObjectInfo)
   Return CBO.FillCollection(Of ObjectInfo)(DataProvider.Instance().GetObjectsByObjectName(ObjectName))
  End Function

  Public Shared Function GetObjects(ByVal ModuleId As Integer) As List(Of ObjectInfo)
   Return CBO.FillCollection(Of ObjectInfo)(DataProvider.Instance().GetObjects(ModuleId))
  End Function

  Public Shared Function GetObjectPackList(ByVal ObjectId As Integer, Version As String) As List(Of ObjectInfo)
   Return CBO.FillCollection(Of ObjectInfo)(DataProvider.Instance().GetObjectPackList(ObjectId, Version))
  End Function

  Public Shared Function GetObjectMetrics(ByVal objectId As Integer, ByVal locale As String) As ObjectMetrics
   Dim res As New ObjectMetrics
   With res
    .CurrentVersion = TextsController.CurrentVersion(objectId, locale)
    .NrOfFiles = TextsController.NrOfFiles(objectId, .CurrentVersion)
    .NrOfItems = TextsController.NrOfItems(objectId, .CurrentVersion)
    .NrOfChangedTexts = TextsController.NrOfChangedTexts(objectId, .CurrentVersion)
    If locale = "" Then
     .NrOfMissingTranslations = -1
    Else
     .NrOfMissingTranslations = TextsController.NrOfMissingTranslations(objectId, locale, .CurrentVersion)
    End If
    If .NrOfItems > 0 Then .PercentageComplete = CInt(Math.Round(((.NrOfItems - .NrOfMissingTranslations) * 100.0) / .NrOfItems))
   End With
   Return res
  End Function

 End Class

End Namespace


Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Tokens

Imports DNNEurope.Modules.LocalizationEditor.Data
Imports System.Collections.Generic

Namespace Business

 Partial Public Class ObjectCoreVersionsController

  Public Shared Sub SetObjectCoreVersion(ObjectId As Int32, Version As String, DnnVersion As String, InstalledByDefault As Boolean)
   Data.DataProvider.Instance().SetObjectCoreVersion(ObjectId, Version, DnnVersion, InstalledByDefault)
  End Sub

  Public Shared Function GetCoreObjects(DnnVersion As String, GetAllObjects As Boolean) As List(Of ObjectInfo)
   Return CBO.FillCollection(Of ObjectInfo)(DataProvider.Instance().GetCoreObjects(DnnVersion, GetAllObjects))
  End Function

 End Class
End Namespace


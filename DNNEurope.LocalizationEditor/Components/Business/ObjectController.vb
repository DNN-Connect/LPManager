Imports DotNetNuke.Common.Utilities

Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " ObjectController "
    Public Class ObjectController

        Public Shared Function GetObject(ByVal ObjectId As Integer) As ObjectInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetObject(ObjectId), GetType(ObjectInfo)), ObjectInfo)
        End Function

        Public Shared Function GetObjectByObjectName(ByVal ObjectName As String) As ObjectInfo
            Return CType(CBO.FillObject(Data.DataProvider.Instance().GetObjectByObjectName(ObjectName), GetType(ObjectInfo)), ObjectInfo)
        End Function

        Public Shared Function GetObjectList() As ArrayList
            Return DotNetNuke.Common.Utilities.CBO.FillCollection(Data.DataProvider.Instance().GetObjectList(), GetType(ObjectInfo))
        End Function

        Public Shared Function AddObject(ByVal objObject As ObjectInfo) As Integer
            Return CType(Data.DataProvider.Instance().AddObject(objObject.ObjectName, objObject.FriendlyName), Integer)
        End Function

        Public Shared Sub DeleteObject(ByVal ObjectId As Integer)
            Data.DataProvider.Instance().DeleteObject(ObjectId)
        End Sub

    End Class
#End Region

End Namespace

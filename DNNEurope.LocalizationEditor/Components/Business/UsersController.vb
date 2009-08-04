Namespace DNNEurope.Modules.LocalizationEditor.Business

#Region " UsersController "
    Public Class UsersController

        Public Shared Function GetUsersFiltered(ByVal filter As String) As DataSet

            Return Data.DataProvider.Instance().GetUsersFiltered(filter)

        End Function

    End Class
#End Region

End Namespace

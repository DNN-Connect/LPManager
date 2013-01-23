Namespace Services.UpdateService
 Public Class DnnPackage

  Public Property PackageName As String = ""
  Public Property ObjectId As Integer = -1
  Public Property Version As String = "00.00.00"
  Public Property LastRetrieved As Date = Date.MinValue
  Public Property TextCount As Integer = 0
  Public Property Translated As Integer = 0
  Public Property TargetLocale As String = ""
  Public Property LastChange As Date = Date.MinValue

  Public Function Clone() As DnnPackage
   Return New DnnPackage With {.LastChange = Me.LastChange, .LastRetrieved = Me.LastRetrieved, .ObjectId = Me.ObjectId, .PackageName = Me.PackageName, .TargetLocale = Me.TargetLocale, .TextCount = Me.TextCount, .Translated = Me.Translated, .Version = Me.Version}
  End Function

 End Class
End Namespace

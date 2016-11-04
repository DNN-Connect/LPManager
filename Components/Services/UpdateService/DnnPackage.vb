Namespace Services.UpdateService
 Public Class DnnPackage

 Public Property PackageName As String = ""
  Public Property Version As String = "00.00.00"
  Public Property TargetLocale As String = ""
  Public Property LanguagePacks As New List(Of DnnLanguagePack)

  Public Function Clone() As DnnPackage
   Return New DnnPackage With {.LanguagePacks = Me.LanguagePacks, .PackageName = Me.PackageName, .TargetLocale = Me.TargetLocale, .Version = Me.Version}
  End Function

 End Class
End Namespace

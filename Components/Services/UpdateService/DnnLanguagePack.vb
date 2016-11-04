Namespace Services.UpdateService
 Public Class DnnLanguagePack

  Public Property TranslatorName As String = ""
  Public Property TranslatorUrl As String = ""
  Public Property PackUrl As String = ""
  Public Property ObjectId As Integer = -1
  Public Property PercentComplete As Double = 0
  Public Property LastModified As Date = Date.MinValue

  Public Function Clone() As DnnLanguagePack
   Return New DnnLanguagePack With {.LastModified = Me.LastModified, .ObjectId = Me.ObjectId, .PackUrl = Me.PackUrl, .PercentComplete = Me.PercentComplete, .TranslatorName = Me.TranslatorName, .TranslatorUrl = Me.TranslatorUrl}
  End Function

 End Class
End Namespace

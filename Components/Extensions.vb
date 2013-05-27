Namespace Common
 Module Extensions

  <System.Runtime.CompilerServices.Extension()>
  Public Sub TryAdd(Of T)(ByRef list As List(Of T), item As T)
   If item IsNot Nothing Then
    list.Add(item)
   End If
  End Sub

 End Module
End Namespace

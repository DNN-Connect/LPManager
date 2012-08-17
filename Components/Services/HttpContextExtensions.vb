Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules.Internal

Namespace Services
 Public Module HttpContextExtensions

  Private Const ModuleIdKey As String = "ModuleId"
  Private Const TabIdKey As String = "TabId"

  <System.Runtime.CompilerServices.Extension()> _
  Public Function FindTabId(context As HttpContextBase) As Integer
   Dim res As Integer = -1
   Dim url As String = context.Request.RawUrl
   Dim m As Match = Regex.Match(url, "(?i)LocalizationEditor/API/(\d+).*(?-i)")
   If m.Success Then
    Int32.TryParse(m.Groups(1).Value, res)
   End If
   Return res
  End Function

  <System.Runtime.CompilerServices.Extension()> _
  Public Function FindModuleId(context As HttpContextBase) As Integer
   Dim res As Integer = -1
   Dim url As String = context.Request.RawUrl
   Dim m As Match = Regex.Match(url, "(?i)LocalizationEditor/API/\d+/(\d+).*(?-i)")
   If m.Success Then
    Int32.TryParse(m.Groups(1).Value, res)
   End If
   Return res
  End Function

  <System.Runtime.CompilerServices.Extension()> _
  Public Function FindModuleInfo(context As HttpContextBase) As ModuleInfo
   Dim tabId As Integer = context.FindTabId()
   Dim moduleId As Integer = context.FindModuleId()

   If moduleId <> Null.NullInteger AndAlso tabId <> Null.NullInteger Then
    Return TestableModuleController.Instance.GetModule(moduleId, tabId)
   End If

   Return Nothing
  End Function
 End Module
End Namespace

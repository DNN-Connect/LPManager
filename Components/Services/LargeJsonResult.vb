' Workaround for issue described in
' http://brianreiter.org/2011/01/03/custom-jsonresult-class-for-asp-net-mvc-to-avoid-maxjsonlength-exceeded-exception/

Imports System.Web.Script.Serialization
Imports System.Web.Mvc

Namespace Services
 Public Class LargeJsonResult
  Inherits JsonResult
  Const JsonRequest_GetNotAllowed As String = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet."

  Public Sub New()
   MaxJsonLength = 1024000
   RecursionLimit = 100
  End Sub

  Public Property MaxJsonLength() As Integer
   Get
    Return m_MaxJsonLength
   End Get
   Set(value As Integer)
    m_MaxJsonLength = value
   End Set
  End Property
  Private m_MaxJsonLength As Integer
  Public Property RecursionLimit() As Integer
   Get
    Return m_RecursionLimit
   End Get
   Set(value As Integer)
    m_RecursionLimit = value
   End Set
  End Property
  Private m_RecursionLimit As Integer

  Public Overrides Sub ExecuteResult(context As ControllerContext)
   If context Is Nothing Then
    Throw New ArgumentNullException("context")
   End If
   If JsonRequestBehavior = JsonRequestBehavior.DenyGet AndAlso [String].Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) Then
    Throw New InvalidOperationException(JsonRequest_GetNotAllowed)
   End If

   Dim response As HttpResponseBase = context.HttpContext.Response

   If Not [String].IsNullOrEmpty(ContentType) Then
    response.ContentType = ContentType
   Else
    response.ContentType = "application/json"
   End If
   If ContentEncoding IsNot Nothing Then
    response.ContentEncoding = ContentEncoding
   End If
   If Data IsNot Nothing Then
    Dim serializer As New JavaScriptSerializer() With {.MaxJsonLength = MaxJsonLength, .RecursionLimit = RecursionLimit}
    response.Write(serializer.Serialize(Data))
   End If
  End Sub
 End Class
End Namespace

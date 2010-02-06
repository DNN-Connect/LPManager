Imports System.Web
Imports System.Web.Services

Public Class GetCube
 Implements System.Web.IHttpHandler

 Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

  Dim PortalId As Integer = CInt(context.Request.Params("pid"))
  Dim ModuleId As Integer = CInt(context.Request.Params("mid"))
  context.Response.ContentType = "text/xml"
  Dim cube As New Business.Cube(PortalId, ModuleId)
  Dim xml As New System.Xml.XmlTextWriter(context.Response.Output)
  cube.WriteCube(xml)

 End Sub

 ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
  Get
   Return False
  End Get
 End Property

End Class
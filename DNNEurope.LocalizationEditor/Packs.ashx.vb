' 
' Copyright (c) 2004-2011 DNN-Europe, http://www.dnn-europe.net
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this 
' software and associated documentation files (the "Software"), to deal in the Software 
' without restriction, including without limitation the rights to use, copy, modify, merge, 
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
' to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or 
' substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
' ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
' 
Imports System.Web
Imports DNNEurope.Modules.LocalizationEditor.Services.DataExchange

Public Class Packs
 Implements System.Web.IHttpHandler

 Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

  context.Response.ContentType = "text/xml"
  Dim xml As New System.Xml.XmlTextWriter(context.Response.Output)

  Dim cmd As String = context.Request.Params("cmd")
  Select Case cmd.ToLower
   Case "list"
    Dim moduleKey As String = context.Request.Params("key")
    Dim locale As String = context.Request.Params("locale")
    Dim objectList As String = context.Request.Params("objects")
    Dim portalId As Integer = DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId
    Dim rl As New RequestList(portalId, moduleKey, locale, objectList)
    rl.WriteData(xml)
   Case Else
  End Select

 End Sub

 ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
  Get
   Return False
  End Get
 End Property

End Class
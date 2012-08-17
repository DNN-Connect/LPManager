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
'
' This code is an adaptation of code published by Rick Strahl on Programmers Heaven
' http://www.programmersheaven.com/2/dotnet-Web-Request
'
'

Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Collections.Specialized

Namespace Services.DataExchange
 Public Class WebRequest

#Region " Private Members "
  Private _Cookies As CookieCollection
  Private _Url As String
  Private _IsInError As Boolean = False
  Private _WebRequest As HttpWebRequest
  Private _WebResponse As HttpWebResponse
  Private _HandleCookies As Boolean = False
  Private _ThrowExceptions As Boolean = False
  Private _ErrorMsg As String = ""
  Private _Timeout As Integer = 30
  Private _ProxyPassword As String = ""
  Private _ProxyUsername As String = ""
  Private _ProxyBypass As String = ""
  Private _ProxyAddress As String = ""
  Private _Password As String = ""
  Private _UserName As String = ""
  Private _PostMode As PostModeType = PostModeType.URLEncoded
  Private _PostData As NameValueCollection
  Private _PostFiles As NameValueCollection
  Private _PostStream As MemoryStream
  Private _PostWriter As BinaryWriter
  Private Const MultiPartBoundary As String = "-----------------------------7cf2a327f01ae"
#End Region

#Region " Properties "
  Public Property Cookies() As CookieCollection
   Get
    Return _Cookies
   End Get
   Set(ByVal Value As CookieCollection)
    _Cookies = Value
   End Set
  End Property
  Public Property Url() As String
   Get
    Return _Url
   End Get
   Set(ByVal Value As String)
    _Url = Value
   End Set
  End Property
  Public Property IsInError() As Boolean
   Get
    Return _IsInError
   End Get
   Set(ByVal Value As Boolean)
    _IsInError = Value
   End Set
  End Property
  Public Property WebRequest() As HttpWebRequest
   Get
    Return _WebRequest
   End Get
   Set(ByVal Value As HttpWebRequest)
    _WebRequest = Value
   End Set
  End Property
  Public Property WebResponse() As HttpWebResponse
   Get
    Return _WebResponse
   End Get
   Set(ByVal Value As HttpWebResponse)
    _WebResponse = Value
   End Set
  End Property
  Public Property HandleCookies() As Boolean
   Get
    Return _HandleCookies
   End Get
   Set(ByVal Value As Boolean)
    _HandleCookies = Value
   End Set
  End Property
  Public Property ThrowExceptions() As Boolean
   Get
    Return _ThrowExceptions
   End Get
   Set(ByVal Value As Boolean)
    _ThrowExceptions = Value
   End Set
  End Property
  Public Property ErrorMsg() As String
   Get
    Return _ErrorMsg
   End Get
   Set(ByVal Value As String)
    _ErrorMsg = Value
   End Set
  End Property
  Public Property Timeout() As Integer
   Get
    Return _Timeout
   End Get
   Set(ByVal Value As Integer)
    _Timeout = Value
   End Set
  End Property
  Public Property ProxyPassword() As String
   Get
    Return _ProxyPassword
   End Get
   Set(ByVal Value As String)
    _ProxyPassword = Value
   End Set
  End Property
  Public Property ProxyUsername() As String
   Get
    Return _ProxyUsername
   End Get
   Set(ByVal Value As String)
    _ProxyUsername = Value
   End Set
  End Property
  Public Property ProxyBypass() As String
   Get
    Return _ProxyBypass
   End Get
   Set(ByVal Value As String)
    _ProxyBypass = Value
   End Set
  End Property
  Public Property ProxyAddress() As String
   Get
    Return _ProxyAddress
   End Get
   Set(ByVal Value As String)
    _ProxyAddress = Value
   End Set
  End Property
  Public Property Password() As String
   Get
    Return _Password
   End Get
   Set(ByVal Value As String)
    _Password = Value
   End Set
  End Property
  Public Property UserName() As String
   Get
    Return _UserName
   End Get
   Set(ByVal Value As String)
    _UserName = Value
   End Set
  End Property
  Public Property PostMode() As PostModeType
   Get
    Return _PostMode
   End Get
   Set(ByVal Value As PostModeType)
    _PostMode = Value
   End Set
  End Property

  Public ReadOnly Property PostData() As NameValueCollection
   Get
    Return _PostData
   End Get
  End Property
  Public ReadOnly Property PostFiles() As NameValueCollection
   Get
    Return _PostFiles
   End Get
  End Property
#End Region

#Region " Constructors "
  Public Sub New()
   _PostData = New NameValueCollection
   _PostFiles = New NameValueCollection
  End Sub
#End Region

#Region " Public Methods "
  Public Sub AddPostdata(ByVal PostCollection As NameValueCollection)
   For Each key As String In PostCollection.Keys
    _PostData.Add(key, PostCollection(key))
   Next
  End Sub

  Public Sub AddPostdata(ByVal VarName As String, ByVal VarValue As String)
   _PostData.Add(VarName, VarValue)
  End Sub

  Public Sub AddPostfile(ByVal VarName As String, ByVal VarFilename As String)
   _PostFiles.Add(VarName, VarFilename)
  End Sub

  Public Function GetResponsePage() As WebPage

   Return New WebPage(Me.GetResponse)

  End Function

  Public Function GetResponse() As String

   Dim Resp As StreamReader = Me.GetResponseStream()
   If Resp Is Nothing Then
    Return ""
   Else
    Dim lcResult As String = Resp.ReadToEnd()
    Resp.Close()
    Return lcResult
   End If

  End Function

  Public Function GetResponseWithEvents(ByVal BufferSize As Long) As String

   Dim Resp As StreamReader = Me.GetResponseStream()
   If Resp Is Nothing Then
    Return ""
   End If

   Dim lnSize As Long = Me.WebResponse.ContentLength
   Dim enc As Encoding = Encoding.GetEncoding(1252)
   Dim loWriter As New StringBuilder(CType(lnSize, Integer))
   Dim lcTemp(CType(lnSize, Integer)) As Char
   Dim oArgs As New OnReceiveDataEventArgs
   oArgs.TotalBytes = lnSize

   lnSize = 1
   Dim lnCount As Integer = 0
   Dim lnTotalBytes As Long = 0

   While lnSize > 0
    lnSize = Resp.Read(lcTemp, 0, CType(BufferSize, Integer))
    If lnSize > 0 Then
     loWriter.Append(lcTemp, 0, CType(lnSize, Integer))
     lnCount += 1
     lnTotalBytes += lnSize
     With oArgs
      .CurrentByteCount = lnTotalBytes
      .NumberOfReads = lnCount
      .CurrentChunk = lcTemp
     End With
     RaiseEvent OnReceiveData(Me, oArgs)
     If oArgs.Cancel Then
      oArgs.Done = True
      RaiseEvent OnReceiveData(Me, oArgs)
      Exit While
     End If
    End If
   End While

   Resp.Close()

   Return loWriter.ToString

  End Function

  Public Function GetResponseStream(Optional ByVal Request As HttpWebRequest = Nothing) As StreamReader

   Try

    If Request Is Nothing Then
     Request = CType(Net.WebRequest.Create(Me.Url), HttpWebRequest)
    End If
    With Request
     '.UserAgent = ""
     .Timeout = Me.Timeout * 1000
    End With

    ' Handle Login
    If Me.UserName.Length > 0 Then
     If Me.UserName.ToUpper = "AUTOLOGIN" Then
      Request.Credentials = CredentialCache.DefaultCredentials
     Else
      Request.Credentials = New NetworkCredential(Me.UserName, Me.Password)
     End If
    End If

    ' Handle Proxy
    If Me.ProxyAddress.Length > 0 Then
     If Me.ProxyAddress.ToUpper = "DEFAULTPROXY" Then
      Request.Proxy = New WebProxy
      Request.Proxy = System.Net.WebRequest.DefaultWebProxy
     Else
      Dim loProxy As New WebProxy(Me.ProxyAddress, True)
      If Me.ProxyBypass.Length > 0 Then
       loProxy.BypassList = Me.ProxyBypass.Split(New Char() {";"c})
      End If
      If Me.ProxyUsername.Length > 0 Then
       loProxy.Credentials = New NetworkCredential(Me.ProxyUsername, Me.ProxyPassword)
      End If
      Request.Proxy = loProxy
     End If
    End If

    ' Handle Cookies
    If Me.HandleCookies Then
     Request.CookieContainer = New CookieContainer
     If (Not Me.Cookies Is Nothing) And Me.Cookies.Count > 0 Then
      Request.CookieContainer.Add(Me.Cookies)
     End If
    End If

    ' Handle Post Data
    If Me.PostData.Count > 0 Or Me.PostFiles.Count > 0 Then

     Request.Method = "POST"
     Select Case Me.PostMode
      Case PostModeType.URLEncoded
       Request.ContentType = "application/x-www-form-urlencoded"
      Case PostModeType.MultipartForm
       Request.ContentType = "multipart/form-data; boundary=" + MultiPartBoundary
       'Write( Encoding.GetEncoding(1252).GetBytes( "--" + this.cMultiPartBoundary + "\r\n" ) )
      Case Else
       Request.ContentType = "text/xml"
     End Select

     ' Add post data
     Dim postStream As New IO.MemoryStream
     Dim postWriter As New IO.BinaryWriter(postStream)
     Dim iTotal As Integer = _PostData.Keys.Count
     Dim i As Integer = 0
     For Each key As String In _PostData.Keys
      i += 1
      Dim str As String
      Select Case _PostMode
       Case PostModeType.URLEncoded
        str = key & "=" & System.Web.HttpUtility.UrlEncode(Encoding.GetEncoding(1252).GetBytes(_PostData(key)))
        If i < iTotal Then
         str &= "&"
        End If
       Case PostModeType.MultipartForm
        str = "--" & MultiPartBoundary & "\r\n" & "Content-Disposition: form-data; name=\""" & key & "\""\r\n\r\n"
       Case Else
        str = _PostData(key)
      End Select
      postWriter.Write(Encoding.GetEncoding(1252).GetBytes(str))
     Next

     ' Add posted files
     For Each key As String In _PostFiles.Keys
      Dim str As String = ""
      Select Case _PostMode
       Case PostModeType.MultipartForm
        Try
         Using loFile As New FileStream(_PostFiles(key), System.IO.FileMode.Open, System.IO.FileAccess.Read)
          Dim lcFile(CType(loFile.Length, Integer)) As Byte
          loFile.Read(lcFile, 0, CType(loFile.Length, Integer))
          postWriter.Write(Encoding.GetEncoding(1252).GetBytes("--" & MultiPartBoundary & "\r\n" & "Content-Disposition: form-data; name=\""" + key + "\""filename=\""" & New FileInfo(_PostFiles(key)).Name + "\""\r\n\r\n"))
          postWriter.Write(lcFile)
          postWriter.Write(Encoding.GetEncoding(1252).GetBytes("\r\n"))
         End Using
        Catch ex As Exception
         Me.ErrorMsg = ex.Message
         Me.IsInError = True
        End Try
       Case Else
        Me.ErrorMsg = "File upload allowed only with Multi-part forms"
        Me.IsInError = True
      End Select
      postWriter.Write(Encoding.GetEncoding(1252).GetBytes(str))
     Next

     ' Inject into request stream
     Dim ReqStream As Stream = Request.GetRequestStream
     postStream.WriteTo(ReqStream)
     postStream.Close()
     postStream = Nothing
     postWriter.Close()
     postWriter = Nothing
     ReqStream.Close()

    End If ' post data


    Dim Response As HttpWebResponse = CType(Request.GetResponse, HttpWebResponse)

    ' Save cookies
    If Me.HandleCookies Then
     If Response.Cookies.Count > 0 Then
      If Me.Cookies Is Nothing Then
       Me.Cookies = Response.Cookies
      Else
       For Each ck As Cookie In Response.Cookies
        Dim Match As Boolean = False
        For Each lck As Cookie In Me.Cookies
         If ck.Name = lck.Name Then
          lck.Value = ck.Value
          Match = True
          Exit For
         End If
        Next
        If Not Match Then
         Me.Cookies.Add(ck)
        End If
       Next
      End If
     End If
    End If

    Me.WebResponse = Response

    Dim enc As Encoding
    Try
     If Response.ContentEncoding.Length > 0 Then
      enc = Encoding.GetEncoding(Response.ContentEncoding)
     Else
      enc = Encoding.GetEncoding(1252)
     End If
    Catch ex As Exception
     enc = Encoding.GetEncoding(1252)
    End Try

    Return New StreamReader(Response.GetResponseStream, enc)

   Catch ex As Exception

    If Me.ThrowExceptions Then
     Throw (ex)
    Else
     Me.ErrorMsg = ex.Message
     Me.IsInError = True
     Return Nothing
    End If

   End Try
  End Function
#End Region

#Region " Event Handling "
  Public Event OnReceiveData As OnReceiveDataHandler

  Public Delegate Sub OnReceiveDataHandler(ByVal sender As Object, ByVal e As OnReceiveDataEventArgs)

  Public Class OnReceiveDataEventArgs
   Public CurrentByteCount As Long = 0
   Public TotalBytes As Long = 0
   Public NumberOfReads As Integer = 0
   Public CurrentChunk As Char()
   Public Done As Boolean = False
   Public Cancel As Boolean = False
  End Class
#End Region

#Region " Public Enums "
  Public Enum PostModeType
   URLEncoded
   MultipartForm
   Raw
  End Enum
#End Region

 End Class
End Namespace

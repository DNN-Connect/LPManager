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

Imports DNNEurope.Modules.LocalizationEditor.Business
Imports DotNetNuke.Framework
Imports System.Runtime.InteropServices


Partial Public Class Pack
 Inherits PageBase

#Region " Private Members "

 Private _ObjectId As Integer = -1
 Private _locale As String = ""
 Private _moduleName As String = ""
 Private _version As String = ""
 Private _type As String = ""
 Private _requestedObject As ObjectInfo = Nothing

#End Region

#Region " Properties "

 Public Property ModuleName() As String
  Get
   Return _moduleName
  End Get
  Set(ByVal value As String)
   _moduleName = value
  End Set
 End Property

 Public Property Locale() As String
  Get
   Return _locale
  End Get
  Set(ByVal value As String)
   _locale = value
  End Set
 End Property

 Public Property ObjectId() As Integer
  Get
   Return _ObjectId
  End Get
  Set(ByVal value As Integer)
   _ObjectId = value
  End Set
 End Property

 Public Property Version() As String
  Get
   Return _version
  End Get
  Set(ByVal value As String)
   _version = value
  End Set
 End Property

 Public Property Type() As String
  Get
   Return _type
  End Get
  Set(ByVal value As String)
   _type = value
  End Set
 End Property


#End Region

#Region " Event Handlers "

 Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
  Globals.ReadQuerystringValue(Me.Request.Params, "ObjectId", ObjectId)
  Globals.ReadQuerystringValue(Me.Request.Params, "Locale", Locale)
  Globals.ReadQuerystringValue(Me.Request.Params, "Version", Version)
  Globals.ReadQuerystringValue(Me.Request.Params, "Type", Type)

  _requestedObject = ObjectController.GetObject(ObjectId)
  If _requestedObject Is Nothing Then Throw New ArgumentException(String.Format("ObjectId with value {0} is not valid.", ObjectId))
  ModuleName = _requestedObject.ObjectName.Replace("\", "_").Replace("/", "_")
 End Sub

 Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
  Dim fn As String = ""
  If Type.ToLower = "full" Then
   fn = LocalizationController.CreateResourcePack(_requestedObject, Version, Locale, True)
  Else
   fn = LocalizationController.CreateResourcePack(_requestedObject, Version, Locale, False)
  End If
  Me.Response.Redirect(DotNetNuke.Common.ApplicationPath & "/" & _requestedObject.Module.HomeDirectory & "/LocalizationEditor/Cache/" & _requestedObject.ModuleId.ToString & "/" & fn, False)
 End Sub

#End Region

End Class

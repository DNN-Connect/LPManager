' 
' Copyright (c) 2004-2009 DNN-Europe, http://www.dnn-europe.net
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

Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports System.Globalization
Imports DotNetNuke.Entities.Users
Imports System.Xml.Schema
Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Namespace Business

 <Serializable(), XmlRoot("Module")> Public Class ModuleInfo
  Implements IHydratable

  ' local property declarations
  Private _moduleId As Integer
  Private _HomeDirectory As String
  Private _OwnerName As String
  Private _OwnerEmail As String
  Private _OwnerOrganization As String
  Private _OwnerUrl As String
  Private _CachePacks As Boolean

#Region " Constructors "

  Public Sub New(ByVal objectId As Integer)
   MyBase.New()
   Using ir As IDataReader = Data.DataProvider.Instance().GetModuleForObject(objectId)
    If ir.Read Then
     Me.Fill(ir)
    End If
   End Using
  End Sub

#End Region

#Region " Public Properties "

  Public Property ModuleId() As Integer
   Get
    Return _moduleId
   End Get
   Set(ByVal value As Integer)
    _moduleId = value
   End Set
  End Property

  Public Property HomeDirectory() As String
   Get
    Return _HomeDirectory
   End Get
   Set(ByVal Value As String)
    _HomeDirectory = Value
   End Set
  End Property

  Public Property OwnerName() As String
   Get
    Return _OwnerName
   End Get
   Set(ByVal Value As String)
    _OwnerName = Value
   End Set
  End Property

  Public Property OwnerEmail() As String
   Get
    Return _OwnerEmail
   End Get
   Set(ByVal value As String)
    _OwnerEmail = value
   End Set
  End Property

  Public Property OwnerOrganization() As String
   Get
    Return _OwnerOrganization
   End Get
   Set(ByVal value As String)
    _OwnerOrganization = value
   End Set
  End Property

  Public Property OwnerUrl() As String
   Get
    Return _OwnerUrl
   End Get
   Set(ByVal value As String)
    _OwnerUrl = value
   End Set
  End Property

  Public Property CachePacks() As Boolean
   Get
    Return _CachePacks
   End Get
   Set(ByVal value As Boolean)
    _CachePacks = value
   End Set
  End Property


#End Region

#Region " IHydratable Implementation "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill
   Try
    ModuleId = Convert.ToInt32(Null.SetNull(dr.Item("ModuleId"), ModuleId))
    HomeDirectory = Convert.ToString(Null.SetNull(dr.Item("HomeDirectory"), HomeDirectory))
    OwnerName = Convert.ToString(Null.SetNull(dr.Item("OwnerName"), OwnerName))
    OwnerEmail = Convert.ToString(Null.SetNull(dr.Item("OwnerEmail"), OwnerEmail))
    OwnerOrganization = Convert.ToString(Null.SetNull(dr.Item("OwnerOrganization"), OwnerOrganization))
    OwnerUrl = Convert.ToString(Null.SetNull(dr.Item("OwnerUrl"), OwnerUrl))
    CachePacks = Convert.ToBoolean(Null.SetNull(dr.Item("CachePacks"), CachePacks))
   Catch ex As Exception
   End Try
  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[]	06/16/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return ModuleId
   End Get
   Set(ByVal value As Integer)
    ModuleId = value
   End Set
  End Property

#End Region

 End Class
End Namespace
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

Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Utilities

Namespace Entities.Objects

 <Serializable(), XmlRoot("Module")> Public Class ModuleInfo
  Implements IHydratable

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
  Public Property ModuleId As Integer = -1
  Public Property HomeDirectory As String = ""
  Public Property OwnerName As String = ""
  Public Property OwnerEmail As String = ""
  Public Property OwnerOrganization As String = ""
  Public Property OwnerUrl As String = ""
  Public Property CachePacks As Boolean = True
  Public Property Attribution As String = ""
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
    Attribution = Convert.ToString(Null.SetNull(dr.Item("Attribution"), Attribution))
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
Imports System.Runtime.Serialization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules

Namespace Entities.Packages
 <DataContract()>
 Public Class PackInfo
  Implements IHydratable

#Region " Properties "
  <DataMember()>
  Public Property ObjectId As Int32
  <DataMember()>
  Public Property ObjectName As String
  <DataMember()>
  Public Property FriendlyName As String
  <DataMember()>
  Public Property InstallPath As String
  <DataMember()>
  Public Property ModuleId As Int32
  <DataMember()>
  Public Property PackageType As String = "Module"
  <DataMember()>
  Public Property Version As String
  <DataMember()>
  Public Property Locale As String
  <DataMember()>
  Public Property NewTexts As Integer
  <DataMember()>
  Public Property TotalTexts As Integer
  <DataMember()>
  Public Property TotalTranslated As Integer
  <DataMember()>
  Public Property LastModified As DateTime
#End Region

#Region " IHydratable Implementation "
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[pdonker]	08/09/2011  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   ObjectId = Convert.ToInt32(Null.SetNull(dr.Item("ObjectId"), ObjectId))
   ObjectName = Convert.ToString(Null.SetNull(dr.Item("ObjectName"), ObjectName))
   FriendlyName = Convert.ToString(Null.SetNull(dr.Item("FriendlyName"), FriendlyName))
   InstallPath = Convert.ToString(Null.SetNull(dr.Item("InstallPath"), InstallPath))
   ModuleId = Convert.ToInt32(Null.SetNull(dr.Item("ModuleId"), ModuleId))
   PackageType = Convert.ToString(Null.SetNull(dr.Item("PackageType"), PackageType))
   Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))

   Locale = Convert.ToString(Null.SetNull(dr.Item("Locale"), Locale))
   NewTexts = Convert.ToInt32(Null.SetNull(dr.Item("NewTexts"), NewTexts))
   TotalTexts = Convert.ToInt32(Null.SetNull(dr.Item("TotalTexts"), TotalTexts))
   TotalTranslated = Convert.ToInt32(Null.SetNull(dr.Item("TotalTranslated"), TotalTranslated))
   LastModified = Convert.ToDateTime(Null.SetNull(dr.Item("LastModified"), LastModified))
   Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))
   Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))
   Version = Convert.ToString(Null.SetNull(dr.Item("Version"), Version))

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	08/09/2011  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return ObjectId
   End Get
   Set(ByVal value As Integer)
    ObjectId = value
   End Set
  End Property
#End Region

 End Class
End Namespace

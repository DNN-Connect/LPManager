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
Imports System.Xml
Imports DNNEurope.Modules.LocalizationEditor.Entities.PartnerPacks
Imports DNNEurope.Modules.LocalizationEditor.Entities.Partners
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects

Namespace Services.DataExchange
 Public Class FeedImporter

#Region " Private Members "
  Private _moduleId As Integer = -1
  Private _partner As PartnerInfo = Nothing
  Private _log As StringBuilder
  Private _settings As ModuleSettings
  Private _handledPartners As New List(Of String)
#End Region

#Region " Constructors "
  Public Sub New(ByVal partner As PartnerInfo)
   _partner = partner
   _moduleId = partner.ModuleId
   _log = New StringBuilder
   _settings = New ModuleSettings("", _partner.ModuleId)
  End Sub
#End Region

#Region " Public Methods "
  Public Sub ImportFeed()
   ImportFeed(False)
  End Sub

  Public Sub ImportFeed(ByVal overrideAutoImportObjects As Boolean)

   If String.IsNullOrEmpty(_partner.CubeUrl) Then Exit Sub
   Dim req As New WebRequest
   With req
    .Url = _partner.CubeUrl
    .PostMode = WebRequest.PostModeType.URLEncoded
   End With
   Dim cube As New XmlDocument
   cube.LoadXml(req.GetResponse)

   _handledPartners.Add(_partner.PartnerName)
   ProcessCube(_partner, cube.DocumentElement, overrideAutoImportObjects, _partner.DownloadAffiliates)

  End Sub
#End Region

#Region " Private Methods "
  Private Sub ProcessCube(ByVal partner As PartnerInfo, ByVal cube As XmlNode, ByVal overrideAutoImportObjects As Boolean, ByVal downloadAffiliates As Boolean)

   ' process Partner data
   partner.AllowDirectDownload = CBool(cube.Attributes("allowDirectDownload").InnerText)
   partner.PartnerName = cube.Attributes("ownerName").InnerText
   If partner.PartnerName = "" Then Exit Sub
   partner.PartnerUrl = Globals.CorrectHttp(cube.Attributes("ownerUrl").InnerText)
   partner.PackUrl = cube.Attributes("packUrl").InnerText
   partner.LastCube = cube.OuterXml
   PartnersController.UpdatePartner(partner)

   ' check existing content
   For Each p As PartnerPackInfo In PartnerPacksController.GetPartnerPacksByPartner(partner.PartnerId)
    If cube.SelectNodes("object[@id='" & p.RemoteObjectId.ToString & "']").Count = 0 Then
     PartnerPacksController.DeletePartnerPack(p)
    End If
   Next

   ' process content
   For Each o As XmlNode In cube.SelectNodes("object")
    Dim objectName As String = o.Attributes("name").InnerText
    Dim obj As ObjectInfo = ObjectsController.GetObjectByObjectName(partner.ModuleId, objectName)
    If obj Is Nothing And (_settings.AutoImportObjects Or overrideAutoImportObjects) Then
     obj = New ObjectInfo
     With obj
      .ObjectName = objectName
      .ModuleId = partner.ModuleId
      .FriendlyName = o.Attributes("friendlyName").InnerText
      .PackageType = o.Attributes("type").InnerText
      .ObjectId = ObjectsController.AddObject(obj)
     End With
    End If
    If obj IsNot Nothing Then
     Dim remoteObjectId As Integer = CInt(o.Attributes("id").InnerText)
     For Each v As XmlNode In o.SelectNodes("version")
      Dim version As String = v.Attributes("name").InnerText
      Dim newTexts As Integer = CInt(v.Attributes("newTexts").InnerText)
      Dim totalTexts As Integer = CInt(v.Attributes("totalTexts").InnerText)
      For Each l As XmlNode In v.SelectNodes("locale")
       Dim locale As String = l.Attributes("name").InnerText
       Dim lTotalTexts As Integer = CInt(l.Attributes("totalTexts").InnerText)
       Dim lastModified As Date = CDate(l.Attributes("lastModified").InnerText)
       Dim partnerPack As PartnerPackInfo = PartnerPacksController.GetPartnerPack(partner.PartnerId, obj.ObjectId, version, locale)
       If partnerPack Is Nothing Then
        partnerPack = New PartnerPackInfo
        With partnerPack
         .LastModified = lastModified
         .Locale = locale
         .RemoteObjectId = remoteObjectId
         .ObjectId = obj.ObjectId
         .PercentComplete = CDec(100 * (lTotalTexts / totalTexts))
         .PartnerId = partner.PartnerId
         .Version = version
        End With
        PartnerPacksController.AddPartnerPack(partnerPack)
       Else
        With partnerPack
         .LastModified = lastModified
         .RemoteObjectId = remoteObjectId
         .PercentComplete = CDec(100 * (lTotalTexts / totalTexts))
        End With
        PartnerPacksController.UpdatePartnerPack(partnerPack)
       End If
       LogWrite("Updating PartnerPack for object {0}. Locale: {1}, Version: {2}", obj.ObjectName, partnerPack.Locale, partnerPack.Version)
      Next
     Next
    End If
   Next

   ' recurse if applicable
   If downloadAffiliates Then
    For Each affiliate As XmlNode In cube.SelectNodes("partners/cube")
     Dim partnerName As String = affiliate.Attributes("ownerName").InnerText
     If partnerName <> _settings.OwnerName And Not _handledPartners.Contains(partnerName) Then
      Dim p As PartnerInfo = PartnersController.GetPartnerByName(_moduleId, partnerName)
      If p Is Nothing Then
       p = New PartnerInfo
       With p
        .ModuleId = _moduleId
        .AllowDirectDownload = CBool(cube.Attributes("allowDirectDownload").InnerText)
        .AllowRedistribute = _partner.AllowRedistribute
        .CubeUrl = ""
        .DownloadAffiliates = downloadAffiliates
        .PackUrl = cube.Attributes("packUrl").InnerText
        .PartnerName = partnerName
        .PartnerUrl = Globals.CorrectHttp(cube.Attributes("ownerUrl").InnerText)
        .PartnerUsername = ""
       End With
       PartnersController.AddPartner(p)
      End If
      ProcessCube(p, affiliate, overrideAutoImportObjects, downloadAffiliates)
      _handledPartners.Add(partnerName)
     End If
    Next
   End If

  End Sub

  Private Sub LogWrite(ByVal format As String, ByVal ParamArray args() As Object)
   Log.AppendFormat(format, args)
   Log.AppendLine()
  End Sub
#End Region

#Region " Properties "
  Public Property Log() As StringBuilder
   Get
    Return _log
   End Get
   Set(ByVal value As StringBuilder)
    _log = value
   End Set
  End Property
#End Region

 End Class
End Namespace

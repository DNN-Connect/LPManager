
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Tokens
Imports DNNEurope.Modules.LocalizationEditor.Entities.Objects

Namespace Services.Packaging
 Public Class TokenReplace
  Inherits DotNetNuke.Services.Tokens.TokenReplace

  Public Sub New(obj As ObjectInfo, contributingUser As UserInfo, locale As String)
   Me.PropertySource("object") = obj
   Me.PropertySource("contributor") = New UserTokenReplace(contributingUser)
   Me.PropertySource("locale") = New LocaleTokenReplace(locale)
  End Sub

  Protected Overrides Function replacedTokenValue(strObjectName As String, strPropertyName As String, strFormat As String) As String
   Return MyBase.replacedTokenValue(strObjectName, strPropertyName, strFormat)
  End Function

 End Class

#Region " LocaleTokenReplace "
 Public Class LocaleTokenReplace
  Implements IPropertyAccess

  Public Property Locale As Globalization.CultureInfo = Nothing

  Public Sub New(locale As String)
   Me.Locale = New Globalization.CultureInfo(locale)
  End Sub

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   If Locale Is Nothing Then Return ""
   Dim OutputFormat As String = String.Empty
   Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
   If strFormat = String.Empty Then
    OutputFormat = "D"
   Else
    OutputFormat = strFormat
   End If
   Select Case strPropertyName.ToLower
    Case "displayname"
     Return PropertyAccess.FormatString(Me.Locale.DisplayName, strFormat)
    Case "englishname"
     Return PropertyAccess.FormatString(Me.Locale.EnglishName, strFormat)
    Case "name"
     Return PropertyAccess.FormatString(Me.Locale.Name, strFormat)
    Case "nativename"
     Return PropertyAccess.FormatString(Me.Locale.NativeName, strFormat)
    Case Else
     PropertyNotFound = True
   End Select
   Return DotNetNuke.Common.Utilities.Null.NullString
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
#End Region

#Region " UserTokenReplace "
 Public Class UserTokenReplace
  Implements IPropertyAccess

  Public Property User As UserInfo = Nothing

  Public Sub New(user As UserInfo)
   Me.User = user
  End Sub

#Region " IPropertyAccess Implementation "
  Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As DotNetNuke.Entities.Users.UserInfo, ByVal AccessLevel As DotNetNuke.Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements DotNetNuke.Services.Tokens.IPropertyAccess.GetProperty
   If User Is Nothing Then Return ""
   Dim res As String = User.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, PropertyNotFound)
   If Not PropertyNotFound Then
    Return res
   End If
   Return User.Profile.GetPropertyValue(strPropertyName)
  End Function

  Public ReadOnly Property Cacheability() As DotNetNuke.Services.Tokens.CacheLevel Implements DotNetNuke.Services.Tokens.IPropertyAccess.Cacheability
   Get
    Return CacheLevel.fullyCacheable
   End Get
  End Property
#End Region

 End Class
#End Region

End Namespace

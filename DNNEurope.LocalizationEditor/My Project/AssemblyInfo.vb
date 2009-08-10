Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("DNNEurope.Modules.LocalizationEditor")> 
<Assembly: AssemblyDescription("Editor for module/core localization")> 
<Assembly: AssemblyCompany("DNN-Europe")> 
<Assembly: AssemblyProduct("DNN-Europe LocalizationEditor")> 
<Assembly: AssemblyCopyright("(c)2009 DNN-Europe")> 
<Assembly: AssemblyTrademark("http://www.dnn-europe.net")> 
<Assembly: CLSCompliant(True)> 

<Assembly: Guid("64877293-5555-47d6-a82a-b0d736eff0b6")> 

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers 
' by using the '*' as shown below:

<Assembly: AssemblyVersion("03.00.03.00")> 
<Assembly: AssemblyFileVersionAttribute("03.00.03.00")> 
<Assembly: AssemblyInformationalVersion("03.00.03.00")> 

'History
'03.00.03 2009-08-10 Janga: User Interface.
'03.00.03 2009-08-10 Janga: Translation of long texts: try different separator charachters.
'03.00.03 2009-08-07 Janga: Translation of long texts by chopping them up into parts and then stitching the translations together again.
'03.00.02 2009-08-07 Janga: User Interface. Disabled AjaxTimerTick.
'03.00.01 2009-08-07 Janga: Version number in history.
'01.00.10 2009-07-30 Janga: User Interface.
'01.00.10 2009-07-29 Janga: User Interface.
'01.00.09 2009-07-16 GK:    MSBuild: Retrieve AssemblyVersion for use as ModuleVersion by way of new XCESS.MSBuild.Tasks.dll.
'01.00.09 2009-07-16 Janga: CssClass of hyperlink will not show when NavigateUrl (= href) is absent or empty.
'                           Used dummy NavigateUrl value "#_self" and appended OnClick() with "return false" so that the href is not followed.
'01.00.08 2009-07-16 Janga: Changed "Import" (pack) to "Upload Pack", conforming to "Download Pack".
'01.00.07 2009-07-15 Janga: Importing DNN 5 extension packages (having manifest file version 5.0) now also works.
'01.00.07 2009-07-15 Janga: MSBuild. =!= Still have to change ModuleVersion in CreateDnnModulePackage.Targets by hand when changing assembly version.
'01.00.06 2009-07-03 Janga: Replaced the RequestPack page (aspx) by a DownloadPack control (ascx), but kept the RequestPack page (aspx).
'01.00.06 2009-07-02 Janga: Started working on replacing the RequestPack page (aspx) by a DownloadPack control (ascx).
'01.00.05 2009-07-02 Janga: Removed the unused "languageeditor" control.
'01.00.05 2009-07-01 Janga: Set height and width of text areas and cells more consistently in the 3 text columns of the resource editor.
'01.00.05 2009-07-01 Janga: Google Translate translates quotes and other HTML characters inconsistently. Workaround: first replace them with escape codes and then back again.
'01.00.05 2009-07-01 Janga: Google Translate translates "{" and "}" to "(" and ")". Workaround: first replace "{" with "(((" and "}" with ")))" and then back again.
'01.00.05 2009-06-30 Janga: In Users page ("Manage permissions"): Moved the list of entered persmissions to the bottom; Changed regular expression for entering locale.
'01.00.05 2009-06-29 Janga: Added functionality to remove a translated module altogether with all its permissions, texts and translations (01.00.05.SqlDataProvider).
'01.00.04 2009-06-26 Janga: Provided image buttons for transferring or translating texts with a title (tooltip).
'01.00.04 2009-06-26 Janga: Sorted rows on ManageModules page ("Manage Modules"): changed stored procedure LocalizationEditor_GetTranslateModuleList. =!= Modified 01.00.02.SqlDataProvider.
'01.00.04 2009-06-26 Janga: Sorted rows on Users page ("Manage Permissions"): changed stored procedure LocalizationEditor_GetPermissions. =!= Modified 01.00.02.SqlDataProvider.
'01.00.04 2009-06-26 Janga: Handle desktop module version values of NULL and "" by converting both to default version of "0" in LocalizationEditor.
'                           Column "Version" of table LocalizationEditor_Text NOT NULL. =!= Modified 01.00.00.SqlDataProvider and 01.00.02.SqlDataProvider.
'01.00.04 2009-06-25 Janga: Moved Google Translation functionality to ResourceEditor page, including "Translate All" links (per panel and for the whole page).
'01.00.04 2009-06-25 Janga: Dereleased Rich Text Editor fields, this means no more FCKeditor! They have been converted to text areas (TextboxMode = MultiLine).
'01.00.03 2009-06-24 Janga: Added 3 links for the whole page: "Transfer All (Original)", "Transfer All (Source)" and "Clear All".
'01.00.03 2009-06-23 Janga: Transfer all texts to localized texts per panel is working in part. =!= TODO: Transfer Rich Text Editor fields.
'01.00.03 2009-06-22 Janga: Transfer all texts to localized texts per panel is working in part.
'01.00.03 2009-06-22 Janga: Clear all localized texts per panel is working in part. =!= TODO: Clear Rich Text Editor fields.
'01.00.03 2009-06-19 Janga: Clear all localized texts per panel is working in part.
'01.00.03 2009-06-18 Janga: Started working on the button bar in ResourceEditor.
'01.00.03 2009-06-18 Janga: Changed stored procedure LocalizationEditor_GetTranslationList: get correct source value (01.00.03.SqlDataProvider).
'01.00.02 2009-06-18 Janga: Changed stored procedure LocalizationEditor_Getpermissions: use ObjectName instead of ModuleFriendlyName (01.00.02.SqlDataProvider).
'01.00.02 2009-06-17 Janga: Referencing DotNetNuke 5.1.0.
'01.00.02 2009-06-16 Janga: Changed stored procedure LocalizationEditor_GetLatestText to include the possibility of the DesktopModules version being null (01.00.02.SqlDataProvider).
'01.00.02 2009-06-16 Janga: Changed ManifestTemplate.dnn to include ManageModules.ascx.resx.
'01.00.02 2009-06-15 Janga: Moved Template.resx into App_LocalResources, otherwise the module would not install ("could not find file ... ").
'01.00.02 2009-06-15 Janga: Referencing DotNetNuke 5.0.1.

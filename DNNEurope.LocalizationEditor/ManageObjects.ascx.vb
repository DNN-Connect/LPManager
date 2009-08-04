Imports System.Collections.Generic
Imports System.IO
Imports System.Xml

Imports DotNetNuke.Services.Localization.Localization

Imports DNNEurope.Modules.LocalizationEditor.Business

Namespace DNNEurope.Modules.LocalizationEditor

    Partial Public Class ManageObjects
        Inherits ModuleBase

#Region " Event Handlers "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            '// Force full postback when using upload control
            DotNetNuke.Framework.AJAX.RegisterPostBackControl(lbImportPackage)

            If Not Me.IsPostBack Then
                BindData()
            End If
        End Sub

        ''' <summary>
        ''' Import a module using a module install package into the localization editor
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub lbImportPackage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lbImportPackage.Click
            '// Check if a file is given
            If Not ctlUpload.HasFile Then
                lblUploadError.Text = GetString("NoFile", LocalResourceFile)
                Return
            End If

            ImportModulePackage(ctlUpload.FileName)

            '// Reload data
            BindData()
        End Sub

        ''' <summary>
        ''' Import an installed module into the localization editor
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub lbImportInstalledModule_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lbImportInstalledObject.Click
            '// Get the id of the installed module
            Dim desktopModuleId As Integer
            If Not Integer.TryParse(ddlInstalledObjects.SelectedValue, desktopModuleId) Then
                Return
            End If

            '// Get the desktopmodule information 
            Dim dm As DotNetNuke.Entities.Modules.DesktopModuleInfo = DotNetNuke.Entities.Modules.DesktopModuleController.GetDesktopModule(desktopModuleId, PortalId)

            '// Add the module into the localization editor
            Dim tm As New ObjectInfo(0, dm.ModuleName, dm.FriendlyName)
            tm.ObjectId = ObjectController.AddObject(tm)

            '// Process the resources for the module
            LocalizationController.ReadResourceFiles(Server.MapPath("~/"), PortalId, tm, UserId)

            '// Reload data
            BindData()
        End Sub

        Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
            Me.Response.Redirect(DotNetNuke.Common.NavigateURL, False)
        End Sub

        Private Sub dlTranslateObjects_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlTranslateObjects.DeleteCommand
            Dim ObjectId As Integer = CInt(dlTranslateObjects.DataKeys(e.Item.ItemIndex))
            ObjectController.DeleteObject(ObjectId)
            BindData()
        End Sub
#End Region

#Region " Private Methods "
        Private Sub BindData()
            '// Load all imported modules
            Dim translatedModules As ArrayList = ObjectController.GetObjectList()
            dlTranslateObjects.DataSource = translatedModules
            dlTranslateObjects.DataBind()

            '// Load the installed modules
            ddlInstalledObjects.Items.Clear()
            For Each dm As DotNetNuke.Entities.Modules.DesktopModuleInfo In DotNetNuke.Entities.Modules.DesktopModuleController.GetDesktopModules(PortalId).Values
                '// If the module is not already imported add it to the list
                Dim isImported As Boolean = False
                For Each tm As ObjectInfo In translatedModules
                    If tm.ObjectName = dm.ModuleName Then
                        isImported = True
                        Exit For
                    End If
                Next
                If Not isImported Then
                    ddlInstalledObjects.Items.Add(New ListItem(dm.FriendlyName, dm.DesktopModuleID.ToString))
                End If
            Next
        End Sub

        Private Class ManifestModuleInfo
            Public ModuleName As String
            Public FriendlyName As String
            Public FolderName As String
            Public Version As String
            Public ResourceFiles As New SortedList
        End Class

        ''' <summary>
        ''' Imports a module and it's resources using a module package
        ''' </summary>
        ''' <param name="modulePackagePath">The path of the module package</param>
        ''' <remarks></remarks>
        Private Sub ImportModulePackage(ByVal modulePackagePath As String)
            '// Create a temporary directory to unpack the package
            Dim tempDirectory As String = PortalSettings.HomeDirectoryMapPath & "LocalizationEditor\~tmp" & Now.ToString("yyyyMMdd-hhmmss") & "-" & (CInt(Rnd() * 1000)).ToString

            '// Check if the temporary directory already exists
            If Not IO.Directory.Exists(tempDirectory) Then
                IO.Directory.CreateDirectory(tempDirectory)
            Else
                Throw New IO.IOException("New directory " & tempDirectory & " already exists")
            End If

            '// Unzip file contents
            ZipHelper.Unzip(ctlUpload.FileContent, tempDirectory)

            '// Find the DNN manifest file
            Dim dnnFiles As String() = IO.Directory.GetFiles(tempDirectory, "*.dnn")
            If dnnFiles.Length = 0 Then Throw New IO.FileNotFoundException("No DNN Manifest file found")
            If dnnFiles.Length > 1 Then Throw New IO.IOException("Multiple DNN Manifest files found")
            Dim dnnManifestFile As String = dnnFiles(0)

            '// Parse DNN manifest file
            Dim manifest As New XmlDocument
            manifest.Load(dnnManifestFile)
            Dim manifestModules As New List(Of ManifestModuleInfo)
            Dim manifestVersion As Integer = 0

            '// Determine version of manifest file
            Dim mainNodes As XmlNodeList
            mainNodes = manifest.SelectNodes("dotnetnuke/packages/package")
            If mainNodes.Count > 0 Then
                manifestVersion = 5
            Else
                mainNodes = manifest.SelectNodes("dotnetnuke/folders/folder")
                If mainNodes.Count > 0 Then
                    manifestVersion = 3
                End If
            End If

            If manifestVersion = 5 Then
                '// Remark about DNN 5 manifest file: it is assumed that only one <desktopModule> node per <package> node exists.

                '// Create a module for each package
                For Each packageNode As XmlNode In mainNodes
                    Dim manifestModule As New ManifestModuleInfo()

                    '// Determine the version
                    If Not packageNode.SelectSingleNode("@version") Is Nothing Then
                        manifestModule.Version = packageNode.SelectSingleNode("@version").InnerText.Trim
                        If String.IsNullOrEmpty(manifestModule.Version) Then
                            manifestModule.Version = "0"    '2009-06-26 Janga:  Default version.
                        End If
                    Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
                    End If

                    '// Determine the desktop module
                    Dim moduleNodes As XmlNodeList
                    moduleNodes = packageNode.SelectNodes("components/component/desktopModule")
                    If moduleNodes.Count = 0 Then
                        Throw New Exception("Could not retrieve desktop module information in DNN Manifest file")
                    End If
                    If moduleNodes.Count > 1 Then
                        Throw New Exception("Multiple desktop modules found in DNN Manifest file")
                    End If

                    For Each dmNode As XmlNode In moduleNodes
                        '// Determine the folder name
                        If Not dmNode("foldername") Is Nothing Then
                            manifestModule.FolderName = dmNode("foldername").InnerText.Replace("/"c, "\"c)
                        Else : Throw New Exception("Could not retrieve folder information in DNN Manifest file")
                        End If

                        '// Determine the module name
                        If Not dmNode("moduleName") Is Nothing Then
                            manifestModule.ModuleName = dmNode("moduleName").InnerText.Replace("/"c, "\"c)
                        Else : Throw New Exception("Could not retrieve module name in DNN Manifest file")
                        End If
                    Next

                    '// Determine the friendly name
                    If Not packageNode("friendlyName") Is Nothing Then
                        manifestModule.FriendlyName = packageNode("friendlyName").InnerText
                    Else : manifestModule.FriendlyName = manifestModule.ModuleName
                    End If

                    '// Find the resource files using the manifest xml
                    Dim resourceFiles As String = ""
                    For Each node As XmlNode In packageNode.SelectNodes("components/component/files/file")
                        Dim resFile As String = ""
                        Dim resDir As String = ""
                        If Not node("name") Is Nothing Then resFile = node("name").InnerText
                        If Not node("path") Is Nothing Then resDir = node("path").InnerText

                        'TODO Support resource files which are already localized
                        If resFile.ToLower.EndsWith("ascx.resx") OrElse resFile.ToLower.EndsWith("aspx.resx") Then
                            '// Determine the resource directory and key for the module
                            Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
                            Dim resKey As String = Path.Combine(Path.Combine(Path.Combine("DesktopModules", manifestModule.FolderName), resDir), resFile)

                            manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
                        End If
                    Next

                    '// Add the manifest module to the collection
                    manifestModules.Add(manifestModule)
                Next
            ElseIf manifestVersion = 3 Then
                '// Create a module for each folder
                For Each folderNode As XmlNode In mainNodes
                    Dim manifestModule As New ManifestModuleInfo()

                    '// Determine the module name
                    If Not folderNode("modulename") Is Nothing Then
                        manifestModule.ModuleName = folderNode("modulename").InnerText
                    ElseIf Not folderNode("friendlyname") Is Nothing Then
                        manifestModule.ModuleName = folderNode("friendlyname").InnerText
                    ElseIf Not folderNode("name") Is Nothing Then
                        manifestModule.ModuleName = folderNode("name").InnerText
                    Else : Throw New Exception("Could not retrieve module name in DNN Manifest file")
                    End If

                    '// Determine the friendly name
                    If Not folderNode("friendlyname") Is Nothing Then
                        manifestModule.FriendlyName = folderNode("friendlyname").InnerText
                    Else : manifestModule.FriendlyName = manifestModule.ModuleName
                    End If

                    '// Determine the folder name
                    If Not folderNode("foldername") Is Nothing Then
                        manifestModule.FolderName = folderNode("foldername").InnerText.Replace("/"c, "\"c)
                    Else : Throw New Exception("Could not retrieve folder information in DNN Manifest file")
                    End If

                    '// Determine the version
                    If Not folderNode("version") Is Nothing Then
                        manifestModule.Version = folderNode("version").InnerText.Trim
                        If String.IsNullOrEmpty(manifestModule.Version) Then
                            manifestModule.Version = "0"    '2009-06-26 Janga:  Default version.
                        End If
                    Else : Throw New Exception("Could not retrieve version information in DNN Manifest file")
                    End If

                    '// Find the resource files using the manifest xml
                    Dim resourceFiles As String = ""
                    For Each node As XmlNode In folderNode.SelectNodes("files/file")
                        Dim resFile As String = ""
                        Dim resDir As String = ""
                        If Not node("name") Is Nothing Then resFile = node("name").InnerText
                        If Not node("path") Is Nothing Then resDir = node("path").InnerText

                        'TODO Support resource files which are already localized
                        If resFile.ToLower.EndsWith("ascx.resx") OrElse resFile.ToLower.EndsWith("aspx.resx") Then
                            '// Determine the resource directory and key for the module
                            Dim resPath As String = Path.Combine(Path.Combine(tempDirectory, resDir), resFile)
                            Dim resKey As String = Path.Combine(Path.Combine(Path.Combine("DesktopModules", manifestModule.FolderName), resDir), resFile)

                            manifestModule.ResourceFiles.Add(resKey, New FileInfo(resPath))
                        End If
                    Next

                    '// Add the manifest module to the collection
                    manifestModules.Add(manifestModule)
                Next
            Else : Throw New Exception("Could not determine version of manifest file")
            End If

            'TODO Use transactions
            '// DNN Manifest file parsed succesfully, now process each module from the manifest
            For Each manifestModule As ManifestModuleInfo In manifestModules

                '// Check if the module is already imported
                Dim objObjectInfo As ObjectInfo = ObjectController.GetObjectByObjectName(manifestModule.ModuleName)
                If objObjectInfo Is Nothing Then
                    '// Create a new translate module
                    objObjectInfo = New ObjectInfo(0, manifestModule.ModuleName, manifestModule.FriendlyName)
                    objObjectInfo.ObjectId = ObjectController.AddObject(objObjectInfo)
                End If

                '// Import or update resource files for this module
                LocalizationController.ProcessResourceFiles(manifestModule.ResourceFiles, tempDirectory, objObjectInfo, manifestModule.Version)
            Next

            'TODO
            ' Read module package manifest and add TranslationModule object
            ' Find the resource files in the package and add them to the DB....only add module package if it contains resource files?

            'Dim tm As New ObjectInfo(0, "Testje", "Lalala")
            'ObjectController.AddObject(tm)
        End Sub
#End Region

    End Class
End Namespace
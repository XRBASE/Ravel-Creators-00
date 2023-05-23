#if UNITY_EDITOR
using System.IO;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentSO))]
public class EnvironmentSOEditor : Editor
{
    private EnvironmentSO _instance;

    //disables interaction while uploading an image or bundle
    private bool _uploadingFile = false;

    //download size for images.
    private ImageSize _size = ImageSize.I1920;

    //fold out state for private environment access.
    private bool _accessOpen = false;

    //coroutine for downloading image, used to cancel download
    private bool _retrievingImg;

    //all used for caching input for when the environment access is changed.
    private string _userMail = "";
    private int _selOrg = 0;
    private string[] _orgNames;
    
    //cache for network errors in retrieving environment, used for showing the error to users.
    private string networkError = "";
    
    /// <summary>
    /// Keep status up to date.
    /// </summary>
    private void OnEnable() {
        if (_instance != null) {
            RefreshEnvironment();
        }
    }

    public override void OnInspectorGUI() {
        _instance = (EnvironmentSO)target;
        
        //draw banner image
        if (_instance.environment.preview == null) {
            _instance.environment.previewSize = ImageSize.None;
        }
        
        if (_instance.environment.previewSize >= ImageSize.I512) {
            RavelEditor.DrawTextureScaledCropGUI(
                new Rect(0, 0, EditorGUIUtility.currentViewWidth, RavelEditor.Branding.bannerHeight),
                _instance.environment.preview.texture, Vector2.one * 0.5f);

            if (RavelEditor.Branding.overlayLogo) {
                RavelEditor.DrawTextureScaledScaleGUI(
                    new Rect(0, 0, EditorGUIUtility.currentViewWidth, RavelEditor.Branding.bannerHeight),
                    RavelEditor.Branding.overlayLogo, Vector2.one * 0.5f, false);
            }
        }
        else if (string.IsNullOrEmpty(networkError) && !_retrievingImg &&
                 _instance.environment.metadataPreviewImage.TryGetUrl(ImageSize.I512, out string url)) {
            _retrievingImg = true;
            EditorImageService.GetSprite(url, ImageSize.I512, OnPreviewRetrieved);
        }

        //Uploading file text (and gui disable)
        if (_uploadingFile) {
            GUILayout.Label("Uploading file, please wait...");
            GUI.enabled = false;
        }

        GUITitle();

        //environment download error handling
        if (!string.IsNullOrEmpty(networkError)) {
            EditorGUILayout.HelpBox($"Download error :{networkError}", MessageType.Error);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh environment")) {
                RefreshEnvironment();
            }

            Color prevCol = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete local file") && EditorUtility.DisplayDialog("Delete environment",
                    "Are you sure you want to delete this environment", "Yes", "No")) {
                DeleteLocalAsset();
            }
            GUI.backgroundColor = prevCol;

            GUILayout.EndHorizontal();
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        if (string.IsNullOrEmpty(_instance.bundleName)) {
            _instance.bundleName = _instance.environment.name;
            EditorUtility.SetDirty(_instance);
        }
        GUILayout.Label("Assetbundle name:", GUILayout.Width(RavelEditorStying.GUI_SPACING_DECA));
        EditorGUI.BeginChangeCheck();
        _instance.bundleName = GUILayout.TextField(_instance.bundleName);
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(_instance);
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
        
        if (RavelEditor.DevUser && GUILayout.Button("copy UUID")) {
            GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);
            GUIUtility.systemCopyBuffer = _instance.environment.environmentUuid;
            Debug.Log("UUID copied!");
        }
        
        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);

        //used to determine what image to download.
        _size = (ImageSize)EditorGUILayout.EnumPopup("image url size:", _size);
        GUIDataUrls();

        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);

        if (!(_instance.environment.published || _instance.environment.submissionInProgress)) {
            //upload and publish
            GUIServerFunctions();
            GUIDelete();
        }
        else {
            if (_instance.environment.published) {
                GUILayout.Label("Upload only available for unpublished environments.");

                if (!_instance.environment.isPublic) {
                    GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);
                    _accessOpen = EditorGUILayout.Foldout(_accessOpen, "Manage access");
                    if (_accessOpen) {
                        RavelEditor.GUIBeginIndent(RavelEditorStying.GUI_SPACING_MILLI);

                        GUIChangeAccess();
                        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);

                        GUIDrawAccess();

                        RavelEditor.GUIEndIndent();
                    }
                }
            }
            else {
                GUILayout.Label("This environment is currently in review and cannot be changed");
                GUIDelete();
            }
        }

        GUI.enabled = true;
    }

//GUI methods for this class that have been simplified in the OnInspectorGUI to improve readability

#region draw GUI methods

    /// <summary>
    /// Draw environment title and refresh button.
    /// </summary>
    private void GUITitle() {
        EditorGUILayout.BeginHorizontal();
        int fontSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = RavelBranding.titleFont;

        GUILayout.Label(_instance.environment.name);
        if (GUILayout.Button("Refresh", GUILayout.Width(RavelEditorStying.GUI_SPACING))) {
            RefreshEnvironment();
        }

        GUI.skin.label.fontSize = fontSize;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Download bundle and image urls, with save and copy buttons.
    /// </summary>
    private void GUIDataUrls() {
        _instance.environment.metadataPreviewImage.TryGetUrl(_size, out string url);
        GUIDrawCopySaveUrl("Image", url, $"IMG_{_instance.environment.name}_{_size.ToString().Substring(1)}", "jpg", 
            !_uploadingFile);

        url = _instance.environment.metadataAssetBundle.assetBundleUrl;
        GUIDrawCopySaveUrl( "Bundle", url, $"BUN_{_instance.environment.name}", "", 
            !_uploadingFile);
    }
    
    private void GUIDrawCopySaveUrl(string label, string url, string fileName, string extension, bool enabled) {
        bool empty = false;
        if (string.IsNullOrEmpty(url)) {
            url = $"No url for {label}.";
            empty = true;
        }
        
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = false;
        if (empty) {
            GUILayout.TextField(url, GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelEditorStying.GUI_SPACING * 2f));
        }
        else {
            GUILayout.TextField($"{label}: \t\t{url}", GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelEditorStying.GUI_SPACING * 2f));
        }
        
        GUI.enabled = !empty && enabled;

        if (GUILayout.Button("Save")) {
            string path = EditorUtility.SaveFilePanel("Save", RavelEditor.CreatorConfig.GetFilePath(),fileName, extension);

            if (!string.IsNullOrEmpty(path)) {
                RavelEditor.CreatorConfig.SetFilePath(path);
                RavelWebRequest req = new RavelWebRequest(url, RavelWebRequest.Method.Get);
                EditorWebRequests.DownloadAndSave(req, path, false, _instance);
            }
        }

        if (GUILayout.Button("copy")) {
            GUIUtility.systemCopyBuffer = url;
            Debug.Log("Url copied!");
        }

        EditorGUILayout.EndHorizontal();
        GUI.enabled = enabled;
    }

    /// <summary>
    /// Draw upload bundle and image and publish button.
    /// </summary>
    private void GUIServerFunctions() {
        if (GUILayout.Button("Upload loading image")) {
            string path = EditorUtility.OpenFilePanel("Upload new loading image", RavelEditor.CreatorConfig.GetFilePath(), RavelEditorStying.IMAGE_EXTENSIONS);

            if (!string.IsNullOrEmpty(path)) {
                RavelEditor.CreatorConfig.SetFilePath(path);
                RavelWebRequest req = CreatorRequest.UploadPreview(_instance.environment.environmentUuid, path);
                EditorWebRequests.SendWebRequest(req, OnImageUploaded, this);
                _uploadingFile = true;
            }
        }

        if (GUILayout.Button("Upload bundle")) {
            string path = EditorUtility.OpenFilePanel("Upload new asset bundle", RavelEditor.CreatorConfig.GetFilePath(), "");

            if (!string.IsNullOrEmpty(path)) {
                RavelEditor.CreatorConfig.SetFilePath(path);
                RavelWebRequest req = CreatorRequest.UploadBundle(_instance.environment.environmentUuid, path);
                EditorWebRequests.SendWebRequest(req, OnBundleUploaded, this);
                _uploadingFile = true;
            }
        }

        if (GUILayout.Button("Publish") && EditorUtility.DisplayDialog("Publish environment",
                "Are you sure you want to publish the current version of this environment?", "Publish", "Cancel")) {

            RavelWebRequest req = CreatorRequest.PublishEnvironment(_instance.environment.environmentUuid);
            EditorWebRequests.SendWebRequest(req, OnRemoteEnvironmentPublished, this);
        }
    }

    /// <summary>
    /// Delete file button with dialog.
    /// </summary>
    private void GUIDelete() {
        Color prevCol = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete environment") && EditorUtility.DisplayDialog("Delete environment",
                "Are you sure you want to delete this environment?", "Delete", "Keep")) {

            RavelWebRequest req = CreatorRequest.DeleteEnvironment(_instance.environment);
            EditorWebRequests.SendWebRequest(req, OnRemoteEnvironmentDeleted, this);
        }
        GUI.backgroundColor = prevCol;
    }

    /// <summary>
    /// Draw change functions for environment access.
    /// </summary>
    private void GUIChangeAccess() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("User e-mail:", GUILayout.Width(100));
        _userMail = GUILayout.TextField(_userMail);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add user") && !string.IsNullOrEmpty(_userMail)) {
            RavelWebRequest req =
                EnvironmentAccessRequest.AddAccessUser(_instance.environment.environmentUuid, _userMail);
            EditorWebRequests.SendWebRequest(req, OnAccessUpdateReceived, this);
        }

        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);

        if (_orgNames == null) {
            RavelEditor.GetUserOrganisations(OnOrganisationsRetrieved, this);
        }
        else {
            _selOrg = EditorGUILayout.Popup("organisation", _selOrg, _orgNames);

            if (GUILayout.Button("Add organisation")) {
                RavelWebRequest req = EnvironmentAccessRequest.AddAccessOrganisation(
                    _instance.environment.environmentUuid,
                    RavelEditor.User.Organisations[_selOrg].organizationId);
                EditorWebRequests.SendWebRequest(req, OnAccessUpdateReceived, this);
            }
        }
    }

    /// <summary>
    /// Draw current access to the environment.
    /// </summary>
    private void GUIDrawAccess() {
        if (_instance.environment.userList != null && _instance.environment.userList.Length > 0) {
            GUILayout.Label("Users who have access:");

            foreach (var user in _instance.environment.userList) {
                DrawAccessor(user.FullName, user.email, true);
            }
        }

        if (_instance.environment.organizations != null && _instance.environment.organizations.Length > 0) {
            GUILayout.Label("Organisations that have access:");

            foreach (var org in _instance.environment.organizations) {
                DrawAccessor(org.organizationName, org.organizationId, false);
            }
        }
    }

    /// <summary>
    /// Draw name field for accessor to private environment. This includes a remove button.
    /// </summary>
    /// <param name="name">name to show in field.</param>
    /// <param name="id">identifier for remove call (user's use their email, organisation user their uuid).</param>
    /// <param name="isUser">is this a user or an organisation.</param>
    private void DrawAccessor(string name, string id, bool isUser) {
        GUILayout.BeginHorizontal();

        bool pEnabled = GUI.enabled;
        GUI.enabled = false;
        GUILayout.TextField(name);
        GUI.enabled = pEnabled;

        if (GUILayout.Button("Remove", GUILayout.Width(RavelEditorStying.GUI_SPACING))) {
            RavelWebRequest req = (isUser)
                ? EnvironmentAccessRequest.DeleteAccessUser(_instance.environment.environmentUuid, id)
                : EnvironmentAccessRequest.DeleteAccessOrganisation(_instance.environment.environmentUuid, id);
            EditorWebRequests.SendWebRequest(req, OnAccessUpdateReceived, this);
        }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Cache user organisation names, so they can be added to the environment.
    /// </summary>
    /// <param name="organisations">List of users organisations.</param>
    /// <param name="success">Did the webcall succeed?</param>
    private void OnOrganisationsRetrieved(Organisation[] organisations, bool success) {
        if (success) {
            _orgNames = new string[organisations.Length];
            for (int i = 0; i < _orgNames.Length; i++) {
                _orgNames[i] = organisations[i].organizationName;
            }
        }
    }

#endregion

    /// <summary>
    /// Deletes the scriptable object reference.
    /// </summary>
    public void DeleteLocalAsset() {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// Make call to backend to refresh this environment.
    /// </summary>
    public void RefreshEnvironment() {
        RavelWebRequest req = CreatorRequest.GetCreatorEnvironment(_instance.environment.environmentUuid);
        EditorWebRequests.SendWebRequest(req, OnEnvironmentUpdateReceived, this);
    }
    
    /// <summary>
    /// Callback for when new environment data has been recieved from the server. 
    /// </summary>
    /// <param name="res">Server response data.</param>
    private void OnEnvironmentUpdateReceived(RavelWebResponse res) {
        if (res.Success) {
            res.DataString = EnvironmentExtensions.RenameStringFromBackend(res.DataString);

            if (res.TryGetData(out Environment env)) {
                _instance.environment = env;
            }

            RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(_instance.environment.environmentUuid);
            EditorWebRequests.GetDataRequest<Environment>(req, UpdateEnvironmentAccess, this);
        }
        else {
            networkError = res.Error.FullMessage;
            Debug.LogError($"Error refreshing environment {_instance.environment.name}");
        }
    }
    
    /// <summary>
    /// Callback from the server when updating the access to a private environment.
    /// </summary>
    /// <param name="res">WebResponse data</param>
    public void OnAccessUpdateReceived(RavelWebResponse res) {
        if (res.Success) {
            RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(_instance.environment.environmentUuid);
            EditorWebRequests.GetDataRequest<Environment>(req, UpdateEnvironmentAccess, this);
        }
    }
    
    /// <summary>
    /// Update user or organisation access to this environment.
    /// </summary>
    /// <param name="toCopy">Environment containing the updated acces</param>
    /// <param name="success"></param>
    private void UpdateEnvironmentAccess(Environment toCopy, bool success) {
        //error in webcall
        if (!success)
            return;
        
        _instance.environment.userList = toCopy.userList;
        _instance.environment.organizations = toCopy.organizations;
    }
    
    /// <summary>
    /// Called by image retrieval service, updates the image of the connected environment.
    /// </summary>
    private void OnPreviewRetrieved(Sprite spr, ImageSize size) {
        _instance.environment.UpdateEnvironmentSprite(spr, size);
        _retrievingImg = false;
    }

    /// <summary>
    /// Callback for when the environment has been deleted on the backend. Deletes the scriptable object.
    /// </summary>
    private void OnRemoteEnvironmentDeleted(RavelWebResponse res) {
        if (res.Success) {
            EditorUtility.DisplayDialog("Delete environment",
                "Environment was deleted on server, deleting reference in project as well.", "ok");
            DeleteLocalAsset();
        }
        else {
            Debug.LogError($"There was an error publishing this environment: ({res.Error.FullMessage})!");
        }
    }

    /// <summary>
    /// Callback for when the environment has been published on the backend.
    /// </summary>
    private void OnRemoteEnvironmentPublished(RavelWebResponse res) {
        if (res.Success) {
            EditorUtility.DisplayDialog("Publish environment", "Environment was published successfully!", "ok");
            RefreshEnvironment();
        }
        else {
            Debug.LogError($"There was an error publishing this environment: ({res.Error.FullMessage})!");
        }
    }

    /// <summary>
    /// Callback for when the image has been uploaded to the backend.
    /// </summary>
    public void OnImageUploaded(RavelWebResponse res) {
        if (res.Success) {
            Debug.Log("Preview image uploaded successfully");
        }

        _uploadingFile = false;
        RefreshEnvironment();
    }

    /// <summary>
    /// Callback for when the bundle has been uploaded to the backend.
    /// </summary>
    public void OnBundleUploaded(RavelWebResponse res) {
        if (res.Success) {
            Debug.Log("Asset bundle uploaded successfully");
        }

        _uploadingFile = false;
        RefreshEnvironment();
    }
}
#endif
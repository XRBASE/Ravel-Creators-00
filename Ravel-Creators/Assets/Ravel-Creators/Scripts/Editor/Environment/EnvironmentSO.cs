using UnityEngine;
#if UNITY_EDITOR
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif

public class EnvironmentSO : ScriptableObject
{
    public Environment environment;
    
#if UNITY_EDITOR

    private void OnEnable() {
        if (environment != null) {
            RefreshEnvironment();
        }
    }
    
    public void RefreshEnvironment() {
        RavelWebRequest req = CreatorRequest.GetCreatorEnvironment(environment.environmentUuid);
        EditorWebRequests.SendWebRequest(req, OnEnvironmentUpdate, this);
    }

    private void OnEnvironmentUpdate(RavelWebResponse res) {
        if (res.Success) {
            res.DataString = EnvironmentExtensions.RenameStringFromBackend(res.DataString);

            if (res.TryGetData(out Environment env)) {
                environment = env;
            }

            RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(environment.environmentUuid);
            EditorWebRequests.GetDataRequest<Environment>(req, UpdateEnvironmentAccess, this);
        }
    }
    
    private void UpdateEnvironmentAccess(Environment toCopy, bool success) {
        //error in webcall
        if (!success)
            return;
        
        environment.userList = toCopy.userList;
        environment.organizations = toCopy.organizations;
    }

    [CustomEditor(typeof(EnvironmentSO))]
    private class EnvironmentSOEditor : Editor
    {
        private EnvironmentSO _instance;
        
        private bool _uploadingFile = false;
        private ImageSize _size = ImageSize.I1920;
        
        private bool _accessOpen = false;

        private EditorCoroutine getImgRoutine = null;

        private void OnPreviewRetrieved(Sprite spr, ImageSize size) {
            _instance.environment.UpdateEnvironmentSprite(spr, size);
            getImgRoutine = null;
        }

        public override void OnInspectorGUI() {
            _instance = (EnvironmentSO)target;
            
            if (_instance.environment.previewSize >= ImageSize.I512) {
                RavelEditor.DrawTextureScaledCropGUI(new Rect(0, 0, EditorGUIUtility.currentViewWidth, RavelBranding.BANNER_HEIGHT), 
                    _instance.environment.preview.texture, Vector2.one * 0.5f);

                if (RavelEditor.Branding.overlayLogo) {
                    RavelEditor.DrawTextureScaledScaleGUI(new Rect(0, 0, EditorGUIUtility.currentViewWidth, RavelBranding.BANNER_HEIGHT),
                        RavelEditor.Branding.overlayLogo, Vector2.one * 0.5f, false);
                }
            }
            else if (getImgRoutine == null && _instance.environment.metadataPreviewImage.TryGetUrl(ImageSize.I512, out string url)) {
                getImgRoutine = EditorImageService.GetSpriteRoutine(url, ImageSize.I512, OnPreviewRetrieved, this);
            }
            
            if (_uploadingFile) {
                GUILayout.Label("Uploading file, please wait...");
                GUI.enabled = false;
            }

            GUITitle();
            
            DrawDefaultInspector();
            
            if (GUILayout.Button("copy UUID")) {
                GUIUtility.systemCopyBuffer = _instance.environment.environmentUuid;
                Debug.Log("UUID copied!");
            }

            _size = (ImageSize)EditorGUILayout.EnumPopup("image url size:", _size);
            
            GUIDataUrls();

            if (!(_instance.environment.published || _instance.environment.submissionInProgress)) {
                GUIServerFunctions();
                GUIDelete();
            }
            else {
                if (_instance.environment.published) {
                    GUILayout.Label("Upload only available for unpublished environments.");
                    if (!_instance.environment.isPublic) {
                        GUILayout.Space(RavelBranding.SPACING_SMALL);
                        _accessOpen = EditorGUILayout.Foldout(_accessOpen, "Manage access");
                        if (_accessOpen) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space(RavelBranding.INDENT_SMALL);
                            EditorGUILayout.BeginVertical();

                            GUIChangeAccess();
                            GUILayout.Space(RavelBranding.INDENT_SMALL);
                            
                            GUIDrawAccess();

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
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

#region draw GUI methods

        private void GUITitle() {
            EditorGUILayout.BeginHorizontal();
            int fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = RavelBranding.FONT_TITLE;
            
            GUILayout.Label(_instance.environment.name);
            if (GUILayout.Button("Refresh", GUILayout.Width(RavelBranding.HORI_BTN_SMALL))) {
                _instance.RefreshEnvironment();
            }
            
            GUI.skin.label.fontSize = fontSize;
            EditorGUILayout.EndHorizontal();
        }

        private void GUIDataUrls() {
            string url;
            bool error = false;
            if (!_instance.environment.metadataPreviewImage.TryGetUrl(_size, out url)) {
                url = $"Error recieving url of size {_size}!";
                error = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            GUILayout.TextField($"Image url \t\t{url}", GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelBranding.HORI_BTN_SMALL * 2f));
            GUI.enabled = !(error || _uploadingFile);
            if (GUILayout.Button("Save")) {
                string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                    $"IMG_{_instance.environment.name}_{_size.ToString().Substring(1)}", "jpg");

                if (!string.IsNullOrEmpty(path)) {
                    RavelWebRequest req = new RavelWebRequest(url, RavelWebRequest.Method.Get);
                    EditorWebRequests.DownloadAndSave(req, path, true, _instance);
                }
            }
            
            if (GUILayout.Button("copy")) {
                GUIUtility.systemCopyBuffer = url;
                Debug.Log("Url copied!");
            }
            GUI.enabled = !_uploadingFile;
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_instance.environment.metadataAssetBundle.assetBundleUrl)) {
                url = _instance.environment.metadataAssetBundle.assetBundleUrl;
                
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                GUILayout.TextField($"Bundle url \t\t{url}", GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelBranding.HORI_BTN_SMALL * 2f));
                GUI.enabled = !_uploadingFile;
                
                if (GUILayout.Button("Save")) {
                    string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                        $"BUN_{_instance.environment.name}", "");

                    if (!string.IsNullOrEmpty(path)) {
                        RavelWebRequest req = new RavelWebRequest(url, RavelWebRequest.Method.Get);
                        EditorWebRequests.DownloadAndSave(req, path, false, _instance);
                    }
                }
                
                if (GUILayout.Button("copy")) {
                    GUIUtility.systemCopyBuffer = url;
                    Debug.Log("Url copied!");
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }

        private void GUIServerFunctions() {
            if (GUILayout.Button("Upload preview")) {
                string path = EditorUtility.OpenFilePanel("Upload new preview", Application.dataPath, "jpg,jpeg,png");

                if (!string.IsNullOrEmpty(path)) {
                    RavelWebRequest req = CreatorRequest.UploadPreview(_instance.environment.environmentUuid, path);
                    EditorWebRequests.SendWebRequest(req, OnImageUploaded, this);
                    _uploadingFile = true;
                }
            }
            
            if (GUILayout.Button("Upload bundle")) {
                string path = EditorUtility.OpenFilePanel("Upload new asset bundle", Application.dataPath, "");

                if (!string.IsNullOrEmpty(path)) {
                    RavelWebRequest req = CreatorRequest.UploadBundle(_instance.environment.environmentUuid, path);
                    EditorWebRequests.SendWebRequest(req, OnBundleUploaded, this);
                    _uploadingFile = true;
                }
            }
            
            if (GUILayout.Button("Publish") && EditorUtility.DisplayDialog("Publish environment", 
                    "Are you sure you want to publish the current version of this environment?", "Publish", "Cancel")) {
                
                RavelWebRequest req = CreatorRequest.PublishEnvironment(_instance.environment.environmentUuid);
                EditorWebRequests.SendWebRequest(req, OnEnvironmentPublished, this);
            }
        }

        private void GUIDelete() {
            if (GUILayout.Button("Delete environment")&& EditorUtility.DisplayDialog("Delete environment", 
                    "Are you sure you want to delete this environment?", "Delete", "Keep")) {
                
                RavelWebRequest req = CreatorRequest.DeleteEnvironment(_instance.environment);
                EditorWebRequests.SendWebRequest(req, OnEnvironmentDeleted, this);
            }
        }

        
        private string _userMail = "";
        private int _selOrg = 0;
        private string[] _orgNames;

        private void GUIChangeAccess() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("User e-mail:", GUILayout.Width(100));
            _userMail = GUILayout.TextField(_userMail);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add user") && !string.IsNullOrEmpty(_userMail)) {
                RavelWebRequest req =
                    EnvironmentAccessRequest.AddAccessUser(_instance.environment.environmentUuid, _userMail);
                EditorWebRequests.SendWebRequest(req, (res) => OnAccessAdded(res, _userMail), this);
            }
            
            GUILayout.Space(RavelBranding.INDENT_SMALL);

            if (_orgNames == null) {
                RavelEditor.GetUserOrganisations(OnOrganisationsRetrieved, this);
            }
            else {
                _selOrg = EditorGUILayout.Popup("organisation", _selOrg, _orgNames);

                if (GUILayout.Button("Add organisation")) {
                    RavelWebRequest req = EnvironmentAccessRequest.AddAccessOrganisation(
                        _instance.environment.environmentUuid,
                        RavelEditor.User.Organisations[_selOrg].organizationId);
                    EditorWebRequests.SendWebRequest(req,
                        (res) => OnAccessAdded(res, RavelEditor.User.Organisations[_selOrg].organizationName),
                        this);
                }
            }
        }

        private void GUIDrawAccess() {
            GUI.enabled = false;
            if (_instance.environment.userList != null && _instance.environment.userList.Length > 0) {
                GUILayout.Label("Users who have access:");
                string userList = "";
                for (int i = 0; i < _instance.environment.userList.Length; i++) {
                    userList += _instance.environment.userList[i].FullName + ", \n";
                }

                userList = userList.Substring(0, userList.Length - 3);
                GUILayout.TextArea(userList);
            }
            
            if (_instance.environment.organizations != null && _instance.environment.organizations.Length > 0) {
                GUILayout.Label("Organisations that have access:");
                string orgList = "";
                for (int i = 0; i < _instance.environment.organizations.Length; i++) {
                    orgList += _instance.environment.organizations[i].organizationName + ", \n";
                }

                orgList = orgList.Substring(0, orgList.Length - 3);
                GUILayout.TextArea(orgList);
            }
            GUI.enabled = true;
        }

        private void OnOrganisationsRetrieved(Organisation[] organisations, bool success) {
            if (success) {
                _orgNames = new string[organisations.Length];
                for (int i = 0; i < _orgNames.Length; i++) {
                    _orgNames[i] = organisations[i].organizationName;
                }
            }
        }

        private void OnAccessAdded(RavelWebResponse res, string addedFor) {
            if (res.Success) {
                Debug.Log($"Added access for {addedFor}");
                
                RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(_instance.environment.environmentUuid);
                EditorWebRequests.GetDataRequest<Environment>(req, _instance.UpdateEnvironmentAccess, this);
            }
        }

#endregion

        private void OnEnvironmentDeleted(RavelWebResponse res) {
            if (res.Success) {
                EditorUtility.DisplayDialog("Delete environment", "Environment was deleted on server, deleting reference in project as well.", "ok");
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_instance));
                AssetDatabase.Refresh();
            }
            else {
                Debug.LogError($"There was an error publishing this environment: ({res.Error.FullMessage})!");
            }
        }
        
        private void OnEnvironmentPublished(RavelWebResponse res) {
            if (res.Success) {
                EditorUtility.DisplayDialog("Publish environment", "Environment was published successfully!", "ok");
                _instance.RefreshEnvironment();
            }
            else {
                Debug.LogError($"There was an error publishing this environment: ({res.Error.FullMessage})!");
            }
        }
        
        public void OnImageUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Preview image uploaded successfully");
            }

            _uploadingFile = false;
            _instance.RefreshEnvironment();
        }
        
        public void OnBundleUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Asset bundle uploaded successfully");
            }
            
            _uploadingFile = false;
            _instance.RefreshEnvironment();
        }
    }
#endif
}

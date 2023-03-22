using UnityEngine;
#if UNITY_EDITOR
using Base.Ravel.Networking;
using UnityEditor;
#endif

public class EnvironmentSO : ScriptableObject
{
    public Environment environment;
    
#if UNITY_EDITOR
    [CustomEditor(typeof(EnvironmentSO))]
    private class EnvironmentSOEditor : Editor
    {
        private EnvironmentSO _instance;
        
        private bool _uploadingFile = false;
        private ImageSize _size = ImageSize.I1920;
        
        private SerializedProperty _envProp;

        private void OnEnable() {
            _envProp = serializedObject.FindProperty("environment");
        }

        public override void OnInspectorGUI() {
            _instance = (EnvironmentSO)target;
            
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
            
            GUIUpload();
            
            GUI.enabled = true;
        }

#region draw GUI methods

        private void GUITitle() {
            EditorGUILayout.BeginHorizontal();
            int fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = RavelBranding.FONT_TITLE;
            
            GUILayout.Label(_instance.environment.name);
            if (GUILayout.Button("Refresh", GUILayout.Width(RavelBranding.HORI_BTN_SMALL))) {
                RefreshEnvironment();
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
            GUI.enabled = !error;
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

        private void GUIUpload() {
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
        }

#endregion

        public void RefreshEnvironment() {
            RavelWebRequest req = CreatorRequest.GetCreatorEnvironment(_instance.environment.environmentUuid);
            EditorWebRequests.SendWebRequest(req, OnEnvironmentUpdate, this);
        }

        private void OnEnvironmentUpdate(RavelWebResponse res) {
            if (res.Success && res.TryGetData(out Environment env)) {
                _instance.environment = env;
            }
        }
        
        public void OnImageUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Preview image uploaded successfully");
            }

            _uploadingFile = false;
            RefreshEnvironment();
        }
        
        public void OnBundleUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Asset bundle uploaded successfully");
            }
            
            _uploadingFile = false;
            RefreshEnvironment();
        }
    }
#endif
}

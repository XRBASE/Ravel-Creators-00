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
        ImageSize _size = ImageSize.I1920;

        public override void OnInspectorGUI() {
            EnvironmentSO instance = (EnvironmentSO)target; 
            DrawDefaultInspector();

            _size = (ImageSize)EditorGUILayout.EnumPopup("image url size:", _size);
            
            string url;
            bool error = false;
            if (!instance.environment.metadataPreviewImage.TryGetUrl(_size, out url)) {
                url = $"Error recieving url of size {_size}!";
                error = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            GUILayout.TextField($"Image url \t\t{url}", GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelBranding.HORI_BTN_SMALL * 2f));
            GUI.enabled = !error;
            if (GUILayout.Button("Save")) {
                string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                    $"IMG_{instance.environment.name}_{_size.ToString().Substring(1)}", "jpg");

                if (!string.IsNullOrEmpty(path)) {
                    RavelWebRequest req = new RavelWebRequest(url, RavelWebRequest.Method.Get);
                    EditorWebRequests.DownloadAndSave(req, path, true, instance);
                }
            }
            
            if (GUILayout.Button("copy")) {
                GUIUtility.systemCopyBuffer = url;
                Debug.Log("Url copied!");
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(instance.environment.metadataAssetBundle.assetBundleUrl)) {
                url = instance.environment.metadataAssetBundle.assetBundleUrl;
                
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                GUILayout.TextField($"Bundle url \t\t{url}", GUILayout.Width(EditorGUIUtility.currentViewWidth - RavelBranding.HORI_BTN_SMALL * 2f));
                GUI.enabled = true;
                
                if (GUILayout.Button("Save")) {
                    string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                        $"BUN_{instance.environment.name}", "");

                    if (!string.IsNullOrEmpty(path)) {
                        RavelWebRequest req = new RavelWebRequest(url, RavelWebRequest.Method.Get);
                        EditorWebRequests.DownloadAndSave(req, path, false, instance);
                    }
                }
                
                if (GUILayout.Button("copy")) {
                    GUIUtility.systemCopyBuffer = url;
                    Debug.Log("Url copied!");
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}

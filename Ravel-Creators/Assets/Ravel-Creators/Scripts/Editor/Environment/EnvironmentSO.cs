using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEngine.Networking;
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
            
            if (GUILayout.Button("copy UUID")) {
                GUIUtility.systemCopyBuffer = instance.environment.environmentUuid;
                Debug.Log("UUID copied!");
            }

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

            
            if (GUILayout.Button("Upload preview")) {
                string path = EditorUtility.OpenFilePanel("Select preview image", Application.dataPath, "jpg,jpeg,png");

                if (!string.IsNullOrEmpty(path)) {
                    //extension without dot
                    string ext = Path.GetExtension(path).Substring(1);
                    RavelWebRequest req = CreatorRequest.UploadPreview(instance.environment.environmentUuid, path, ext);
                    EditorWebRequests.SendWebRequest(req, OnImageUploaded, this);
                }
            }
            
            if (GUILayout.Button("Upload bundle")) {
                string path = EditorUtility.OpenFilePanel("Select asset bundle", Application.dataPath, "");

                if (!string.IsNullOrEmpty(path)) {
                    RavelWebRequest req = CreatorRequest.UploadBundle(instance.environment.environmentUuid, path);
                    EditorWebRequests.SendWebRequest(req, OnBundleUploaded, this);
                }
            }
        }
        
        //EditorCoroutineUtility.StartCoroutine(UploadImage(instance.environment.environmentUuid, _tex), this);
        private IEnumerator UploadImage(string envUuid, Texture2D tex) {
            List<IMultipartFormSection> pictureData = new();
            //pictureData.Add(new MultipartFormFileSection("file", tex.EncodeToJPG(), "Image", "image/jpg"));
            pictureData.Add(new MultipartFormFileSection("file", tex.EncodeToJPG(), "Image", "image/jpg"));
            string url =
                $"https://dev.ravel.systems/api/v1/environments/uploads/preview-images?environmentUuid={envUuid}";

            UnityWebRequest req = UnityWebRequest.Post(url, pictureData);
            req.SetRequestHeader("Authorization", "Bearer " + TokenWebRequest.GetToken());

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success) {
                Debug.Log("Success");
            }
            else {
                Debug.LogError($"Failure: ({req.error})");
            }
        }

        public void OnImageUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Preview image uploaded successfully");
            }
        }
        
        public void OnBundleUploaded(RavelWebResponse res) {
            if (res.Success) {
                Debug.Log("Asset bundle uploaded successfully");
            }
        }
    }
#endif
}

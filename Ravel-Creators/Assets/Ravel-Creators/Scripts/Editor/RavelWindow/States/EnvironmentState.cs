using System;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEngine;

public class EnvironmentState : CreatorWindowState
{
    private const int ENVIRONMENT_BTN_COUNT = 3;
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Environment; }
    }

    public Environment CurEnv { get { return _environments[_envIndex]; } }

    private bool _envFoldOpen = true;
    private string[] _names;
    private Environment[] _environments;
    private int _envIndex = -1;
    private bool _retrievingImg = false;
    private Sprite _curEnvImg;

    public EnvironmentState(CreatorWindow wnd) : base(wnd) { }
    
    public override void OnGUI(Rect position) {
        if (GUILayout.Button("Create")) {
            CreateEnvironmentWindow.OpenWindow();
        }

        _envFoldOpen = EditorGUILayout.Foldout(_envFoldOpen, "Existing");

        if (_envFoldOpen) {
            if (GUILayout.Button("Fetch remote environments")) {
                FetchRemoteEnvironments();
            }
            
            if (GUILayout.Button("Fetch local environments")) {
                FetchLocalEnvironments();
            }

            if (GUILayout.Button("Clear environments")) {
                Clear();
            }
            GUILayout.Space(RavelBranding.SPACING_SMALL);
            
            if (_environments != null && _environments.Length > 0) {
                if (_envIndex < 0) {
                    _envIndex = 0;
                }

                EditorGUI.BeginChangeCheck();
                _envIndex = GUILayout.SelectionGrid(_envIndex, _names, ENVIRONMENT_BTN_COUNT);
                if (EditorGUI.EndChangeCheck()) {
                    _curEnvImg = null;
                }

                if (_curEnvImg == null && !_retrievingImg && _environments[_envIndex].metadataPreviewImage
                        .TryGetUrl(ImageSize.I1024,
                            out string url)) {
                    EditorImageService.GetSprite(url, ImageSize.I256, OnImageRetrieved);
                    _retrievingImg = true;
                }

                //indent by space
                GUILayout.BeginHorizontal();
                GUILayout.Space(RavelBranding.SPACING_SMALL);
                GUILayout.BeginVertical();
                GUI.enabled = false;
                GUILayout.Space(RavelBranding.SPACING_SMALL);
                GUILayout.TextField($"Name: \t\t{CurEnv.name}");
                GUILayout.TextField($"Guid: \t\t{CurEnv.environmentUuid}");

                GUILayout.TextArea($"Short summary: \t{CurEnv.shortSummary}");
                GUILayout.TextArea($"Long summary: \t{CurEnv.longSummary}");

                GUILayout.Toggle(CurEnv.isPublic, "public:");
                GUILayout.Toggle(CurEnv.published, "published:");
                GUI.enabled = true;

                //undo indent
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save as asset")) {
                    string path = EditorUtility.SaveFilePanel("Save environment", Application.dataPath, 
                        $"ENV_{CurEnv.name}", "asset");

                    if (!string.IsNullOrEmpty(path)) {
                        EnvironmentSO so = ScriptableObject.CreateInstance<EnvironmentSO>();
                        so.environment = CurEnv;

                        path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.Refresh();
                    }
                }
                if (CurEnv.metadataPreviewImage.TryGetUrl(ImageSize.I1920, out string imgUrl) && GUILayout.Button("Save image")) {
                    string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                        $"IMG_{CurEnv.name}_1920", "jpg");

                    if (!string.IsNullOrEmpty(path)) {
                        RavelWebRequest req = new RavelWebRequest(imgUrl, RavelWebRequest.Method.Get);
                        EditorWebRequests.DownloadAndSave(req, path, true, this);
                    }
                }
                if (!string.IsNullOrEmpty(CurEnv.metadataAssetBundle.assetBundleUrl) && GUILayout.Button("Save bundle")) {
                    string path = EditorUtility.SaveFilePanel("Save bundle", Application.dataPath, 
                        $"BUN_{CurEnv.name}", "");

                    if (!string.IsNullOrEmpty(path)) {
                        RavelWebRequest req = new RavelWebRequest(CurEnv.metadataAssetBundle.assetBundleUrl, RavelWebRequest.Method.Get);
                        EditorWebRequests.DownloadAndSave(req, path, true, this);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(RavelBranding.SPACING_SMALL);
                if (_curEnvImg != null) {
                    RavelEditor.DrawTextureScaledGUI(new Vector2(0, GUILayoutUtility.GetLastRect().yMax),
                        position.width, _curEnvImg.texture);
                }
            }
        }
    }
    
    /// <summary>
    /// Fetches all remote environments (this includes the environments in the project).
    /// </summary>
    private void FetchRemoteEnvironments() {
        RavelWebRequest res = CreatorRequest.GetCreatorEnvironments(RavelEditor.User.userUUID, false);
        EditorWebRequests.SendWebRequest(res, OnEnvironmentsRetrieved, this);
    }

    /// <summary>
    /// Fetches all local environments, based on the scriptable objects in the project.
    /// </summary>
    private void FetchLocalEnvironments() {
        string[] paths = AssetDatabase.FindAssets("t:"+ typeof(EnvironmentSO));
        _environments = new Environment[paths.Length];
        for (int i = 0; i < paths.Length; i++) {
            _environments[i] = AssetDatabase.LoadAssetAtPath<EnvironmentSO>(paths[i]).environment;
        }

        _names = _environments.GetNames();
    }

    private void OnEnvironmentsRetrieved(RavelWebResponse res) {
        if (res.Success) {
            //rename public var, cus that's a bad name
            res.DataString = EnvironmentExtensions.RenameStringFromBackend(res.DataString);
            
            if (res.TryGetCollection(out ProxyCollection<Environment> environments)) {
                _environments = environments.Array;
                _names = _environments.GetNames();
            }
            else {
                Debug.LogError($"error fetching and or converting environments error: {res.Error.FullMessage}, data ({res.DataString})!");
            }
        }
        else {
            Debug.LogError($"error fetching environments {res.Error.FullMessage}!");
        }
    }

    private void OnImageRetrieved(Sprite sprite, ImageSize size) {
        _curEnvImg = sprite;
        _retrievingImg = false;
    }

    private void Clear() {
        _environments = Array.Empty<Environment>();
        _names = Array.Empty<string>();
        _envIndex = -1;
    }
}

using System;
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class EnvironmentState : CreatorWindowState
{
    private const int ENVIRONMENT_BTN_COUNT = 3;
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Environments; }
    }

    public Environment CurEnv { get { return _environments[_envIndex]; } }

    private Location _location = Location.None;
    

    private int _envIndex = -1;
    private string[] _names;
    private Environment[] _environments;
    
    private EditorCoroutine _getImageRoutine;
    private Texture2D _curEnvTex;

    public EnvironmentState(CreatorWindow wnd) : base(wnd) { }
    
    public override void OnGUI(Rect position) {
        if (GUILayout.Button("Create")) {
            CreateEnvironmentWindow.OpenWindow();
        }

        //select location and fetch matching environments
        GUIEnvLocation();
        
        
        if (_location != Location.None) {
            GUILayout.Space(RavelBranding.SPACING_SMALL);
            
            if (_environments != null && _environments.Length > 0) {
                int prev = _envIndex; 
                _envIndex = GUIGetEnvironmentIndex();
                
                if (prev != _envIndex) {
                    GetEnvironmentImage();
                }

                GUIDrawEnvData();
                //environment controls
                GUILayout.BeginHorizontal();
                GUISaveSelectEnv();
                GUISaveEnvImage();
                GUISaveEnvBundle();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(RavelBranding.SPACING_SMALL);
                //Debug.Log($"_curEnvImg {_curEnvImg} {_curEnvImg.name}");
                if (_curEnvTex != null && _getImageRoutine == null) {
                    RavelEditor.DrawTextureScaleWidthGUI(new Vector2(0, GUILayoutUtility.GetLastRect().yMax),
                        position.width, _curEnvTex);
                }
            }
        }
    }
    
#region draw GUI methods

    /// <summary>
    /// Shows the location foldout and grid options panel in the GUI.
    /// </summary>
    private void GUIEnvLocation() {
        //none counts as foldout closed, otherwise one of the tabs is used and selected
        bool foldout = _location != Location.None;
        bool forceRefresh = false;
        
        if (!EditorGUILayout.Foldout(foldout, "Existing")) {
            _location = Location.None;
            return;
        }
        
        if (_location == Location.None) {
            _location = (Location)1;
            forceRefresh = true;
        }
        
        EditorGUI.BeginChangeCheck();
        _location = (Location) GUILayout.SelectionGrid((int)_location - 1, GetLocationNames(), (int)Location.Length - 1) + 1;
        if (EditorGUI.EndChangeCheck() || forceRefresh) {
            switch (_location) {
                case Location.Local:
                    FetchLocalEnvironments();
                    break;
                case Location.Remote:
                case Location.Published:
                    FetchRemoteEnvironments(_location == Location.Published);
                    break;
            }
        }
    }

    /// <summary>
    /// Draws environment selection grid and a matching id for the currently selected environment.
    /// </summary>
    private int GUIGetEnvironmentIndex() {
        if (_envIndex < 0) {
            return 0;
        }

        _envIndex = Mathf.Clamp(_envIndex, 0, _environments.Length - 1);

        return GUILayout.SelectionGrid(_envIndex, _names, ENVIRONMENT_BTN_COUNT);
    }

    /// <summary>
    /// Draws the info about the currently selected environment.
    /// </summary>
    private void GUIDrawEnvData() {
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
    }
    
    /// <summary>
    /// Draws save button for the currently selected environment.
    /// This button turns into a select button for local environments.
    /// </summary>
    private void GUISaveSelectEnv() {
        //Scriptable object tool
        if (_location == Location.Local) {
            if (GUILayout.Button("Select asset")) {
                if (TryGetLocalEnvironmentSO(CurEnv, out EnvironmentSO so)) {
                    Selection.activeObject = so;
                }
                else {
                    Debug.LogWarning($"Could not find local asset for environment {CurEnv.name}");
                    FetchLocalEnvironments();
                }
            }
        }
        else {
            if (GUILayout.Button("Save as asset")) {
                if (TryGetLocalEnvironmentSO(CurEnv, out EnvironmentSO so) &&
                    EditorUtility.DisplayDialog("Duplicate reference", "Environment reference already exists in project!", "Ok")) {
                    
                    //select existing reference
                    Selection.activeObject = so;
                }
                else {
                    string path = EditorUtility.SaveFilePanel("Save environment", Application.dataPath, 
                        $"ENV_{CurEnv.name}", "asset");

                    if (!string.IsNullOrEmpty(path)) {
                        so = ScriptableObject.CreateInstance<EnvironmentSO>();
                        so.environment = CurEnv;

                        path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws a save image button, for saving the environment image.
    /// </summary>
    private void GUISaveEnvImage() {
        if (CurEnv.metadataPreviewImage.TryGetUrl(ImageSize.I1920, out string imgUrl) && GUILayout.Button("Save image")) {
            string path = EditorUtility.SaveFilePanel("Save image", Application.dataPath, 
                $"IMG_{CurEnv.name}_1920", "jpg");

            if (!string.IsNullOrEmpty(path)) {
                RavelWebRequest req = new RavelWebRequest(imgUrl, RavelWebRequest.Method.Get);
                EditorWebRequests.DownloadAndSave(req, path, true, this);
            }
        }
    }

    /// <summary>
    /// Draws a save bundle button, for saving the environment assetbundle.
    /// </summary>
    private void GUISaveEnvBundle() {
        if (!string.IsNullOrEmpty(CurEnv.metadataAssetBundle.assetBundleUrl) && GUILayout.Button("Save bundle")) {
            string path = EditorUtility.SaveFilePanel("Save bundle", Application.dataPath, 
                $"BUN_{CurEnv.name}", "");

            if (!string.IsNullOrEmpty(path)) {
                RavelWebRequest req = new RavelWebRequest(CurEnv.metadataAssetBundle.assetBundleUrl, RavelWebRequest.Method.Get);
                EditorWebRequests.DownloadAndSave(req, path, true, this);
            }
        }
    }

#endregion
    
    /// <summary>
    /// Fetches all local environments, based on the scriptable objects in the project.
    /// </summary>
    private void FetchLocalEnvironments() {
        string[] paths = AssetDatabase.FindAssets("t:"+ typeof(EnvironmentSO));
        _environments = new Environment[paths.Length];
        for (int i = 0; i < paths.Length; i++) {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            _environments[i] = AssetDatabase.LoadAssetAtPath<EnvironmentSO>(paths[i]).environment;
        }

        _names = _environments.GetNames();
        GetEnvironmentImage();
    }
    
    /// <summary>
    /// Tries to get a scriptable object for the given environment.
    /// </summary>
    private bool TryGetLocalEnvironmentSO(Environment environment, out EnvironmentSO so) {
        string[] paths = AssetDatabase.FindAssets("t:"+ typeof(EnvironmentSO));
        for (int i = 0; i < paths.Length; i++) {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            so = AssetDatabase.LoadAssetAtPath<EnvironmentSO>(paths[i]);
            if (environment.environmentUuid == so.environment.environmentUuid) {
                return true;
            }
        }

        so = null;
        return false;
    }
    
    /// <summary>
    /// Fetches all remote environments (this includes the environments in the project).
    /// </summary>
    private void FetchRemoteEnvironments(bool isPublished) {
        RavelWebRequest res = CreatorRequest.GetCreatorEnvironments(RavelEditor.User.userUUID, isPublished);
        EditorWebRequests.SendWebRequest(res, OnEnvironmentsRetrieved, this);
    }

    /// <summary>
    /// Called by webrequest class when the environment request has been answered by the backend.
    /// </summary>
    /// <param name="res">Webresponse containing environment data.</param>
    private void OnEnvironmentsRetrieved(RavelWebResponse res) {
        if (res.Success) {
            //rename public var, cus that's a bad name
            res.DataString = EnvironmentExtensions.RenameStringFromBackend(res.DataString);
            
            if (res.TryGetCollection(out ProxyCollection<Environment> environments)) {
                _environments = environments.Array;
                _names = _environments.GetNames();

                GetEnvironmentImage();
            }
            else {
                Debug.LogError($"error fetching and or converting environments error: {res.Error.FullMessage}, data ({res.DataString})!");
            }
        }
        else {
            Debug.LogError($"error fetching environments {res.Error.FullMessage}!");
        }
    }
    
    /// <summary>
    /// Retrieves the current environment image and stops retrieving images if this is already being done.
    /// </summary>
    private void GetEnvironmentImage() {
        if (_getImageRoutine != null) {
            EditorCoroutineUtility.StopCoroutine(_getImageRoutine);
            _getImageRoutine = null;
        }

        if (_envIndex < 0 || _envIndex >= _environments.Length)
            return;
                    
        if(!CurEnv.metadataPreviewImage.TryGetUrl(ImageSize.I1024, out string url))
        {
            Debug.LogError($"Could not retrieve image for url ({url})");
            return;
        }
        Debug.Log($"Retrieve image for url ({url})");
        _getImageRoutine = EditorImageService.GetSpriteRoutine(url, ImageSize.I1024, OnImageRetrieved, this);
    }
    
    /// <summary>
    /// Called by image service when an environment image has been retrieved.
    /// </summary>
    private void OnImageRetrieved(Sprite sprite, ImageSize size) {
        if (TryGetLocalEnvironmentSO(CurEnv, out EnvironmentSO so)) {
            so.environment.UpdateEnvironmentSprite(sprite, size);
        }
        
        _curEnvTex = sprite.texture;
        _curEnvTex.Apply();

        EditorCoroutineUtility.StopCoroutine(_getImageRoutine);
        _getImageRoutine = null;
    }

    /// <summary>
    /// return all names of location enum, without none and lenght
    /// </summary>
    private string[] GetLocationNames() {
        string[] names = new string[(int)Location.Length - 1];
        for (int i = 1; i < names.Length + 1; i++) {
            names[i - 1] = ((Location) i).ToString();
        }

        return names;
    }

    private enum Location
    {
        None = 0,
        Local,
        Remote,
        Published,
        Length
    }
}

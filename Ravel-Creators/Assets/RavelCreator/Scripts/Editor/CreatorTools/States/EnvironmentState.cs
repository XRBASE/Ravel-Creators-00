using System;
using System.IO;
using Base.Ravel.Networking;
using MathBuddy.Strings;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Environment overview page in creator window.
/// </summary>
public class EnvironmentState : CreatorWindowState
{
    //amount of environment per row in the selection grid.
    private const int ENVIRONMENT_BTN_COUNT = 3;
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Environments; }
    }

    protected override Vector2 MinSize {
        get { return (_location == Location.None)? new Vector2(510, 185) : new Vector2(510, 400); }
    }

    /// <summary>
    /// Currently selected environment.
    /// </summary>
    private Environment CurEnv { get { return _environments[_envIndex]; } }

    /// <summary>
    /// Location from which the environments are shown
    /// </summary>
    private Location _location = Location.None;
    
    //Used for environment display and selection.
    private int _envIndex = -1;
    private string[] _names;
    private Environment[] _environments;
    
    //used for image retrieval and canceling the retrieval.
    private EditorCoroutine _getImageRoutine;
    //texture of image that was retrieved.
    private Texture2D _curEnvTex;

    private Vector2 _scroll;

    public EnvironmentState(CreatorWindow wnd) : base(wnd) { }

    public override void OnGUI(Rect position) {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        
        if (GUILayout.Button("Create")) {
            CreateEnvironmentWindow.OpenWindow();
        }

        //select location and fetch matching environments
        GUIEnvLocation();
        
        
        if (_location != Location.None) {
            GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);
            
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
                if (_location is Location.Project or Location.Unpublished) {
                    GUIDeleteEnvironment();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);
                //Debug.Log($"_curEnvImg {_curEnvImg} {_curEnvImg.name}");
                if (_curEnvTex != null && _getImageRoutine == null) {
                    RavelEditor.DrawTextureScaleWidthGUI(new Vector2(0, GUILayoutUtility.GetLastRect().yMax),
                        position.width, _curEnvTex);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
    
#region draw GUI methods

    /// <summary>
    /// Shows the location foldout and grid options panel in the GUI.
    /// </summary>
    private void GUIEnvLocation() {
        //none counts as foldout closed, otherwise one of the tabs is used and selected
        bool forceRefresh = false;

        EditorGUILayout.LabelField("Existing");
        
        if (_location == Location.None) {
            _location = (Location)1;
            forceRefresh = true;
        }
        
        EditorGUI.BeginChangeCheck();
        _location = (Location) GUILayout.SelectionGrid((int)_location - 1, GetLocationNames(), (int)Location.Length - 1) + 1;
        if (EditorGUI.EndChangeCheck() || forceRefresh) {
            RefreshEnvironments();
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
        RavelEditor.GUIBeginIndent(RavelEditorStying.GUI_SPACING_MICRO);
        
        GUI.enabled = false;
        GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);
        GUILayout.TextField($"Name: \t\t{CurEnv.name}");
        GUILayout.TextField($"Guid: \t\t{CurEnv.environmentUuid}");

        GUILayout.TextArea($"Short summary: \t{CurEnv.shortSummary}");
        GUILayout.TextArea($"Long summary: \t{CurEnv.longSummary}");

        GUILayout.Toggle(CurEnv.isPublic, "public:");
        GUILayout.Toggle(CurEnv.published, "published:");
        GUI.enabled = true;

        RavelEditor.GUIEndIndent();
    }
    
    /// <summary>
    /// Draws save button for the currently selected environment.
    /// This button turns into a select button for local environments.
    /// </summary>
    private void GUISaveSelectEnv() {
        //Scriptable object tool
        if (_location == Location.Project) {
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
                    string path = EditorUtility.SaveFilePanel("Save environment", RavelEditorSettings.Get().GetFilePath(), 
                        $"ENV_{CurEnv.name}", "asset");

                    if (!string.IsNullOrEmpty(path)) {
                        RavelEditorSettings.Get().SetFilePath(path);

                        so = ScriptableObject.CreateInstance<EnvironmentSO>();
                        so.environment = CurEnv;

                        if (!path.IsSubpathOf(Application.dataPath)) {
                            Debug.LogError("Cannot save environment asset outside of project!");
                            return;
                        }
                        path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        
                        AssetDatabase.CreateAsset(so, path);
                        AssetDatabase.Refresh();
                        Selection.activeObject = so;
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
            string path = EditorUtility.SaveFilePanel("Save image",  RavelEditorSettings.Get().GetFilePath(), 
                $"IMG_{CurEnv.name}_1920", "jpg");

            if (!string.IsNullOrEmpty(path)) {
                RavelEditorSettings.Get().SetFilePath(path);
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
            string path = EditorUtility.SaveFilePanel("Save bundle", RavelEditorSettings.Get().GetFilePath(), 
                $"BUN_{CurEnv.name}", "");

            if (!string.IsNullOrEmpty(path)) {
                RavelEditorSettings.Get().SetFilePath(path);
                RavelWebRequest req = new RavelWebRequest(CurEnv.metadataAssetBundle.assetBundleUrl, RavelWebRequest.Method.Get);
                EditorWebRequests.DownloadAndSave(req, path, false, this);
            }
        }
    }

    private void GUIDeleteEnvironment() {
        Color prevCol = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete environment") && EditorUtility.DisplayDialog( "Delete environment","Are you sure you want to permanently delete this environment?", "Yes", "No")) {
            if (TryGetLocalEnvironmentSO(CurEnv, out EnvironmentSO envObj)) {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(envObj));
                AssetDatabase.Refresh();
            }
            
            RavelWebRequest req = CreatorRequest.DeleteEnvironment(CurEnv);
            EditorWebRequests.SendWebRequest(req, OnEnvironmentDeletedResponse, this);
        }
        GUI.backgroundColor = prevCol;
    }

#endregion

    private void OnEnvironmentDeletedResponse(RavelWebResponse res) {
        if (res.Success) {
            RefreshEnvironments();
        }
        else {
            Debug.LogError($"Error refreshing environments: {res.Error.FullMessage}");
        }
    }

    private void RefreshEnvironments() {
        switch (_location) {
            case Location.Project:
                FetchLocalEnvironments();
                break;
            case Location.Unpublished:
            case Location.Published:
                FetchRemoteEnvironments(_location == Location.Published);
                break;
        }
    }
    
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
            _curEnvTex = null;
            return;
        }
        
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

    /// <summary>
    /// Retrieval locations (None means the foldout is closed, Length is used for the top-bar button count).
    /// </summary>
    private enum Location
    {
        None = 0,
        Project,
        Unpublished,
        Published,
        Length
    }
}

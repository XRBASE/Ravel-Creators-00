using UnityEditor;
using UnityEngine;

public class ConfigState : CreatorWindowState
{
    private const int PATH_TRUNC_FOLDERS = 3;
    
    public CreatorConfig Config {
        get { return RavelEditor.Config; }
        set { RavelEditor.Config = value; }
    }

    public ConfigState(CreatorWindow wnd) : base(wnd) { }
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Configuration; }
    }
    
    protected override Vector2 MinSize {
        get { return new Vector2(700, 250); }
    }
    
    private Vector2 _scroll;
    private int _versioningMode = 0;
    
    public override void OnGUI(Rect position) {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        GUIDrawMailCaching(position);
        GUIDrawBundlePath(position);
        GUIDrawBundleTools(position);

        GUILayout.Space(RavelBranding.SPACING_SMALL);
        EditorGUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save configuration")) {
            Config.SaveConfig();
            Debug.Log("Configuration saved!");
        }
        if (GUILayout.Button("Load configuration")) {
            Config = CreatorConfig.LoadCurrent();
            Debug.Log("Configuration loaded!");
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reset configuration")) {
            Config = new CreatorConfig();
        }
    }

#region #region draw GUI methods

    private void GUIDrawMailCaching(Rect position) {
        EditorGUI.BeginChangeCheck();
        Config.saveUserMail = GUILayout.Toggle(Config.saveUserMail, "Save email");
        if (EditorGUI.EndChangeCheck() && !Config.saveUserMail) {
            Config.userMail = "";
        }

        GUILayout.BeginHorizontal();
        GUI.enabled = false;
        GUILayout.Label("Saved e-mail address:", GUILayout.Width(RavelBranding.LABEL_MED));
        GUILayout.TextField(Config.userMail, GUILayout.Width(position.width - (RavelBranding.LABEL_MED + RavelBranding.TXT_BTN_SMALL + 15f)));
        GUI.enabled = true;
        if (GUILayout.Button("Clear", GUILayout.Width(RavelBranding.TXT_BTN_SMALL))) {
            Config.userMail = "";
        }
        GUILayout.EndHorizontal();
    }
    
    private void GUIDrawBundlePath(Rect position) {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Asset bundle path:", GUILayout.Width(RavelBranding.LABEL_MED));
        
        //18f is spacing between elements
        GUI.enabled = false;
        
        
        string truncPath = GetLastFoldersOfPath(Config.bundlePath, PATH_TRUNC_FOLDERS);
        //width of all other elements and spacing (18f at the end is 3 spacing for each element and one spacing in front and end)
        float otherWidth = RavelBranding.LABEL_MED + RavelBranding.TXT_BTN_MED + RavelBranding.TXT_BTN_SMALL + 18f;
        GUILayout.TextField(" (...) " + truncPath, 
            GUILayout.Width(position.width - otherWidth));
        GUI.enabled = true;
        
        if (GUILayout.Button("Select folder", GUILayout.Width(RavelBranding.TXT_BTN_MED))) {
            string path = EditorUtility.OpenFolderPanel("Select bundle location", Config.GetFilePath(), "Assetbundle output");
            if (!string.IsNullOrEmpty(path))
            {
                Config.bundlePath = path;
            }
        }
        
        if (GUILayout.Button("Copy", GUILayout.Width(RavelBranding.TXT_BTN_SMALL))) {
            GUIUtility.systemCopyBuffer = Config.bundlePath;
            Debug.Log("Path copied!");
        }
        GUILayout.EndHorizontal();
    }

    private void GUIDrawBundleTools(Rect position) {
        Config.autoClean = GUILayout.Toggle(Config.autoClean, "auto cleanup build files");
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("versioning");
        _versioningMode = EditorGUILayout.Popup(_versioningMode, CreatorConfig.VERSIONING_OPTIONS);
        EditorGUILayout.EndHorizontal();
        
        if (CreatorConfig.IsCustomVersioning(_versioningMode)) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Custom versioning string!");
            Config.versioning = GUILayout.TextArea(Config.versioning);
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(Config.versioning) || !Config.versioning.Contains('1')) {
                EditorGUILayout.HelpBox("Versioning should always include the character 1, this character will be replaced with the actual number of the assetbundle build!", MessageType.Error);
            }
        }
    }

#endregion
    private string GetLastFoldersOfPath(string path, int folders) {
        //truncate to make more readable
        int lastDash = path.LastIndexOf('/');
        
        //if last char is not a /, iterate one folder less than assigned
        if (path[^1] != '/')
            folders--;
        
        for (int i = 0; i < folders; i++) {
            lastDash = path.Substring(0,lastDash).LastIndexOf('/');
        }

        return path.Substring(lastDash + 1);
    }
}

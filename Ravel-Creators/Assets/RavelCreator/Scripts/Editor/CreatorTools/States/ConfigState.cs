using UnityEditor;
using UnityEngine;

/// <summary>
/// Tab for inputting project configuration.
/// </summary>
public class ConfigState : CreatorWindowState
{
    //Amount of folders to show in the path options, it only shows the last folders this way, so it is more clear what 
    //actual folders have been selected.
    private const int PATH_TRUNC_FOLDERS = 3;
    
    //Provides easy access to the configuration. The config is stored in Editor, so it is also accessible before the window has been opened.
    /// <summary>
    /// Configuration file for most of the data in the window.
    /// </summary>
    public CreatorConfig Config {
        get { return RavelEditor.CreatorConfig; }
        set { RavelEditor.CreatorConfig = value; }
    }

    public ConfigState(CreatorWindow wnd) : base(wnd) { }
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Configuration; }
    }
    
    protected override Vector2 MinSize {
        get { return new Vector2(700, 225); }
    }
    
    private Vector2 _scroll;

    public override void OnStateClosed() {
        base.OnStateClosed();
        Config.SaveConfig();
    }
    
    public override void OnGUI(Rect position) {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        
        GUIDrawMailCaching(position);
        GUIDrawBundlePath(position);
        GUIDrawBuildConfig();

        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);
        if (GUILayout.Button("Reset")) {
            Config = new CreatorConfig();
        }
        EditorGUILayout.EndScrollView();
    }

#region #region draw GUI methods

    /// <summary>
    /// Draws user email that was cached, clear button for the mail only and the toggle to disable caching in the whole project.
    /// </summary>
    private void GUIDrawMailCaching(Rect position) {
        EditorGUI.BeginChangeCheck();
        Config.saveUserMail = GUILayout.Toggle(Config.saveUserMail, "Save e-mail");
        if (EditorGUI.EndChangeCheck() && !Config.saveUserMail) {
            Config.userMail = "";
        }

        GUILayout.BeginHorizontal();
        GUI.enabled = false;
        GUILayout.Label("Saved e-mail address:", GUILayout.Width(RavelEditorStying.GUI_SPACING_DECA));
        GUILayout.TextField(Config.userMail, GUILayout.Width(position.width - (RavelEditorStying.GUI_SPACING_DECA + RavelEditorStying.GUI_SPACING + 15f)));
        GUI.enabled = true;
        if (GUILayout.Button("Clear", GUILayout.Width(RavelEditorStying.GUI_SPACING))) {
            Config.userMail = "";
            Config.SaveConfig();
        }
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Draws path (truncated) to the asset bundle folder and a button with which the path can be set.
    /// </summary>
    private void GUIDrawBundlePath(Rect position) {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Assetbundle path:", GUILayout.Width(RavelEditorStying.GUI_SPACING_DECA));
        
        //18f is spacing between elements
        GUI.enabled = false;
        
        
        if (GetLastFoldersOfPath(Config.bundlePath, PATH_TRUNC_FOLDERS, out string truncPath)) {
            truncPath = "(...)" + truncPath;
        }
        //width of all other elements and spacing (18f at the end is 3 spacing for each element and one spacing in front and end)
        float otherWidth = RavelEditorStying.GUI_SPACING_DECA * 2 + RavelEditorStying.GUI_SPACING + 18f;
        GUILayout.TextField(truncPath, GUILayout.Width(position.width - otherWidth));
        GUI.enabled = true;
        
        if (GUILayout.Button("Select folder", GUILayout.Width(RavelEditorStying.GUI_SPACING_DECA))) {
            string path = EditorUtility.OpenFolderPanel("Select bundle location", Config.GetFilePath(), "Assetbundle output");
            if (!string.IsNullOrEmpty(path))
            {
                Config.bundlePath = path;
            }
        }
        
        if (GUILayout.Button("Copy", GUILayout.Width(RavelEditorStying.GUI_SPACING))) {
            GUIUtility.systemCopyBuffer = Config.bundlePath;
            Debug.Log("Path copied!");
        }
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Draws configuration settings for the build process.
    /// </summary>
    private void GUIDrawBuildConfig() {
        Config.autoClean = GUILayout.Toggle(Config.autoClean, "auto cleanup bundle files");
    }

#endregion
    /// <summary>
    /// Splits the path on dashes and removes everything apart from the last folders in the path. 
    /// </summary>
    /// <param name="path">path that is being truncated.</param>
    /// <param name="folders">amount of folders (the last ones) to keep showing.</param>
    /// <param name="truncated">truncated path (or whole if path was shorter than amount of folders specified).</param>
    /// <returns>True/False path was made shorter.</returns>
    private bool GetLastFoldersOfPath(string path, int folders, out string truncated) {
        //truncate to make more readable
        int lastDash = path.LastIndexOf('/');
        
        //if last char is not a /, iterate one folder less than assigned
        if (path[^1] != '/')
            folders--;
        
        for (int i = 0; i < folders; i++) {
            if (path.Substring(0,lastDash).Contains('/'))
            {
                lastDash = path.Substring(0,lastDash).LastIndexOf('/');
            }
            else {
                //if path is of less folders than specified amount, return the whole path
                truncated = path;
                return false;
            }
        }

        truncated = path.Substring(lastDash + 1);
        return true;
    }
}

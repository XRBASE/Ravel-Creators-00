using UnityEditor;
using UnityEngine;

/// <summary>
/// Overview of local bundles and their version numbering.
/// </summary>
public class BundleState : CreatorWindowState
{
    //The version data is saved in the creator config, as project wide config is saved there, but this window displays it.
    
    //constants for recognizing the custom and no version data.
    private const string CUSTOM_VERSIONING = "Custom";
    public const string NO_VERSIONING = "None";
    
    //other built-in versioning types
    public static readonly string[] VERSIONING_OPTIONS = { NO_VERSIONING, "(1,2)", "_1_2", CUSTOM_VERSIONING };
    
    //Width of screen that is reserved for the versioning (21f is extra padding for spacing between elements and scrollbar width)
    private const float VERSION_WIDTH = (RavelEditorStying.INT_LAB_WIDTH + RavelEditorStying.GUI_SPACING_MILLI + RavelEditorStying.GUI_SPACING_CENTI + 21f) * 2f;
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Bundles; }
    }
    protected override Vector2 MinSize {
        get { return new Vector2(350, 210); }
    }
    
    //Provides easy access to the configuration. The config is stored in Editor, so it is also accessible before the window has been opened.
    /// <summary>
    /// Configuration file for most of the data in the window.
    /// </summary>
    public EditorBundles Config {
        get { return RavelEditor.EditorBundles; }
        set { RavelEditor.EditorBundles = value; }
    }
    
    private Vector2 _scroll;
    //cache for version input of the user.
    private int _versioningMode;
    
    public BundleState(CreatorWindow wnd) : base(wnd) { }

    public override void OnSwitchState() {
        base.OnSwitchState();
        Config.UpdateValues();
        
        //When the window is opened, set the correct versioning, based on the value in the creator config.
        if (string.IsNullOrEmpty(RavelEditor.CreatorPanelSettings.versioning)) {
            //no versioning
            _versioningMode = 0;
        }
        else {
            bool found = false;
            for (int i = 0; i < VERSIONING_OPTIONS.Length; i++) {
                if (RavelEditor.CreatorPanelSettings.versioning == VERSIONING_OPTIONS[i]) {
                    _versioningMode = i;
                    found = true;
                }
            }

            if (!found) {
                //custom versioning
                _versioningMode = 3;
            }
        }
        
    }

    //Whenever the window or tab is closed, save the configuration
    public override void OnStateClosed() {
        base.OnStateClosed();
        Config.SaveConfig();
    }
    
    /// <summary>
    /// Check if option with given index is the CUSTOM version of the options.
    /// </summary>
    /// <param name="index">index of the used option</param>
    private static bool IsCustomVersioning(int index) {
        return VERSIONING_OPTIONS[index] == CUSTOM_VERSIONING;
    }
	
    /// <summary>
    /// Check if option with given index is the NO VERSIONING version of the options.
    /// </summary>
    /// <param name="index">index of the used option</param>
    private static bool NoVersioning(int index) {
        return VERSIONING_OPTIONS[index] == NO_VERSIONING;
    }

    public override void OnGUI(Rect position) {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        
        //versioning string input
        GUIDrawVersioning(position);

        GUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);
        GUIDrawRefreshButton();
        //all bundles and their version numbers 
        for (int i = 0; i < Config.bundles.Count; i++) {
            GUIDrawBundleInfo(Config.bundles[i], position);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
#region #region draw GUI methods

    private void GUIDrawRefreshButton() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Refresh", GUILayout.Width(RavelEditorStying.GUI_SPACING * 2))) {
            Config.UpdateValues();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Drawn in version screen, but saved in the creator config, as that contains all user settings.
    /// </summary>
    private void GUIDrawVersioning(Rect position) {
        //version string drop-down
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("versioning: ", GUILayout.Width(position.width - VERSION_WIDTH));
        _versioningMode = EditorGUILayout.Popup(_versioningMode, VERSIONING_OPTIONS);
        EditorGUILayout.EndHorizontal();
        
        //string input in the case of custom versioning
        if (IsCustomVersioning(_versioningMode)) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Custom versioning string!");
            RavelEditor.CreatorPanelSettings.versioning = GUILayout.TextArea(RavelEditor.CreatorPanelSettings.versioning);
            EditorGUILayout.EndHorizontal();

            if (!RavelEditor.CreatorPanelSettings.versioning.Contains('1') || !RavelEditor.CreatorPanelSettings.versioning.Contains('2')) {
                EditorGUILayout.HelpBox("Not both numbers, 1 for major and 2 for minor, are included in the custom versioning! " +
                                        "Numbers not included in this custom string will not be shown in the bundle name.", MessageType.Warning);
            }
        }
        else if (NoVersioning(_versioningMode)) {
            RavelEditor.CreatorPanelSettings.versioning = "";
        }
        else {
            RavelEditor.CreatorPanelSettings.versioning = VERSIONING_OPTIONS[_versioningMode];
        }
        
        //toggle for version incrementation (on build)
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Increment minor version every build", GUILayout.Width(position.width - VERSION_WIDTH));
        RavelEditor.CreatorPanelSettings.incrementMinorVersionOnBuild = EditorGUILayout.Toggle(RavelEditor.CreatorPanelSettings.incrementMinorVersionOnBuild);
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck()) {
            //whenever changes are made in the editor code above, save the creator config. 
            RavelEditor.CreatorPanelSettings.SaveConfig();
        }

        //Sample string using either the first bundle or a sample string 
        string sampleLabel = "Cool_Ravel_World";
        int maj = 0;
        int min = 1;
        if (Config.bundles.Count >= 1) {
            sampleLabel = Config.bundles[0].name;
            maj = Config.bundles[0].vMajor;
            min = Config.bundles[0].vMinor;
        }

        sampleLabel += Config.GetVersionString(maj, min);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Version sample:", GUILayout.Width(position.width - VERSION_WIDTH));
        GUILayout.Label($"{sampleLabel}");
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Draws info about the bundle data, including a version display, in which the numbers can be changed.
    /// </summary>
    /// <param name="bundle">Bundle of which the data is drawn.</param>
    /// <param name="position">GUI position rect.</param>
    private void GUIDrawBundleInfo(EditorBundles.BundleData bundle, Rect position) {
        GUILayout.BeginHorizontal();
        
        GUI.enabled = false;
        //width of version labels and input
        //21f = spacing between elements and space for the scrollbar if the list gets too long.
        GUILayout.TextField(bundle.name, GUILayout.Width(position.width - VERSION_WIDTH));
        GUI.enabled = true;
        
        //GUILayout.FlexibleSpace();
        GUILayout.Label("major: ", GUILayout.Width(RavelEditorStying.GUI_SPACING_CENTI));
        bundle.vMajor = GUIDrawVersionNumber(bundle.vMajor);
        GUILayout.Label("minor: ", GUILayout.Width(RavelEditorStying.GUI_SPACING_CENTI));
        bundle.vMinor = GUIDrawVersionNumber(bundle.vMinor);
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Sub call for drawing one of the two version numbers including the plus button to increment it.
    /// </summary>
    /// <param name="num">Current version number</param>
    /// <returns>New version number after input.</returns>
    private int GUIDrawVersionNumber(int num) {
        num = Mathf.Abs(EditorGUILayout.IntField(num, GUILayout.Width(RavelEditorStying.INT_LAB_WIDTH)));

        if (GUILayout.Button("+", GUILayout.Width(RavelEditorStying.GUI_SPACING_MILLI))) {
            num++;
        }

        return num;
    }
#endregion
}

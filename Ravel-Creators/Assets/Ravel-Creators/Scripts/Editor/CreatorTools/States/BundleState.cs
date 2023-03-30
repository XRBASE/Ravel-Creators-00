using UnityEditor;
using UnityEngine;

public class BundleState : CreatorWindowState
{
    private const string CUSTOM_VERSIONING = "Custom";
    public const string NO_VERSIONING = "None";
    public static readonly string[] VERSIONING_OPTIONS = { NO_VERSIONING, "(1,2)", "_1_2", CUSTOM_VERSIONING };
    
    //width of screen that is reserved for the versioning (21f is extra padding for spacing between elements and scrollbar width)
    private const float VERSION_WIDTH = (RavelBranding.INT_FIELD_999 + RavelBranding.TOOLBAR_BTN_SQUARE + RavelBranding.TXT_LAB_MICRO + 21f) * 2f;
    
    public BundleConfig Config {
        get { return RavelEditor.BundleConfig; }
        set { RavelEditor.BundleConfig = value; }
    }
    
    public override CreatorWindow.State State {
        get { return CreatorWindow.State.Bundles; }
    }
    protected override Vector2 MinSize {
        get { return new Vector2(350, 210); }
    }

    private Vector2 _scroll;
    private int _versioningMode;
    
    public BundleState(CreatorWindow wnd) : base(wnd) {
    }

    public override void OnSwitchState() {
        base.OnSwitchState();
        Config.UpdateValues();
        
        //set correct versioning
        if (string.IsNullOrEmpty(RavelEditor.CreatorConfig.versioning)) {
            //no versioning
            _versioningMode = 0;
        }
        else {
            bool found = false;
            for (int i = 0; i < VERSIONING_OPTIONS.Length; i++) {
                if (RavelEditor.CreatorConfig.versioning == VERSIONING_OPTIONS[i]) {
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

    public override void OnStateClosed() {
        base.OnStateClosed();
        Config.SaveConfig();
    }

    public override void OnGUI(Rect position) {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        
        GUIDrawVersioning(position);
        
        for (int i = 0; i < Config.bundles.Count; i++) {
            GUIDrawBundleInfo(Config.bundles[i], position);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
#region #region draw GUI methods

    /// <summary>
    /// Drawn in version screen, but saved in the creator config, as that contains all user settings.
    /// </summary>
    private void GUIDrawVersioning(Rect position) {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("versioning: ", GUILayout.Width(position.width - VERSION_WIDTH));
        _versioningMode = EditorGUILayout.Popup(_versioningMode, VERSIONING_OPTIONS);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label("Increment minor version every build", GUILayout.Width(position.width - VERSION_WIDTH));
        RavelEditor.CreatorConfig.incrementMinorVersionOnBuild = EditorGUILayout.Toggle(RavelEditor.CreatorConfig.incrementMinorVersionOnBuild);
        EditorGUILayout.EndHorizontal();
      
        if (IsCustomVersioning(_versioningMode)) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Custom versioning string!");
            RavelEditor.CreatorConfig.versioning = GUILayout.TextArea(RavelEditor.CreatorConfig.versioning);
            EditorGUILayout.EndHorizontal();

            if (!RavelEditor.CreatorConfig.versioning.Contains('1') || !RavelEditor.CreatorConfig.versioning.Contains('2')) {
                EditorGUILayout.HelpBox("Not both numbers, 1 for major and 2 for minor, are included in the custom versioning! " +
                                        "Numbers not included in this custom string will not be shown in the bundle name.", MessageType.Warning);
            }
        }
        else if (NoVersioning(_versioningMode)) {
            RavelEditor.CreatorConfig.versioning = "";
        }
        else {
            RavelEditor.CreatorConfig.versioning = VERSIONING_OPTIONS[_versioningMode];
        }

        if (EditorGUI.EndChangeCheck()) {
            RavelEditor.CreatorConfig.SaveConfig();
        }

        string sampleLabel = "[BundleName]";
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
    
    private void GUIDrawBundleInfo(BundleConfig.BundleData bundle, Rect position) {
        GUILayout.BeginHorizontal();
        
        GUI.enabled = false;
        //width of version labels and input
        //21f = spacing between elements and space for the scrollbar if the list gets too long.
        GUILayout.TextField(bundle.name, GUILayout.Width(position.width - VERSION_WIDTH));
        GUI.enabled = true;
        
        //GUILayout.FlexibleSpace();
        GUILayout.Label("major: ", GUILayout.Width(RavelBranding.TXT_LAB_MICRO));
        bundle.vMajor = GUIDrawVersionNumber(bundle.vMajor);
        GUILayout.Label("minor: ", GUILayout.Width(RavelBranding.TXT_LAB_MICRO));
        bundle.vMinor = GUIDrawVersionNumber(bundle.vMinor);
        GUILayout.EndHorizontal();
    }

    private int GUIDrawVersionNumber(int num) {
        num = Mathf.Abs(EditorGUILayout.IntField(num, GUILayout.Width(RavelBranding.INT_FIELD_999)));

        if (GUILayout.Button("+", GUILayout.Width(RavelBranding.TOOLBAR_BTN_SQUARE))) {
            num++;
        }

        return num;
    }
#endregion
    
    public static bool IsCustomVersioning(int index) {
        return VERSIONING_OPTIONS[index] == CUSTOM_VERSIONING;
    }
	
    public static bool NoVersioning(int index) {
        return VERSIONING_OPTIONS[index] == NO_VERSIONING;
    }
}

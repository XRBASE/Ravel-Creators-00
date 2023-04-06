using Base.Ravel.Config;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

/// <summary>
/// Static class that is responsible for drawing the toolbar buttons next to the play buttons.
/// </summary>
[InitializeOnLoad]
public class RavelToolbar
{
    //configuration file for current scene.
    private static SceneConfiguration _config;

    /// <summary>
    /// This is called on init and recompile.
    /// </summary>
    static RavelToolbar() {
        //Used for gui drawing the buttons
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        
        //used to retrieve scene config when swapping scene's
        EditorSceneManager.sceneOpened += OnSceneLoaded;
        SceneConfiguration.environmentUpdated += RefreshConfig;
    }
    
    ~RavelToolbar() {
        EditorSceneManager.sceneOpened -= OnSceneLoaded;
        SceneConfiguration.environmentUpdated -= RefreshConfig;
    }

    private static void OnSceneLoaded(Scene s, OpenSceneMode mode) {
        RefreshConfig();
    }
    
    /// <summary>
    /// Used when recompiling to retrieve config
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        RefreshConfig();
    }

    /// <summary>
    /// Tries to find a valid configuration in the scene, that is used to build to and display the name of.
    /// </summary>
    public static void RefreshConfig() {
        //called on startup and untitled scene's 
        if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
            return;
        
        SceneConfiguration[] configs = GameObject.FindObjectsOfType<SceneConfiguration>();
        if (configs.Length == 0) {
            _config = SceneConfiguration.ShowNoConfigDialog();
            return;
        } 
        
        if (configs.Length > 1) {
            Debug.LogWarning("Found multiple scene configurations, this is not supported in the build process. Defaulting to first found configuration");
        }

        _config = configs[0];
    }

    /// <summary>
    /// OnGUI call for all toolbar items.
    /// </summary>
    public static void OnRightToolbarGUI() {
        //moves the items to the right side of the play button.
        GUILayout.FlexibleSpace();
        
        //Ravel logo
        if (RavelEditor.Branding.logoSquare && GUILayout.Button(RavelEditor.Branding.logoSquare, 
                RavelEditorStying.imageBtn)) {
            Debug.Log("BOK BOK, You pressed the hidden chicken button!");
        }

        //Bundle/environment label.
        bool canBuild = (_config != null && _config.environmentSO != null);
        string bundleName = "";
        if (_config != null) {
            bundleName = (_config.environmentSO != null)? _config.environmentSO.bundleName : "Missing environment!";
            GUI.enabled = false;
            GUILayout.TextField(bundleName, GUILayout.Width(RavelEditorStying.GUI_SPACING_DECA));
        }
        GUI.enabled = canBuild;
        
        //preview environment in ravel web
        if (GUILayout.Button(new GUIContent("Preview", "Build and preview asset bundle"), RavelEditorStying.txtBtnSmall)) {
            bool cleanup = RavelEditor.CreatorConfig.autoClean;

            BundleBuilder.BuildOpenScene(bundleName, true, cleanup);
        }
        if (GUILayout.Button(new GUIContent("Build", "Build asset bundle"), RavelEditorStying.txtBtnSmall)) {
            bool cleanup = RavelEditor.CreatorConfig.autoClean;

            BundleBuilder.BuildOpenScene(bundleName, false, false);
        }
        GUI.enabled = true;

        //Refreshes the configuration file
        if (GUILayout.Button(new GUIContent("Refresh", "Refresh configuration"), RavelEditorStying.txtBtnSmall)) {
            RefreshConfig();
        }

        //Backend mode that is used to preview on, only accessible for dev users.
        GUI.enabled = RavelEditor.DevUser;
        bool curMode = AppConfig.Networking.Mode == NetworkConfig.AppMode.Live;
        //user picks between app and live
        bool pickedMode = EditorGUILayout.Popup("", curMode ? 0 : 1, new[] { "App", "Dev" }, 
            GUILayout.Width(RavelEditorStying.GUI_SPACING_DECI)) == 0;
        
        if (curMode != pickedMode &&
            EditorUtility.DisplayDialog("Switch app modes", "Are you sure you want to switch app modes? You'll have to log in again.", "Yes", "No")) {
            //switch app modes
            RavelEditor.OnLogout(false);
            CreatorWindow.OpenAccount();
            
            AppConfig.Networking.Mode = (pickedMode) ? NetworkConfig.AppMode.Live : NetworkConfig.AppMode.Development;
        }
        GUI.enabled = true;

        GUILayout.Space(RavelEditorStying.GUI_SPACING_MILLI);
    }
}

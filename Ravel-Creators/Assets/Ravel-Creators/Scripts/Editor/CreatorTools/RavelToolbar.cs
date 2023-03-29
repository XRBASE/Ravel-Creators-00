using System;
using Base.Ravel.Config;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class RavelToolbar
{
    /// <summary>
    /// contains the GUI styles that are passed on to the buttons in the toolbar
    /// </summary>
    private static class ToolbarStyle
    {
        public static GUIStyle imageInBtn;
        public static GUIStyle imageBtn;
        public static GUIStyle txtBtnSmall;

        static ToolbarStyle() {
            imageInBtn = new GUIStyle("Command")
            {
                imagePosition = ImagePosition.ImageAbove,
                fixedWidth = RavelBranding.TOOLBAR_BTN_SQUARE,
                padding = new RectOffset(0,0,2,0),
            };
            imageBtn = new GUIStyle()
            {
                imagePosition = ImagePosition.ImageOnly,
                fixedWidth = RavelBranding.TOOLBAR_BTN_SQUARE,
                padding = new RectOffset(0,0,2,0),
                
            };
            txtBtnSmall = new GUIStyle("Command")
            {
                //fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Normal,

                fixedWidth = RavelBranding.TOOLBAR_BTN_TXT_SMALL,
                fixedHeight = RavelBranding.TOOLBAR_BTN_SQUARE,
                padding = new RectOffset(0,0,2,0),
            };
        }
    }

    private static SceneConfiguration _config;
    
    static RavelToolbar() {
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        EditorSceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene s, LoadSceneMode mode) {
        RefreshConfig();
    }
    
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        RefreshConfig();
    }

    private static void RefreshConfig() {
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

    public static void OnRightToolbarGUI() {
        GUILayout.FlexibleSpace();
        
        if (RavelEditor.Branding.logoSquare && GUILayout.Button(RavelEditor.Branding.logoSquare, 
                ToolbarStyle.imageBtn)) {
            Application.OpenURL("https://www.youtube.com/watch?v=CnnvbFTuge8&ab_channel=gijsjaradijsja");
        }

        string bundleName;
        bool cleanup = RavelEditor.Config.autoClean;
        if (_config != null) {
            bundleName = _config.environmentSO.bundleName;
            GUI.enabled = false;
            GUILayout.TextField(bundleName, GUILayout.Width(RavelBranding.SPACING_BIG));
            GUI.enabled = true;
        }
        else {
            bundleName = new Guid().ToString();
            cleanup = true;
        }
        
        if (GUILayout.Button("Preview", ToolbarStyle.txtBtnSmall)) {
            BundleBuilder.BuildOpenScene(bundleName, cleanup);
        }
        GUI.enabled = true;
        if (GUILayout.Button("Clear", ToolbarStyle.txtBtnSmall)) {
            BundleBuilder.ClearAllBundles();
        }
        
        if (GUILayout.Button("Get Config", ToolbarStyle.txtBtnSmall)) {
            RefreshConfig();
        }

        GUI.enabled = RavelEditor.DevUser;
        bool curMode = AppConfig.Networking.Mode == NetworkConfig.AppMode.Live;
        //user picks between app and live
        bool pickedMode = EditorGUILayout.Popup("", curMode ? 0 : 1, new[] { "App", "Dev" }, 
            GUILayout.Width(RavelBranding.TOOLBAR_BTN_TXT_SMALL)) == 0;
        
        if (curMode != pickedMode &&
            EditorUtility.DisplayDialog("Switch app modes", "Are you sure you want to switch app modes? You'll have to log in again.", "Yes", "No")) {
            //switch app modes
            RavelEditor.OnLogout(false);
            CreatorWindow.OpenAccount();
            
            AppConfig.Networking.Mode = (pickedMode) ? NetworkConfig.AppMode.Live : NetworkConfig.AppMode.Development;
        }
        GUI.enabled = true;

        GUILayout.Space(RavelBranding.SPACING_MED);
    }
}

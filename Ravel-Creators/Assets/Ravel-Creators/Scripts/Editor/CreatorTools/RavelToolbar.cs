using Base.Ravel.Config;
using UnityEditor;
using UnityEngine;
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

    static RavelToolbar() {
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
    }

    public static void OnRightToolbarGUI() {
        GUILayout.FlexibleSpace();
        
        if (RavelEditor.Branding.logoSquare && GUILayout.Button(RavelEditor.Branding.logoSquare, 
                ToolbarStyle.imageBtn)) {
            Application.OpenURL("https://www.youtube.com/watch?v=CnnvbFTuge8&ab_channel=gijsjaradijsja");
        }

        if (GUILayout.Button("Preview", ToolbarStyle.txtBtnSmall)) {
            BundleBuilder.BuildOpenScene();
        }

        GUI.enabled = RavelEditor.DevUser;
        bool curMode = AppConfig.Networking.Mode == NetworkConfig.AppMode.Live;
        //user picks between app and live
        bool pickedMode = EditorGUILayout.Popup("", curMode ? 0 : 1, new[] { "App", "Dev" },
            ToolbarStyle.txtBtnSmall) == 0;
        
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

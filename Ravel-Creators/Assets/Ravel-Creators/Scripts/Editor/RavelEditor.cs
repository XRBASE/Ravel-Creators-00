using Base.Ravel.Networking.Authorization;
using Base.Ravel.Users;
using UnityEditor;
using UnityEngine;

public static class RavelEditor
{
    public static bool LoggedIn {
        get { return User != null; }
    }
    public static  User User { get; set; }

    public static RavelBranding Branding {
        get {
            if (_branding == null) {
                _branding = Resources.Load<RavelBranding>("Branding/BrandingConfig");
            }

            return _branding;
        }
    }

    private static RavelBranding _branding;

    public static void OnLogin(User user, LoginRequest.TokenResponse token) {
        User = user;
    }
    
    //also call when webrequests fail because of authority reasons
    public static void OnLogout() {
        User = null;
        PlayerCache.DeleteKey(LoginRequest.SYSTEMS_TOKEN_KEY);
        //EditorCache.DeleteKey(LoginRequest.SYSTEMS_TOKEN_KEY);
        
        Debug.LogError("Not logged in, please open the account window and log in (Topbar, Ravel, Account).");
    }

    /// <summary>
    /// Draws whole texture and scales height based on given width
    /// </summary>
    /// <param name="pos">Position on which the texture is drawn.</param>
    /// <param name="width">Width of the texture, height is calculated from this.</param>
    /// <param name="tex">Texture to draw.</param>
    public static void DrawTextureScaledGUI(Vector2 pos, float width, Texture2D tex) {
        float px = ((float)tex.height / tex.width) * width;
        Rect coords = new Rect(0,0,1,1);
        
        GUI.DrawTextureWithTexCoords(new Rect(pos, new Vector2(width, px)), tex, coords);
        EditorGUILayout.Space(px);
    }

    /// <summary>
    /// Draws texture in given rect and masks the not visible parts of the texture
    /// </summary>
    /// <param name="mask">Mask rect, in which texture is drawn.</param>
    /// <param name="tex">Texture to draw.</param>
    /// <param name="poi">Point of interest, zooms in on this position.</param>
    public static void DrawTextureScaledGUI(Rect mask, Texture2D tex, Vector2 poi) {
        Vector2 res = new Vector2(tex.width, tex.height);
        Rect coords = new Rect(0,0,1,1);

        float pos;
        float px;
        float dec;

        float m = mask.width / mask.height;
        float t = (float)tex.width / tex.height;
		
        if (m > t) {
            //match x, scale y
            //find res y, when x has mask res
            px = (res.y / res.x) * mask.width;
			
            //find decimal of image shown
            dec = mask.height / px;
            pos = Mathf.Clamp(poi.y - dec / 2f, 0, 1f - dec);
			
            coords = new Rect(0, pos, 1, dec);
        }
        else {
            //match y, scale x
            //find res x, when y has mask res
            px = (res.x / res.y) * mask.height;
			
            //find decimal of image shown
            dec = mask.width / px;
            pos = Mathf.Max(0.5f - dec / 2f, poi.x - dec / 2f);
            coords = new Rect(pos, 0, dec, 1);
        }
		
        GUI.DrawTextureWithTexCoords(mask, tex, coords);
        EditorGUILayout.Space(mask.height);
    }
    
}

[InitializeOnLoad]
class StartupHelper
{
    static StartupHelper() {
        EditorApplication.update += StartUp;
        
    }

    static void StartUp() {
        EditorApplication.update -= StartUp;
        
        CreatorWindow wnd = CreatorWindow.GetWindow(CreatorWindow.State.Account, false);
        (wnd.Tab as AccountState).TryLoginWithToken();
    }
}

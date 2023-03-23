using System;
using Base.Ravel.Networking.Authorization;
using Base.Ravel.Networking.Organisations;
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

    public static void OnLogin(User user) {
        User = user;
    }
    
    //also call when webrequests fail because of authority reasons
    public static void OnLogout() {
        User = null;
        PlayerCache.DeleteKey(LoginRequest.SYSTEMS_TOKEN_KEY);
        
        Debug.LogError("Not logged in, please open the account window and log in (Topbar, Ravel, Account).");
    }

    private static bool _retrievingOrganisations;
    public static void GetUserOrganisations(Action<Organisation[], bool> onRetrieved, object sender) {
        if (_retrievingOrganisations)
            return;
        
        if (User == null) {
            CreatorWindow.OpenAccount();
            throw new Exception("User not found, please log in first!");
        }

        if (User.Organisations != null) {
            Debug.LogWarning("Reusing previous organisations, to refresh organisations, please select refresh " +
                             "user data in the account window (Ravel>Creators>Account).");
            onRetrieved?.Invoke(User.Organisations, true);
        }

        _retrievingOrganisations = true;
        if (onRetrieved == null) {
            onRetrieved = OnOrganisationsRetrieved;
        }
        else {
            onRetrieved = OnOrganisationsRetrieved + onRetrieved;
        }
        EditorWebRequests.GetDataCollectionRequest(OrganisationRequest.GetUsersOrganisations(User.userUUID), onRetrieved, sender);
    }

    private static void OnOrganisationsRetrieved(Organisation[] organisations, bool success) {
        if (success) {
            User.Organisations = organisations;
        }

        _retrievingOrganisations = false;
    }

    /// <summary>
    /// Draws whole texture and scales height based on given width
    /// </summary>
    /// <param name="pos">Position on which the texture is drawn.</param>
    /// <param name="width">Width of the texture, height is calculated from this.</param>
    /// <param name="tex">Texture to draw.</param>
    /// <param name="addSpace">Adds a guilayout space with the height of the image, to ensure layout items are placed below the image.</param>
    public static void DrawTextureScaleWidthGUI(Vector2 pos, float width, Texture2D tex, bool addSpace = true) {
        float px = ((float)tex.height / tex.width) * width;
        Rect coords = new Rect(0,0,1,1);
        
        GUI.DrawTextureWithTexCoords(new Rect(pos, new Vector2(width, px)), tex, coords);
        if (addSpace) {
            EditorGUILayout.Space(px);
        }
    }
    
    /// <summary>
    /// Draws whole texture and scales height based on given width
    /// </summary>
    /// <param name="pos">Position on which the texture is drawn.</param>
    /// <param name="width">Width of the texture, height is calculated from this.</param>
    /// <param name="tex">Texture to draw.</param>
    /// <param name="addSpace">Adds a guilayout space with the height of the image, to ensure layout items are placed below the image.</param>
    public static void DrawTextureScaleHeightGUI(Vector2 pos, float height, Texture2D tex, bool addSpace = true) {
        float px = ((float)tex.width / tex.height) * height;
        Rect coords = new Rect(0,0,1,1);
        
        GUI.DrawTextureWithTexCoords(new Rect(pos, new Vector2(px, height)), tex, coords);
        if (addSpace) {
            EditorGUILayout.Space(px);
        }
    }

    /// <summary>
    /// Draws texture in given rect and masks the not visible parts of the texture
    /// </summary>
    /// <param name="mask">Mask rect, in which texture is drawn.</param>
    /// <param name="tex">Texture to draw.</param>
    /// <param name="poi">Point of interest, zooms in on this position.</param>
    /// <param name="addSpace">Adds a guilayout space with the height of the image, to ensure layout items are placed below the image.</param>
    public static void DrawTextureScaledCropGUI(Rect mask, Texture2D tex, Vector2 poi, bool addSpace = true) {
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
        if (addSpace) {
            EditorGUILayout.Space(mask.height);
        }
    }
    
    /// <summary>
    /// Draws texture in given rect and masks the not visible parts of the texture
    /// </summary>
    /// <param name="mask">Mask rect, in which texture is drawn.</param>
    /// <param name="tex">Texture to draw.</param>
    /// <param name="poi">Point of interest, zooms in on this position.</param>
    /// <param name="addSpace">Adds a guilayout space with the height of the image, to ensure layout items are placed below the image.</param>
    public static void DrawTextureScaledScaleGUI(Rect mask, Texture2D tex, Vector2 poi, bool addSpace = true) {
        Vector2 res = new Vector2(tex.width, tex.height);
        Rect coords = new Rect(0,0,1,1);
        
        float px;

        float m = mask.width / mask.height;
        float t = (float)tex.width / tex.height;
		
        if (m > t) {
            //match y, scale x
            //find res x, when y has mask res
            px = (res.x / res.y) * mask.height;

            mask.x = (mask.width - px) / 2f;
            mask.width = px;
        }
        else {
            //match y, scale x
            //find res x, when y has mask res
            px = (res.y / res.x) * mask.width;

            mask.y = (mask.height - px) / 2f;
            mask.height = px;
        }
		
        GUI.DrawTextureWithTexCoords(mask, tex, coords);
        if (addSpace) {
            EditorGUILayout.Space(mask.height);
        }
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

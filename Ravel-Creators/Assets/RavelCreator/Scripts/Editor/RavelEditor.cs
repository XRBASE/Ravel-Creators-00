using System;
using Base.Ravel.Networking.Authorization;
using Base.Ravel.Networking.Organisations;
using Base.Ravel.Users;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manager class for editor user data and usefull editor functions
/// </summary>
public static class RavelEditor
{
    /// <summary>
    /// Project configuration.
    /// </summary>
    public static CreatorConfig CreatorConfig {
        get {
            if (_creatorConfig == null) {
                _creatorConfig = CreatorConfig.LoadCurrent();
            }

            return _creatorConfig;
        }
        set { _creatorConfig = value; }
    }
    private static CreatorConfig _creatorConfig;
    
    /// <summary>
    /// Per-bundle configurations, mostly version numbers.
    /// </summary>
    public static BundleConfig BundleConfig {
        get {
            if (_bundleConfig == null) {
                _bundleConfig = BundleConfig.LoadCurrent();
            }

            return _bundleConfig;
        }
        set { _bundleConfig = value; }
    }
    private static BundleConfig _bundleConfig;

    /// <summary>
    /// Checks if the user is set.
    /// </summary>
    public static bool LoggedIn {
        get { return User != null; }
    }
    
    /// <summary>
    /// Currently logged-in user.
    /// </summary>
    public static  User User { get; set; }
    
    /// <summary>
    /// Does the current user have dev:access? enables UUID copy buttons.
    /// </summary>
    public static bool DevUser { get; private set; }
    
    
    private static bool _retrievingOrganisations;
    private static Action<Organisation[], bool> _onOrgsRetrieved;
    
    /// <summary>
    /// This object contains custom settable branding files for the editor tools.
    /// </summary>
    public static RavelBranding Branding {
        get {
            if (_branding == null) {
                _branding = Resources.Load<RavelBranding>("Branding/BrandingConfig");
            }

            if (_branding == null) {
                Debug.LogError("Missing branding file (Resources/Branding/BrandingConfig.asset)");
            }
            return _branding;
        }
    }
    private static RavelBranding _branding;
    
    [MenuItem("Ravel/Clear editor cache", false)]
    public static void ClearCache() {
        if (EditorUtility.DisplayDialog("Clear cache",
                "This will delete all configuration data and version numbering, are you sure?", "Yes", "No")) {
            EditorCache.Clear();
        }
    }

    /// <summary>
    /// Sets the user (creator) after login.
    /// </summary>
    public static void OnLogin(User user) {
        User = user;
        RavelToolbar.RefreshConfig();
    }

    /// <summary>
    /// Set the system authorities, and determine if user should have access to development features.
    /// </summary>
    /// <param name="auths">authority responses from the backend.</param>
    public static void SetAuthorities(string[] auths) {
        for (int i = 0; i < auths.Length; i++) {
            if (auths[i] == "dev:access") {
                DevUser = true;
            }
        }
    }
    
    /// <summary>
    /// Logs out the user.
    /// </summary>
    public static void OnLogout(bool log) {
        User = null;
        PlayerCache.DeleteKey(LoginRequest.SYSTEMS_TOKEN_KEY);
        
        if (log)
            Debug.LogWarning("Not logged in, please open the account window and log in (Topbar, Ravel, Account).");
    }
    
    /// <summary>
    /// Retrieve the organisations of the currently logged in user.
    /// </summary>
    /// <param name="onRetrieved">Callback for when the organisations have been retrieved.</param>
    /// <param name="sender">Sender object, this object runs the coroutine and the coroutine stops when the object is deleted.</param>
    public static void GetUserOrganisations(Action<Organisation[], bool> onRetrieved, object sender) {
        if (User == null) {
            CreatorWindow.OpenAccount();
            throw new Exception("User not found, please log in first!");
        }
        
        if (User.Organisations != null) {
            Debug.LogWarning("Reusing previous organisations, to refresh organisations, please select refresh " +
                             "user data in the account window (Ravel>Creators>Account).");
            onRetrieved?.Invoke(User.Organisations, true);
            return;
        }
        
        //subscribe callbacks
        if (onRetrieved != null) {
            _onOrgsRetrieved += onRetrieved;
        }
        
        //if response not made make response, otherwise wait for response and call all subscribers. 
        if (!_retrievingOrganisations) {
            _retrievingOrganisations = true;
            EditorWebRequests.GetDataCollectionRequest<Organisation>(OrganisationRequest.GetUsersOrganisations(User.userUUID), 
                OnOrganisationsRetrieved, sender);
        }
    }

    /// <summary>
    /// Callback for when organisations have been retrieved. Calls all subscribed callbacks with user organisation data. 
    /// </summary>
    private static void OnOrganisationsRetrieved(Organisation[] organisations, bool success) {
        if (success) {
            User.Organisations = organisations;
        }
        
        _onOrgsRetrieved?.Invoke(organisations, success);
        
        _onOrgsRetrieved = null;
        _retrievingOrganisations = false;
    }

    /// <summary>
    /// Indent GUI with given amount of pixels.
    /// </summary>
    /// <param name="sizePx">size of indent.</param>
    public static void GUIBeginIndent(float sizePx) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(sizePx);
        GUILayout.BeginVertical();
    }

    /// <summary>
    /// End a previously opened indent (ends the most recent indent).
    /// </summary>
    public static void GUIEndIndent() {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
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
    /// <returns>Texture coords that show how the image was scaled</returns>
    public static Rect DrawTextureScaledCropGUI(Rect mask, Texture2D tex, Vector2 poi, bool addSpace = true) {
        Rect coords = GetScaleCropCoords(mask, tex, poi);
		
        GUI.DrawTextureWithTexCoords(mask, tex, coords);
        if (addSpace) {
            EditorGUILayout.Space(mask.height);
        }

        return coords;
    }

    public static Rect GetScaleCropCoords(Rect mask, Texture2D tex, Vector2 poi) {
        Vector2 res = new Vector2(tex.width, tex.height);
        Rect coords = new Rect(0,0,1,1);

        float pos;
        float px;
        float dec;

        float m = mask.width / mask.height;
        float t = (float)tex.width / tex.height;

        if (Mathf.Abs(m - t) <= MathBuddy.FloatingPoints.LABDA) {
            return coords;
        }
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
            pos = Mathf.Clamp(poi.x - dec / 2f, 0, 1f - dec);
            coords = new Rect(pos, 0, dec, 1);
        }

        return coords;
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

    /// <summary>
    /// Gets an array of names for the enum, removes values outside of start and length, if those values are set, otherwise returns all names.
    /// </summary>
    /// <param name="start">start index.</param>
    /// <param name="length">amount of names to return (0 returns all)</param>
    public static string[] GetEnumNames<T>(int start = 0, int length = 0) where T : Enum {
        T[] values = (T[]) Enum.GetValues(typeof(T));
        if (values.Length == 0)
            return Array.Empty<string>();
        
        if (length <= 0) {
            length = values.Length - start;
        }
        
        string[] names = new string[length];
        for (int i = 0, j = start; i < length; i++, j++) {
            names[i] = values[j].ToString();
        }

        return names;
    }

    /// <summary>
    /// Searches the project for files (outside of the scene) of this type, and returns them.
    /// </summary>
    /// <param name="filter">additional filtering, apart from the type.</param>
    public static T[] GetAllAssetsOfType<T>(string filter = "") where T : UnityEngine.Object {
        string[] paths = AssetDatabase.FindAssets($"t:{typeof(T)} {filter}");
        T[] data = new T[paths.Length];
        for (int i = 0; i < paths.Length; i++) {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            data[i] = (T) AssetDatabase.LoadAssetAtPath(paths[i], typeof(T));
        }

        return data;
    }
}

/// <summary>
/// Helper class that is invoked the moment unity starts up.
/// </summary>
[InitializeOnLoad]
class StartupHelper
{
    static StartupHelper() {
        //waits for the first update call, then opens the window and unsubscribes.
        //this is because the window doesn't stay open if it is opened on the actual start-up (it's too early).
        EditorApplication.update += StartUp;
    }

    static void StartUp() {
        EditorApplication.update -= StartUp;
        
        CreatorWindow wnd = CreatorWindow.GetWindow(CreatorWindow.State.Account, false);
        (wnd.Tab as AccountState).TryLoginWithToken(true);
    }
}

using Base.Ravel.Networking.Authorization;
using Base.Ravel.Users;
using UnityEditor;
using UnityEngine;

public static class RavelEditor
{
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
}

[InitializeOnLoad]
class StartupHelper
{
    static StartupHelper() {
        EditorApplication.update += StartUp;
        
    }

    static void StartUp() {
        EditorApplication.update -= StartUp;
        
        AccountWindow wnd = AccountWindow.GetWindow(false);
        wnd.TryLoginWithToken();
    }
}

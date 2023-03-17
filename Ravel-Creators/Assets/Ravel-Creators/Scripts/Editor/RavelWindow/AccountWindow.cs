using Base.Ravel.Networking;
using Base.Ravel.Networking.Authorization;
using Base.Ravel.Networking.Users;
using Base.Ravel.Users;
using UnityEditor;
using UnityEngine;

public class AccountWindow : EditorWindow
{
	private const float BANNER_HEIGHT = 100f;

	private Vector2 _scroll;
	
	private string email = "";
	private string pass = "";

	[MenuItem("Ravel/Account")]
	public static void OpenWindow() {
		GetWindow(true);
	}
	
	public static AccountWindow GetWindow(bool show = true) {
		AccountWindow wnd = GetWindow<AccountWindow>();
		
		if (show) {
			wnd.Show();	
		}
		return wnd;
	}

	private void OnGUI() {
		bool loggedIn = RavelEditor.User != null;

		if (RavelEditor.Branding.banner) {
			DrawTextureScaledGUI(new Rect(0, 0, position.width, BANNER_HEIGHT), RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}

		_scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(position.width));

		if (!loggedIn) { 
			GUILayout.Label($"email:");
			email = GUILayout.TextField(email);
			GUILayout.Label($"password");
			pass = GUILayout.PasswordField(pass, '*');
			if (GUILayout.Button("Log in")) {
				LoginUserPass(email, pass);
			}
		}
		else {
			GUILayout.Label($"Logged in as user {RavelEditor.User.FullName}");
			if (GUILayout.Button("Log out")) {
				RavelEditor.OnLogout();
			}
		}
		
		EditorGUILayout.EndScrollView();
	}

	private void DrawTextureScaledGUI(Rect mask, Texture2D tex, Vector2 poi) {
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
		EditorGUILayout.Space(BANNER_HEIGHT);
	}

	/// <summary>
	/// Tries to log in using the cached token and otherwise opens the window and shows the login screen.
	/// </summary>
	public void TryLoginWithToken() {
		if (PlayerCache.TryGetString(LoginRequest.SYSTEMS_TOKEN_KEY, out string jsonData)) {
			//move string into player cache, so token request will pick it up
			PlayerCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
			//uses token stored in cache
			RavelWebRequest req = UserRequest.GetSelf();
			EditorWebRequests.SendWebRequest(req, ProcessLoginResponse, this);
			Close();
		}
		else {
			//no login cached, show window
			Show();
		}
	}

	private void LoginUserPass(string email, string password) {
		RavelWebRequest login = LoginRequest.UserPassRequest(email, password);
		EditorWebRequests.SendWebRequest(login, ProcessLoginResponse, this);
	}

	private void ProcessLoginResponse(RavelWebResponse res) {
		if (res.Success && res.TryGetData(out User user)) {
			//this does not happen when checking an already cached token, but does happen when logging in using the window.
			if (res.TryGetData(out LoginRequest.TokenResponse token) && !string.IsNullOrEmpty(token.accessToken)) {
				string jsonData = JsonUtility.ToJson(token);
				//EditorCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
				PlayerCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
			}
			
			RavelEditor.OnLogin(user, token);
			email = "";
			pass = "";
		}
		else {
			RavelEditor.OnLogout();
		}
	}
}

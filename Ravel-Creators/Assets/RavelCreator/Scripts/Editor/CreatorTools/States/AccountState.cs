using Base.Ravel.Networking;
using Base.Ravel.Networking.Authorization;
using Base.Ravel.Networking.Users;
using Base.Ravel.Users;
using UnityEngine;

public class AccountState : CreatorWindowState
{
	private string email = "";
	private string pass = "";
	private bool _remember = false;

	public override CreatorWindow.State State {
		get { return CreatorWindow.State.Account; }
	}

	protected override Vector2 MinSize {
		get { return new Vector2(390, 210); }
	}

	public AccountState(CreatorWindow wnd) : base(wnd) {
		_remember = RavelEditor.CreatorPanelSettings.saveUserMail;
	}

	public override void OnSwitchState() {
		base.OnSwitchState();
		if (RavelEditor.CreatorPanelSettings.saveUserMail) {
			email = RavelEditor.CreatorPanelSettings.userMail;
		}
	}

	public override void OnGUI(Rect position) {
		if (!RavelEditor.LoggedIn) { 
			//When not logged in, show email, pass, login and remember me
			
			GUILayout.Label($"email:");
			email = GUILayout.TextField(email);
			GUILayout.Label($"password");
			pass = GUILayout.PasswordField(pass, '*');

			//if remember is disabled, this is local only (so log in with another account won't clear the cache)
			_remember = GUILayout.Toggle(_remember, "remember login");

			if (GUILayout.Button("Log in")) {
				//if remember is set to true, the mail and remember value are both saved in cache
				if (_remember) {
					RavelEditor.CreatorPanelSettings.saveUserMail = true;
					RavelEditor.CreatorPanelSettings.userMail = email;
					
					RavelEditor.CreatorPanelSettings.SaveConfig();
				}
				
				LoginUserPass(email, pass);
			}
		}
		else {
			//Otherwise show name of user, refresh button and copy uuid if you're a dev
			
			GUILayout.Label($"Logged in as user {RavelEditor.User.FullName}");
			if (RavelEditor.DevUser && GUILayout.Button("Copy UUID")) {
				GUIUtility.systemCopyBuffer = RavelEditor.User.userUUID;
			}
			
			if (GUILayout.Button("Refresh user data")) {
				TryLoginWithToken(false);
			}

			if (GUILayout.Button("Log out")) {
				RavelEditor.OnLogout(true);
			}
		}
	}
	
	/// <summary>
	/// Tries to log in using the cached token and otherwise opens the window and shows the login screen.
	/// </summary>
	public void TryLoginWithToken(bool closeWindow) {
		if (PlayerCache.TryGetString(LoginRequest.SYSTEMS_TOKEN_KEY, out string jsonData)) {
			//move string into player cache, so token request will pick it up
			PlayerCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
			//uses token stored in cache
			RavelWebRequest req = UserRequest.GetSelf();
			EditorWebRequests.SendWebRequest(req, ProcessLoginResponse, this);
			if (closeWindow)
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

	/// <summary>
	/// This is used to validate the token and to log in the user. It also gets the organisations for the user, so it can
	/// be used to refresh the data of the user as well. 
	/// </summary>
	/// <param name="res">WebResponse containing the login/user data.</param>
	private void ProcessLoginResponse(RavelWebResponse res) {
		if (res.Success && res.TryGetData(out User user)) {
			//this does not happen when checking an already cached token, but does happen when logging in using the window.
			if (res.TryGetData(out LoginRequest.TokenResponse token) && !string.IsNullOrEmpty(token.accessToken)) {
				string jsonData = JsonUtility.ToJson(token);
				//EditorCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
				PlayerCache.SetString(LoginRequest.SYSTEMS_TOKEN_KEY, jsonData);
			}
			
			RavelEditor.OnLogin(user);
			RavelEditor.GetUserOrganisations(null, this);
			//used for checking for dev users.
			RavelEditor.SetAuthorities(token.systemAuthorities);
			
			email = "";
			pass = "";
		}
		else {
			RavelEditor.OnLogout(true);
		}
	}
}

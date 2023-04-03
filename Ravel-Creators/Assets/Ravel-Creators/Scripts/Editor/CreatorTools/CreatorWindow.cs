using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Parent of the creator window. These gui calls happen for each of the tabs in the window.
/// The tabs are also called through this class.
/// </summary>
public class CreatorWindow : EditorWindow
{
	/// <summary>
	/// Currently open tab.
	/// </summary>
	public CreatorWindowState Tab {
		get {
			if (!_states.ContainsKey(_tab)) {
				_states.Add(_tab, CreateState(_tab));
			}
			return _states[_tab];
		}
	}

	private bool _bannerEnabled = true;
	
	private State _tab = State.Account;
	//used to detect state switching
	private State _prevTab = State.None;
	//contains references to all states
	private Dictionary<State, CreatorWindowState> _states = new Dictionary<State, CreatorWindowState>();
	
	[MenuItem("Ravel/Creator/Account", false, 1)]
	public static void OpenAccount() {
		GetWindow(State.Account);
	}

	[MenuItem("Ravel/Creator/Environments", false)]
	public static void OpenEnvironment() {
		GetWindow(State.Environments);
	}
	
	[MenuItem("Ravel/Creator/Images", false)]
	public static void OpenImages() {
		GetWindow(State.Images);
	}
	
	[MenuItem("Ravel/Creator/Bundles", false)]
	public static void OpenBundles() {
		GetWindow(State.Bundles);
	}
	
	[MenuItem("Ravel/Creator/Configuration", false)]
	public static void OpenConfig() {
		GetWindow(State.Configuration);
	}

	/// <summary>
	/// Either creates or retrieves reference to the creator window.
	/// </summary>
	/// <param name="tab">tab on which to open the creator window.</param>
	/// <param name="show">Should the window be opened, or just retrieved.</param>
	public static CreatorWindow GetWindow(State tab, bool show = true) {
		CreatorWindow wnd = GetWindow<CreatorWindow>();
		wnd.SwitchTab(tab);
		
		if (show) {
			wnd.Show();	
		}
		return wnd;
	}

	private void OnDestroy() {
		//called when the window is closed
		Tab.OnStateClosed();
	}

	/// <summary>
	/// Switch tab of window.
	/// </summary>
	public void SwitchTab(State newTab) {
		_tab = newTab;
	}

	public void EnableBanner(bool enabled) {
		_bannerEnabled = enabled;
	}

	/// <summary>
	/// Creates a new instance of a tab/state
	/// </summary>
	/// <param name="tab">state to create</param>
	public CreatorWindowState CreateState(State tab) {
		switch (tab) {
			case State.Account:
				return new AccountState(this);
			case State.Environments:
				return new EnvironmentState(this);
			case State.Images:
				return new ImageState(this);
			case State.Bundles:
				return new BundleState(this);
			case State.Configuration:
				return new ConfigState(this);
			default:
				throw new Exception($"Missing creator window state ({tab})");
		}
	}

	private void OnGUI() {
		if (!RavelEditor.LoggedIn) {
			SwitchTab(State.Account);
			//cannot switch tabs when not logged in
			GUI.enabled = false;
		}
		
		//plus and minus one correct for removing none from the state names
		_tab = (State)GUILayout.Toolbar((int)_tab - 1, RavelEditor.GetEnumNames<State>(1)) + 1;
		//if it was disabled, re-enable gui
		GUI.enabled = true;

		if (_tab != _prevTab) {
			if (_prevTab != State.None) {
				_states[_prevTab].OnStateClosed();				
			}
			Tab.OnSwitchState();
			_prevTab = _tab;
		}
		
		if (_bannerEnabled && RavelEditor.Branding.banner) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(0, GUILayoutUtility.GetLastRect().yMax, position.width, RavelEditor.Branding.bannerHeight), 
				RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}
		
		Tab.OnGUI(position);
	}
	
	/// <summary>
	/// Tabs for the creator window. New tabs also have to be added to the CreateState method.
	/// </summary>
	public enum State
	{
		None = 0,
		Account,
		Environments,
		Images,
		Bundles,
		Configuration,
	}
}

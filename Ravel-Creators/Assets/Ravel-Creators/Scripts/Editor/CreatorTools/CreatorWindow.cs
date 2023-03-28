using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathBuddy.Strings;

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

	private Vector2 _scroll;
	
	private State _tab;
	private Dictionary<State, CreatorWindowState> _states = new Dictionary<State, CreatorWindowState>();
	
	[MenuItem("Ravel/Creator/Account", false, 1)]
	public static void OpenAccount() {
		GetWindow(State.Account);
	}

	[MenuItem("Ravel/Creator/Environments", false, 1)]
	public static void OpenEnvironment() {
		GetWindow(State.Environments);
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

	/// <summary>
	/// Switch tab of window.
	/// </summary>
	public void SwitchTab(State newTab) {
		_tab = newTab;
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
		
		_tab = (State)GUILayout.Toolbar((int)_tab, GetStateNames());
		//if it was disabled, re-enable gui
		GUI.enabled = true;
		
		if (RavelEditor.Branding.banner) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(0, GUILayoutUtility.GetLastRect().yMax, position.width, RavelBranding.BANNER_HEIGHT), 
				RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}
		
		_scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(position.width));
		Tab.OnGUI(position);
			
		RavelEditor.DrawTextureScaledCropGUI(new Rect(position.width / 2f - 50,  GUILayoutUtility.GetLastRect().yMax, 100f, 100f), 
			RavelEditor.Branding.daan, Vector2.one * 0.5f);
			
		EditorGUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Re-formats the state enum below into more readable names, so they can be shown as the names of the tabs.
	/// </summary>
	private string[] GetStateNames() {
		string[] names = Enum.GetNames(typeof(State));
		for (int i = 0; i < names.Length; i++) {
			names[i] = names[i].ToString(StringExtentions.NamingCastType.UpperCamelCase,
				StringExtentions.NamingCastType.UserFormatting);
		}

		return names;
	}
	
	/// <summary>
	/// Tabs for the creator window. New tabs also have to be added to the CreateState method.
	/// </summary>
	public enum State
	{
		Account,
		Environments,
	}
}

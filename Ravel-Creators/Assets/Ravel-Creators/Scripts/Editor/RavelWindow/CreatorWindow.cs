using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathBuddy.Strings;

public class CreatorWindow : EditorWindow
{
	public CreatorWindowState Tab {
		get {
			if (!_states.ContainsKey(_tab)) {
				_states.Add(_tab, GetState(_tab));
			}
			return _states[_tab];
		}
	}

	private Vector2 _scroll;
	
	private State _tab;
	private Dictionary<State, CreatorWindowState> _states = new Dictionary<State, CreatorWindowState>();

	//[MenuItem("Ravel/Creator", false, 0)]
	public static void OpenWindow() {
		GetWindow();
	}
	
	[MenuItem("Ravel/Creator/Account", false, 1)]
	public static void OpenAccount() {
		GetWindow(State.Account);
	}

	[MenuItem("Ravel/Creator/Environments", false, 1)]
	public static void OpenEnvironment() {
		GetWindow(State.Environments);
	}

	public static CreatorWindow GetWindow(State s = State.None, bool show = true) {
		CreatorWindow wnd = GetWindow<CreatorWindow>();
		if (s != State.None) {
			wnd.SetState(s);
		}
		
		if (show) {
			wnd.Show();	
		}
		return wnd;
	}

	public void SetState(State newState) {
		_tab = newState;
	}

	public CreatorWindowState GetState(State s) {
		switch (s) {
			case State.Account:
				return new AccountState(this);
			case State.Environments:
				return new EnvironmentState(this);
			default:
				throw new Exception($"Missing creator window state ({s})");
		}
	}

	private void OnGUI() {
		if (!RavelEditor.LoggedIn) {
			_tab = State.Account;
			GUI.enabled = false;
		}
		
		_tab = (State)GUILayout.Toolbar((int)_tab, GetStateNames());
		GUI.enabled = true;
		
		if (RavelEditor.Branding.banner) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(0, GUILayoutUtility.GetLastRect().yMax, position.width, RavelBranding.BANNER_HEIGHT), 
				RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}

		
		if (_tab != State.None) {
			_scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(position.width));
			Tab.OnGUI(position);
		}
		
		if (RavelEditor.Branding.daan) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(position.width / 2f - 50,  GUILayoutUtility.GetLastRect().yMax, 100f, 100f), 
				RavelEditor.Branding.daan, Vector2.one * 0.5f);
		}

		if (_tab != State.None) {
			EditorGUILayout.EndScrollView();
		}
	}

	private string[] GetStateNames() {
		string[] names = Enum.GetNames(typeof(State));
		for (int i = 0; i < names.Length; i++) {
			names[i] = names[i].ToString(StringExtentions.NamingCastType.UpperCamelCase,
				StringExtentions.NamingCastType.UserFormatting);
		}

		return names;
	}

	public enum State
	{
		None = 0,
		Account,
		Environments,
	}
}

using System;
using Base.Ravel.Networking;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CreateEnvironmentWindow : EditorWindow
{
	private Environment _environment;
	private Vector2 _scroll;

	[MenuItem("Ravel/Create new environment", false, 3)]
	public static void OpenWindow() {
		GetWindow();
	}

	public static CreateEnvironmentWindow GetWindow() {
		CreateEnvironmentWindow wnd = GetWindow<CreateEnvironmentWindow>();
		return wnd;
	}

	private void OnGUI() {
		if (!RavelEditor.LoggedIn) {
			Debug.LogError("Please log in before creating environments!");
			Close();
			CreatorWindow.GetWindow(CreatorWindow.State.Account);

			return;
		}
		
		_scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(position.width));
		
		if (RavelEditor.Branding.banner) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(0, 0, position.width, RavelBranding.BANNER_HEIGHT), 
				RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}
		EditorGUILayout.Space(RavelBranding.INDENT_SMALL);

		if (_environment == null) {
			_environment = new Environment();
			_environment.isPublic = true;
		}

		bool canCreate = true;

		_environment.name = EditorGUILayout.TextField("Name: ", _environment.name);
		if (string.IsNullOrWhiteSpace(_environment.name)) {
			canCreate = false;
		}
		
		_environment.shortSummary = EditorGUILayout.TextField("Short summary: ", _environment.shortSummary);
		if (string.IsNullOrWhiteSpace(_environment.shortSummary)) {
			canCreate = false;
		}
		
		_environment.longSummary = EditorGUILayout.TextField("Long summary: ",_environment.longSummary);
		if (string.IsNullOrWhiteSpace(_environment.longSummary)) {
			canCreate = false;
		}

		_environment.isPublic = GUILayout.SelectionGrid((_environment.isPublic ? 0 : 1), new[] { "Public", "Private" }, 2) == 0;

		EditorGUILayout.Space(RavelBranding.INDENT_SMALL);
		GUI.enabled = canCreate;
		if (GUILayout.Button("Create")) {
			CreatorRequest req = CreatorRequest.CreateEnvironment(RavelEditor.User.userUUID, _environment);
			EditorWebRequests.SendWebRequest(req, OnCreateSent, this);
		}
		GUI.enabled = true;
		EditorGUILayout.EndScrollView();
	}

	private void OnCreateSent(RavelWebResponse res) {
		if (res.Success) {
			string json = EnvironmentExtensions.RenameStringFromBackend(res.DataString);
			Environment newEnv = JsonConvert.DeserializeObject<Environment>(json);
			
			EnvironmentSO so = CreateInstance<EnvironmentSO>();
			so.environment = newEnv;

			string path = EditorUtility.SaveFilePanel("Save environment", Application.dataPath, 
				$"ENV_{newEnv.name}", "asset");

			if (string.IsNullOrEmpty(path)) {
				if (EditorUtility.DisplayDialog("Cancel save?",
					    "Do you want to cancel saving?" +
					    "The environment has already been created, you are only cancelling saving the file." +
					    "To delete this environment, use the environment tab of the ravel window or the website.",
					    "Save environment", "Cancel save")) {

					path = EditorUtility.SaveFilePanel("Save environment", Application.dataPath,
						$"ENV_{newEnv.name}", "asset");
				} else {
					Debug.LogWarning("Environment created, but not saved.");
				}
			}

			if (!string.IsNullOrEmpty(path)) {
				path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
				
				AssetDatabase.CreateAsset(so, path);
				
				AssetDatabase.Refresh();

				Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
				Close();
			}
			
			_environment = new Environment();
		}
		else {
			Debug.LogError($"Failed to create environment ({res.Error.FullMessage}) ({res.DataString})");
		}
	}
}

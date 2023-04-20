using Base.Ravel.Networking;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Separate window for creating a new environment on the server.
/// </summary>
public class CreateEnvironmentWindow : EditorWindow
{
	//data class for caching users input
	private Environment _environment;
	
	[MenuItem("Ravel/Create new environment", false, 3)]
	public static void OpenWindow() {
		CreateEnvironmentWindow wnd = GetWindow<CreateEnvironmentWindow>();
		wnd.minSize = new Vector2(290, 230);
	}

	private void OnGUI() {
		if (!RavelEditor.LoggedIn) {
			Debug.LogError("Please log in before creating environments!");
			Close();
			CreatorWindow.GetWindow(CreatorWindow.State.Account);

			return;
		}
		
		if (RavelEditor.Branding.banner) {
			RavelEditor.DrawTextureScaledCropGUI(new Rect(0, 0, position.width, RavelEditor.Branding.bannerHeight), 
				RavelEditor.Branding.banner, RavelEditor.Branding.bannerPOI);
		}
		EditorGUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);
		
		if (_environment == null) {
			_environment = new Environment();
			//auto set to true, as it is the easier option of the two.
			_environment.isPublic = true;
		}

		//checks if enough data is set for a create.
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

		EditorGUILayout.Space(RavelEditorStying.GUI_SPACING_MICRO);
		GUI.enabled = canCreate;
		if (GUILayout.Button("Create")) {
			CreatorRequest req = CreatorRequest.CreateEnvironment(RavelEditor.User.userUUID, _environment);
			EditorWebRequests.SendWebRequest(req, OnCreateResponse, this);
		}
		GUI.enabled = true;
	}

	/// <summary>
	/// Callback from server after calling create environment.
	/// </summary>
	/// <param name="res">WebResponse data</param>
	private void OnCreateResponse(RavelWebResponse res) {
		if (res.Success) {
			//renames public var so it can be used in dotnet
			string json = EnvironmentExtensions.RenameStringFromBackend(res.DataString);
			Environment newEnv = JsonUtility.FromJson<Environment>(json);
			
			EnvironmentSO so = CreateInstance<EnvironmentSO>();
			so.environment = newEnv;

			//Open saving dialogs, so user can save the file at her chosen location, or cancel saving.
			string path = EditorUtility.SaveFilePanel("Save environment", RavelEditor.CreatorConfig.GetFilePath(), 
				$"ENV_{newEnv.name}", "asset");

			if (string.IsNullOrEmpty(path)) {
				if (EditorUtility.DisplayDialog("Cancel save?",
					    "Do you want to cancel saving?" +
					    "The environment has already been created, you are only cancelling saving the file." +
					    "To delete this environment, use the environment tab of the ravel window or the website.",
					    "Save environment", "Cancel save")) {

					path = EditorUtility.SaveFilePanel("Save environment", RavelEditor.CreatorConfig.GetFilePath(),
						$"ENV_{newEnv.name}", "asset");
				} else {
					Debug.LogWarning("Environment created, but not saved.");
				}
			}

			//Cache chosen path, create asset and refresh asset database
			if (!string.IsNullOrEmpty(path)) {
				RavelEditor.CreatorConfig.SetFilePath(path);

				if (!path.Contains(Application.dataPath)) {
					Debug.LogError("Chosen path should be inside of the project for environment asset files.");
					return;
				}
				path = path.Substring(Application.dataPath.Length - 6);
				
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

using System.Xml.Resolvers;
using UnityEditor;
using UnityEngine;

public class CreateEnvironmentWindow : EditorWindow
{
	private Environment _environment;
	
	[MenuItem("Ravel/CreateEnv", false, 3)]
	public static void OpenWindow() {
		GetWindow();
	}

	public static CreateEnvironmentWindow GetWindow() {
		CreateEnvironmentWindow wnd = EditorWindow.GetWindow<CreateEnvironmentWindow>();

		wnd._environment = new Environment();
		return wnd;
	}

	private void OnGUI() {
		if (!RavelEditor.LoggedIn) {
			Debug.LogError("Please log in before creating environments!");
			Close();
			CreatorWindow.GetWindow(CreatorWindow.State.Account);
		}

		bool canCreate = true;

		_environment.name = EditorGUILayout.TextField("Name: ", _environment.name);
		if (string.IsNullOrWhiteSpace(_environment.name)) {
			canCreate = false;
		}
		
		_environment.shortSummary = EditorGUILayout.TextArea("Short summary: ", _environment.shortSummary);
		if (string.IsNullOrWhiteSpace(_environment.shortSummary)) {
			canCreate = false;
		}
		
		_environment.longSummary = EditorGUILayout.TextArea("Long summary: ", _environment.longSummary);
		if (string.IsNullOrWhiteSpace(_environment.longSummary)) {
			canCreate = false;
		}
		
		_environment.isPublic = EditorGUILayout.Toggle("public environment: ", _environment.isPublic);

		if (canCreate && GUILayout.Button("Create")) {
			
		}
	}
}

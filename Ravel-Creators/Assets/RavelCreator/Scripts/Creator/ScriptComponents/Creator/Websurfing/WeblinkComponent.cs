using Base.Ravel.Creator.Components;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Component used to open a given webURL as the result of a trigger event.
/// </summary>
[AddComponentMenu("Ravel/Websurfing/Weblink component")]
public partial class WeblinkComponent : ComponentBase
{
	public override ComponentData Data {
		get { return null; }
	}

	[SerializeField] private string url;
	
	protected override void BuildComponents() { } 
	protected override void DisposeData() { }
	public void OpenURL() { }

	private void OpenUrlDirect() {
		Application.OpenURL(url);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(WeblinkComponent))]
	private class WeblinkComponentEditor : Editor
	{

		public override void OnInspectorGUI() {
			WeblinkComponent instance = (WeblinkComponent)target;

			DrawDefaultInspector();
			EditorGUILayout.BeginHorizontal();
			if (!string.IsNullOrEmpty(instance.url) && 
			    !(instance.url.StartsWith("https://") || instance.url.StartsWith("http://"))) {

				EditorGUILayout.HelpBox("Please add Https:// in front of your url, as the weblink will otherwise " +
				                        "be opened as subdomain of Ravel (i.e. Https://Ravel.com/[URL]).", MessageType.Warning);
				
				if (GUILayout.Button("fix", GUILayout.Width(100), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f))) {
					instance.url = "https://" + instance.url;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			if (GUILayout.Button("Open url")) {
				instance.OpenUrlDirect();
			}
		}
	}
#endif
}

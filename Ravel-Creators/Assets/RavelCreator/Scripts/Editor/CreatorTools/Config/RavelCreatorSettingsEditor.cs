#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class RavelCreatorSettings
{
	[CustomEditor(typeof(RavelCreatorSettings))]
	public class RavelCreatorSettingsEditor : Editor
	{
		private const float BTN_WIDTH = 100;
		
		private RavelCreatorSettings _instance;

		private void OnEnable() {
			_instance = (RavelCreatorSettings)target;
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			GUIDrawFolderAndSetBtn(ref _instance._filePath,
				new GUIContent("File path", $"(Last used) Path that is opened when accessing files.\n {_instance._filePath}"));
			GUIDrawFolderAndSetBtn(ref _instance._bundlePath,
				new GUIContent("Bundle path", $"The location at which assetbundles are saved.\n {_instance._bundlePath}"));
		}

		private void GUIDrawFolderAndSetBtn(ref string refField, GUIContent content) {
			EditorGUILayout.BeginHorizontal();
			string data = EditorGUILayout.TextField(content, refField);
			if (Directory.Exists(data)) {
				refField = data;
			}
			else {
				Debug.LogWarning("Filled in path is invalid.");
			}

			if (GUILayout.Button("Select folder", GUILayout.Width(BTN_WIDTH))) {
				string path = EditorUtility.OpenFolderPanel($"Select {content.text}", refField, "");
				if (!string.IsNullOrEmpty(path)) {
					refField = path;
				}
				else {
					Debug.LogWarning("Setting file path canceled.");
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif
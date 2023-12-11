#if UNITY_EDITOR
using System;
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

			GUIDrawFolderAndSetBtn(_instance._filePath, _instance.SetFilePath,
				new GUIContent("File path", $"(Last used) Path that is opened when accessing files.\n {_instance._filePath}"));
			GUIDrawFolderAndSetBtn(_instance._bundlePath, _instance.SetBundlePath,
				new GUIContent("Bundle path", $"The location at which assetbundles are saved.\n {_instance._bundlePath}"));
		}

		private void GUIDrawFolderAndSetBtn(string value, Action<string> setValueAction, GUIContent content) {
			EditorGUILayout.BeginHorizontal();
			
			string data = EditorGUILayout.TextField(content, value);
			if (!Directory.Exists(data)) {
				Debug.LogWarning("Filled in path is invalid.");
				
				//set data back to the same value, so value does not change.
				data = value;
			}

			if (GUILayout.Button("Select folder", GUILayout.Width(BTN_WIDTH))) {
				data = EditorUtility.OpenFolderPanel($"Select {content.text}", value, "");
				
				if (string.IsNullOrEmpty(data)) {
					Debug.LogWarning("Setting file path canceled.");
					
					//set data back to the same value, so value does not change.
					data = value;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (value != data) {
				setValueAction?.Invoke(data);
			}
		}
	}
}
#endif
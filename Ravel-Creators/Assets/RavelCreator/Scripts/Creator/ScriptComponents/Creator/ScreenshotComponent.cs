using System;
using Base.Ravel.Creator.Components;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Ravel/Screenshot downloader")]
public partial class ScreenshotComponent : ComponentBase
{
	public override ComponentData Data {
		get { return _data; }
	}
	[SerializeField, HideInInspector] private ScreenshotComponentData _data;

	protected override void BuildComponents() { }

	protected override void DisposeData() { }

	public void DownloadScreenshot() { }

#if UNITY_EDITOR
	[CustomEditor(typeof(ScreenshotComponent))]
	private class ScreenshotComponentEditor : Editor
	{
		private ScreenshotComponent _instance;
		private SerializedProperty _data;
		private SerializedProperty _evtProperty;

		public void OnEnable() {
			_instance = (ScreenshotComponent)target;
			_data = serializedObject.FindProperty("_data");
		}

		public override void OnInspectorGUI() {
			

			DrawDefaultInspector();
			EditorGUI.BeginChangeCheck();
			_instance._data.fileName = EditorGUILayout.TextField(
				new GUIContent("Filename", "The initial filename of the file, user can still change it."),
				_instance._data.fileName);
			
			_instance._data.sizeMin = EditorGUILayout.ObjectField(
				new GUIContent("Min anchor", "The minimal screen position of the made screenshot (Lower left corner of the image)."),
				_instance._data.sizeMin, typeof(Transform), true) as Transform;
			_instance._data.sizeMax = EditorGUILayout.ObjectField(
				new GUIContent("Max anchor", "The maximum screen position of the made screenshot (upper right corner of the image)."),
				_instance._data.sizeMax, typeof(Transform), true) as Transform;
			
			GUIDrawCallback("beforeScreenshot");
			GUIDrawCallback("afterScreenshot");

			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(_instance);
			}
		}
		
		/// <summary>
		/// Draws event property with given name in GUI.
		/// </summary>
		private void GUIDrawCallback(string propertyName) {
			_evtProperty = _data.FindPropertyRelative(propertyName);
			EditorGUILayout.PropertyField(_evtProperty);
			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}

[Serializable]
public class ScreenshotComponentData : ComponentData
{
	public string fileName = "ravel-screenshot";
	public Transform sizeMin, sizeMax;
	public UnityEvent beforeScreenshot, afterScreenshot;
}

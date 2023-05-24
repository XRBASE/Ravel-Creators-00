using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Used to create a container for loading files in, either 2D or 3D.
	/// </summary>
	[AddComponentMenu("Ravel/File content")]
	public partial class FileContentComponent : ComponentBase, INetworkId
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField, HideInInspector] protected FileContentData _data;

		public bool Networked { get { return true; } }

		public int ID {
			get { return _data.id; }
			set { _data.id = value; }
		}

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		/// <summary>
		/// This resets the visuals to the original visuals in the scene.
		/// </summary>
		public void CloseFile() { }

		/// <summary>
		/// This changes the current file into another file (pop-up opens for selecting what file)
		/// </summary>
		public void ChangeFile() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(FileContentComponent))]
		private class FileContentComponentEditor : Editor
		{
			private SerializedProperty _data;
			private SerializedProperty _fileLoadedEvent;
			private SerializedProperty _fileClosedEvent;
			private FileContentComponent _instance;

			public void OnEnable() {
				_instance = (FileContentComponent)target;
				
				_data = serializedObject.FindProperty("_data");
				_fileLoadedEvent = _data.FindPropertyRelative("onFileLoaded");
				_fileClosedEvent = _data.FindPropertyRelative("onFileClosed");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				EditorGUI.BeginChangeCheck();
				//2D or 3D file
				_instance._data.type =
					(FileContentData.Type)EditorGUILayout.EnumPopup("Content type", _instance._data.type);

				EditorGUILayout.Space();
				//used for future implementation of CMS
				_instance._data.name = EditorGUILayout.TextField("Name", _instance._data.name);
				GUILayout.Label("Description");
				_instance._data.description = EditorGUILayout.TextArea(_instance._data.description);

				EditorGUILayout.Space();
				//screens need a canvas
				if (_instance._data.type == FileContentData.Type.Screen_2D) {
					_instance._data.canvas =
						EditorGUILayout.ObjectField("Canvas", _instance._data.canvas, typeof(Canvas), true) as Canvas;
					
					if (_instance._data.canvas == null) {
						EditorGUILayout.HelpBox("File content (Screen_2D) is missing a canvas reference, the component won't work.",
							MessageType.Error);
					}
				}

				//Selection collider.
				_instance._data.collider =
					EditorGUILayout.ObjectField("Collider, selector", _instance._data.collider, typeof(BoxCollider), true) as
						BoxCollider;
				if (_instance._data.collider == null) {
					EditorGUILayout.HelpBox($"File content ({_instance._data.type}) is missing a collider reference, the component won't work.",
						MessageType.Error);
				}
				
				EditorGUILayout.Space();
				//Event for open and closing file.
				EditorGUILayout.PropertyField(_fileLoadedEvent);
				EditorGUILayout.PropertyField(_fileClosedEvent);
				serializedObject.ApplyModifiedProperties();
				
				EditorGUILayout.Space();
				//Canvas needs specific alignment, this shows an error and fix button if that alignment is not used.
				if (_instance._data.type == FileContentData.Type.Screen_2D && _instance._data.canvas != null &&
				    (((RectTransform)_instance._data.canvas.transform).pivot - new Vector2(0.5f, 0f)).magnitude >
				    0.001f) {
					
					EditorGUILayout.HelpBox("File content (Screen_2D): Canvas pivot should be in the lower center (0.5f, 0f) of the screen",
						MessageType.Error);
					if (GUILayout.Button("Fix")) {
						((RectTransform)_instance._data.canvas.transform).pivot = new Vector2(0.5f, 0f);
						EditorUtility.SetDirty(_instance._data.canvas);
					}
				}
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
		}
#endif
	}

	[Serializable]
	public class FileContentData : ComponentData
	{
		public int id;
		public Type type;
		public string name, description;
		public BoxCollider collider;
		
		public UnityEvent onFileLoaded;
		public UnityEvent onFileClosed;

		//screen only
		public Canvas canvas;
		

		public enum Type
		{
			Screen_2D,
			Model_3D
		}
	}
}
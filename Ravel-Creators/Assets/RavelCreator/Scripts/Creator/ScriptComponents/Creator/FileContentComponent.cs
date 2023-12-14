using System;
using Base.Ravel.Creator.Components.Naming;
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
	[HelpURL("https://www.notion.so/thenewbase/File-content-f80dd126bba14d40b9b63ca036fa051f")]
	public partial class FileContentComponent : ComponentBase, IUniqueId, INameIdentifiedObject
	{
		public override ComponentData Data {
			get { return _data; }
		}
		
		public bool SetUniqueID { get { return true; } }

		public int ID {
			get { return _data.id; }
			set { _data.id = value; }
		}

		public string Name {
			get { return _data.name; }
		}

		public FileContentData.Type Type {
			get { return _data.type; }
		}
		
		public DynamicContentMetaData MetaData {
			get { return _metaData; }
		}

		[SerializeField] protected FileContentData _data;
		[SerializeField] protected DynamicContentMetaData _metaData;

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
			private FileContentComponent _instance;
			private bool _namePass = false;

			public void OnEnable() {
				_instance = (FileContentComponent)target;

				_namePass = false;
			}

			public override void OnInspectorGUI() {
				EditorGUI.BeginChangeCheck();
				_instance._data.name = EditorGUILayout.TextField("Name", _instance._data.name);
				
				if (!_namePass || EditorGUI.EndChangeCheck()) {
					if (!NameAvailabilityCheck.Check(_instance)) {
						if (string.IsNullOrEmpty(_instance.Name)) {
							EditorGUILayout.HelpBox(
								"No name set for filecomponent, file components require a unique name for content managenent!",
								MessageType.Error);
						}
						else {
							EditorGUILayout.HelpBox(
								$"File content name: {_instance._data.name} was used multiple times, please use unique names for file content!",
								MessageType.Error);
						}

						return;
					}
					
					_namePass = true;
					EditorUtility.SetDirty(_instance);
				}
				
				DrawDefaultInspector();
				
				if (_instance._data.type == FileContentData.Type.Screen_2D) {
					if (_instance._data.canvas != null) {
						EditorGUILayout.HelpBox($"2D content requires a canvas, on which content can be displayed.",
							MessageType.Error);

						if ((((RectTransform)_instance._data.canvas.transform).pivot - new Vector2(0.5f, 0f))
						    .magnitude >
						    0.001f) {
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.HelpBox("File content (Screen_2D): Canvas pivot should be in the lower center (0.5f, 0f) of the screen",
								MessageType.Error);
							if (GUILayout.Button("Fix", GUILayout.Width(100))) {
								((RectTransform)_instance._data.canvas.transform).pivot = new Vector2(0.5f, 0f);
								EditorUtility.SetDirty(_instance._data.canvas);
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				else if (_instance._data.canvas != null) {
					EditorGUILayout.HelpBox($"3D content does not require a canvas.", MessageType.Info);
				}
			}
		}
#endif
	}

	[Serializable]
	public class FileContentData : ComponentData
	{
		[HideInInspector] public int id;
		public Type type;
		[HideInInspector] public string name;
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
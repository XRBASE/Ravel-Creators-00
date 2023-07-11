using System;
using Base.Ravel.Creator.Components.Naming;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games.Collectibles
{
	[AddComponentMenu("Ravel/Games/Collectibles/Collectible")]
	public partial class CollectibleComponent : ComponentBase, IUniqueId, INameIdentifiedObject
	{
		//even when not networked, the collectibles require unique id's, so always pass this value as true.
		public bool SetUniqueID {
			get { return true; }
		}

		public int ID {
			get { return _data.id; }
			set { _data.id = value; }
		}

		public string Name {
			get { return _data.name; }
			set { _data.name = value; }
		}

		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField, HideInInspector] private CollectibleData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public void Collect() { }
		public void ResetItem() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(CollectibleComponent))]
		private class CollectibleComponentEditor : Editor
		{
			private CollectibleComponent _instance;
			private SerializedProperty _data;
			private SerializedProperty _evtProperty;

			private bool _nameValid = false;

			private void OnEnable() {
				_instance = (CollectibleComponent)target;
				_data = serializedObject.FindProperty("_data");

				if (string.IsNullOrEmpty(_instance._data.name)) {
					_instance._data.name = _instance.gameObject.name;
				}
				_nameValid = NameAvailabilityCheck.Check(_instance);
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();
				bool dirty = false;
				EditorGUI.BeginChangeCheck();

				_instance._data.name = EditorGUILayout.TextField("Name", _instance._data.name);
				if (EditorGUI.EndChangeCheck()) {
					dirty = true;
					
					_nameValid = NameAvailabilityCheck.Check(_instance);
				}

				if (!_nameValid) {
					EditorGUILayout.HelpBox($"Collectible name should be unique. Name {_instance._data.name} is already taken!", MessageType.Error);
				}
				
				
				if (!dirty) {
					EditorGUI.BeginChangeCheck();
				}

				GUIDrawCallback("onCollected");
				GUIDrawCallback("onReset");

				if (dirty || EditorGUI.EndChangeCheck()) {
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
	public class CollectibleData : ComponentData
	{
		public int id;
		public string name;

		public UnityEvent onCollected;
		public UnityEvent onReset;
	}
}
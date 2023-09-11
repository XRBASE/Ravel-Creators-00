using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games.Collectibles
{
	[AddComponentMenu("Ravel/Games/Collectibles/CollectibleGame")]
	public partial class CollectiblesGameComponent : RavelGameComponent
	{
		public override RavelGameData BaseData {
			get { return _data; }
		}
		
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField, HideInInspector] private CollectibleGameData _data;
		protected override void BuildComponents() { }

		protected override void DisposeData() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(CollectiblesGameComponent), true)]
		private class CollectiblesGameComponentEditor : RavelGameComponentEditor
		{
			private CollectiblesGameComponent _instance;
			private SerializedProperty _data;
			private SerializedProperty _serializedProperty;

			private string _nameCache;

			protected override void OnEnable() {
				base.OnEnable();
				_instance = (CollectiblesGameComponent)target;
				_data = serializedObject.FindProperty("_data");
			}
			
			public override void OnInspectorGUI() {
				base.OnInspectorGUI();
				EditorGUI.BeginChangeCheck();
				
				_instance._data.networked = EditorGUILayout.Toggle("Networked", _instance._data.networked);
				_instance._data.collectibleScore = EditorGUILayout.IntField(
					new GUIContent("Score factor", "Points earned per collected item"), _instance._data.collectibleScore);
				
				GUIDrawProperty("collectibles");
            
				GUIDrawProperty("onAnyCollected");
				GUIDrawProperty("onAllCollected");
				GUIDrawProperty("onGameProgress");
				
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
			
			/// <summary>
			/// Draws event property with given name in GUI.
			/// </summary>
			private void GUIDrawProperty(string propertyName) {
				_serializedProperty = _data.FindPropertyRelative(propertyName);
				EditorGUILayout.PropertyField(_serializedProperty);
				serializedObject.ApplyModifiedProperties();
			}
		}
#endif
	}
	
	[Serializable]
	public class CollectibleGameData : RavelGameData
	{
		public bool networked;
		public int collectibleScore = 1;

		public UnityEvent onAnyCollected;
		public UnityEvent onAllCollected;

		public List<CollectibleComponent> collectibles;
	}
}
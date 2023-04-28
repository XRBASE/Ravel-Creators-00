using System;
using System.Collections.Generic;
using Base.Ravel.UI.Hints;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	public partial class HintComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}
		[SerializeField, HideInInspector] private HintData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		public void TriggerHint() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(HintComponent))]
		private class HintComponentEditor : Editor
		{
			private HintComponent _instance;
			private SerializedProperty _data;
			private SerializedProperty _hints;

			private void OnEnable() {
				_instance = (HintComponent)target;
				_data = serializedObject.FindProperty("_data");
				_hints = _data.FindPropertyRelative("hints");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				_instance._data.showOnAwake = EditorGUILayout.Toggle("Show hints on awake", _instance._data.showOnAwake);
				
				EditorGUILayout.PropertyField(_hints);
				serializedObject.ApplyModifiedProperties();

				if (GUILayout.Button("Create new hint") && Hint.CreateAndSaveHintEditor(out Hint hint)) {
					_instance._data.hints.Add(hint);
				}
			}
		}
#endif
	}
	
	[Serializable]
	public class HintData : ComponentData
	{
		public bool showOnAwake;
		public List<Hint> hints = new List<Hint>();
	}
}
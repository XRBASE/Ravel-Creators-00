using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	public partial class TimerComponent : ComponentBase
	{
		public override ComponentData Data { get; }
		[SerializeField, HideInInspector] private TimerData _data;
		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void StartTimer() { }

		/// <summary>
		/// Resets the timer, so that it starts counting again.
		/// If the timer has stopped, start needs to be called again after this call.
		/// </summary>
		public void ResetTimer() { }

		/// <summary>
		/// Stops the timer from counting without firing the attached events.
		/// </summary>
		public void StopTimer() { }

		/// <summary>
		/// Stops the timer from counting and fires the attached events.
		/// </summary>
		public void FinishTimer() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(TimerComponent))]
		private class TimerComponentEditor : Editor
		{
			private TimerComponent _instance;
			private SerializedProperty _data;
			private SerializedProperty onStartEvt;
			private SerializedProperty onFinEvt;

			private void OnEnable() {
				_instance = (TimerComponent)target;
				_data = serializedObject.FindProperty("_data");
				onStartEvt = _data.FindPropertyRelative("onStart");
				onFinEvt = _data.FindPropertyRelative("onFinish");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				EditorGUI.BeginChangeCheck();
				_instance._data.duration =
					Mathf.Abs(EditorGUILayout.FloatField("Timer duration", _instance._data.duration));
				_instance._data.startOnAwake = EditorGUILayout.Toggle("Start on awake", _instance._data.startOnAwake);

				EditorGUILayout.PropertyField(onStartEvt);
				EditorGUILayout.PropertyField(onFinEvt);

				serializedObject.ApplyModifiedProperties();

				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
		}
#endif
	}

	[Serializable]
	public class TimerData : ComponentData
	{
		public float duration;
		public bool startOnAwake;
		public UnityEvent onStart;
		public UnityEvent onFinish;
	}
}
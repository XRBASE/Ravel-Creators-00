using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	//this should change the help url, check it though
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

			private void OnEnable() {
				_instance = (TimerComponent)target;
				_data = serializedObject.FindProperty("_data");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				EditorGUI.BeginChangeCheck();
				_instance._data.duration =
					Mathf.Abs(EditorGUILayout.FloatField("Timer duration", _instance._data.duration));
				_instance._data.startOnAwake = EditorGUILayout.Toggle("Start on awake", _instance._data.startOnAwake);

				_instance._data.hasCountdown = EditorGUILayout.Toggle(
					new GUIContent("Has countdown", "countdowns use UI to show the timer counting down to zero"),
					_instance._data.hasCountdown);
				if (_instance._data.hasCountdown) {
					_instance._data.speed = Mathf.Max(1, EditorGUILayout.IntField(
						new GUIContent("Speed", "How fast does the system count down (1x = once every second)."), _instance._data.speed));

					_instance._data.animateProps = (CountDownAnimationProps)
						EditorGUILayout.EnumFlagsField(
							new GUIContent("Animate properties", "Properties that fade/transition while counting down"),
							_instance._data.animateProps);
					
					_instance._data.oddText = EditorGUILayout.ObjectField(
						new GUIContent("Odd number output", "Odd numbers are shown in this TMP_Field."), 
						_instance._data.oddText, typeof(TMP_Text), true) as TMP_Text;
					
					_instance._data.evenText = EditorGUILayout.ObjectField(
						new GUIContent("Even number output", "Even numbers are shown in this TMP_Field."),
						_instance._data.evenText, typeof(TMP_Text), true) as TMP_Text;
				}

				GUIDrawCallback("onStart");
				GUIDrawCallback("onFinish");

				serializedObject.ApplyModifiedProperties();

				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
			
			/// <summary>
			/// Draws event property with given name in GUI.
			/// </summary>
			private void GUIDrawCallback(string propertyName) {
				var eventProp = _data.FindPropertyRelative(propertyName);
				EditorGUILayout.PropertyField(eventProp);
				serializedObject.ApplyModifiedProperties();
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

		public bool hasCountdown = false;
		public int speed;
		public TMP_Text evenText;
		public TMP_Text oddText;
		public CountDownAnimationProps animateProps;
	}

	[Flags]
	public enum CountDownAnimationProps
	{
		None = 0,
		Fade = 1<<0,
		ScaleUp = 1<<1,
	}
}
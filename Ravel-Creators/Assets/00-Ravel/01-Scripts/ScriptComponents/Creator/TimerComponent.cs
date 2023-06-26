using System;
using Base.Ravel.Creator.Components;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class TimerComponent : ComponentBase
{
	public override ComponentData Data { get; }
	[SerializeField, HideInInspector] private TimerData _data;
	protected override void BuildComponents() { }

	protected override void DisposeData() { }

	public void StartTimer() { }

	public void ResetTimer() { }

	public void StopTimer() { }

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
			_instance._data._startOnAwake = EditorGUILayout.Toggle("Start on awake", _instance._data._startOnAwake);
			
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
	public bool _startOnAwake;
	public UnityEvent onStart;
	public UnityEvent onFinish;
}

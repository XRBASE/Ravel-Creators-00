using System;
using System.Collections.Generic;
using Base.Ravel.TranslateAttributes;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Conditions
{
	public partial class ConditionComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeReference, HideInInspector] private ConditionComponentData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Manually checks the value of the condition and fires appropriate events when the result changes.
		/// </summary>
		public void CheckCondition() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(ConditionComponent))]
		private class ConditionComponentEditor : Editor
		{
			private ConditionComponent _instance;
			private SerializedProperty _data;

			public void OnEnable() {
				_instance = (ConditionComponent)target;

				if (_instance._data == null) {
					//set initial data class when it has not been set yet.
					_instance._data = new TransformComponentData();
				}

				_data = serializedObject.FindProperty("_data");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				EditorGUI.BeginChangeCheck();
				_instance._data.type = (ConditionType)EditorGUILayout.EnumPopup("Condition type", _instance._data.type);
				if (EditorGUI.EndChangeCheck()) {
					//swaps out the data type based on the condition type.
					switch (_instance._data.type) {
						case ConditionType.TransformCondition:
							_instance._data = new TransformComponentData(_instance._data);
							break;
						case ConditionType.MultiCondition:
							_instance._data = new MultiConditionComponentData(_instance._data);
							break;
					}

					_data = serializedObject.FindProperty("_data");

					EditorUtility.SetDirty(_instance);
					//return so the unity editor set's up the serialization for the new data field.
					return;
				}

				EditorGUI.BeginChangeCheck();
				_instance._data.DrawInspector(serializedObject, _data);
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
		}
#endif
	}

	public enum ConditionType
	{
		TransformCondition = 1,
		MultiCondition = 2,
	}

	/// <summary>
	/// Shared base for the component data classes.
	/// </summary>
	[Serializable]
	public class ConditionComponentData : ComponentData
	{
		public ConditionType type;
		public bool autoCheck;

		public UnityEvent<bool> onConditionChange;
		public UnityEvent onConditionSuccess;
		public UnityEvent onConditionFail;

		public ConditionComponentData() { }

		public ConditionComponentData(ConditionComponentData copy) {
			onConditionChange = copy.onConditionChange;

			onConditionSuccess = copy.onConditionSuccess;
			onConditionFail = copy.onConditionFail;
		}

		//inspector has been put into data class, as the data classes can be swapped out based on the condition type.
#if UNITY_EDITOR
		public virtual void DrawInspector(SerializedObject serializedObject, SerializedProperty dataProp) {
			autoCheck = EditorGUILayout.Toggle(
				new GUIContent("Auto check",
					"Should condition check its value automatically or is the check called manually"), autoCheck);

			GUIDrawCallback("onConditionChange", serializedObject, dataProp);

			GUIDrawCallback("onConditionSuccess", serializedObject, dataProp);
			GUIDrawCallback("onConditionFail", serializedObject, dataProp);
		}

		/// <summary>
		/// Draws event property with given name in GUI.
		/// </summary>
		private void GUIDrawCallback(string propertyName, SerializedObject serializedObject,
		                             SerializedProperty dataProp) {
			SerializedProperty evtProperty = dataProp.FindPropertyRelative(propertyName);
			EditorGUILayout.PropertyField(evtProperty);
			serializedObject.ApplyModifiedProperties();
		}

#endif
	}

	[Serializable]
	public class MultiConditionComponentData : ConditionComponentData
	{
		public List<ConditionComponent> conditions;

		public MultiConditionComponentData() {
			type = ConditionType.MultiCondition;
		}

		public MultiConditionComponentData(ConditionComponentData copy) : base(copy) {
			type = ConditionType.MultiCondition;
		}

#if UNITY_EDITOR
		public override void DrawInspector(SerializedObject serializedObject, SerializedProperty dataProp) {
			SerializedProperty conProperty = dataProp.FindPropertyRelative("conditions");
			EditorGUILayout.PropertyField(conProperty);
			serializedObject.ApplyModifiedProperties();

			base.DrawInspector(serializedObject, dataProp);
		}
#endif
	}

	[Serializable]
	public class TransformComponentData : ConditionComponentData
	{
		/// <summary>
		/// Is position altered by this script?
		/// </summary>
		private bool CheckPos {
			get { return attributes.HasFlag(TransformAttribute.Position); }
		}

		/// <summary>
		/// Is rotation altered by this script?
		/// </summary>
		private bool CheckRot {
			get { return attributes.HasFlag(TransformAttribute.Rotation); }
		}

		/// <summary>
		/// Is scale altered by this script?
		/// </summary>
		private bool CheckScale {
			get { return attributes.HasFlag(TransformAttribute.Scale); }
		}

		public Transform checkTransform;
		public TransformAttribute attributes;
		public TransformSpace space;

		public Vector3 requiredPosition;
		public Vector3 requiredRotation;
		public Vector3 requiredScale;

		public TransformComponentData() {
			type = ConditionType.TransformCondition;
		}

		public TransformComponentData(ConditionComponentData copy) : base(copy) {
			type = ConditionType.TransformCondition;
		}

#if UNITY_EDITOR
		public override void DrawInspector(SerializedObject serializedObject, SerializedProperty dataProp) {
			checkTransform =
				EditorGUILayout.ObjectField("Transform to check", checkTransform, typeof(Transform), true) as Transform;
			attributes = (TransformAttribute)EditorGUILayout.EnumFlagsField("Attributes", attributes);
			space = (TransformSpace)EditorGUILayout.EnumPopup("Space", space);

			if (CheckPos)
				requiredPosition = EditorGUILayout.Vector3Field("Required position", requiredPosition);
			if (CheckRot)
				requiredRotation = EditorGUILayout.Vector3Field("Required rotation", requiredRotation);
			if (CheckScale)
				requiredScale = EditorGUILayout.Vector3Field("Required scale", requiredScale);

			base.DrawInspector(serializedObject, dataProp);
		}
#endif
	}
}
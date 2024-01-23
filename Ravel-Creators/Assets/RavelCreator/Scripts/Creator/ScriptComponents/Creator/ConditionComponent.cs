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

		[SerializeReference] private ConditionComponentData _data;

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
				if (_instance._data.type !=  _instance._data.ContainerType) {
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
				
				DrawDefaultInspector();
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
	public abstract class ConditionComponentData : ComponentData
	{
		public abstract ConditionType ContainerType {
			get;
		}
		
		public ConditionType type;
		[Tooltip("Should condition be checked automatically or is it called externally by another component.")] 
		public bool autoCheck;

		[Tooltip("Fires when condition is met or unmet")]
		public UnityEvent<bool> onConditionChange;
		[Tooltip("Fires when condition is met")]
		public UnityEvent onConditionSuccess;
		[Tooltip("Fires when condition is not met")]
		public UnityEvent onConditionFail;

		public ConditionComponentData() { }

		public ConditionComponentData(ConditionComponentData copy) {
			onConditionChange = copy.onConditionChange;

			onConditionSuccess = copy.onConditionSuccess;
			onConditionFail = copy.onConditionFail;
		}
	}

	[Serializable]
	public class MultiConditionComponentData : ConditionComponentData
	{
		public override ConditionType ContainerType {
			get { return ConditionType.MultiCondition; }
		}
		
		public List<ConditionComponent> conditions;

		public MultiConditionComponentData() {
			type = ConditionType.MultiCondition;
		}

		public MultiConditionComponentData(ConditionComponentData copy) : base(copy) {
			type = ConditionType.MultiCondition;
		}
	}

	[Serializable]
	public class TransformComponentData : ConditionComponentData
	{
		public override ConditionType ContainerType {
			get { return ConditionType.TransformCondition; }
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
	}
}
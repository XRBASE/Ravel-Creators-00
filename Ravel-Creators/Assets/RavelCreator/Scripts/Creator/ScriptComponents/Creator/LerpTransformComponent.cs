using System;
using Base.Ravel.TranslateAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Moves/Rotates/Scales an set object by values as defined in the component.
	/// </summary>
	public partial class LerpTransformComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField] private LerpTransformData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Lerp transform data in direction as the set values.
		/// </summary>
		public void Transform() { }

		/// <summary>
		/// Lerp transform data in reversed direction as the set values.
		/// </summary>
		public void TransformReversed() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(LerpTransformComponent))]
		private class LerpTransformComponentEditor : Editor
		{
			public override void OnInspectorGUI() {
				LerpTransformComponent instance = (LerpTransformComponent)target;

				DrawDefaultInspector();
				
				if (instance._data.translationObject == null) {
					EditorGUILayout.HelpBox("No object set to change the transform of", MessageType.Error);
				}
				
				if (!instance._data.attributes.HasFlag(TransformAttribute.Position) && instance._data.positionChange.magnitude > 0) {
					instance._data.positionChange = Vector3.zero;
					EditorUtility.SetDirty(instance);
				}

				if (!instance._data.attributes.HasFlag(TransformAttribute.Rotation) && instance._data.rotationChange.magnitude > 0) {
					instance._data.rotationChange = Vector3.zero;
					EditorUtility.SetDirty(instance);
				}

				if (!instance._data.attributes.HasFlag(TransformAttribute.Scale) && instance._data.rotationChange.magnitude > 0) {
					instance._data.scaleChange = Vector3.zero;
					EditorUtility.SetDirty(instance);
				}
			}
		}
#endif
	}

	[Serializable]
	public class LerpTransformData : ComponentData
	{
		public Transform translationObject;
		public TransformAttribute attributes;
		public TransformSpace space;
		
		[Tooltip("Apply values as delta, instead of from to values")]
		public bool delta;

		public Vector3 positionChange;
		public Vector3 rotationChange;
		public Vector3 scaleChange;

		public float duration = 1f;
		public bool transformOnAwake;
	}
}
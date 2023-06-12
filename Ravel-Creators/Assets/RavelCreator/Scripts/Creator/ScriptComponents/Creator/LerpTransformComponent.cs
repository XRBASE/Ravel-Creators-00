using System;
using Base.Ravel.Creator.Components;
using Base.Ravel.TranslateAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Moves/Rotates/Scales an set object by values as defined in the component.
/// </summary>
public partial class LerpTransformComponent : ComponentBase
{
	public override ComponentData Data {
		get { return _data; }
	}
	[SerializeField, HideInInspector] private LerpTransformData _data;

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
			
			EditorGUI.BeginChangeCheck();
			instance._data.translationObject = EditorGUILayout.ObjectField( "Object to translate", instance._data.translationObject, typeof(Transform), true) as Transform;
			if (instance._data.translationObject == null) {
				EditorGUILayout.HelpBox("No object set to change the transform of", MessageType.Error);
			}
			
			instance._data.attributes = (TransformAttribute)EditorGUILayout.EnumFlagsField("Attributes", instance._data.attributes);
			instance._data.space = (TransformSpace)EditorGUILayout.EnumPopup("Space", instance._data.space);
			instance._data.delta = EditorGUILayout.Toggle("Apply delta position", instance._data.delta);
			
			EditorGUILayout.Space();
			string postfix = (instance._data.delta) ? " (direction)" : " (go to)";
			string tooltip = (instance._data.delta)
				? "Moves in direction with this amount of units per second."
				: "Moves towards this value, over the time set in duration.";

			if (instance._data.attributes.HasFlag(TransformAttribute.Position)) {
				instance._data.positionChange =
					EditorGUILayout.Vector3Field(new GUIContent("Position" + postfix, tooltip), instance._data.positionChange);
			} else if (instance._data.positionChange.magnitude > 0) {
				instance._data.positionChange = Vector3.zero;
			}

			if (instance._data.attributes.HasFlag(TransformAttribute.Rotation)) { 
				instance._data.rotationChange =
					EditorGUILayout.Vector3Field(new GUIContent("Rotation" + postfix, tooltip), instance._data.rotationChange);
			} else if (instance._data.rotationChange.magnitude > 0) {
				instance._data.rotationChange = Vector3.zero;
			}

			if (instance._data.attributes.HasFlag(TransformAttribute.Scale)) {
				instance._data.scaleChange =
					EditorGUILayout.Vector3Field(new GUIContent("Scale" + postfix, tooltip), instance._data.scaleChange);
			} else if (instance._data.rotationChange.magnitude > 0) {
				instance._data.scaleChange = Vector3.zero;
			}

			EditorGUILayout.Space();
			instance._data.duration = EditorGUILayout.FloatField(new GUIContent("Duration", "The duration it takes to complete the movement, in seconds"), instance._data.duration);
			instance._data.transformOnAwake = EditorGUILayout.Toggle(new GUIContent("Transform on awake", "Make changes happen directly after the GameObject is enabled."), instance._data.transformOnAwake);
			
			if (EditorGUI.EndChangeCheck()) {
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
	public bool delta;

	public Vector3 positionChange;
	public Vector3 rotationChange;
	public Vector3 scaleChange;

	public float duration = 1f;
	public bool transformOnAwake;
}

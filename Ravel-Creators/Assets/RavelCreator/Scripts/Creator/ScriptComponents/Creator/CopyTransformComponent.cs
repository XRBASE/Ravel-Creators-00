using System;
using Base.Ravel.Creator.Components;
using Base.Ravel.TranslateAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Copies the values of one objects transform onto another object, based on component input.
/// </summary>
public partial class CopyTransformComponent : ComponentBase
{
	public override ComponentData Data {
		get { return _data; }
	}
	[SerializeField, HideInInspector] private CopyTransformData _data;

	protected override void BuildComponents() { }

	protected override void DisposeData() { }

	public void CopyValues() { }

	public void SetCopyTransform(Transform transform) { }

	public void SetActive(bool active) { }

#if UNITY_EDITOR
	[CustomEditor(typeof(CopyTransformComponent))]
	private class CopyTransformComponentEditor : Editor
	{
		//order of these attributes is dependent on the CopyAtttributes enum.
		private readonly string[] ATTRIBUTE_NAMES = new [] {"position", "rotation", "scale"};
		private readonly string[] AXIS_NAMES = new [] {"x", "y", "z"};
		
		public override void OnInspectorGUI() {
			CopyTransformComponent instance = (CopyTransformComponent)target;

			bool dirty = false;
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			EditorGUI.BeginChangeCheck();
			instance._data.active = EditorGUILayout.Toggle(
				new GUIContent("Active", "Active components will automatically copy the values in the update loop"),
				instance._data.active);
			
			instance._data.attributes = (TransformAttribute)EditorGUILayout.EnumFlagsField(
				new GUIContent("Attributes", "Attributes to copy"),
				instance._data.attributes);
			
			instance._data.copySpace = (TransformSpace)EditorGUILayout.EnumPopup(
				new GUIContent("Copy space", "Copy world or local values"),
				instance._data.copySpace);
			instance._data.applySpace = (TransformSpace)EditorGUILayout.EnumPopup(
				new GUIContent("Apply space", "Apply copied values to world or local"),
				instance._data.applySpace);

			instance._data.copyPlayer = EditorGUILayout.Toggle(
				new GUIContent("Copy player", "Copy values of the local players transform"),
				instance._data.copyPlayer);

			if (EditorGUI.EndChangeCheck()) {
				if (instance._data.copyPlayer && instance._data.toCopy != null) {
					instance._data.toCopy = null;
					dirty = true;
				}
			}

			if (!dirty) {
				EditorGUI.BeginChangeCheck();
			}
			if (!instance._data.copyPlayer) {
				instance._data.toCopy = EditorGUILayout.ObjectField(
					new GUIContent("Copy transform", "Transform of which the values will be copied"),
					instance._data.toCopy, typeof(Transform), true) as Transform;
			}
            
			GUILayout.Label("Axes to copy:");
			for (int i = 0; i < ATTRIBUTE_NAMES.Length; i++) {
				//checks index of name against the position in the CopyAtttributes enum (order should match).
				if (instance._data.attributes.HasFlag((TransformAttribute) (1 << i))) {
					GUILayout.Label(ATTRIBUTE_NAMES[i]);
					GUILayout.BeginHorizontal();
					EditorGUI.indentLevel++;
					for (int j = 0; j < AXIS_NAMES.Length; j++) {
						instance._data.axes[i * ATTRIBUTE_NAMES.Length + j] =
							EditorGUILayout.Toggle(instance._data.axes[i * ATTRIBUTE_NAMES.Length + j], 
								GUILayout.Width(EditorGUIUtility.singleLineHeight * 2));
					}
					EditorGUI.indentLevel--;
					GUILayout.EndHorizontal();
				}
			}
            
			
			if (dirty || EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(instance);
			}
			EditorGUI.indentLevel = indent;
		}
	}
#endif
}

[Serializable]
public class CopyTransformData : ComponentData
{
	public TransformAttribute attributes;
    
	public TransformSpace copySpace;
	public TransformSpace applySpace;

	public bool copyPlayer;
    
	public Transform toCopy;
	public bool[] axes = new bool[9];

	public bool active = true;
}

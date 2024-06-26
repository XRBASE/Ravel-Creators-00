using System;
using Base.Ravel.Creator.Components;
using SpaceShift.CustomAttributes;
using UnityEditor;
using UnityEngine;

public partial class CustomAvatarComponent : ComponentBase
{
	public override ComponentData Data {
		get { return _data; }
	}
	[SerializeField] private CustomAvatarData _data;

	protected override void BuildComponents(){}

	protected override void DisposeData() { }

	public void EnableForLocalPlayer() { }

	public void DisableForLocalPlayer() { }

	[Serializable]
	private class CustomAvatarData : ComponentData {
		public GameObject avatar;
		public bool avatarVisible = true;
		[Tooltip("Gender of selected avatar, 0 = feminine, 1 = masculine"), Range(0,1)] 
		public float gender = 0.5f;
		[ReadOnly] public string guid;
	}
	
#if UNITY_EDITOR
	
	private void RegenerateGUID() {
		_data.guid = Guid.NewGuid().ToString();
		EditorUtility.SetDirty(this);
	}
	
	[CustomEditor(typeof(CustomAvatarComponent))]
	private class CustomAvatarComponentEditor : Editor {
		private CustomAvatarComponent _instance;

		private void OnEnable() {
			_instance = (CustomAvatarComponent)target;
			if (string.IsNullOrEmpty(_instance._data.guid)) {
				_instance.RegenerateGUID();
			}
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if (GUILayout.Button("Regenerate GUID")) {
				_instance.RegenerateGUID();
			}
		}
	}
#endif
}
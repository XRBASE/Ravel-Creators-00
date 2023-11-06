using System;
using UnityEngine;
using MathBuddy.Flags;
using Base.Ravel.FeatureFlags;
using Base.Ravel.Modules;
using Base.Ravel.Permissions;
using Base.Ravel.Networking.Authorities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Roles
{
	public partial class VisibleWithComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}
		[SerializeField, HideInInspector] private VisibleWithData _data;

		protected override void DisposeData() { }

		protected override void BuildComponents() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(VisibleWithComponent))]
		private class VisibleWithComponentEditor : Editor
		{
			private VisibleWithComponent _instance;

			public override void OnInspectorGUI() {
				_instance = (VisibleWithComponent)target;
				DrawDefaultInspector();
				
				EditorGUI.BeginChangeCheck();

				_instance._data.type = (VisibleWithData.VisWithType) EditorGUILayout.EnumPopup(
					new GUIContent("Type of check", "What type of permission is looked for in the check"),
					_instance._data.type);
				
				GUIDrawRequiredType();
				
				_instance._data.checkFor = (FlagExtensions.FlagPositive) EditorGUILayout.EnumPopup(
					new GUIContent("Check for", "Look for permission, or lack thereof?"),
					_instance._data.checkFor);
				_instance._data.checkType = (FlagExtensions.CheckType) EditorGUILayout.EnumPopup(
					new GUIContent("Check type", "Should the permissions all be included, or any of them?"),
					_instance._data.checkType);

				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}

			/// <summary>
			/// Draw's the field of the required enum for the type that has been selected.
			/// </summary>
			private void GUIDrawRequiredType() {
				switch (_instance._data.type) {
					case VisibleWithData.VisWithType.Auth:
						ClampRequired<SystemAuthorities>();
						_instance._data.required = (int)(SystemAuthorities)EditorGUILayout.EnumFlagsField(
							new GUIContent("Required (system) authorities", "What type of authority is required for visibility."),
							(SystemAuthorities)_instance._data.required);
						break;
					case VisibleWithData.VisWithType.Permission:
						ClampRequired<Permission>();
						_instance._data.required = (int)(Permission)EditorGUILayout.EnumFlagsField(
							new GUIContent("Required permission", "What type of permisison(s) are required for visibility."),
							(Permission)_instance._data.required);
						break;
					case VisibleWithData.VisWithType.FeatureFlag:
						ClampRequired<FlagType>();
						_instance._data.required = (int)(FlagType)EditorGUILayout.EnumFlagsField(
							new GUIContent("Required feature flag", "What type of feature flag(s) are required for visibility."),
							(FlagType)_instance._data.required);
						break;
					case VisibleWithData.VisWithType.Module:
						ClampRequired<ModuleFlags>();
						_instance._data.required = (int)(ModuleFlags)EditorGUILayout.EnumFlagsField(
							new GUIContent("Required feature flag", "What type of feature flag(s) are required for visibility."),
							(ModuleFlags)_instance._data.required);
						break;
				}
			}

			/// <summary>
			/// Sets required value to 0 if it is higher than possible.
			/// </summary>
			/// <typeparam name="T">Flags enum for which the value needs to be clamped.</typeparam>
			private void ClampRequired<T>() where T: Enum {
				int max = Enum.GetValues(typeof(T)).Length;
				//Retrieves the number above the highest flag number, so we can check if the flag can be set.
				max = 1 << (max - 1);
				
				if (_instance._data.required > max) {
					_instance._data.required = 0;
				}
			}
		}
#endif
	}

	[Serializable]
	public class VisibleWithData : ComponentData
	{
		public VisWithType type;
		
		public int required;
		public FlagExtensions.FlagPositive checkFor;
		public FlagExtensions.CheckType checkType;
		
		public enum VisWithType
		{
			Permission = 0,
			Auth = 1,
			FeatureFlag = 2,
			Module = 3
		}
	}
}
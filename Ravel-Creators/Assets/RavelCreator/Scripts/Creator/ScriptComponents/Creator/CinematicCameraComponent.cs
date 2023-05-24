using System;
using Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Used to control the camera into a specific cinematic viewpoint.
	/// </summary>
	[AddComponentMenu("Ravel/Cinematic Camera")]
	public partial class CinematicCameraComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}
		[SerializeField, HideInInspector] private CinematicCamData _data;
		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Add and activate camera behaviour.
		/// </summary>
		public void ActivateCamera() { }

		/// <summary>
		/// Deactivate and remove camera behaviour.
		/// </summary>
		public void DeactivateCamera() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(CinematicCameraComponent))]
		private class CinematicCameraComponentEditor : Editor
		{
			public override void OnInspectorGUI() {
				CinematicCameraComponent instance = (CinematicCameraComponent)target;
				DrawDefaultInspector();
				
				EditorGUI.BeginChangeCheck();
				//virtual cam + button to create one a child
				instance._data.virtualCam = EditorGUILayout.ObjectField("Virtual camera", instance._data.virtualCam,
					typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

				if (instance._data.virtualCam == null) {
					EditorGUILayout.HelpBox("Select a virtual camera first", MessageType.Error);
					if (GUILayout.Button("Create")) {
						GameObject vc = new GameObject("virtual camera");
						vc.transform.SetParent(instance.transform);
						vc.transform.localPosition = Vector3.zero;
						vc.transform.localRotation = Quaternion.identity;
						instance._data.virtualCam = vc.AddComponent<CinemachineVirtualCamera>();
						EditorUtility.SetDirty(instance);
					}
					else {
						return;
					}
				}
				
				//can viewpoint be escaped through escape key?
				instance._data.canOverride =
					EditorGUILayout.Toggle("Can player exit view", instance._data.canOverride);

				//Transition and blend time
				instance._data.overrideBlendTime =
					EditorGUILayout.Toggle("Override blend/transition duration", instance._data.overrideBlendTime);
				if (instance._data.overrideBlendTime) {
					instance._data.blendTime = Mathf.Max(0, EditorGUILayout.FloatField("Blend/Transition duration", instance._data.blendTime));
				} else if (instance._data.blendTime >= 0f) {
					instance._data.blendTime = -1f;
					EditorUtility.SetDirty(instance);
				}
				
				//follow transform handling
				instance._data.followPlayer =
					EditorGUILayout.Toggle("Follow player transform", instance._data.followPlayer);
				instance._data.followType =
					(CinematicCamData.FollowType)EditorGUILayout.EnumPopup("Follow behaviour", instance._data.followType);
				EditorGUILayout.LabelField(CinematicCamData.FOLLOW_TYPE_DESCRIPTIONS[(int)instance._data.followType]);
				
				//Additional data based on follow type.
				if (!instance._data.followPlayer && instance._data.followType != CinematicCamData.FollowType.None) {
					//not following player, so this transform is followed.
					instance._data.followTarget = EditorGUILayout.ObjectField("Follow target", instance._data.followTarget,
						typeof(Transform), true) as Transform;
					if (instance._data.followTarget == null) {
						EditorGUILayout.HelpBox("Follow behaviour without target", MessageType.Warning);
					}
				} else if (instance._data.followTarget != null) {
					//No need for a follow target, clear the target and set dirty.
					instance._data.followTarget = null;
					EditorUtility.SetDirty(instance);
				}
				
				if (instance._data.followType is CinematicCamData.FollowType.LookatDeltaTarget) {
					//Follow and look target required, so make a field for the look specific target.
					instance._data.lookTarget = EditorGUILayout.ObjectField("Look target", instance._data.lookTarget,
						typeof(Transform), true) as Transform;
					
					if (instance._data.lookTarget == null) {
						EditorGUILayout.HelpBox("No look target set", MessageType.Warning);
					}
				}
				
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(instance);
				}
			}
		}
#endif
	}

	[Serializable]
	public class CinematicCamData : ComponentData
	{
		public CinemachineVirtualCamera virtualCam;
		
		public bool canOverride;
		//ghost could be added, but should be tested further before adding it.

		public bool overrideBlendTime;
		public float blendTime;
		
		public FollowType followType;
		public bool followPlayer;
		public Transform followTarget;
		public Transform lookTarget;
		
		public enum FollowType
		{
			None = 0,
			Offset = 1,
			PositionDeltaOnly = 2,
			Lookat = 3,
			LookatDeltaTarget = 4,

		}
		
		//for display in editor
		public static string[] FOLLOW_TYPE_DESCRIPTIONS = new[] {
			"Camera behaviour: Static camera position and rotation.",
			"Camera behaviour: Offset in position and rotation as it was when the view started.",
			"Camera behaviour: Only applies delta position changes to the camera.",
			"Camera behaviour: Static, but looking at the target.",
			"Camera behaviour: Same as Offset, but looking at lookFollow transform."
		};
	}
}

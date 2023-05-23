using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// This component creates a chair where the seat transform is the position that the player sits down and the collider
	/// is the interactable that the player needs to click on to sit down.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	[AddComponentMenu("Ravel/Seat")]
	public partial class SeatComponent : ComponentBase, INetworkId
	{
		public override ComponentData Data { get; }
		[SerializeField, HideInInspector] private SeatData _data;

		public bool Networked { get { return true; } }

		public int ID {
			get { return _data.id;}
			set { _data.id = value; }
		}

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(SeatComponent))]
		private class SeatComponentEditor : Editor
		{
			private SeatComponent _instance;

			public void OnEnable() {
				_instance = (SeatComponent)target;
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();
				
				EditorGUI.BeginChangeCheck();
				_instance._data.seat = EditorGUILayout.ObjectField("Seat", _instance._data.seat, typeof(Transform), true) as Transform;
				
				bool found = false;
				float distance = Mathf.Infinity;
				string name = "";
				if (_instance._data.seat != null) {
					//searches for collider that is neither chair nor seat transform, to determine how high the seat is. 
					RaycastHit[] hits = Physics.RaycastAll(_instance._data.seat.position, Vector3.down, 10);
					
					if (hits.Length > 0) {
						for (int i = 0; i < hits.Length; i++) {
							if (hits[i].transform != _instance._data.seat && hits[i].transform != _instance.gameObject.transform) {
								distance = hits[i].distance;
								found = true;
								name = hits[i].transform.gameObject.name;
								break;
							}
						}
					}
				}
				
				if (!found) {
					GUILayout.Label("No floor found");
				}
				else {
					//offers option to move the seat (child) or parent object to adjust the seat height to the correct height.
					GUILayout.Label($"Floor ({name}) found at distance {distance}");
					if (Mathf.Abs(distance - SeatData.PERFECT_SEAT_HEIGHT) > MathBuddy.FloatingPoints.LABDA) {
						EditorGUILayout.HelpBox($"The perfect height between the chair and the floor is {SeatData.PERFECT_SEAT_HEIGHT}. Do you want to set this height?",
							MessageType.Warning);
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button($"Fix parent ({_instance.gameObject.name})")) {
							_instance.transform.position += Vector3.down * (distance - SeatData.PERFECT_SEAT_HEIGHT);
						}
						if (GUILayout.Button($"Fix child ({_instance._data.seat.gameObject.name})")) {
							_instance._data.seat.position += Vector3.down * (distance - SeatData.PERFECT_SEAT_HEIGHT);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}
		}
#endif
	}

	[Serializable]
	public class SeatData : ComponentData
	{
		public const float PERFECT_SEAT_HEIGHT = 0.6f;
		
		public int id;
		public Transform seat;
	}
}
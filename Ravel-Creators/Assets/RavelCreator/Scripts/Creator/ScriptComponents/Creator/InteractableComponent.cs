using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Skeleton part of the class, contains code and methods for creators, but no implementation
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public partial class InteractableComponent : ComponentBase, INetworkId
	{
		public bool Networked { 
			get { return _data.networked;}
			set { _data.networked = value; } 
		}
		
		public int ID {
			get { return _data.id;}
			set { _data.id = value; }
		}
		
		public override ComponentData Data {
			get { return _data; }
		}
		
		[SerializeField, HideInInspector] private InteractableData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		/// <summary>
		/// Triggers the interactable (as if it was triggered naturally).
		/// </summary>
		public void Activate() { }

		/// <summary>
		/// Set's component of switch type to enabled
		/// </summary>
		public void EnableSwitch() { }

		/// <summary>
		/// Set's component of switch type to disabled.
		/// </summary>
		public void DisableSwitch() { }
		
		
#if UNITY_EDITOR
		[CustomEditor(typeof(InteractableComponent))]
		private class InteractableComponentEditor : Editor
		{
			private InteractableComponent _instance;
			private SerializedProperty _data;
			private SerializedProperty _evtProperty;

			private void OnEnable() {
				_instance = (InteractableComponent)target;
				_data = serializedObject.FindProperty("_data");
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();
				EditorGUI.BeginChangeCheck();
				//Determine type of interaction
				_instance._data.type = (InteractableData.Type)EditorGUILayout.EnumPopup("Interaction type", _instance._data.type);
				
				//Should trigger be networked. 
				_instance._data.networked = EditorGUILayout.Toggle("Networked interaction", _instance._data.networked);
				
				EditorGUILayout.Space();
				//option for delaying reaction.
				_instance._data.delayed = EditorGUILayout.Toggle("Delayed response", _instance._data.delayed);
				if (_instance._data.delayed) {
					_instance._data.delayTime = EditorGUILayout.FloatField("Duration", _instance._data.delayTime);
				}

				EditorGUILayout.Space();
				//hover activation and callbacks.
				_instance._data.hasHover = EditorGUILayout.Toggle("Has hover interactions", _instance._data.hasHover);
				if (_instance._data.hasHover) {
					GUIDrawCallback("onHoverEnter");
					
					GUIDrawCallback("onHoverExit");
				}

				//Specific callbacks for the type of interaction.
				switch (_instance._data.type) {
					case InteractableData.Type.Click:
						GUIDrawCallback("onClick");
						break;
					case InteractableData.Type.Trigger:
						GUIDrawCallback("onEnter");
						
						GUIDrawCallback("onExit");
						break;
					case InteractableData.Type.Switch:
						GUIDrawCallback("onSwitchOn");

						GUIDrawCallback("onSwitchOff");
						break;
				}

				ClearUnusedCallbacks();
				if (EditorGUI.EndChangeCheck()) {
					EditorUtility.SetDirty(_instance);
				}
			}

			/// <summary>
			/// Draws event property with given name in GUI.
			/// </summary>
			private void GUIDrawCallback(string propertyName) {
				_evtProperty = _data.FindPropertyRelative(propertyName);
				EditorGUILayout.PropertyField(_evtProperty);
				serializedObject.ApplyModifiedProperties();
			}

			/// <summary>
			/// Removes any callback data that is not connected to the selected type of interaction.
			/// </summary>
			private void ClearUnusedCallbacks() {
				if (!_instance._data.hasHover) {
					_instance._data.onHoverEnter = null;
					_instance._data.onHoverExit = null;
				}
				
				switch (_instance._data.type) {
					case InteractableData.Type.Click:
						_instance._data.onEnter = null;
						_instance._data.onExit = null;
						
						_instance._data.onSwitchOn = null;
						_instance._data.onSwitchOff = null;
						break;
					case InteractableData.Type.Trigger:
						_instance._data.onClick = null;
						
						_instance._data.onSwitchOn = null;
						_instance._data.onSwitchOff = null;
						break;
					case InteractableData.Type.Switch:
						_instance._data.onClick = null;
						
						_instance._data.onEnter = null;
						_instance._data.onExit = null;
						break;
				}
			}
		}
#endif
	}
	
	[Serializable]
	public class InteractableData : ComponentData
	{
		//generic data
		public Type type = Type.Click;
		public bool hasHover = false;

		[Tooltip("Delay is not added to the hover callbacks!")]
		public bool delayed = false;
		public float delayTime = 0f;

		public UnityEvent onHoverEnter;
		public UnityEvent onHoverExit;

		//click data
		public UnityEvent onClick;

		//Trigger data
		public UnityEvent onEnter;
		public UnityEvent onExit;

		//switch data
		public UnityEvent onSwitchOn;
		public UnityEvent onSwitchOff;

		//network data
		public bool networked = false;
		[HideInInspector] public int id = -1;

		public enum Type
		{
			Click,
			Trigger,
			Switch,
		}
	}
}
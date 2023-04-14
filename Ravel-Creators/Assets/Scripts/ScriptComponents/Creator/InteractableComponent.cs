using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Skeleton part of the class, contains code and methods for creators, but no implementation
	/// </summary>
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
		
		protected override ComponentData Data {
			get { return _data; }
		}
		
		[SerializeField] private InteractableData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public void Activate() { }
		public void EnableSwitch() { }
		public void DisableSwitch() { }
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

		//collider data
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
			Collider,
			Switch,
		}
	}
}
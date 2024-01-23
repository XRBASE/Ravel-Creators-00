using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	public abstract partial class BaseInteractableComponent : ComponentBase, IUniqueId
	{
		public bool SetUniqueID {
			get { return ((InteractableComponentData)Data).networked; }
		}

		public int ID {
			get { return ((InteractableComponentData)Data).id; } 
			set { ((InteractableComponentData)Data).id = value; }
		}

		public abstract override ComponentData Data { get; }

		public void Activate() { }
		public void SetInteractable(bool isInteractable) { }

		public virtual void SetDataFromInteractable(InteractableData data) {
			InteractableComponentData compData = (InteractableComponentData)Data;
			
			compData.hasHover = data.hasHover;
			compData.onHoverEnter = data.onHoverEnter;
			compData.onHoverExit = data.onHoverExit;

			compData.interactable = data.interactable;
			
			compData.networked = data.networked;
			compData.id = data.id;
			
			compData.delayed = data.delayed;
			if (compData.delayed) {
				compData.delay = data.delayTime;
			} 
			else {
				compData.delay = 0f;
			}
		}
	}
	
	[Serializable]
	public class InteractableComponentData : ComponentData
	{
		public bool hasHover;
		public UnityEvent onHoverEnter;
		public UnityEvent onHoverExit;

		public bool interactable;
		
		public bool networked;
		[HideInInspector] public int id;
		
		public bool delayed;
		public float delay;
	}
}
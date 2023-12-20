using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	[AddComponentMenu("Ravel/Interactables/LookInteractable")]
	public partial class LookInteractableComponent : BaseInteractableComponent
	{
		public override ComponentData Data {
			get { return _data; }
		}
		[SerializeField] private LookInteractableData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public override void SetDataFromInteractable(InteractableData data) {
			_data = new LookInteractableData();
			
			_data.onLook = data.onLook;
			_data.onLost = data.onLost;

			_data.threshold = data.threshold;
			
			base.SetDataFromInteractable(data);
		}
	}

	[Serializable]
	public class LookInteractableData : InteractableComponentData
	{
		public UnityEvent onLook;
		public UnityEvent onLost;

		public float threshold;
	}
}
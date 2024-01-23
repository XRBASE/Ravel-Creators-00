using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	[AddComponentMenu("Ravel/Interactables/ClickInteractable")]
	public partial class ClickInteractableComponent : BaseInteractableComponent
	{
		public override ComponentData Data {
			get { return _data; }
		}
		
		[SerializeField] private ClickInteractableData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public override void SetDataFromInteractable(InteractableData data) {
			_data = new ClickInteractableData();
			
			_data.onClick = data.onClick;
			
			base.SetDataFromInteractable(data);
		}
	}
	
	[Serializable]
	public class ClickInteractableData : InteractableComponentData
	{
		public UnityEvent onClick;
	}
}
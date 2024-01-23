using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	[AddComponentMenu("Ravel/Interactables/SwitchInteractable")]
	public partial class SwitchInteractableComponent : BaseInteractableComponent
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField] private SwitchInteractableData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public void DisableSwitch() { }
		public void EnableSwitch() { }

		public override void SetDataFromInteractable(InteractableData data) {
			_data = new SwitchInteractableData();
			
			_data.onSwitchEnabled = data.onSwitchOn;
			_data.onSwitchDisabled = data.onSwitchOff;
			
			base.SetDataFromInteractable(data);
		}
	}

	[Serializable]
	public class SwitchInteractableData : InteractableComponentData
	{
		public UnityEvent onSwitchEnabled;
		public UnityEvent onSwitchDisabled;
	}
}

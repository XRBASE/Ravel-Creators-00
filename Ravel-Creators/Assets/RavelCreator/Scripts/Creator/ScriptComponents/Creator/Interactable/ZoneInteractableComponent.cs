using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Ravel/Interactables/ZoneInteractable")]
    public partial class ZoneInteractableComponent : BaseInteractableComponent
    {
        public override ComponentData Data {
            get { return _data; }
        }

        [SerializeField] private ZoneInteractableData _data;

        protected override void BuildComponents() { }
        protected override void DisposeData() { }

        public override void SetDataFromInteractable(InteractableData data) {
            _data = new ZoneInteractableData();
            
            _data.onEnterZone = data.onEnter;
            _data.onExitZone = data.onExit;
            
            base.SetDataFromInteractable(data);
        }
    }

    [Serializable]
    public class ZoneInteractableData : InteractableComponentData
    {
        public UnityEvent onEnterZone;
        public UnityEvent onExitZone;
    }
}

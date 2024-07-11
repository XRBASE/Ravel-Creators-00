using Base.Ravel.Creator.Components;
using UnityEngine;

public partial class ObjIndicatorHandle : ComponentBase
{
    public override ComponentData Data { get { return _data;} }

    [SerializeField] private IndicatorData _data;

    protected override void BuildComponents() { }

    protected override void DisposeData() { }

    public void SetTarget(Transform target) { }

    public void ClearTarget() { }

    public class IndicatorData : ComponentData {
        public bool enabled = true;
        public Sprite icoOverride;
    }
}

using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Adding this component to a GameObject with a collider makes the player move along with this object while colliding with it
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public partial class ParentOnCollideComponent : ComponentBase
    {
        public override ComponentData Data { get; }

        protected override void BuildComponents()
        {
        }

        protected override void DisposeData()
        {
        }

    }
}
using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Adding this component to a GameObject with a collider makes the player move along with this object while colliding with it
    /// </summary>
    [AddComponentMenu("Ravel/Moving Platform")]
    public partial class MovingPlatformComponent : ComponentBase
    {
        public override ComponentData Data { get; }
        protected override void BuildComponents(){}

        protected override void DisposeData(){}

#if UNITY_EDITOR
        [CustomEditor(typeof(MovingPlatformComponent))]
        private class MovingPlatformComponentEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                GUILayout.Label(
                    "Adding this component to a GameObject with a collider makes\nthe player move along with this object while colliding with it");
                DrawDefaultInspector();
            }
#endif
        }
    }
}
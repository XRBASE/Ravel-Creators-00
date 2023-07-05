using System;
using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Adding this component to a GameObject with a collider makes the player move along with this object while colliding with it
    /// </summary>
    [AddComponentMenu("Ravel/MovingPlatform")]
    public partial class ParentOnCollideComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        [SerializeField, HideInInspector] private ParentOnCollideData _data;

        protected override void BuildComponents()
        {
        }

        protected override void DisposeData()
        {
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ParentOnCollideComponent))]
        private class ParentOnCollideComponentEditor : Editor
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

    [Serializable]
    public class ParentOnCollideData : ComponentData
    {
    }
}
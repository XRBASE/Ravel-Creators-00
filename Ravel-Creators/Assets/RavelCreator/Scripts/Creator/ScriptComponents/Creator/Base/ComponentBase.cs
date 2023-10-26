using System;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// This class is used as a base class for all creator components, it exists of three parts:
    /// - The skeleton, exists in the creator project, this class only contains the empty public (accessible) methods, so they can be called
    /// - The body, exists in the ravel project and fills up the methods with their implementations, so that after import the called methods execute their supposed behaviour
    /// - The data, exists in both versions and is used to serialize the data of the assetbundle, into the corresponding classes. 
    /// </summary>
    public abstract class ComponentBase : MonoBehaviour
    {
        public abstract ComponentData Data { get; }

        protected bool _componentsBuild = false; 

        protected void Awake() {
            BuildComponents();
        }

        /// <summary>
        /// Create the required components to implement the behaviour, and transfer data into the right classes.
        /// </summary>
        protected virtual void BuildComponents() {
            _componentsBuild = true;
        }
        
        /// <summary>
        /// Should be called after building the components has finished, to clear up the used memory
        /// </summary>
        protected abstract void DisposeData();
        
#if UNITY_EDITOR
        public virtual GizmoIconType Icon {
            get { return GizmoIconType.Default; }
        }
        
        /// <summary>
        /// Icons with the same name (in Assets/Gizmos folder will be loaded as the icon of this script).
        /// </summary>
        public enum GizmoIconType
        {
            Default = 0,
        }
#endif
    }

    /// <summary>
    /// Data container, used to transfer data from an assetbundle into the replacing scripts in the Ravel project.
    /// </summary>
    [Serializable]
    public abstract class ComponentData
    {
        
    }
}
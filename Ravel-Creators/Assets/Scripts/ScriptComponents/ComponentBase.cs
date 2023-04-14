using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
        protected abstract ComponentData Data { get; }

        protected void Awake() {
            BuildComponents();
        }

        /// <summary>
        /// Create the required components to implement the behaviour, and transfer data into the right classes.
        /// </summary>
        protected abstract void BuildComponents();
        
        /// <summary>
        /// Should be called after building the components has finished, to clear up the used memory
        /// </summary>
        protected abstract void DisposeData();
    }

    /// <summary>
    /// Data container, used to transfer data from an assetbundle into the replacing scripts in the Ravel project.
    /// </summary>
    [Serializable]
    public abstract class ComponentData
    {
        
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components.Quiz
{
    /// <summary>
    /// This abstract parent defines the base for selector visual classes. In it the types van be attached to a selector like a
    /// Toggle or Trigger with which the answer can be selected and a visual that shows the data that matches the answer. 
    /// </summary>
    public abstract partial class MultiChoiceAnswer : MonoBehaviour
    {
        public UnityEvent<int> onItemSelected;
        public UnityEvent<int> onItemDeselect;
        
        /// <summary>
        /// Set unselect based on network data call. This changes the toggle, only if it has not yet been changed.
        /// </summary>
        public abstract void Unselect();
        
        /// <summary>
        /// Set select based on network data call. This changes the toggle, only if it has not yet been changed.
        /// </summary>
        public abstract void Select();
    }
}
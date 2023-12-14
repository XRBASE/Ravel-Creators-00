using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Ravel.Creator.Components.Quiz
{
    /// <summary>
    /// This class contains answers of string type and is selected or deselected using a toggle. The toggle itself is coupled
    /// to a toggle group in the template instance, so that it ensures only one answer is selected at a time. If this is not done,
    /// the last filled in answer will count.
    /// </summary>
    public partial class StringToggleAnswer : MultiChoiceAnswer
    {
        [SerializeField,
         Tooltip("Toggle for selection behaviour. Connect to a toggle group component, so only one can be selected.")]
        private Toggle _toggle;

        [SerializeField, Tooltip("In this field the answer text will be shown.")]
        private TMP_Text _field;
        
        /// <summary>
        /// Unselects this item.
        /// </summary>
        public override void Unselect() { }

        /// <summary>
        /// Selects this item
        /// </summary>
        public override void Select() { }
    }
}
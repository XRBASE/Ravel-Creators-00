using UnityEngine;
using UnityEngine.UI;

namespace Base.Ravel.Creator.Components.Quiz
{
    /// <summary>
    /// This class uses a sprite as data and a toggle as selector. The toggle should be connected to a togglegroup to ensure
    /// No other answers can be selected at the same time. 
    /// </summary>
    public partial class SpriteToggleAnswer : MultiChoiceAnswer
    {
        [SerializeField,
         Tooltip("Toggle for selection behaviour. Connect to a toggle group component, so only one can be selected.")]
        private Toggle _toggle;

        [SerializeField, Tooltip("In this graphic the answer sprite will be shown.")]
        private Image _img;

        public override void Unselect() { }

        public override void Select() { }
    }
}
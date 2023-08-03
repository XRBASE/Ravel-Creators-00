using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Base.Ravel.Creator.Components.Quiz
{
    public partial class QuizVisuals : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// Provides access to the TMP field of the question, so that it can be set by the editor
        /// (Its value is set to longest question, so that it is harder to make the box too small).
        /// </summary>
        public string QuestionTxt {
            set { _questionField.text = value; }
        }
#endif
        
        /// <summary>
        /// This list can be set in the inspector and it should contain all types of answer that this box will spawn. For now
        /// (see class comment) this is only one type, but in the case of a quiz that contains multiple types, all of those
        /// templates will be included in this list.
        /// </summary>
        [SerializeField, Tooltip("in scene samples (per used type) of templates.")]
        private List<MultiChoiceAnswer> _typeTemplates = new List<MultiChoiceAnswer>();

        [SerializeField] private TMP_Text _questionField;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Animation _wrongAnswerAnimation;

        /// <summary>
        /// Called as result of network data, this selects the corresponding answer visual.
        /// </summary>
        public void SelectMultiAnswer(int answerId) { }

        /// <summary>
        /// Called in the case of a reset, this unselects the answer, so that the previous answers are cleared when the quiz is reset.
        /// </summary>
        public void UnselectMultiAnswer(int answerId) { }
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Base.Ravel.Creator.Components.Assessments {
    public partial class AssessmentUIBoxComponent : ComponentBase {
        public override ComponentData Data {
            get { return _data; }
        }

        [SerializeField] private AssessmentUIBoxData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        /// <summary>
        /// Submit the given answer
        /// </summary>
        public void Submit() { }
    }

    [Serializable]
    public class AssessmentUIBoxData : ComponentData {
        [Tooltip("Unique index used to assign questions to this container")]
        public int index;
        
        [Tooltip("Question is shown in this field")]
        public TMP_Text questionField;
        [Tooltip("This button is used to submit the answer with")]
        public Button submitBtn;
        [Tooltip("This is the template for all answer toggles.")]
        public AssessmentUIAnswerComponent answerTemplate;
        [Tooltip("Answers to open questions are filled in in this field.")]
        public TMP_InputField answerInput;
        [Tooltip("Should the box continue to the next question if a wrong answer has been given.")] 
        public bool acceptWrongAnswers;
        
        public UnityEvent onComplete;
        public UnityEvent onReset;
    }
}
using System;
using Base.Ravel.CustomAttributes;
using MathBuddy.Strings;
using UnityEngine;

namespace Base.Ravel.Creator.Components.Quiz
{
    [Serializable]
    public abstract class Question
    {
        /// <summary>
        /// Returns true only for the abstract base type of this question, used in editor classes to find useless references.
        /// </summary>
        public virtual bool Abstract {
            get { return true; }
        }

        /// <summary>
        /// The actual question as string.
        /// </summary>
        public string QuestionTxt {
            get { return _question; }
        }

        /// <summary>
        /// Is it an open or a multichoice question 
        /// </summary>
        public bool IsOpen {
            get { return _isOpen; }
        }

        [SerializeField] private string _question = "";

        protected bool _isOpen = false;

#if UNITY_EDITOR
        /// <summary>
        /// Called to perform checks in questions themselves and highlight certain data in the editor.
        /// </summary>
        public virtual void OnEditorChanges() { }
#endif
    }

    [Serializable]
    public class OpenQuestion : Question
    {
        public override bool Abstract {
            get { return false; }
        }
        
        public string CorrectAnswer {
            get { return _correctAnswer; }
        }

        public bool CountCapitals {
            get { return _countCapitals; }
        }
        
        public bool CountWhiteSpace {
            get { return _countWhitespace; }
        }

        [SerializeField, Tooltip("What answer should be given by the player in order to pass the question?")] 
        private string _correctAnswer;
        [SerializeField, Tooltip("Should capitalisation mismatches be counted as a wrong answer?")] 
        private bool _countCapitals = true;
        [SerializeField, Tooltip("Should whitespaces mismatches (enter and spacebar) be counted as wrong answers?")] 
        private bool _countWhitespace = true;

        public OpenQuestion()
        {
            _isOpen = true;
        }

#if UNITY_EDITOR
        private bool _prevCapitals = true;
        
        public override void OnEditorChanges()
        {
            if (string.IsNullOrEmpty(_correctAnswer))
                return;

            if (_prevCapitals != _countCapitals) {
                //showcase captial and whitespace replacement in editor, to highlight the effects of the boolean values.
                if (!_countCapitals) {
                    _correctAnswer = _correctAnswer.ToUpper(); 
                }

                if (!_countWhitespace) {
                    _correctAnswer = StringExtentions.RemoveWhitespace(_correctAnswer);
                }
                _prevCapitals = _countCapitals;
            }
        }
#endif
    }

    [Serializable]
    public class MultiChoiceQuestion : Question
    {
        public override bool Abstract {
            get { return false; }
        }
        
        public AnswerSet Answers {
            get { return _answers; }
        }

        /// <summary>
        /// Id of correct answer in answerset
        /// </summary>
        public int CorrectAnswer {
            get { return _correctAnswerId; }
        }
        
        public AnswerSet.Type Type {
            get { return _answerType; }
        }

        //used to highlight what type of answers are set in this question
        [SerializeField, ReadOnly] 
        private AnswerSet.Type _answerType;
        
        [SerializeReference] private AnswerSet _answers;
        [SerializeField] private int _correctAnswerId;

        public MultiChoiceQuestion(AnswerSet.Type type)
        {
            _isOpen = false;
            _answers = AnswerSet.GetSetOfType(type);
            _answerType = type;
        }
        
#if UNITY_EDITOR
        //used to show a textual version of the correct answer, so it's more obvious what answer has been picked by the user.
        [SerializeField, ReadOnly] protected string _answerString;
        //prevent dubble logging of errors, nad just log once.
        private bool _logged = false;

        public override void OnEditorChanges()
        {
            if (_answers != null && _answers.Length > 0) {
                if (_correctAnswerId < _answers.Length) {
                    _answerString = _answers[_correctAnswerId].ToString();
                    _logged = false;
                } else if (_correctAnswerId != 0) {
                    _answerString = $"ERROR! No answer with if {_correctAnswerId} exists!";
                    if (!_logged) {
                        Debug.LogError(_answerString);
                        _logged = true;
                    }
                }
            } else {
                _answerString = $"WARNING! No answers set!";
                if (!_logged) {
                    Debug.LogWarning(_answerString);
                    _logged = true;
                }
            }
        }
#endif
    }
}
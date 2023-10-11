using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizSampleFloor
{
    public class FloorQuiz : MonoBehaviour
    {
        private Question CurQuestion {
            get { return _questions[_questionId]; }
        }

        [SerializeField] private FloorAnswer _answerTemplate;
        [SerializeField] private TMP_Text _questionField;
        [SerializeField] private Button _submitBtn;
        
        [SerializeField] private List<Question> _questions;
        private int _questionId = 0;
        private State _state = State.ShowQuestion;
        
        private List<FloorAnswer> _answerObjects;

        private void Start() {
            _answerObjects = new List<FloorAnswer>();
            
            _submitBtn.onClick.AddListener(OnSubmit);

            SetQuestion();
            
            //set button interactable state
            OnAnwerVoteChanged(false);
        }

        private void SetQuestion() {
            for (int i = 0; i < _answerObjects.Count; i++) {
                _answerObjects[i].gameObject.SetActive(false);
            }
            
            _questionField.text = CurQuestion.question;

            if (!_answerObjects.Contains(_answerTemplate)) {
                _answerObjects.Add(_answerTemplate);
                _answerTemplate.onVoteChanged = OnAnwerVoteChanged;
            }

            for (int i = 0; i < Mathf.Max(CurQuestion.answers.Count, _answerObjects.Count); i++) {
                if (i < CurQuestion.answers.Count) {
                    if (i >= _answerObjects.Count) {
                        FloorAnswer fa = Instantiate(_answerTemplate, _answerTemplate.transform.parent);
                        fa.onVoteChanged = OnAnwerVoteChanged;
                        
                        _answerObjects.Add(fa);
                    }

                    _answerObjects[i].Answer = CurQuestion.answers[i];
                    _answerObjects[i].gameObject.SetActive(true);
                } else {
                    _answerObjects[i].Reset();
                }
            }
        }

        private void OnAnwerVoteChanged(bool hasVote) {
            if (hasVote || _state == State.ShowAnswer) {
                _submitBtn.interactable = true;
            }
            else {
                for (int i = 0; i < _answerObjects.Count; i++) {
                    if (_answerObjects[i].gameObject.activeSelf && _answerObjects[i].HasVote) {
                        _submitBtn.interactable = true;
                        return;
                    }
                }
                _submitBtn.interactable = false;
            }
        }

        public void OnSubmit() {
            if (_state == State.ShowQuestion) {
                for (int i = 0; i < _answerObjects.Count; i++) {
                    _answerObjects[i].ShowResult(i == CurQuestion.correctAnswer);
                }
                
                _state = State.ShowAnswer;
            }
            else {
                for (int i = 0; i < _answerObjects.Count; i++) {
                    _answerObjects[i].Reset();
                }
                
                _questionId = (_questions.Count + (_questionId + 1)) % _questions.Count;
                SetQuestion();
                
                _state = State.ShowQuestion;
                
                //update button interactable
                OnAnwerVoteChanged(false);
            }
        }

        public void OnReset() {
            for (int i = 0; i < _answerObjects.Count; i++) {
                _answerObjects[i].Reset();
            }

            _state = State.ShowQuestion;
        }

        private enum State
        {
            ShowQuestion = 0,
            ShowAnswer = 1
        }
    }
    
    [Serializable]
    class Question
    {
        public string question;
        public List<string> answers;
        
        public int correctAnswer;
    }
}
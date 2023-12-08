using System;
using System.Collections.Generic;
using Base.Ravel.Creator.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuizSampleFloor
{
    public class FloorQuiz : MonoBehaviour, IUniqueId
    {
        [SerializeField] private FloorAnswer _answerTemplate;
        [SerializeField] private TMP_Text _questionField;
        [SerializeField] private Button _submitBtn;

        [SerializeField] private List<Question> _questions;
        [SerializeField] private bool _networked = false;
        
        [SerializeField] private UnityEvent onQuizFinished;
        [SerializeField] private UnityEvent onQuizReset;
        
        [HideInInspector, SerializeField] private int _id;
        
        public bool SetUniqueID {
            get { return true; }
        }
        public int ID {
            get { return _id;}
            set { _id = value; }
        }
        
        public void OnReset() { }

        private void SetQuestion() { }

        private void SetQuizFinished() { }

        private void OnAnswerVoteChanged(bool hasVote) { }

        public void OnSubmit() { }

        private void ShowAnswers() { }

        private void ShowQuestion() { }
    }
    
    [Serializable]
    class Question
    {
        public string question;
        public List<string> answers;
        
        public int correctAnswer;
    }
}
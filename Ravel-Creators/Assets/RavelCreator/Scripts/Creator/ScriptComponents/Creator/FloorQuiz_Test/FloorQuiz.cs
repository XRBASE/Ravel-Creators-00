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
        private const string SHOW_QUESTION_LABEL = "Submit";
        private const string SHOW_ANSWER_LABEL = "Next";
        
        public bool SetUniqueID {
            get { return true; }
        }
        public int ID {
            get { return _id;}
            set { _id = value; }
        }

        [SerializeField] private FloorAnswer _answerTemplate;
        [SerializeField] private TMP_Text _questionField;
        [SerializeField] private Button _submitBtn;

        [SerializeField] private List<Question> _questions;
        [SerializeField] private bool _networked = false;

        [SerializeField, Tooltip("Should following events be fired when room is joined, or only when state is reached while the player is there.")]
        private bool _fireOnRoomJoin = false;
        
        [SerializeField] private UnityEvent onQuizFinished;
        [SerializeField] private UnityEvent onQuizReset;
        
        [HideInInspector, SerializeField] private int _id;
        
        public void OnReset() {}
        
        public void OnSubmit() {}
    }
    
    [Serializable]
    class Question
    {
        public string question;
        public List<string> answers;
        
        public int correctAnswer;
    }
}
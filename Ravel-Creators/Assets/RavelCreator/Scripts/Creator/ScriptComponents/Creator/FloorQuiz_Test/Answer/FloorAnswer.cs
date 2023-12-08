using System;
using TMPro;
using UnityEngine;

namespace QuizSampleFloor
{
    [RequireComponent(typeof(Collider))]
    public class FloorAnswer : MonoBehaviour
    {
        public string Answer {
            get { return _answerField.text; }
            set { _answerField.text = value; }
        }

        public bool HasVote { get; set; }

        public Action<bool> onVoteChanged;
        
        [SerializeField] private Color _highlight = new(1, 1, 0);
        [SerializeField] private Color _correct = new(0, 1, 0);
        [SerializeField] private Color _false = new(1, 0, 0);

        [SerializeField] private TMP_Text _answerField;
        [SerializeField] private Material _sourceMaterial;
        [SerializeField] private MeshRenderer[] _highlightRenderers;

        private void Awake() { }

        public void Reset() { }

        public void ShowResult(bool isCorrectAnswer) { }

        private void OnTriggerEnter(Collider other) { }

        private void OnTriggerExit(Collider other) { }
    }
}
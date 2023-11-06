using System;
using System.Collections.Generic;
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
        private Color _original;

        [SerializeField] private TMP_Text _answerField;
        [SerializeField] private Material _sourceMaterial;
        [SerializeField] private MeshRenderer[] _highlightRenderers;

        private Material _itemMaterial;
        private bool _resultShown = false;

        private void Start() {
            _itemMaterial = new Material(_sourceMaterial);
            _itemMaterial.color = _sourceMaterial.color;
            _original = _sourceMaterial.color;

            
            for (int i = 0; i < _highlightRenderers.Length; i++) {
                _highlightRenderers[i].material = _itemMaterial;
            }
        }

        public void Reset() {
            _itemMaterial.color = _original;
            HasVote = false;
            onVoteChanged?.Invoke(false);
            _resultShown = false;
        }

        public void ShowResult(bool isCorrectAnswer) {
            //for now we change the material color here, but to make it more flexible we can add a visualizer here that 
            //shows an animation, or changes the material or whatever
            _itemMaterial.color = (isCorrectAnswer ? _correct : _false);
            _resultShown = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (!_resultShown) {
                _itemMaterial.color = _highlight;
            }
            
            HasVote = true;
            onVoteChanged?.Invoke(true);
        }

        private void OnTriggerExit(Collider other) {
            if (!_resultShown) {
                _itemMaterial.color = _original;
            }

            HasVote = false;
            onVoteChanged?.Invoke(false);
        }
    }
}
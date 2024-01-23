using System;
using Base.Ravel.Creator.Components.Naming;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games
{
    //after game events
    [DefaultExecutionOrder(100)]
    public abstract partial class RavelGameComponent : ComponentBase, IUniqueId, INameIdentifiedObject
    {
        public bool SetUniqueID {
            get { return true; }
        }
        
        public string Name {
            get { return BaseData.name; }
        }
        
        public int ID {
            get { return BaseData.id;}
            set { BaseData.id = value; }
        }

        public abstract RavelGameData BaseData { get; }

        public void StartGame() { }
        public void StopGame() { }
        public void ResetGame() { }
        
#if UNITY_EDITOR
    [CustomEditor(typeof(RavelGameComponent), true)]
    public class RavelGameComponentEditor : Editor
    {
        private RavelGameComponent _baseInstance;
        
        private bool _nameValid;

        protected virtual void OnEnable() {
            _baseInstance = (RavelGameComponent)target;
        }
        
        public override void OnInspectorGUI() {
            _nameValid = NameAvailabilityCheck.Check(_baseInstance);

            if (!_nameValid) {
                _baseInstance.BaseData.name = EditorGUILayout.TextField("Name", _baseInstance.BaseData.name);
                
                EditorGUILayout.HelpBox($"Game name should be unique. Name {_baseInstance.BaseData.name} is already taken!", MessageType.Error);
                return;
            }
            DrawDefaultInspector();
        }
    }
#endif
    }
    
    [Serializable]
    public class RavelGameData : ComponentData
    {
        [HideInInspector] public int id;
        public string name;

        public bool startOnLoad;
        public bool hasScore;
        public ScoreboardComponent scoreboard;

        public UnityEvent onGameStart;  
        public UnityEvent onGameStop;
        public UnityEvent onGameReset;
        
        public UnityEvent<int> onGameProgress;
        public UnityEvent<string> onLocalScoreChange;
    }
}

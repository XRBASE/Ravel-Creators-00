using System;
using Base.Ravel.Creator.Components.Naming;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games
{
    public abstract partial class RavelGameComponent : ComponentBase, IUniqueId, INameIdentifiedObject
    {
        public bool SetUniqueID {
            get { return true; }
        }

        public string Name {
            get { return BaseData.name; }
        }

        public int ID {
            get { return BaseData.id; }
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
        private SerializedProperty _baseData;
        private SerializedProperty _baseSerializedProperty;
        
        private bool _nameValid;

        protected virtual void OnEnable() {
            _baseInstance = (RavelGameComponent)target;
            _baseData = serializedObject.FindProperty("_data");

            if (string.IsNullOrEmpty(_baseInstance.BaseData.name)) {
                _baseInstance.BaseData.name = _baseInstance.gameObject.name;
            }
            _nameValid = NameAvailabilityCheck.Check(_baseInstance);
        }
        
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            bool dirty = false;
            EditorGUI.BeginChangeCheck();
            
            _baseInstance.BaseData.name = EditorGUILayout.TextField("Name", _baseInstance.BaseData.name);
            if (EditorGUI.EndChangeCheck()) {
                dirty = true;
					
                _nameValid = NameAvailabilityCheck.Check(_baseInstance);
            }
            else {
                EditorGUI.BeginChangeCheck();
            }

            if (!_nameValid) {
                EditorGUILayout.HelpBox($"Game name should be unique. Name {_baseInstance.BaseData.name} is already taken!", MessageType.Error);
            }

            _baseInstance.BaseData.startOnLoad = EditorGUILayout.Toggle(
                new GUIContent("start on load","Automatically start the game, after it's been loaded."), _baseInstance.BaseData.startOnLoad);
            _baseInstance.BaseData.hasScore = EditorGUILayout.Toggle("Has score", _baseInstance.BaseData.hasScore);
            if (_baseInstance.BaseData.hasScore) {
                _baseInstance.BaseData.scoreboard = EditorGUILayout.ObjectField("Scoreboard", _baseInstance.BaseData.scoreboard,
                    typeof(ScoreboardComponent), true) as ScoreboardComponent;
            }
            
            GUIDrawProperty("onGameStart");
            GUIDrawProperty("onGameStop");
            GUIDrawProperty("onGameReset");
            
            if (dirty || EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_baseInstance);
            }
        }
        
        /// <summary>
        /// Draws event property with given name in GUI.
        /// </summary>
        private void GUIDrawProperty(string propertyName) {
            _baseSerializedProperty = _baseData.FindPropertyRelative(propertyName);
            EditorGUILayout.PropertyField(_baseSerializedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    }
    
    [Serializable]
    public class RavelGameData : ComponentData
    {
        public int id;
        public string name;

        public bool startOnLoad;
        public bool hasScore;
        public ScoreboardComponent scoreboard;

        public UnityEvent onGameStart;  
        public UnityEvent onGameStop;
        public UnityEvent onGameReset;
    }
}

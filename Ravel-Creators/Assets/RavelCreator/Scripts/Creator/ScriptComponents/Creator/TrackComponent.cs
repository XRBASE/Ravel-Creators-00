using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// This component creates a track which the player can navigate on
    /// </summary>
    [AddComponentMenu("Ravel/Track")]
    public partial class TrackComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        /// <summary>
        /// When this is enabled, the track will complete when the player has reached the same position as the first transform in the trackPositions
        /// </summary>
        public bool FinishOnTrackStarted
        {
            get { return _data.finishTrackOnStartReached; }
            set { _data.finishTrackOnStartReached = value; }
        }

        /// <summary>
        /// When this is enabled, the player will automatically walk over the track
        /// </summary>
        public bool AutoWalk
        {
            get { return _data.autoWalk; }
            set { _data.autoWalk = value; }
        }

        [SerializeField, HideInInspector] private TrackData _data;

        /// <summary>
        /// When this is called, the player will set the navigation target to the track targets until the last target has been reached
        /// </summary>
        public void StartTrack()
        {
        }

        /// <summary>
        /// When this is called, the player will set the navigation target to the track targets in reversed order until the last target has been reached
        /// </summary>
        public void StartTrackReversed()
        {
        }


        protected override void BuildComponents()
        {
        }

        protected override void DisposeData()
        {
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(TrackComponent))]
        private class TrackComponentEditor : Editor
        {
            private TrackComponent _instance;
            private SerializedProperty _data;
            private SerializedProperty _onTrackStarted;
            private SerializedProperty _onTrackEnded;
            private SerializedProperty _trackTargets;

            public void OnEnable()
            {
                _instance = (TrackComponent) target;
                _data = serializedObject.FindProperty("_data");
                _trackTargets = _data.FindPropertyRelative("trackTargets");
                _onTrackStarted = _data.FindPropertyRelative("onTrackStarted");
                _onTrackEnded = _data.FindPropertyRelative("onTrackEnded");
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                EditorGUI.BeginChangeCheck();

                _instance._data.finishTrackOnStartReached = EditorGUILayout.Toggle("Finish Track On Start Reached",
                    _instance._data.finishTrackOnStartReached);

                _instance._data.autoWalk = EditorGUILayout.Toggle("Auto Walk", _instance._data.autoWalk);

                _instance._data.run = EditorGUILayout.Toggle("Run", _instance._data.run);
                
                EditorGUILayout.PropertyField(_onTrackStarted);

                EditorGUILayout.PropertyField(_onTrackEnded);

                EditorGUILayout.PropertyField(_trackTargets);
                
                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_instance);
                }
            }
        }
#endif
    }

    [Serializable]
    public class TrackData : ComponentData
    {
        public UnityEvent onTrackStarted, onTrackEnded;
        public bool finishTrackOnStartReached;
        public bool autoWalk;
        public bool run;
        public List<Transform> trackTargets;
    }
}
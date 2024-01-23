using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// This component creates a track which the player can navigate on
    /// </summary>
    [AddComponentMenu("Ravel/Track")]
    [HelpURL("https://www.notion.so/thenewbase/Track-aac7da0473194265985395da1df7c134")]
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

        [SerializeField] private TrackData _data;

        /// <summary>
        /// When this is called, the player will set the navigation target to the track targets until the last target has been reached
        /// </summary>
        public void StartTrack() { }

        /// <summary>
        /// When this is called, the player will set the navigation target to the track targets in reversed order until the last target has been reached
        /// </summary>
        public void StartTrackReversed() { }


        protected override void BuildComponents() { }

        protected override void DisposeData() { }
    }
    
    [Serializable]
    public class TrackData : ComponentData
    {
        
        [Tooltip("Finish Track On Start Reached")]
        public bool finishTrackOnStartReached;
        [Tooltip("Move forwards without input")]
        public bool autoWalk;
        [Tooltip("Always run on path")]
        public bool run;
        [Tooltip("Points on the path in order")]
        public List<Transform> trackTargets;
        
        public UnityEvent onTrackStarted, onTrackEnded;
    }
}
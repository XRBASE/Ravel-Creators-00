using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
    [AddComponentMenu("Ravel/NPC")]
    public partial class NPCComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        [SerializeField] private NPCData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        /// <summary>
        /// Set the anchor to the current position of the NPC and change the npc state to Idle
        /// </summary>
        public void IdleWalk() { }

        public void Follow(Transform target) { }

        public void FollowLocalPlayer() { }

        public void MoveToLocalPlayer() { }

        public void LookAtLocalPlayer() { }

        public void LookAtTarget(Transform target) { }

        public void Idle() { }

        public void Emote(int index) { }

        public void WalkToTarget(Transform target) { }

        public void RunToTarget(Transform target) { }

        public void TeleportToTarget(Transform target) { }

        public void MoveToTarget(Transform target) { }

        /// <summary>
        /// Anchor functions as a returning or target transform for the NPC, once the anchor is set, you can walk/run/teleport or IdleWalk on this anchor
        /// </summary>
        /// <param name="transform">anchor position</param>
        public void SetAnchor(Transform transform) { }

        public void Jump() { }

#region Deprecated functions

        public void WalkToAnchor() { }

        public void RunToAnchor() { }

        public void TeleportToAnchor() { }

        public void MoveToAnchor() { }

#endregion
    }


    [Serializable]
    public class NPCData : ComponentData
    {
        [Tooltip("Radius in which random walk is executed")] 
        public float walkRadius = 3f;
        [Tooltip("Time in between walking to different positions when random walking")]
        public float idleWalkUpdateTimeOut = 5f;
        
        public Animator animator;
        public UnityEvent onInitialised;
    }
}
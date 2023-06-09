using System;
using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    [AddComponentMenu("Ravel/NPC")]
    public partial class NPCComponent : ComponentBase
    {
        public override ComponentData Data { get; }

        [SerializeField, HideInInspector] private NPCData _data;

        protected override void BuildComponents() {}

        protected override void DisposeData(){}

        /// <summary>
        /// Set the anchor to the current position of the NPC and change the npc state to Idle
        /// </summary>
        public void IdleWalk()
        {
        }

        public void Follow(Transform target)
        {
            
        }

        public void FollowLocalPlayer()
        {
        }

        public void MoveToLocalPlayer()
        {
            
        }

        public void LookAtLocalPlayer()
        {
        }

        public void LookAtTarget(Transform target)
        {
            
        }

        public void Idle()
        {
        }

        public void Emote(int index)
        {
        }

        public void WalkToAnchor()
        {
        }

        public void RunToAnchor()
        {
        }

        public void TeleportToAnchor()
        {
        }

        /// <summary>
        /// Anchor functions as a returning or target transform for the NPC, once the anchor is set, you can walk/run/teleport or IdleWalk on this anchor
        /// </summary>
        /// <param name="transform">anchor position</param>
        public void SetAnchor(Transform transform)
        {
        }

        public void Jump()
        {
        }

        public void MoveToAnchor()
        {
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(NPCComponent))]
        private class NPCComponentEditor : Editor
        {
            private NPCComponent _instance;
            private SerializedProperty _data;
            private SerializedProperty _animator;

            public void OnEnable()
            {
                _instance = (NPCComponent) target;
                _data = serializedObject.FindProperty("_data");
                _animator = _data.FindPropertyRelative("animator");
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                EditorGUI.BeginChangeCheck();

                _instance._data.walkRadius = EditorGUILayout.FloatField("Walk Radius",
                    _instance._data.walkRadius);
                
                _instance._data.idleWalkUpdateTimeOut = EditorGUILayout.FloatField("Idle Walk Update Timeout",
                    _instance._data.idleWalkUpdateTimeOut);

                EditorGUILayout.PropertyField(_animator);
                
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
    public class NPCData : ComponentData
    {
        public float walkRadius = 3f;
        public float idleWalkUpdateTimeOut = 5f;
        public Animator animator;
    }
}
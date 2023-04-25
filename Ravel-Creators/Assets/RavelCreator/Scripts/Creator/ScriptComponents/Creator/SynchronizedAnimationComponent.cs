using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    [RequireComponent(typeof(Animation))]
    public partial class SynchronizedAnimationComponent : ComponentBase, INetworkId
    {
        public override ComponentData Data {
            get { return null; }
        }

        [SerializeField, HideInInspector] protected AnimationData _data;
		
        public bool Networked {
            get { return true; }
        }
        
        public int ID {
            get { return _data.id;}
            set { _data.id = value; }
        }

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        public void Play() { }
        public void Pause() { }
        public void Stop(){}
        
#if UNITY_EDITOR
        [CustomEditor(typeof(SynchronizedAnimationComponent))]
        private class SynchronizedAnimationComponentEditor : Editor
        {
            private Animation _anim;
            private SynchronizedAnimationComponent _instance;
            
            private void OnEnable() {
                _instance = (SynchronizedAnimationComponent)target;
                _anim = _instance.gameObject.GetComponent<Animation>();
            }

            public override void OnInspectorGUI() {
                DrawDefaultInspector();
				
                GUILayout.Label("Any animation with this component bound to it will be synchronized among players.\n" +
                                "It does require the play, pause and stop callbacks to be executed through this script.");
                
                if (_anim != null) {
                    int c = _anim.GetClipCount();
                    if (c == 0) {
                        EditorGUILayout.HelpBox(
                            $"Animator has no clips (animations)!",
                            MessageType.Error);
                    } else {
                        if (_anim.clip == null) {
                            EditorGUILayout.HelpBox(
                                $"No active clip set, please select a clip in the animation component!",
                                MessageType.Warning);
                        }
                        else if (c > 1) {
                            EditorGUILayout.HelpBox(
                                $"Multiple clips(animations) not (yet) supported, this script will only play the first animation ({_anim.clip.name})!",
                                MessageType.Warning);
                        }
                        else if (!_anim.clip.legacy) {
                            EditorGUILayout.HelpBox(
                                $"{_anim.clip.name} animation should be marked as legacy clip! Non legacy clips might not play correctly",
                                MessageType.Warning);
                            if (GUILayout.Button("Fix")) {
                                _anim.clip.legacy = true;
                                EditorUtility.SetDirty(_anim.clip);
                                AssetDatabase.SaveAssets();
                            }
                        }
                    }
                }
            }
        }
#endif
    }

    [Serializable]
    public class AnimationData : ComponentData
    {
        public int id;
    }
}
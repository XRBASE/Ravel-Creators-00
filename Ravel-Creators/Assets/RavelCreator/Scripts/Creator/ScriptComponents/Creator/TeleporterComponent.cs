using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{

    /// <summary>
    /// Teleport component used for teleporting the player to different spawnpoints in the scene.
    /// </summary>
    [AddComponentMenu("Ravel/Teleporter")]
    public partial class TeleporterComponent : ComponentBase
    {
        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField, HideInInspector] private TeleportedData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        /// <summary>
        /// Teleport player to spawnposition set in inspector.
        /// </summary>
        public void Teleport() { }

        /// <summary>
        /// Teleport player to spawnpoint with given ID.
        /// </summary>
        /// <param name="id">index of spawnpoint to teleport to.</param>
        public void TeleportToId(int id) { }

#if UNITY_EDITOR
        [CustomEditor(typeof(TeleporterComponent))]
        private class TeleporterComponentEditor : Editor
        {
            private TeleporterComponent _instance;

            private void OnEnable() {
                _instance = (TeleporterComponent)target;
            }

            public override void OnInspectorGUI() {
                DrawDefaultInspector();
                
                EditorGUI.BeginChangeCheck();
                //two part system, either the id is used to retrieve the reference, or the refrence is used to retrieve the id.
                
                _instance._data.locationId = EditorGUILayout.IntField("Location id", _instance._data.locationId);
                if (EditorGUI.EndChangeCheck()) {
                    if (SpawnPointComponent.TryGetById(_instance._data.locationId, out SpawnPointComponent spawn)) {
                        if (spawn != _instance._data.location) {
                            _instance._data.location = spawn;

                            EditorUtility.SetDirty(_instance);
                        }
                    } else {
                        _instance._data.location = null;
                        
                        EditorUtility.SetDirty(_instance);
                        Debug.LogWarning($"Spawnposition with ID {_instance._data.locationId} not found!");
                    }
                }
                
                EditorGUI.BeginChangeCheck();
                _instance._data.location = EditorGUILayout.ObjectField("Location spawnpoint", _instance._data.location,
                    typeof(SpawnPointComponent), true) as SpawnPointComponent;
                if (EditorGUI.EndChangeCheck()) {
                    if (_instance._data.location != null) {
                        _instance._data.locationId = _instance._data.location.ID;
                    }
                    else {
                        _instance._data.locationId = -1;
                    }
                    EditorUtility.SetDirty(_instance);
                }

                if (_instance._data.locationId < 0) {
                    EditorGUILayout.HelpBox("Teleporters without location only work when calling the TeleportToId(id) function", 
                        MessageType.Warning);
                }
            }
        }
#endif
    }

    [Serializable]
    public class TeleportedData : ComponentData
    {
        public int locationId = -1;
#if UNITY_EDITOR
        //location is only used to clarify the behaviour for creators, not needed outside of the editor.
        public SpawnPointComponent location;

#endif
    }
}
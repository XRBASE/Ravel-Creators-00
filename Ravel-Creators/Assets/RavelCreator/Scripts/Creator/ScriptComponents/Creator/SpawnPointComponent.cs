using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// This component offers a spawnpoint (0 is where all players start). These points are also reused for in space teleporting of players.
    /// </summary>
    [HelpURL("https://www.notion.so/thenewbase/Spawnpoint-84f121367ed449c8993d8bd16d8eb9f1")]
    public partial class SpawnPointComponent : ComponentBase
    {
        public int ID {
            get { return _data.id; }
        }

        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField] private SpawnPointData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        /// <summary>
        /// Teleports player to this spawnpoint
        /// </summary>
        public void TeleportHere() { }

        /// <summary>
        /// Walks the player to this spawnpoint (if distance is big enough, the teleport will still be triggered).
        /// </summary>
        public void WalkHere() { }

#if UNITY_EDITOR
        /// <summary>
        /// Retrieves spawnpoint based on the given id and outs the result, or returns false.
        /// </summary>
        public static bool TryGetById(int id, out SpawnPointComponent spawn) {
            SpawnPointComponent[] spawns = FindObjectsOfType<SpawnPointComponent>();
            for (int i = 0; i < spawns.Length; i++) {
                if (spawns[i].ID == id) {
                    spawn = spawns[i];
                    return true;
                }
            }

            spawn = null;
            return false;
        }

        /// <summary>
        /// Snaps transform to closest collider below it.
        /// </summary>
        public void SnapToCollider() {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit)) {
                transform.position = hit.point;
            }
        }

        [CustomEditor(typeof(SpawnPointComponent))]
        private class SpawnPointComponentEditor : Editor
        {
            public override void OnInspectorGUI() {
                SpawnPointComponent instance = (SpawnPointComponent)target; 
                DrawDefaultInspector();
                
                //show which spawn point is the default
                bool pEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.Toggle("Is default spawnpoint", instance._data.id == 0);
                GUI.enabled = pEnabled;
                
                if (GUILayout.Button("Snap to nearest collider (below)")) {
                    instance.SnapToCollider();
                    EditorUtility.SetDirty(instance);
                }
            }
        }
#endif
    }

    [Serializable]
    public class SpawnPointData : ComponentData
    {
        [Tooltip("(Unique) spawn index")]
        public int id = 0;
    }
}
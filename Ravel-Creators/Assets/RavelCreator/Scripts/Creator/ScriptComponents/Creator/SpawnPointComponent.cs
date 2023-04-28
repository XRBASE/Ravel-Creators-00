using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    public partial class SpawnPointComponent : ComponentBase
    {
        public int ID {
            get { return _data.id; }
        }

        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField, HideInInspector] private SpawnPointData _data;
        
        protected override void BuildComponents() {}

        protected override void DisposeData() { }

        public void TeleportHere() { }
        public void WalkHere() { }

#if UNITY_EDITOR
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

                EditorGUI.BeginChangeCheck();
                instance._data.id = EditorGUILayout.IntField("(Unique) spawn index", instance._data.id);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(instance);
                }

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
        //TODO: name?
        public int id = 0;
    }
}
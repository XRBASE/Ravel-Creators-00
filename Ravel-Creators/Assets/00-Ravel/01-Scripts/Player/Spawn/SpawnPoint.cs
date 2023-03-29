using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnPoint : MonoBehaviour
{
    public const int DEFAULT = 0;

    [SerializeField] private int _id;
    public void MoveToSpawnPoint() { }
    
    public void TeleportToSpawnPoint() { }
    
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnPoint))]
    private class SpawnPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var node = (SpawnPoint) target;
            if(node._id== DEFAULT)
                EditorGUILayout.HelpBox("This is the default spawnpoint", MessageType.Info);
            else
                EditorGUILayout.HelpBox($"Make sure you have 1 spawnpoint set as the default with id = {DEFAULT}. This will be the main spawnpoint for the players. Also make sure that each spawnpoint has a unique ID", MessageType.Info);
            if (GUILayout.Button("Snap nearest collider below"))
            {
                if (Physics.Raycast(node.transform.position, Vector3.down, out var hit))
                {
                    node.transform.position = hit.point;
                }
            }
        }
    }
    #endif
}

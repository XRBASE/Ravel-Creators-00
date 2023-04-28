#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Base.Ravel.UI.Hints
{
    [CreateAssetMenu(menuName = "Ravel/Hints", fileName = "Hint", order = 7)]
    public class Hint : ScriptableObject
    {
        [SerializeField, HideInInspector] private string _guid;
        
        public string hintName;
        public string text;
        public Sprite sprite;

        [Tooltip("0 is highest priority and will be displayed first")]
        public int priority;
        

#if UNITY_EDITOR
        public void SetNewGUID() {
            _guid = GUID.Generate().ToString();
            EditorUtility.SetDirty(this);
        }
        
        public static bool CreateAndSaveHintEditor(out Hint created, bool select = true) {
            string path = EditorUtility.SaveFilePanel("Save hint reference", Application.dataPath, "Hint", "asset");

            if (!string.IsNullOrEmpty(path)) {
                Hint hint = ScriptableObject.CreateInstance<Hint>();
                hint.SetNewGUID();

                //path to folder root (-6 is for including the Assets folder)
                path = path.Substring(Application.dataPath.Length - 6);
                
                AssetDatabase.CreateAsset(hint, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (select) {
                    Selection.activeObject = hint;
                }
                
                created = hint;
                return true;
            }

            created = null;
            return false;
        }
        
        [CustomEditor(typeof(Hint))]
        public class HintEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                Hint hint = (Hint) target;
                DrawDefaultInspector();
                
                if (string.IsNullOrEmpty(hint._guid)) {
                    hint.SetNewGUID();
                }

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUILayout.TextField("Guid",hint._guid);
                GUI.enabled = true;
                if (GUILayout.Button("Regenerate", GUILayout.Width(100))) {
                    hint.SetNewGUID();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
    }
}


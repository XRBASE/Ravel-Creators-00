using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Displays user data of the local user, using callbacks to send the data to an image, TMP or something else.
    /// </summary>
    [AddComponentMenu("Ravel/Display player info")]
    public partial class DisplayPlayerInfoComponent : ComponentBase
    {
        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField, HideInInspector] private DisplayPlayerInfoData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

#if UNITY_EDITOR
        [CustomEditor(typeof(DisplayPlayerInfoComponent))]
        private class DisplayPlayerInfoComponentEditor : Editor
        {
            private DisplayPlayerInfoComponent _instance;
            private SerializedProperty _data;
            private SerializedProperty _evtProperty;

            private void OnEnable() {
                _instance = (DisplayPlayerInfoComponent)target;
                _data = serializedObject.FindProperty("_data");
            }

            public override void OnInspectorGUI() {
                DrawDefaultInspector();
                
                EditorGUI.BeginChangeCheck();
                _instance._data.infoType = (DisplayPlayerInfoData.InfoType)EditorGUILayout.EnumPopup("Data to show:", 
                    _instance._data.infoType);
                _instance._data.retrieveOnAwake =
                    EditorGUILayout.Toggle("Retrieve data on awake", _instance._data.retrieveOnAwake);

                if (_instance._data.SpriteInfo) {
                    _evtProperty = _data.FindPropertyRelative("onSpriteRetrieved");
                    EditorGUILayout.PropertyField(_evtProperty);
                    serializedObject.ApplyModifiedProperties();
                }
                else {
                    _evtProperty = _data.FindPropertyRelative("onStringRetrieved");
                    EditorGUILayout.PropertyField(_evtProperty);
                    serializedObject.ApplyModifiedProperties();
                }
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(_instance);
                }
            }
        }
#endif
    }
    
    [Serializable]
    public class DisplayPlayerInfoData : ComponentData
    {
        public bool SpriteInfo {
            get { return infoType == InfoType.ProfilePicture;}
        }
            
        public InfoType infoType;
        public bool retrieveOnAwake;

        public UnityEvent<string> onStringRetrieved;
        public UnityEvent<Sprite> onSpriteRetrieved;

        public enum InfoType
        {
            FirstName,
            LastName,
            FullName,
            ProfilePicture,
        }
    }
}

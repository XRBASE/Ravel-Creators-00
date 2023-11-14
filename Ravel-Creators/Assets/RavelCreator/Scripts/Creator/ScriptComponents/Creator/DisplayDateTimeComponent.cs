using System;
using Base.Ravel.Creator.Components;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Displays current date on textmeshprougui
    /// </summary>
    [AddComponentMenu("Ravel/Display Date")]
    [HelpURL("https://www.notion.so/thenewbase/Display-Date-Component-e8fbf232ef944cc6b648390a27679e30?pvs=4")]
    public partial class DisplayDateTimeComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        protected override void BuildComponents()
        {
        }

        protected override void DisposeData()
        {
        }

        public void DisplayDate()
        {
        }

        [SerializeField, HideInInspector] private DisplayDateTimeData _data;


#if UNITY_EDITOR
        [CustomEditor(typeof(DisplayDateTimeComponent))]
        public class DisplayDateComponentEditor : Editor
        {
            private DisplayDateTimeComponent _instance;
            private SerializedProperty _data;

            private void OnEnable()
            {
                _instance = (DisplayDateTimeComponent) target;
                _data = serializedObject.FindProperty("_data");
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("textMeshProUGUI"));
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("dateTimeFormat"));
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("prefixText"));
                if (_instance._data.dateTimeFormat == DisplayDateTimeData.DateTimeFormat.Custom)
                {
                    EditorGUILayout.PropertyField(_data.FindPropertyRelative("customDateTimeFormat"));
                }

                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_instance);
                }
            }
        }
#endif
    }
}

[Serializable]
public class DisplayDateTimeData : ComponentData
{
    public TextMeshProUGUI textMeshProUGUI;
    public DateTimeFormat dateTimeFormat;
    public string customDateTimeFormat;
    public string prefixText;

    public enum DateTimeFormat
    {
        Date,
        LongDate,
        DateTime,
        Time,
        Custom
    }
}
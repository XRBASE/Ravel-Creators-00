using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Displays current date on textmeshprougui
    /// </summary>
    [AddComponentMenu("Ravel/Display Date Time")]
    [HelpURL("https://www.notion.so/thenewbase/Display-Date-Component-e8fbf232ef944cc6b648390a27679e30?pvs=4")]
    public partial class DisplayDateTimeComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        protected override void BuildComponents(){}

        protected override void DisposeData(){}

        public void ShowDateTime(){}
        
        public static string ParseDateTime(DisplayDateTimeData.DateTimeFormat format, string customFormat = default)
        {
            switch (format)
            {
                case DisplayDateTimeData.DateTimeFormat.Date:
                    return DateTime.Now.ToShortDateString();
                case DisplayDateTimeData.DateTimeFormat.Time:
                    return $"{DateTime.Now:HH:mm:ss}";
                case DisplayDateTimeData.DateTimeFormat.LongDate:
                    return DateTime.Now.ToLongDateString();
                case DisplayDateTimeData.DateTimeFormat.DateTime:
                    return $"{DateTime.Now.ToShortDateString()}, {DateTime.Now:HH:mm:ss}";
                case DisplayDateTimeData.DateTimeFormat.Custom:
                    return DateTime.Now.ToString(customFormat);
                default:
                    return String.Empty;
            }
        }


        [SerializeField, HideInInspector] private DisplayDateTimeData _data;

#if UNITY_EDITOR
        [CustomEditor(typeof(DisplayDateTimeComponent))]
        public class DisplayDateTimeComponentEditor : Editor
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
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("textMeshProUGUI"));
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("prefixText"));
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("dateTimeFormat"));
                if (_instance._data.dateTimeFormat == DisplayDateTimeData.DateTimeFormat.Custom)
                {
                    EditorGUILayout.PropertyField(_data.FindPropertyRelative("customDateTimeFormat"));
                }
                EditorGUILayout.PropertyField(_data.FindPropertyRelative("postfixText"));
                if (GUILayout.Button("Preview Text"))
                {
                    _instance._data.textMeshProUGUI.text =
                        _instance._data.prefixText + ParseDateTime(_instance._data.dateTimeFormat, _instance._data.customDateTimeFormat) + _instance._data.postfixText;
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

    [Serializable]
    public class DisplayDateTimeData : ComponentData
    {
        public TextMeshProUGUI textMeshProUGUI;
        public string prefixText;
        public string postfixText;
        public DateTimeFormat dateTimeFormat;
        public string customDateTimeFormat;

        public enum DateTimeFormat
        {
            Date,
            LongDate,
            DateTime,
            Time,
            Custom
        }
    }
}
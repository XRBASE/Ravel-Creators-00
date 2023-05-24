using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
    /// <summary>
    /// Adds a conversation to the bundle, in which messages can be saved and by using triggers to call next or previous
    /// the conversation can be held.
    /// </summary>
    [AddComponentMenu("Ravel/Dialog")]
    public partial class DialogComponent : ComponentBase
    {
        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField, HideInInspector] private DialogData _data;

        protected override void BuildComponents() { }

        protected override void DisposeData() { }

        /// <summary>
        /// Resets the whole dialog, so the conversation can be restarted.
        /// </summary>
        public void ResetDialog() { }

        /// <summary>
        /// Start dialog at message first message (resets if this needs to happen)
        /// </summary>
        public void StartDialog() { }

        /// <summary>
        /// Move to the next message in the conversation.
        /// </summary>
        public void NextMessage() { }

        /// <summary>
        /// Move back one message in the conversation.
        /// </summary>
        public void PrevMessage() { }

#if UNITY_EDITOR
        [CustomEditor(typeof(DialogComponent))]
        private class DialogComponentEditor : Editor
        {
            private DialogComponent _instance;
            
            private SerializedObject _serObj;
            private SerializedProperty _messages;
            private SerializedProperty _event;
            
            //checks the template object and confirms the text output is connected to it.
            private bool _transformsConnected = false;
            
            public void OnEnable() {
                _instance = (DialogComponent)target;
                _serObj = new SerializedObject(_instance);
                _messages = _serObj.FindProperty("_data").FindPropertyRelative("messages");

                _transformsConnected = false;
                //precheck if the transforms are connected, so reloading the scene, or losing data does throw a not connected error.
                if (_instance._data.displayField?.transform != null &&
                    _instance._data.displayTemplate?.transform != null) {
                    if (CheckIfTransformsConnected(_instance._data.displayField.transform, _instance._data.displayTemplate.transform)) {
                        _transformsConnected = true;
                    }
                }
            }

            public override void OnInspectorGUI() {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                DrawDefaultInspector();
                //checks for any changes, so object can be marked as dirty
                bool dirty = false;
                EditorGUI.BeginChangeCheck();
                
                //list of message info.
                EditorGUILayout.PropertyField(_messages,
                    new GUIContent("Messages", "These are the shown messages in order."));
                
                //usage
                _instance._data._usage = (DialogData.DisplayUsage)
                    EditorGUILayout.EnumPopup("Display usage", _instance._data._usage);
                EditorGUI.indentLevel = 1;
                string description = DialogData.GetUsageDescription(_instance._data._usage);
                float h = description.Split('\n').Length;
                EditorGUILayout.LabelField(description, GUILayout.Height(EditorGUIUtility.singleLineHeight * h));
                EditorGUI.indentLevel = 0;
                EditorGUILayout.Space();
                
                //boolean values.
                _instance._data._playOnAwake =
                    EditorGUILayout.Toggle("Play dialog on awake", _instance._data._playOnAwake);
                _instance._data._autoNext =
                    EditorGUILayout.Toggle(new GUIContent("Auto next message", "Move to the next message automatically when the clip and delay have passed."), 
                        _instance._data._autoNext);
                if (_instance._data._autoNext) {
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.LabelField("Use message -> options -> delay to set the amount of time,\n" +
                                               "before the next message is show. Messages without delay are not skipped.", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                    EditorGUI.indentLevel = 0;
                }
                
                //audiosource (checks the messages for messages containing a clip field).
                if (_instance._data.hasAudio) {
                    _instance._data.audioSource = EditorGUILayout.ObjectField("AudioSource",
                        _instance._data.audioSource, typeof(AudioSource), true) as AudioSource;
                }
                
                _instance._data.writeText = EditorGUILayout.Toggle(
                    new GUIContent("Write text", "Should text appear letter for letter in the prefab"),
                    _instance._data.writeText);
                if (_instance._data.writeText) {
                    EditorGUILayout.FloatField(
                        new GUIContent("Message write speed",
                            "Speed at which words are witten in the display prefab, in letters per second"),
                        _instance._data.messageSpeed);
                }
                
                //stop chance check, so checks can be reused for the template objects.
                dirty = EditorGUI.EndChangeCheck();

                
                EditorGUILayout.LabelField("Display/Message prefab");
                //begin prefab/template change check
                bool templateDirty = false;
                EditorGUI.BeginChangeCheck();
                _instance._data.displayTemplate = EditorGUILayout.ObjectField(new GUIContent("Display template", "This template acts as a prefab for the displayed messages."),
                    _instance._data.displayTemplate, typeof(GameObject), true) as GameObject;
                
                _instance._data.displayField = EditorGUILayout.ObjectField(new GUIContent("Display field", "Field in which the messages are displayed."),
                    _instance._data.displayField, typeof(TMP_Text), true) as TMP_Text;
                //This checks both the textfield and the parent, as any change in either of them triggers the following statements 
                templateDirty = EditorGUI.EndChangeCheck();
                
                //when missing a prefab, an error is shown, when not missing one, a check is performed to determine whether the textfield is in the template object.
                //if not, another error is thrown.
                if (_instance._data.displayTemplate == null || _instance._data.displayField == null) {
                    EditorGUILayout.HelpBox("Dialog requires both a display and field reference to display messages on!", MessageType.Error);
                }
                else{
                    if (templateDirty) {
                        if (!CheckIfTransformsConnected(_instance._data.displayField.transform,
                                _instance._data.displayTemplate.transform)) {
                            _transformsConnected = false;
                        }
                        else {
                            _transformsConnected = true;
                        }
                    }
                    
                    if (!_transformsConnected) {
                        EditorGUILayout.HelpBox("Dialog field should be the same transform or child of template object.", MessageType.Error);
                    }
                }
                
                //shows the possible events for the dialog.
                _event = _serObj.FindProperty("_data").FindPropertyRelative("onDialogFinished");
                EditorGUILayout.PropertyField(_event, new GUIContent("On dialog finished", "Called when all messages have been read by the user"));
                
                _event = _serObj.FindProperty("_data").FindPropertyRelative("onMessageClose");
                EditorGUILayout.PropertyField(_event,
                    new GUIContent("On message close", "Called when any message closes, passes the message index"));
                
                _event = _serObj.FindProperty("_data").FindPropertyRelative("onMessageShow");
                EditorGUILayout.PropertyField(_event,
                    new GUIContent("On message show", "Called when any message opens, passes the message index"));
                //Applies event changes
                _serObj.ApplyModifiedProperties();
                
                //when object has changed, check if it still has audio (and save) and mark the object as dirty
                if (dirty || templateDirty) {
                    _instance._data.hasAudio = CheckMessagesForAudio();
                    
                    EditorUtility.SetDirty(_instance);
                }
                
                EditorGUI.indentLevel = indent;
            }
            
            /// <summary>
            /// Checks if the field transform is contained within the template transform.
            /// </summary>
            private bool CheckIfTransformsConnected(Transform field, Transform template) {
                if (field == template)
                    return true;

                Transform cache = field;
                while (cache.parent != null) {
                    if (cache.parent == template) {
                        return true;
                    }
                    cache = cache.parent;
                }

                return false;
            }
            
            /// <summary>
            /// Checks through all the messages, to see if any of them has audio enabled. It is presumed if the flag is there
            /// that the audio is. Otherwise it will throw an error because of the missing clip.
            /// </summary>
            private bool CheckMessagesForAudio() {
                bool hasAudio = false;
                for (int i = 0; i < _instance._data.messages.Length; i++) {
                    if (_instance._data.messages[i].HasAudio) {
                        return true;
                    }
                }

                return false;
            }
        }
#endif
    }

    [Serializable]
    public class DialogData : ComponentData
    {
        //dialog data
        public DialogMessage[] messages;
        public DisplayUsage _usage;
        public bool _playOnAwake;
        public bool _autoNext;
        public bool writeText = true;
        public float messageSpeed = 20f;
        
        //audio data
        public bool hasAudio;
        public AudioSource audioSource;

        //display data
        public GameObject displayTemplate;
        public TMP_Text displayField;
        
        //events
        public UnityEvent onDialogFinished;
        public UnityEvent<int> onMessageClose;
        public UnityEvent<int> onMessageShow;

        public static string GetUsageDescription(DisplayUsage usage) {
            return USAGE_DESCRIPTION[(int)usage];
        }
        
        /// <summary>
        /// Either reuse the template for the whole dialog, swap between two displays, or keep all displays (one per message).
        /// </summary>
        public enum DisplayUsage
        {
            Reuse = 0,
            Cycle = 1,
            Keep = 2,
        }

        private static readonly string[] USAGE_DESCRIPTION = new[] {
            "Reuses the same dialog template for each of the messages", 
            "Cycles between two instances of the template. \nOne is always visible, the other can be used for fade in/out.", 
            "Keep retains the used dialog templates. \nNew messages are instantiated, \nso the message history can be kept."
        };
    }

    /// <summary>
    /// Simple class for containing the dialog data that the creator has filled in.
    /// </summary>
    [Serializable]
    public class DialogMessage
    {
        /// <summary>
        /// Runtime id of the message itself, so events can be called.
        /// </summary>
        public int ID { get; set; }
        public virtual bool IsTimed {
            get { return options.HasFlag(MessageFlags.Delay); }
        }
	
        public virtual bool HasAudio {
            get { return options.HasFlag(MessageFlags.Audio); }
        }
	
        public string text;
        [SerializeField] private MessageFlags options;
        public float delay = -1;
        public AudioClip clip;
        
        /// <summary>
        /// Optional data for creator to add to messages.
        /// </summary>
        [Flags]
        private enum MessageFlags
        {
            None = 0,
            Delay = 1,
            Audio = 2
        }
        
#if UNITY_EDITOR
        // IngredientDrawer
        [CustomPropertyDrawer(typeof(DialogMessage))]
        public class DialogMessageDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
                //determines the height of the drawn rect. Cannot use variables, as the drawer class is used for all properties.
                //height of single line is always used to draw the foldout on
                float h = EditorGUIUtility.singleLineHeight;
                if (property.isExpanded) {
                    //text and options always drawn when property is expanded
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text"));
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options"));
                    
                    //add height of flags, only if the flags have been included.
                    MessageFlags flags = GetFlags(property);
                    if (flags.HasFlag(MessageFlags.Delay))
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("delay"));
                    if (flags.HasFlag(MessageFlags.Audio))
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("clip"));
                }
                
                return h;
            }

            private MessageFlags GetFlags(SerializedProperty property) {
                return (MessageFlags) property.FindPropertyRelative("options").enumValueFlag;
            }

            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                
                //draws foldout thingie with name as label, and then saves the state into property expanded, so it can be retrieved later.
                var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(rect, property.isExpanded, label);
                //caches used height values, for further positions
                float h = rect.height;
                
                if (property.isExpanded) {
                    //text field
                    rect = new Rect(position.x, position.y + h, position.width,
                        EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")));
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("text"), new GUIContent("Text"));
                    h += rect.height;

                    //options dropdown
                    rect = new Rect(position.x, position.y + h, position.width,
                        EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options")));
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("options"), new GUIContent("Options"));
                    h += rect.height;
                    
                    //retrieve flags value as value
                    MessageFlags flags = GetFlags(property);
                    
                    //if delay, draw delay
                    if (flags.HasFlag(MessageFlags.Delay)) {
                        rect = new Rect(position.x, position.y + h, position.width,
                            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("delay")));
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("delay"), new GUIContent("delay"));
                        h += rect.height;
                    }
                    
                    //if audio, draw audio
                    if (flags.HasFlag(MessageFlags.Audio)) {
                        rect = new Rect(position.x, position.y + h, position.width,
                            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("clip")));
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("clip"), new GUIContent("clip"));
                    }
                }
                //stop of the foldout
                EditorGUI.EndFoldoutHeaderGroup();
                
                // Set indent back to what it was
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}
using System;
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
    [AddComponentMenu("Ravel/Dialog/Dialog component")]
    [HelpURL("https://www.notion.so/thenewbase/Dialog-265c38d502a74d1fad14815c0eb73b35")]
    public partial class DialogComponent : ComponentBase
    {
        public override ComponentData Data {
            get { return _data; }
        }
        [SerializeField] private DialogData _data;

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

            //previous message count
            private int pMsgCount = 0;
            
            public void OnEnable() {
                _instance = (DialogComponent)target;
            }
            
            public override void OnInspectorGUI() {
                EditorGUI.BeginChangeCheck();
                pMsgCount = _instance._data.messages.Length;
                DrawDefaultInspector();
                //checks for any changes, so object can be marked as dirty
                bool dirty = EditorGUI.EndChangeCheck();
                
                
                if (dirty) {
                    if (pMsgCount < _instance._data.messages.Length) {
                        //message added
                        DialogMessage newMsg;
                        
                        //last valid msg
                        DialogMessage sampleMsg = _instance._data.messages[pMsgCount - 1];
                        for (int i = pMsgCount; i < _instance._data.messages.Length; i++) {
                            newMsg = _instance._data.messages[i];
                            newMsg.CopyData(sampleMsg);
                        }
                    }
                }

                //when missing a prefab, an error is shown, when not missing one, a check is performed to determine whether the textfield is in the template object.
                //if not, another error is thrown.
                if (_instance._data.displayTemplate == null) {
                    EditorGUILayout.HelpBox("Dialog requires a display on which messages are shown!", MessageType.Error);
                } else {
                    if (!_instance._data.displayTemplate.HasHeaderReference)
                        EditorGUILayout.HelpBox("Cannot display messages with header, header is not set in display component!", MessageType.Warning);
                    if (!_instance._data.displayTemplate.HasBodyReference)
                        EditorGUILayout.HelpBox("Cannot display messages, no body set in display component!", MessageType.Error);
                        
                }
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
        public DialogDisplay displayTemplate;
        
        //events
        public UnityEvent onDialogStart;
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
        
        public virtual bool HasHeader {
            get { return options.HasFlag(MessageFlags.Header); }
        }
        
        public virtual bool HasColor {
            get { return options.HasFlag(MessageFlags.Color); }
        }
        
        public virtual bool HasImg {
            get { return options.HasFlag(MessageFlags.Image); }
        }
	
        public string header;
        public bool playerNameHeader;
        public string body;
        public Color color;
        public Sprite img;
        public bool userPlayerImg;
        
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
            Delay = 1<<0,
            Audio = 1<<1,
            Header =1<<2,
            Color = 1<<3,
            Image = 1<<4,
        }

        /// <summary>
        /// Copy message internal data (header, color, img and options).
        /// </summary>
        /// <param name="copyFrom">image from which data is copied.</param>
        public void CopyData(DialogMessage copyFrom) {
            options = copyFrom.options;
            if (copyFrom.HasHeader) {
                header = copyFrom.header;
            }

            if (copyFrom.HasColor) {
                color = copyFrom.color;
            }

            if (copyFrom.HasImg) {
                img = copyFrom.img;
            }
        }
        
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(DialogMessage))]
        public class DialogMessageDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
                //determines the height of the drawn rect. Cannot use variables, as the drawer class is used for all properties.
                //height of single line is always used to draw the foldout on
                float h = EditorGUIUtility.singleLineHeight;
                if (property.isExpanded) {
                    //text and options always drawn when property is expanded
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("body"));
                    h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options"));
                    
                    //add height of flags, only if the flags have been included.
                    MessageFlags flags = GetFlags(property);
                    if (flags.HasFlag(MessageFlags.Delay))
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("delay"));
                    if (flags.HasFlag(MessageFlags.Audio))
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("clip"));
                    if (flags.HasFlag(MessageFlags.Header)) {
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("playerNameHeader"));
                        if (!property.FindPropertyRelative("playerNameHeader").boolValue)
                            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("header"));
                    }
                    if (flags.HasFlag(MessageFlags.Color)) {
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("color"));
                    }
                    if (flags.HasFlag(MessageFlags.Image)) {
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("userPlayerImg"));
                        if (!property.FindPropertyRelative("userPlayerImg").boolValue)
                            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("img"));
                    }
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
                    //retrieve flags value as value
                    MessageFlags flags = GetFlags(property);
                    
                    //if header, draw header
                    if (flags.HasFlag(MessageFlags.Header)) {
                        rect = new Rect(position.x, position.y + h, position.width,
                                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("playerNameHeader")));
                        
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("playerNameHeader"), new GUIContent("Player name header", "Header uses player name as data."));
                        h += rect.height;

                        if (!property.FindPropertyRelative("playerNameHeader").boolValue) {
                            rect = new Rect(position.x, position.y + h, position.width,
                                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("header")));
                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("header"), new GUIContent("header"));
                            h += rect.height;
                        }
                        else {
                            property.FindPropertyRelative("header").stringValue = "Player";
                        }
                    }
                    
                    //text field
                    rect = new Rect(position.x, position.y + h, position.width,
                        EditorGUI.GetPropertyHeight(property.FindPropertyRelative("body")));
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("body"), new GUIContent("message"));
                    h += rect.height;

                    //options dropdown
                    rect = new Rect(position.x, position.y + h, position.width,
                        EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options")));
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("options"), new GUIContent("Options"));
                    h += rect.height;

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
                        h += rect.height;
                    } else if (property.FindPropertyRelative("clip").objectReferenceValue != null) {
                        property.FindPropertyRelative("clip").objectReferenceValue = null;
                    }

                    if (flags.HasFlag(MessageFlags.Color)) {
                        //color for custom colored items.
                        rect = new Rect(position.x, position.y + h, position.width,
                            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("color")));
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("color"),
                            new GUIContent("color",
                                "If display contains highlight graphics, they will get assigned this color while showing this message."));
                        h += rect.height;
                    }
                    
                    if (flags.HasFlag(MessageFlags.Image)) {
                        //sprite for option item
                        rect = new Rect(position.x, position.y + h, position.width,
                            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("userPlayerImg")));

                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("userPlayerImg"),
                            new GUIContent("use player image",
                                "Show player's profile image if one is found."));
                        h += rect.height;

                        if (!property.FindPropertyRelative("userPlayerImg").boolValue) {
                            rect = new Rect(position.x, position.y + h, position.width,
                                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("img")));

                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("img"),
                                new GUIContent("profile image",
                                    "profile image to show with this message."));
                            h += rect.height;
                        }
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
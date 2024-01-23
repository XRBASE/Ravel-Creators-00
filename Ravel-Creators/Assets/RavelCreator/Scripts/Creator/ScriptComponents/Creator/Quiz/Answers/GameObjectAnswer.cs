using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Quiz
{
    /// <summary>
    /// The game object answers can't use toggles, so this class recreates it by using triggers. If a trigger is selected it
    /// invokes an event using it's index. Any selected triggers that have another index will deselect themselves.
    /// </summary>
    public partial class GameObjectAnswer : MultiChoiceAnswer
    {
        [SerializeField, Tooltip("Parent object under which the answer gameobject is spawned")]
        private Transform _parent;

        [SerializeField,
         Tooltip("Gameobject that is toggled on and off based on whether the object has been selected.")]
        private GameObject _selectionVisual;

        /// <summary>
        /// Toggles selection (Selects when unselected and vice versa).
        /// </summary>
        public void ToggleSelect() { }

        /// <summary>
        /// Unselects this item.
        /// </summary>
        public override void Unselect() { }

        /// <summary>
        /// Selects this item
        /// </summary>
        public override void Select() { }

#if UNITY_EDITOR
        [CustomEditor(typeof(GameObjectAnswer))]
        private class GameObjectAnswerEditor : Editor
        {
	        public override void OnInspectorGUI() {
		        DrawDefaultInspector();
		        EditorGUILayout.HelpBox("The game object version of an answer requires the creator to call the select and unselection options. This can be performed using the interactable component.", MessageType.Info);
	        }
        }
#endif
    }
}
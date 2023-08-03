using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Quiz
{
	public partial class QuizComponent : MonoBehaviour
	{
		public bool Networked {
			get { return _networked; }
		}

		public int ID {
			get { return _id;}
			set { _id = value; }
		}
		
		/// <summary>
		/// The type of answers processed in this quiz, excluding open questions. setting this value clears the list of questions,
		/// so proceed with caution.
		/// </summary>
		[SerializeField, HideInInspector] protected AnswerSet.Type _type = AnswerSet.Type.None;
        
		[SerializeField] private QuizVisuals _visual;
		[SerializeField] private Button _submitBtn;

		[SerializeField, Tooltip("Active quizes show the questions directly and remain active until finished. Idle quizes are disabled until enabled by another quiz.")] 
		private QuizState _initialState = QuizState.Active;
		
		[SerializeField, Tooltip("Networks the made choices and submit call!")] 
		private bool _networked = false;
		
		[SerializeField, HideInInspector]
		private int _id;
		
		/// <summary>
		/// List of questions, add needs to be done with the button below this list (in the editor). This is because questions
		/// are added as a child type of question and not as question themselves.
		/// </summary>
		[SerializeReference] protected List<Question> _questions = new List<Question>();
		
		/// <summary>
		/// Event for each of the questions, passes id and true/false question answered correctly This call is not networked (but could be if needed).
		/// </summary>
		[HideInInspector] public UnityEvent<int, bool> onQuestionAnswered;
		/// <summary>
		/// Called after all questions have been answered correctly This call is networked.
		/// </summary>
		[HideInInspector] public UnityEvent onQuizCompleted;

		public void ResetQuiz() { }

		/// <summary>
		/// Used in assets and events, moves the quiz from idle state into the active state, showing the first question.
		/// </summary>
		public void ActivateQuiz() { }

		public void SubmitAnswer() { }

		/// <summary>
		/// Call to finish all the questions in this box. 
		/// </summary>
		public void CompleteQuiz() { }
		
		private enum QuizState
		{
			Idle = 0,
			Active = 1,
			Finished = 2,
		}

#if UNITY_EDITOR
        /// <summary>
        /// Editor version to add question of given type to the list of questions, so all of the fields can be serialized and filled in.
        /// </summary>
        private void AddQuestion()
        {
            _questions.Add(new MultiChoiceQuestion(_type));
        }
        
        /// <summary>
        /// Adds serializable open question to the list.
        /// </summary>
        private void AddOpenQuestion()
        {
            _questions.Add(new OpenQuestion());
        }

        [CustomEditor(typeof(QuizComponent), true)]
        private class MonoQuizEditor : Editor
        {
	        private QuizComponent _instance;
	        private SerializedProperty onQuestionAnsweredEvt;
	        private SerializedProperty onQuizCompletedEvt;
	        
	        public void OnEnable() {
		        _instance = (QuizComponent)target;
		        onQuestionAnsweredEvt = serializedObject.FindProperty("onQuestionAnswered");
		        onQuizCompletedEvt = serializedObject.FindProperty("onQuizCompleted");
	        }
	        
            public override void OnInspectorGUI() {
	            bool dirty = false;
	            
                //check the type first, if it changes, the whole list of questions needs to be cleared as we're going to use another type of answer.
                EditorGUI.BeginChangeCheck();
                _instance._type = (AnswerSet.Type) EditorGUILayout.EnumPopup("Question answer type:", _instance._type);
                if (EditorGUI.EndChangeCheck()) {
	                _instance._questions.Clear();
	                EditorUtility.SetDirty(_instance);
                }

                //only continue if an actual type has been selected.
                if (_instance._type == AnswerSet.Type.None)
                    return;

                //track changes in the inspector and fire the editor method on the question when changes do occur.
                EditorGUI.BeginChangeCheck();
                DrawDefaultInspector();
                if (EditorGUI.EndChangeCheck()) {
	                int maxLenght = 0;
	                int maxI = 0;

	                //This tracks the length of all questions and shows the biggest one, it helps to ensure question boxes\
	                //are made to be the appropriate size.
	                if (_instance._questions.Count > 0) {
		                for (int i = 0; i < _instance._questions.Count; i++) {
			                if (_instance._questions[i] == null || _instance._questions[i].Abstract) {
				                _instance._questions[i] = new MultiChoiceQuestion(_instance._type);
				                dirty = true;
			                }

			                _instance._questions[i].OnEditorChanges();
			                if (_instance._questions[i].QuestionTxt.Length > maxLenght) {
				                maxI = i;
				                maxLenght = _instance._questions[i].QuestionTxt.Length;
			                }
		                }
		                
		                _instance._visual.QuestionTxt = _instance._questions[maxI].QuestionTxt;
	                }
                }

                //Adds questions to the list, but gives them the right type so there is actually data to serialize and use.
                if (GUILayout.Button("Add question")) {
	                _instance.AddQuestion();
                }
                if (GUILayout.Button("Add open question")) {
	                _instance.AddOpenQuestion();
                }

                if (!dirty) {
	                EditorGUI.BeginChangeCheck();
                }
                EditorGUILayout.PropertyField(onQuizCompletedEvt);
                EditorGUILayout.PropertyField(onQuestionAnsweredEvt);
                serializedObject.ApplyModifiedProperties();
                if (dirty || EditorGUI.EndChangeCheck()) {
	                EditorUtility.SetDirty(_instance);
                }
            }
        }
#endif
	}
}
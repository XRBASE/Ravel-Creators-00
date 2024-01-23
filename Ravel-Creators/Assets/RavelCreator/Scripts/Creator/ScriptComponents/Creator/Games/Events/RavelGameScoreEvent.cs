using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games
{
	[AddComponentMenu("Ravel/Games/Score event")]
	public partial class RavelGameScoreEvent : MonoBehaviour
	{
		[SerializeField] private RavelGameComponent _connectedTo;

		[SerializeField, HideInInspector] private EventType _type;

		[SerializeField, HideInInspector] private UnityEvent _defaultEvent;
		[SerializeField, HideInInspector] private UnityEvent<bool> _boolEvent;
		[SerializeField, HideInInspector] private UnityEvent<string> _stringEvent;
		[SerializeField, HideInInspector] private UnityEvent<float> _floatEvent;

		[SerializeField, HideInInspector] private ProgressEvalType _progEval;
		[SerializeField, HideInInspector] private int _percentage;
		[SerializeField, HideInInspector] private string _format = "";
		
		private enum EventType
		{
			None = 0,
			LocalScore,
			ProgressTrigger,
			ProgressValue,
			ProgressString,
			GameStart,
			GameStop,
		}

		private enum ProgressEvalType
		{
			Equals,
			More,
			Less
		}
		
#if UNITY_EDITOR
		[CustomEditor(typeof(RavelGameScoreEvent))]
		private class GameScoreEventEditor : Editor
		{
			private RavelGameScoreEvent _instance;
			
			private List<RavelGameComponent> _games;
			private List<string> _gameNames;
			private int _selGame;

			public void OnEnable() {
				_instance = (RavelGameScoreEvent)target;
				GetGames();
				if (_instance._connectedTo == null) {
					_instance._connectedTo = _games[_selGame];
					EditorUtility.SetDirty(_instance);
				}
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				bool dirty = false;

				EditorGUI.BeginChangeCheck();
				_selGame = EditorGUILayout.Popup("Game", _selGame, _gameNames.ToArray());
				if (EditorGUI.EndChangeCheck()) {
					dirty = true;

					_instance._connectedTo = _games[_selGame];
				}
				else {
					EditorGUI.BeginChangeCheck();
				}

				_instance._type = (EventType)EditorGUILayout.EnumPopup("event type", _instance._type);
				switch (_instance._type) {
					case EventType.LocalScore:
						GUIDrawProperty("_stringEvent",
							"Called when local score changes, passes score as string as parameter");
						_instance._format = EditorGUILayout.TextField(new GUIContent("Format",
								"The format is used to parse the score into a text [0] is replaced with the (local) players score"),
							_instance._format);
						break;
					case EventType.ProgressTrigger:
						GUIDrawProperty("_boolEvent",
							"Called when the game progress passes percentage set in this component bool returns result of progress check");
						_instance._progEval =
							(ProgressEvalType)EditorGUILayout.EnumPopup("Progress evaluation", _instance._progEval);
						_instance._percentage = EditorGUILayout.IntField("percentage", _instance._percentage);

						EditorGUILayout.LabelField(
							$"Event will trigger when the game's progress is {_instance._progEval} " +
							$"{(_instance._progEval == ProgressEvalType.Equals ? "" : "than")} {_instance._percentage}%.");
						break;
					case EventType.ProgressValue:
						GUIDrawProperty("_floatEvent", "Event with game progress 0 - 100%");
						break;
					case EventType.ProgressString:
						GUIDrawProperty("_stringEvent",
							"Called when game progress changes, parses value into given format string.");
						_instance._format = EditorGUILayout.TextField(new GUIContent("Format",
								"The format is used to parse the progress percentage into a text [0] is replaced with the percentage of progress"),
							_instance._format);
						break;
					case EventType.GameStart:
						GUIDrawProperty("_defaultEvent", "Called when the game starts");
						break;
					case EventType.GameStop:
						GUIDrawProperty("_defaultEvent", "Called when the game stops");
						break;
				}
				
				serializedObject.ApplyModifiedProperties();

				if (dirty || EditorGUI.EndChangeCheck()) {
					if (string.IsNullOrEmpty(_instance._format)) {
						switch (_instance._type) {
							case EventType.ProgressString:
								_instance._format = "Progress: [0]%";
								break;
							case EventType.LocalScore:
								_instance._format = "score: [0]";
								break;
						}
					}

					EditorUtility.SetDirty(_instance);
				}
			}

			/// <summary>
			/// Draws event property with given name in GUI.
			/// </summary>
			private void GUIDrawProperty(string propertyName, string tooltip = "") {
				var prop = serializedObject.FindProperty(propertyName);
				EditorGUILayout.PropertyField(prop, new GUIContent(propertyName, tooltip));
			}

			private void GetGames() {
				_games = FindObjectsOfType<RavelGameComponent>(true).ToList();
				_gameNames = new List<string>();
				for (int i = 0; i < _games.Count; i++) {
					_gameNames.Add(_games[i].Name);
				}

				if (_instance._connectedTo != null) {
					_selGame = _games.IndexOf(_instance._connectedTo);
				}
				else {
					_selGame = 0;
				}
			}
		}
#endif
	}
}
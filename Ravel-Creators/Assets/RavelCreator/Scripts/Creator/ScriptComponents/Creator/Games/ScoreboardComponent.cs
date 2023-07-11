using System;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games
{
	public partial class ScoreboardComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField, HideInInspector] private ScoreboardData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public void ResetScores() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(ScoreboardComponent))]
		private class ScoreboardComponentEditor : Editor
		{
			public override void OnInspectorGUI() {
				ScoreboardComponent instance = (ScoreboardComponent)target;

				DrawDefaultInspector();

				instance._data.maxEntries =
					EditorGUILayout.IntField("Maximum score entries", instance._data.maxEntries);
				instance._data.templateParent = EditorGUILayout.ObjectField(
					new GUIContent("Score entry template object",
						"Parent object for first score entry (copied for new entries)."),
					instance._data.templateParent,
					typeof(GameObject), true) as GameObject;
				instance._data.templateScoreField = EditorGUILayout.ObjectField(
					new GUIContent("Score text display",
						"Score is shown in this text field (in the score entry template)."),
					instance._data.templateScoreField,
					typeof(TMP_Text), true) as TMP_Text;
				instance._data.templateNameField = EditorGUILayout.ObjectField(
					new GUIContent("Name text display",
						"Player name is shown in this text field (in the score entry template)."),
					instance._data.templateNameField,
					typeof(TMP_Text), true) as TMP_Text;
			}
		}
#endif
	}

	[Serializable]
	public class ScoreboardData : ComponentData
	{
		public int maxEntries = 5;
		public GameObject templateParent;
		public TMP_Text templateScoreField;
		public TMP_Text templateNameField;

	}
}
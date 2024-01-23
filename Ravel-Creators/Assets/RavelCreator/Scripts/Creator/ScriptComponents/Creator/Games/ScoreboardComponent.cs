using System;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components.Games
{
	[AddComponentMenu("Ravel/Games/Scoreboard")]
	public partial class ScoreboardComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField] private ScoreboardData _data;

		protected override void BuildComponents() { }
		protected override void DisposeData() { }

		public void ResetScores() { }
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
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Ravel.Creator.Components.Assessments {
	public partial class AssessmentUIAnswerComponent : ComponentBase {
		public override ComponentData Data
		{
			get { return _data; }
		}

		[SerializeField] private AssessmentUIAnswerData _data;
		
		protected override void BuildComponents() { }

		protected override void DisposeData() { }

	}
	
	[Serializable]
	public class AssessmentUIAnswerData : ComponentData {
		[Tooltip("The answer as text is shown in this field")]
		public TMP_Text answerField;
		[Tooltip("Open answers can be typed in this field, it will show the answer text while nothing is typed.")]
		public TMP_InputField openAnswerField;
		[Tooltip("This toggle is used to select the answer.")]
		public Toggle selectAnswerToggle;
	}
}
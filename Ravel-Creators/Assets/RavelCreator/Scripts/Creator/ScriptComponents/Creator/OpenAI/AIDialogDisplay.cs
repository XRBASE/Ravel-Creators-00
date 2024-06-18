using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Base.Ravel.Creator.Components.OpenAI {
	public partial class AIDialogDisplay : ComponentBase {
		public override ComponentData Data {
			get { return _data; }
		}

		public AIDialogData _data;

		protected override void DisposeData() { }

		public void Query() { }
		public void Dispose() { }
		public void Initialize() { }
	}

	[Serializable]
	public class AIDialogData : ComponentData {
		public TMP_InputField queryField;
		public TMP_Text answerField;

		public Button submit;
		public string botName;
		public string botIdentity;

		public UnityEvent OnInitialize;
		public UnityEvent OnQuestionAsked;
		public UnityEvent OnAnswerRecieved;
		public UnityEvent OnDispose;
	}
}
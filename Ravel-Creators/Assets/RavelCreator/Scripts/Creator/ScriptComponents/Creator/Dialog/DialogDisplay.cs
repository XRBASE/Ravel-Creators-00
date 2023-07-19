using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	[AddComponentMenu("Ravel/Dialog/Dialog display")]
	public partial class DialogDisplay : MonoBehaviour
	{
		public bool HasHeaderReference {
			get { return _header != null; }
		}

		public bool HasBodyReference {
			get { return _body != null; }
		}

		[SerializeField] private TMP_Text _header;
		[SerializeField] private TMP_Text _body;
		[SerializeField, Tooltip("When set, these graphics will change colors, based on the colors added in the messages.")] 
		private List<Graphic> _colorHighlights;

		public void NextMessage() { }
		public void PrevMessage() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(DialogDisplay))]
		private class DialogDisplayEditor : Editor
		{

			public override void OnInspectorGUI() {
				DialogDisplay instance = (DialogDisplay)target;

				DrawDefaultInspector();
				if (instance._header == null) {
					EditorGUILayout.HelpBox("Cannot display header messages if header field is not set!", MessageType.Warning);
				}
				
				if (instance._body == null) {
					EditorGUILayout.HelpBox("Body field required for showing messages using dialog", MessageType.Error);
				}
			}
		}
#endif
	}
}
using System;
using System.Collections.Generic;
using Base.Ravel.UI.Hints;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Adds oe or multiple hints to the hint panel when the ShowHints method is called.
	/// </summary>
	[AddComponentMenu("Ravel/Hint")]
	[HelpURL("https://www.notion.so/thenewbase/Hint-f83ec49d3de346cfb69f724483201b9d")]
	public partial class HintComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}
		[SerializeField] private HintData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Activates the hint(s) and places them in the list of shown hints.
		/// </summary>
		public void ShowHints() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(HintComponent))]
		private class HintComponentEditor : Editor
		{
			private HintComponent _instance;
			
			private void OnEnable() {
				_instance = (HintComponent)target;
			}

			public override void OnInspectorGUI() {
				DrawDefaultInspector();

				//Creates and selects a new hint scriptable object, which is automatically added to the hint list.
				if (GUILayout.Button("Create new hint") && Hint.CreateAndSaveHintEditor(out Hint hint)) {
					_instance._data.hints.Add(hint);
					EditorUtility.SetDirty(_instance);
				}
			}
		}
#endif
	}
	
	[Serializable]
	public class HintData : ComponentData
	{
		public bool showOnAwake;
		public List<Hint> hints = new List<Hint>();
	}
}
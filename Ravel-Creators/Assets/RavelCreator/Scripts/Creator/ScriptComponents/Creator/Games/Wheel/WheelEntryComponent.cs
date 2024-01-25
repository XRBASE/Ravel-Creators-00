using System;
using Base.Ravel.Creator.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class WheelEntryComponent : ComponentBase
{
	public override ComponentData Data {
		get { return _data; }
	}
	[SerializeField] private WheelEntryData _data;

	protected override void DisposeData() { }

	[Serializable]
	public class WheelEntryData : ComponentData
	{
		[Tooltip("This will display the segment number of the wheel, matching this entry")]
		public TMP_Text segmentNumberField;
		[Tooltip("In this field the player will be able to fill in a name matching the entry")]
		public TMP_InputField nameInputField;
		[Tooltip("This button will remove the entry")]
		public Button removeButton;
	}
}

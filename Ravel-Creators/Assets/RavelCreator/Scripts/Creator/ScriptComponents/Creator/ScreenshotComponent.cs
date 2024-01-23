using System;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
	[AddComponentMenu("Ravel/Screenshot downloader")]
	public partial class ScreenshotComponent : ComponentBase
	{
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField] private ScreenshotComponentData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Cut pixels between defined anchor points (based on current viewpoint) and donwload the result as a jpg file.
		/// </summary>
		public void DownloadScreenshot() { }
	}

	[Serializable]
	public class ScreenshotComponentData : ComponentData
	{
		[Tooltip("The initial filename of the file, user can still change it.")]
		public string fileName = "ravel-screenshot";
		[Tooltip("Lower right corner of the printscreen.")]
		public Transform sizeMin;
		[Tooltip("Upper left corner of the printscreen.")]
		public Transform sizeMax;
		public UnityEvent beforeScreenshot, afterScreenshot;
	}
}
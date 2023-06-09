using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Ravel.BackendData.DynamicContent
{
	/// <summary>
	/// Used for serializing all content info of one environment
	/// </summary>
	[Serializable]
	public class DynamicContentEnvDescription
	{
		public List<DynamicContentEnvEntry> content2D;
		public List<DynamicContentEnvEntry> content3D;

		public DynamicContentEnvDescription() {
			content2D = new List<DynamicContentEnvEntry>();
			content3D = new List<DynamicContentEnvEntry>();
		}

		public void Add2DEntry(string uniqueName, DynamicContentMetaData metaData) {
			content2D.Add(new DynamicContentEnvEntry(uniqueName, metaData));
		}
		
		public void Add3DEntry(string uniqueName, DynamicContentMetaData metaData) {
			content3D.Add(new DynamicContentEnvEntry(uniqueName, metaData));
		}
	}
	
	/// <summary>
	/// Used for serializing one component, using content.
	/// </summary>
	[Serializable]
	public class DynamicContentEnvEntry
	{
		public string uniqueName;
		public DynamicContentMetaData metaData;

		public DynamicContentEnvEntry(string uniqueName, DynamicContentMetaData metaData) {
			this.uniqueName = uniqueName;
			this.metaData = metaData;
		}
	}
	
	/// <summary>
	/// Used for serializing all content info of one environment
	/// </summary>
	[Serializable]
	public class DynamicContentSpaceDescription
	{
		public List<DynamicContentSpaceEntry> content2D;
		public List<DynamicContentSpaceEntry> content3D;

		public DynamicContentSpaceDescription() {
			content2D = new List<DynamicContentSpaceEntry>();
			content3D = new List<DynamicContentSpaceEntry>();
		}

		public void Add2DEntry(string uniqueName, string url, DynamicContentMetaData metaData) {
			content2D.Add(new DynamicContentSpaceEntry(uniqueName, url, metaData));
		}
		
		public void Add3DEntry(string uniqueName, string url, DynamicContentMetaData metaData) {
			content3D.Add(new DynamicContentSpaceEntry(uniqueName, url, metaData));
		}
	}
	
	/// <summary>
	/// Used for serializing one component, using content.
	/// </summary>
	[Serializable]
	public class DynamicContentSpaceEntry
	{
		public bool HasFile {
			get { return !string.IsNullOrEmpty(downloadUrl); }
		}

		public string uniqueName;
		public string downloadUrl;
		public string fileName;
		public DynamicContentMetaData metaData;

		public DynamicContentSpaceEntry(string uniqueName, string url, DynamicContentMetaData metaData) {
			this.uniqueName = uniqueName;
			this.metaData = metaData;
		}
	}
	
}
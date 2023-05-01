#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Base.Ravel.Creator.Components;
using Base.Ravel.Networking;
using Unity.VisualScripting;
using UnityEngine;

public static class FileManagement
{
	public static bool FileContentNameAvailable(string name, FileContentComponent owner) {
		FileContentComponent[] components = GameObject.FindObjectsOfType<FileContentComponent>(true);
		for (int i = 0; i < components.Length; i++) {
			if (components[i] == owner)
				continue;
			if (owner.Type == components[i].Type && name == components[i].Name) {
				return false;
			}
		}

		return true;
	}

	public static string GetDynamicContentJson() {
		FileContentComponent[] components = GameObject.FindObjectsOfType<FileContentComponent>(true);
		ContentData data = new ContentData(components);
		
		return JsonUtility.ToJson(data);
	}

	[Serializable]
	private class ContentData
	{
		[SerializeField] private List<ContentEntry> content2D;
		[SerializeField] private List<ContentEntry> content3D;
		
		public ContentData(FileContentComponent[] components) {
			content2D = new List<ContentEntry>();
			content3D = new List<ContentEntry>();

			for (int i = 0; i < components.Length; i++) {
				ContentEntry entry = new ContentEntry(components[i]);
				switch (entry.type) {
					case FileContentData.Type.Screen_2D:
						content2D.Add(entry);
						break;
					case FileContentData.Type.Model_3D:
						content3D.Add(entry);
						break;
					default:
						throw new Exception($"Missing type exception, missing content type: ({entry.type})");
				}
			}
		}
		
		[Serializable]
		private class ContentEntry
		{
			[DoNotSerialize] public FileContentData.Type type;
			
			public string uniqueName;
			public FileContentComponent.FileComponentMetaData metaData;
			
			public ContentEntry(FileContentComponent component) {
				type = component.Type;
				uniqueName = component.Name;
				metaData = component.MetaData;
			}
		}
	}
}
#endif
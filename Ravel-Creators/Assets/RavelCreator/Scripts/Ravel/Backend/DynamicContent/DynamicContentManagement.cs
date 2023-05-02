#if UNITY_EDITOR
using System;
using Base.Ravel.Creator.Components;
using UnityEngine;

namespace Base.Ravel.BackendData.DynamicContent
{
	public static class DynamicContentManagement
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

		public static bool TryGetDynamicContentJson(out string json) {
			FileContentComponent[] components = GameObject.FindObjectsOfType<FileContentComponent>(true);
			
			DynamicContentEnvDescription description = new DynamicContentEnvDescription();
			for (int i = 0; i < components.Length; i++) {
				if (string.IsNullOrEmpty(components[i].Name))
					continue;
				
				switch (components[i].Type) {
					case FileContentData.Type.Screen_2D:
						description.Add2DEntry(components[i].Name, components[i].MetaData);
						break;
					case FileContentData.Type.Model_3D:
						description.Add3DEntry(components[i].Name, components[i].MetaData);
						break;
					default:
						throw new Exception(
							$"Missing type exception for dynamic content. Missing type {components[i].Type}");
				}
			}

			json = JsonUtility.ToJson(description);
			return components.Length > 0;
		}
	}
}
#endif
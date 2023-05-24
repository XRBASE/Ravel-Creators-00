#if UNITY_EDITOR
using System;
using Base.Ravel.Creator.Components;
using UnityEngine;

namespace Base.Ravel.BackendData.DynamicContent
{
	/// <summary>
	/// Creator tools for dynamic content to enable usage of components in editor too.
	/// </summary>
	public static class DynamicContentManagement
	{
		/// <summary>
		/// Checks current scene for dynamic content holders, and checks whether given name has already been used.
		/// </summary>
		/// <param name="name">Name to check other content holders against.</param>
		/// <param name="owner">owner component, so this component can be skipped.</param>
		/// <returns>True/False can the currently given name, be used for the owner component?</returns>
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

		/// <summary>
		/// Tries to extract a content json string out of the currently open scene, which contains the dynamic content.
		/// Returns false if there is no content at all and the entries should just be cleared. 
		/// </summary>
		/// <param name="json">content string output.</param>
		/// <returns>True/False scene contained any dynamic content</returns>
		/// <exception cref="Exception">Missing type of content.</exception>
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
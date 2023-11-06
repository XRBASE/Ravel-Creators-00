using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Config file for saving bundle preferences in the editor cache
/// </summary>
[Serializable]
public class EditorBundles
{
	/// <summary>
	/// List of local environment bundle data.
	/// </summary>
	public List<BundleData> bundles;

	public EditorBundles() {
		bundles = new List<BundleData>();
	}

	/// <summary>
	/// Save this configuration in the editor cache.
	/// </summary>
	public void SaveConfig() {
		RavelCreatorSettings.Get().SaveBundleConfig(this);
	}
	
	/// <summary>
	/// Loads the current config file from the editor cache.
	/// </summary>
	public static EditorBundles LoadConfig() {
		return RavelCreatorSettings.Get().GetBundleConfig();
	}

	/// <summary>
	/// Update local values by fetching the cache values and comparing them to the environments in the project. Create new
	/// bundles for new environments and remove bundles that are not in the project anymore.
	/// </summary>
	public void UpdateValues() {
		//go through all local assets and check if bundles match. Create missing bundles and save id's of bundles that
		//have been identified in the project
		EnvironmentSO[] envs = RavelEditor.GetAllAssetsOfType<EnvironmentSO>();
		bool found = false;
		List<int> foundIds = new List<int>();
		for (int i = 0; i < envs.Length; i++) {
			if (envs[i].environment == null) {
				continue;
			}

			found = false;
			for (int j = 0; j < bundles.Count; j++) {
				if (envs[i].environment.environmentUuid == bundles[j].guid) {
					if (!string.IsNullOrEmpty(envs[i].bundleName)) {
						bundles[j].name = envs[i].bundleName;
					}
					else if (!string.IsNullOrEmpty(bundles[j].name)) {
						envs[i].bundleName = bundles[j].name;
						EditorUtility.SetDirty(envs[i]);
					}

					found = true;
					foundIds.Add(j);
					break;
				}
			}

			if (!found) {
				foundIds.Add(bundles.Count);
				if (string.IsNullOrEmpty(envs[i].bundleName) && !string.IsNullOrEmpty(envs[i].environment.name)) {
					envs[i].bundleName = envs[i].environment.name;
					EditorUtility.SetDirty(envs[i]);
				}

				bundles.Add(new BundleData(envs[i].bundleName, envs[i].environment.environmentUuid, 0, 0));
			}
		}

		//remove any id's that are not in the project anymore
		for (int i = 0; i < bundles.Count; i++) {
			if (!foundIds.Contains(i)) {
				bundles.RemoveAt(i);
			}
		}

		AssetDatabase.SaveAssets();
	}

	/// <summary>
	/// Find related bundle data for an environment object.
	/// </summary>
	/// <param name="env">environment object for which the data is being retrieved (or created).</param>
	public BundleData GetBundleData(EnvironmentSO env) {
		for (int i = 0; i < bundles.Count; i++) {
			if (bundles[i].guid == env.environment.environmentUuid) {
				return bundles[i];
			}
		}

		if (string.IsNullOrEmpty(env.bundleName) && !string.IsNullOrEmpty(env.environment.name)) {
			env.bundleName = env.environment.name;
			EditorUtility.SetDirty(env);
		}

		bundles.Add(new BundleData(env.bundleName, env.environment.environmentUuid, 0, 0));
		return bundles[^1];
	}
	
	/// <summary>
	/// Create a version postfix string that matches the given bundle data.
	/// </summary>
	/// <param name="bundle">bundle of which the version number is used.</param>
	/// <returns>String containing version numbering formatted according to the configuration.</returns>
	public string GetVersionString(BundleData bundle) {
		return GetVersionString(bundle.vMajor, bundle.vMinor);
	}

	/// <summary>
	/// Create a version postfix string that matches the given version numbers.
	/// </summary>
	/// <param name="major">Major version number to include.</param>
	/// <param name="minor">Minor version number to include.</param>
	/// <returns>String containing version numbering formatted according to the configuration.</returns>
	public string GetVersionString(int major, int minor) {
		if (string.IsNullOrEmpty(RavelEditor.CreatorPanelSettings.versioning)) {
			//no versioning
			return "";
		}

		string version = RavelEditor.CreatorPanelSettings.versioning;
		if (version.Contains('1')) {
			version = version.Replace("1", major.ToString());
		}

		if (version.Contains('2')) {
			version = version.Replace("2", minor.ToString());
		}

		return version;
	}

	/// <summary>
	/// Data class for saving data about the environments bundles. All this data is environment specific and local to the project. 
	/// </summary>
	[Serializable]
	public class BundleData
	{
		public string name;
		public string guid;
		public int vMajor;
		public int vMinor;

		public BundleData(string name, string guid, int vMajor, int vMinor) {
			this.name = name;
			this.guid = guid;
			this.vMajor = vMajor;
			this.vMinor = vMinor;
		}
	}
}

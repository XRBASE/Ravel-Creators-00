using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;

/// <summary>
/// Config file for saving bundle preferences in the editor cache
/// </summary>
[Serializable]
public class BundleConfig
{
	private const string CONFIG_KEY = "BUND_CFG";

	public List<BundleData> bundles;

	public BundleConfig() {
		bundles = new List<BundleData>();
	}

	public void SaveConfig() {
		EditorCache.SetString(CONFIG_KEY, JsonConvert.SerializeObject(this));
	}

	public void UpdateValues() {
		string json = EditorCache.GetString(CONFIG_KEY);
		if (!string.IsNullOrEmpty(json)) {
			//default values
			bundles.Clear();
			JsonConvert.PopulateObject(json, this);
		}

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

	public static BundleConfig LoadCurrent() {
		string json = EditorCache.GetString(CONFIG_KEY);
		if (string.IsNullOrEmpty(json)) {
			//default values
			return new BundleConfig();
		}

		return JsonConvert.DeserializeObject<BundleConfig>(json);
	}

	public string GetVersionString(BundleData bundle) {
		return GetVersionString(bundle.vMajor, bundle.vMinor);
	}

	public string GetVersionString(int major, int minor) {
		if (string.IsNullOrEmpty(RavelEditor.CreatorConfig.versioning)) {
			//no versioning
			return "";
		}

		string version = RavelEditor.CreatorConfig.versioning;
		if (version.Contains('1')) {
			version = version.Replace("1", major.ToString());
		}

		if (version.Contains('2')) {
			version = version.Replace("2", minor.ToString());
		}

		return version;
	}

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

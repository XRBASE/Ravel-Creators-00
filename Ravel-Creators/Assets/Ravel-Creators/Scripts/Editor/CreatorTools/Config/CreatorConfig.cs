using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Config file for saving creator preferences in the editor cache
/// </summary>
[Serializable]
public class CreatorConfig
{
	private const string CONFIG_KEY = "CREATOR_CONFIG";
	public static readonly string DEFAULT_FILE_PATH = Application.dataPath;
	public static readonly string DEFAULT_BUNDLE_PATH = Application.dataPath + "/StreamingAssets/";

	public bool saveUserMail = false;
	public string userMail;
	
	public string bundlePath;
	public string prevFilePath;

	public CreatorConfig() {
		saveUserMail = false;
		userMail = "";

		bundlePath = DEFAULT_BUNDLE_PATH;
		prevFilePath = DEFAULT_FILE_PATH;
	}

	public string GetFilePath() {
		if (string.IsNullOrEmpty(prevFilePath)) {
			return DEFAULT_FILE_PATH;
		}
		else {
			return prevFilePath;
		}
	}

	public void SetFilePath(string path) {
		prevFilePath = path;
	}
	
	public void SaveConfig() {
		EditorCache.SetString(CONFIG_KEY, JsonConvert.SerializeObject(this));
	}
	
	public static CreatorConfig LoadCurrent() {
		string json = EditorCache.GetString(CONFIG_KEY);
		if (string.IsNullOrEmpty(json)) {
			//default values
			return new CreatorConfig();
		}
		
		return JsonConvert.DeserializeObject<CreatorConfig>(json);
	}
}

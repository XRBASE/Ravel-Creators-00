using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Config file for saving creator preferences in the editor cache. These are all configurations that are project wide and not environment specific.
/// </summary>
[Serializable]
public class CreatorConfig
{
	//saved under this key in editor prefs.
	private const string CONFIG_KEY = "CREA_CFG";
	
	//default location for files and bundles, these paths are cached on use, so the user won't have to select the same folder every time.
	private static readonly string DEFAULT_FILE_PATH = Application.dataPath;
	private static readonly string DEFAULT_BUNDLE_PATH = Application.dataPath + "/StreamingAssets/";

	//Should the e-mail address be cached, so you never have to type it again?
	public bool saveUserMail = false;
	//cache data for the users e-mail address
	public string userMail;
	
	//cache for the previously used filepath, so new save prompts can be opened on this location
	public string prevFilePath;
	
	//the used and cached asset-bundle output location
	public string bundlePath;
	//should asset-bundle data be deleted automatically after building the bundle.
	public bool autoClean;
	//string that defines how the version extension to the file name will be build up.
	//		A 1 in this string is replaced by the major number and a 2 by the minor one.
	public string versioning;
	//This bool will add one to the minor version, each time an environment is build.
	public bool incrementMinorVersionOnBuild;

	//Creates a default configuration.
	public CreatorConfig() {
		saveUserMail = false;
		userMail = "";

		bundlePath = DEFAULT_BUNDLE_PATH;
		prevFilePath = DEFAULT_FILE_PATH;

		incrementMinorVersionOnBuild = true;
		versioning = "_1_2";
		autoClean = false;
	}
	
	/// <summary>
	/// Save this configuration to the editor cache.
	/// </summary>
	public void SaveConfig() {
		EditorCache.SetString(CONFIG_KEY, JsonConvert.SerializeObject(this));
	}
	
	/// <summary>
	/// Loads the currently cached configuration.
	/// </summary>
	public static CreatorConfig LoadCurrent() {
		string json = EditorCache.GetString(CONFIG_KEY);
		if (string.IsNullOrEmpty(json)) {
			//default values
			return new CreatorConfig();
		}
		
		return JsonConvert.DeserializeObject<CreatorConfig>(json);
	}

	/// <summary>
	/// Returns the currently cached file path.
	/// </summary>
	public string GetFilePath() {
		if (string.IsNullOrEmpty(prevFilePath)) {
			return DEFAULT_FILE_PATH;
		}
		else {
			return prevFilePath;
		}
	}

	/// <summary>
	/// Callback to cache a filepath, so it will be used in the next file prompt.
	/// </summary>
	public void SetFilePath(string path) {
		prevFilePath = path;
	}
}

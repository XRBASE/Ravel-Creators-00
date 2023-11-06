using System;

/// <summary>
/// Config file for saving creator preferences in the editor cache. These are all configurations that are project wide and not environment specific.
/// </summary>
[Serializable]
public class CreatorPanelSettings
{
	//Should the e-mail address be cached, so you never have to type it again?
	public bool saveUserMail = false;
	//cache data for the users e-mail address
	public string userMail;
	
	//should asset-bundle data be deleted automatically after building the bundle.
	public bool autoClean;
	//string that defines how the version extension to the file name will be build up.
	//		A 1 in this string is replaced by the major number and a 2 by the minor one.
	public string versioning;
	//This bool will add one to the minor version, each time an environment is build.
	public bool incrementMinorVersionOnBuild;

	//Creates a default configuration.
	public CreatorPanelSettings() {
		saveUserMail = false;
		userMail = "";

		incrementMinorVersionOnBuild = true;
		versioning = "_1_2";
		autoClean = false;
	}
	
	/// <summary>
	/// Save this configuration to the editor cache.
	/// </summary>
	public void SaveConfig() {
		RavelCreatorSettings.Get().SaveCreatorConfig(this);
	}

	public static CreatorPanelSettings LoadConfig() {
		return RavelCreatorSettings.Get().GetCreatorConfig();
	}
}

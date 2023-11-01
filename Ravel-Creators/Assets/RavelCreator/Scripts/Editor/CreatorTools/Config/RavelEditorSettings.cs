using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/Config/EditorSettings", fileName = EDITOR_SETTINGS_NAME, order = 0)]
public class RavelEditorSettings : ScriptableObject
{
	private const string EDITOR_SETTINGS_PATH = "/RavelCreator/Resources/";
	private const string EDITOR_SETTINGS_NAME = "RavelEditorSettings";
	
	/// <summary>
	/// Load configuration from the resources folder.
	/// </summary>
	public static RavelEditorSettings Load()
	{
		RavelEditorSettings cfg = Resources.Load<RavelEditorSettings>("Config/" + EDITOR_SETTINGS_NAME);
		if (!cfg) {
			string path = Application.dataPath + EDITOR_SETTINGS_PATH;
			cfg = ScriptableObject.CreateInstance<RavelEditorSettings>();
			
			AssetDatabase.CreateAsset(cfg, path + EDITOR_SETTINGS_NAME + ".asset");
			AssetDatabase.Refresh();
		}
		
		return cfg;
	}
	
	public CreatorConfig _panelConfiguration;
}

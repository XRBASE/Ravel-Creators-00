using System.IO;
using MathBuddy.Strings;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/Config/EditorSettings", fileName = EDITOR_SETTINGS_NAME, order = 0)]
public class RavelEditorSettings : ScriptableObject
{
	private const string EDITOR_SETTINGS_PATH = "Assets/RavelCreator/Resources/Config/";
	private const string EDITOR_SETTINGS_NAME = "RavelEditorSettings";
	
	private static readonly string DEFAULT_FILE_PATH = "";
	private static readonly string DEFAULT_BUNDLE_PATH = "/StreamingAssets/";

	private static RavelEditorSettings _instance;

	[SerializeField] private string _filePath;
	[SerializeField] private string _bundlePath;
	
	/// <summary>
	/// Load configuration from the resources folder.
	/// </summary>
	public static RavelEditorSettings Get()
	{
		if (!_instance) {
			//load from resource folder
			_instance = Resources.Load<RavelEditorSettings>("Config/" + EDITOR_SETTINGS_NAME);
			if (!_instance) {
				//create in resource folder.
				string path = EDITOR_SETTINGS_PATH;
				_instance = CreateInstance<RavelEditorSettings>();
				_instance.OnCreate();

				AssetDatabase.CreateAsset(_instance, path + EDITOR_SETTINGS_NAME + ".asset");
				AssetDatabase.Refresh();
			}
		}

		return _instance;
	}

	private void OnCreate() {
		_filePath = GetFilePath();
		_bundlePath = GetBundlePath();
	}
	
	[SerializeField] private CreatorConfig _panelConfig;
	[SerializeField] private BundleConfig _bundleConfig;

	public void SaveCreatorConfig(CreatorConfig config) {
		_panelConfig = config;
	}

	public CreatorConfig GetCreatorConfig() {
		return _panelConfig;
	}

	public void SaveBundleConfig(BundleConfig config) {
		_bundleConfig = config;
	}

	public BundleConfig GetBundleConfig() {
		return _bundleConfig;
	}

	private bool IsPathInProject(string path) {
		return path.IsSubpathOf(Application.dataPath);
	}

	public void SetFilePath(string path) {
		if (IsPathInProject(path)) {
			if (File.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}
			
			_filePath = path;
		}
	}

	public void SetBundlePath(string path) {
		if (IsPathInProject(path)) {
			if (File.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}
			
			_bundlePath = path;
		}
	}

	public string GetFilePath() {
		if (string.IsNullOrEmpty(_filePath)) {
			_filePath = Application.dataPath + DEFAULT_FILE_PATH;
		}

		return _filePath;
	}

	public string GetBundlePath() {
		if (string.IsNullOrEmpty(_filePath)) {
			_bundlePath = Application.dataPath + DEFAULT_BUNDLE_PATH;
		}

		return _bundlePath;
	}
}

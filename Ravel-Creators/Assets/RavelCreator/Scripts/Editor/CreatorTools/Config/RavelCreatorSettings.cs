using System.IO;
using MathBuddy.Strings;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/Config/EditorSettings", fileName = EDITOR_SETTINGS_NAME, order = 0)]
public partial class RavelCreatorSettings : ScriptableObject
{
	private const string EDITOR_SETTINGS_PATH = "Assets/RavelCreator/Resources/Config/";
	private const string EDITOR_SETTINGS_NAME = "RavelEditorSettings";
	
	private static readonly string DEFAULT_FILE_PATH = "";
	private static readonly string DEFAULT_BUNDLE_PATH = "/StreamingAssets/";

	private static RavelCreatorSettings _instance;

	[SerializeField] private CreatorPanelSettings _creatorPanelSettings;
	[SerializeField] private EditorBundles _editorBundles;
	
	[SerializeField, HideInInspector] private string _filePath;
	[SerializeField, HideInInspector] private string _bundlePath;

	/// <summary>
	/// Load configuration from the resources folder.
	/// </summary>
	public static RavelCreatorSettings Get()
	{
		if (!_instance) {
			//load from resource folder
			_instance = Resources.Load<RavelCreatorSettings>("Config/" + EDITOR_SETTINGS_NAME);
			if (!_instance) {
				//create in resource folder.
				string path = EDITOR_SETTINGS_PATH;
				_instance = CreateInstance<RavelCreatorSettings>();
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

	public void SaveCreatorConfig(CreatorPanelSettings panelSettings) {
		_creatorPanelSettings = panelSettings;
		EditorUtility.SetDirty(this);
	}

	public CreatorPanelSettings GetCreatorConfig() {
		return _creatorPanelSettings;
	}

	public void SaveBundleConfig(EditorBundles config) {
		_editorBundles = config;
		EditorUtility.SetDirty(this);
	}

	public EditorBundles GetBundleConfig() {
		return _editorBundles;
	}

	private bool IsPathInProject(string path) {
		return path.IsSubpathOf(Application.dataPath);
	}

	public void SetFilePath(string path) {
		if (IsPathInProject(path)) {
			if (File.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}

			if (_filePath != path) {
				_filePath = path;
				EditorUtility.SetDirty(this);
			}
		}
		else {
			Debug.LogWarning("Cannot set file path outside of project!");
		}
	}

	public void SetBundlePath(string path) {
		if (IsPathInProject(path)) {
			if (File.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}

			if (_bundlePath != path) {
				_bundlePath = path;
				EditorUtility.SetDirty(this);
			}
		}
		else {
			Debug.LogWarning("Cannot set file path outside of project!");
		}
	}

	public string GetFilePath() {
		if (string.IsNullOrEmpty(_filePath)) {
			_filePath = Application.dataPath + DEFAULT_FILE_PATH;
		}

		return _filePath;
	}

	public string GetBundlePath() {
		if (string.IsNullOrEmpty(_bundlePath)) {
			_bundlePath = Application.dataPath + DEFAULT_BUNDLE_PATH;
		}

		return _bundlePath;
	}
}

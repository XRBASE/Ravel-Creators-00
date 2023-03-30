using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base.Ravel.LogList;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BundleBuilder
{
	private static LogList _logs;

	[MenuItem("Ravel/AssetBundles/Clear references", false)]
	private static void ClearBundles() {
		ClearAllBundles();
	}
	
	[MenuItem("Ravel/AssetBundles/Delete bundles", false)]
	private static void DeleteAllBundles() {
		DeleteBundles();
	}
	
	public static void BuildOpenScene(string bundleName, bool autoCleanFiles) {
		if (_logs == null) {
			_logs = new LogList();
		}
		else {
			_logs.Clear();
		}

		Scene s = EditorSceneManager.GetActiveScene();
		if (EditorSceneManager.loadedSceneCount > 1) {
			if (EditorUtility.DisplayDialog("Build open scene",
				    $"There are mutliple scene's open, only the active scene ({s.name}), will actually be built into the bundle",
				    "Build", "Cancel")) {
				//unload all other scene's
				EditorSceneManager.OpenScene(s.path, OpenSceneMode.Single);
				_logs.AddLog($"Multiple scenes, building active scene ({s.name})", Log.LogType.Warning);

				s = EditorSceneManager.GetActiveScene();
			}
			else {
				_logs.AddLog($"Multiple scenes, cancelling build.", Log.LogType.Error);
				return;
			}
		}

		if (!TryGetConfig(out SceneConfiguration config)) {
			return;
		}

		if (string.IsNullOrEmpty(bundleName)) {
			if (string.IsNullOrEmpty(config.environmentSO.bundleName)) {
				bundleName = Guid.NewGuid().ToString();
				autoCleanFiles = true;
			}
			else {
				bundleName = config.environmentSO.bundleName;
			}
		}

		BundleConfig.BundleData data = RavelEditor.BundleConfig.GetBundleData(config.environmentSO);
		bundleName += RavelEditor.BundleConfig.GetVersionString(data);
		
		if (s.isDirty) {
			EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { s });
		}

		if (!EditorUtility.DisplayDialog("Build confirmation",
			    $"Building scene {s.name} and uploading it to environment " +
			    $"{config.environmentSO.environment.name}, this will override the previous assetbundle. Are you sure?",
			    "Yes", "Cancel build")) {
			return;
		}
		
		Debug.Log($"Build scene {s.name} for environment {config.environmentSO.environment.name}");
		string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
		string[] assets;
		bool sceneAdded = false;
		foreach (string name in bundleNames) {
			assets = AssetDatabase.GetAssetPathsFromAssetBundle(name);
			foreach (string assetPath in assets) {
				if (name == bundleName && assetPath == s.path) {
					sceneAdded = true;
					continue;
				}
				AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant("", "");
			}
			if (name != bundleName)
				AssetDatabase.RemoveAssetBundleName(name, true);
		}
		if (!sceneAdded) {
			AssetImporter.GetAtPath(s.path).SetAssetBundleNameAndVariant(bundleName, "");
		}

		string path = RavelEditor.CreatorConfig.bundlePath;
		if (!Directory.Exists(path)) {
			_logs.AddLog($"Bundle directory {path} does not exist, creating it!", Log.LogType.Warning);
			Directory.CreateDirectory(path);
		}
		var assetBundleManifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None,
			BuildTarget.WebGL);
		
		if(RavelEditor.CreatorConfig.incrementMinorVersionOnBuild){
			data.vMinor++;
			RavelEditor.BundleConfig.SaveConfig();
		}

		_logs.AddLog($"Uploading bundle!", Log.LogType.Log);
		RavelWebRequest req = CreatorRequest.UploadBundle(config.environmentSO.environment.environmentUuid,
			Path.Combine(path, bundleName));

		//only send delete action along if auto cleanup is enabled
		if (autoCleanFiles) {
			EditorWebRequests.SendWebRequest(req, 
				(res) => OnBundleUploaded(res, config.environmentSO.environment, () => DeleteBundle(path + bundleName)), config);
		}
		else {
			EditorWebRequests.SendWebRequest(req, 
				(res) => OnBundleUploaded(res, config.environmentSO.environment, null), config);
		}
		if (File.Exists(path + "/StreamingAssets")) {
			DeleteBundle(path + "/StreamingAssets");
		}
	}

	private static void OnBundleUploaded(RavelWebResponse res, Environment env, Action onComplete) {
		if (res.Success) {
			_logs.AddLog($"Opening preview in browser!", Log.LogType.Log);
			Application.OpenURL(CreatorRequest.GetPreviewUrl(env));
		}
		else {
			_logs.AddLog($"Error uploading build {res.Error.FullMessage}!", Log.LogType.Error);
		}
		onComplete?.Invoke();
	}

	private static void DeleteBundle(string bundlePath) {
		Debug.Log($"Deleting bundle at path {bundlePath}");
		
		File.Delete(bundlePath);
		File.Delete(bundlePath + ".manifest");
		
		if (File.Exists(bundlePath + ".meta")) {
			File.Delete(bundlePath + ".meta");
		}
		
		if (File.Exists(bundlePath + ".manifest.meta")) {
			File.Delete(bundlePath + ".manifest.meta");
		}
	}

	public static void DeleteBundles() {
		List<string> files = Directory.GetFiles(RavelEditor.CreatorConfig.bundlePath).ToList();
		if (files.Count == 0) {
			Debug.LogWarning($"Cannot delete bundles, no files found at {RavelEditor.CreatorConfig.bundlePath}");
			return;
		}
		
		List<string> kill = new List<string>();

		string name;
		string bundlesDisplay = "";
		for (int i = 0; i < files.Count; i++) {
			name = files[i];

			//only kill files without extention, with a manifest file, and kill the meta files too
			if (Path.GetExtension(name) == "" && files.Contains(name + ".manifest")) {
				kill.Add(name);
				bundlesDisplay += $"{name},";
			}
		}

		if (kill.Count == 0) {
			//no bundles found
			Debug.LogWarning($"Cannot delete bundles, no bundles found at {RavelEditor.CreatorConfig.bundlePath}");
			return;
		}
		//remove last comma
		bundlesDisplay = bundlesDisplay.Substring(0, bundlesDisplay.Length - 1);

		if (!EditorUtility.DisplayDialog("Deleting all bundles",
			    $"Are you sure you want to delete the following files: \n {bundlesDisplay}?", "Yes", "No")) {
			return;
		}

		for (int i = 0; i < kill.Count; i++) {
			File.Delete(kill[i]);
		}
		AssetDatabase.Refresh();
	}

	public static void ClearAllBundles() {
		string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
		string[] assets;
		
		foreach (string name in bundleNames) {
			assets = AssetDatabase.GetAssetPathsFromAssetBundle(name);
			foreach (string path in assets) {
				AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("", "");
			}
			AssetDatabase.RemoveAssetBundleName(name, true);
		}
	}

	private static bool TryGetConfig(out SceneConfiguration config) {
		SceneConfiguration[] configs = GameObject.FindObjectsOfType<SceneConfiguration>();
		if (configs.Length > 1) {
			if (!EditorUtility.DisplayDialog("Multiple configs",
				    $"Found multiple configuration files in the scene, this is not supported...\n" +
				    $"Continue building for {configs[0].environmentSO.environment.name}?", "Continue", "Cancel")) {

				_logs.AddLog("Multiple configs, cancelling build", Log.LogType.Error);
				
				config = null;
				return false;
			}

			_logs.AddLog($"Multiple configs, continue with environment {configs[0].environmentSO.environment.name}",
				Log.LogType.Warning);
		} else if (configs.Length == 0) {
			if (SceneConfiguration.ShowNoConfigDialog() != null) {
				_logs.AddLog("Created new configuration object in scene.", Log.LogType.Log, false);
			}
			_logs.AddLog($"No config in scene, cancelling build.", Log.LogType.Error);
			
			config = null;
			return false;
		}
		
		config = configs[0];
		if (config.environmentSO == null) {
			EditorUtility.DisplayDialog("No environment",
				$"No environment file found in the scene configuration! Please add and environment to the configuration.", "Ok");
			_logs.AddLog($"No environment in config, cancelling build.", Log.LogType.Error);

			Selection.activeObject = config;
			return false;
		}
		
		return true;
	}
}

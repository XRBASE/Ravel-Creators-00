using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base.Ravel.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Owner class of all asset-bundle build related code.
/// </summary>
public static class BundleBuilder
{
	/// <summary>
	/// This call clears all the internal references to asset-bundles within unity, without deleting files
	/// </summary>
	[MenuItem("Ravel/AssetBundles/Clear references", false)]
	private static void ClearBundles() {
		ClearAllBundleReferences();
	}
	
	[MenuItem("Ravel/AssetBundles/Delete bundles", false)]
	private static void DeleteAllBundles() {
		if (EditorUtility.DisplayDialog("Delete bundles",
			    "This will delete all asset-bundles in the bundle folder, are you sure?", "Yes", "No")) {
			DeleteBundles();
		}
	}
	
	/// <summary>
	/// Creates an asset-bundle build of the open scene, uploads it to the matching environment and opens the preview link.
	/// </summary>
	/// <param name="bundleName">Name of asset-bundle that is being built.</param>
	/// <param name="autoCleanFiles">"Should the created bundle files be deleted after building?"</param>
	public static void BuildOpenScene(string bundleName, bool preview, bool autoCleanFiles) {
		Scene s = EditorSceneManager.GetActiveScene();
		
		//check for multiple scenes (if so, only the active one will be build).
		if (EditorSceneManager.loadedSceneCount > 1) {
			if (EditorUtility.DisplayDialog("Build open scene",
				    $"There are multiple scene's open, only the active scene ({s.name}), will actually be built into the bundle",
				    "Build", "Cancel")) {
				//unload all other scene's
				EditorSceneManager.OpenScene(s.path, OpenSceneMode.Single);
				Debug.LogWarning($"Multiple scenes, building active scene ({s.name})");

				s = EditorSceneManager.GetActiveScene();
			}
			else {
				Debug.LogError("Multiple scenes, cancelling build.");
				return;
			}
		}

		//See if there is an active scene configuration in the scene, otherwise cancel the build.
		if (!TryGetConfig(out SceneConfiguration config)) {
			return;
		}

		BundleConfig.BundleData data = RavelEditor.BundleConfig.GetBundleData(config.environmentSO);
		
		//Check if the buil name was set, if not try to set the name that is set in the environment, if even that fails, 
		//assign a guid to the bundle as name. GUID builds are always auto cleaned up.
		if (string.IsNullOrEmpty(bundleName)) {
			if (string.IsNullOrEmpty(config.environmentSO.bundleName)) {
				bundleName = config.environmentSO.environment.name + Guid.NewGuid().ToString();
				autoCleanFiles = true;
				
				//no version set, if GUID is added.
			}
			else {
				bundleName = config.environmentSO.bundleName;
				bundleName += RavelEditor.BundleConfig.GetVersionString(data);
			}
		}
		else {
			bundleName += RavelEditor.BundleConfig.GetVersionString(data);
		}
		
		//If scene contains changes, ask the creator if she wants to save them.
		if (s.isDirty) {
			EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { s });
		}

		string buildMsg;
		if (preview) {
			buildMsg = $"Building scene {s.name} and uploading it to environment " +
			           $"{config.environmentSO.environment.name}, this will override the previous assetbundle. Are you sure?";
		}
		else {
			buildMsg = $"Building scene {s.name} in folder {RavelEditor.CreatorConfig.bundlePath}, are you sure?";
		}
		
		//Last chance for user to cancel. This dialog shows what scene is assigned into what environment.
		if (!EditorUtility.DisplayDialog("Build confirmation", buildMsg, "Yes", "Cancel build")) {
			return;
		}
		
		//Other asset-bundles are being cleared in preparation of the bundle build.
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
		
		//If scene was not already assigned to the right bundle, assign the scene to it.
		if (!sceneAdded) {
			AssetImporter.GetAtPath(s.path).SetAssetBundleNameAndVariant(bundleName, "");
		}

		//Get the bundle path from the config and create missing folders if they're not already there
		string path = RavelEditor.CreatorConfig.bundlePath;
		if (!Directory.Exists(path)) {
			Debug.LogWarning($"Bundle directory {path} does not exist, creating it!");
			Directory.CreateDirectory(path);
		}
		
		//build the actual bundle
		var assetBundleManifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None,
			BuildTarget.WebGL);
		
		//increment version numbers.
		if(RavelEditor.CreatorConfig.incrementMinorVersionOnBuild){
			data.vMinor++;
			RavelEditor.BundleConfig.SaveConfig();
		}

		if (preview) {
			//send result to webserver.
			Debug.Log("Uploading bundle!");
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
		}
		
		//always delete the streaming assets bundle
		if (File.Exists(path + "/StreamingAssets")) {
			DeleteBundle(path + "/StreamingAssets");
		}

		if (!autoCleanFiles) {
			AssetDatabase.Refresh();
		}
	}

	/// <summary>
	/// Called as result of the bundle upload. When the bundle has been successfully uploaded, this opens the preview URL.
	/// </summary>
	/// <param name="res">web response of the serven.</param>
	/// <param name="env">environment that the build was pushed to.</param>
	/// <param name="onComplete">callback for when the build has opened.</param>
	private static void OnBundleUploaded(RavelWebResponse res, Environment env, Action onComplete) {
		if (res.Success) {
			Debug.Log("Opening preview in browser!");
			Application.OpenURL(CreatorRequest.GetPreviewUrl(env));
		}
		else {
			Debug.LogError($"Error uploading build {res.Error.FullMessage}!");
		}
		onComplete?.Invoke();
	}

	/// <summary>
	/// Deletes a builds files from the given path. This includes the meta files and the manifest files. 
	/// </summary>
	/// <param name="bundlePath">path to the bundle file, including the bundles filename.</param>
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

	/// <summary>
	/// Deletes all bundles from the build file path, set in the configuration. Bundles are recognized by their manifest files,
	/// and so this call will only delete files without extension that have another file with the manifest extension.
	/// </summary>
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

	/// <summary>
	/// Clears all references used in unity to any asset-bundle.
	/// </summary>
	public static void ClearAllBundleReferences() {
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

	/// <summary>
	/// Tries to get a scene configuration object from the currently active scene.
	/// </summary>
	/// <param name="config">The configuration will be outputted into this reference.</param>
	/// <returns>True/False, configuration file was found and contains an environment.</returns>
	private static bool TryGetConfig(out SceneConfiguration config) {
		SceneConfiguration[] configs = GameObject.FindObjectsOfType<SceneConfiguration>();
		if (configs.Length > 1) {
			if (!EditorUtility.DisplayDialog("Multiple configs",
				    $"Found multiple configuration files in the scene, this is not supported...\n" +
				    $"Continue building for {configs[0].environmentSO.environment.name}?", "Continue", "Cancel")) {
				Debug.LogError("Multiple configs, cancelling build");
				
				config = null;
				return false;
			}

			Debug.LogWarning($"Multiple configs, continue with environment {configs[0].environmentSO.environment.name}");
		} else if (configs.Length == 0) {
			if (SceneConfiguration.ShowNoConfigDialog() != null) {
				Debug.Log("Created new configuration object in scene.");
			}
			Debug.LogError("No config in scene, cancelling build.");
			
			config = null;
			return false;
		}
		
		config = configs[0];
		if (config.environmentSO == null) {
			EditorUtility.DisplayDialog("No environment",
				$"No environment file found in the scene configuration! Please add and environment to the configuration.", "Ok");
			Debug.LogError("No environment in config, cancelling build.");

			Selection.activeObject = config;
			return false;
		}
		
		return true;
	}
}

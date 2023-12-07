using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base.Ravel.BackendData.DynamicContent;
using Base.Ravel.Config;
using Base.Ravel.Creator.Components;
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
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
		
		EditorCoroutineUtility.StartCoroutine(BuildScene(s, bundleName, preview, autoCleanFiles), s);
	}

	private static IEnumerator BuildScene(Scene s, string bundleName, bool preview, bool autoCleanFiles) {
		//See if there is an active scene configuration in the scene, otherwise cancel the build.
		//Missing config errors are shown to the user.
		if (!TryGetConfig(out SceneConfiguration config)) {
			Debug.LogWarning("Could not find scene configuration component! Cancelling build.");
			yield break;
		}

		if (config.environmentSO == null) {
			EditorUtility.DisplayDialog("Missing environment!",
				"This scene requires an environment to which the build can be uploaded, please link the environment " +
				"asset to the scene configuration object.",
				"Ok");

			Selection.activeObject = config;
			yield break;
		}

		if (config.environmentSO.environment.mode != NetworkConfig.AppMode.Unknown &&
		    config.environmentSO.environment.mode != AppConfig.Networking.Mode) {
			EditorUtility.DisplayDialog("Appmode mismatch!",
				$"You are trying to build an {config.environmentSO.environment.mode}, but the currently selected mode" +
				$"is {AppConfig.Networking.Mode}. Cancelling build.",
				"Ok");
			
			yield break;
		}
		
		if (config.environmentSO.environment.published) {
			EditorUtility.DisplayDialog("Environment already published",
				"Cannot rebuild environments that have already published. Please link another environment to this scene.",
				"Ok");
			
			yield break;
		}
		
		
		Camera[] cams = GameObject.FindObjectsOfType<Camera>();
		List<int> camInstIds = new List<int>();
		for (int i = 0; i < cams.Length; i++) {
			if (cams[i].gameObject.activeSelf) {
				cams[i].gameObject.SetActive(false);
				camInstIds.Add(cams[i].GetInstanceID());
			}
		}

		//this data contains the version numbering of the bundle, as saved in editor prefs.
		EditorBundles.BundleData data = RavelEditor.EditorBundles.GetBundleData(config.environmentSO);
		
		//Check if the build name was set, if not try to set the name that is set in the environment, if even that fails, 
		//assign a guid to the bundle as name. GUID builds are always auto cleaned up.
		if (string.IsNullOrEmpty(bundleName)) {
			if (string.IsNullOrEmpty(config.environmentSO.bundleName)) {
				bundleName = config.environmentSO.environment.name + Guid.NewGuid().ToString();
				autoCleanFiles = true;
				
				//no version set, if GUID is added.
			}
			else {
				bundleName = config.environmentSO.bundleName;
				bundleName += RavelEditor.EditorBundles.GetVersionString(data);
			}
		}
		else {
			bundleName += RavelEditor.EditorBundles.GetVersionString(data);
		}
		
		//If scene contains changes, ask the creator if she wants to save them.
		if (s.isDirty && !EditorUtility.DisplayDialog("Unsaved changes in scene",
				    "The scene contains unsaved changed, but the build progress requires saving. If you don't want to save the scene, the preview will be cancelled",
				    "Save", "Don't save")) {
				yield break;
		}
		//Sets the id's for all networked components
		IDProvider.SetSceneIDs();

		//clear current dynamic content
		RavelWebRequest req = DynamicContentRequest.ClearDynamicContentRequest(config.environmentSO.environment);
		yield return req.Send();
		
		RavelWebResponse res = new RavelWebResponse(req);
		if (!res.Success) {
			EditorUtility.DisplayDialog("Error clearing dynamic content",
				$"Could not clear previously used dynamic content: \n{res.Error.FullMessage}!", "Ok");
			
			throw new Exception($"Could not clear previously used dynamic content: \n{res.Error.FullMessage}!");
		}
		
		//Upload file content to backend, using names in scene.
		if (DynamicContentManagement.TryGetDynamicContentJson(out string json)) {
			req = DynamicContentRequest.AddDynamicContentRequest(config.environmentSO.environment, json);
			yield return req.Send();
			
			res = new RavelWebResponse(req);
			if (!res.Success) {
				EditorUtility.DisplayDialog("Error setting dynamic content",
					$"There was an error setting the dynamic content: \n{res.Error.FullMessage}!", "Ok");
				
				throw new Exception($"Dynamic content error: \n{res.Error.FullMessage}!");
			}
		}
		
		//always save, but cancel build if dynamic content fails 
		EditorSceneManager.SaveScene(s);
		
		//Dialog for what is being build and last change to cancel it.
		string dialogMsg;
		if (preview) {
			dialogMsg = $"Building scene {s.name} and uploading it to environment " +
			           $"{config.environmentSO.environment.name}, this will override the previous assetbundle. Are you sure?";
		}
		else {
			dialogMsg = $"Building scene {s.name} in folder {RavelCreatorSettings.Get().GetBundlePath()}, are you sure?";
		}
		
		//Last chance for user to cancel. This dialog shows what scene is assigned into what environment.
		if (!EditorUtility.DisplayDialog("Build confirmation", dialogMsg, "Yes", "Cancel build")) {
			yield break;
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
		string path = RavelCreatorSettings.Get().GetBundlePath();
		if (!Directory.Exists(path)) {
			Debug.LogWarning($"Bundle directory {path} does not exist, creating it!");
			Directory.CreateDirectory(path);
		}
		
		//build the actual bundle
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.WebGL);
		
		//increment version numbers.
		if(RavelEditor.CreatorPanelSettings.incrementMinorVersionOnBuild){
			data.vMinor++;
			RavelEditor.EditorBundles.SaveConfig();
		}

		if (preview) {
			//send result to webserver.
			Debug.Log("Uploading bundle!");
			req = CreatorRequest.UploadBundle(config.environmentSO.environment.environmentUuid,
				Path.Combine(path, bundleName));

			yield return req.Send();
			res = new RavelWebResponse(req);

			if (!res.Success) {
				Debug.LogError($"Error uploading bundle: {res.Error.FullMessage}");
			}

			Debug.Log("Opening preview in browser!");
			Application.OpenURL(CreatorRequest.GetPreviewUrl(config.environmentSO.environment));
			
			if (autoCleanFiles) {
				DeleteBundle(path + bundleName);
			}
		}
		
		//always delete the streaming assets bundle
		if (File.Exists(path + "/StreamingAssets")) {
			DeleteBundle(path + "/StreamingAssets");
		}

		if (!autoCleanFiles) {
			AssetDatabase.Refresh();
		}
		
		cams = GameObject.FindObjectsOfType<Camera>(true);
		for (int i = 0; i < cams.Length; i++) {
			if (camInstIds.Contains(cams[i].GetInstanceID())) {
				cams[i].gameObject.SetActive(true);
			}
		}
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
		List<string> files = Directory.GetFiles(RavelCreatorSettings.Get().GetBundlePath()).ToList();
		if (files.Count == 0) {
			Debug.LogWarning($"Cannot delete bundles, no files found at {RavelCreatorSettings.Get().GetBundlePath()}");
			return;
		}
		
		List<string> toDelete = new List<string>();

		string name;
		string bundlesDisplay = "";
		for (int i = 0; i < files.Count; i++) {
			name = files[i];

			//only delete files without extention, with a manifest file, and delete the meta files too
			if (Path.GetExtension(name) == "" && files.Contains(name + ".manifest")) {
				toDelete.Add(name);
				bundlesDisplay += $"{name},";
			}
		}

		if (toDelete.Count == 0) {
			//no bundles found
			Debug.LogWarning($"Cannot delete bundles, no bundles found at {RavelCreatorSettings.Get().GetBundlePath()}");
			return;
		}
		//remove last comma
		bundlesDisplay = bundlesDisplay.Substring(0, bundlesDisplay.Length - 1);

		if (!EditorUtility.DisplayDialog("Deleting all bundles",
			    $"Are you sure you want to delete the following files: \n {bundlesDisplay}?", "Yes", "No")) {
			return;
		}

		for (int i = 0; i < toDelete.Count; i++) {
			File.Delete(toDelete[i]);
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

using Base.Ravel.LogList;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BundleBuilder
{
	private static LogList _logs;
	
	public static void BuildOpenScene() {
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
		
		Debug.Log($"Build scene {s.name} for environment {config.environmentSO.environment.name}");
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
			
			_logs.AddLog($"Multiple configs, continue with environment {configs[0].environmentSO.environment.name}", Log.LogType.Warning);
		} else if (configs.Length == 0) {
			EditorUtility.DisplayDialog("No config",
				$"No configuration file found (in the scene)! Please fill in the configuration to the scene.", "Ok");
			
			_logs.AddLog($"No config in scene, cancelling build.", Log.LogType.Error);
			
			GameObject configObject = new GameObject("Scene Configuration");
			configObject.AddComponent<SceneConfiguration>();
			Selection.activeObject = configObject;
			
			_logs.AddLog($"No config in scene, created and selected config object in scene.", Log.LogType.Log);
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

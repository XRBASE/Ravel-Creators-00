using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// In-scene configuration to bind a scene to a specific environment.
/// Can be re-used for specific movement settings too.
/// </summary>
public class SceneConfiguration : MonoBehaviour
{
    public EnvironmentSO environmentSO;
    
#if UNITY_EDITOR
    /// <summary>
    /// Dialog that is shown to the user in cases where the configuration is needed, but not found. Also offers an option
    /// To auto create the config for the user, though it will only work when an environment is actually set.
    /// </summary>
    public static SceneConfiguration ShowNoConfigDialog() {
        if (EditorUtility.DisplayDialog("No config",
                $"No configuration file found in scene! Do you want to create a new scene configuration?.",
                "Yes", "No")) {
				
            GameObject configObject = new GameObject("SceneConfig");
            Selection.activeObject = configObject;

            Debug.Log("Created new configuration object in scene.");
            return configObject.AddComponent<SceneConfiguration>();
        }

        return null;
    }
    
#endif
}

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneConfiguration : MonoBehaviour
{
    public EnvironmentSO environmentSO;
    
#if UNITY_EDITOR
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

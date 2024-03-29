using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

/// <summary>
/// In-scene configuration to bind a scene to a specific environment.
/// Can be re-used for specific movement settings too.
/// </summary>
public class SceneConfiguration : MonoBehaviour
{
    [HideInInspector] public EnvironmentSO environmentSO;
    public bool teleportEnabled = true;
    
#if UNITY_EDITOR

    public void OnValidate() {
        SceneConfiguration[] configs = FindObjectsOfType<SceneConfiguration>();

        if (configs.Length > 1 && EditorUtility.DisplayDialog("Double config",
                "There is already a scene configuration in the scene. No more than one config is allowed per scene!",
                "ok")) {

            for (int i = 0; i < configs.Length; i++) {
                if (configs[i] != this) {
                    EditorGUIUtility.PingObject(configs[i].gameObject);
                }
            }

            Debug.LogError(
                "There is already a scene configuration in the scene. No more than one config is allowed per scene!");

            //destroy component when allowed to. 
            EditorApplication.delayCall += () => { DestroyImmediate(this); };
        }
    }

    public static Action environmentUpdated;
    
    [CustomEditor(typeof(SceneConfiguration))]
    private class SceneConfigurationEditor : Editor
    {
        public override void OnInspectorGUI() {
            SceneConfiguration instance = (SceneConfiguration)target;

            DrawDefaultInspector();
            EditorGUI.BeginChangeCheck();
            instance.environmentSO = EditorGUILayout.ObjectField("Environment:", instance.environmentSO, typeof(EnvironmentSO), false) as EnvironmentSO;
            if (EditorGUI.EndChangeCheck()) {
                environmentUpdated?.Invoke();
            }
        }
    }
    
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

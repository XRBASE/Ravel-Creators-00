using UnityEditor;

/// <summary>
/// Cache adapter for retrieving and setting data, so we can remove the playerprefs at some point.
/// </summary>
public static class EditorCache
{
    /// <summary>
    /// Set string in playerprefs under key.
    /// </summary>
    public static void SetString(string key, string value) {
        EditorPrefs.SetString(key, value);
    }

    /// <summary>
    /// Get string from playerprefs key and return it.
    /// </summary>
    public static string GetString(string key) {
        return EditorPrefs.GetString(key);
    }

    /// <summary>
    /// Check if string with key exists, if so out that value and return true. 
    /// </summary>
    public static bool TryGetString(string key, out string value) {
        if (EditorPrefs.HasKey(key)) {
            value = EditorPrefs.GetString(key);
            return true;
        }
        else {
            value = "";
            return false;
        }
    }

    /// <summary>
    /// Set int in playerprefs under key.
    /// </summary>
    public static void SetInt(string key, int value) {
        EditorPrefs.SetInt(key, value);
    }

    /// <summary>
    /// Get int from playerprefs key and return it.
    /// </summary>
    public static int GetInt(string key) {
        return EditorPrefs.GetInt(key);
    }

    /// <summary>
    /// Check if int with key exists, if so out that value and return true.
    /// </summary>
    public static bool TryGetInt(string key, out int value) {
        if (EditorPrefs.HasKey(key)) {
            value = EditorPrefs.GetInt(key);
            return true;
        }
        else {
            value = 0;
            return false;
        }
    }

    /// <summary>
    /// Set float in playerprefs under key.
    /// </summary>
    public static void SetFloat(string key, float value) {
        EditorPrefs.SetFloat(key, value);
    }

    /// <summary>
    /// Get float from playerprefs key and return it.
    /// </summary>
    public static float GetFloat(string key) {
        return EditorPrefs.GetFloat(key);
    }

    /// <summary>
    /// Check if float with key exists, if so out that value and return true.
    /// </summary>
    public static bool TryGetFloat(string key, out float value) {
        if (EditorPrefs.HasKey(key)) {
            value = EditorPrefs.GetFloat(key);
            return true;
        }
        else {
            value = 0f;
            return false;
        }
    }

    /// <summary>
    /// Delete entry that is saved under key.
    /// </summary>
    public static void Delete(string key) {
        EditorPrefs.DeleteKey(key);
    }

    /// <summary>
    /// Clear all preferences.
    /// </summary>
    public static void Clear() {
        EditorPrefs.DeleteAll();
    }
}
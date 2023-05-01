using System.Collections.Generic;
using Base.Ravel.Networking.Authorization;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Cache adapter for retrieving and setting data, so we can remove the playerprefs at some point.
/// </summary>
public static class EditorCache
{
    //even when clearing all playercache data, do not clear the token (as it will destroy webrequest behaviours)
    private static readonly KeyData[] NEVER_REMOVE_KEYS = new[] { new KeyData(LoginRequest.SYSTEMS_TOKEN_KEY, KeyData.DataType.String) };
    
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

    public static void Clear() {
        List<object> doNotRemoveData = new List<object>();
        foreach (var key in NEVER_REMOVE_KEYS) {
            doNotRemoveData.Add(key.GetData());
        }
        
        Debug.Log("Editor cache cleared");
        EditorPrefs.DeleteAll();

        for (int i = 0; i < NEVER_REMOVE_KEYS.Length; i++) {
            NEVER_REMOVE_KEYS[i].SetData(doNotRemoveData[i]);
        }
    }

    private struct KeyData
    {
        public string key;
        public DataType type;

        public KeyData(string key, DataType type) {
            this.key = key;
            this.type = type;
        }

        public object GetData() {
            switch (type) {
                case KeyData.DataType.Int:
                    return GetInt(key);
                case KeyData.DataType.Float:
                    return GetFloat(key);
                case KeyData.DataType.String:
                    return GetString(key);
                default:
                    return null;
            }
        }
        
        public void SetData(object value) {
            switch (type) {
                case DataType.Int:
                    SetInt(key, (int) value);
                    break;
                case DataType.Float:
                    SetFloat(key, (float)value);
                    break;
                case DataType.String:
                    SetString(key, (string)value);
                    break;
            }
        }

        public enum DataType
        {
            Int,
            Float,
            String
        }
    }
}
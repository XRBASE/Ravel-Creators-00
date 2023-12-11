using System;
using System.Collections.Generic;
using System.Linq;
using Base.Ravel.Config;
using Base.Ravel.CustomAttributes;
using Base.Ravel.Users;
using UnityEngine;

/// <summary>
/// Data class for environments, this data is pulled from the server and put into a scriptable object as container class.
/// </summary>
[Serializable]
public class Environment
{
    [ReadOnly]
    public string environmentUuid;
    
    [ReadOnly]
    public string name;
    [ReadOnly] 
    public string shortSummary;
    [ReadOnly]
    public string longSummary;

    //track whether it is a development or live environment.
    [ReadOnly]
    public NetworkConfig.AppMode mode = NetworkConfig.AppMode.Unknown;
    
    //container class for all size image urls.
    [HideInInspector]
    public ImageSizeUrls metadataPreviewImage;
    [HideInInspector]
    public Assetbundle metadataAssetBundle;
    
    //this variable is called public in the backend and is renamed when the string is downloaded, because public is a bad
    //variable name.
    [ReadOnly] 
    public bool isPublic;
    [ReadOnly] 
    public bool published;
    [HideInInspector] 
    public bool submissionInProgress;

    [HideInInspector] public User[] userList;
    [HideInInspector] public Organisation[] organizations;

    //caching data for sprites, so they don't have to be downloaded so often for local environments.
    [HideInInspector] public ImageSize previewSize = ImageSize.None;
    [HideInInspector] public Sprite preview;
    
    /// <summary>
    /// Called when sprite is downloaded caches image if it is bigger than the previous version.
    /// </summary>
    /// <param name="spr">sprite that was downloaded.</param>
    /// <param name="size">size of the sprite</param>
    public void UpdateEnvironmentSprite(Sprite spr, ImageSize size) {
        if (size >= previewSize) {
            preview = spr;
            previewSize = size;
        }
    }
}

/// <summary>
/// Data class for saving the assetbundle url, so it can be serialized into environment.
/// </summary>
[Serializable]
public class Assetbundle
{
    public string assetBundleUrl;
}

/// <summary>
/// Extension calls for the environment class, contains usefull data calls. 
/// </summary>
public static class EnvironmentExtensions
{
    /// <summary>
    /// Returns string array with the names of the given set of environments.
    /// </summary>
    public static string[] GetNames(this IEnumerable<Environment> data) {
        Environment[] envs = data as Environment[] ?? data.ToArray();
        if (envs.Length == 0) {
            return Array.Empty<string>();
        }
        string[] output = new string[envs.Length];

        for (int i = 0; i < envs.Length; i++) {
            output[i] = envs[i].name;
        }

        return output;
    }
    
    /// <summary>
    /// Renames the good variable name into the bad one, so it can be pushed back into the backend (isPublic changes into public).
    /// </summary>
    /// <param name="json">json string of one or more environments.</param>
    public static string RenameStringToBackend(string json) {
        return json.Replace("\"isPublic\":", "\"public\":");
    }
    
    /// <summary>
    /// Renames the bad variable name into the good one, so normal developers can use it (public changes into isPublic). 
    /// </summary>
    /// <param name="json">json string of one or more environments.</param>
    public static string RenameStringFromBackend(string json) {
        return json.Replace("\"public\":", "\"isPublic\":");
    }
}
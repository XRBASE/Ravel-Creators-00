using System;
using System.Collections.Generic;
using System.Linq;
using Base.Ravel.CustomAttributes;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Environment
{
    [ReadOnly]
    public string environmentUuid;
    
    [ReadOnly]
    public new string name;
    [ReadOnly] 
    public string shortSummary;
    [ReadOnly]
    public string longSummary;
    
    [HideInInspector]
    public ImageSizeUrls metadataPreviewImage;
    [HideInInspector]
    public Assetbundle metadataAssetBundle;
    
    [ReadOnly] 
    public bool isPublic;
    [ReadOnly] 
    public bool published;
}

public static class EnvironmentExtensions
{
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

    public static string RenameStringToBackend(string json) {
        return json.Replace("\"isPublic\":", "\"public\":");
    }
    
    public static string RenameStringFromBackend(string json) {
        return json.Replace("\"public\":", "\"isPublic\":");
    }
}

[Serializable]
public class Assetbundle
{
    public string assetBundleUrl;
}
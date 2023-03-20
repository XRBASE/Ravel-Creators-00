using System;
using System.Collections.Generic;
using System.Linq;
using Base.Ravel.CustomAttributes;

[Serializable]
public class Environment
{
    [ReadOnly]
    public string environmentUuid;
    [ReadOnly]
    public new string name;
    [ReadOnly] 
    public string description;
    [ReadOnly] 
    public string imageUrl;
    [ReadOnly] 
    public string assetBundleUrl;
    [ReadOnly] 
    public bool active;
    [ReadOnly] 
    public bool publicEnvironment;
}

public static class EnvironmentExtentions
{
    public static string[] GetNames(this IEnumerable<Environment> data) {
        Environment[] envs = data as Environment[] ?? data.ToArray();
        string[] output = new string[envs.Length];

        for (int i = 0; i < envs.Length; i++) {
            output[i] = envs[i].name;
        }

        return output;
    }
}

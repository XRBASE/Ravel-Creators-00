using UnityEngine;
#if UNITY_EDITOR
using Base.Ravel.Networking;
using UnityEditor;
#endif

/// <summary>
/// Scriptable object container for environments, so the backend data can be saved into the project as file. Also contains
/// the GUI code for environments.
/// </summary>
public class EnvironmentSO : ScriptableObject
{
    public Environment environment;
}

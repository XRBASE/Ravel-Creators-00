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
    

#if UNITY_EDITOR

    //cache for network errors in retrieving environment, used for showing the error to users.
    [HideInInspector] public string networkError = "";
    
    /// <summary>
    /// Keep status up to date.
    /// </summary>
    private void OnEnable() {
        if (environment != null) {
            RefreshEnvironment();
        }
    }
    
    /// <summary>
    /// Deletes the scriptable object reference.
    /// </summary>
    public void DeleteLocalAsset() {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// Make call to backend to refresh this environment.
    /// </summary>
    public void RefreshEnvironment() {
        RavelWebRequest req = CreatorRequest.GetCreatorEnvironment(environment.environmentUuid);
        EditorWebRequests.SendWebRequest(req, OnEnvironmentUpdateReceived, this);
    }

    /// <summary>
    /// Callback for when new environment data has been recieved from the server. 
    /// </summary>
    /// <param name="res">Server response data.</param>
    private void OnEnvironmentUpdateReceived(RavelWebResponse res) {
        if (res.Success) {
            res.DataString = EnvironmentExtensions.RenameStringFromBackend(res.DataString);

            if (res.TryGetData(out Environment env)) {
                environment = env;
            }

            RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(environment.environmentUuid);
            EditorWebRequests.GetDataRequest<Environment>(req, UpdateEnvironmentAccess, this);
        }
        else {
            networkError = res.Error.FullMessage;
            Debug.LogError($"Error refreshing environment {environment.name}");
        }
    }
    
    /// <summary>
    /// Callback from the server when updating the access to a private environment.
    /// </summary>
    /// <param name="res">WebResponse data</param>
    public void OnAccessUpdateReceived(RavelWebResponse res) {
        if (res.Success) {
            RavelWebRequest req = EnvironmentAccessRequest.GetEnvironmentAccessData(environment.environmentUuid);
            EditorWebRequests.GetDataRequest<Environment>(req, UpdateEnvironmentAccess, this);
        }
    }
    
    /// <summary>
    /// Update user or organisation access to this environment.
    /// </summary>
    /// <param name="toCopy">Environment containing the updated acces</param>
    /// <param name="success"></param>
    private void UpdateEnvironmentAccess(Environment toCopy, bool success) {
        //error in webcall
        if (!success)
            return;
        
        environment.userList = toCopy.userList;
        environment.organizations = toCopy.organizations;
    }
#endif
}

using System;
using System.Collections;
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

public static class EditorWebRequests
{
    public static void SendWebRequest(RavelWebRequest req, Action<RavelWebResponse> onReqSent, object sender) {
        EditorCoroutineUtility.StartCoroutine(SendRequest(req, onReqSent), sender);
    }

    private static IEnumerator SendRequest(RavelWebRequest req, Action<RavelWebResponse> onReqSent) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (!res.Success) {
            //TODO: unauthorized logout (401)
            Debug.LogError($"Webresponse Error: {res.Error.FullMessage}).");
        }
        
        req.DisposeData();
        onReqSent?.Invoke(res);
    }

    public static void DownloadAndSave(RavelWebRequest req, string path, bool select, object sender) {
        EditorCoroutineUtility.StartCoroutine(DownloadAndSaveRequest(req, path, select), sender);
    }

    private static IEnumerator DownloadAndSaveRequest(RavelWebRequest req, string path, bool select) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (!res.Success) {
            Debug.Log($"Error downloading file: ({req.Request.url}): {res.Error.FullMessage}).");
            yield break;
        }

        byte[] bytes = res.DataByte;
        File.WriteAllBytes(path, bytes);
        
        req.DisposeData();
        if (path.Contains("Assets")) {
            AssetDatabase.Refresh();

            if (select) {
                path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            }
        }
    }
    
    /// <summary>
    /// Send webrequest to retrieve single instance of data from the backend.
    /// </summary>
    /// <param name="req">webrequest that requests the data</param>
    /// <param name="dataRetrieved">callback for when request is made, boolean is false when webrequest failed.</param>
    /// <param name="sender">send object to run the coroutine on.</param>
    public static void GetDataRequest<T>(RavelWebRequest req, Action<T, bool> dataRetrieved, object sender) {
        EditorCoroutineUtility.StartCoroutine(RetrieveData(req, dataRetrieved), sender);
    }
    
    /// <summary>
    /// Send webrequest to retrieve multiple instances of data from the backend.
    /// </summary>
    /// <param name="req">webrequest that requests the data</param>
    /// <param name="dataRetrieved">callback for when request is made, boolean is false when webrequest failed.</param>
    /// <param name="sender">send object to run the coroutine on.</param>
    public static void GetDataCollectionRequest<T>(RavelWebRequest req, Action<T[], bool> dataRetrieved, object sender) {
        EditorCoroutineUtility.StartCoroutine(RetrieveData(req, dataRetrieved), sender);
    }

    private static IEnumerator RetrieveData<T>(RavelWebRequest req, Action<T, bool> dataRetrieved) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (res.Success && res.TryGetData(out T data)) {
            dataRetrieved?.Invoke(data, true);
        }
        else {
            dataRetrieved?.Invoke(default, false);
        }
    }
    
    private static IEnumerator RetrieveData<T>(RavelWebRequest req, Action<T[], bool> dataRetrieved) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (res.Success && res.TryGetCollection(out ProxyCollection<T> data)) {
            dataRetrieved?.Invoke(data.Array, true);
        }
        else {
            dataRetrieved?.Invoke(Array.Empty<T>(), false);
        }
    }
}


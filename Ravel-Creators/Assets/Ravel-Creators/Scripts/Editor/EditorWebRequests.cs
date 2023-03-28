using System;
using System.Collections;
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

/// <summary>
/// Editor web request class for sending and retrieving webrequests.
/// </summary>
public static class EditorWebRequests
{
    /// <summary>
    /// Send given webrequest.
    /// </summary>
    /// <param name="req">Request to send.</param>
    /// <param name="onReqSent">Callback for when response is received.</param>
    /// <param name="sender">Sender object, this object runs the coroutine and the coroutine stops when the object is deleted.</param>
    public static void SendWebRequest(RavelWebRequest req, Action<RavelWebResponse> onReqSent, object sender) {
        EditorCoroutineUtility.StartCoroutine(SendRequest(req, onReqSent), sender);
    }

    /// <summary>
    /// IENumerator for running the webrequests. Sends request, puts it in a webresponse and fires the callback.
    /// </summary>
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

    /// <summary>
    /// Directly download file data from webrequest on local disk.
    /// </summary>
    /// <param name="req">WebRequest that returns the download data.</param>
    /// <param name="path">Path where the file should be saved.</param>
    /// <param name="select">Should the file be selected after download (only works for files in the project).</param>
    /// <param name="sender">Sender object, this object runs the coroutine and the coroutine stops when the object is deleted.</param>
    public static void DownloadAndSave(RavelWebRequest req, string path, bool select, object sender) {
        EditorCoroutineUtility.StartCoroutine(DownloadAndSaveRequest(req, path, select), sender);
    }

    /// <summary>
    /// IENumerator of the download file call.
    /// </summary>
    private static IEnumerator DownloadAndSaveRequest(RavelWebRequest req, string path, bool select) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (!res.Success) {
            Debug.LogError($"Error downloading file: ({req.Request.url}): {res.Error.FullMessage}).");
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
    /// <param name="sender">Sender object, this object runs the coroutine and the coroutine stops when the object is deleted.</param>
    public static void GetDataRequest<T>(RavelWebRequest req, Action<T, bool> dataRetrieved, object sender) {
        EditorCoroutineUtility.StartCoroutine(RetrieveData(req, dataRetrieved), sender);
    }
    
    /// <summary>
    /// Send webrequest to retrieve multiple instances of data from the backend.
    /// </summary>
    /// <param name="req">webrequest that requests the data</param>
    /// <param name="dataRetrieved">callback for when request is made, boolean is false when webrequest failed.</param>
    /// <param name="sender">Sender object, this object runs the coroutine and the coroutine stops when the object is deleted.</param>
    public static void GetDataCollectionRequest<T>(RavelWebRequest req, Action<T[], bool> dataRetrieved, object sender) {
        EditorCoroutineUtility.StartCoroutine(RetrieveData(req, dataRetrieved), sender);
    }

    /// <summary>
    /// IENumerator of the data (singular) retrieval calls.
    /// </summary>
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
    
    /// <summary>
    /// IENumerator of the data (collection) retrieval calls.
    /// </summary>
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


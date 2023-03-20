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
        
        if (select && path.Contains("Assets")) {
            AssetDatabase.Refresh();
            path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
        }
    }
}


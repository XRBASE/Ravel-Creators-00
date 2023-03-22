using System;
using System.Collections;
using Base.Ravel.Networking;
using Base.Ravel.Networking.Authorization;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public static class EditorWebRequests
{
    public static void SendWebRequest(RavelWebRequest req, Action<RavelWebResponse> onReqSent, object sender) {
        EditorCoroutineUtility.StartCoroutine(SendRequest(req, onReqSent), sender);
    }

    private static IEnumerator SendRequest(RavelWebRequest req, Action<RavelWebResponse> onReqSent) {
        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (!res.Success) {
            string token = PlayerCache.GetString(LoginRequest.SYSTEMS_TOKEN_KEY);
            Debug.LogError($"Webresponse Error: {res.Error.FullMessage}) ({token}).");
        }
        Debug.Log($"Webresponse ({req.Request.url}): Success:({res.Success}, {res.ResultCode}).");
        onReqSent?.Invoke(res);
    }
}


using System;
using System.Collections;
using Base.Ravel.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

/// <summary>
/// For not not connected to backend, download handler for images. Downloads images and stores them using given key
/// </summary>
public static class EditorImageService
{
    /// <summary>
    /// Used for image retrieval that can sometimes be cancelled, uses the routine, which cannot be async, but can be cancelled.
    /// </summary>
    /// <param name="url">Image url from which data is retrieved.</param>
    /// <param name="size">Size of the image that is retrieved, used in callback.</param>
    /// <param name="callback">Callback for when image has been retrieved.</param>
    /// <param name="owner">Owner class, object is used for Coroutine running, Coroutine also stops when this object is removed.</param>
    public static EditorCoroutine GetSpriteRoutine(string url, ImageSize size, Action<Sprite, ImageSize> callback,
                                                   object owner) {
        return EditorCoroutineUtility.StartCoroutine(RetrieveSprite(url, size, callback), owner);
    }

    /// <summary>
    /// IENumerator for sending the coroutine WebRequest.
    /// </summary>
    private static IEnumerator RetrieveSprite(string url, ImageSize size, Action<Sprite, ImageSize> callback) {
        SpriteRequest req = new SpriteRequest(url);

        yield return req.Send();

        RavelWebResponse res = new RavelWebResponse(req);
        if (res.Success && res.TryGetSprite(out Sprite result)) {
            callback?.Invoke(result, size);
        }
        else {
            Debug.LogError($"Error downloading sprite: {res.Error.FullMessage}.");
        }
    }

    /// <summary>
    /// Starts and returns the image download routine, for given url.
    /// </summary>
    /// <param name="url">url from which to retrieve the sprite</param>
    /// <param name="callback">callback called with created sprite</param>
    public static void GetSprite(string url, ImageSize size, Action<Sprite, ImageSize> callback) {
        if (string.IsNullOrWhiteSpace(url))
            return;

        SpriteRequest req = new SpriteRequest(url);
        AsyncOperation ao = req.Send();

        ao.completed += (operation) => OnSpriteRetrieved(req, size, callback);
    }

    /// <summary>
    /// Response callback for when the get request is completed (non-coroutine version only).
    /// </summary>
    private static void OnSpriteRetrieved(SpriteRequest req, ImageSize size, Action<Sprite, ImageSize> callback) {
        RavelWebResponse res = new RavelWebResponse(req);
        if (res.Success && res.TryGetSprite(out Sprite result)) {
            callback?.Invoke(result, size);
        }
        else {
            Debug.LogError($"Error downloading sprite: {res.Error.FullMessage}.");
        }
    }
}
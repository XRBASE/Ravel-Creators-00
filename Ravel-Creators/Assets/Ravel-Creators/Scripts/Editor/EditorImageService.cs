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

/// <summary>
/// All image size that can be used in the backend.
/// </summary>
public enum ImageSize
{
    None = 0,
    I256 = 1,
    I512 = 2,
    I1024 = 3,
    I1280 = 4,
    I1920 = 5,
}

/// <summary>
/// Struct to save the different sizing urls in for images.
/// </summary>
[Serializable]
public struct ImageSizeUrls
{
    //Set of urls for retrieving the specific images.
    public string url256,
        url512,
        url1024,
        url1280,
        url1920;

    /// <summary>
    /// Tries to retrieve the url of the image, based on the size.
    /// </summary>
    /// <param name="size">size of the image that needs to be downloaded.</param>
    /// <param name="url">url output if it is found.</param>
    /// <returns>True/False any url was found.</returns>
    public bool TryGetUrl(ImageSize size, out string url) {
        switch (size) {
            case ImageSize.I256:
                url = url256;
                break;
            case ImageSize.I512:
                url = url512;
                break;
            case ImageSize.I1024:
                url = url1024;
                break;
            case ImageSize.I1280:
                url = url1280;
                break;
            case ImageSize.I1920:
                url = url1920;
                break;
            default:
                url = "";
                return false;
        }

        return !string.IsNullOrEmpty(url);
    }
}
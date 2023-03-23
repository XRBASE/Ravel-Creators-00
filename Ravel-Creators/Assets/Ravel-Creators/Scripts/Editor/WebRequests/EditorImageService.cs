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
        public static EditorCoroutine GetSpriteRoutine(string url, ImageSize size, Action<Sprite, ImageSize> callback, object owner) {
            return EditorCoroutineUtility.StartCoroutine(RetrieveSprite(url, size, callback), owner);
        }

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
        public static void GetSprite(string url, ImageSize size, Action<Sprite, ImageSize> callback)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;
            
            SpriteRequest req = new SpriteRequest(url);
            AsyncOperation ao = req.Send();
            
            ao.completed += (operation) => OnSpriteRetrieved(req, size, callback);
        }

        private static void OnSpriteRetrieved(SpriteRequest req, ImageSize size, Action<Sprite, ImageSize> callback)
        {
            RavelWebResponse res = new RavelWebResponse(req);
            if (res.Success && res.TryGetSprite(out Sprite result)) {
                callback?.Invoke(result, size);
            }
            else {
                Debug.LogError($"Error downloading sprite: {res.Error.FullMessage}.");
            }
        }
    }
    
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
    public string url256,
        url512,
        url1024,
        url1280,
        url1920;

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
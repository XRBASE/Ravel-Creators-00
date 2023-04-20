using System;
using System.IO;
using Base.Ravel.Config;
using Base.Ravel.Networking;
using UnityEngine;
using File = UnityEngine.Windows.File;

/// <summary>
/// Creator webrequests shown on the API documentation under tab Environments.
/// </summary>
public class CreatorRequest : TokenWebRequest
{
    public CreatorRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version) {
        _url += "environments/" + postfix;
    }

    public CreatorRequest(Method method, string postfix, string data, string version) : base(method, "api/", version) {
        _url += "environments/" + postfix;
        _data = data;
    }

    public CreatorRequest(string postfix, WWWForm form, string version = "v1/") : base ("api/", version, form) {
        _url += "environments/" + postfix;
    }

    /// <summary>
    /// Creates a new environment on server.
    /// </summary>
    /// <param name="userUuid">UUID of the user that creates the environment.</param>
    /// <param name="env">environment which will be created (contains name, descriptions and isPublic)</param>
    public static CreatorRequest CreateEnvironment(string userUuid, Environment env) {
        string json = JsonUtility.ToJson(env);
        json = EnvironmentExtensions.RenameStringToBackend(json);
        
        return new CreatorRequest(Method.PostJSON, $"{userUuid}", json, "v1/");
    }
    
    /// <summary>
    /// Deletes pushed environment from the server (only works for unpublished environments).
    /// </summary>
    /// <param name="env">environment that has to be deleted.</param>
    public static CreatorRequest DeleteEnvironment(Environment env) {
        if (env.published) {
            throw new Exception("Cannot delete published environments");
        }
        return new CreatorRequest(Method.Delete, $"{env.environmentUuid}");
    }
    
    /// <summary>
    /// Publishes an environment (without review)
    /// </summary>
    /// <param name="envUuid">UUID of environment that will be pushed.</param>
    public static CreatorRequest PublishEnvironment(string envUuid) {
        return new CreatorRequest(Method.Put, $"publish/{envUuid}", "v2/");
    }
    
    /// <summary>
    /// Uploads a image (located at path) as preview for environment with given UUID.
    /// </summary>
    public static CreatorRequest UploadPreview(string envUuid, string imagePath) {
        WWWForm form = new WWWForm();
        string fName = Path.GetFileName(imagePath);
        form.AddBinaryData("image", File.ReadAllBytes(imagePath), fName);

        return new CreatorRequest($"uploads/preview-images?environmentUuid={envUuid}", form);
    }
    
    /// <summary>
    /// Uploads an assetbundle (located at path) as bundle for environment with given UUID.
    /// </summary>
    public static CreatorRequest UploadBundle(string envUuid, string bundlePath) {
        WWWForm form = new WWWForm();
        string fName = Path.GetFileName(bundlePath);
        form.AddBinaryData("file", File.ReadAllBytes(bundlePath), fName);

        return new CreatorRequest($"uploads/asset-bundles?environmentUuid={envUuid}", form);
    }
    
    /// <summary>
    /// Get all environments for a given creator.
    /// </summary>
    /// <param name="userUuid">user UUID of user.</param>
    /// <param name="isPublished">Should the published or unpublished environments be pushed.</param>
    public static CreatorRequest GetCreatorEnvironments(string userUuid, bool isPublished) {
        CreatorRequest req = new CreatorRequest(Method.Get, $"{userUuid}");
        req.AddParameter("isPublished", isPublished.ToString());
        return req;
    }
    
    /// <summary>
    /// Get one specific creator environment based on it's UUID
    /// </summary>
    public static CreatorRequest GetCreatorEnvironment(string envUuid) {
        return new CreatorRequest(Method.Get, $"single/{envUuid}");
    }

    public static string GetPreviewUrl(Environment env) {
        string url = AppConfig.Networking.SiteUrl;
        url += $"creators/environments/preview/{env.environmentUuid}";

        return url;
    }
}

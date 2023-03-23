using System;
using System.IO;
using Base.Ravel.Networking;
using Newtonsoft.Json;
using UnityEngine;
using File = UnityEngine.Windows.File;

public class CreatorRequest : TokenWebRequest
{
    public CreatorRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version) {
        _url += "environments/" + postfix;
    }

    public CreatorRequest(Method method, string postfix, string data, string version) : base(method, "api/",
        version) {
        _url += "environments/" + postfix;
        _data = data;
    }

    public CreatorRequest(string postfix, WWWForm form, string version = "v1/") : base ("api/", version, form) {
        _url += "environments/" + postfix;
    }

    public static CreatorRequest CreateEnvironment(string userUuid, Environment env) {
        string json = JsonConvert.SerializeObject(env);
        json = EnvironmentExtensions.RenameStringToBackend(json);
        
        return new CreatorRequest(Method.PostJSON, $"{userUuid}", json, "v1/");
    }
    
    public static CreatorRequest DeleteEnvironment(Environment env) {
        if (env.published) {
            throw new Exception("Cannot delete published environments");
        }
        return new CreatorRequest(Method.Delete, $"{env.environmentUuid}");
    }
    
    public static CreatorRequest PublishEnvironment(string envUuid) {
        return new CreatorRequest(Method.Put, $"submissions/{envUuid}", "v1/");
    }
    
    public static CreatorRequest UploadPreview(string envUuid, string imagePath) {
        WWWForm form = new WWWForm();
        string fName = Path.GetFileName(imagePath);
        form.AddBinaryData("image", File.ReadAllBytes(imagePath), fName);

        return new CreatorRequest($"uploads/preview-images?environmentUuid={envUuid}", form);
    }
    
    public static CreatorRequest UploadBundle(string envUuid, string bundlePath) {
        WWWForm form = new WWWForm();
        string fName = Path.GetFileName(bundlePath);
        form.AddBinaryData("file", File.ReadAllBytes(bundlePath), fName);

        return new CreatorRequest($"uploads/asset-bundles?environmentUuid={envUuid}", form);
    }
    
    public static CreatorRequest GetCreatorEnvironments(string userUuid, bool isPublished) {
        CreatorRequest req = new CreatorRequest(Method.Get, $"{userUuid}");
        req.AddParameter("isPublished", isPublished.ToString());
        return req;
    }
    
    public static CreatorRequest GetCreatorEnvironment(string envUuid) {
        return new CreatorRequest(Method.Get, $"single/{envUuid}");
    }
}

using System.Net;
using System.Text;
using Base.Ravel.Networking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Windows;

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

    public static CreatorRequest CreateEnvironment(string userUuid, Environment env) {
        string json = JsonConvert.SerializeObject(env);
        json = EnvironmentExtensions.RenameStringToBackend(json);
        
        return new CreatorRequest(Method.PostJSON, $"{userUuid}", json, "v1/");
    }
    
    public static CreatorRequest UploadPreview(string envUuid, string filePath, string extension) {
        string data = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
        
        CreatorRequest req = new CreatorRequest(Method.PostBytes, $"uploads/preview-images", data, "v1/");
        req.AddParameter("environmentUuid", envUuid);
        req.AddHeader("Content-Type", $"image/{extension}");
        //req.AddHeader("name", "preview.jpg");
        //req.AddHeader("name", "preview.jpg");
        //req.AddHeader("type", "image/jpeg");

        return req;
    }
    
    public static CreatorRequest UploadBundle(string envUuid, string bundlePath) {

        string data = Encoding.UTF8.GetString(File.ReadAllBytes(bundlePath));
        
        CreatorRequest req = new CreatorRequest(Method.PostBytes, $"uploads/asset-bundles", data, "v1/");
        req.AddParameter("environmentUuid", envUuid);
        
        return req;
    }
    
    public static CreatorRequest GetCreatorEnvironments(string userUuid) {
        return new CreatorRequest(Method.Get, $"{userUuid}");
    }
}

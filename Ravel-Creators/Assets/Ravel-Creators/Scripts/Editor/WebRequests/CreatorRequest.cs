using Base.Ravel.Networking;
using Newtonsoft.Json;
using UnityEngine;

public class CreatorRequest : TokenWebRequest
{
    public CreatorRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version) {
        _url += "environments/" + postfix;
    }

    public CreatorRequest(Method method, string postfix, string json, string version) : base(method, "api/",
        version) {
        _url += "environments/" + postfix;
        _json = json;
    }

    public static CreatorRequest CreateEnvironment(string userUuid, Environment env) {
        string json = JsonConvert.SerializeObject(env);
        json = json.Replace("\"isPublic\":", "\"public\":");
        
        return new CreatorRequest(Method.PostJSON, $"{userUuid}", json, "v1/");
    }
    
    public static CreatorRequest GetCreatorEnvironments(string userUuid) {
        return new CreatorRequest(Method.Get, $"{userUuid}");
    }
}

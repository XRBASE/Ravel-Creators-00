using Base.Ravel.Networking;
using UnityEngine;

public class CreatorRequest : TokenWebRequest
{
    public CreatorRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version) {
        _url += "spaces/env/" + postfix;
    }

    public CreatorRequest(Method method, string postfix, string json, string version) : base(method, "api/",
        version) {
        _url += "spaces/env/" + postfix;
        _json = json;
    }

    public static CreatorRequest PublishEnvironment(Environment environment) {
        string json = JsonUtility.ToJson(environment);
        
        return new CreatorRequest(Method.PostJSON, $"publish", json, "v1/");
    }
    
    public static CreatorRequest GetCreatorEnvironments(string userUuid) {
        return new CreatorRequest(Method.Get, $"creators/{userUuid}");
    }
}

using Base.Ravel.Networking;

public class EnvironmentAccessRequest : TokenWebRequest
{
	public EnvironmentAccessRequest(Method method, string postfix, string data = "", string version = "v1/") : base(method, "api/", version) {
		_url += "spaces/env/" + postfix;
		_data = data;
	}

	public static EnvironmentAccessRequest AddAccessUser(string environmentUuid, string userMail) {
		string data = "{\"userEmail\": \"" + userMail + "\"}";
		return new EnvironmentAccessRequest(Method.PostJSON, $"{environmentUuid}/users", data);
	}
	
	public static EnvironmentAccessRequest AddAccessOrganisation(string environmentUuid, string orgUuid) {
		return new EnvironmentAccessRequest(Method.Post, $"{environmentUuid}/organizations/{orgUuid}");
	}
	
	public static EnvironmentAccessRequest GetEnvironmentAccessData(string environmentUuid) {
		return new EnvironmentAccessRequest(Method.Get, $"{environmentUuid}/creators");
	}
}

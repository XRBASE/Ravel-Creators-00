using Base.Ravel.Networking;

/// <summary>
/// Callbacks to change the access users have to private environments.
/// </summary>
public class EnvironmentAccessRequest : TokenWebRequest
{
	public EnvironmentAccessRequest(Method method, string postfix, string data = "", string version = "v1/") : base(method, "api/", version) {
		_url += "spaces/env/" + postfix;
		_data = data;
	}

	/// <summary>
	/// Add access to the environment for one user.
	/// </summary>
	/// <param name="environmentUuid">UUID of the environment.</param>
	/// <param name="userMail">E-mailadress of user to give access to</param>
	public static EnvironmentAccessRequest AddAccessUser(string environmentUuid, string userMail) {
		string data = "{\"userEmail\": \"" + userMail + "\"}";
		return new EnvironmentAccessRequest(Method.PostJSON, $"{environmentUuid}/users", data);
	}
	
	/// <summary>
	/// Add access to the environment for an organisation.
	/// </summary>
	/// <param name="environmentUuid">UUID of the environment.</param>
	/// <param name="orgUuid">UUID of the organisation to which to provide access.</param>
	public static EnvironmentAccessRequest AddAccessOrganisation(string environmentUuid, string orgUuid) {
		return new EnvironmentAccessRequest(Method.Post, $"{environmentUuid}/organizations/{orgUuid}");
	}
	
	/// <summary>
	/// Remove access to an environment for one specific user.
	/// </summary>
	/// <param name="environmentUuid">UUID of the environment.</param>
	/// <param name="userMail">E-mailadress of the user of which the access is revoked.</param>
	public static EnvironmentAccessRequest DeleteAccessUser(string environmentUuid, string userMail) {
		string data = "{\"userEmail\": \"" + userMail + "\"}";
		return new EnvironmentAccessRequest(Method.DeleteJSON, $"{environmentUuid}/users", data);
	}
	
	/// <summary>
	/// Remove access to an environment for an organisation.
	/// </summary>
	/// <param name="environmentUuid">UUID of the environment.</param>
	/// <param name="orgUuid">UUID for the organisation of which the access is revoked</param>
	/// <returns></returns>
	public static EnvironmentAccessRequest DeleteAccessOrganisation(string environmentUuid, string orgUuid) {
		return new EnvironmentAccessRequest(Method.Delete, $"{environmentUuid}/organizations/{orgUuid}");
	}
	
	/// <summary>
	/// Retrieve the current access data of a specific environment based on its UUID.
	/// </summary>
	public static EnvironmentAccessRequest GetEnvironmentAccessData(string environmentUuid) {
		return new EnvironmentAccessRequest(Method.Get, $"{environmentUuid}/creators");
	}
}

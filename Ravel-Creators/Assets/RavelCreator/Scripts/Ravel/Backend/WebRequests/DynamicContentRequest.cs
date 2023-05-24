namespace Base.Ravel.Networking
{
	/// <summary>
	/// Creator project calls for dynamic content (included, so the calls in the project match)
	/// </summary>
	public class DynamicContentRequest : TokenWebRequest
	{
		public DynamicContentRequest(Method method, string postfix, string version = "v1/") : base(method, "api/",
			version) {
			_url += "environments/" + postfix;
		}

		public DynamicContentRequest(Method method, string postfix, string data, string version) : base(method, "api/",
			version) {
			_url += "environments/" + postfix;
			_data = data;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Adds all entries specified in the json to the environment.
		/// </summary>
		public static DynamicContentRequest AddDynamicContentRequest(Environment environment, string contentJson) {
			return new DynamicContentRequest(Method.PostJSON, $"dynamic-contents/{environment.environmentUuid}", contentJson,
				"v1/");
		}
		
		/// <summary>
		/// Removes all entries specified in the json from the environment.
		/// </summary>
		public static DynamicContentRequest DeleteDynamicContentRequest(Environment environment, string contentJson) {
			return new DynamicContentRequest(Method.DeleteJSON, $"dynamic-contents/{environment.environmentUuid}",
				contentJson, "v1/");
		}

		/// <summary>
		/// Removes all dynamic content containers from a given environment.
		/// </summary>
		public static DynamicContentRequest ClearDynamicContentRequest(Environment environment) {
			return new DynamicContentRequest(Method.Post, $"dynamic-contents/{environment.environmentUuid}/clear");
		}
#endif
	}
}
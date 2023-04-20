namespace Base.Ravel.Networking.Users
{
    /// <summary>
    /// Overlaying class to retrieve specific web requests from the user subset of the backend.
    /// </summary>
    public class UserRequest : TokenWebRequest
    {
        /// <summary>
        /// Base constructor for user webrequests. api/ is also added, as this is not a constant part of the webrequests.
        /// </summary>
        /// <param name="method">Method of webcall (Post, Get, Put, etc)</param>
        /// <param name="postfix">Last part of the webrequest: part after: BaseUrl/users/</param>
        /// <param name="version">Version modifier to add to the called url, v1/ by default.</param>
        public UserRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version)
        {
            _url += "users/" + postfix;
        }

        /// <summary>
        /// Retrieve local user, as in last logged in user (user matching saved token).
        /// </summary>
        /// <returns></returns>
        public static UserRequest GetSelf()
        {
            return new UserRequest(Method.Get, $"self");
        }
        
        /// <summary>
        /// Retrieve data of a user based on the email address of the user.
        /// </summary>
        /// <param name="userEmail">email of the user that needs to be retrieved.</param>
        public static UserRequest GetUserByEmail(string userEmail)
        {
            return new UserRequest(Method.Get, $"email/{userEmail}");
        }

        /// <summary>
        /// Get users by giving their email addresses.
        /// </summary>
        /// <param name="emails">list of email addresses from which the users are retrieved.</param>
        public static UserRequest GetUsersByEmail(string[] emails)
        {
            UserRequest req = new UserRequest(Method.Get, $"emails");
            string separator = "&userEmails=";
            string val = "";
            for (int i = 0; i < emails.Length; i++) {
                val += emails[i] + separator;
            }
            req.AddParameter("userEmails", val);

            return req;
        }
        
        /// <summary>
        /// Get user based on their user id.
        /// </summary>
        /// <param name="uuid">uuid of the user that is to be retrieved.</param>
        public static UserRequest GetUserByUUID(string uuid)
        {
            return new UserRequest(Method.Get, $"uuid/{uuid}");
        }

        /// <summary>
        /// Get users based on their uuids.
        /// </summary>
        /// <param name="uuids">list of uuids from which to retrieve the users.</param>
        public static UserRequest GetUsersByUuid(string[] uuids)
        {
            UserRequest req = new UserRequest(Method.Get, $"uuids/uuids");
            string separator = "&uuids=";
            string val = "";
            for (int i = 0; i < uuids.Length; i++) {
                val += uuids[i] + separator;
            }
            req.AddParameter("uuids", val);

            return req;
        }
    }
}
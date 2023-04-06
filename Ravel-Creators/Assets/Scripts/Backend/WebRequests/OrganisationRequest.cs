namespace Base.Ravel.Networking.Organisations
{
    /// <summary>
    /// Overlaying class to retrieve specific web requests from the organisation subset of the backend.
    /// </summary>
    public class OrganisationRequest : TokenWebRequest
    {
        
        /// <summary>
        /// Base constructor for organisation webrequests. api/ is also added, as this is not a constant part of the webrequests.
        /// </summary>
        /// <param name="method">method of webcall (Post, Get, Put, etc)</param>
        /// <param name="postfix">Last part of the webrequest: part after: BaseUrl/organisations/</param>
        /// <param name="version">version modifier to add to the called url, v1/ by default.</param>
        public OrganisationRequest(Method method, string postfix, string version = "v1/") : base(method, "api/", version)
        {
            _url += "organizations/" + postfix;
        }
        
        /// <summary>
        /// Get all users in the organisation matching the given uuid.
        /// </summary>
        /// <param name="orgUuid">uuid of organisation.</param>
        public static OrganisationRequest GetAllOrgsUsers(string orgUuid)
        {
            return new OrganisationRequest(Method.Get, $"users/contacts/{orgUuid}");
        }
        
        /// <summary>
        /// Retrieves role proxy for users role in given organisation.
        /// </summary>
        /// <param name="userUuid">uuid of user for which to retrieve role.</param>
        /// <param name="organisation">organisation of which to retrieve the roles.</param>
        public static OrganisationRequest GetOrganisationRoleOfUser(string userUuid, string organisation)
        {
            return new OrganisationRequest(Method.Get, $"users/roles/{organisation}/usersuuid/{userUuid}");
        }

        /// <summary>
        /// Retrieve a list of all organisations that the user is in.
        /// </summary>
        /// <param name="userUuid">Uuid of the user for which to retrieve the organisations.</param>
        public static OrganisationRequest GetUsersOrganisations(string userUuid)
        {
            return new OrganisationRequest(Method.Get, $"users/uuid/{userUuid}");
        }
        
        /// <summary>
        /// Get users in the given organisation.
        /// </summary>
        /// <param name="organisationName">name of the organisation from which to retrieve the users.</param>
        public static OrganisationRequest GetOrganisationsUsers(string organisationName)
        {
            return new OrganisationRequest(Method.Get, $"organizations/{organisationName}");
        }

        /// <summary>
        /// Get a list of all active organisations in ravel.
        /// </summary>
        /// <returns></returns>
        public static OrganisationRequest GetAllOrganisations()
        {
            return new OrganisationRequest(Method.Get, "active/");
        }
        
        /// <summary>
        /// Retrieve data for an organisation by name.
        /// </summary>
        /// <param name="organisationName">Name of the organisation for which to retrieve the data.</param>
        public static OrganisationRequest GetOrganisationByName(string organisationName)
        {
            return new OrganisationRequest(Method.Get, $"active/{organisationName}");
        }
    }
}


using System;
using UnityEngine;
using Time = MathBuddy.Time.Time;

namespace Base.Ravel.Networking.Authorization
{
    /// <summary>
    /// Overlaying class to retrieve specific web requests from the login subset of the backend.
    /// </summary>
    public class LoginRequest : RavelWebRequest
    {
        /// <summary>
        /// Key to use when retrieving old access tokens from playerprefs. 
        /// </summary>
        public const string SYSTEMS_TOKEN_KEY = "SYSTEMS_TOKEN";
        
        /// <summary>
        /// Base constructor for login webrequests. api/ is also added, as this is not a constant part of the webrequests.
        /// </summary>
        /// <param name="postfix">Last part of the webrequest: part after: BaseUrl/auth/</param>
        /// <param name="data">serialized json data to add to the webrequest body.</param>
        /// <param name="api">Left empty, no api part in these urls.</param>
        /// <param name="version">version modifier to add to the called url, v1/ by default.</param>
        protected LoginRequest(string postfix, string data, string api = "", string version = "v1/") : base(Method.PostJSON, api, version)
        {
            _data = data;
            _url += "auth/" + postfix;
        }

        /// <summary>
        /// Request token using login data (PC).
        /// </summary>
        public static LoginRequest UserPassRequest(string email, string pass)
        {
            string json = JsonUtility.ToJson(new LoginUserPassProxy(email, pass));
            return new LoginRequest("login", json);
        }
        
        /// <summary>
        /// Request code for login VR
        /// </summary>
        public static LoginRequest CodeRequest()
        {
            string json = JsonUtility.ToJson(new ClientIdProxy());
            return new LoginRequest("passwordless/request", json);
        }
        
        /// <summary>
        /// Authenticate (has user paired headset on website)
        /// </summary>
        public static LoginRequest AuthenticateCodeRequest(string code)
        { 
            string json = JsonUtility.ToJson(new ClientCodeProxy(code));
            return new LoginRequest("passwordless/authenticate", json);
        }
        
        /// <summary>
        /// Response to contain the user token data in. This response is also saved in the player prefs for userlogin caching.
        /// </summary>
        [Serializable]
        public class TokenResponse
        {
            public string accessToken;
            public string[] systemAuthorities;
        }
        
        /// <summary>
        /// Code response, retrieved when logging in with the VRHeadset.
        /// </summary>
        [Serializable]
        public class CodeResponse
        {
            public string code;
            public string expiresAt;

            public DateTime ExpiryDate {
                get { return Time.DateFromString(expiresAt); }
            }
        }

        /// <summary>
        /// Proxy to send to the server, containing the user data (is parsed through json and put into the body).
        /// </summary>
        [Serializable]
        private class LoginUserPassProxy
        {
            [SerializeField]
            private string email, password;

            public LoginUserPassProxy(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }

        /// <summary>
        /// Used for adding client id data to proxy classes (that are send to the backend).
        /// (for now?) client id is constant and always the same value.
        /// </summary>
        [Serializable]
        private class ClientIdProxy
        {
            [SerializeField]
            protected string clientId = "SZovEQpEQuiiXn2rpCh78jp8BJxMTwax";
        }
        
        /// <summary>
        /// The proxy class containing the code, that is added in the login request body when logging in from a VR headset.
        /// </summary>
        [Serializable]
        private class ClientCodeProxy : ClientIdProxy
        {
            [SerializeField]
            private string code;

            public ClientCodeProxy(string code)
            {
                this.code = code;
            }
        }
    }
}
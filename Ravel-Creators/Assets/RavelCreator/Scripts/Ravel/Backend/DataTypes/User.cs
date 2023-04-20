using System;
using Base.Ravel.BackendData.Data;

namespace Base.Ravel.Users
{
    [Serializable]
    public class User : DataContainer
    {
        public override string Key {
            get { return userUUID; }
        }

        /// <summary>
        /// Firstname Lastname (of user).
        /// </summary>
        public string FullName {
            get { return $"{firstName} {lastName}"; }
        }

        /// <summary>
        /// Returns either the full body, or if that one doesn't exist the half body url
        /// </summary>
        public string AvatarUrl {
            get { return !string.IsNullOrEmpty(avatarUrlFullBody) ? avatarUrlFullBody: avatarUrl; }
        }

        //Ravel ID
        public string userUUID;
        
        //get userdata response
        public string firstName;
        public string lastName;
        
        //disabled so it is not visible for unique users.
        public string email;
        
        public string avatarUrl;
        public string avatarUrlFullBody;
        
        public string profileImageUrl;
        
        public Organisation[] Organisations {
            get { return _organisations; }
            set { _organisations = value; }
        }
        
        private Organisation[] _organisations;
        
        /// <summary>
        /// Overwrite this user's data with other userdata. Any filled in field in other will be applied to this user.
        /// </summary>
        /// <param name="other">Other user to merge into this user.</param>
        public override bool Overwrite(DataContainer data)
        {
            if (data.GetType() == typeof(User)) {
                bool hasChanges = false;
                User other = (User) data;
                
                if(!string.IsNullOrEmpty(other.userUUID)) {
                    userUUID = other.userUUID;
                    hasChanges = true;
                }
                if(!string.IsNullOrEmpty(other.firstName)) {
                    firstName = other.firstName;
                    hasChanges = true;
                }
                if(!string.IsNullOrEmpty(other.lastName)) {
                    lastName = other.lastName;
                    hasChanges = true;
                }
                if(!string.IsNullOrEmpty(other.email)) {
                    email = other.email;
                    hasChanges = true;
                }
                if (!string.IsNullOrEmpty(other.avatarUrl)) {
                    avatarUrl = other.avatarUrl;
                    hasChanges = true;
                }
                if (!string.IsNullOrEmpty(other.avatarUrlFullBody)) {
                    avatarUrlFullBody = other.avatarUrlFullBody;
                    hasChanges = true;
                }
                
                return hasChanges;
            }

            throw GetOverwriteFailedException(data);
        }
        
        /// <summary>
        /// Return username(full) and userid. 
        /// </summary>
        public override string ToString() { return $"{FullName}({userUUID})"; }
    }
}
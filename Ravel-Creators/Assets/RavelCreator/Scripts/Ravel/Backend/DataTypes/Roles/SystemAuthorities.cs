using System;

namespace Base.Ravel.Networking.Authorities
{
    public static class Authorities
    {
        public static SystemAuthorities FromString(string[] values)
        {
            SystemAuthorities auth = SystemAuthorities.none;
            
            string[] names = Enum.GetNames(typeof(SystemAuthorities));
            
            for (int i = 1; i < names.Length; i++) {
                for (int j = 0; j < values.Length; j++) {
                    values[j] = values[j].Replace(':', '_');
                    if (names[i] == values[j]) {
                        auth |= (SystemAuthorities)(1 << (i - 1));
                    }
                }
            }

            return auth;
        }
    } 
    
    [Flags]
    public enum SystemAuthorities
    {
        none = 0,
        user_read = 1<<0,
        user_create = 1<<1,
        user_update = 1<<2,
        user_delete = 1<<3,
        user_admin_access = 1<<4,
        dev_access = 1<<5,
        admin_access = 1<<6,
    }
}
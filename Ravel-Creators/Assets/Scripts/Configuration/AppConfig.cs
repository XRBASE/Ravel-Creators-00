namespace Base.Ravel.Config
{
    public static class AppConfig
    {
        /// <summary>
        /// Get (and possibly load) the networking config from the resource folder. 
        /// </summary>
        public static NetworkConfig Networking {
            get {
                if (!_netConfig) {
                    _netConfig = NetworkConfig.Load();
                }

                return _netConfig;
            }
        }

        private static NetworkConfig _netConfig;
    }
}
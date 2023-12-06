using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Config
{
    [CreateAssetMenu(menuName = "Ravel/Config/Networking", fileName = "NetConfig", order = 0)]
    public class NetworkConfig : ScriptableObject
    {
        public const string REGION = "EU";
        
        public string DataServiceBaseUrl {
            get { return GetBaseUrl(_mode, true); }
        }
        
        public string SiteUrl {
            get { return GetSiteUrl(_mode); }
        }
        
        /// <summary>
        /// Realtime app id for photon
        /// </summary>
        public string RealtimeAppId {
            get { return GetRealtimeId(_mode, true); }
        }
        
        /// <summary>
        /// App id for Agora
        /// </summary>
        public string AgoraAppId {
            get { return GetAgoraId(_mode, true); }
        }

        public AppMode Mode {
            get { return _mode; }
            set { _mode = value; }
        }
        
        public bool reconnectOnDisconnect;
        public float reconnectDuration;
        public int maxReconnectTries;
        
        /// <summary>
        /// Mode to which this config applies, TODO: also connects to correlated database
        /// </summary>
        [SerializeField] private AppMode _mode;

        private string _backendOverrideUrl = "";
        private bool _shouldOverrideBackendUrl = false;

#region BackendUrls

        public void OverrideBackendUrl(string url)
        {
            _shouldOverrideBackendUrl = !string.IsNullOrEmpty(url);
            _backendOverrideUrl = url;
        }

        private string GetBaseUrl(AppMode mode, bool debugValue = false)
        {
            string url;
            if (_shouldOverrideBackendUrl) {
                url = _backendOverrideUrl;
            }
            else {
                switch (mode) {
                    case AppMode.PersistentSample:
                    case AppMode.Development:
                        url = "https://dev.ravel.systems/";
                        break;
                    case AppMode.Test:
                        url = "https://test.ravel.systems/";
                        break;
                    case AppMode.Live:
                        url = "https://live.ravel.systems/";
                        break;
                    case AppMode.LocalHost:
                        url = "http://localhost:8080/";
                        break;
                    default:
                        throw new MissingFieldException($"No baseurl (Dataservices) for appMode {mode} found!");
                }
            }
            
            if (debugValue)
                Debug.Log($"Using backend mode {mode} url: ({url})");
            return url;
        }

        private string GetSiteUrl(AppMode mode) {
            string url;
            switch (mode) {
                case AppMode.PersistentSample:
                case AppMode.Development:
                    url = "https://dev.ravel.world/";
                    break;
                case AppMode.Test:
                    url = "https://demo.ravel.world/";
                    break;
                case AppMode.Live:
                    url = "https://app.ravel.world/";
                    break;
                case AppMode.LocalHost:
                    url = "http://localhost:8080/";
                    break;
                default:
                    throw new MissingFieldException($"No baseurl (Dataservices) for appMode {mode} found!");
            }
            return url;
        }

        #endregion

#region PhotonIds

        //realtimeIds
        // there is only a development (that is also test) app id and a live one
        //dev:  41149b28-2061-4424-9d2b-0d392ebc8027
        //test: 41149b28-2061-4424-9d2b-0d392ebc8027
        //live: e2bdfa09-1a97-41ab-acbf-a33e10c96a30
        //peristentRoomSample: df17aba7-0290-4273-bed4-45a142d93827
        private string GetRealtimeId(AppMode m, bool debugValue = false)
        {
            string id;
            switch (m) {
                case AppMode.Development:
                case AppMode.Test:
                    id = "41149b28-2061-4424-9d2b-0d392ebc8027";
                    break;
                case AppMode.Live:
                    id = "e2bdfa09-1a97-41ab-acbf-a33e10c96a30";
                    break;
                case AppMode.PersistentSample:
                    id = "93f34f51-bb3f-4ed4-9431-5f26d135efe4"; //"df17aba7-0290-4273-bed4-45a142d93827";
                    break;
                default:
                    throw new MissingFieldException($"No ids for appMode {m} found!");
            }
            
            //TODO: move debug in networker class, not here
            if (debugValue)
                Debug.Log($"Using {m} realtime id ({id})");
            return id;
        }

#endregion

#region AgoraIds
        //agora ids
        // there is only a development (that is also test) app id and a live one
        //test version jasper:  8cf3286b37474c13af969ce398ede20c
        private string GetAgoraId(AppMode m, bool debugValue = false)
        {
            string id;
            switch (m) {
                case AppMode.Development:
                case AppMode.Test:
                case AppMode.Live:
                case AppMode.PersistentSample:
                    id = "b5ef7fb8a6004bbab48d15fe4e6b13fd";
                    break;
                default:
                    throw new MissingFieldException($"No agora ids for appMode {m} found!");
            }
            
            if (debugValue)
                Debug.Log($"Using {m} agora id ({id})");
            return id;
        }
#endregion

        /// <summary>
        /// Load configuration from the resources folder.
        /// </summary>
        public static NetworkConfig Load()
        {
            NetworkConfig cfg = Resources.Load<NetworkConfig>("Config/NetConfig");
            if (!cfg) {
                throw new FileNotFoundException($"No network config found!");
            }

            //TODO: live when no dev build
            /*
    #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            cfg._mode = NetworkConfig.AppMode.Live;
    #endif*/
            return cfg;
        }

        public enum AppMode
        {
            Unknown = -1,
            Development = 0,
            Test = 1,
            Live = 2,
            PersistentSample = 3,
            LocalHost = 4,
        }
        
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(NetworkConfig))]
        private class NetworkConfigEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                
                EditorGUILayout.Space();
                NetworkConfig cfg = ((NetworkConfig) target);
                string url = cfg.GetBaseUrl(cfg._mode);
                GUILayout.Label($"using backend url: \t {url}");
                EditorGUILayout.Space();
                string id = cfg.GetRealtimeId(cfg._mode);
                GUILayout.Label($"using realtime id: \t {id}");
                id = cfg.GetAgoraId(cfg._mode);
                GUILayout.Label($"using Agora id: \t\t {id}");
            }
        }
        #endif
    }
}
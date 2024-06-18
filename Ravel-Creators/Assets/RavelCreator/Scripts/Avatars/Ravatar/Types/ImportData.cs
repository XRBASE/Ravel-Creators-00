using System;
using Base.Ravel.CharacterAnimation;
using UnityEngine;

namespace Base.Ravel.RAvatars {
	/// <summary>
	/// Data class containing the data that is used for avatar importing. This clas can be inherited from to create different
	/// types of avatar importer. It is coupled with an import action to create a whole import process from start to finish.
	/// </summary>
	[Serializable]
	public abstract class ImportData {
		public abstract AvatarType Type { get; }

		//AVATR: remove public setter for this!
		public string GUID { get; set; }

		public enum AvatarType {
			ReadyPlayerMe,
			Prefab,
		}

		public ImportData(string guid) {
			GUID = guid;
		}
	}
	
	[Serializable]
    public class PrefabImportData : ImportData
    {
        private const string RESOURCE_PREFIX = "Avatars/Prefab/";
            
        public override AvatarType Type {
            get { return AvatarType.Prefab; }
        }

        //using scene or project asset
        public GameObject prefab;
        //should new instance be instantiated
        public bool preloaded;
        //use when loading from resource folder, leave empty otherwise
        public string resourcePath;

        public CharacterSettings charSettings;
        
        /// <summary>
        /// Creates prefab data for prefabs loaded from within the resource folder.
        /// </summary>
        /// <param name="resourcePath">path of resource starting at the "Resources/Avatars/Prefab/ folder."</param>
        /// <param name="gender">Used for the gender selection of animations (0 = feminine, 1 = masculine)</param>
        public PrefabImportData(string resourcePath, string guid, float gender = 0.5f) : base(guid) {
            this.resourcePath = resourcePath;
            charSettings = new CharacterSettings(gender);
        }
        
        /// <summary>
        /// Creates prefab data for prefab files (in or outside of the scene).
        /// </summary>
        /// <param name="prefab">prefab object, either in the scene or project hierarchy."</param>
        /// <param name="preloaded">Is the prefab object in the scene (true) or project hierarchy (false)."</param>
        /// <param name="gender">Used for the gender selection of animations (0 = feminine, 1 = masculine)</param>
        public PrefabImportData(GameObject prefab, bool preloaded, string guid, float gender = 0.5f) : base(guid) {
            this.prefab = prefab;
            this.preloaded = preloaded;
            
            charSettings = new CharacterSettings(gender);
        }
        
        //temporary use for testing
#if UNITY_EDITOR
        public static ImportData TestData {
            get { return new PrefabImportData(RESOURCE_PREFIX + "PF_Blub", "SamplePrefabBlub", 0.0f); }
        }
#endif
    }
    
	public class RPMImportData : ImportData
	{
		public override AvatarType Type {
			get { return AvatarType.ReadyPlayerMe; }
		}

		public string url;

		public RPMImportData(string url, string guid) : base(guid) {
			this.url = url;
		}
        
		//temporary test data for easy testing
#if UNITY_EDITOR
		public static ImportData TestData {
			get { return new RPMImportData("https://models.readyplayer.me/64006df45167081fc2e9ada0.glb", "SampleFemm"); }
            
			//masculine option
			//get { return new RPMImportData("https://models.readyplayer.me/6582ec545d2ee499ada120a1.glb", "SampleMasc"); }
		}
#endif
	}
}

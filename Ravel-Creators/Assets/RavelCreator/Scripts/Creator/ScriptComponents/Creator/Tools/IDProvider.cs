using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
#if UNITY_EDITOR
	public static class IDProvider
	{
		public static void SetSceneIDs() {
			int cId = 1;
			IUniqueId[] idHolders = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IUniqueId>().ToArray();
			for (int i = 0; i < idHolders.Length; i++) {
				if (idHolders[i].SetUniqueID) {
					idHolders[i].ID = cId;
					EditorUtility.SetDirty(idHolders[i] as MonoBehaviour);
					cId++;
				}
			}
		}
	}
#endif
	
	/// <summary>
	/// Used to set custom id's in bundles, and track objects that hold these id's
	/// </summary>
	public interface IUniqueId
	{
		/// <summary>
		/// False if object instance is not networked.
		/// </summary>
		public bool SetUniqueID { get; }

		/// <summary>
		/// ID used for identification in network traffic
		/// </summary>
		public int ID { get; set; }
	}
}

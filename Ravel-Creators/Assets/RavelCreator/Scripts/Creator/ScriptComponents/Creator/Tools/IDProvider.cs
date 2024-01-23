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
		/// <summary>
		/// Cycles through the whole scene and any IUniqueId Monobehaviour class, to assign it unique id's.
		/// </summary>
		public static void SetSceneIDs() {
			int cId = 1;
			IUniqueId[] idHolders = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<IUniqueId>().ToArray();
			for (int i = 0; i < idHolders.Length; i++) {
				if (idHolders[i].SetUniqueID) {
					idHolders[i].ID = cId;
					EditorUtility.SetDirty(idHolders[i] as MonoBehaviour);
					cId++;
				}
			}
		}

		/// <summary>
		/// Cycles through all items of type T and returns (if any) the type that has the id that is given as parameter.
		/// </summary>
		/// <param name="id">id of item to be found.</param>
		/// <param name="found">found item result</param>
		/// <typeparam name="T">Type of item that is being searched for (use monobehaviour for all types).</typeparam>
		/// <returns>True/False item with matching id was found.</returns>
		public static bool TryGet<T>(int id, out T found) where T : MonoBehaviour, IUniqueId {
			IUniqueId[] idHolders = GameObject.FindObjectsOfType<T>(true).ToArray();
			foreach (var item in idHolders) {
				if (item.SetUniqueID && item.ID == id) {
					found = (T)item;
					return true;
				}
			}

			found = default;
			return false;
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
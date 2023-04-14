using System.Linq;
using UnityEditor;
using UnityEngine;

public static class IDProvider
{
	public static void SetSceneIDs() {
		int cId = 1;
		INetworkId[] idHolders = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<INetworkId>().ToArray();
		for (int i = 0; i < idHolders.Length; i++) {
			if (idHolders[i].Networked) {
				idHolders[i].ID = cId;
				EditorUtility.SetDirty(idHolders[i] as MonoBehaviour);
				cId++;
			}
		}
	}
    
}

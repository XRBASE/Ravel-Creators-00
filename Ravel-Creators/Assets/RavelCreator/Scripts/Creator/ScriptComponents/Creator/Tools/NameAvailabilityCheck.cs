using UnityEngine;

namespace Base.Ravel.Creator.Components.Naming
{
	public static class NameAvailabilityCheck
	{
		/// <summary>
		/// Checks if given name is not used by any monobehaviour components of the same type.
		/// </summary>
		/// <param name="owner">owner of which the name is being checked.</param>
		/// <typeparam name="T">type of Monobehaviour.</typeparam>
		/// <returns>True/False name is available.</returns>
		public static bool Check<T>(T owner) where T : MonoBehaviour, INameIdentifiedObject {
			T[] nameHolders = GameObject.FindObjectsOfType<T>(true);
			for (int i = 0; i < nameHolders.Length; i++) {
				if (nameHolders[i] == owner)
					continue;
				
				if (nameHolders[i].Name == owner.Name)
					return false;
			}

			return true;
		}
	}

	public interface INameIdentifiedObject
	{
		public string Name { get; }
	}
}
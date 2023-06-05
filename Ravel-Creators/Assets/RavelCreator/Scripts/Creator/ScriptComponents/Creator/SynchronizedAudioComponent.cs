using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	/// <summary>
	/// Syncronized audio component, no data needed all data retrieved based on the audiosource Monobehaviour
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Ravel/Synchronized audio")]
	public partial class SynchronizedAudioComponent : ComponentBase, INetworkId
	{
		public bool Networked {
			get { return true; }
		}
		public int ID {
			get { return _data.id;}
			set { _data.id = value; }
		}
		
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField, HideInInspector] private PlayableData _data;

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		/// <summary>
		/// Play the audio file.
		/// </summary>
		public void Play() { }

		/// <summary>
		/// Pauses the audio file.
		/// </summary>
		public void Pause() { }

		/// <summary>
		/// Stop the audio file (next play will play from start of file).
		/// </summary>
		public void Stop() { }

#if UNITY_EDITOR
		[CustomEditor(typeof(SynchronizedAudioComponent))]
		private class SynchronizedAudioComponentEditor : Editor
		{
			public override void OnInspectorGUI() {
				DrawDefaultInspector();
				
				GUILayout.Label("Any audiosource with this component bound to it will be synchronized among players.\n" +
				                "It does require the play, pause and stop callbacks to be executed through this script.");
			}
		}
#endif
	}
	
	[Serializable]
	public class PlayableData : ComponentData
	{
		public int id;
	}
}
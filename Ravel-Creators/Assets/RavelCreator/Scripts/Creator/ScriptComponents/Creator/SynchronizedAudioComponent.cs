using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Base.Ravel.Creator.Components
{
	[RequireComponent(typeof(AudioSource))]
	public partial class SynchronizedAudioComponent : ComponentBase, INetworkId
	{
		public override ComponentData Data {
			get { return null; }
		}
		
		/// <summary>
		/// The component is always networked, but it's id is set through the audio source management system and so this
		/// bool is false to prevent the id from beind accessed. the component just changes the network status of the audio source.
		/// </summary>
		public bool Networked {
			get { return false; }
		}
		public int ID {
			get { return -1;}
			set { throw new Exception("ID for synced audio is set internally!"); }
		}

		protected override void BuildComponents() { }

		protected override void DisposeData() { }

		public void Play() { }
		public void Pause() { }
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
}
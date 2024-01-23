using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components.Games.Collectibles
{
	[AddComponentMenu("Ravel/Games/Collectibles/CollectibleGame")]
	public partial class CollectiblesGameComponent : RavelGameComponent
	{
		public override RavelGameData BaseData {
			get { return _data; }
		}
		
		public override ComponentData Data {
			get { return _data; }
		}

		[SerializeField] private CollectibleGameData _data;
		protected override void BuildComponents() { }

		protected override void DisposeData() { }
	}
	
	[Serializable]
	public class CollectibleGameData : RavelGameData
	{
		public bool networked;
		[Tooltip("Points earned per collected item")]
		public int scoreFactor = 1;

		public UnityEvent onAnyCollected;
		public UnityEvent onAllCollected;

		public List<CollectibleComponent> collectibles;
	}
}
/// <summary>
/// Used to set custom id's in bundles, and track objects that hold these id's
/// </summary>
public interface INetworkId
{
	/// <summary>
	/// False if object instance is not networked.
	/// </summary>
	public bool Networked { get; set; }
	
	/// <summary>
	/// ID used for identification in network traffic
	/// </summary>
	public int ID { get; set; }
}

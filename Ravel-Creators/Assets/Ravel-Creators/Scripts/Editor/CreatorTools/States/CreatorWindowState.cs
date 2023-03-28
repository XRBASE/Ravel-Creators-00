using UnityEngine;

/// <summary>
/// Base class for sub-windows of the Creator window. 
/// </summary>
public abstract class CreatorWindowState
{
	/// <summary>
	/// State identifier of this window.
	/// </summary>
	public abstract CreatorWindow.State State { get; } 

	private CreatorWindow _wnd;
	
	public CreatorWindowState(CreatorWindow wnd) {
		_wnd = wnd;
	}
	
	public abstract void OnGUI(Rect position);

	/// <summary>
	/// Opens creator window to this sub-page.
	/// </summary>
	public void Show() {
		_wnd.SwitchTab(State);
		_wnd.Show();
	}

	/// <summary>
	/// Closes the creator window
	/// </summary>
	public void Close() {
		_wnd.Close();
	}
}

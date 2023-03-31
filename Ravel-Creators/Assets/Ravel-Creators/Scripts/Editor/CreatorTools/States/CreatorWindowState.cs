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
	
	/// <summary>
	/// Minimum size for the window
	/// </summary>
	protected abstract Vector2 MinSize { get; } 

	//reference to containing editor window.
	private CreatorWindow _wnd;

	public CreatorWindowState(CreatorWindow wnd) {
		_wnd = wnd;
	}
	
	/// <summary>
	/// Called to draw GUI of the window
	/// </summary>
	/// <param name="position">Rect in which is being drawn + position of the current place where draw is performed.</param>
	public abstract void OnGUI(Rect position);

	/// <summary>
	/// Called when switching out of the state, or closing the whole window.
	/// </summary>
	public virtual void OnStateClosed() { }
	
	/// <summary>
	/// Called when switching to this window.
	/// </summary>
	public virtual void OnSwitchState() {
		SetMinSize();
	}

	/// <summary>
	/// Sets the minimum size of the window.
	/// </summary>
	protected void SetMinSize() {
		_wnd.minSize = MinSize;
	}

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

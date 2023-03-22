public abstract class CreatorWindowState
{
	public abstract CreatorWindow.State State { get; } 

	private CreatorWindow _wnd;
	
	public CreatorWindowState(CreatorWindow wnd) {
		_wnd = wnd;
	}
	
	public abstract void OnGUI();

	public void Show() {
		_wnd.SetState(State);
		_wnd.Show();
	}

	public void Close() {
		_wnd.Close();
	}
}

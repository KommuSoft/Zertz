using System;
using OpenTK;
using OpenTK.Input;

namespace GTZ.Rendering {
	
	public interface IScene : ILoadable, IUnloadable, ISelectionRenderable, ITimeSensitive {
		
		Camera Camera {
			get;
			set;
		}
		MessageBoard MessageBoard {
			get;
			set;
		}
		MainWindow MainWindow {
			get;
			set;
		}
		ComponentContainer ComponentContainer {
			get;
			set;
		}
		
		void MouseMoveSelectionChanged (int oldsel, int sel);
		void ClickedItem (int sel);
		void RenderDashboard (FrameEventArgs e);
		bool HandleKeyDown (Key key);
		void OnResize (EventArgs e);
		void PerformAction (string action);
		
	}
}


using System;
using OpenTK;
using OpenTK.Input;

namespace Zertz.Rendering {
	
	public abstract class SceneBase : IScene {
		
		private Camera camera;
		private MessageBoard messageBoard;
		private MainWindow mainWindow;
		private ComponentContainer cc;
		
		public ComponentContainer ComponentContainer {
			get {
				return this.cc;
			}
			set {
				this.cc = value;
			}
		}
		public Camera Camera {
			get {
				return this.camera;
			}
			set {
				this.camera = value;
			}
		}
		public MessageBoard MessageBoard {
			get {
				return this.messageBoard;
			}
			set {
				this.messageBoard = value;
			}
		}
		public MainWindow MainWindow {
			get {
				return this.mainWindow;
			}
			set {
				this.mainWindow = value;
			}
		}
		
		public abstract void AdvanceTime (float time);
		public abstract void Render (FrameEventArgs e);
		public virtual void SelectionRender (FrameEventArgs e) {
			this.Render(e);
		}
		public abstract void OnLoad (EventArgs e);
		public abstract void OnUnload (EventArgs e);
		public abstract void MouseMoveSelectionChanged (int oldsel, int sel);
		public abstract void ClickedItem (int sel);
		public abstract bool HandleKeyDown (Key key);
		public virtual void RenderDashboard (FrameEventArgs e) {}
		public virtual void OnResize (EventArgs e) {}
		public virtual void PerformAction (string action) {}
		
	}
}


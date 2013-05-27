using System;
using OpenTK;
using OpenTK.Input;

namespace Zertz.Rendering {

	public class TopMenuScene : SceneBase {
		
		private OpenGLLargeButtonControl[] oglbcs;
		private string[] buttonTexts = new string[] {"Skirimish","Campaign","Netwerk","Opties","Afsluiten"};
		
		public TopMenuScene () {
			
		}
		
		public override void Render (OpenTK.FrameEventArgs e) {}
		public override void AdvanceTime (float time) {}
		public override void OnUnload (EventArgs e) {}
		public override void MouseMoveSelectionChanged (int oldsel, int sel) {}
		public override void OnLoad (EventArgs e) {
			oglbcs = new OpenGLLargeButtonControl[5];
			for(int i = 0x00; i < oglbcs.Length; i++) {
				oglbcs[i] = new OpenGLLargeButtonControl();
				oglbcs[i].Text = buttonTexts[i];
				this.ComponentContainer.Add(oglbcs[i]);
			}
			oglbcs[0x00].OnLoad(e);
			this.OnResize(e);
		}
		public override bool HandleKeyDown (Key key) {
			return false;
		}
		public override void OnResize (EventArgs e) {
			int dy = (this.MainWindow.Height-oglbcs.Length*oglbcs[0x00].Height)/(oglbcs.Length+1);
			int y = dy;
			dy += oglbcs[0x00].Height;
			for(int i = 0x00; i < oglbcs.Length; i++) {
				oglbcs[i].X = (this.MainWindow.Width-oglbcs[i].Width)/2;
				oglbcs[i].Y = y;
				y += dy;
			}
		}
		public override void ClickedItem (int sel) {}
		
	}
	
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GTZ.Rendering {
	
	public class PrologueScene : SceneBase {
		
		private PopupBook book;
		private static readonly Regex[] regexes = new Regex[] {new Regex(@"^book item ([0-9]+)( i)? (-?[0-9]+.[0-9]+)$",RegexOptions.Compiled),new Regex(@"^PBI( -?[0-9]+.[0-9]+)*( \{( (-?[0-9]+.[0-9]+),(-?[0-9]+.[0-9]+))* \})? ([0-9]+)$",RegexOptions.Compiled)};
		private readonly List<PopupBookItem> pbis = new List<PopupBookItem>();
		
		public PrologueScene () {
		}
		
		public override void AdvanceTime (float time) {
			this.book.AdvanceTime(time);
		}
		public override void Render (FrameEventArgs e) {
			GL.PushMatrix();
			this.Camera.Render(e);
			this.book.Render(e);
			GL.PopMatrix();
		}
		public override void OnLoad (EventArgs e) {
			this.Camera.RotateXZTarget = 15.0f;
			this.OnResize(new EventArgs());
			Stream s = File.Open("prologueScene.sc",FileMode.Open,FileAccess.Read);
			this.MainWindow.SceneLoader.LoadScene(s);
			s.Close();
			this.book = new PopupBook(this.pbis.ToArray());
			this.book.OnLoad(e);
		}
		public override void PerformAction (string action) {
			Match m;
			if(action.StartsWith("PBI")) {
				m = regexes[0x01].Match(action);
				if(m.Success) {
					float[] f = new float[0x09];
					for(int i = 0x00; i < 0x09; i++) {
						f[i] = float.Parse(m.Groups[0x01].Captures[i].Value,NumberFormatInfo.InvariantInfo);
					}
					int np = m.Groups[0x04].Captures.Count;
					PointF[] pts = new PointF[np];
					float px, py;
					for(int i = 0x00; i < np; i++) {
						px = float.Parse(m.Groups[0x04].Captures[i].Value,NumberFormatInfo.InvariantInfo);
						py = float.Parse(m.Groups[0x05].Captures[i].Value,NumberFormatInfo.InvariantInfo);
						pts[i] = new PointF(px,py);
					}
					float dtr = (float) (Math.PI/180.0d);
					int layer = int.Parse(m.Groups[0x06].Value);
					this.pbis.Add(new PopupBookItem(f[0x00],f[0x01],f[0x02]*dtr,f[0x03],f[0x04],f[0x05],f[0x06],f[0x07],f[0x08],pts,layer));
				}
			}
			else if(action.StartsWith("book")) {
				m = regexes[0x00].Match(action);
				if(m.Success) {
					int index = int.Parse(m.Groups[0x01].Value);
					float angle = float.Parse(m.Groups[0x03].Value,NumberFormatInfo.InvariantInfo);
					this.book.SetItemAngle(index,(float) (angle*Math.PI/180.0d),m.Groups[0x02].Success);
				}
				else if(action.EndsWith("open")) {
					this.book.Open();
				}
				else if(action.EndsWith("close")) {
					this.book.Close();
				}
			}
		}
		public override void OnUnload (EventArgs e) {}
		public override void MouseMoveSelectionChanged (int oldsel, int sel) {}
		public override void ClickedItem (int sel) {}
		public override bool HandleKeyDown (Key key) {
			return false;
		}
		public override void OnResize (EventArgs e) {
		}
		
	}
}


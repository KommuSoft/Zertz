using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GTZ.Utils;

namespace GTZ.Rendering {
	
	public abstract class OpenGLComponent : ILoadable, IRenderable, ISelectable, ILocatable2d, ISizable {
		
		private Rectangle bounds = Rectangle.Empty;
		private bool selected = false;
		
		public int Height {
			get {
				return this.bounds.Height;
			}
			set {
				this.bounds.Height = value;
			}
		}
		public int Width {
			get {
				return this.bounds.Width;
			}
			set {
				this.bounds.Width = value;
			}
		}
		public int X {
			get {
				return this.bounds.X;
			}
			set {
				this.bounds.X = value;
			}
		}
		public int Y {
			get {
				return this.bounds.Y;
			}
			set {
				this.bounds.Y = value;
			}
		}
		public bool Selected {
			get {
				return this.selected;
			}
			set {
				this.selected = value;
			}
		}
		public Rectangle Bounds {
			get {
				return this.bounds;
			}
		}
		protected virtual int DefaultWidth {
			get {
				return 0x0100;
			}
		}
		protected virtual int DefaultHeight {
			get {
				return 0x0100;
			}
		}
		
		protected OpenGLComponent () {
			this.bounds.Width = DefaultWidth;
			this.bounds.Height = DefaultHeight;
		}
		protected OpenGLComponent (int width, int height) {
			this.bounds.Width = width;
			this.bounds.Height = height;
		}
		
		protected virtual void InternalRender (FrameEventArgs e) {}
		public void Render (FrameEventArgs e) {
			GL.PushMatrix();
			GL.Translate(this.bounds.X,this.bounds.Y,0.0f);
			this.InternalRender(e);
			GL.PopMatrix();
		}
		public virtual void OnLoad (EventArgs e) {}
		public virtual void OnMouseEnter () {}
		public virtual void OnMouseLeave () {}
		public virtual void OnMouseMove (Point p) {}
		
	}
}


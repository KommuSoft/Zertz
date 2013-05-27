using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace Zertz.Rendering {
	
	public class ComponentContainer : IRenderable {
		
		private readonly HashSet<OpenGLComponent> components = new HashSet<OpenGLComponent>();
		private OpenGLComponent hover;
		
		public ComponentContainer () {
		}
		
		public void Add (OpenGLComponent component) {
			lock(this.components) {
				this.components.Add(component);
			}
		}
		public void Remove (OpenGLComponent component) {
			lock(this.components) {
				this.components.Remove(component);
			}
		}
		public void Clear () {
			lock(this.components) {
				this.components.Clear();
			}
		}
		
		public void Render (FrameEventArgs e) {
			lock(this.components) {
				foreach(OpenGLComponent ogc in this.components) {
					ogc.Render(e);
				}
			}
		}
		public bool OnMouseMove (MouseMoveEventArgs e) {
			Point p = e.Position;
			if(this.hover != null) {
				if(this.hover.Bounds.Contains(p)) {
					this.hover.OnMouseMove(p-(Size) this.hover.Bounds.Location);
					return true;
				}
			}
			foreach(OpenGLComponent oglc in this.components) {
				if(oglc.Bounds.Contains(p)) {
					if(this.hover != null) {
						this.hover.OnMouseLeave();
					}
					this.hover = oglc;
					this.hover.OnMouseEnter();
					return true;
				}
			}
			if(this.hover != null) {
				this.hover.OnMouseLeave();
			}
			this.hover = null;
			return false;
		}
		public bool OnMouseDown (MouseButtonEventArgs e) {
			return false;
		}
		public bool OnMouseUp (MouseButtonEventArgs e) {
			return false;
		}
		
	}
	
}
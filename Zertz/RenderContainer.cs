using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Zertz.Utils;

namespace Zertz.Rendering {
	
	public class RenderContainer : IRenderable, IPhysical, ITimeSensitive {
		
		private readonly Dictionary<int,IRenderable> objects = new Dictionary<int,IRenderable>();
		private readonly SortedList<int,ISelectable> selects = new SortedList<int,ISelectable>();
		private readonly List<ITimeSensitive> timeObjects = new List<ITimeSensitive>();
		private readonly List<IPhysical> physObjects = new List<IPhysical>();
		private ISelectable selected = null;
		private int ind = -0x01;
		
		public ISelectable Selected {
			get {
				return this.selected;
			}
		}
		
		public IRenderable this [int id] {
			get {
				lock(objects) {
					IRenderable val;
					if(objects.TryGetValue(id,out val)) {
						return val;
					}
					else {
						return null;
					}
				}
			}
		}
		
		public RenderContainer () {
		}
		
		public void Add (int id, IRenderable render) {
			lock(objects) {
				this.objects.Add(id,render);
			}
			if(render is ISelectable) {
				lock(this.selects) {
					this.selects.Add(id,(ISelectable) render);
				}
			}
			if(render is IPhysical) {
				lock(this.physObjects) {
					this.physObjects.Add((IPhysical) render);
				}
			}
			else if(render is ITimeSensitive) {
				lock(this.timeObjects) {
					this.timeObjects.Add((ITimeSensitive) render);
				}
			}
		}
		public void AdvanceTime (float time, Vector3 gravity, Vector3 wind) {
			lock(this.physObjects) {
				foreach(IPhysical p in this.physObjects) {
					p.AdvanceTime(time,gravity,wind);
				}
			}
			lock(this.timeObjects) {
				foreach(ITimeSensitive ts in this.timeObjects) {
					ts.AdvanceTime(time);
				}
			}
		}
		private void replaceSelected (ISelectable nextSelected) {
			if(this.selected != null) {
				this.selected.Selected = false;
			}
			this.selected = nextSelected;
			if(this.selected != null) {
				this.selected.Selected = true;
			}
		}
		public void SelectNext () {
			lock(this.selects) {
				this.ind = (this.ind+0x01)%this.selects.Count;
				this.replaceSelected(this.selects.Values[ind]);
			}
		}
		public void SelectPrevious () {
			lock(this.selects) {
				this.ind = (this.ind+this.selects.Count-0x01)%this.selects.Count;
				this.replaceSelected(this.selects.Values[ind]);
			}
		}
		public void SelectName (int name) {
			ISelectable isel;
			lock(this.selects) {
				if(this.selects.TryGetValue(name,out isel)) {
					this.replaceSelected(isel);
				}
				else {
					this.replaceSelected(null);
				}
			}
		}
		public void AdvanceTime (float time) {
			lock(this.timeObjects) {
				foreach(ITimeSensitive ts in this.timeObjects) {
					ts.AdvanceTime(time);
				}
			}
		}
		public void Render (OpenTK.FrameEventArgs e) {
			lock(objects) {
				foreach(KeyValuePair<int,IRenderable> ir in this.objects) {
					GL.LoadName(ir.Key);
					ir.Value.Render(e);
				}
				GL.LoadName(-0x01);
			}
		}
		
	}
	
}
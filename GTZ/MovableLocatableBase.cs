using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public class MovableLocatableBase : LocatableBase, IRenderMoveable {
		
		private float time = 0.0f;
		private RenderMover mover = RenderMoveManager.GenerateStaticMover(new Vector3());
		
		public override Vector3 Location {
			get {
				return this.RenderMover(this);
			}
		}
		public float MoveTime {
			get {
				return this.time;
			}
			set {
				this.time = value;
			}
		}
		public RenderMover RenderMover {
			get {
				return this.mover;
			}
			set {
				this.time = 0.0f;
				this.mover = value;
			}
		}
		
		public virtual void FinishedMoveChain (Vector3 v) {}
		
	}
}


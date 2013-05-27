using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public abstract class LocatableBase : ILocatable {
		
		private Vector3 location = Vector3.Zero;
		private Vector3 locationTarget = Vector3.Zero;
		
		protected Vector3 LocationTarget {
			get {
				return this.locationTarget;
			}
		}
		public virtual Vector3 Location {
			get {
				return this.location;
			}
			set {
				this.location = value;
				this.locationTarget = value;
			}
		}
		
		public void MoveTo (Vector3 targetLocation) {
			this.locationTarget = targetLocation;
			if(this.locationTarget != this.location) {
				this.OnTargetLocationChanged(targetLocation);
			}
		}
		protected virtual void OnTargetLocationChanged (Vector3 targetLocation) {}
		
	}
	
}
using System;

namespace GTZ.Rendering {
	
	public abstract class SizableBase {
		
		private int width;
		private int height;
		
		public int Width {
			get {
				return this.width;
			}
			set {
				this.width = value;
			}
		}
		public int Height {
			get {
				return this.height;
			}
			set {
				this.height = value;
			}
		}
		
		protected SizableBase (int width, int height) {
			this.width = width;
			this.height = height;
		}
		
	}
	
}
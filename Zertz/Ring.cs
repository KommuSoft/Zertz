using System;
using System.Drawing;

namespace Zertz.Mathematics {
	
	public class Ring : IShape2D {
		
		private float x, y, r1, r2, r1s, r2s;
		
		public RectangleF Bounds {
			get {
				return new RectangleF(x-r2,y-r2,2.0f*r2,2.0f*r2);
			}
		}
		
		public Ring (float x, float y, float r1, float r2) {
			this.x = x;
			this.y = y;
			this.r1 = r1;
			this.r2 = r2;
			this.r1s = this.r1*this.r1;
			this.r2s = this.r2*this.r2;
		}
		
		public bool Contains (float x, float y) {
			float dx = x-this.x;
			float dy = y-this.y;
			float rs = dx*dx+dy*dy;
			return (r1s <= rs && rs <= r2s);
		}
		
	}
}


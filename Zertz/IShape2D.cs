using System;
using System.Drawing;

namespace Zertz.Mathematics {
	
	public interface IShape2D {
		
		RectangleF Bounds {
			get;
		}
		
		bool Contains (float x, float y);
		
	}
}


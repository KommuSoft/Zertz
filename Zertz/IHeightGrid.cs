using System;
using OpenTK;
using Zertz.Utils;
using Zertz.Mathematics;

namespace Zertz.Rendering {
	
	public interface IHeightGrid {
		
		Vector3[] GetVectors (IShape2D shape);
		
	}
	
}
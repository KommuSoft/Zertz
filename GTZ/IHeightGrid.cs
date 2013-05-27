using System;
using OpenTK;
using GTZ.Utils;
using GTZ.Mathematics;

namespace GTZ.Rendering {
	
	public interface IHeightGrid {
		
		Vector3[] GetVectors (IShape2D shape);
		
	}
	
}
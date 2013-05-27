using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace GTZ.Rendering {
	
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexT2fN3fV3f {
		
		public Vector2 Texture;
		public Vector3 Normal;
		public Vector3 Position;
		
	}
	
}
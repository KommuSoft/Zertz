using System;
using OpenTK;

namespace GTZ.Rendering {

	public interface IRenderable {
		
		void Render (FrameEventArgs e);
		
	}
	
}
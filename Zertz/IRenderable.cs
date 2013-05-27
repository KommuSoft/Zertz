using System;
using OpenTK;

namespace Zertz.Rendering {

	public interface IRenderable {
		
		void Render (FrameEventArgs e);
		
	}
	
}
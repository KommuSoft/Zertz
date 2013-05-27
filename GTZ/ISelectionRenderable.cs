using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public interface ISelectionRenderable : IRenderable {
		
		void SelectionRender (FrameEventArgs e);
		
	}
	
}
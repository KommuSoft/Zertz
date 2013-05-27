using System;
using OpenTK;

namespace Zertz.Rendering {
	
	public interface ISelectionRenderable : IRenderable {
		
		void SelectionRender (FrameEventArgs e);
		
	}
	
}
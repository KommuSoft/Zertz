using System;
using OpenTK;

namespace Zertz.Rendering {
	
	public interface IRenderMoveable {
		
		float MoveTime {
			get;
			set;
		}
		RenderMover RenderMover {
			get;
			set;
		}
		void FinishedMoveChain (Vector3 v);
		
	}
	
}
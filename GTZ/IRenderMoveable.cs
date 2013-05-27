using System;
using OpenTK;

namespace GTZ.Rendering {
	
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
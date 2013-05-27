using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public interface IPhysical {
		
		void AdvanceTime (float time, Vector3 gravity, Vector3 wind);
		
	}
	
}
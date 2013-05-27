using System;
using OpenTK;

namespace Zertz.Rendering {
	
	public interface IPhysical {
		
		void AdvanceTime (float time, Vector3 gravity, Vector3 wind);
		
	}
	
}
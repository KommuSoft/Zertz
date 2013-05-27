using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public interface ILocatable {
		
		Vector3 Location {
			get;
			set;
		}
		
		void MoveTo (Vector3 location);
		
	}
	
}
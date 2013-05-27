using System;

namespace GTZ.Zertz {
	
	public abstract class ZertzMove {
		
		public abstract bool CanBeExecuted (ZertzGame game);
		public abstract void Execute (ZertzGame game);
		
	}
}


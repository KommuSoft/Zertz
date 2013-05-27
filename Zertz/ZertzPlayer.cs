using System;

namespace Zertz.Zertz {
	
	public abstract class ZertzPlayer : IPlayer {
		
		public abstract string Name {
			get;
		}
		
		public ZertzPlayer () {
			
		}
		
		public void StartTurn ()
		{
			
		}
		public void StopTurn ()
		{
			
		}
		public void ReceiveMessage (IPlayer sender, string message) {
			
		}
		
	}
	
}
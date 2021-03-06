using System;

namespace Zertz {
	
	public interface IPlayer {
		
		string Name {
			get;
		}
		
		void StartTurn ();
		void StopTurn ();
		void ReceiveMessage (IPlayer sender, string message);
		
	}
	
}
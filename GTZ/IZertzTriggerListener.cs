using System;

namespace GTZ.Zertz {
	
	public interface IZertzTriggerListener {
		
		void TakeBall (ZertzBallType ball);
		void SelectPiece (HexLocation location);
		void CaptureLand (HexLocation location);
		void EndTurn ();
		
	}
	
}
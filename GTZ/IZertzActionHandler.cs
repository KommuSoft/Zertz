using System;

namespace GTZ.Zertz {
	
	public interface IZertzActionHandler {
		
		void PerformHop (HexLocation fr, HexLocation to);
		void PutBall (ZertzBallContainerType containertype, ZertzBallType type, HexLocation location);
		void CaptureBall (HexLocation location, ZertzBallContainerType type);
		void RemovePiece (HexLocation location);
		void ChangePlayer (int newPlayer);
		void ChangeMoveState (int newState);
		
	}
	
}
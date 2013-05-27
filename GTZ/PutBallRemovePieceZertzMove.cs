using System;

namespace GTZ.Zertz {
	
	public class PutBallRemovePieceZertzMove : ZertzMove {
		
		private HexLocation put, rem;
		private ZertzBallType balltype;
		private static readonly char[] ballTypeChars = new char[] {'b','g','w'};
		
		public PutBallRemovePieceZertzMove (HexLocation put, ZertzBallType ball, HexLocation rem) {
		}
		
		public override string ToString () {
			return String.Format("{0}:{1}/{2}",this.put,ballTypeChars[(byte) this.balltype],this.rem);
		}
		public override bool CanBeExecuted (ZertzGame game) {
			return false;//TODO: implement
		}
		public override void Execute (ZertzGame game) {
			//TODO: implement
		}
		
	}
	
}
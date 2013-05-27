using System;

namespace Zertz.Zertz {
	
	public struct ZertzPiece {
		
		public static readonly ZertzPiece Vacant = new ZertzPiece(0x01);
		public static readonly ZertzPiece Dead = new ZertzPiece(0x00);
		private byte state;
		
		public bool ContainsBall {
			get {
				return ((this.state&0xe) != 0x00);
			}
		}
		public bool IsVacant {
			get {
				return ((this.state&0x0f) == 0x01);
			}
		}
		public bool IsAlive {
			get {
				return ((this.state&0x01) != 0x00);
			}
		}
		public byte State {
			get {
				return this.state;
			}
		}
		public ZertzBallType BallType {
			get {
				return (ZertzBallType) ((this.state&0xe)>>0x02);
			}
		}
		public bool ContainsBlackBall {
			get {
				return ((this.state&0x02) != 0x00);
			}
		}
		public bool ContainsGrayBall {
			get {
				return ((this.state&0x04) != 0x00);
			}
		}
		public bool ContainsWhiteBall {
			get {
				return ((this.state&0x08) != 0x00);
			}
		}
		
		private ZertzPiece (byte state) {
			this.state = state;
		}
		
		public bool CanKill () {
			return (this.IsAlive && !this.ContainsBall);
		}
		public void Kill () {
			if(this.ContainsBall) {
				throw new InvalidZertzActionException("Unable to kill a piece containing a ball");
			}
			else {
				this.state &= 0xfe;
			}
		}
		public bool CanPutBall () {
			return (this.IsAlive && !this.ContainsBall);
		}
		public void PutBall (ZertzBallType ballType) {
			if(!this.CanPutBall()) {
				throw new InvalidZertzActionException("Unable to put a ball on a piece already containing a ball");
			}
			else {
				this.state |= (byte) (0x02<<(byte) ballType);
			}
		}
		public bool CanDropBall () {
			return (this.IsAlive && this.ContainsBall);
		}
		public ZertzBallType DropBall () {
			if(!this.CanDropBall()) {
				throw new InvalidZertzActionException("Can't drop the ball of a piece containing none.");
			}
			ZertzBallType t = (ZertzBallType) ((this.state&0xe)>>0x02);
			this.state &= 0xf1;
			return t;
		}
		
	}
	
}
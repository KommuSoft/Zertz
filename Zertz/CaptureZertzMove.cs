using System;
using System.Text;

namespace GTZ.Zertz {
	
	public class CaptureZertzMove : ZertzMove {
		
		private readonly HexLocation offset;
		private readonly HexDirection[] hops;
		private HexLocation[] locationCache;
		private static readonly string[] directionNames = new string[] {"l","lu","ru","r","rd","ld"};
		
		public CaptureZertzMove (HexLocation offset, params HexDirection[] hops) {
			this.offset = offset;
			this.hops = hops;
		}
		
		public override bool CanBeExecuted (ZertzGame game) {
			ZertzBoard zb = game.Board;
			HexLocation zl = offset, dzl;
			ZertzPiece zp = zb[zl];
			if(!zp.IsAlive || !zp.ContainsBall) {
				this.locationCache = null;
				return false;
			}
			this.locationCache = new HexLocation[hops.Length+0x01];
			for(int i = 0x00; i < hops.Length; i++) {
				dzl = HexLocation.NeighbourDirections[(byte) hops[i]];
				zl += dzl;
				zp = zb[zl];
				this.locationCache[i] = zl;
				if(!zp.IsAlive || !zp.ContainsBall) {
					this.locationCache = null;
					return false;
				}
				zl += dzl;
				zp = zb[zl];
				if(!zp.IsAlive || zp.ContainsBall) {
					this.locationCache = null;
					return false;
				}
			}
			this.locationCache[hops.Length] = zl;
			return true;
		}
		public override void Execute (ZertzGame game) {
			if(this.locationCache == null && !this.CanBeExecuted(game)) {
				throw new InvalidZertzException("Can't execute invalid move.");
			}
			else {
				ZertzBoard zb = game.Board;
				zb[this.locationCache[this.hops.Length]].PutBall(zb[offset].DropBall());
				for(int i = 0x00; i < hops.Length; i++) {
					ZertzBallType zbt = game.Board[this.locationCache[i++]].DropBall();//TODO: collect
				}
			}
			
		}
		public override string ToString () {
			StringBuilder sb = new StringBuilder(this.offset.ToString());
			for(int i = 0x00; i < hops.Length; i++) {
				sb.Append(String.Format("-{0}",directionNames[(byte) hops[i]]));
			}
			return sb.ToString();
		}
		
	}
	
}
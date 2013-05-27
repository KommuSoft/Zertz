using System;
using System.Collections.Generic;

namespace Zertz.Rendering.Zertz {
	
	public class ZertzBoardRenderer {
		
		private readonly ZertzBallRenderer[,] balls;
		private readonly ZertzRingRenderer[,] rings;
		private readonly Dictionary<ZertzRingRenderer,HexLocation> ringlocs;
		private readonly Dictionary<ZertzBallRenderer,HexLocation> balllocs;
		
		public ZertzBoardRenderer (int width, int height) {
			this.balls = new ZertzBallRenderer[width,height];
			this.rings = new ZertzRingRenderer[width,height];
			this.balllocs = new Dictionary<ZertzBallRenderer,HexLocation>();
			this.ringlocs = new Dictionary<ZertzRingRenderer,HexLocation>();
		}
		
		public void PutBall (ZertzBallRenderer ball, HexLocation loc) {
			balllocs.Add(ball,loc);
			balls[loc.X,loc.Y] = ball;
		}
		public ZertzBallRenderer RemoveBall (HexLocation loc) {
			ZertzBallRenderer zbr = balls[loc.X,loc.Y];
			balllocs.Remove(zbr);
			balls[loc.X,loc.Y] = null;
			return zbr;
		}
		public ZertzBallRenderer GetBallAt (HexLocation loc) {
			return this.balls[loc.X,loc.Y];
		}
		public void PutRing (ZertzRingRenderer ring, HexLocation loc) {
			ringlocs.Add(ring,loc);
			rings[loc.X,loc.Y] = ring;
		}
		public ZertzRingRenderer RemoveRing (HexLocation loc) {
			ZertzRingRenderer zrr = rings[loc.X,loc.Y];
			ringlocs.Remove(zrr);
			rings[loc.X,loc.Y] = null;
			return zrr;
		}
		public ZertzRingRenderer GetRingAt (HexLocation loc) {
			return this.rings[loc.X,loc.Y];
		}
		
	}
	
}
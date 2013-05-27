using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Zertz.Zertz {
	
	[TestFixture()]
	public class ZertzBoardTest {
		
		private HexLocation[] generateAllLocations () {
			HexLocation[] hls = new HexLocation[0x25];
			int k = 0x00;
			sbyte j, o1, o2;
			for(sbyte i = 0x00; i <= 0x06; i++) {
				o1 = (sbyte) Math.Max(0x00,0x03-i);
				o2 = (sbyte) Math.Min(0x06,0x09-i);
				for(j = o1; j <= o2; j++) {
					hls[k++] = new HexLocation(i,j);
				}
			}
			return hls;
		}
		private ZertzBoard buildSampleBoard0 () {
			HexLocation[] hls;
			return ZertzBoard.OffsetBoard(out hls);
		}
		private ZertzBoard buildSampleBoard1 () {
			HexLocation[] hls;
			ZertzBoard board = ZertzBoard.OffsetBoard(out hls);
			ZertzPiece dead = ZertzPiece.Dead;
			ZertzPiece blac = ZertzPiece.Vacant;
			blac.PutBall(ZertzBallType.Black);
			ZertzPiece gray = ZertzPiece.Vacant;
			gray.PutBall(ZertzBallType.Gray);
			ZertzPiece whit = ZertzPiece.Vacant;
			whit.PutBall(ZertzBallType.White);
			board[0x00,0x04] = dead;
			board[0x00,0x05] = blac;
			board[0x00,0x06] = gray;
			board[0x01,0x02] = whit;
			board[0x01,0x06] = dead;
			board[0x02,0x01] = dead;
			board[0x03,0x00] = dead;
			board[0x03,0x01] = whit;
			board[0x04,0x00] = dead;
			board[0x04,0x03] = dead;
			board[0x04,0x04] = dead;
			board[0x04,0x05] = dead;
			board[0x05,0x02] = blac;
			board[0x05,0x03] = dead;
			board[0x05,0x04] = dead;
			board[0x06,0x02] = dead;
			board[0x06,0x03] = dead;
			return board;
		}
		private ZertzBoard buildSampleBoard2 () {
			HexLocation[] hls;
			ZertzBoard board = ZertzBoard.OffsetBoard(out hls);
			ZertzPiece dead = ZertzPiece.Dead;
			ZertzPiece blac = ZertzPiece.Vacant;
			blac.PutBall(ZertzBallType.Black);
			ZertzPiece gray = ZertzPiece.Vacant;
			gray.PutBall(ZertzBallType.Gray);
			ZertzPiece whit = ZertzPiece.Vacant;
			whit.PutBall(ZertzBallType.White);
			board[0x00,0x03] = dead;
			board[0x00,0x06] = dead;
			board[0x01,0x06] = dead;
			board[0x02,0x06] = dead;
			board[0x03,0x06] = dead;
			board[0x05,0x02] = dead;
			board[0x06,0x02] = dead;
			board[0x06,0x01] = dead;
			board[0x03,0x05] = dead;
			board[0x04,0x05] = dead;
			board[0x04,0x04] = dead;
			board[0x05,0x04] = dead;
			board[0x05,0x03] = dead;
			board[0x06,0x03] = dead;
			board[0x01,0x03] = whit;
			board[0x02,0x04] = whit;
			board[0x02,0x03] = gray;
			board[0x05,0x00] = gray;
			board[0x03,0x01] = blac;
			return board;
		}
		private ZertzBoard buildSampleBoard3 () {//DIAGRAM 4 WITHOUT REMOVED PIECE
			HexLocation[] hls;
			ZertzBoard board = ZertzBoard.OffsetBoard(out hls);
			ZertzPiece dead = ZertzPiece.Dead;
			ZertzPiece blac = ZertzPiece.Vacant;
			blac.PutBall(ZertzBallType.Black);
			ZertzPiece gray = ZertzPiece.Vacant;
			gray.PutBall(ZertzBallType.Gray);
			ZertzPiece whit = ZertzPiece.Vacant;
			whit.PutBall(ZertzBallType.White);
			board[0x00,0x03] = dead;
			board[0x00,0x06] = dead;
			board[0x01,0x06] = dead;
			board[0x02,0x06] = dead;
			board[0x03,0x06] = dead;
			board[0x05,0x02] = dead;
			board[0x06,0x02] = dead;
			board[0x06,0x01] = dead;
			board[0x03,0x05] = dead;
			board[0x04,0x05] = dead;
			board[0x04,0x04] = dead;
			board[0x05,0x04] = dead;
			board[0x05,0x03] = dead;
			board[0x06,0x03] = dead;
			board[0x04,0x01] = dead;
			board[0x05,0x01] = dead;
			board[0x06,0x00] = dead;
			board[0x04,0x02] = dead;
			
			board[0x00,0x05] = whit;
			board[0x01,0x02] = whit;
			board[0x05,0x00] = gray;
			board[0x00,0x04] = blac;
			board[0x03,0x03] = blac;
			return board;
		}
		private ZertzBoard buildSampleBoard4 () {//DIAGRAM 4 WITH REMOVED PIECE
			HexLocation[] hls;
			ZertzBoard board = ZertzBoard.OffsetBoard(out hls);
			ZertzPiece dead = ZertzPiece.Dead;
			ZertzPiece blac = ZertzPiece.Vacant;
			blac.PutBall(ZertzBallType.Black);
			ZertzPiece gray = ZertzPiece.Vacant;
			gray.PutBall(ZertzBallType.Gray);
			ZertzPiece whit = ZertzPiece.Vacant;
			whit.PutBall(ZertzBallType.White);
			board[0x00,0x03] = dead;
			board[0x00,0x06] = dead;
			board[0x01,0x06] = dead;
			board[0x02,0x06] = dead;
			board[0x03,0x06] = dead;
			board[0x05,0x02] = dead;
			board[0x06,0x02] = dead;
			board[0x06,0x01] = dead;
			board[0x03,0x05] = dead;
			board[0x04,0x05] = dead;
			board[0x04,0x04] = dead;
			board[0x05,0x04] = dead;
			board[0x05,0x03] = dead;
			board[0x06,0x03] = dead;
			board[0x04,0x01] = dead;
			board[0x05,0x01] = dead;
			board[0x06,0x00] = dead;
			board[0x04,0x02] = dead;
			board[0x04,0x00] = dead;//removed
			
			board[0x00,0x05] = whit;
			board[0x01,0x02] = whit;
			board[0x05,0x00] = gray;
			board[0x00,0x04] = blac;
			board[0x03,0x03] = blac;
			return board;
		}
		private ZertzBoard buildSampleBoard9 () {
			HexLocation[] hls;
			ZertzBoard board = ZertzBoard.OffsetBoard(out hls);
			ZertzPiece blac = ZertzPiece.Vacant;
			blac.PutBall(ZertzBallType.Black);
			for(int i = 0x00; i < 0x07; i++) {
				for(int j = 0x00; j < 0x07; j++) {
					if(board[i,j].IsAlive) {
						board[i,j] = blac;
					}
				}
			}
			return board;
		}
		private void innerTestIsFree (ZertzBoard board, HexLocation[] frees) {
			HashSet<HexLocation> hls = new HashSet<HexLocation>();
			foreach(HexLocation l in frees) {
				hls.Add(l);
			}
			HexLocation hl;
			for(sbyte i = 0x00; i < 0x07; i++) {
				for(sbyte j = 0x00; j < 0x07; j++) {
					hl = new HexLocation(i,j);
					Assert.AreEqual(hls.Contains(hl),board.IsFree(hl));
				}
			}
		}
		private void innerTestCanCapture (ZertzBoard board, HexLocation[] capture) {
			HashSet<HexLocation> hls = new HashSet<HexLocation>();
			foreach(HexLocation l in capture) {
				hls.Add(l);
			}
			HexLocation hl;
			for(sbyte i = 0x00; i < 0x07; i++) {
				for(sbyte j = 0x00; j < 0x07; j++) {
					hl = new HexLocation(i,j);
					Assert.AreEqual(hls.Contains(hl),board.CanCapture(hl));
				}
			}
		}
		private void innerTestCanHop2 (ZertzBoard board, HexLocation[] hops) {
			HashSet<HexLocation> hls = new HashSet<HexLocation>();
			foreach(HexLocation l in hops) {
				hls.Add(l);
			}
			HexLocation hl;
			for(sbyte i = 0x00; i < 0x07; i++) {
				for(sbyte j = 0x00; j < 0x07; j++) {
					hl = new HexLocation(i,j);
					Assert.AreEqual(hls.Contains(hl),board.CanHop(hl));
				}
			}
		}
		private void innerTestIsVacant (ZertzBoard board, HexLocation[] vacant) {
			HashSet<HexLocation> hls = new HashSet<HexLocation>();
			foreach(HexLocation l in vacant) {
				hls.Add(l);
			}
			HexLocation hl;
			for(sbyte i = 0x00; i < 0x07; i++) {
				for(sbyte j = 0x00; j < 0x07; j++) {
					hl = new HexLocation(i,j);
					Assert.AreEqual(hls.Contains(hl),board.IsVacant(hl));
				}
			}
		}
		
		[Test()]
		public void TestIsFree () {
			innerTestIsFree(buildSampleBoard0(),new HexLocation[] {new HexLocation(0,3),new HexLocation(0,4),new HexLocation(0,5),new HexLocation(0,6),new HexLocation(1,6),new HexLocation(2,6),new HexLocation(3,6),new HexLocation(4,5),new HexLocation(5,4),new HexLocation(6,3),new HexLocation(6,2),new HexLocation(6,1),new HexLocation(6,0),new HexLocation(5,0),new HexLocation(4,0),new HexLocation(3,0),new HexLocation(2,1),new HexLocation(1,2)});
			innerTestIsFree(buildSampleBoard1(),new HexLocation[] {new HexLocation(0,3),new HexLocation(5,0),new HexLocation(6,0),new HexLocation(6,1),new HexLocation(2,6),new HexLocation(3,6),new HexLocation(3,5),new HexLocation(3,4)});
			//TODO: 2-8
			innerTestIsFree(buildSampleBoard9(),new HexLocation[] {});
		}
		[Test()]
		public void TestHasFree () {
			Assert.IsTrue(buildSampleBoard0().HasFree());
			Assert.IsTrue(buildSampleBoard1().HasFree());
			Assert.IsTrue(buildSampleBoard2().HasFree());
			Assert.IsTrue(buildSampleBoard3().HasFree());
			Assert.IsTrue(buildSampleBoard4().HasFree());
			Assert.IsFalse(buildSampleBoard9().HasFree());
		}
		[Test()]
		public void TestIsVacant () {
			innerTestIsVacant(buildSampleBoard0(),this.generateAllLocations());
			//TODO: 1-8
			innerTestIsVacant(buildSampleBoard9(),new HexLocation[] {});
		}
		[Test()]
		public void TestCanHop1 () {
			Assert.IsFalse(buildSampleBoard0().CanHop());
			Assert.IsFalse(buildSampleBoard1().CanHop());
			Assert.IsTrue(buildSampleBoard2().CanHop());
			Assert.IsFalse(buildSampleBoard3().CanHop());
			Assert.IsFalse(buildSampleBoard4().CanHop());
			Assert.IsFalse(buildSampleBoard9().CanHop());
		}
		[Test()]
		public void TestCanCapture () {
			innerTestCanCapture(buildSampleBoard0(),new HexLocation[] {});
			innerTestCanCapture(buildSampleBoard1(),new HexLocation[] {});
			innerTestCanCapture(buildSampleBoard2(),new HexLocation[] {});
			innerTestCanCapture(buildSampleBoard3(),new HexLocation[] {});
			innerTestCanCapture(buildSampleBoard4(),new HexLocation[] {new HexLocation(5,0)});
			innerTestCanCapture(buildSampleBoard9(),this.generateAllLocations());
		}
		[Test()]
		public void TestCanHop2 () {
			innerTestCanHop2(buildSampleBoard0(),new HexLocation[] {});
			innerTestCanHop2(buildSampleBoard1(),new HexLocation[] {});
			innerTestCanHop2(buildSampleBoard2(),new HexLocation[] {new HexLocation(2,4),new HexLocation(2,3),new HexLocation(1,3)});
			innerTestCanHop2(buildSampleBoard3(),new HexLocation[] {});
			innerTestCanHop2(buildSampleBoard4(),new HexLocation[] {});
			innerTestCanHop2(buildSampleBoard9(),new HexLocation[] {});
		}
		[Test()]
		public void TestHasVacant () {
			Assert.IsTrue(buildSampleBoard0().HasVacant());
			Assert.IsTrue(buildSampleBoard1().HasVacant());
			Assert.IsTrue(buildSampleBoard2().HasVacant());
			Assert.IsTrue(buildSampleBoard3().HasVacant());
			Assert.IsTrue(buildSampleBoard4().HasVacant());
			Assert.IsFalse(buildSampleBoard9().HasVacant());
		}
		
	}
	
}
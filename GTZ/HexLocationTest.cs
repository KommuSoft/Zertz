using System;
using NUnit.Framework;

namespace GTZ.Zertz {
	
	[TestFixture()]
	public class HexLocationTest {
		
		[Test()]
		public void TestConstructor () {
			HexLocation zl;
			for(sbyte x = sbyte.MinValue; x < sbyte.MaxValue; x++) {
				for(sbyte y = sbyte.MinValue; y < sbyte.MaxValue; y++) {
					zl = new HexLocation(x,y);
					Assert.AreEqual(x,zl.X);
					Assert.AreEqual(y,zl.Y);
				}
			}
		}
		[Test()]
		public void TestDirection () {
			for(int i = 0x00; i < 0x06; i++) {
				Assert.AreEqual((HexDirection) i,HexLocation.NeighbourDirections[i].HexDirection);
			}
		}
		[Test()]
		public void TestOperators () {
			HexLocation hla = new HexLocation(0,0);
			HexLocation hlb = new HexLocation(1,0);
			HexLocation hlc = new HexLocation(-1,0);
			HexLocation hld = new HexLocation(0,1);
			HexLocation hle = new HexLocation(0,-1);
			HexLocation hlf = new HexLocation(1,-1);
			HexLocation hlg = new HexLocation(-1,1);
			HexLocation hlh = new HexLocation(2,0);
			HexLocation hli = new HexLocation(2,1);
			HexLocation hlj = new HexLocation(2,2);
			HexLocation hlk = new HexLocation(1,3);
			HexLocation hll = new HexLocation(0,4);
			HexLocation hlm = new HexLocation(-4,4);
			Assert.AreEqual(hlb,hlb-hla);
			Assert.AreEqual(hlc,hlc-hla);
			Assert.AreEqual(hld,hld-hla);
			Assert.AreEqual(hle,hle-hla);
			Assert.AreEqual(hlf,hlf-hla);
			Assert.AreEqual(hlg,hlg-hla);
			Assert.AreEqual(hlh,hlh-hla);
			Assert.AreEqual(hlb,hlb+hla);
			Assert.AreEqual(hlc,hlc+hla);
			Assert.AreEqual(hld,hld+hla);
			Assert.AreEqual(hle,hle+hla);
			Assert.AreEqual(hlf,hlf+hla);
			Assert.AreEqual(hlg,hlg+hla);
			Assert.AreEqual(hlh,hlh+hla);
			Assert.AreEqual(hlb,!hlh);
			Assert.AreEqual(HexLocation.Invalid,!hli);
			Assert.AreEqual(HexLocation.Invalid,!hlj);
			Assert.AreEqual(HexLocation.Invalid,!hlk);
			Assert.AreEqual(hld,!hll);
			Assert.AreEqual(hlg,!hlm);
		}
		[Test()]
		public void TestShiftOperator () {
			for(int i = 0x00; i < 0x08; i++) {
				sbyte bm = (sbyte) (sbyte.MinValue>>i);
				sbyte bM = (sbyte) (sbyte.MaxValue>>i);
				for(sbyte b = bm; b < bM; b++) {
					for(sbyte c = bm; c < bM; c++) {
						HexLocation hl = new HexLocation(b,c)<<i;
						Assert.AreEqual(b<<i,hl.X);
						Assert.AreEqual(c<<i,hl.Y);
					}
				}
			}
		}
		
	}
	
}
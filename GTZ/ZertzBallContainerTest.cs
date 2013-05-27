using System;
using NUnit.Framework;

namespace GTZ.Zertz {
	
	[TestFixture()]
	public class ZertzBallContainerTest {
		
		[Test()]
		public void TestConstructor () {
			ZertzBallContainer zbc;
			for(byte b = byte.MinValue; b < byte.MaxValue; b++) {
				for(byte c = byte.MinValue; c < byte.MaxValue; c++) {
					for(byte d = byte.MinValue; d < byte.MaxValue; d++) {
						zbc = new ZertzBallContainer(b,c,d);
						Assert.AreEqual(b,zbc[ZertzBallType.White]);
						Assert.AreEqual(c,zbc[ZertzBallType.Gray]);
						Assert.AreEqual(d,zbc[ZertzBallType.Black]);
					}
				}
			}
		}
		
	}
	
}
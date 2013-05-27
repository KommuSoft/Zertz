using System;

namespace Zertz.Utils {
	
	public static class UniversalRandom {
		
		private static Random rand = new Random();
		
		public static double NextDouble () {
			return rand.NextDouble();
		}
		public static double NextSignedDouble () {
			return 2.0f*rand.NextDouble()-1.0f;
		}
		
	}
	
}
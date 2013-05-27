using System;
using System.IO;

namespace Zertz {
	
	public interface IStreamable {
		
		void ReadData (BinaryReader br);
		void WriteData (BinaryWriter bw);
		
	}
	
}
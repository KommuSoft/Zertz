using System;
using System.IO;

namespace GTZ {
	
	public interface IStreamable {
		
		void ReadData (BinaryReader br);
		void WriteData (BinaryWriter bw);
		
	}
	
}
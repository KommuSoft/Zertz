using System;
using System.Runtime.Serialization;

namespace GTZ.Zertz {
	
	public class InvalidZertzException : Exception {
		
		public InvalidZertzException () : base() {}
		public InvalidZertzException (string message) : base(message) {}
		public InvalidZertzException (SerializationInfo info, StreamingContext context) : base(info,context) {}
		public InvalidZertzException (string message, Exception innerException) : base(message,innerException) {}
		
	}
	
}
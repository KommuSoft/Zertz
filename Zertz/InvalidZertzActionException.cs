using System;
using System.Runtime.Serialization;

namespace Zertz.Zertz {
	
	public class InvalidZertzActionException : InvalidZertzException {
		
		public InvalidZertzActionException () : base() {}
		public InvalidZertzActionException (string message) : base(message) {}
		public InvalidZertzActionException (SerializationInfo info, StreamingContext context) : base(info,context) {}
		public InvalidZertzActionException (string message, Exception innerException) : base(message,innerException) {}
		
	}
	
}
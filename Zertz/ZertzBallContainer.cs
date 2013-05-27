using System;

namespace Zertz.Zertz {
	
	public class ZertzBallContainer {
		
		private int counters;
		
		public byte this [ZertzBallType ballType] {
			get {
				int sh = ((byte) ballType)<<0x03;
				return (byte) ((counters>>sh)&0xff);
			}
			set {
				int sh = ((byte) ballType)<<0x03;
				this.counters &= ~(0xff<<sh);
				this.counters |= value<<sh;
			}
		}
		public byte NumberOfWhite {
			get {
				return (byte) (this.counters>>0x10);
			}
		}
		public byte NumberOfGray {
			get {
				return (byte) ((this.counters>>0x08)&0xff);
			}
		}
		public byte NumberOfBlack {
			get {
				return (byte) (this.counters&0xff);
			}
		}
		public int Total {
			get {
				return ((this.counters&0xff)+((this.counters>>0x08)&0xff)+(this.counters>>0x10));
			}
		}
		
		private ZertzBallContainer (int counters) {
			this.counters = counters;
		}
		public ZertzBallContainer () : this(0x00) {}
		public ZertzBallContainer (byte white, byte gray, byte black) : this((white<<0x10)|(gray<<0x08)|black) {}
		
		public void Add (ZertzBallContainer container) {
			this.counters += container.counters;
		}
		public void Add (ZertzBallType ball) {
			this.counters += 0x01<<(((byte) ball)<<0x03);
		}
		public void Add (ZertzBallType ball, byte number) {
			this.counters += number<<(((byte) ball)<<0x03);
		}
		public static ZertzBallContainer Offset () {
			return new ZertzBallContainer(0x050709);
		}
		public static ZertzBallContainer Empty () {
			return new ZertzBallContainer(0x000000);
		}
		public bool CanFinish () {
			byte white = this.NumberOfWhite;
			if(white >= 0x03) {
				return true;
			}
			else {
				byte gray = this.NumberOfGray;
				if(gray >= 0x04) {
					return true;
				}
				else {
					byte black = this.NumberOfBlack;
					return (black >= 0x05 || (white >= 0x02 && gray >= 0x02 && black >= 0x02));
				}
			}
		}
		public override bool Equals (object obj) {
			if(obj is ZertzBallContainer) {
				return (((ZertzBallContainer) obj).counters == this.counters);
			}
			return false;
		}
		public override int GetHashCode () {
			return this.counters;
		}
		public static bool operator == (ZertzBallContainer c1, ZertzBallContainer c2) {
			return (c1.counters == c2.counters);
		}
		public static bool operator != (ZertzBallContainer c1, ZertzBallContainer c2) {
			return (c1.counters != c2.counters);
		}
		
	}
	
}
using System;

namespace Zertz {
	
	public struct HexLocation : IComparable<HexLocation> {
		
		private ushort coordinate;
		
		public static readonly HexLocation Left			= new HexLocation(0xff00);
		public static readonly HexLocation Right		= new HexLocation(0x0100);
		public static readonly HexLocation LeftUp		= new HexLocation(0xff01);
		public static readonly HexLocation RightUp		= new HexLocation(0x0001);
		public static readonly HexLocation LeftDown		= new HexLocation(0x00ff);
		public static readonly HexLocation RightDown	= new HexLocation(0x01ff);
		public static readonly HexLocation Invalid		= new HexLocation(0x8080);
		public static readonly HexLocation[] NeighbourDirections = new HexLocation[] {HexLocation.Left,HexLocation.LeftUp,HexLocation.RightUp,HexLocation.Right,HexLocation.RightDown,HexLocation.LeftDown};
		
		public sbyte X {
			get {
				return unchecked((sbyte) (this.coordinate>>0x08));
			}
		}
		public sbyte Y {
			get {
				return unchecked((sbyte) (this.coordinate&0xff));
			}
		}
		public ushort Signature {
			get {
				return this.coordinate;
			}
		}
		public bool IsNeighbourDirections {
			get {
				sbyte x = this.X;
				if(x < -0x01 || x > 0x01) {
					return false;
				}
				sbyte y = this.Y;
				return (y >= -0x01 && y <= 0x01 && (x|y) != 0x00 && x*y <= 0x00);
			}
		}
		public HexDirection HexDirection {
			get {
				sbyte x = this.X, y = this.Y;
				//int state = ((x&0x03)<<0x02)|(y&0x03);
				int state = (~(x|y))&0x02;
				state |= state>>0x01;
				state &= 0x02|(x&0x01);
				state |= ((x>>0x01)&y)&0x01;
				state |= (((~x&y)>>0x01)&(~x))&0x01;
				state |= (y&0x2)<<0x01;
				return (HexDirection) state;
			}
		}
		
		private HexLocation (ushort coordinate) {
			this.coordinate = coordinate;
		}
		public HexLocation (sbyte x, sbyte y) {
			byte xb = unchecked((byte) x);
			byte yb = unchecked((byte) y);
			this.coordinate = unchecked((ushort) ((xb<<0x08)|yb));
		}
		
		public HexLocation[] GetNeighbours () {
			return new HexLocation[] {this+HexLocation.Left,this+HexLocation.LeftUp,this+HexLocation.RightUp,this+HexLocation.Right,this+HexLocation.RightDown,this+HexLocation.LeftDown};
		}
		public override bool Equals (object obj) {
			if(obj is HexLocation) {
				return this.coordinate == ((HexLocation) obj).coordinate;
			}
			else {
				return false;
			}
		}
		public override int GetHashCode () {
			return this.coordinate|((~this.coordinate)<<0x10);
		}
		public int CompareTo (HexLocation other) {
			return this.coordinate.CompareTo(other.coordinate);
		}
		public static HexLocation Parse (string s) {
			if(s == null) {
				throw new ArgumentNullException("s");
			}
			else if(s.Length < 0x02) {
				throw new FormatException();
			}
			return new HexLocation((sbyte) (s[0x00]-'A'),sbyte.Parse(s.Substring(0x01)));
		}
		public override string ToString () {
			return string.Format ("{0}{1}", (char) ('A'+X), Y+0x01);
		}
		public static HexLocation operator - (HexLocation hl) {
			return new HexLocation((sbyte) -hl.X,(sbyte) -hl.Y);
		}
		public static HexLocation operator + (HexLocation zla, HexLocation zlb) {
			ushort za = zla.coordinate;
			ushort zb = zlb.coordinate;
			return new HexLocation((ushort) (((za+zb)&0x00ff)|(((za>>0x08)+(zb>>0x08))<<0x08)));
		}
		public static HexLocation operator - (HexLocation zla, HexLocation zlb) {
			return zla+(-zlb);
		}
		public static HexLocation operator ! (HexLocation hl) {//normalisation
			sbyte x = hl.X;
			sbyte y = hl.Y;
			if(x == 0x00 || y == 0x00 || x == -y) {
				return new HexLocation((sbyte) Math.Sign(x),(sbyte) Math.Sign(y));
			}
			else {
				return HexLocation.Invalid;
			}
		}
		public static explicit operator HexLocation (string s) {
			return Parse(s);
		}
		public static explicit operator string (HexLocation zl) {
			return zl.ToString();
		}
		public static bool operator == (HexLocation hl1, HexLocation hl2) {
			return (hl1.coordinate == hl2.coordinate);
		}
		public static bool operator != (HexLocation hl1, HexLocation hl2) {
			return (hl1.coordinate != hl2.coordinate);
		}
		public static HexLocation operator << (HexLocation hl, int digits) {
			int s = hl.coordinate;
			int mask = (0xff<<digits)&0xff;
			mask |= mask<<0x08;
			return new HexLocation((ushort) ((s<<digits)&mask));
		}
		
	}
	
}
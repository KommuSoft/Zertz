using System;
using OpenTK;

namespace Zertz.Utils {
	
	public static class Maths {
		
		public static readonly float Sqrt3_4 = (float) Math.Sqrt(0.75d);
		public static readonly float Sqrt1_2 = (float) Math.Sqrt(0.5d);
		private static readonly Random random = new Random();
		public static readonly float Pi = 3.141592653589793f;
		private static readonly float rad2scale = 4096f/3.141592653589793f/2f;
		private static readonly float pad = 256*3.141592653589793f;
		private static float[] cosinus;
		private static float[] sinus;
		private static bool trig = false;
		
		private static void buildTrig () {
			Maths.sinus = new float[4096];
			Maths.cosinus = new float[4096];
			for(int i = 0; i < 4096; i++) {
				Maths.sinus[i] = (float) Math.Sin((float) i/rad2scale);
				Maths.cosinus[i] = (float) Math.Cos((float) i/rad2scale);
			}
			Maths.trig = true;
		}
		public static int NumberOfTailingZeros (uint data) {
			uint mask = 0x01;
			int n = 0x00;
			while(mask != 0x00 && (mask&data) == 0x00) {
				mask <<= 0x01;
				n++;
			}
			return n;
		}
		public static int NumberOfHeadingZeros (uint data) {
			uint mask = 0x80000000;
			int n = 0x00;
			while(mask != 0x00 && (mask&data) == 0x00) {
				mask >>= 0x01;
				n++;
			}
			return n;
		}
		public static int NumberOfTailingZeros (byte data) {
			byte mask = 0x01;
			int n = 0x00;
			while(mask != 0x00 && (mask&data) == 0x00) {
				mask <<= 0x01;
				n++;
			}
			return n;
		}
		public static float Deg2Rad (float deg) {
			return deg*0.0174532925194f;
		}
		public static float Rad2Deg (float rad) {
			return rad*57.295779514719f;
		}
		public static float Pythagoras (float a, float b) {
			return (float) Math.Sqrt(a*a+b*b);
		}
		public static int Pythagoras (int a, int b) {
			return (int) Math.Sqrt(a*a+b*b);
		}
		public static float RandomWithDelta (float averidge, float delta) {
			return averidge+Maths.Random()*delta;
		}
		public static float Cos (float angle) {
			if(!Maths.trig)
				Maths.buildTrig();
			return cosinus[(int)((angle+Maths.pad)*Maths.rad2scale)&0xfff];
		}
		public static float Sin (float angle) {
			if(!Maths.trig)
				Maths.buildTrig();
			return sinus[(int)((angle+Maths.pad)*Maths.rad2scale)&0xfff];
		}
		public static int Crop (int num, int min, int max) {
			return ((num < min) ? min : ((num > max) ? max : num ) );
		}
		public static uint Crop (uint num, uint min, uint max) {
			return ((num < min) ? min : ((num > max) ? max : num ) );
		}
		public static uint Crop (uint num, int min, int max) {
			return ((num < min) ? (uint) min : ((num > max) ? (uint) max : num ) );
		}
		public static float Crop (float num, float min, float max) {
			return ((num < min) ? min : ((num > max) ? max : num ) );
		}
		public static bool Inrange (int num, int min, int max) {
			return ((num >= min) && (num < max));
		}
		public static float Random () {
			return (float) (Maths.random.NextDouble()*2-1);
		}
		public static float UnsignedRandom () {
			return (float) Maths.random.NextDouble();
		}
		public static float Random (float min, float max) {
			return (float) (Maths.random.NextDouble()*(max-min)+min);
		}
		public static float Interpolate (float a, float b, float d) {
			float f = (1f-Maths.Cos(d*Maths.Pi))*0.5f;
			return a+f*(b-a);
		}
		public static void PlanarInterpolation (Vector3[] points, out float A, out float B, out float C) {
			int n = points.Length;
			float a = 0.0f, b = 0.0f, c = 0.0f, d = 0.0f, e = 0.0f, f = 0.0f, g = 0.0f, h = 0.0f;
			Vector3 v;
			for(int i = 0x00; i < n; i++) {
				v = points[i];
				a += v.X*v.X;	b += v.X*v.Y;	c += v.X;
				d += v.Y*v.Y;	e += v.Y;		f += v.X*v.Z;
				g += v.Y*v.Z;	h += v.Z;
			}
			float t = 1.0f/(a*(e*e-d*n)+b*b*n-2.0f*b*c*e+c*c*d);
			A = t*(b*(g*n-e*h)+f*(e*e-d*n)+c*(d*h-e*g));
			B = t*(a*(g*n-e*h)+f*(c*e-b*n)+b*c*h-c*c*g);
			C = t*(a*(e*g-d*h)+b*b*h-b*c*g+f*(c*d-b*e));
		}
		public static Vector3 HexVector (HexLocation hl, float spacing, float height) {
			int x = hl.X-0x03;
			int y = hl.Y-0x03;
			return new Vector3((x+0.5f*y)*spacing,height,(float) Sqrt3_4*y*spacing);
		}
		public static int ToNextPow2 (int number) {
			number--;
			number |= number>>0x10;
			number |= number>>0x08;
			number |= number>>0x04;
			number |= number>>0x02;
			number |= number>>0x01;
			number++;
			return number;
		}
		
	}
}


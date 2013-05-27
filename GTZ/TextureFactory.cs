using System;
using System.Drawing;
using System.Drawing.Imaging;
using GTZ.Utils;

namespace GTZ.Rendering {

	public static class TextureFactory {
		
		private static float[,] noiseBuffer;
		private static bool noiseBufferInitialized = false;
		public static float Pi = 3.141592653589793f;
		public static int Minx, Maxx, Miny, Maxy;
		public static float Deg2Rad = TextureFactory.Pi/180f;
		
		public static Texture Sky (int w, int h, float density) {
			uint[] colors = new uint[2];
			colors[0] = 0x003399;
			colors[1] = 0xffffff;
			return TextureFactory.Perlin(w,h,0.5f,2.8f*density,8,1024).Colorize(Colors.MakeGradient(colors,1024));
		}
		public static Texture Marble (int w, int h, float density) {
			uint[] colors = new uint[3];
			colors[0] = 0x111111;
			colors[1] = 0x696070;
			colors[2] = 0xffffff;
			return TextureFactory.Wave(w,h,0.5f,0.64f*density,6,1024).Colorize(Colors.MakeGradient(colors,1024));
		}
		public static Texture Wood (int w, int h, float density) {
			uint[] colors = new uint[3];
			colors[0] = 0x332211;
			colors[1] = 0x523121;
			colors[2] = 0x996633;
			return TextureFactory.Grain(w,h,0.5f,3f*density,3,8,1024).Colorize(Colors.MakeGradient(colors,1024));
		}
		public static Texture Random (int w, int h) {
			int nc = (int) Maths.Random(2,6);
			uint[] colors = new uint[nc];
			for(int i = 0; i < nc; i++)
				colors[i] = Colors.Random();
			float persistency = Maths.Random(0.4f,0.9f);
			float density = Maths.Random(0.5f,3f);
			int samples = (int) Maths.Random(1,7f);
			return TextureFactory.Perlin(w,h,persistency,density,samples,1024).Colorize(Colors.MakeGradient(colors,1024));
		}
		public static Texture Checkerboard (int w, int h, int cellbits, uint oddColor, uint evenColor) {
			Texture t = new Texture(w,h);
			int pos = 0;
			for(int y = 0; y < h; y++)
				for(int x = 0; x < w; x++)
					t.Pixel[pos++] = (((((x>>cellbits)+(y>>cellbits))&1)==0) ? evenColor : oddColor );
			return t;
		}
		public static Texture Perlin (int w, int h, float persistency, float density, int samples, int scale) {
			TextureFactory.initNoiseBuffer();
			Texture t = new Texture(w,h);
			int pos = 0;
			float wavelength = (float) ((w > h) ? w : h )/density;
			for(int y = 0; y < h; y++)
				for(int x = 0; x < w; x++)
					t.Pixel[pos++] = (uint)((float) scale*TextureFactory.perlin2d(x,y,wavelength,persistency,samples));
			return t;
		}
		public static Texture Wave (int w, int h, float persistency, float density, int samples, int scale) {
			TextureFactory.initNoiseBuffer();
			Texture t = new Texture(w,h);
			int pos = 0;
			float wavelength = (float) ((w > h) ? w : h )/density;
			for(int y = 0; y < h; y++)
				for(int x = 0; x < w; x++)
					t.Pixel[pos++] = (uint) ((double) scale*(Math.Sin(32*TextureFactory.perlin2d(x,y,wavelength,persistency,samples))*0.5+0.5));
			return t;
		}
		public static Texture Grain (int w, int h, float persistency, float density, int samples, int levels, int scale) {
			TextureFactory.initNoiseBuffer();
			Texture t = new Texture(w,h);
			int pos = 0;
			float wavelength = (float) ((w > h) ? w : h)/density;
			float perlin;
			for(int y = 0; y < h; y++)
				for(int x = 0; x < w; x++) {
					perlin = (float) levels*TextureFactory.perlin2d(x,y,wavelength,persistency,samples);
					t.Pixel[pos++] = (uint)((float) scale*(perlin-(float)(int)perlin));
				}
			return t;
		}
		private static float perlin2d (float x, float y, float wavelength, float persistence, int samples) {
			float sum = 0;
			float freq = 1f/wavelength;
			float amp = persistence;
			float range = 0;
			for(int i = 0; i < samples; i++) {
				sum += amp*TextureFactory.interpolatedNoise(x*freq,y*freq,i);
				range += amp;
				amp *= persistence;
				freq *= 2;
			}
			return Maths.Crop(sum/persistence*0.5f+0.5f,0,1);
		}
		private static float interpolatedNoise (float x, float y, int octave) {
			int intx = (int) x;
			int inty = (int) y;
			float fracx = x-(float)intx;
			float fracy = y-(float)inty;
			float i1 = Maths.Interpolate(TextureFactory.noise(intx,inty,octave),TextureFactory.noise(intx+1,inty,octave),fracx);
			float i2 = Maths.Interpolate(TextureFactory.noise(intx,inty+1,octave),TextureFactory.noise(intx+1,inty+1,octave),fracx);
			return Maths.Interpolate(i1,i2,fracy);
		}
		private static float smoothNoise (int x, int y, int o) {
			return (TextureFactory.noise(x-1,y-1,o)+TextureFactory.noise(x+1,y-1,o)+TextureFactory.noise(x-1,y+1,o)+TextureFactory.noise(x+1,y+1,o))/16+(TextureFactory.noise(x-1,y,o)+TextureFactory.noise(x+1,y,o)+TextureFactory.noise(x,y-1,o)+TextureFactory.noise(x,y+1,o))/8+TextureFactory.noise(x,y,o)/4;
		}
		private static float noise (int x, int y, int octaves) {
			return TextureFactory.noiseBuffer[octaves&3,(x+y*57)&8191];
		}
		private static float noise (int seed, int octave) {
			int id = octave&3;
			int n = (seed<<13)^seed;
			if(id == 0)
				return (float) (1f- ((n * (n * n * 15731 + 789221) + 1376312589)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			if(id == 1)
				return (float) (1f- ((n * (n * n * 12497 + 604727) + 1345679039)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			if(id == 2)
				return (float) (1f- ((n * (n * n * 19087 + 659047) + 1345679627)&0x7FFFFFFF)*0.000000000931322574615478515625f);
			return (float) (1f- ((n * (n * n * 16267 + 694541) + 1345679501)&0x7FFFFFFF)*0.000000000931322574615478515625f);
		}
		private static void initNoiseBuffer () {
			if(TextureFactory.noiseBufferInitialized)
				return;
			TextureFactory.noiseBuffer=new float[4,8192];
			for(int octave = 0; octave < 4; octave++)
				for(int i = 0; i < 8192; i++)
					TextureFactory.noiseBuffer[octave,i] = TextureFactory.noise(i,octave);
			TextureFactory.noiseBufferInitialized = true;
		}
		
	}
	
}
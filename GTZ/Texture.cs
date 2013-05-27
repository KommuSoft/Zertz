using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using OGL = OpenTK.Graphics.OpenGL;
using GTZ.Utils;

namespace GTZ.Rendering {
	
	public class Texture {
		
		public int Width;
		public int Height;
		public int BitWidth;
		public int BitHeight;
		public uint[] Pixel;
		public string Path = null;
		
		public Texture (int w, int h) {
			this.Height = h;
			this.Width = w;
			this.Pixel = new uint[w*h];
			this.Cls();
		}
		public Texture (int w, int h, uint[] data) {
			this.Height = h;
			this.Width = w;
			this.Pixel = new uint[w*h];
			for(int i = 0; i < w*h; i++)
				this.Pixel[i] = data[i];
		}
		public Texture (Image img) {
			this.LoadTexture(img);
		}
		public Texture (Stream stream) {
			this.LoadTexture(Image.FromStream(stream));
		}
		public Texture (string filename) {
			this.Path = filename.Substring(0,filename.LastIndexOf('\\')+1);
			this.LoadTexture(new Bitmap(filename));
		}
		
		public void Resize () {
			double log2inv = 1.0d/Math.Log(2.0d);
			int w = (int) Math.Pow(2.0d,this.BitWidth=(int) (Math.Log(this.Width)*log2inv));
			int h = (int) Math.Pow(2.0d,this.BitHeight=(int) (Math.Log(this.Height)*log2inv));
			this.Resize(w,h);
		}
		public void Resize (int w, int h) {
			this.setSize(w,h);
		}
		public Texture Put (Texture newData) {
			Array.Copy(newData.Pixel,0,this.Pixel,0,this.Width*this.Height);
			return this;
		}
		public Texture Mix (Texture newData) {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.Mix(this.Pixel[i],newData.Pixel[i]);
			return this;
		}
		public Texture Add (Texture additive) {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.Add(this.Pixel[i],additive.Pixel[i]);
			return this;
		}
		public Texture Sub (Texture subtractive) {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.Sub(this.Pixel[i],subtractive.Pixel[i]);
			return this;
		}
		public Texture Inv () {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.Inv(this.Pixel[i]);
			return this;
		}
		public Texture Multiply (Texture multiplicative) {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.Multiply(this.Pixel[i],multiplicative.Pixel[i]);
			return this;
		}
		public void Cls () {
			for(int i = 0; i < this.Pixel.Length; i++)
				this.Pixel[i] = 0;
		}
		public Texture ToAverage () {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.GetAverage(this.Pixel[i]);
			return this;
		}
		public Texture ToGray () {
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = Colors.GetGray(this.Pixel[i]);
			return this;
		}
		public Texture ValToGray () {
			uint intensity;
			for(int i = this.Width*this.Height-1; i >= 0; i--) {
				intensity = Maths.Crop(this.Pixel[i],0,255);
				this.Pixel[i] = Colors.GetColor(intensity,intensity,intensity);
			}
			return this;
		}
		public Texture Colorize (uint[] pal) {
			int range = pal.Length-1;
			for(int i = this.Width*this.Height-1; i >= 0; i--)
				this.Pixel[i] = pal[Maths.Crop(this.Pixel[i],0,range)];
			return this;
		}
		public static Texture BlendTopDown (Texture top, Texture down) {
			down.Resize(top.Width,top.Height);
			Texture t = new Texture(top.Width,top.Height);
			int pos = 0;
			int alpha;
			for(int y = 0; y < top.Height; y++) {
				alpha = 255*y/(top.Height-1);
				for(int x = 0; x < top.Width; x++) {
					t.Pixel[pos] = Colors.Transparency(down.Pixel[pos],top.Pixel[pos],alpha);
					pos++;
				}
			}
			return t;
		}
		public unsafe void LoadTexture (Image img) {
			int h = img.Height;
			int w = img.Width;
			this.Width = w;
			this.Height = h;
			Bitmap bmp = new Bitmap(img);
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,this.Width,this.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
			int n = w*h;
			this.Pixel = new uint[n];
			uint* scan = (uint*) bmd.Scan0.ToPointer();
			for(int i = 0x00; i < Pixel.Length; i++) {
				Pixel[i] = *scan;
				scan++;
			}
			bmp.UnlockBits(bmd);
		}
		private void setSize (int w, int h) {
			int offset = w*h;
			int offset2;
			if(w*h != 0) {
				uint[] newpixels = new uint[w*h];
				for(int j = h-1; j >= 0; j--) {
					offset -= w;
					offset2 = (j*this.Height/h)*this.Width;
					for(int i = w-1; i >= 0; i--)
						newpixels[i+offset] = this.Pixel[(i*this.Width/w)+offset2];
				}
				this.Width = w;
				this.Height = h;
				this.Pixel = newpixels;
			}
		}
		private bool inrange (int a, int b, int c) {
			return (a>=b) & (a<c);
		}
		public Texture GetClone () {
			Texture t = new Texture(this.Width,this.Height);
			for(int i = 0; i < this.Pixel.Length; i++)
				t.Pixel[i] = this.Pixel[i];
			return t;
		}
		public void QuadMirror () {
			uint[] newPixels = new uint[Pixel.Length<<0x02];
			int s = 0x00;
			int m1 = 0x00;
			int m2 = (Width<<0x01)-0x01;
			int m3 = (Width*(Height-0x01))<<0x02;
			int m4 = ((Width*Height)<<0x02)-0x01;
			for(int y = 0x00; y < Height; y++) {
				for(int x = 0x00; x < Width; x++) {
					newPixels[m1++] = newPixels[m2--] = newPixels[m3++] = newPixels[m4--] = Pixel[s++];
				}
				m1 += Width;
				m2 += 0x03*Width;
				m3 -= 0x03*Width;
				m4 -= Width;
			}
			this.Pixel = newPixels;
			this.Width <<= 0x01;
			this.Height <<= 0x01;
		}
		public unsafe Bitmap ToBitmap () {
			Bitmap bmp = new Bitmap(this.Width,this.Height);
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,this.Width,this.Height),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			uint* scan = (uint*) bmd.Scan0.ToPointer();
			for(int i = 0x00; i < Pixel.Length; i++) {
				*scan = Pixel[i];
				scan++;
			}
			bmp.UnlockBits(bmd);
			return bmp;
		}
		public int GenerateOpenGLBuffer () {
			int texture = OGL.GL.GenTexture();
			OGL.GL.BindTexture(OGL.TextureTarget.Texture2D,texture);
			OGL.GL.TexParameter(OGL.TextureTarget.Texture2D, OGL.TextureParameterName.TextureMagFilter, (int)OGL.All.Linear);
			OGL.GL.TexParameter(OGL.TextureTarget.Texture2D, OGL.TextureParameterName.TextureMinFilter, (int)OGL.All.Linear);
			OGL.GL.TexImage2D(OGL.TextureTarget.Texture2D,0x00,OGL.PixelInternalFormat.Rgba,Width,Height,0x00,OGL.PixelFormat.Bgra,OGL.PixelType.UnsignedByte,this.Pixel);
			return texture;
		}
		
	}
}


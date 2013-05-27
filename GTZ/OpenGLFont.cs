using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using OpenTK.Graphics;
using GTZ.Utils;

namespace GTZ.Rendering {
	
	public class OpenGLFont {
		
		private static int fontOffset = -0x01;
		private static float[] glyphWidth;
		
		public static float FontHeight {
			get {
				return 32.0f;
			}
		}
		
		public static void LoadFont () {
			if(fontOffset != -0x01)
				return;
			BinaryReader br = new BinaryReader(File.Open("font.dat",FileMode.Open,FileAccess.Read));
			fontOffset = GL.GenLists(0x0100);
			GL.ShadeModel(ShadingModel.Flat);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment,0x01);//1 B/ 8 pixels
			glyphWidth = new float[0x0100];
			byte[] cache;
			int max;
			int min;
			for(int i = 0x00; i < 0x0100; i++) {
				cache = br.ReadBytes(128);
				if(i != 0x20) {
					max = 0x00;
					min  = 0x20;
					uint line = 0x00;
					for(int j = 0x00; j < 0x80;) {
						for(int k = j+0x04; j < k;) {
							line = (line<<0x08)|cache[j++];
						}
						max = Math.Max(max,0x20-Maths.NumberOfTailingZeros(line));
						min = Math.Min(min,Maths.NumberOfHeadingZeros(line));
					}
				}
				else {
					max = 0x1c;
					min = 0x00;
				}
				max -= min-0x04;
				GL.NewList(fontOffset+i,ListMode.Compile);
				GL.Bitmap(0x20,0x20,min,32.0f,max,0.0f,cache);
				GL.EndList();
				glyphWidth[i] = max;
			}
			
			br.Close();
		}
		
		public static unsafe void PrintString (string text) {
			GL.PushAttrib(AttribMask.ListBit);
			GL.ListBase(fontOffset);
			fixed(char* t = text) {
				GL.CallLists(text.Length,ListNameType.UnsignedShort,(IntPtr) t);
			}
			GL.PopAttrib();
		}
		public static void splitWidth (string text, float width, Queue<string> split) {
			float cw = 0.0f, cl = 0.0f;
			string line = string.Empty;
			string word = string.Empty;
			foreach(char c in text) {
				if(c == ' ') {
					if(cl+cw > width) {
						split.Enqueue(line);
						cl = 0.0f;
						line = word.Substring(0x01);
						cw = cl;
						word = string.Empty;
					}
					else {
						cl += cw;
						line += word;
						word = string.Empty;
						cw = 0.0f;
					}
				}
				word += c;
				cw += glyphWidth[(int) c];
			}
			split.Enqueue(line+word);
		}
		public static unsafe void collectFontData () {
			Bitmap bmp = new Bitmap("/home/willem/Projects/CursusAIDemos/CursusAIDemos/resources/font1.png");
			BitmapData bmd = bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int* ptr = (int*) (void*) bmd.Scan0, coff, loff, toff;
			BinaryWriter bw = new BinaryWriter(File.Open("font.dat",FileMode.OpenOrCreate,FileAccess.Write));
			int fco = 0x00, gray, pixel;
			byte[] fontdata = new byte[128];
			byte[] cropCache = new byte[0x08];
			for(int y = 0x00; y < 0x10; y++) {
				for(int x = 0x00; x < 0x10; x++) {
					coff = ptr+0x20*bmd.Stride*y/0x04+0x20*x;
					fco = 0x00;
					for(int l = 0x00; l < 0x20; l++) {
						loff = coff+bmd.Stride*(0x20-l)/0x04;
						toff = loff;
						for(int r = 0x00; r < 0x04; r++) {
							for(int o = 0x00; o < 0x08; o++) {
								pixel = *toff;
								gray = (pixel&0x0000ff)+((pixel&0x00ff00)>>0x08)+((pixel&0xff0000)>>0x10);
								if(gray > 0x180) {
									cropCache[o] = 0x01;
								}
								else {
									cropCache[o] = 0x00;
								}
								toff++;
							}
							fontdata[fco++] = (byte)	((cropCache[0x00]<<0x07)|(cropCache[0x01]<<0x06)|(cropCache[0x02]<<0x05)|(cropCache[0x03]<<0x04)|
														 (cropCache[0x04]<<0x03)|(cropCache[0x05]<<0x02)|(cropCache[0x06]<<0x01)|cropCache[0x07]);
						}
					}
					bw.Write(fontdata);
				}
			}
			bw.Close();
		}
		
	}
}

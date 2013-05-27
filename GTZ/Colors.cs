using System;

namespace GTZ.Utils {
	
	public static class Colors {
			
			private static uint pixel, color, overflow, scale, r, g, b;
			public static uint Alpha = 0xff000000;
			public static uint Red = 0xff0000;
			public static uint Green = 0xff00;
			public static uint Blue = 0xff;
			public static uint Mask7Bit = 0xfefeff;
			public static uint Mask6Bit = 0xfcfcfc;
			public static uint Rgb = 0xffffff;
			
			public static uint GetRed (uint c) {
				return (c & Colors.Red)>>16;
			}
			public static uint GetGreen (uint c) {
				return (c & Colors.Green)>>8;
			}
			public static uint GetBlue (uint c) {
				return c & Colors.Blue;
			}
			public static uint GetColor (int r, int g, int b) {
				return (uint) (Colors.Alpha|(r<<16)|(g<<8)|b);
			}
			public static uint GetColor (uint r, uint g, uint b) {
				return (uint) (Colors.Alpha|(r<<16)|(g<<8)|b);
			}
			public static uint GetGray (uint color) {
				uint r = ((color & Colors.Red)>>16);
				uint g = ((color & Colors.Green)>>8);
				uint b = (color & Colors.Blue);
				uint Y = (r*3+g*6+b)/10;
				return Colors.Alpha|(Y<<16)|(Y<<8)|Y;
			}
			public static uint GetAverage (uint color) {
				return (((color & Colors.Red)>>16)+((color & Colors.Green)>>8)+(color & Colors.Blue))/3;
			}
			public static uint GetCropColor (uint r, uint g, uint b) {
				return (uint) (Colors.Alpha|(Maths.Crop(r,0,255)<<16)|(Maths.Crop(g,0,255)<<8)|Maths.Crop(b,0,255));
			}
			public static uint GetCropColor (int r, int g, int b) {
				return (uint) (Colors.Alpha|(Maths.Crop(r,0,255)<<16)|(Maths.Crop(g,0,255)<<8)|Maths.Crop(b,0,255));
			}
			public static uint Add (uint color1, uint color2) {
				Colors.pixel=(color1 & Colors.Mask7Bit)+(color2 & Colors.Mask7Bit);
				Colors.overflow = Colors.pixel&0x1010100;
				Colors.overflow = Colors.overflow-(Colors.overflow>>8);
				return Colors.Alpha|Colors.overflow|Colors.pixel;
			}
			public static uint Sub (uint color1, uint color2) {
				Colors.pixel=(color1 & Colors.Mask7Bit)+(~color2 & Colors.Mask7Bit);
				Colors.overflow = ~Colors.pixel&0x1010100;
				Colors.overflow = Colors.overflow-(Colors.overflow>>8);
				return Colors.Alpha|(~Colors.overflow & Colors.pixel);
			}
			public static uint Subneg (uint color1, uint color2) {
				Colors.pixel=(color1 & Colors.Mask7Bit)+(color2 & Colors.Mask7Bit);
				Colors.overflow = ~Colors.pixel&0x1010100;
				Colors.overflow = Colors.overflow-(Colors.overflow>>8);
				return Colors.Alpha|(~Colors.overflow & Colors.pixel);
			}
			public static uint Inv (uint color) {
				return Colors.Alpha|(~color);
			}
			public static uint Mix (uint color1, uint color2) {
				return Colors.Alpha|(((color1 & Colors.Mask7Bit)>>1)+((color2 & Colors.Mask7Bit)>>1));
			}
			public static uint Scale (uint color, int factor) {
				if(factor == 0)
					return 0;
				if(factor == 255)
					return color;
				if(factor == 127)
					return (color&0xfefefe)>>1;
				Colors.r = (uint) ((((color>>16)&255)*factor)>>8);
				Colors.g = (uint) ((((color>>8)&255)*factor)>>8);
				Colors.b = (uint) (((color&255)*factor)>>8);
				return Colors.Alpha|(Colors.r<<16)|(Colors.g<<8)|Colors.b;
			}
			public static uint Multiply (uint color1, uint color2) {
				if((color1 & Colors.Rgb) == 0)
					return 0;
				if((color2 & Colors.Rgb) == 0)
					return 0;
				Colors.r = (((color1>>16)&255)*((color2>>16)&255))>>8;
				Colors.g = (((color1>>8)&255)*((color2>>8)&255))>>8;
				Colors.b = ((color1&255)*(color2&255))>>8;
				return Colors.Alpha|(Colors.r<<16)|(Colors.g<<8)|Colors.b;
			}
			public static uint Transparency (uint bkgrd, uint color, int alpha) {
				if(alpha == 0)
					return color;
				if(alpha == 255)
					return bkgrd;
				if(alpha == 127)
					return Colors.Mix(bkgrd,color);
				Colors.r = (uint) ((alpha*(((bkgrd>>16)&255)-((color>>16)&255))>>8)+((color>>16)&255));
				Colors.g = (uint) ((alpha*(((bkgrd>>8)&255)-((color>>8)&255))>>8)+((color>>8)&255));
				Colors.b = (uint) ((alpha*((bkgrd&255)-(color&255))>>8)+(color&255));
				return Colors.Alpha|(Colors.r<<16)|(Colors.g<<8)|Colors.b;
			}
			public static uint Random (uint color, uint delta) {
				uint r = (color>>16)&255;
				uint g = (color>>8)&255;
				uint b = color&255;
				r = (uint) (r+(int) (Maths.Random()*(float) delta));
				g = (uint) (g+(int) (Maths.Random()*(float) delta));
				b = (uint) (b+(int) (Maths.Random()*(float) delta));
				return Colors.GetCropColor(r,g,b);
			}
			public static uint Random () {
				return (uint) (Maths.UnsignedRandom()*16777216f);
			}
			public static uint[] MakeGradient (uint[] colors, int size) {
				uint[] pal = new uint[size];
				uint c1, c2;
				int pos1, pos2, range;
				uint r, g, b, r1, g1, b1, r2, g2, b2, dr, dg, db;
				if(colors.Length == 1) {
					c1 = colors[0];
					for(int i = 0; i < size; i++)
						pal[i] = c1;
					return pal;
				}
				for(int c = 0; c < colors.Length-1; c++) {
					c1 = colors[c];
					c2 = colors[c+1];
					pos1 = size*c/(colors.Length-1);
					pos2 = size*(c+1)/(colors.Length-1);
					range = pos2-pos1;
					r1 = Colors.GetRed(c1)<<16;
					g1 = Colors.GetGreen(c1)<<16;
					b1 = Colors.GetBlue(c1)<<16;
					r2 = Colors.GetRed(c2)<<16;
					g2 = Colors.GetGreen(c2)<<16;
					b2 = Colors.GetBlue(c2)<<16;
					dr = (uint) ((r2-r1)/range);
					dg = (uint) ((g2-g1)/range);
					db = (uint) ((b2-b1)/range);
					r = r1;
					g = g1;
					b = b1;
					for(int i = pos1; i < pos2; i++) {
						pal[i] = Colors.GetColor(r>>16,g>>16,b>>16);
						r += dr;
						g += dg;
						b += db;
					}
				}
				return pal;
			}
			
		}
	
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Zertz.Utils {
	
	public static class GraphicsUtils {

		private static readonly double invPhi = 0.5d*(Math.Sqrt(5.0d)-1.0d);
		private static readonly double minusInvPhi = 0.5d*(3.0d-Math.Sqrt(5.0d));
		
		public static unsafe void DrawSepia (Bitmap bmp, int x1, int y1, int w, int h, int x2, int y2) {
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,bmp.Width,bmp.Height),ImageLockMode.ReadWrite,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int stride = bmd.Stride>>0x02;
			uint* p1l = (uint*) bmd.Scan0;//line
			uint* p2l = p1l+stride*y2+x2;
			p1l += stride*y1+x1;
			uint* p1 = p1l;
			uint* p2 = p2l;
			uint c = 0x00;
			uint cr, cg, cb, cr2, cg2, cb2;
			for(int y = 0x00; y < h; y++) {
				for(int x = 0x00; x < w; x++) {
					c = *(p1++);
					cr = (c>>0x010)&0xff;
					cg = (c>>0x08)&0xff;
					cb = (c&0xff);
					/*
					0.393f*cr+0.769f*cg+0.189f*cb
					0.349f*cr+0.686f*cg+0.168f*cb
					0.272f*cr+0.534f*cg+0.131f*cb
					//
					101		197		48	|	0x65	0xc5	0x30
					89		176		43	|	0x59	0xb0	0x2b
					70		137		34	|	0x46	0x89	0x22
					*/
					cr2 = Math.Max(Math.Min(	0x65*cr+0xc5*cg+0x30*cb,0xff00),0x00);
					cg2 = Math.Max(Math.Min(	0x59*cr+0xb0*cg+0x2b*cb,0xff00),0x00);
					cb2 = Math.Max(Math.Min((	0x46*cr+0x89*cg+0x22*cb),0xff00),0x00);
					c = (uint) (c&0xff000000|(cr2<<0x08)&0xff0000|cg2&0xff00|(cb2>>0x08));
					*(p2++) = c;
				}
				p1l += stride;
				p1 = p1l;
				p2l += stride;
				p2 = p2l;
			}
			//load to GPU
			bmp.UnlockBits(bmd);
		}
		public static void DrawGlass (Graphics g, GraphicsPath gp) {
			Region oldReg = g.Clip;
			Region reg = new Region(gp);
			g.Clip = reg;
			//draw glass
			RectangleF bounds = gp.GetBounds();
			float half = bounds.Top+0.5f*bounds.Height;
			bounds.Height = (float) (2.0d*minusInvPhi*bounds.Width);
			bounds.Y = half-bounds.Height;
			LinearGradientBrush lgbGlass = new LinearGradientBrush(new PointF(0.0f,bounds.Top),new PointF(0.0f,bounds.Bottom),Color.FromArgb(0x50,0xff,0xff,0xff),Color.FromArgb(0x80,0xff,0xff,0xff));
			g.FillEllipse(lgbGlass,bounds);
			g.Clip = oldReg;
		}
		public static GraphicsPath RoundedRectangle (float x, float y, float w, float h, float r) {
			GraphicsPath gp = new GraphicsPath();
			float r2 = 2.0f*r; 
			gp.AddArc(x,y,r2,r2,180.0f,90.0f);
			gp.AddLine(x+r,y,x+w-r,y);
			gp.AddArc(x+w-r2,y,r2,r2,270.0f,90.0f);
			gp.AddLine(x+w,y+r,x+w,y+h-r);
			gp.AddArc(x+w-r2,y+h-r2,r2,r2,0.0f,90.0f);
			gp.AddLine(x+w-r,y+h,x+r,y+h);
			gp.AddArc(x,y+h-r2,r2,r2,90.0f,90.0f);
			gp.AddLine(x,y+h-r,x,y+r);
			gp.CloseFigure();
			return gp;
		}
		
	}
	
}
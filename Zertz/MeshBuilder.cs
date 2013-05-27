using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Zertz.Rendering {
	
	public static class MeshBuilder {//general structure: X3,N3,T2
		
		public const float PopupDepth = 0.001f;
		
		public static int BuildPopupPolygon (PointF[] polygon) {
			float d = PopupDepth;
			int n = polygon.Length;
			int i = 0x00;
			float[] xnt = new float[0x10*n];
			float px, py;
			for(int k = 0x00; k < n; k++) {
				px = polygon[k].X;
				py = polygon[k].Y;
				
				xnt[i++] = px-0.5f;
				xnt[i++] = 1.0f-py;
				xnt[i++] = d;//X'0
				xnt[i++] = 0.0f;
				xnt[i++] = 0.0f;
				xnt[i++] = 1.0f;//N
				xnt[i++] = px;
				xnt[i++] = py;//T
				
			}
			
			for(int k = n-0x01; k >= 0x00; k--) {
				px = polygon[k].X;
				py = polygon[k].Y;
				
				xnt[i++] = px-0.5f;
				xnt[i++] = 1.0f-py;
				xnt[i++] = -d;//X'0
				xnt[i++] = 0.0f;
				xnt[i++] = 0.0f;
				xnt[i++] = -1.0f;//N
				xnt[i++] = px;
				xnt[i++] = py;//T
			}
				
			/*//reverse
				xnt[j--] = 1.0f-py;	xnt[j--] = px;//T'1
				xnt[j--] = -1.0f;	xnt[j--] = 0.0f;	xnt[j--] = 0.0f;//N
				xnt[j--] = -d;		xnt[j--] = 1.0f-py;	xnt[j--] = px-0.5f;//X*/
			//}
			
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;
		}
		public static int BuildPopupRectangle () {
			float d = PopupDepth;
			float[] xnt = new float[0x40];
			int i = 0x00;
			xnt[i++] = -0.5f;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X'0
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//T
			xnt[i++] = -0.5f;
			xnt[i++] = 1.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			xnt[i++] = 0.5f;
			xnt[i++] = 1.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//T
			xnt[i++] = 0.5f;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = 1.0f;
			
			xnt[i++] = 0.5f;
			xnt[i++] = 0.0f;
			xnt[i++] = d;//X'1
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = 1.0f;//T
			xnt[i++] = 0.5f;
			xnt[i++] = 1.0f;
			xnt[i++] = d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//T
			xnt[i++] = -0.5f;
			xnt[i++] = 1.0f;
			xnt[i++] = d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			xnt[i++] = -0.5f;
			xnt[i++] = 0.0f;
			xnt[i++] = d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;
		}
		public static int BuildCuboid (float w, float h, float d) {
			float ty0 = h/(2.0f*h+d);
			float ty1 = 1.0f-ty0;
			float tx0 = 1.0f/(2.0f*h+2.0f*w);
			float tx1 = (w+h)*tx0;
			tx0 *= w;
			float tx2 = tx0+tx1;
			float[] xnt = new float[0xc0];
			int i = 0x00;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X'0
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx0;
			xnt[i++] = ty1;//T
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx0;
			xnt[i++] = ty0;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = ty0;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = ty1;
			
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = -d;//X'1
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty1;//T
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty0;//T
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty0;//T
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty1;
			
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = -d;//X'2
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty1;//T
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = 1.0f;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = 1.0f;//T
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = -d;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty1;
			
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X'3
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty0;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = 0.0f;//T
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = 0.0f;//T
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty0;
			
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X'3
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty0;//T
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx0;
			xnt[i++] = ty0;//T
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx0;
			xnt[i++] = ty1;//T
			xnt[i++] = 0.0f;
			xnt[i++] = -h;
			xnt[i++] = -d;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx1;
			xnt[i++] = ty1;
			
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = -d;//X'3
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty1;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = -d;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = ty1;//T
			xnt[i++] = w;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 1.0f;
			xnt[i++] = ty0;//T
			xnt[i++] = w;
			xnt[i++] = -h;
			xnt[i++] = 0.0f;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = tx2;
			xnt[i++] = ty0;
			
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;			
		}
		public static int BuildZertzCup (float w, float t, float h, float h1, float hp, float ballRadius, out float ballHeight, out int bufferN) {
			int n = 0x04*0x09;//int n = 0x04*(0x09+segments);
			bufferN = 0x04*n;
			float[] xnt = new float[0x08*bufferN];
			int i = 0x00;
			float he = h1-h;
			float nrx = (float)(1.0d/Math.Sqrt(hp*hp+t*t));
			float nry = t*nrx;
			nrx *= hp;
			ballHeight = (nrx/nry+nry/nrx)*nrx*ballRadius+h1;
			float hp2 = 0.5f*hp+h1;
			hp += h1;
			//float ht = hp+tileHeight;
			//float rt = tileRadius;
			
			
			xnt[i++] = w;
			xnt[i++] = hp2;
			xnt[i++] = -w;//X'0
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = hp2;
			xnt[i++] = -w;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = he;
			xnt[i++] = -w;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = he;
			xnt[i++] = -w;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = -w-t;
			xnt[i++] = hp2;
			xnt[i++] = -w-t;//X'1
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = hp2;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = he;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w-t;
			xnt[i++] = he;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = -1.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = -w;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X'2
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = hp;
			xnt[i++] = -w;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = h1;
			xnt[i++] = -w;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = h1;
			xnt[i++] = -w-t;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = w;
			xnt[i++] = hp;
			xnt[i++] = -w;//X'3
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = h1;
			xnt[i++] = -w-t;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = h1;
			xnt[i++] = -w;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = w;
			xnt[i++] = hp;
			xnt[i++] = -w;//X'4
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = hp;
			xnt[i++] = -w;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = -w-t;
			xnt[i++] = h1;
			xnt[i++] = -w-t;//X'5
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w-t;
			xnt[i++] = h1;
			xnt[i++] = -w;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w-t;
			xnt[i++] = hp;
			xnt[i++] = -w;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w-t;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X
			xnt[i++] = -1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = w+t;
			xnt[i++] = h1;
			xnt[i++] = -w;//X'6
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = h1;
			xnt[i++] = -w-t;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = hp;
			xnt[i++] = -w-t;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w+t;
			xnt[i++] = hp;
			xnt[i++] = -w;//X
			xnt[i++] = 1.0f;
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = w;
			xnt[i++] = hp2;
			xnt[i++] = -w;//X'7
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = -nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = h1;
			xnt[i++] = -w-0.5f*t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = -nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = h1;
			xnt[i++] = -w-0.5f*t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = -nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = hp2;
			xnt[i++] = -w;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = -nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			
			xnt[i++] = -w;
			xnt[i++] = hp2;
			xnt[i++] = -w-t;//X'8
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = -w;
			xnt[i++] = h1;
			xnt[i++] = -w-0.5f*t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = h1;
			xnt[i++] = -w-0.5f*t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			xnt[i++] = w;
			xnt[i++] = hp2;
			xnt[i++] = -w-t;//X
			xnt[i++] = 0.0f;
			xnt[i++] = nry;
			xnt[i++] = nrx;//N
			xnt[i++] = 0.0f;
			xnt[i++] = 0.0f;//T
			
			/*double delta = 2.0d*Math.PI/(segments-0x01);
			float sina, cosa, sinb = 0.0f, cosb = 1.0f;
			for(int j = 0x01; j <= segments; j++) {
				sina = sinb;	cosa = cosb;
				sinb = (float) Math.Sin(j*delta);	cosb = (float) Math.Cos(j*delta);
				
				xnt[i++] = sina*rt-w-0.5f*t;	xnt[i++] = ht;		xnt[i++] = cosa*rt-w-0.5f*t;//X'0
				xnt[i++] = sina;	xnt[i++] = 0.0f;	xnt[i++] = cosa;//N
				xnt[i++] = 0.0f;	xnt[i++] = 0.0f;//T
				
				xnt[i++] = sina*rt-w-0.5f*t;	xnt[i++] = hp;		xnt[i++] = cosa*rt-w-0.5f*t;//X
				xnt[i++] = sina;	xnt[i++] = 0.0f;	xnt[i++] = cosa;//N
				xnt[i++] = 0.0f;	xnt[i++] = 0.0f;//T
				
				xnt[i++] = sinb*rt-w-0.5f*t;	xnt[i++] = hp;		xnt[i++] = cosb*rt-w-0.5f*t;//X
				xnt[i++] = sinb;	xnt[i++] = 0.0f;	xnt[i++] = cosb;//N
				xnt[i++] = 0.0f;	xnt[i++] = 0.0f;//T
				
				xnt[i++] = sinb*rt-w-0.5f*t;	xnt[i++] = ht;		xnt[i++] = cosb*rt-w-0.5f*t;//X
				xnt[i++] = sinb;	xnt[i++] = 0.0f;	xnt[i++] = cosb;//N
				xnt[i++] = 0.0f;	xnt[i++] = 0.0f;//T
			}*/
			
			float x, y, z, nx, ny, nz, tx, ty;
			for(int j = 0x00; i < xnt.Length;) {
				x = xnt[j++];
				y = xnt[j++];
				z = xnt[j++];
				nx = xnt[j++];
				ny = xnt[j++];
				nz = xnt[j++];
				tx = xnt[j++];
				ty = xnt[j++];
				xnt[i++] = z;
				xnt[i++] = y;
				xnt[i++] = -x;
				xnt[i++] = nz;
				xnt[i++] = ny;
				xnt[i++] = -nx;
				xnt[i++] = tx;
				xnt[i++] = ty+0.25f;
			}
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;
		}
		public static int GenerateCilinder (float height, float r, int segments, out int bufferN) {
			bufferN = (segments+0x01)<<0x01;
			float[] xnt = new float[bufferN<<0x03];
			
			double delta = 2.0d*Math.PI/segments;
			double alpha = 0.0d;
			float sina, cosa, sinar, cosar;
			float h_2 = 0.5f*height;
			float dt = 1.0f/(segments+0x01);
			float t = 0.0f;
			for(int j = 0x00, i = 0x00; j <= segments; j++) {
				sina = (float)Math.Sin(alpha);
				sinar = r*sina;
				cosa = (float)Math.Cos(alpha);
				cosar = r*cosa;
				
				xnt[i++] = sinar;
				xnt[i++] = h_2;
				xnt[i++] = cosar;//X
				xnt[i++] = sina;
				xnt[i++] = 0.0f;
				xnt[i++] = cosa;//N
				xnt[i++] = t;
				xnt[i++] = 1.0f;//T
				
				xnt[i++] = sinar;
				xnt[i++] = -h_2;
				xnt[i++] = cosar;//X
				xnt[i++] = sina;
				xnt[i++] = 0.0f;
				xnt[i++] = cosa;//N
				xnt[i++] = t;
				xnt[i++] = 0.0f;//T
				
				alpha += delta;
				t += dt;
			}
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;
		}
		public static int GenerateHeightMap (Texture texture, float dxz, float dy, int granularity, out int idBuffer, out int idN) {
			int w = texture.Width;
			int h = texture.Height;
			int lnx = (w/granularity);
			int lnz = (h/granularity);
			idN = (lnx*(lnz-0x01))<<0x01;
			int[] idData = new int[idN];
			dxz *= granularity;
			int k = 0x00;
			for(int i = (lnz-0x01)*lnx-0x01, j = i+lnx; i >= 0x00;) {
				idData[k++] = j--;
				idData[k++] = i--;
			}
			GL.GenBuffers(0x01, out idBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, idBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int)*idData.Length), idData, BufferUsageHint.StaticDraw);
			float[] xntData = new float[(lnx*lnz)<<0x03];
			float vy, ty, dyx, dyz, ntx = 1.0f/(lnx-0x01), nty = 1.0f/(lnz-0x01), nn, dxzsq = 4.0f*dxz*dxz;
			uint[] pixel = texture.Pixel;
			float factor = dy/255.0f;
			k = 0x00;
			int l = 0x00;
			float x0 = -0.5f*dxz*(lnx-0x01), z0 = -0.5f*dxz*(lnz-0x01);
			for(int i = 0x00; i < lnz; i++) {
				vy = dxz*i+z0;
				ty = nty*i;
				l = w*i*granularity;
				for(int j = 0x00; j < lnx; j++) {
					xntData[k++] = dxz*j+x0;
					xntData[k++] = (pixel[l]&0xff)*factor;
					xntData[k++] = vy;
					if(i > 0x00 && j > 0x00 && i < (lnz-0x01) && j < (lnx-0x01)) {
						//fill
						dyx = (pixel[l-0x01]&0xff)-(pixel[l+0x01]&0xff)*factor;
						dyz = (pixel[l-w]&0xff)-(pixel[l+w]&0xff)*factor;
						nn = (float)(1.0d/Math.Sqrt(dxzsq+dyx*dyx+dyz*dyz));
						xntData[k++] = dyx*nn;
						xntData[k++] = 2.0f*dxz*nn;
						xntData[k++] = dyz*nn;
					}
					else {
						//border
						xntData[k++] = 0.0f;
						xntData[k++] = 1.0f;
						xntData[k++] = 0.0f;
					}
					xntData[k++] = ntx*j;
					xntData[k++] = ty;
					l += granularity;
				}
			}
			int xntBuff;
			GL.GenBuffers(0x01, out xntBuff);
			GL.BindBuffer(BufferTarget.ArrayBuffer, xntBuff);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float)*xntData.Length), xntData, BufferUsageHint.StaticDraw);
			return xntBuff;
		}
		public static int BuildRing (float height, float r, float R, int segments, out int bufferN) {
			float segi = 1.0f/segments;
			double gamma = 2.0d*Math.PI/segments;
			/*idN = segments<<0x04;
			int[] id = new int[idN];
			int j = 0x00;
			int k = 0x00;
			for(int i = 0x00; i < segments; i++) {
				for(int l = 0x00; l < 0x04; l++) {
					id[j++] = k++;
					id[j++] = k++;
					id[j++] = k+0x07;
					id[j++] = k+0x06;	
				}
			}
			GL.GenBuffers(0x01,out idBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,idBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer,new IntPtr(id.Length*sizeof(int)),id,BufferUsageHint.StaticDraw);*/
			bufferN = (segments+0x01)<<0x03;
			float[] xnt = new float[bufferN<<0x03];//VERTEX3-NORMAL3-TEXTURE2
			float h2 = 0.5f*height;
			float sin, cos;
			double alpha;
			float ty;
			float tx1 = 0.5f*(R-r)/(height+R-r);
			int j0 = 0x00, j1 = (segments+0x01)<<0x04, j2 = j1<<0x01, j3 = j1+j2;
			for(int i = 0x00; i <= segments; i++) {
				ty = segi*i;
				alpha = gamma*i;
				sin = (float)Math.Sin(alpha);
				cos = (float)Math.Cos(alpha);
				
				//r Top
				xnt[j0++] = r*cos;
				xnt[j0++] = -h2;
				xnt[j0++] = r*sin;//X
				xnt[j0++] = 0.0f;
				xnt[j0++] = -1.0f;
				xnt[j0++] = 0.0f;//N
				xnt[j0++] = 0.0f;
				xnt[j0++] = ty;//T
				
				//R Top
				xnt[j0++] = R*cos;
				xnt[j0++] = -h2;
				xnt[j0++] = R*sin;//X
				xnt[j0++] = 0.0f;
				xnt[j0++] = -1.0f;
				xnt[j0++] = 0.0f;//N
				xnt[j0++] = tx1;
				xnt[j0++] = ty;//T
				
				//T Rig
				xnt[j1++] = R*cos;
				xnt[j1++] = -h2;
				xnt[j1++] = R*sin;//X
				xnt[j1++] = cos;
				xnt[j1++] = 0.0f;
				xnt[j1++] = sin;//N
				xnt[j1++] = tx1;
				xnt[j1++] = ty;//T
				
				//B Rig
				xnt[j1++] = R*cos;
				xnt[j1++] = h2;
				xnt[j1++] = R*sin;//X
				xnt[j1++] = cos;
				xnt[j1++] = 0.0f;
				xnt[j1++] = sin;//N
				xnt[j1++] = 0.5f;
				xnt[j1++] = ty;//T
				
				//R Bot
				xnt[j2++] = R*cos;
				xnt[j2++] = h2;
				xnt[j2++] = R*sin;//X
				xnt[j2++] = 0.0f;
				xnt[j2++] = 1.0f;
				xnt[j2++] = 0.0f;//N
				xnt[j2++] = 0.5f;
				xnt[j2++] = ty;//T
				
				//r Bot
				xnt[j2++] = r*cos;
				xnt[j2++] = h2;
				xnt[j2++] = r*sin;//X
				xnt[j2++] = 0.0f;
				xnt[j2++] = 1.0f;
				xnt[j2++] = 0.0f;//N
				xnt[j2++] = 0.5f+tx1;
				xnt[j2++] = ty;//T
				
				//B Lef
				xnt[j3++] = r*cos;
				xnt[j3++] = h2;
				xnt[j3++] = r*sin;//X
				xnt[j3++] = -cos;
				xnt[j3++] = 0.0f;
				xnt[j3++] = -sin;//N
				xnt[j3++] = 0.5f+tx1;
				xnt[j3++] = ty;//T
				//T Lef
				xnt[j3++] = r*cos;
				xnt[j3++] = -h2;
				xnt[j3++] = r*sin;//X
				xnt[j3++] = -cos;
				xnt[j3++] = 0.0f;
				xnt[j3++] = -sin;//N
				xnt[j3++] = 1.0f;
				xnt[j3++] = ty;//T
			}
			/*for(int i = 0x00; j0 < xnt.Length;) {
				for(k = 0x00; k < 0x07; k++) {
					xnt[j0++] = xnt[i++];
				}
				xnt[j0++] = 1.0f;
				i++;
			}*/
			int dataBuffer;
			GL.GenBuffers(0x01, out dataBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(xnt.Length*sizeof(float)), xnt, BufferUsageHint.StaticDraw);
			return dataBuffer;
		}
		public static unsafe int BuildSphere (int depth, float radius, out int sphereN) {
			int delta = 0x018*(int)Math.Pow(0x04, depth);
			int n = 0x14*delta;
			float[] data = new float[n];
			sphereN = n>>0x03;
			fixed(float* pointer = data) {
				double x = Math.Sqrt(0.5d);
				double z = x;
				subdivide(pointer, x, 0.0d, z, 0.0d, z, x, -x, 0.0d, z, depth, radius);//
				subdivide(pointer+delta, 0.0d, z, x, -z, x, 0.0d, -x, 0.0d, z, depth, radius);//
				subdivide(pointer+0x02*delta, 0.0d, z, x, 0.0, z, -x, -z, x, 0.0d, depth, radius);//
				subdivide(pointer+0x03*delta, z, x, 0.0d, 0.0, z, -x, 0.0d, z, x, depth, radius);//
				subdivide(pointer+0x04*delta, x, 0.0d, z, z, x, 0.0d, 0.0d, z, x, depth, radius);//
				subdivide(pointer+0x05*delta, x, 0.0d, z, z, -x, 0.0, z, x, 0.0d, depth, radius);//
				subdivide(pointer+0x06*delta, z, -x, 0.0, x, 0.0d, -z, z, x, 0.0d, depth, radius);//
				subdivide(pointer+0x07*delta, z, x, 0.0d, x, 0.0d, -z, 0.0d, z, -x, depth, radius);//
				subdivide(pointer+0x08*delta, x, 0.0d, -z, -x, 0.0d, -z, 0.0d, z, -x, depth, radius);//
				subdivide(pointer+0x09*delta, x, 0.0d, -z, 0.0d, -z, -x, -x, 0.0d, -z, depth, radius);//
				subdivide(pointer+0x0a*delta, x, 0.0d, -z, z, -x, 0.0d, 0.0d, -z, -x, depth, radius);//
				subdivide(pointer+0x0b*delta, z, -x, 0.0d, 0.0d, -z, x, 0.0d, -z, -x, depth, radius);//
				subdivide(pointer+0x0c*delta, 0.0d, -z, x, -z, -x, 0.0d, 0.0d, -z, -x, depth, radius);//
				subdivide(pointer+0x0d*delta, 0.0d, -z, x, -x, 0.0d, z, -z, -x, 0.0d, depth, radius);//
				subdivide(pointer+0x0e*delta, 0.0d, -z, x, x, 0.0d, z, -x, 0.0d, z, depth, radius);//
				subdivide(pointer+0x0f*delta, z, -x, 0.0d, x, 0.0d, z, 0.0d, -z, x, depth, radius);//
				subdivide(pointer+0x10*delta, -z, -x, 0.0d, -x, 0.0d, z, -z, x, 0.0d, depth, radius);//
				subdivide(pointer+0x11*delta, -x, 0.0d, -z, -z, -x, 0.0d, -z, x, 0.0d, depth, radius);//
				subdivide(pointer+0x12*delta, 0.0, z, -x, -x, 0.0d, -z, -z, x, 0.0d, depth, radius);
				subdivide(pointer+0x13*delta, -z, -x, 0.0d, -x, 0.0d, -z, 0.0, -z, -x, depth, radius);
				int number;
				GL.GenBuffers(0x01, out number);
				GL.BindBuffer(BufferTarget.ArrayBuffer, number);
				GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(n*sizeof(float)), data, BufferUsageHint.StaticDraw);
				return number;
			}
		}
		
		private static unsafe void subdivide (float* pointer, double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, int depth, float ra) {
			if(depth > 0x00) {
				depth--;
				double x12 = x1+x2;
				double r = x12*x12;
				double y12 = y1+y2;
				r += y12*y12;
				double z12 = z1+z2;
				r += z12*z12;
				r = 1.0d/Math.Sqrt(r);
				x12 *= r;
				y12 *= r;
				z12 *= r;
				
				double x23 = x2+x3;
				r = x23*x23;
				double y23 = y2+y3;
				r += y23*y23;
				double z23 = z2+z3;
				r += z23*z23;
				r = 1.0d/Math.Sqrt(r);
				x23 *= r;
				y23 *= r;
				z23 *= r;
				
				double x13 = x1+x3;
				r = x13*x13;
				double y13 = y1+y3;
				r += y13*y13;
				double z13 = z1+z3;
				r += z13*z13;
				r = 1.0d/Math.Sqrt(r);
				x13 *= r;
				y13 *= r;
				z13 *= r;
				
				int delta = 0x18*(int)Math.Pow(0x04, depth);
				subdivide(pointer, x1, y1, z1, x12, y12, z12, x13, y13, z13, depth, ra);
				subdivide(pointer+delta, x2, y2, z2, x23, y23, z23, x12, y12, z12, depth, ra);
				subdivide(pointer+0x02*delta, x3, y3, z3, x13, y13, z13, x23, y23, z23, depth, ra);
				subdivide(pointer+0x03*delta, x12, y12, z12, x23, y23, z23, x13, y13, z13, depth, ra);
			}
			else {
				float txo = (float)(0.5f*Math.Atan2(z1, x1)/Math.PI+0.5d), ty = (float)(Math.Asin(y1)/Math.PI+0.5d);
				*pointer = (float)(ra*x1);
				pointer++;
				*pointer = (float)(ra*y1);
				pointer++;
				*pointer = (float)(ra*z1);
				pointer++;	//X
				*pointer = (float)x1;
				pointer++;
				*pointer = (float)y1;
				pointer++;
				*pointer = (float)z1;
				pointer++;		//N
				*pointer = txo;
				pointer++;
				*pointer = ty;
				pointer++;															//T
				
				float tx = (float)(0.5f*Math.Atan2(z2, x2)/Math.PI+0.5d);
				ty = (float)(Math.Asin(y2)/Math.PI+0.5d);
				if(tx < 0.75f && txo > 0.75f) {
					tx += 1.0f;
				}
				else if(tx > 0.75f && txo < 0.75f) {
					tx -= 1.0f;
				}
				
				*pointer = (float)(ra*x2);
				pointer++;
				*pointer = (float)(ra*y2);
				pointer++;
				*pointer = (float)(ra*z2);
				pointer++;	//X
				*pointer = (float)x2;
				pointer++;
				*pointer = (float)y2;
				pointer++;
				*pointer = (float)z2;
				pointer++;		//N
				*pointer = tx;
				pointer++;
				*pointer = ty;
				pointer++;															//T
				
				tx = (float)(0.5f*Math.Atan2(z3, x3)/Math.PI+0.5d);
				ty = (float)(Math.Asin(y3)/Math.PI+0.5d);
				if(tx < 0.75f && txo > 0.75f) {
					tx += 1.0f;
				}
				else if(tx > 0.75f && txo < 0.75f) {
					tx -= 1.0f;
				}
				
				*pointer = (float)(ra*x3);
				pointer++;
				*pointer = (float)(ra*y3);
				pointer++;
				*pointer = (float)(ra*z3);
				pointer++;	//X
				*pointer = (float)x3;
				pointer++;
				*pointer = (float)y3;
				pointer++;
				*pointer = (float)z3;
				pointer++;		//N
				*pointer = tx;
				pointer++;
				*pointer = ty;
				pointer++;															//T
			}
		}
		
	}
	
}
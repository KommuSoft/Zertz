using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GTZ.Mathematics;

namespace GTZ.Rendering {
	
	public class WaterSimulator : ILoadable, IUnloadable, IRenderable, ITimeSensitive, IHeightGrid {
		
		private float[,] t0;
		private float[,] t1;
		private Vector3[] PosNor;
		private int m;
		private int n;
		private float dxy;
		private float dt;
		private float c;
		private int glRef = -0x01, glInd, glN;
		
		public WaterSimulator (int m, int n, float dxy, float dt, float c, float dz) {
			this.m = m;
			this.n = n;
			this.dxy = dxy;
			this.dt = dt;
			this.c = c;
			this.t0 = new float[m,n];
			this.t1 = new float[m,n];
			this.PosNor = new Vector3[0x02*m*n];
			this.generateRandomBase(dz);
		}
		private void generateRandomBase (float dz) {
			float f = 0.5f*dz*dxy;
			Random rand = new Random();
			int delta = m*n;
			int delta2 = n*(m-0x01);
			for(int i = n*(m-0x01); i >= 0x00; i -= n) {
				PosNor[delta+i] = Vector3.UnitY;//N
				PosNor[delta+i+n-0x01] = Vector3.UnitY;//N
			}
			for(int j = n-0x01; j >= 0x00; j--) {
				PosNor[delta+j] = Vector3.UnitY;//N
				PosNor[delta+delta2+j] = Vector3.UnitY;//N
			}
			for(int i = m-0x02; i > 0x00; i--) {
				for(int j = n-0x02; j > 0x00; j--) {
					t0[i,j] = t1[i,j] = 0.25f*(t1[i,j+0x01]+t1[i+0x01,j+0x01]+t1[i+0x01,j]+t1[i+0x01,j-0x01])+(float) (rand.NextDouble()*dxy*dz-f);
				}
			}
			int k = m*n-0x01;
			float x0 = -0.5f*dxy;
			float y0 = x0*(n-0x01);
			x0 *= (m-0x01);
			for(int i = m-0x01; i >= 0x00; i--) {
				for(int j = n-0x01; j >= 0x00; j--) {
					PosNor[k--] = new Vector3(i*this.dxy+x0,t1[i,j],j*this.dxy+y0);
				}
			}
			this.calculateNormals();
		}
		private void calculateNormals () {
			int pointerN = m*n+n+0x01;//first normal
			float dxysq = 4.0f*this.dxy*this.dxy, dzz, dzx, no;
			for(int i = 0x01; i < m-0x01; i++) {
				for(int j = 0x01; j < n-0x01; j++) {
					dzz = t0[i-0x01,j]-t0[i+0x01,j];
					dzx = t0[i,j-0x01]-t0[i,j+0x01];
					no = (float) (1.0d/Math.Sqrt(dxysq+dzx*dzx+dzz*dzz));
					PosNor[pointerN++] = new Vector3(no*dzz,2.0f*no*this.dxy,no*dzx);
				}
				pointerN += 0x02;
			}
		}
		public Vector3[] GetVectors (IShape2D shape) {
			RectangleF r = shape.Bounds;
			float x0 = -0.5f*dxy;
			float y0 = x0*(n-0x01);
			x0 *= (m-0x01);
			double dxyinv = 1.0d/this.dxy;
			int xi1 = Math.Max(0x00,Math.Min(n-0x01,(int) Math.Floor((r.X-x0)*dxyinv)));
			int xi2 = Math.Max(0x00,Math.Min(n-0x01,(int) Math.Ceiling((r.Right-x0)*dxyinv)));
			int yi1 = Math.Max(0x00,Math.Min(m-0x01,(int) Math.Floor((r.Y-y0)*dxyinv)));
			int yi2 = Math.Max(0x00,Math.Min(m-0x01,(int) Math.Ceiling((r.Bottom-y0)*dxyinv)));
			int dx = xi2-xi1;
			int dxa = n-dx-0x01;
			int k = yi1*n+xi1, K;
			Vector3 v;
			Stack<Vector3> vs = new Stack<Vector3>();
			for(int y = yi1; y <= yi2; y++) {
				for(K = k+dx; k <= K; k++) {
					v = PosNor[k];
					if(shape.Contains(v.X,v.Z)) {
						vs.Push(v);
					}
				}
				k += dxa;
			}
			return vs.ToArray();
		}
		public void OnLoad (EventArgs e) {
			if(this.glRef != -0x01)
				return;
			short[] indices = new short[(n*(m-0x01))<<0x01];
			int k = 0x00;
			short l1 = 0x00, l2 = (short) n;
			for(int i = (m-0x01)*n-0x01; i >= 0x00; i--) {
				indices[k++] = l2++;
				indices[k++] = l1++;
			}
			this.glN = indices.Length;
			GL.GenBuffers(0x01,out this.glRef);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.glRef);
			GL.BufferData(BufferTarget.ArrayBuffer,new IntPtr(this.PosNor.Length*Vector3.SizeInBytes),this.PosNor,BufferUsageHint.DynamicDraw);
			GL.GenBuffers(0x01,out this.glInd);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,this.glInd);
			GL.BufferData(BufferTarget.ElementArrayBuffer,new IntPtr(indices.Length*sizeof(short)),indices,BufferUsageHint.StaticDraw);
		}
		public void OnUnload (EventArgs e) {}
		public void AdvanceTime (float time) {
			float lambda = this.c*this.dt/this.dxy;
			lambda *= lambda;
			float gamma = (2.0f-4.0f*lambda);
			float[,] tt = t0;
			t0 = t1;
			t1 = tt;
			//fill in tt
			int vp = n*(m-0x01)-0x02;//vertexPointer
			for(int i = m-0x02; i > 0x00; i--) {
				for(int j = n-0x02; j > 0x00; j--) {
					t1[i,j] = this.PosNor[vp--].Y = lambda*(t0[i-0x01,j]+t0[i+0x01,j]+t0[i,j-0x01]+t0[i,j+0x01])+gamma*t0[i,j]-t1[i,j];
				}
				vp -= 0x02;
			}
			this.calculateNormals();
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.glRef);
			GL.BufferData(BufferTarget.ArrayBuffer,new IntPtr(this.PosNor.Length*Vector3.SizeInBytes),this.PosNor,BufferUsageHint.DynamicDraw);
		}
		public void Render (OpenTK.FrameEventArgs e) {
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.glRef);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,this.glInd);
			GL.VertexPointer(0x03,VertexPointerType.Float,Vector3.SizeInBytes,0x00);
			GL.NormalPointer(NormalPointerType.Float,Vector3.SizeInBytes,m*n*Vector3.SizeInBytes);
			GL.DrawElements(BeginMode.QuadStrip,this.glN,DrawElementsType.UnsignedShort,0x00);
			GL.PopClientAttrib();
		}
	}
}


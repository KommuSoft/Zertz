using System;
using GTZ.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GTZ.Rendering {
	
	public class PopupBookRopeItem : ITimeSensitive, ILoadable, IRenderable {
		
		private readonly float k;
		private readonly float g;
		private readonly Vector3[] xv;
		private readonly Vector3 objectSize;
		private readonly float ex;
		private readonly int n;
		private readonly float beta;
		private readonly float ropeLength;
		private uint glRef;
		
		public PopupBookRopeItem (float k, float beta, float g, float length, int n, Vector3 objectSize) {
			this.k = k;
			this.ropeLength = length;
			this.g = g;
			this.n = n;
			this.xv = new Vector3[n<<0x01];
			this.objectSize = objectSize;
			this.ex = 0.5f*length/((n-0x01)*(n-0x01));
			init();
		}
		
		private void init () {
			this.xv[0x00] = new Vector3(0.0f,0.0f,0.0f);
			int i = 0x01;
			for(; i < n;) {
				this.xv[i++] = new Vector3(0.5f*ropeLength*(float) UniversalRandom.NextSignedDouble(),0.5f*ropeLength*(float) UniversalRandom.NextSignedDouble(),0.5f*ropeLength*(float) UniversalRandom.NextSignedDouble());
			}
			for(; i < xv.Length;) {
				this.xv[i++] = new Vector3(0.0f,0.0f,0.0f);
			}
		}
		
		public void AdvanceTime (float time) {
			float bet = time-time*time*beta, gt = g*time, kt = this.k*time;
			for(int i = n+0x01; i < xv.Length;) {
				xv[i++].Y -= gt;
			}
			Vector3 dx;
			float dxl, dl;
			for(int i = 0x00, j = 0x01, k = n; j < n; i++, j++) {
				dx = xv[j]-xv[i];
				dxl = dx.Length;
				dl = dxl-ex;
				dl *= kt;
				dx = dx/dxl*dl;
				xv[k++] += dx;
				xv[k] -= dx;
			}
			for(int i = 0x01, j = n+0x01; i < n;) {
				xv[i++] += bet*xv[j++];
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer,glRef);
			GL.BufferData(BufferTarget.ArrayBuffer,new IntPtr(Vector3.SizeInBytes*this.n),this.xv,BufferUsageHint.DynamicDraw);
		}
		public void OnLoad (EventArgs e) {
			GL.GenBuffers(0x01,out glRef);
			GL.BindBuffer(BufferTarget.ArrayBuffer,glRef);
			GL.BufferData(BufferTarget.ArrayBuffer,new IntPtr(Vector3.SizeInBytes*this.n),this.xv,BufferUsageHint.DynamicDraw);
		}
		public void Render (FrameEventArgs e) {
			GL.Color3(1.0f,0.0f,0.0f);
			GL.PushAttrib(AttribMask.EnableBit);
			GL.Disable(EnableCap.Lighting);
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,glRef);
			GL.VertexPointer(0x03,VertexPointerType.Float,0x03*sizeof(float),0x00);
			GL.DrawArrays(BeginMode.LineStrip,0x00,n);
			GL.PopClientAttrib();
			GL.PopAttrib();
		}
		
	}
	
}
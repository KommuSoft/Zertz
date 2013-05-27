using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GTZ.Utils;

namespace GTZ.Rendering {
	
	public class LinuxFlag : LocatableBase, IPhysical, IRenderable, ISelectable {
		
		private float d0;
		private float d1;
		private int m;
		private int n;
		private int vx_Size;
		private int vx_buffer;
		private int elN;
		private int el_buffer;
		private float spring_constant;
		private float damping;
		private GCHandle verticesHandle;
		//private Resource flagTexture;
		private VertexT2fN3fV3f[] vertices;
		private Vector3[] velocity;
		private float radius;
		private float flagHeight;
		private float poleHeight;
		private float poleRadius;
		private Vector3 colorv = new Vector3(0.4f,0.2f,0.0f);
		private bool selected = false;
		
		public bool Selected {
			get {
				return this.selected;
			}
			set {
				this.selected = value;
				if(value) {
					this.colorv.Y = 0.0f;
					this.colorv.Z = 0.0f;
				}
				else {
					this.colorv = new Vector3(0.4f,0.2f,0.0f);
				}
			}
		}
		
		public LinuxFlag (int m, int n, float flagWidth, float spring_constant, float damping, float flagAltitude, float poleRadius) {
			//this.flagTexture = ResourceManager.GetTextureResource("tux.png");
			this.m = m;
			this.n = n;
			int N = m*n;
			#region generate_vertices
			int n1 = n-0x01;
			int m1 = m-0x01;
			float t0x = 1.0f/n1;
			float t0y = 1.0f/m1;
			float d0 = flagWidth*t0x;
			this.d0 = d0;
			this.d1 = (float) (Math.Sqrt(2.0f)*d0);
			this.flagHeight = flagWidth*m/n;
			this.poleHeight = flagHeight+flagAltitude;
			this.poleRadius = poleRadius;
			int arrayPos = 0x00;
			VertexT2fN3fV3f[] vertices = new VertexT2fN3fV3f[N];
			this.verticesHandle = GCHandle.Alloc(vertices,GCHandleType.Pinned);
			Vector3 nrm = new Vector3(0.0f,0.0f,-1.0f);
			float py, ty;
			for(int y = 0x00; y < m; y++) {
				py = flagHeight-d0*y+flagAltitude;
				ty = t0y*y;
				for(int x = 0x00; x < n; x++) {
					vertices[arrayPos].Texture = new Vector2(x*t0x,ty);
					vertices[arrayPos].Normal = nrm;
					vertices[arrayPos++].Position = new Vector3(d0*x,py,0.0f);
				}
			}
			int vertexBuffer;
			GL.GenBuffers(0x01,out vertexBuffer);
			this.vx_buffer = vertexBuffer;
			this.vx_Size = vertices.Length*0x08*sizeof(float);
			this.velocity = new Vector3[N];
			this.vertices = vertices;
			this.loadVertices();
			#endregion
			#region generate_indices
			this.elN = 6*m1*n1;
			int N2 = this.elN;
			int[] indices = new int[N2];
			arrayPos = 0x00;
			int l1 = 0;
			int l2 = n;
			int l3;
			for(; l2 < N;) {
				l3 = l1+n1;
				for(; l1 < l3;) {
					indices[arrayPos++] = l2++;
					indices[arrayPos++] = l1;
					indices[arrayPos++] = l2;//l2+0x01
					
					indices[arrayPos++] = l1++;
					indices[arrayPos++] = l1;//l1+0x01
					indices[arrayPos++] = l2;//l2+0x01
				}
				l1++;
				l2++;
			}
			int indexBuffer;
			GL.GenBuffers(0x01, out indexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer,new IntPtr(indices.Length*sizeof(int)),indices,BufferUsageHint.StaticDraw);
			this.el_buffer = indexBuffer;
			#endregion
			#region set_variables
			this.spring_constant = spring_constant;
			this.damping = damping;
			#endregion
		}
		
		~LinuxFlag () {
			this.verticesHandle.Free();
		}
		
		private void loadVertices () {
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.vx_buffer);
			GL.BufferData(BufferTarget.ArrayBuffer,new IntPtr(this.vx_Size),this.verticesHandle.AddrOfPinnedObject(),BufferUsageHint.StaticDraw);
		}
		
		private Vector3 getForce (int x, int y, Vector3 b, float stdDistance) {
			if(x < 0x00 || y < 0x00 || x >= this.n|| y >= this.m)
				return Vector3.Zero;
			Vector3 diff = this.vertices[y*this.n+x].Position-b;
			float len = diff.Length;
			if(len == 0.0f) {
				return Vector3.Zero;
			}
			else {
				float delta = this.spring_constant*(len-stdDistance);
				//if(Math.Abs(delta) > 50.0f)
				//	return Vector3.Zero;
				return diff*delta/len;
			}
		}
		private void updateNormals () {
			int pos = 0x00;
			Vector3 dx, dy;
			for(int y = 0x00; y < m; y++) {
				for(int x = 0x00; x < n; x++) {
					if(x == 0x00) {
						dx = this.vertices[pos+0x01].Position-this.vertices[pos].Position;
					}
					else if(x == n-1) {
						dx = this.vertices[pos].Position-this.vertices[pos-0x01].Position;
					}
					else {
						dx = this.vertices[pos+0x01].Position-this.vertices[pos-0x01].Position;
					}
					if(y == 0x00) {
						dy = this.vertices[pos+n].Position-this.vertices[pos].Position;
					}
					else if(y == m-1) {
						dy = this.vertices[pos].Position-this.vertices[pos-n].Position;
					}
					else {
						dy = this.vertices[pos+n].Position-this.vertices[pos-n].Position;
					}
					this.vertices[pos++].Normal = Vector3.Normalize(Vector3.Cross(dx,dy));
				}
			}
		}
		public void Render (FrameEventArgs e) {
			GL.PushMatrix();
			GL.Translate(this.Location);
			//this.flagTexture.activate();
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.PushAttrib(AttribMask.EnableBit|AttribMask.LightingBit);
			GL.LightModel(LightModelParameter.LightModelTwoSide,0x01);
			//GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.CullFace);
			GL.Color3(colorv);//GL.Color3(1.0f,1.0f,1.0f);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.vx_buffer);
			int stride = 0x08*sizeof(float);
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,IntPtr.Zero);
			GL.NormalPointer(NormalPointerType.Float,stride,new IntPtr(Vector2.SizeInBytes));
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,new IntPtr(Vector2.SizeInBytes+Vector3.SizeInBytes));
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,this.el_buffer);
			GL.DrawElements(BeginMode.Triangles,this.elN,DrawElementsType.UnsignedInt,0x00);
			GL.Finish();
			GL.PopAttrib();
			GL.PopClientAttrib();
			float r2 = 0.5f*this.poleRadius;
			float y = this.poleHeight;
			GL.Begin(BeginMode.Quads);
			GL.Normal3(0.0f,0.0f,-1.0f);
			GL.Vertex3(-r2,0.0f,-r2);
			GL.Vertex3(-r2,y,-r2);
			GL.Vertex3(r2,y,-r2);
			GL.Vertex3(r2,0.0f,-r2);
			
			GL.Normal3(0.0f,0.0f,1.0f);
			GL.Vertex3(r2,0.0f,r2);
			GL.Vertex3(r2,y,r2);
			GL.Vertex3(-r2,y,r2);
			GL.Vertex3(-r2,0.0f,r2);
			
			GL.Normal3(-1.0f,0.0f,0.0f);
			GL.Vertex3(-r2,0.0f,r2);
			GL.Vertex3(-r2,y,r2);
			GL.Vertex3(-r2,y,-r2);
			GL.Vertex3(-r2,0.0f,-r2);
			
			GL.Normal3(1.0f,0.0f,0.0f);
			GL.Vertex3(r2,0.0f,-r2);
			GL.Vertex3(r2,y,-r2);
			GL.Vertex3(r2,y,r2);
			GL.Vertex3(r2,0.0f,r2);
			
			GL.Normal3(0.0f,1.0f,0.0f);
			GL.Vertex3(r2,y,-r2);
			GL.Vertex3(-r2,y,-r2);
			GL.Vertex3(-r2,y,r2);
			GL.Vertex3(r2,y,r2);
			GL.End();
			GL.PopMatrix();
		}
		public void AdvanceTime (float time, Vector3 gravity, Vector3 wind) {
			time = Math.Min(time,0.04f);
			Vector3 A, P;
			Vector3 windN = Vector3.Normalize(wind);
			int m = this.m;
			int m1 = m-0x01;
			int n = this.n;
			int n1 = n-0x01;
			float damping = this.damping;
			float d0 = this.d0;
			float d1 = this.d1;
			#region update_velocities
			for(int y = 0x00; y < m; y++) {
				for(int x = 0x01; x < n; x++) {
					int pos = y*n+x;
					A = gravity+wind*Math.Abs(Vector3.Dot(windN,this.vertices[pos].Normal));
					P = this.vertices[pos].Position;
					A += this.getForce(x-0x01,y,P,d0)+this.getForce(x+0x01,y,P,d0)+this.getForce(x,y-0x01,P,d0)+this.getForce(x,y+0x01,P,d0);
					A += this.getForce(x-0x01,y-0x01,P,d1)+this.getForce(x+0x01,y-0x01,P,d1)+this.getForce(x-0x01,y+0x01,P,d1)+this.getForce(x+0x01,y+0x01,P,d1);
					this.velocity[pos] += A*time;
					this.velocity[pos] *= damping;
				}
			}
			Vector3 dx;
			for(int i = this.velocity.Length-0x01; i >= 0x00; i--) {
				dx = this.velocity[i]*time;
				if(dx.LengthFast <= 0.1f) {
					this.vertices[i].Position += dx;
				}
				else {
					this.vertices[i].Position += 0.1f*Vector3.Normalize(dx);
				}
			}
			#endregion
			this.updateNormals();
			this.loadVertices();
		}
		
	}
}
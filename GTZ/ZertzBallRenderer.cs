using System;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GTZ.Utils;
using GTZ.Zertz;

namespace GTZ.Rendering.Zertz {
	
	public class ZertzBallRenderer : MovableLocatableBase, IRenderable, ISelectable, ITimeSensitive {
		
		private static int numberOfInstances = 0x00;
		private static object lck = new object();
		private static int xntBuff = 0x00, xntN = 0x00, texBuff = 0x00;
		private static Vector3[] colorVectors = new Vector3[] {new Vector3(0.1f,0.1f,0.1f),new Vector3(0.45f,0.45f,0.45f),new Vector3(1.0f,1.0f,1.0f)};
		public const float RADIUS = 0.15f;
		public const int DEPTH = 0x01;
		private ZertzBallType type;
		private Vector3 colorv;
		private bool selected = false;
		private ZertzBallContainerType container = ZertzBallContainerType.Common;
		
		public ZertzBallType Type {
			get {
				return this.type;
			}
		}
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
					this.colorv = colorVectors[(byte) type];
				}
			}
		}
		public ZertzBallContainerType Container {
			get {
				return this.container;
			}
			set {
				this.container = value;
			}
		}
		
		public ZertzBallRenderer (ZertzBallType type) {
			register();
			this.type = type;
			this.colorv = colorVectors[(byte) type];
		}
		
		~ZertzBallRenderer () {
			unregister();
		}
		
		private void register () {
			lock(lck) {
				if(numberOfInstances <= 0x00) {
					xntBuff = MeshBuilder.BuildSphere(DEPTH,RADIUS,out xntN);//loading mesh
					Texture t = TextureFactory.Marble(512,512,0.15f);
					t.QuadMirror();
					texBuff = t.GenerateOpenGLBuffer();
				}
				numberOfInstances++;
			}
		}
		private void unregister () {
			lock(lck) {
				numberOfInstances--;
				if(numberOfInstances <= 0x00) {
					Unloader.DeleteBuffer(ref xntBuff);
					Unloader.DeleteTexture(texBuff);
				}
			}
		}
		
		public static ZertzBallRenderer[] GenerateBalls (ZertzBallContainer zbc, RenderContainer rc, ZertzCupRenderer zcr, int offsetid) {
			int id = offsetid;
			ZertzBallType zbt;
			ZertzBallRenderer zbr;
			ZertzBallRenderer[] list = new ZertzBallRenderer[zbc.Total];
			int j = 0x00;
			for(byte b = 0x00; b < 0x03; b++) {
				zbt = (ZertzBallType) b;
				for(int i = 0x00; i < zbc[zbt]; i++) {
					zbr = new ZertzBallRenderer(zbt);
					list[j++] = zbr;
					rc.Add(id++,zbr);
					zbr.RenderMover = RenderMoveManager.GenerateStaticMover(zcr.CommonContainer.Add(zbr));
				}
			}
			return list;
		}
		public void AdvanceTime (float time) {
			this.MoveTime += time;
		}
		public void Render (OpenTK.FrameEventArgs e) {
			GL.PushAttrib(AttribMask.EnableBit);
			GL.BindTexture(TextureTarget.Texture2D,texBuff);
			GL.PushMatrix();
			GL.Translate(this.RenderMover(this));
			GL.Enable(EnableCap.Texture2D);
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.Color3(colorv);
			GL.BindBuffer(BufferTarget.ArrayBuffer,xntBuff);
			int stride = sizeof(float)<<0x03;
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.DrawArrays(BeginMode.Triangles,0x00,xntN);
			GL.PopMatrix();
			GL.Color3(1.0f,1.0f,1.0f);
			GL.PopClientAttrib();
			GL.PopAttrib();
		}
		
	}
	
}
using System;
using System.Collections.Generic;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GTZ.Rendering {
	
	public class PopupBook : IRenderable, ILoadable, ITimeSensitive {
		
		private int cuboidGL, popupGL, glTa, glTb, glB;
		private readonly int maxLayer = 0x00;
		private float angle = 0.0f;
		private bool opening = false;
		private bool closing = false;
		private const float OPENING_SPEED = 100.0f;
		private const float WIDTH = 16.0f;//7.865853f;
		private const float THICK = 0.375f;//0.75f;
		private const float DEPTH = 8.5f;//6.18f;
		private const float WALL = 8.0f;
		private readonly PopupBookItem[] items;
		private readonly PopupBookItem[] leftItems;
		private readonly PopupBookItem[] rightItems;
		private readonly Matrix4[] matrices;
		private Matrix4 txL = new Matrix4(0.5f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,1.0f);
		private Matrix4 txR = new Matrix4(0.5f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.0f,0.0f,0.0f,1.0f,0.0f,0.5f,0.0f,0.0f,1.0f);
		private Matrix4 topBook = Matrix4.Identity;
		private Matrix4 bgLeft = Matrix4.Identity;
		private Matrix4 bgRight = Matrix4.Identity;
		
		public PopupBook (PopupBookItem[] items) {
			this.items = (PopupBookItem[]) items.Clone();
			Stack<PopupBookItem> leftI = new Stack<PopupBookItem>();
			Stack<PopupBookItem> righI = new Stack<PopupBookItem>();
			this.matrices = new Matrix4[items.Length];
			foreach(PopupBookItem pbi in items) {
				if(pbi.Layer > this.maxLayer) {
					this.maxLayer = pbi.Layer;
				}
				if(pbi.X <= 0.0f) {
					leftI.Push(pbi);
				}
				else {
					righI.Push(pbi);
				}
			}
			this.maxLayer++;
			this.leftItems = leftI.ToArray();
			Array.Sort(this.leftItems);
			this.rightItems = righI.ToArray();
			Array.Sort(this.rightItems);
			this.AdvanceTime(0.0f);
		}
		
		public void Open () {
			this.opening = true;
		}
		public void Close () {
			this.closing = true;
		}
		public void SetItemAngle (int index, float angle, bool immidiatly) {
			this.items[index].ToAngle(angle,immidiatly);
		}
		public void OnLoad (EventArgs e) {
			this.cuboidGL = MeshBuilder.BuildCuboid(0.5f*WIDTH,THICK,DEPTH);
			this.popupGL = MeshBuilder.BuildPopupRectangle();
			this.glTa = new Texture(Assembly.GetExecutingAssembly().GetManifestResourceStream("GTZ.resources.popbook_bg.jpg")).GenerateOpenGLBuffer();
			this.glTb = new Texture(Assembly.GetExecutingAssembly().GetManifestResourceStream("GTZ.resources.gods.png")).GenerateOpenGLBuffer();
			this.glB = new Texture(Assembly.GetExecutingAssembly().GetManifestResourceStream("GTZ.resources.book.jpg")).GenerateOpenGLBuffer();
			pbri.OnLoad(e);
		}
		public void AdvanceTime (float time) {
			if(this.opening) {
				this.angle = Math.Min(300.0f,OPENING_SPEED*time+angle);
				if(this.angle >= 300.0f) {
					this.opening = false;
				}
			}
			else if(this.closing) {
				this.angle = Math.Max(0.0f,angle-OPENING_SPEED*time);
				if(this.angle <= 0.0f) {
					this.closing = false;
				}
			}
			
			int i = 0x00;
			Matrix4 M;
			PopupBookItem pbi;
			for(; i < rightItems.Length; i++) {
				pbi = this.rightItems[i];
				pbi.AdvanceTime(time);
				M = Matrix4.Scale(pbi.Width,pbi.Height,1.0f)*Matrix4.CreateRotationX(pbi.Angle)*Matrix4.CreateRotationY(-pbi.Theta)*Matrix4.CreateTranslation(pbi.X,MeshBuilder.PopupDepth*pbi.Layer,-pbi.Y);
				this.matrices[i] = M;
			}
			for(int j = 0x00; j < leftItems.Length; j++) {
				pbi = this.leftItems[j];
				pbi.AdvanceTime(time);
				M = Matrix4.Scale(pbi.Width,pbi.Height,1.0f)*Matrix4.CreateRotationX(pbi.Angle)*Matrix4.CreateRotationY((float) (Math.PI-pbi.Theta))*Matrix4.CreateTranslation(-pbi.X,MeshBuilder.PopupDepth*pbi.Layer,-DEPTH+pbi.Y);
				this.matrices[i++] = M;
			}
			
			double a = Math.Max(0.0f,Math.PI-Math.Min(angle*Math.PI/180.0d,Math.PI));
			float sin = (float) Math.Sin(a);
			float cos = (float) Math.Cos(a);
			this.topBook = new Matrix4(-cos,sin,0.0f,0.0f,sin,cos,0.0f,0.0f,0.0f,0.0f,-1.0f,0.0f,0.0f,0.0f,-DEPTH,1.0f);
			
			a = -Math.Max(-0.5d*Math.PI,Math.Min(0.0f,-1.5d*Math.PI+this.angle*Math.PI/180.0d));//;
			sin = (float) Math.Sin(a);
			cos = (float) Math.Cos(a);
			this.bgRight = Matrix4.Scale(0.5f*WIDTH,WALL,1.0f)*Matrix4.CreateRotationX((float) a)*Matrix4.CreateTranslation(0.25f*WIDTH,MeshBuilder.PopupDepth*maxLayer,-DEPTH);
			
			a = -Math.Max(-0.5d*Math.PI,Math.Min(0.0f,-5.0d/3.0d*Math.PI+this.angle*Math.PI/180.0d));//;
			sin = (float) Math.Sin(a);
			cos = (float) Math.Cos(a);
			this.bgLeft = Matrix4.Scale(0.5f*WIDTH,WALL,1.0f)*Matrix4.CreateRotationX((float) a)*Matrix4.CreateRotationY((float) Math.PI)*Matrix4.CreateTranslation(0.25f*WIDTH,MeshBuilder.PopupDepth*maxLayer,0.0f);
			this.pbri.AdvanceTime(time);
		}
		public void Render (FrameEventArgs e) {
			GL.Color3(1.0f,1.0f,1.0f);
			GL.Translate(0.0f,-0.25f*WALL,0.5f*DEPTH);
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.PushAttrib(AttribMask.EnableBit);
			GL.Enable(EnableCap.Texture2D);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.cuboidGL);
			int stride = 0x08*sizeof(float);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.BindTexture(TextureTarget.Texture2D,this.glB);
			GL.DrawArrays(BeginMode.Quads,0x00,0x18);
			
			GL.PushMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.BindTexture(TextureTarget.Texture2D,this.glTa);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.popupGL);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.MultMatrix(ref this.bgRight);
			GL.MatrixMode(MatrixMode.Texture);
			GL.LoadMatrix(ref this.txR);
			GL.DrawArrays(BeginMode.Quads,0x00,0x08);
			GL.LoadIdentity();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			GL.Disable(EnableCap.Blend);
			GL.BindTexture(TextureTarget.Texture2D,this.glB);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.cuboidGL);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.PushMatrix();
			GL.MultMatrix(ref this.topBook);
			GL.DrawArrays(BeginMode.Quads,0x00,0x18);
			
			GL.PushMatrix();
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D,this.glTa);
			GL.BindBuffer(BufferTarget.ArrayBuffer,this.popupGL);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.MultMatrix(ref this.bgLeft);
			GL.MatrixMode(MatrixMode.Texture);
			GL.LoadMatrix(ref this.txL);
			GL.DrawArrays(BeginMode.Quads,0x00,0x08);
			GL.LoadIdentity();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			GL.PopMatrix();
			
			GL.Enable(EnableCap.Blend);
			GL.BindTexture(TextureTarget.Texture2D,this.glTb);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
			for(int i = 0x00; i < this.rightItems.Length; i++) {
				GL.PushMatrix();
				GL.MultMatrix(ref this.matrices[i]);
				GL.MatrixMode(MatrixMode.Texture);
				GL.LoadMatrix(this.rightItems[i].TextureMatrix);
				if(this.rightItems[i].DefaultRendering) {
					GL.DrawArrays(BeginMode.Quads,0x00,0x08);
				}
				else {
					this.rightItems[i].Render(e);
				}
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PopMatrix();
			}
			
			GL.PushMatrix();
			GL.MultMatrix(ref this.topBook);
			
			for(int i = 0x00; i < this.leftItems.Length; i++) {
				GL.PushMatrix();
				GL.MultMatrix(ref this.matrices[i+this.rightItems.Length]);
				GL.MatrixMode(MatrixMode.Texture);
				GL.LoadMatrix(this.leftItems[i].TextureMatrix);
				if(this.leftItems[i].DefaultRendering) {
					GL.DrawArrays(BeginMode.Quads,0x00,0x08);
				}
				else {
					this.leftItems[i].Render(e);
				}
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PopMatrix();
			}
			
			GL.MatrixMode(MatrixMode.Texture);
			GL.LoadIdentity();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			
			GL.PopClientAttrib();
			GL.PopAttrib();
			GL.PushMatrix();
			GL.Translate(0.0f,2.0f,0.0f);
			this.pbri.Render(e);
			GL.PopMatrix();
		}
		
	}
	
}
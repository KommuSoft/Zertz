using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Img = System.Drawing.Imaging;
using GTZ.EgyptStyle;
using GTZ.Utils;
using OpenTK;
using Glu = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GTZ.Rendering {
	
	public class StateIndicator : SizableBase, ILoadable, IRenderable {
		
		private float rRadio;
		private PointF[] statePoints;
		private int texturePointer;
		private Vector3[] vs;
		private int currentState = -0x01;
		private readonly Bitmap visualState;
		private readonly RectangleF visualBounds;
		private readonly float visualFactor;
		private int visualGL = -0x01;
		public int CurrentState {
			get {
				return this.currentState;
			}
			set {
				this.currentState = value;
			}
		}
		
		public StateIndicator (int width, int height, float rRadio, Bitmap visualState, RectangleF visualBounds, float visualFactor, PointF[] statePoints) : base(width,height) {
			this.rRadio = rRadio;
			this.statePoints = statePoints;
			this.toDelta();
			this.visualState = visualState;
			this.visualBounds = visualBounds;
			this.visualFactor = visualFactor;
		}
		
		private void toDelta () {
			this.vs = new Vector3[this.statePoints.Length];
			Vector3 v,v2;
			if(this.vs.Length > 0x00) {
				v = new Vector3(statePoints[0x00].X,statePoints[0x00].Y,0.0f);
				this.vs[0x00] = v;
				for(int i = 0x01; i < this.vs.Length; i++) {
					v2 = new Vector3(statePoints[i].X,statePoints[i].Y,0.0f);
					this.vs[i] = v2-v;
					v = v2;
				}
			}
		}
		public void RefreshVisual () {
			Img.BitmapData bmd = this.visualState.LockBits(new Rectangle(0x00,0x00,this.visualState.Width,this.visualState.Height),Img.ImageLockMode.ReadOnly,Img.PixelFormat.Format32bppArgb);
			GL.BindTexture(TextureTarget.Texture2D,this.visualGL);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);
			GL.TexImage2D(TextureTarget.Texture2D,0x00,PixelInternalFormat.Rgba,this.visualState.Width,this.visualState.Height,0x00,PixelFormat.Bgra,PixelType.UnsignedByte,bmd.Scan0);
			this.visualState.UnlockBits(bmd);
		}
		public void OnLoad (EventArgs e) {
			this.texturePointer = EgyptInformation.Textures.GetTexture(0x00);
			this.visualGL = GL.GenTexture();
			this.RefreshVisual();
		}
		public void Render (OpenTK.FrameEventArgs e) {
			GL.Color4(1.0f,1.0f,1.0f,1.0f);
			GL.BlendColor(1.0f,1.0f,1.0f,1.0f);
			GL.PushAttrib(AttribMask.EnableBit);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.Blend);
			GL.BindTexture(TextureTarget.Texture2D,this.texturePointer);
			float f1_4 = 0.25f;
			float f1_2 = 0.5f;
			float f3_4 = 0.75f;
			
			GL.PushMatrix();
			//GL.MultTransposeMatrix(ref this.mLeft);
			GL.Begin(BeginMode.Quads);
			
			//left cartouche
			GL.TexCoord2(0.0f,0.0f);		GL.Vertex2(0.0f,0.0f);
			GL.TexCoord2(f1_4,0.0f);		GL.Vertex2(64.0f,0.0f);
			GL.TexCoord2(f1_4,f1_4);		GL.Vertex2(64.0f,64.0f);
			GL.TexCoord2(0.0f,f1_4);		GL.Vertex2(0.0f,64.0f);
			
			GL.TexCoord2(f1_4,0.0f);		GL.Vertex2(64.0f,0.0f);
			GL.TexCoord2(f1_2,0.0f);		GL.Vertex2(this.Width-64.0f,0.0f);
			GL.TexCoord2(f1_2,f1_4);		GL.Vertex2(this.Width-64.0f,64.0f);
			GL.TexCoord2(f1_4,f1_4);		GL.Vertex2(64.0f,64.0f);
			
			GL.TexCoord2(f1_2,0.0f);		GL.Vertex2(this.Width-64.0f,0.0f);
			GL.TexCoord2(f3_4,0.0f);		GL.Vertex2(this.Width,0.0f);
			GL.TexCoord2(f3_4,f1_4);		GL.Vertex2(this.Width,64.0f);
			GL.TexCoord2(f1_2,f1_4);		GL.Vertex2(this.Width-64.0f,64.0f);
			
			GL.TexCoord2(0.0f,f1_4);		GL.Vertex2(0.0f,64.0f);
			GL.TexCoord2(f1_4,f1_4);		GL.Vertex2(64.0f,64.0f);
			GL.TexCoord2(f1_4,f1_2);		GL.Vertex2(64.0f,this.Height);
			GL.TexCoord2(0.0f,f1_2);		GL.Vertex2(0.0f,this.Height);
			
			GL.TexCoord2(f1_4,f1_4);		GL.Vertex2(64.0f,64.0f);
			GL.TexCoord2(f1_2,f1_4);		GL.Vertex2(this.Width-64.0f,64.0f);
			GL.TexCoord2(f1_2,f1_2);		GL.Vertex2(this.Width-64.0f,this.Height);
			GL.TexCoord2(f1_4,f1_2);		GL.Vertex2(64.0f,this.Height);
			
			GL.TexCoord2(f1_2,f1_4);		GL.Vertex2(this.Width-64.0f,64.0f);
			GL.TexCoord2(f3_4,f1_4);		GL.Vertex2(this.Width,64.0f);
			GL.TexCoord2(f3_4,f1_2);		GL.Vertex2(this.Width,this.Height);
			GL.TexCoord2(f1_2,f1_2);		GL.Vertex2(this.Width-64.0f,this.Height);
			
			for(int i = 0x00; i < this.CurrentState; i++) {
				GL.End();
				GL.Translate(this.Width*this.vs[i].X,this.Height*this.vs[i].Y,0.0f);
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0.0f,f3_4);		GL.Vertex2(-this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_4,f3_4);		GL.Vertex2(this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_4,1.0f);		GL.Vertex2(this.rRadio,this.rRadio);
				GL.TexCoord2(0.0f,1.0f);		GL.Vertex2(-this.rRadio,this.rRadio);
			}
			if(this.currentState >= 0x00) {
				GL.End();
				GL.Translate(this.Width*this.vs[this.CurrentState].X,this.Height*this.vs[this.CurrentState].Y,0.0f);
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(f1_4,f3_4);		GL.Vertex2(-this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_2,f3_4);		GL.Vertex2(this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_2,1.0f);		GL.Vertex2(this.rRadio,this.rRadio);
				GL.TexCoord2(f1_4,1.0f);		GL.Vertex2(-this.rRadio,this.rRadio);
			}
			for(int i = this.CurrentState+0x01; i < this.vs.Length; i++) {
				GL.End();
				GL.Translate(this.Width*this.vs[i].X,this.Height*this.vs[i].Y,0.0f);
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0.0f,f3_4);		GL.Vertex2(-this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_4,f3_4);		GL.Vertex2(this.rRadio,-this.rRadio);
				GL.TexCoord2(f1_4,1.0f);		GL.Vertex2(this.rRadio,this.rRadio);
				GL.TexCoord2(0.0f,1.0f);		GL.Vertex2(-this.rRadio,this.rRadio);
			}
			/*//left wing
			GL.TexCoord2(f1_12,0.0f);		GL.Vertex2(0.5f*this.Height,0.0f);
			GL.TexCoord2(0.25f,0.0f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,0.0f);
			GL.TexCoord2(0.25f,0.5f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,this.Height);
			GL.TexCoord2(f1_12,0.5f);		GL.Vertex2(0.5f*this.Height,this.Height);
			//body
			GL.TexCoord2(0.25f,0.0f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,0.0f);
			GL.TexCoord2(0.5f,0.0f);		GL.Vertex2(this.innerWidth*0.5f,0.0f);
			GL.TexCoord2(0.5f,0.5f);		GL.Vertex2(this.innerWidth*0.5f,this.Height);
			GL.TexCoord2(0.25f,0.5f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,this.Height);
			//LEFT TURQUOISE
			GL.Color4(1.0f,1.0f,1.0f,leftClock);
			GL.BlendColor(1.0f,1.0f,1.0f,leftClock);
			//left cartouche
			GL.TexCoord2(0.0f,0.5f);		GL.Vertex2(0.0f,0.0f);
			GL.TexCoord2(f1_12,0.5f);		GL.Vertex2(0.5f*this.Height,0.0f);
			GL.TexCoord2(f1_12,1.0f);		GL.Vertex2(0.5f*this.Height,this.Height);
			GL.TexCoord2(0.0f,1.0f);		GL.Vertex2(0.0f,this.Height);
			//left wing
			GL.TexCoord2(f1_12,0.5f);		GL.Vertex2(0.5f*this.Height,0.0f);
			GL.TexCoord2(0.25f,0.5f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,0.0f);
			GL.TexCoord2(0.25f,1.0f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,this.Height);
			GL.TexCoord2(f1_12,1.0f);		GL.Vertex2(0.5f*this.Height,this.Height);
			//body
			GL.TexCoord2(0.25f,0.5f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,0.0f);
			GL.TexCoord2(0.5f,0.5f);		GL.Vertex2(this.innerWidth*0.5f,0.0f);
			GL.TexCoord2(0.5f,1.0f);		GL.Vertex2(this.innerWidth*0.5f,this.Height);
			GL.TexCoord2(0.25f,1.0f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,this.Height);*/
			
			GL.End();
			/*GL.PopMatrix();
			GL.PushMatrix();
			GL.MultTransposeMatrix(ref this.mRight);
			GL.Begin(BeginMode.Quads);
			
			//right wing
			GL.TexCoord2(1.0f-f1_12,0.0f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,0.0f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,0.5f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,this.Height);
			GL.TexCoord2(1.0f-f1_12,0.5f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,this.Height);
			//right cartouche
			GL.TexCoord2(1.0f,0.0f);		GL.Vertex2(this.innerWidth,0.0f);
			GL.TexCoord2(1.0f-f1_12,0.0f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,0.0f);
			GL.TexCoord2(1.0f-f1_12,0.5f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,this.Height);
			GL.TexCoord2(1.0f,0.5f);		GL.Vertex2(this.innerWidth,this.Height);
			//body
			GL.TexCoord2(0.5f,0.0f);		GL.Vertex2(this.innerWidth*0.5f,0.0f);
			GL.TexCoord2(0.75f,0.0f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,0.5f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,this.Height);
			GL.TexCoord2(0.5f,0.5f);		GL.Vertex2(this.innerWidth*0.5f,this.Height);
			//RIGHT TURQUOISE
			GL.Color4(1.0f,1.0f,1.0f,rightClock);
			GL.BlendColor(1.0f,1.0f,1.0f,rightClock);
			//body
			GL.TexCoord2(0.5f,0.5f);		GL.Vertex2(this.innerWidth*0.5f,0.0f);
			GL.TexCoord2(0.75f,0.5f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,1.0f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,this.Height);
			GL.TexCoord2(0.5f,1.0f);		GL.Vertex2(this.innerWidth*0.5f,this.Height);
			//right wing
			GL.TexCoord2(1.0f-f1_12,0.5f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,0.5f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,0.0f);
			GL.TexCoord2(0.75f,1.0f);		GL.Vertex2(this.innerWidth*0.5f+1.5f*this.Height,this.Height);
			GL.TexCoord2(1.0f-f1_12,1.0f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,this.Height);
			//right cartouche
			GL.TexCoord2(1.0f,0.5f);		GL.Vertex2(this.innerWidth,0.0f);
			GL.TexCoord2(1.0f-f1_12,0.5f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,0.0f);
			GL.TexCoord2(1.0f-f1_12,1.0f);	GL.Vertex2(this.innerWidth-0.5f*this.Height,this.Height);
			GL.TexCoord2(1.0f,1.0f);		GL.Vertex2(this.innerWidth,this.Height);
			GL.End();*/
			GL.PopMatrix();
			
			GL.BindTexture(TextureTarget.Texture2D,this.visualGL);
			GL.Begin(BeginMode.Quads);
			float x0 = visualBounds.X;
			float x1 = visualBounds.Right;
			float y0 = visualBounds.Y;
			float y1 = visualBounds.Bottom;
			float vy0 = 64.0f*(1.0f-Maths.Sqrt1_2);
			float vx1 = this.Width-vy0;
			float vx0 = vx1-(this.Height-vy0)*this.visualFactor;
			GL.TexCoord2(x0,y0);		GL.Vertex2(vx0,vy0);
			GL.TexCoord2(x1,y0);		GL.Vertex2(vx1,vy0);
			GL.TexCoord2(x1,y1);		GL.Vertex2(vx1,this.Height);
			GL.TexCoord2(x0,y1);		GL.Vertex2(vx0,this.Height);
			GL.End();
			
			
			GL.PopAttrib();
		}
		
		
	}
}


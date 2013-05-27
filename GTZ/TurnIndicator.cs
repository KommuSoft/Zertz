using GTZ.EgyptStyle;
using GTZ.Utils;
using OpenTK;
using Glu = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GTZ.Rendering {

	public class TurnIndicator : SizableBase, IRenderable, ITimeSensitive, ILoadable {
		
		private int texturePointer = -0x01;
		private float leftClock = 1.0f, rightClock = 0.0f;
		private float lFactor = -1.0f, rFactor = -1.0f;
		private float angle = (float) (-Math.PI);
		private Matrix4 mLeft = Matrix4.Identity, mRight = Matrix4.Identity;
		private float innerWidth;
		
		public byte Turn {
			set {
				if((value&0x01)==0x01)
					this.lFactor = 1.0f;
				else
					this.lFactor = -1.0f;
				if((value&0x02)==0x02)
					this.rFactor = 1.0f;
				else
					this.rFactor = -1.0f;
			}
		}
		
		public TurnIndicator (int width, int height) : base(width,height) {
			this.innerWidth = width;
			this.onTurnChanged(0x01);
		}
		
		private void onTurnChanged (byte newOwner) {
			this.Turn = newOwner;
		}
		public void AdvanceTime (float time) {
			angle += 0.5f*time;
			this.calculateFlight(Math.Max((float) (-0.5d*Math.PI),Math.Min(0.0f,angle)),Math.Max(Math.Min(1.0f,2.0f*angle),0.0f));
		}
		private void calculateFlight (float angle, float thickness) {
			this.innerWidth = (this.Width-6.0f*this.Height)*thickness+6.0f*this.Height;
			float dx = 0.5f*(this.Width-this.innerWidth);
			float cos = (float) Math.Cos(angle);
			float sin = (float) Math.Sin(angle);
			float w = 0.5f*this.innerWidth;
			float h = 0.5f*this.Height;
			float c1 = h-w;
			float c2 = h+w;
			this.mLeft = new Matrix4(	cos,	-sin,	0.0f,	cos*c1+sin*h-c1+dx,
										sin,	cos,	0.0f,	h+sin*c1-cos*h,
										0.0f,	0.0f,	1.0f,	0.0f,
										0.0f,	0.0f,	0.0f,	1.0f);
			this.mRight = new Matrix4(	cos,	sin,	0.0f,	c2-cos*c2-sin*h+dx,
										-sin,	cos,	0.0f,	h+sin*c2-cos*h,
										0.0f,	0.0f,	1.0f,	0.0f,
										0.0f,	0.0f,	0.0f,	1.0f);
		}
		public void OnLoad (EventArgs e) {
			Bitmap bmp = new Bitmap(this.Height*0x06,this.Height*0x02);
			Graphics g = Graphics.FromImage(bmp);
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			
			float w = 6.0f*this.Height;
			int hInt = this.Height;
			float h = (float) hInt;
			float margin = h/48.0f;
			float m2 = 2.0f*margin;
			float r = h/36.0f;///36.0f
			float r2 = 2.0f*r;
			float h_2 = 0.5f*h;
			float Ro = h_2-margin;
			float alpha = (float) (180.0d/Math.PI*Math.Acos(1.0d-0.5d*r/Ro));
			float beta = (float) (180.0d/Math.PI*Math.Acos(1.0d-r/Ro));
			float betaY = (float) Math.Sqrt(Ro*Ro-(Ro-r)*(Ro-r));
			float sqrtR2 = (float) (Math.Sqrt(0.5d)*(h-m2-r2));
			float __R2 = h_2-margin-r;
			__R2 *= __R2;
			for(int l = 0x00; l < 0x02; l++) {
				using(GraphicsPath gpNocturne = new GraphicsPath()) {
					//left wing
					gpNocturne.AddArc(0.5f*w-h+m2,margin,h-m2,h-m2,180.0f+alpha,360.0f-2.0f*alpha);
					gpNocturne.AddArc(0.5f*w+2.0f*(m2-h)+r,margin,h-m2,h-m2,alpha,90.0f-alpha);
					gpNocturne.AddLine(0.5f*w+1.5f*m2+r-1.5f*h,h-margin,h_2,h-margin);
					gpNocturne.AddArc(margin,margin,h-m2,h-m2,90.0f,90.0f-beta);
					gpNocturne.AddLine(margin+r,h_2+betaY,margin+r,h-margin);
					gpNocturne.AddLine(margin+r,h-margin,margin,h-margin);
					gpNocturne.AddLine(margin,h-margin,margin,margin);
					gpNocturne.AddLine(margin,margin,margin+r,margin);
					gpNocturne.AddLine(margin+r,margin,margin+r,h_2-betaY);
					gpNocturne.AddArc(margin,margin,h-m2,h-m2,180.0f+beta,90.0f-beta);
					gpNocturne.AddLine(h_2,margin,0.5f*w+1.5f*m2+r-1.5f*h,margin);
					gpNocturne.AddArc(0.5f*w+2.0f*(m2-h)+r,margin,h-m2,h-m2,270.0f,90.0f-alpha);
					gpNocturne.CloseFigure();
					//right wing
					gpNocturne.AddArc(0.5f*w,margin,h-m2,h-m2,360.0f-alpha,2.0f*alpha-360.0f);
					gpNocturne.AddArc(0.5f*w+h-m2-r,margin,h-m2,h-m2,180.0f-alpha,alpha-90.0f);
					gpNocturne.AddLine(0.5f*w+2.0f*(h-m2)-r,h-margin,w-h_2,h-margin);
					gpNocturne.AddArc(w-h+margin,margin,h-m2,h-m2,90.0f,beta-90.0f);
					gpNocturne.AddLine(w-margin-r,h_2+betaY,w-margin-r,h-margin);
					gpNocturne.AddLine(w-margin-r,h-margin,w-margin,h-margin);
					gpNocturne.AddLine(w-margin,h-margin,w-margin,margin);
					gpNocturne.AddLine(w-margin,margin,w-margin-r,margin);
					gpNocturne.AddLine(w-margin-r,margin,w-margin-r,h_2-betaY);
					gpNocturne.AddArc(w-h+margin,margin,h-m2,h-m2,-beta,beta-90.0f);
					gpNocturne.AddLine(w-h_2,margin,0.5f*w+2.0f*(h-m2)-r,margin);
					gpNocturne.AddArc(0.5f*w+h-m2-r,margin,h-m2,h-m2,270.0f,alpha-90.0f);
					gpNocturne.CloseFigure();
					if(l == 0x00)
						g.FillPath(EgyptInformation.Brushes.EgyptNocturne,gpNocturne);
					else
						g.FillPath(EgyptInformation.Brushes.EgyptPaintBlue,gpNocturne);
						//g.FillPath(new SolidBrush(Color.FromArgb(0x4c,0x41,0x0b)),gpNocturne);
					using(GraphicsPath gpGold = new GraphicsPath()) {
						gpGold.AddPath(gpNocturne,false);
						//left inner
						gpGold.AddArc(margin+r,margin+r,h-m2-r2,h-m2-r2,90.0f,180.0f);
						gpGold.AddArc(0.5f*w+2.0f*(m2-h)+r2,margin+r,h-m2-r2,h-m2-r2,270.0f,180.0f);
						gpGold.CloseFigure();
						//left ellipse
						gpGold.AddEllipse(0.5f*w-h+m2+r,margin+r,h-m2-r2,h-m2-r2);
						gpGold.CloseFigure();
						//right inner
						gpGold.AddArc(w-h+margin+r,margin+r,h-m2-r2,h-m2-r2,90.0f,-180.0f);
						gpGold.AddArc(0.5f*w+h-m2,margin+r,h-m2-r2,h-m2-r2,270.0f,-180.0f);
						gpGold.CloseFigure();
						//right ellipse
						gpGold.AddEllipse(0.5f*w+r,margin+r,h-m2-r2,h-m2-r2);
						gpGold.CloseFigure();
						g.FillPath(EgyptInformation.Brushes.EgyptGold,gpGold);
						GraphicsUtils.DrawGlass(g,gpGold);
					}
					Matrix M = new Matrix();
					M.Scale(sqrtR2,sqrtR2);
					RectangleF bounds;
					Matrix Mtrans = new Matrix();
					GraphicsPath gpHedjet = EgyptInformation.GraphicsPaths.Hedjet();
					gpHedjet.Transform(M);
					bounds = gpHedjet.GetBounds();
					Mtrans.Translate(-0.5f*bounds.Width,0.0f);
					Mtrans.Scale(-1.0f,1.0f,MatrixOrder.Append);
					Mtrans.Translate(0.5f*(w-h+m2),h_2-0.5f*bounds.Height,MatrixOrder.Append);
					gpHedjet.Transform(Mtrans);
					g.FillPath(EgyptInformation.Brushes.EgyptPaintWhite,gpHedjet);
					GraphicsUtils.DrawGlass(g,gpHedjet);
					GraphicsPath gpDeshret = EgyptInformation.GraphicsPaths.Deshret();
					gpDeshret.Transform(M);
					bounds = gpDeshret.GetBounds();
					Mtrans.Reset();
					Mtrans.Translate(0.5f*(w+h-m2-bounds.Width),h_2-0.5f*bounds.Height);
					gpDeshret.Transform(Mtrans);
					g.FillPath(EgyptInformation.Brushes.EgyptPaintRed,gpDeshret);
					GraphicsUtils.DrawGlass(g,gpDeshret);
				}
				g.TranslateTransform(0.0f,h);
			}
			BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,bmp.Width,bmp.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			this.texturePointer = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D,this.texturePointer);
			GL.TexEnv(TextureEnvTarget.TextureEnv,TextureEnvParameter.TextureEnvMode,(float) TextureEnvMode.Modulate);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,(float) TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,(float) TextureMagFilter.Linear);
			Glu.Glu.Build2DMipmap(Glu.TextureTarget.Texture2D, (int) PixelInternalFormat.Four,bmp.Width,bmp.Height,OpenTK.Graphics.PixelFormat.Bgra,Glu.PixelType.UnsignedByte,bmd.Scan0);
			bmp.UnlockBits(bmd);
		}
		public void Render (OpenTK.FrameEventArgs e) {
			this.leftClock = (float) Math.Min(1.0d,Math.Max(0.0d,this.leftClock+this.lFactor*e.Time));
			this.rightClock = (float) Math.Min(1.0d,Math.Max(0.0d,this.rightClock+this.rFactor*e.Time));
			GL.Color4(1.0f,1.0f,1.0f,1.0f);
			GL.BlendColor(1.0f,1.0f,1.0f,1.0f);
			GL.PushAttrib(AttribMask.EnableBit);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.Blend);
			GL.BindTexture(TextureTarget.Texture2D,this.texturePointer);
			float f1_12 = 1.0f/12.0f;
			float f1_6 = 1.0f/6.0f;
			
			GL.PushMatrix();
			GL.MultTransposeMatrix(ref this.mLeft);
			GL.Begin(BeginMode.Quads);
			
			//left cartouche
			GL.TexCoord2(0.0f,0.0f);		GL.Vertex2(0.0f,0.0f);
			GL.TexCoord2(f1_12,0.0f);		GL.Vertex2(0.5f*this.Height,0.0f);
			GL.TexCoord2(f1_12,0.5f);		GL.Vertex2(0.5f*this.Height,this.Height);
			GL.TexCoord2(0.0f,0.5f);		GL.Vertex2(0.0f,this.Height);
			//left wing
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
			GL.TexCoord2(0.25f,1.0f);		GL.Vertex2(this.innerWidth*0.5f-1.5f*this.Height,this.Height);
			
			GL.End();
			GL.PopMatrix();
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
			GL.End();
			GL.PopMatrix();
			GL.PopAttrib();
		}
		
	}
	
}
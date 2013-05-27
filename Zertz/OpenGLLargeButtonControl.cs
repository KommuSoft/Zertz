using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Zertz.EgyptStyle;

namespace Zertz.Rendering {
	
	public class OpenGLLargeButtonControl : OpenGLComponent {
		
		private static int texture;
		private string text;
		private float to = 0.0f;
		
		protected override int DefaultWidth {
			get {
				return 0x0100;
			}
		}
		protected override int DefaultHeight {
			get {
				return 0x40;
			}
		}
		public string Text {
			get {
				return this.text;
			}
			set {
				this.text = value;
				this.Width = Math.Max(this.Width,0x02*this.Height+0x14*this.text.Length);
			}
		}
		
		public OpenGLLargeButtonControl () {
		}
		
		public override void OnLoad (EventArgs e) {
			texture = EgyptInformation.Textures.GetTexture(0x01);
		}
		
		public override void OnMouseEnter () {
			this.to = 0.25f;
		}
		public override void OnMouseLeave ()
		{
			this.to = 0.0f;
		}
		protected override void InternalRender (FrameEventArgs e) {
			GL.Color4(1.0f,1.0f,1.0f,1.0f);
			GL.BlendColor(1.0f,1.0f,1.0f,1.0f);
			GL.PushAttrib(AttribMask.EnableBit);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
			GL.BindTexture(TextureTarget.Texture2D,texture);
			
			GL.Begin(BeginMode.Quads);
			
			GL.TexCoord2(0.0f,to);			GL.Vertex2(0.0f,0.0f);
			GL.TexCoord2(0.5f,to);			GL.Vertex2(this.Height,0.0f);
			GL.TexCoord2(0.5f,to+0.25f);	GL.Vertex2(this.Height,this.Height);
			GL.TexCoord2(0.0f,to+0.25f);	GL.Vertex2(0.0f,this.Height);
			
			GL.TexCoord2(0.5f,to);			GL.Vertex2(this.Height,0.0f);
			GL.TexCoord2(1.0f,to);			GL.Vertex2(this.Width-this.Height+1.0f,0.0f);
			GL.TexCoord2(1.0f,to+0.25f);	GL.Vertex2(this.Width-this.Height+1.0f,this.Height);
			GL.TexCoord2(0.5f,to+0.25f);	GL.Vertex2(this.Height,this.Height);
			
			GL.TexCoord2(0.5f,to);			GL.Vertex2(this.Width-this.Height,0.0f);
			GL.TexCoord2(0.0f,to);			GL.Vertex2(this.Width,0.0f);
			GL.TexCoord2(0.0f,to+0.25f);	GL.Vertex2(this.Width,this.Height);
			GL.TexCoord2(0.5f,to+0.25f);	GL.Vertex2(this.Width-this.Height,this.Height);
			
			GL.End();
			
			GL.Disable(EnableCap.Texture2D);
			GL.RasterPos2((this.Width>>0x01)-0x0a*this.Text.Length-0x05,(this.Height>>0x01)-0x10);
			OpenGLFont.PrintString(this.Text);
			
			GL.PopAttrib();
		}
		
	}
}


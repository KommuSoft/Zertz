using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Zertz.Rendering {
	
	public class SubtitleRenderer : IRenderable, ITimeSensitive {
		
		private float wwidth;
		private float wheight;
		private const float height = 64.0f;
		private bool enabled = false;
		private string fulltext = string.Empty;
		private string[] text = new string[0x00];
		private float remain = 0.0f;
		
		public SubtitleRenderer () {
		}
		
		public void SetText (float seconds, string text) {
			this.Enable();
			this.remain = seconds;
			this.fulltext = text;
			this.recalcText();
		}
		public void AdvanceTime (float time) {
			if(this.remain > 0.0f) {
				this.remain = Math.Max(0.0f, this.remain-time);
				if(this.remain <= 0.0f) {
					this.text = new string[0x00];
				}
			}
		}
		public void Enable () {
			this.enabled = true;
		}
		public void Disable () {
			this.enabled = false;
		}
		public void Render (FrameEventArgs e) {
			if(this.enabled) {
				/*
				GL.Color3(0.0f,0.0f,0.0f);
				GL.Vertex2(0.0f,0.0f);
				GL.Vertex2(this.wwidth,0.0f);
				GL.Vertex2(0.0f,height);
				GL.Vertex2(this.wwidth,height);
				
				GL.Vertex2(0.0f,this.wheight-height);
				GL.Vertex2(this.wwidth,this.wheight-height);
				GL.Vertex2(0.0f,this.wheight);
				GL.Vertex2(this.wwidth,this.wheight);//*/
				
				GL.Color3(0.0f, 0.5f, 1.0f);
				for(int i = 0x00; i < this.text.Length; i++) {
					GL.RasterPos2(10.0f, this.wheight-height+OpenGLFont.FontHeight*i);
					OpenGLFont.PrintString(this.text[i]);
				}
				GL.Color3(1.0f, 1.0f, 1.0f);
			}
		}
		private void recalcText () {
			Queue<string> txt = new Queue<string>();
			OpenGLFont.splitWidth(this.fulltext, this.wwidth-150.0f, txt);
			this.text = txt.ToArray();
		}
		public void OnResize (int width, int height) {
			this.wwidth = width;
			this.wheight = height;
			this.recalcText();
		}
		
	}
	
}
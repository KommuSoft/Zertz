using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Zertz.Rendering {
	
	public class MessageBoard : IRenderable {
		
		public const uint DEFAULT_MESSAGE_LIFETIME = 5000;
		
		private readonly Queue<Message> queue = new Queue<Message>();
		private uint messageLifeTime = DEFAULT_MESSAGE_LIFETIME;
		
		public uint MessageLifeTime {
			get {
				return this.messageLifeTime;
			}
			set {
				this.messageLifeTime = value;
			}
		}
		
		public MessageBoard () {
		}
		
		public void Render (OpenTK.FrameEventArgs e) {
			GL.Color3(1.0f,0.0f,0.0f);
			int y = 0x00;
			DateTime now = DateTime.Now;
			int i = 0x00;
			lock(this.queue) {
				foreach(Message m in this.queue) {
					if(m.Released >= now) {
						GL.RasterPos2(0,y);
						m.Render(e);
						y += 0x20;
					}
					else {
						i++;
					}
				}
				while(i-- > 0x00) {
					queue.Dequeue();
				}
			}
		}
		
		public void PostMessage (IMessagePoster poster, string text) {
			Message m = new Message(poster,text,messageLifeTime);
			lock(this.queue) {
				this.queue.Enqueue(m);
			}
		}
		
		private struct Message : IRenderable {
			
			private readonly string text;
			private readonly DateTime posted;
			private readonly DateTime released;
			private readonly IMessagePoster poster;
			private string totalMessage;
			
			public string Text {
				get {
					return this.text;
				}
			}
			public DateTime Posted {
				get {
					return this.posted;
				}
			}
			public DateTime Released {
				get {
					return this.released;
				}
			}
			public IMessagePoster Poster {
				get {
					return this.poster;
				}
			}
			
			public Message (string text, uint milliseconds) : this(text,DateTime.Now,milliseconds) {}
			public Message (IMessagePoster poster, string text, uint milliseconds) : this(poster,text,DateTime.Now,milliseconds) {}
			public Message (string text, DateTime posted, uint milliseconds) : this(null,text,posted,milliseconds) {}
			public Message (IMessagePoster poster, string text, DateTime posted, uint milliseconds) {
				this.poster = poster;
				this.text = text;
				this.posted = posted;
				this.released = new DateTime(this.posted.Ticks).AddMilliseconds(milliseconds);
				this.totalMessage = string.Empty;
				this.totalMessage = generateTotalMessage();
			}
			
			private string generateTotalMessage () {
				return String.Format("[{0}{1}] {2}",((this.poster != null) ? this.poster.PosterName+"@" : string.Empty),this.posted.ToLongTimeString(),this.text);
			}
			
			public void Render (OpenTK.FrameEventArgs e) {
				OpenGLFont.PrintString(this.totalMessage);
			}
			
		}
		
	}
}
using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Zertz.Rendering {
	
	public class PopupBookItem : ITimeSensitive, IComparable<PopupBookItem>, IRenderable {
		
		private readonly float x, y, theta, width, height;
		private readonly int layer;
		private float angle, angleTarget;
		private const float ANGLE_SPEED = (float)(0.3d*Math.PI);
		private float[] textureMatrix;
		private readonly int glRef = -0x01, glN;
		
		public float X {
			get {
				return this.x;
			}
		}
		public float Y {
			get {
				return this.y;
			}
		}
		public float Theta {
			get {
				return this.theta;
			}
		}
		public float Width {
			get {
				return this.width;
			}
		}
		public float Height {
			get {
				return this.height;
			}
		}
		public float Angle {
			get {
				return this.angle;
			}
		}
		public float[] TextureMatrix {
			get {
				return this.textureMatrix;
			}
		}
		public bool DefaultRendering {
			get {
				return (this.glRef == -0x01);
			}
		}
		public int Layer {
			get {
				return this.layer;
			}
		}
		
		public PopupBookItem (float x, float y, float theta, float width, float height) : this(x,y,theta,width,height,0.5f*(float) Math.PI) {
		}
		public PopupBookItem (float x, float y, float theta, float width, float height, float angle) : this(x,y,theta,width,height,angle,angle) {
		}
		public PopupBookItem (float x, float y, float theta, float width, float height, float angle, float angleTarget) : this(x,y,theta,width,height,angle,angleTarget,0.0f,0.0f,1.0f,1.0f,0x00) {
		}
		public PopupBookItem (float x, float y, float theta, float width, float height, float angle, float angleTarget, float tx0, float ty0, float tx1, float ty1, int layer) {
			this.x = x;
			this.y = y;
			this.theta = theta;
			this.width = width;
			this.height = height;
			this.angle = angle;
			this.angleTarget = angleTarget;
			this.textureMatrix = new float[] {
				tx1-tx0,
				0.0f,
				0.0f,
				0.0f,
				0.0f,
				ty1-ty0,
				0.0f,
				0.0f,
				0.0f,
				0.0f,
				1.0f,
				0.0f,
				tx0,
				ty0,
				0.0f,
				1.0f
			};
			this.layer = layer;
		}
		public PopupBookItem (float x, float y, float theta, float width, float height, float t0x, float t0y, float t1x, float t1y, PointF[] points, int layer) : this(x,y,theta,width,height,0.5f*(float) Math.PI,0.5f*(float) Math.PI,t0x,t0y,t1x,t1y,layer) {
			this.glN = points.Length;
			if(this.glN > 0x00) {
				this.glRef = MeshBuilder.BuildPopupPolygon(points);
			}
			else {
				this.glRef = -0x01;
			}
		}
		
		public int CompareTo (PopupBookItem other) {
			return -this.Y.CompareTo(other.Y);
		}
		public void AdvanceTime (float time) {
			float d = this.angleTarget-this.angle;
			float s = Math.Sign(d);
			float r = s*Math.Min(Math.Abs(d), ANGLE_SPEED*time);
			this.angle += r;
		}
		public void ToUp () {
			this.angleTarget = 0.0f;
		}
		public void ToNear () {
			this.angleTarget = (float)(0.5d*Math.PI);
		}
		public void ToFar () {
			this.angleTarget = (float)(-0.5d*Math.PI);
		}
		public void ToAngle (float angle, bool immidiatly) {
			this.angleTarget = angle;
			if(immidiatly) {
				this.angle = angle;
			}
		}
		public void Render (FrameEventArgs e) {
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			int stride = 0x08*sizeof(float);
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.glRef);
			GL.VertexPointer(0x03, VertexPointerType.Float, stride, 0x00);
			GL.NormalPointer(NormalPointerType.Float, stride, 0x03*sizeof(float));
			GL.TexCoordPointer(0x02, TexCoordPointerType.Float, stride, 0x06*sizeof(float));
			GL.DrawArrays(BeginMode.Triangles, 0x00, this.glN);
			GL.DrawArrays(BeginMode.Triangles, this.glN, this.glN);
			GL.PopClientAttrib();
		}
		
	}
}


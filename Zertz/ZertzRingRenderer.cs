using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Zertz.Utils;
using Zertz.Zertz;

namespace Zertz.Rendering.Zertz {
	
	public class ZertzRingRenderer : MovableLocatableBase, IRenderable, ITimeSensitive, ISelectable {
		
		private static int numberOfInstances = 0x00;
		private static int xntBuff = 0x00, bufferN = 0x00, texBuff = 0x00;
		private static object lck = new object();
		
		public const float INNER_RADIUS = 0.08f;
		public const float OUTER_RADIUS = 0.32f;
		public const float THICKNESS = 0.025f;
		public const float SPACING = 0.05f;
		public const float TIME_FACTOR = 0.25f;
		public const float TIME_OFFSET = 2.0f;
		
		private Vector3 tt;
		private readonly HexLocation hexLocation;
		private readonly ZertzGame game;
		private Vector3 boardLocation;
		private Vector3 tileLocation;
		private RingLocationState locationState = RingLocationState.Pile;
		private float moveHeight;
		private Vector3 tileVector;
		private bool selected = false;
		private Vector3 colorv = new Vector3(1.0f,1.0f,1.0f);
		
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
					this.colorv = new Vector3(1.0f,1.0f,1.0f);
				}
			}
		}
		public Vector3 BoardLocation {
			get {
				return this.boardLocation;
			}
		}
		public Vector3 TileLocation {
			get {
				return this.tileLocation;
			}
			set {
				this.tileLocation = value;
			}
		}
		public HexLocation HexLocation {
			get {
				return this.hexLocation;
			}
		}
		public float MoveHeight {
			get {
				return this.moveHeight;
			}
			set {
				this.moveHeight = value;
			}
		}
		
		public ZertzRingRenderer (ZertzGame game, HexLocation hexLocation, Vector3 tileVector, Vector3 tileEscape, float time) {
			register();
			this.tt = new Vector3((float) UniversalRandom.NextDouble(),(float) UniversalRandom.NextDouble(),0.0f);
			this.hexLocation = hexLocation;
			this.boardLocation = Maths.HexVector(hexLocation,2.0f*OUTER_RADIUS+SPACING,0.5f*THICKNESS);
			this.tileVector = tileVector;
			this.RenderMover = RenderMoveManager.GenerateWaitMover(time,this.tileVector,RenderMoveManager.GenerateMoveMover(this.tileVector,tileEscape,2.0f,RenderMoveManager.GenerateHopMover(tileEscape,this.boardLocation,1.5f,null)));
			//this.RenderMover = RenderMoveManager.GenerateStaticMover(this.tileVector);
		}
		~ ZertzRingRenderer () {
			unregister();
		}
		
		private void register () {
			lock(lck) {
				if(numberOfInstances <= 0x00) {
					xntBuff = MeshBuilder.BuildRing(THICKNESS,INNER_RADIUS,OUTER_RADIUS,0x0c, out bufferN);
					Texture t = TextureFactory.Wood(512,512,3.0f/4.0f*0.618f);
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
					Unloader.DeleteBuffer(ref texBuff);
				}
			}
		}
		
		public static ZertzRingRenderer[] GenerateRings (RenderContainer rc, ZertzBoardRenderer board, ZertzGame game, ZertzCupRenderer cup, HexLocation[] hls, int offsetid) {
			int n = hls.Length;
			int id = offsetid;
			ZertzRingRenderer zrr;
			ZertzRingRenderer[] list = new ZertzRingRenderer[n];
			ZertzTileRenderer ztr;
			float time = n*TIME_FACTOR+TIME_OFFSET;
			for(int i = 0x00; i < n; i++) {
				ztr = cup.MinimalTile;
				zrr = new ZertzRingRenderer(game,hls[i],ztr.NextVector,ztr.TileEscapeLocation,time);
				board.PutRing(zrr,hls[i]);
				ztr.Add(zrr);
				list[i] = zrr;
				rc.Add(id++,zrr);
				time -= TIME_FACTOR;
			}
			cup.ClearTiles();
			return list;
		}
		public void AdvanceTime (float time) {
			this.MoveTime += time;
		}
		public void Render (FrameEventArgs e) {
			GL.PushAttrib(AttribMask.EnableBit);
			GL.PushMatrix();
			GL.Translate(this.RenderMover(this));
			GL.MatrixMode(MatrixMode.Texture);
			GL.PushMatrix();
			GL.Translate(tt);
			GL.Color3(this.colorv);
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D,texBuff);
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			//GL.EnableClientState(ArrayCap.IndexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,xntBuff);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer,idBuff);
			int stride = sizeof(float)<<0x03;
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.DrawArrays(BeginMode.QuadStrip,0x00,bufferN);
			//GL.DrawElements(BeginMode.Quads,bufferN,DrawElementsType.UnsignedInt,0x00);
			GL.Color3(1.0f,1.0f,1.0f);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();
			GL.PopClientAttrib();
			GL.PopAttrib();
		}
		
		private enum RingLocationState : byte {
			Pile	= 0x00,
			ToPile	= 0x01,
			ToBoard	= 0x02,
			Board	= 0x03
		}
		
	}
}
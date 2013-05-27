using System;
using System.Reflection;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using GTZ.Zertz;
using GTZ.Mathematics;

namespace GTZ.Rendering.Zertz {
	
	public class ZertzCupRenderer : IRenderable, ILoadable, ITimeSensitive {
		
		private int cupBuff, cupN;
		private float ballheight;
		private WaterSimulator ws;
		public const float HALF_WIDTH = 3.45f;
		public const float THICKNESS = 0.3f;
		public const float HEIGHT = 1.0f;
		public const float BORDER_HEIGHT = 0.2f;
		public const float CORNER_HEIGHT = 0.3f;
		public const int GRID_POINTS = 50;
		private ZertzTileRenderer ztra, ztrb;
		private ZertzContainerRenderer zcrc, zcrpa, zcrpb;
		
		public ZertzTileRenderer MinimalTile {
			get {
				if(this.ztra.NumberOfPieces <= this.ztrb.NumberOfPieces) {
					return this.ztra;
				}
				else {
					return this.ztrb;
				}
			}
		}
		public ZertzContainerRenderer this [ZertzBallContainerType type] {
			get {
				switch(type) {
					case ZertzBallContainerType.Common :
						return this.zcrc;
					case ZertzBallContainerType.Player1 :
						return this.zcrpa;
					case ZertzBallContainerType.Player2 :
						return this.zcrpb;
					default :
						return null;
				}
			}
		}
		public float BallHeight {
			get {
				return this.ballheight;
			}
		}
		public ZertzContainerRenderer CommonContainer {
			get {
				return this.zcrc;
			}
		}
		public ZertzContainerRenderer Player1Container {
			get {
				return this.zcrpa;
			}
		}
		public ZertzContainerRenderer Player2Container {
			get {
				return this.zcrpb;
			}
		}
		
		public ZertzCupRenderer (RenderContainer rc, int id) {
			this.ws = new WaterSimulator(GRID_POINTS,GRID_POINTS,2.0f*HALF_WIDTH/(GRID_POINTS-0x01),0.3f,0.05f,1.0f);//dz=0.12f
			float c = HALF_WIDTH+0.5f*THICKNESS;
			this.ztra = new ZertzTileRenderer(c,BORDER_HEIGHT+CORNER_HEIGHT,c);
			this.ztrb = new ZertzTileRenderer(-c,BORDER_HEIGHT+CORNER_HEIGHT,-c);
			float alt = (float) (0.5f*Math.Sqrt(5.0f)-0.5f)*ZertzTileRenderer.TILE_HEIGHT;
			float wid = (ZertzTileRenderer.TILE_HEIGHT-alt);
			LinuxFlag lfa = new LinuxFlag(8,8,wid,250.0f,0.98f,alt,ZertzTileRenderer.TILE_RADIUS);
			lfa.Location = new Vector3(c,BORDER_HEIGHT+CORNER_HEIGHT,-c);
			rc.Add(id,lfa);
			LinuxFlag lfb = new LinuxFlag(8,8,wid,250.0f,0.98f,alt,ZertzTileRenderer.TILE_RADIUS);
			lfb.Location = new Vector3(-c,BORDER_HEIGHT+CORNER_HEIGHT,c);
			rc.Add(id+0x01,lfb);
		}
		
		/*public void SetBallPositions (ZertzBallRenderer[] balls) {
			float z1 = HALF_WIDTH-2.0f*ZertzBallRenderer.RADIUS, x = HALF_WIDTH+0.5f*THICKNESS, y = this.ballheight, dz = 3.0f*ZertzBallRenderer.RADIUS;
			float z2 = -z1, z3 = z1;
			Vector3 v;
			foreach(ZertzBallRenderer zbr in balls) {
				switch(zbr.Type) {
					case ZertzBallType.White :
						v = new Vector3(-x,y,z1);
						z1 -= dz;
						break;
					case ZertzBallType.Gray :
						v = new Vector3(-x,y,z2);
						z2 += dz;
						break;
					case ZertzBallType.Black :
						v = new Vector3(x,y,z3);
						z3 -= dz;
						break;
					default :
						v = Vector3.Zero;
						break;
				}
				zbr.RenderMover = RenderMoveManager.GenerateStaticMover(v);
			}
		}*/
		/*public void SetRingPositions (ZertzRingRenderer[] rings) {
			int mask = ~0x01;
			float r = HALF_WIDTH+0.5f*THICKNESS, y0 = ZertzRingRenderer.THICKNESS+BORDER_HEIGHT+CORNER_HEIGHT;
			float moveHeight = BORDER_HEIGHT+CORNER_HEIGHT+ZertzTileRenderer.TILE_HEIGHT;
			for(int i = 0x00; i < rings.Length; i++) {
				rings[i].TileLocation = new Vector3(r,2.0f*ZertzRingRenderer.THICKNESS*(i&mask)+y0,r);
				rings[i].MoveHeight = moveHeight;
				r = -r;
			}
		}*/
		public void Render (OpenTK.FrameEventArgs e) {
			GL.Color3(0.4f,0.2f,0.0f);
			this.ztra.Render(e);
			this.ztrb.Render(e);
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer,cupBuff);
			int stride = sizeof(float)<<0x03;
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.DrawArrays(BeginMode.Quads,0x00,cupN);
			
			/*GL.PushAttrib(AttribMask.EnableBit);
			GL.Color3(1.0f,1.0f,1.0f);
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D,glT);
			GL.BindBuffer(BufferTarget.ArrayBuffer,glBuff);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,glIdBuff);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			GL.DrawElements(BeginMode.QuadStrip,glN,DrawElementsType.UnsignedInt,0x00);
			GL.PopAttrib();//*/
			
			GL.PopClientAttrib();
			GL.Color3(0.09f,0.275f,0.17f);
			//ws.Render(e);
		}
		public void AdvanceTime (float time) {
			ws.AdvanceTime(time);
		}
		public void ClearTiles () {
			this.ztra.Clear();
			this.ztrb.Clear();
		}
		public void OnLoad (EventArgs e) {
			this.cupBuff = MeshBuilder.BuildZertzCup(HALF_WIDTH,THICKNESS,HEIGHT,BORDER_HEIGHT,CORNER_HEIGHT, ZertzBallRenderer.RADIUS, out this.ballheight,out this.cupN);
			this.zcrc = new ZertzContainerRenderer(ZertzBallContainerType.Common,HALF_WIDTH+0.5f*THICKNESS,HALF_WIDTH,this.ballheight);
			this.zcrpa = new ZertzContainerRenderer(ZertzBallContainerType.Player1,HALF_WIDTH+0.5f*THICKNESS,HALF_WIDTH,this.ballheight);
			this.zcrpb = new ZertzContainerRenderer(ZertzBallContainerType.Player2,HALF_WIDTH+0.5f*THICKNESS,HALF_WIDTH,this.ballheight);
			/*this.glBuff = MeshBuilder.GenerateHeightMap(new Texture(Assembly.GetExecutingAssembly().GetManifestResourceStream("GTZ.resources.EarthElevation_2048x1024.jpg")),HALF_WIDTH/1024.0f,0.25f,0x02,out this.glIdBuff, out this.glN);
			this.glT = new Texture(Assembly.GetExecutingAssembly().GetManifestResourceStream("GTZ.resources.EarthMap_2048x1024.jpg")).GenerateOpenGLBuffer();//*/
			ws.OnLoad(e);
		}
		
	}
	
}
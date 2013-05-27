using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Zertz.Utils;

namespace Zertz.Rendering.Zertz {
	
	public class ZertzTileRenderer : IRenderable {
	
		private readonly List<ZertzRingRenderer> rings = new List<ZertzRingRenderer>();
		private readonly Vector3 v;
		private static int numberOfInstances = 0x00;
		private static object lck = new object();
		private static int dataBuff, dataN;
		private Vector3 offset;
		public const float TILE_RADIUS = 0.05f;
		public const float TILE_HEIGHT = 2.0f;
		public const int TILE_SEGMENTS = 0x08;
		
		public int NumberOfPieces {
			get {
				return this.rings.Count;
			}
		}
		public Vector3 NextVector {
			get {
				return this.offset;
			}
		}
		public Vector3 TileEscapeLocation {
			get {
				return new Vector3(v.X,v.Y+0.5f*TILE_HEIGHT,v.Z);
			}
		}
		
		public ZertzTileRenderer (float x, float y, float z) {
			register();
			this.offset = new Vector3(x,y+ZertzRingRenderer.THICKNESS,z);
			this.v = new Vector3(x,y+0.5f*TILE_HEIGHT,z);
		}
		~ ZertzTileRenderer () {
			unregister();
		}
		
		private void register () {
			lock(lck) {
				if(numberOfInstances <= 0x00) {
					dataBuff = MeshBuilder.GenerateCilinder(TILE_HEIGHT,TILE_RADIUS,TILE_SEGMENTS,out dataN);
				}
				numberOfInstances++;
			}
		}
		private void unregister () {
			lock(lck) {
				numberOfInstances--;
				if(numberOfInstances <= 0x00) {
					Unloader.DeleteBuffer(ref dataBuff);
				}
			}
		}
		public void Add (ZertzRingRenderer ring) {
			this.rings.Add(ring);
			this.offset.Y += 4.0f*ZertzRingRenderer.THICKNESS;
		}
		public void Clear () {
			this.offset.Y -= 4.0f*ZertzRingRenderer.THICKNESS*this.rings.Count;
			this.rings.Clear();
		}
		public void Remove (ZertzRingRenderer ring) {
			this.rings.Remove(ring);
		}
		public void Render (OpenTK.FrameEventArgs e) {
			//GL.PushAttrib
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.PushMatrix();
			GL.Translate(v);
			GL.BindBuffer(BufferTarget.ArrayBuffer,dataBuff);
			int stride = sizeof(float)<<0x03;
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.DrawArrays(BeginMode.QuadStrip,0x00,dataN);
			GL.PopMatrix();
			GL.PopClientAttrib();
		}
		
	}
	
}
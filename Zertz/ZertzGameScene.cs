using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using Zertz.Zertz;
using Zertz.Utils;
using Zertz.Rendering.Zertz;

namespace Zertz.Rendering {
	
	public class ZertzGameScene : SceneBase, IZertzActionHandler, IMessagePoster {
		
		private int idxBuff, vrxBuff, texBuffA, texBuffB, sphBuff, sphN;
		private readonly ZertzGame game;
		private HexLocation[] hls;
		private readonly ZertzCupRenderer zcr;
		private RenderContainer rc = new RenderContainer();
		private readonly ZertzBoardRenderer boardR;
		private float alpha = 0.0f;
		private readonly TurnIndicator ti = new TurnIndicator(800,64);
		private readonly StateIndicator si;
		private Vector3 tiv = Vector3.Zero;
		private Vector3 siv = Vector3.Zero;
		private SceneActionState state = SceneActionState.Select;
		
		public string PosterName {
			get {
				return "Monitor";
			}
		}
		public OpenTK.Graphics.Color4 PosterColor {
			get {
				return new OpenTK.Graphics.Color4(1.0f,0.0f,0.0f,1.0f);
			}
		}
		
		public ZertzGameScene () {
			this.zcr = new ZertzCupRenderer(rc,0x100);
			this.game = new ZertzGame(out hls);
			this.game.RegisterActionHandler(this);
			this.boardR = new ZertzBoardRenderer(this.game.Board.Width,this.game.Board.Height);
			si = new StateIndicator(800,200,16.0f,this.game.Board.VisualRepresentation,this.game.Board.TextureBounds,this.game.Board.VisualFactor,new PointF[] {new PointF(1.0f/12.0f,0.5f),new PointF(2.0f/12.0f,0.33f),new PointF(2.0f/12.0f,0.67f),new PointF(3.0f/12.0f,0.33f),new PointF(3.0f/12.0f,0.67f),new PointF(4.0f/12.0f,0.5f),new PointF(5.0f/12.0f,0.5f)});
		}
		
		public override void AdvanceTime (float time) {
			alpha += 0.0014f*(2.0f*(float) UniversalRandom.NextDouble()-1.0f);
			Vector3 wind = new Vector3(1.530f*(float) Math.Cos(alpha),1.530f,1.530f*(float) Math.Sin(alpha));//
			Vector3 g = new Vector3(0.0f,-0.9f,0.0306f);
			this.zcr.AdvanceTime(time);
			this.rc.AdvanceTime(time,g,wind);
			this.ti.AdvanceTime(time);
		}
		
		public override void Render (FrameEventArgs e) {//TODO: clean up (unused buffers)
			GL.PushAttrib(AttribMask.EnableBit);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			//GL.Translate(0.0f,5.0f,-13.0f);
			//GL.Rotate(45.0f,1.0f,0.0f,0.0f);
			this.Camera.Render(e);
			//GL.PushMatrix();
			//GL.Color3(0.66f,0.66f,0.66f);
			//GL.Translate(0.0f,0.2f,5.0f);//dy = 0.075f
			//GL.Rotate(-90.0f*time,0.5f,0.3f,0.2f);
			//GL.Translate(0.0f,-7.0f,Math.Min(2.0f,0.5f*time)-2.0f);
			this.rc.Render(e);
			this.zcr.Render(e);
			int stride = sizeof(float)<<0x03;
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.EnableClientState(ArrayCap.IndexArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.Color3(1.0f,1.0f,1.0f);
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D,this.texBuffA);
			//GL.Scale(0.6f,0.6f,0.6f);
			GL.BindBuffer(BufferTarget.ArrayBuffer,vrxBuff);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,idxBuff);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			//GL.DrawElements(BeginMode.Quads,512,DrawElementsType.UnsignedInt,0x00);
			//zb.Render(e);
			GL.Translate(0.0f,(float) Math.Sqrt(0.15f),0.0f);
			GL.BindTexture(TextureTarget.Texture2D,this.texBuffB);
			GL.BindBuffer(BufferTarget.ArrayBuffer,sphBuff);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer,idxBuff);
			GL.VertexPointer(0x03,VertexPointerType.Float,stride,0x00);
			GL.NormalPointer(NormalPointerType.Float,stride,0x03*sizeof(float));
			GL.TexCoordPointer(0x02,TexCoordPointerType.Float,stride,0x06*sizeof(float));
			//GL.DrawArrays(BeginMode.Triangles,0x00,sphN);
			GL.PopClientAttrib();
			//GL.PopMatrix();
			GL.PopAttrib();
		}
		public override void RenderDashboard (FrameEventArgs e) {
			GL.PushMatrix();
			GL.Translate(tiv);
			this.ti.Render(e);
			GL.PopMatrix();
			GL.PushMatrix();
			GL.Translate(siv);
			this.si.Render(e);
			GL.PopMatrix();
		}
		public override void SelectionRender (FrameEventArgs e) {
			GL.PushAttrib(AttribMask.EnableBit);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			this.Camera.Render(e);
			//GL.Translate(0.0f,0.2f,5.0f);
			this.rc.Render(e);
			GL.PopAttrib();
		}
		public override void OnLoad (EventArgs e) {
			this.ti.OnLoad(e);
			this.si.CurrentState = 0x00;
			this.si.OnLoad(e);
			this.zcr.OnLoad(e);
			ZertzBallRenderer.GenerateBalls(ZertzBallContainer.Offset(),this.rc,this.zcr,0x00);
			ZertzRingRenderer.GenerateRings(this.rc,this.boardR,this.game,this.zcr,this.hls,0x40);
			this.OnResize(e);
			//this.rc.Add(0x100,);
		}
		public override void OnUnload (EventArgs e) {
			GL.DeleteBuffers(0x01,ref this.idxBuff);
			GL.DeleteBuffers(0x01,ref this.vrxBuff);
		}
		public override void OnResize (EventArgs e) {
			this.ti.Width = Math.Max(this.MainWindow.Width>>0x01,ti.Height<<0x03);
			tiv.X = (this.MainWindow.Width-this.ti.Width)/0x02;
			siv.Y = this.MainWindow.Height-si.Height;
			siv.X = (this.MainWindow.Width-this.si.Width)/0x02;
		}
		public override bool HandleKeyDown (Key key) {
			switch(key) {
				case Key.Plus :
					this.rc.SelectNext();
					break;
				case Key.Minus :
					this.rc.SelectPrevious();
					break;
				case Key.Enter :
					this.SelectedItem(this.rc.Selected);
					break;
				case Key.Escape :
					Environment.Exit(0x00);
					break;
				default :
					return false;
			}
			return true;
		}
		public override void MouseMoveSelectionChanged (int oldsel, int sel) {
			this.rc.SelectName(sel);
		}
		public override void ClickedItem (int sel) {
			SelectedItem((ISelectable) this.rc[sel]);
		}
		public void SelectedItem (ISelectable sel) {
			if(sel == null) {
				return;
			}
			try {
				if(sel is ZertzBallRenderer) {
					ZertzBallRenderer zbr = (ZertzBallRenderer) sel;
					if(zbr.Container != ZertzBallContainerType.None) {
						this.game.TakeBall(zbr.Type);
					}
				}
				else if(sel is ZertzRingRenderer) {
					ZertzRingRenderer zrr = (ZertzRingRenderer) sel;
					this.game.SelectPiece(zrr.HexLocation);
				}
				else if(sel is LinuxFlag) {
					if(this.state == SceneActionState.Select) {
						this.state = SceneActionState.Caputure;
					}
					else {
						this.game.EndTurn();
					}
				}
			}
			catch(Exception e) {
				Console.WriteLine(e);
				this.MessageBoard.PostMessage(this,e.Message);
			}
		}
		
		#region ACTION_LISTENERS
		public void PutBall (ZertzBallContainerType containertype, ZertzBallType type, HexLocation location) {
			ZertzContainerRenderer zbcr = this.zcr[containertype];
			ZertzBallRenderer zbr = zbcr.GetBallOfType(type);
			zbcr.Remove(zbr);
			this.boardR.PutBall(zbr,location);
			zbr.RenderMover = RenderMoveManager.GenerateHopMover(zbr.Location,Maths.HexVector(location,2.0f*ZertzRingRenderer.OUTER_RADIUS+ZertzRingRenderer.SPACING,0.5f*ZertzRingRenderer.THICKNESS),1.5f,null);
		}
		public void CaptureBall (HexLocation location, ZertzBallContainerType type) {
			
		}
		public void PerformHop (HexLocation fr, HexLocation to) {
			ZertzBallRenderer ball = this.boardR.RemoveBall(fr);
			this.boardR.PutBall(ball,to);
			Vector3 la = ball.Location;
			Vector3 lc = Maths.HexVector(to,2.0f*ZertzRingRenderer.OUTER_RADIUS+ZertzRingRenderer.SPACING,0.5f*ZertzRingRenderer.THICKNESS);
			Vector3 lb = 0.5f*(la+lc);
			lb.Y += 0.5f;
			ball.RenderMover = RenderMoveManager.GenerateHopMover(la,lb,lc,0.5f,null);
		}
		public void ChangePlayer (int newPlayer) {
			this.ti.Turn = (byte) (newPlayer+0x01);
			this.state = SceneActionState.Select;
			//this.Camera.PositionTarget = new Vector3(0.0f,-5.0f,17.0f*newPlayer-8.5f);
			//this.Camera.PhiTarget = 180.0f*newPlayer;
			this.Camera.LoadSavedCameraPosition(newPlayer);
		}
		public void RemovePiece (HexLocation location) {
			ZertzRingRenderer zrr = this.boardR.RemoveRing(location);
			Vector3 via = this.zcr.MinimalTile.TileEscapeLocation;
			Vector3 des = this.zcr.MinimalTile.NextVector;
			this.zcr.MinimalTile.Add(zrr);
			zrr.RenderMover = RenderMoveManager.GenerateReverseHopMover(zrr.Location,via,1.0f,RenderMoveManager.GenerateMoveMover(via,des,2.0f,null));//,RenderMoveManager.GenerateStaticMover(des)
		}
		public void ChangeMoveState (int newState) {
			this.si.CurrentState = newState;
			this.game.Board.Repaint();
			this.si.RefreshVisual();
		}
		#endregion
		
		private enum SceneActionState : byte {
			Select		= 0x00,
			Caputure	= 0x01
		}
		
	}
	
}
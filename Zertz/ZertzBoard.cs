using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL;
using Zertz.EgyptStyle;
using Zertz.Utils;

namespace Zertz.Zertz {
	
	public class ZertzBoard {
	
		private readonly ZertzPiece[,] pieces;
		private readonly Bitmap visual;
		private const float vr_m = 2.5f;//margin
		private const float vr_r = 5.0f;//small radius
		private const float vr_R = 15.0f;//large radius
		private const float vr_rb = 10.0f;//ball radius
		private const float vr_s = 2.5f;//spacing
		private readonly RectangleF textureBounds;
		private readonly float visualFactor;
		
		public ZertzPiece this [HexLocation zl] {
			get {
				if(zl.X < 0x00 || zl.Y < 0x00 || zl.X >= this.pieces.GetLength(0x00) || zl.Y >= this.pieces.GetLength(0x01)) {
					return ZertzPiece.Dead;
				}
				else {
					return this.pieces[zl.X,zl.Y];
				}
			}
			internal set {
				if(zl.X < 0x00 || zl.Y < 0x00 || zl.X >= this.pieces.GetLength(0x00) || zl.Y >= this.pieces.GetLength(0x01)) {
					throw new InvalidZertzActionException("Unable to set a piece out of range!");
				}
				else {
					this.pieces[zl.X,zl.Y] = value;
				}
			}
		}
		public ZertzPiece this [int x, int y] {
			get {
				if(x < 0x00 || y < 0x00 || x >= this.pieces.GetLength(0x00) || y >= this.pieces.GetLength(0x01)) {
					return ZertzPiece.Dead;
				}
				else {
					return this.pieces[x,y];
				}
			}
			internal set {
				if(x < 0x00 || y < 0x00 || x >= this.pieces.GetLength(0x00) || y >= this.pieces.GetLength(0x01)) {
					throw new InvalidZertzActionException("Unable to set a piece out of range!");
				}
				else {
					this.pieces[x,y] = value;
				}
			}
		}
		public int Width {
			get {
				return this.pieces.GetLength(0x00);
			}
		}
		public int Height {
			get {
				return this.pieces.GetLength(0x01);
			}
		}
		public Bitmap VisualRepresentation {
			get {
				return this.visual;
			}
		}
		public RectangleF TextureBounds {
			get {
				return this.textureBounds;
			}
		}
		public float VisualFactor {
			get {
				return this.visualFactor;
			}
		}
		
		private ZertzBoard (ZertzPiece[,] pieces) {
			this.pieces = pieces;
			int wo = (int) Math.Ceiling(2.0f*vr_m+2.0f*7.0f*vr_R+6.0f*vr_s);
			int ho = (int) Math.Ceiling(2.0f*vr_m+2.0f*vr_R+(2.0f*6.0f*vr_R+6.0f*vr_s)*Maths.Sqrt3_4);
			int w = Maths.ToNextPow2(wo);
			int h = Maths.ToNextPow2(ho);
			float wr = (float) wo/w;
			float hr = (float) ho/h;
			this.visualFactor = (float) wo/ho;
			this.textureBounds = new RectangleF(0.5f-0.5f*wr,0.5f-0.5f*hr,wr,hr);
			this.visual = new Bitmap(w,h);
			this.Repaint();
		}
		
		public void Repaint () {
			float R2pS = 2.0f*vr_R+vr_s;
			
			float xo = 0.5f*visual.Width;
			float yo = 0.5f*visual.Height-3.0f*Maths.Sqrt3_4*R2pS;
			GraphicsPath gpRing = new GraphicsPath();
			gpRing.AddEllipse(-vr_R+xo,-vr_R+yo,2.0f*vr_R,2.0f*vr_R);
			gpRing.AddEllipse(-vr_r+xo,-vr_r+yo,2.0f*vr_r,2.0f*vr_r);
			GraphicsPath gpBall = new GraphicsPath();
			gpBall.AddEllipse(-vr_rb+xo,-vr_rb+yo,2.0f*vr_rb,2.0f*vr_rb);
			float dx;
			int k;
			Matrix M = new Matrix();
			Matrix My = new Matrix();
			My.Translate(0.0f,Maths.Sqrt3_4*R2pS);
			ZertzPiece zp;
			Brush[] bs = new Brush[]{EgyptInformation.Brushes.EgyptNocturne,EgyptInformation.Brushes.EgyptPaintYellow,EgyptInformation.Brushes.EgyptPaintWhite};
			using(Graphics g = Graphics.FromImage(this.visual)) {
				g.Clear(Color.Transparent);
				for(int j = 0x00; j < 0x07; j++) {
					for(int i = 0x00; i < 0x07; i++) {
						zp = this.pieces[i,j];
						k = zp.State;
						if(k > 0x00) {
							dx = R2pS*((i-0x03)+0.5f*(j-0x03));
							M.Reset();
							M.Translate(dx,0.0f);
							gpRing.Transform(M);
							g.FillPath(EgyptInformation.Brushes.EgyptPaintRed,gpRing);
							g.DrawPath(Pens.Black,gpRing);
							if(zp.ContainsBall) {
								gpBall.Transform(M);
								g.FillPath(bs[(int) zp.BallType],gpBall);
								g.DrawPath(Pens.Black,gpBall);
								M.Reset();
								M.Translate(-dx,0.0f);
								gpBall.Transform(M);
								gpRing.Transform(M);
							}
							else {
								M.Reset();
								M.Translate(-dx,0.0f);
								gpRing.Transform(M);
							}
						}
					}
					gpRing.Transform(My);
					gpBall.Transform(My);
				}
			}
		}
		public bool HasNeighbours (HexLocation zl) {
			foreach(HexLocation dir in HexLocation.NeighbourDirections) {
				if(this[zl+dir].IsAlive) {
					return true;
				}
			}
			return false;
		}
		#region STATE_CONDITIONS
		public bool CanCapture (HexLocation hl) {
			ZertzPiece zp = this[hl];
			if(!zp.IsAlive || !zp.ContainsBall) {
				return false;
			}
			HashSet<HexLocation> visited = new HashSet<HexLocation>();
			Stack<HexLocation> todo = new Stack<HexLocation>();
			visited.Add(hl);
			todo.Push(hl);
			HexLocation hla, nei;
			while(todo.Count > 0x00) {
				hla = todo.Pop();
				for(int i = 0x00; i < 0x06; i++) {
					nei = hla+HexLocation.NeighbourDirections[i];
					zp = this[nei];
					if(zp.IsAlive) {
						if(zp.ContainsBall) {
							if(!visited.Contains(nei)) {
								visited.Add(nei);
								todo.Push(nei);
							}
						}
						else {
							return false;
						}
					}
				}
			}
			int nIsland = visited.Count;
			int nCurrent;
			int m = this.pieces.GetLength(0x00);
			int n = this.pieces.GetLength(0x01);
			bool unique = true;
			//not the main part (or it is the only island!)
			for(sbyte i = 0x00; i < m; i++) {
				for(sbyte j = 0x00; j < n; j++) {
					hla = new HexLocation(i,j);
					zp = this[hla];
					if(zp.IsAlive && !visited.Contains(hla)) {
						//we discovered a new island
						unique = false;
						nCurrent = 0x01;
						visited.Add(hla);
						todo.Push(hla);
						while(todo.Count > 0x00) {
							hla = todo.Pop();
							for(int k = 0x00; k < 0x06; k++) {
								nei = hla+HexLocation.NeighbourDirections[k];
								zp = this[nei];
								if(zp.IsAlive) {
									if(!visited.Contains(nei)) {
										visited.Add(nei);
										todo.Push(nei);
										nCurrent++;
										if(nCurrent >= nIsland) {
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
			return unique;
		}
		public bool CanHop () {//CH1
			int m = this.pieces.GetLength(0x00);
			int n = this.pieces.GetLength(0x01);
			for(sbyte i = 0x00; i < m; i++) {
				for(sbyte j = 0x00; j < n; j++) {
					if(CanHop(new HexLocation(i,j))) {
						return true;
					}
				}
			}
			return false;
		}
		public bool CanHop (HexLocation hl) {//CH2
			ZertzPiece zp = this[hl];
			if(!zp.IsAlive || !zp.ContainsBall) {
				return false;
			}
			HexLocation zla, dir;
			for(int i = 0x00; i < 0x06; i++) {
				dir = HexLocation.NeighbourDirections[i];
				zla = hl+dir;
				zp = this[zla];
				if(zp.IsAlive && zp.ContainsBall) {
					zla += dir;
					zp = this[zla];
					if(zp.IsAlive && !zp.ContainsBall) {
						return true;
					}
				}
			}
			return false;
		}
		public bool CanHop (HexLocation zl, HexDirection dir) {//CH3
			HexLocation zla = zl;
			ZertzPiece zp = this[zla];
			HexLocation hldir = HexLocation.NeighbourDirections[(byte) dir];
			if(zp.IsAlive && zp.ContainsBall) {
				zla += hldir;
				zp = this[zla];
				if(zp.IsAlive && zp.ContainsBall) {
					zla += hldir;
					zp = this[zla];
					if(zp.IsAlive && !zp.ContainsBall) {
						return true;
					}
				}
			}
			return false;
		}
		public bool HasFree () {//HF
			int m = this.pieces.GetLength(0x00);
			int n = this.pieces.GetLength(0x01);
			for(sbyte i = 0x00; i < m; i++) {
				for(sbyte j = 0x00; j < n; j++) {
					if(this.IsFree(new HexLocation(i,j))) {
						return true;
					}
				}
			}
			return false;
		}
		public bool HasVacant () {//HV
			int m = this.pieces.GetLength(0x00);
			int n = this.pieces.GetLength(0x01);
			for(int i = 0x00; i < m; i++) {
				for(int j = 0x00; j < n; j++) {
					if(this[i,j].IsVacant) {
						return true;
					}
				}
			}
			return false;
		}
		public bool IsFree (HexLocation hl) {//IF
			ZertzPiece zp = this[hl];
			if(zp.IsAlive && !zp.ContainsBall) {
				HexLocation zla, zlb;
				zlb = HexLocation.NeighbourDirections[0x05]+hl;
				for(int i = 0x00; i < 0x06; i++) {
					zla = zlb;
					zlb = HexLocation.NeighbourDirections[i];
					if(!this[zla].IsAlive && !this[hl+zlb].IsAlive && !this[zla+zlb].IsAlive) {
						return true;
					}
					zlb += hl;
				}
			}
			return false;
		}
		public bool IsVacant (HexLocation hl) {//IV
			return this[hl].IsVacant;
		}
		#endregion
		#region ACTION_COMMANDS
		public ZertzBallType DoHopMove (HexLocation zl, HexDirection dir) {
			HexLocation hldir = HexLocation.NeighbourDirections[(byte) dir];
			HexLocation zla = zl, zlb = zla+hldir, zlc = zlb+hldir;
			//if(this[zla].CanDropBall() && this[zlb].CanDropBall() && this[zlc].CanPutBall()) {
			ZertzPiece zpa = this[zla];
			ZertzPiece zpb = this[zlb];
			ZertzPiece zpc = this[zlc];
			zpc.PutBall(zpa.DropBall());
			ZertzBallType t = zpb.DropBall();
			this[zla] = zpa;
			this[zlb] = zpb;
			this[zlc] = zpc;
			return t;
			//}
			//throw new InvalidZertzActionException(String.Format("Can't perform this hop operation: [{0}:{1}]",zl,dir));
		}
		public ZertzBallContainer CaptureLandPieces (HexLocation hl) {//CLP
			HashSet<HexLocation> visited = new HashSet<HexLocation>();
			Stack<HexLocation> todo = new Stack<HexLocation>();
			ZertzBallContainer zbc = ZertzBallContainer.Empty();
			ZertzPiece zp;
			zbc[this[hl].BallType]++;
			this[hl] = ZertzPiece.Dead;
			visited.Add(hl);
			todo.Push(hl);
			HexLocation hla, nei;
			while(todo.Count > 0x00) {
				hla = todo.Pop();
				for(int i = 0x00; i < 0x06; i++) {
					nei = hla+HexLocation.NeighbourDirections[i];
					if(!visited.Contains(nei)) {
						visited.Add(nei);
						zp = this[nei];
						if(zp.IsAlive) {
							zbc[zp.BallType]++;
							this[nei] = ZertzPiece.Dead;
							todo.Push(nei);
						}
					}
				}
			}
			return zbc;
		}
		public void PutZertzBall (HexLocation hl, ZertzBallType type) {//PZB
			ZertzPiece piece = ZertzPiece.Vacant;
			piece.PutBall(type);
			this[hl] = piece;
		}
		public void RemoveZertzPiece (HexLocation hl) {//RZP
			this[hl] = ZertzPiece.Dead;
		}
		#endregion
		public static ZertzBoard OffsetBoard (out HexLocation[] hls) {
			ZertzPiece[,] pieces = new ZertzPiece[0x07,0x07];
			hls = new HexLocation[0x25];
			int o1, o2, j, k = 0x00;
			for(int i = 0x00; i <= 0x06; i++) {
				o1 = Math.Max(0x00,0x03-i);
				o2 = Math.Min(0x06,0x09-i);
				for(j = 0x00; j < o1; j++) {
					pieces[i,j] = ZertzPiece.Dead;
				}
				for(; j <= o2; j++) {
					pieces[i,j] = ZertzPiece.Vacant;
					hls[k++] = new HexLocation((sbyte) i,(sbyte) j);
				}
				for(; j <= 0x06; j++) {
					pieces[i,j] = ZertzPiece.Dead;
				}
			}
			return new ZertzBoard(pieces);
		}
		public ZertzBoard Clone () {
			return new ZertzBoard((ZertzPiece[,]) this.pieces.Clone());
		}
		
	}
	
}
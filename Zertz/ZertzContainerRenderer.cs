using System;
using System.Collections.Generic;
using OpenTK;
using Zertz.Zertz;

namespace Zertz.Rendering.Zertz {
	
	public class ZertzContainerRenderer {
		
		private readonly List<ZertzBallRenderer> balls = new List<ZertzBallRenderer>();
		private int white = 0x00, gray = 0x00, black = 0x00;
		private readonly ZertzBallContainerType type;
		private readonly float row;
		private readonly float offset;
		private readonly float ballHeight;
		
		public ZertzContainerRenderer (ZertzBallContainerType type, float row, float offset, float ballHeight) {
			this.type = type;
			this.row = row;
			this.offset = offset;
			this.ballHeight = ballHeight;
		}
		
		
		public Vector3 GetNextPosition (ZertzBallType zbt) {
			float dz = 3.0f*ZertzBallRenderer.RADIUS;
			float o = this.offset-4.5f*ZertzBallRenderer.RADIUS;
			if(this.type == ZertzBallContainerType.Common) {
				switch(zbt) {
					case ZertzBallType.Black :
						return new Vector3(row,ballHeight,o-black*dz);
					case ZertzBallType.Gray :
						return new Vector3(-row,ballHeight,o-gray*dz);
					case ZertzBallType.White :
						return new Vector3(-row,ballHeight,white*dz-o);
					default :
						return Vector3.Zero;
				}
			}
			else {
				int factor = 0x03-0x02*(byte) this.type;
				return new Vector3((offset-(balls.Count+1.5f)*3.0f*ZertzBallRenderer.RADIUS)*factor,ballHeight,factor*row);
			}
		}
		public Vector3 Add (ZertzBallRenderer zbr) {
			lock(this.balls) {
				Vector3 v = GetNextPosition(zbr.Type);
				switch(zbr.Type) {
					case ZertzBallType.Black :
						this.black++;
						break;
					case ZertzBallType.Gray :
						this.gray++;
						break;
					case ZertzBallType.White :
						this.white++;
						break;
				}
				this.balls.Add(zbr);
				return v;
			}
		}
		public void Remove (ZertzBallRenderer zbr) {
			lock(this.balls) {
				zbr.Container = ZertzBallContainerType.None;
				switch(zbr.Type) {
					case ZertzBallType.Black :
						this.black--;
						break;
					case ZertzBallType.Gray :
						this.gray--;
						break;
					case ZertzBallType.White :
						this.white--;
						break;
				}
				this.balls.Remove(zbr);
			}
		}
		public ZertzBallRenderer GetBallOfType (ZertzBallType type) {
			lock(this.balls) {
				for(int i = this.balls.Count-0x01; i >= 0x00; i--) {
					if(this.balls[i].Type == type) {
						return this.balls[i];
					}
				}
			}
			return null;
		}
		
	}
	
}
using System;
using OpenTK;

namespace GTZ.Rendering {
	
	public delegate Vector3 RenderMover (IRenderMoveable irm);
	
	public static class RenderMoveManager {
		
		public static RenderMover GenerateStaticMover (Vector3 pos) {
			return new StaticMover(pos).RenderMover;
		}
		public static RenderMover GenerateHopMover (Vector3 la, Vector3 lb, Vector3 lc, float time, RenderMover next) {
			float ti = 1.0f/time;
			float t2i2 = 2.0f*ti*ti;
			return new HopMover(	t2i2*(la.X+lc.X-2.0f*lb.X),
									t2i2*(la.Y+lc.Y-2.0f*lb.Y),
									t2i2*(la.Z+lc.Z-2.0f*lb.Z),
									ti*(4.0f*lb.X-3.0f*la.X-lc.X),
									ti*(4.0f*lb.Y-3.0f*la.Y-lc.Y),
									ti*(4.0f*lb.Z-3.0f*la.Z-lc.Z),
									la.X,
									la.Y,
									la.Z,
									time,
									getNext(next,lc)).RenderMover;
		}
		public static RenderMover GenerateHopMover (Vector3 la, Vector3 lb, float time, RenderMover next) {
			float t2i2 = 1.0f/time;
			t2i2 *= t2i2;
			return new HopMover(	t2i2*(lb.X-la.X),
									t2i2*(lb.Y-la.Y-time),
									t2i2*(lb.Z-la.Z),
									0.0f,
									1.0f,
									0.0f,
									la.X,
									la.Y,
									la.Z,
									time,
									getNext(next,lb)).RenderMover;
		}
		public static RenderMover GenerateReverseHopMover (Vector3 la, Vector3 lb, float time, RenderMover next) {
			float t2i2 = 1.0f/time;
			t2i2 *= t2i2;
			return new HopMover(	t2i2*(la.X-lb.X),
									t2i2*(la.Y-lb.Y-time),
									t2i2*(la.Z-lb.Z),
									0.0f,
									1.0f,
									0.0f,
									lb.X,
									lb.Y,
									lb.Z,
									time,
									getNext(next,lb),
									-1.0f,
									1.0f).RenderMover;
		}
		public static RenderMover GenerateMoveMover (Vector3 la, Vector3 lb, float speed, RenderMover next) {
			return new MoveMover(la.X,la.Y,la.Z,lb.X,lb.Y,lb.Z,speed/(lb-la).Length,getNext(next,lb)).RenderMover;
		}
		public static RenderMover GenerateWaitMover (float time, Vector3 loc, RenderMover next) {
			return new WaitMover(loc,getNext(next,loc),time).RenderMover;
		}
		public static RenderMover GenerateEventWaitMover (Vector3 loc, EventHandler e, RenderMover next) {
			return new EventWaitMover(loc,e,getNext(next,loc)).RenderMover;
		}
		private static RenderMover getNext (RenderMover next, Vector3 v) {
			if(next == null) {
				return new StaticMover(v).RenderMover;
			}
			else {
				return next;
			}
		}
		
		private interface IRenderMover {
			
			Vector3 RenderMover (IRenderMoveable irm);
			
		}
		
		private class EventWaitMover : IRenderMover {
			
			private bool waiting = true;
			private DateTime dt;
			private Vector3 loc;
			private RenderMover next;
			
			public EventWaitMover (Vector3 loc, EventHandler e, RenderMover next) {
				this.loc = loc;
				e += new EventHandler(event_activated);
				this.next = next;
			}
			
			private void event_activated (object s, EventArgs e) {
				dt = DateTime.Now;
				waiting = false;
			}
			public Vector3 RenderMover (IRenderMoveable irm) {
				if(waiting) {
					return this.loc;
				}
				else {
					irm.RenderMover = this.next;
					irm.MoveTime = (float) (DateTime.Now-this.dt).TotalSeconds;
					return this.next(irm);
				}
			}
			
		}
		private class StaticMover : IRenderMover {
			
			private readonly Vector3 v;
			
			public StaticMover (Vector3 v) {
				this.v = v;
			}
			
			public Vector3 RenderMover (IRenderMoveable irm) {
				return v;
			}
			
		}
		private class WaitMover : IRenderMover {
			
			private readonly RenderMover next;
			private readonly float time;
			private readonly Vector3 v;
			
			public WaitMover (Vector3 loc, RenderMover next, float time) {
				this.v = loc;
				this.time = time;
				this.next = next;
			}
			public Vector3 RenderMover (IRenderMoveable irm) {
				float t = irm.MoveTime;
				if(t < time) {
					return this.v;
				}
				else {
					irm.RenderMover = next;
					irm.MoveTime = t-time;
					return next(irm);
				}
			}
			
		}
		private class MoveMover : IRenderMover {
			
			private readonly float xa, ya, za, xb, yb, zb, factor;
			private readonly RenderMover next;
			
			public MoveMover (float xa, float ya, float za, float xb, float yb, float zb, float factor, RenderMover next) {
				this.xa = xa;
				this.ya = ya;
				this.za = za;
				this.xb = xb;
				this.yb = yb;
				this.zb = zb;
				this.factor = factor;
				this.next = next;
			}
			
			public Vector3 RenderMover (IRenderMoveable irm) {
				float t = this.factor*irm.MoveTime;
				if(t < 1.0f) {
					float ti = 1.0f-t;
					return new Vector3(xa*ti+xb*t,ya*ti+yb*t,za*ti+zb*t);
				}
				else {
					irm.RenderMover = this.next;
					//TODO: calculate time
					return this.next(irm);
				}
			}
			
		}
		private class HopMover : IRenderMover {
			
			private readonly float ax_2, ay_2, az_2, vx, vy, vz, x, y, z, dt;
			private readonly float ft, ot;
			private readonly RenderMover next;
			
			public HopMover (float ax_2, float ay_2, float az_2, float vx, float vy, float vz, float x, float y, float z, float dt, RenderMover next) {
				this.ax_2 = ax_2;
				this.ay_2 = ay_2;
				this.az_2 = az_2;
				this.vx = vx;
				this.vy = vy;
				this.vz = vz;
				this.x = x;
				this.y = y;
				this.z = z;
				this.dt = dt;
				this.ft = 1.0f;
				this.ot = 0.0f;
				this.next = next;
			}
			public HopMover (float ax_2, float ay_2, float az_2, float vx, float vy, float vz, float x, float y, float z, float dt, RenderMover next, float ft, float ot) : this(ax_2,ay_2,az_2,vx,vy,vz,x,y,z,dt,next) {
				this.ft = ft;
				this.ot = ot;
			}
			
			public Vector3 RenderMover (IRenderMoveable irm) {
				Vector3 v;
				float t = irm.MoveTime;
				if(t < this.dt) {
					t = ft*t+ot;
					float t2 = t*t;
					v = new Vector3(ax_2*t2+vx*t+x,ay_2*t2+vy*t+y,az_2*t2+vz*t+z);
				}
				else {
					irm.RenderMover = next;
					v = next(irm);
				}
				return v;
			}
			
		}
		
	}
	
}
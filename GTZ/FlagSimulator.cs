using System;
using OpenTK;

/*namespace GTZ.Rendering {
	
	public class FlagSimulator : ITimeSensitive {
		
		private readonly int m, n;
		private readonly float dxy, sqrt2dxy, k, g;
		private readonly Vector3[] xnv;
		
		public FlagSimulator (int m, int n, float dxy, float springConstant, float gravity) {
			this.m = m;
			this.n = n;
			this.dxy = dxy;
			this.k = springConstant;
			this.g = gravity;
			this.sqrt2dxy = (float) (Math.Sqrt(2.0d)*dxy);
			this.xnv = new Vector3[0x03*m*n];
			setup();
		}
		
		private void setup () {
			float x, y = 0.0f;
			int N = m*n;
			int i = 0x00, i2;
			for(; i < N; ) {
				i2 = i+m;
				for(; i < i2; i++) {
					xnv[i] = new Vector3(i*dxy,y,0.0f);
				}
				y += dxy;
			}
			for(i2 = N<<0x01; i < i2; i++) {
				xnv[i] = Vector3.UnitZ;
			}
			for(i2 += N; i < i2; i++) {
				xnv[i] = Vector3.Zero;
			}
		}
		
		public void AdvanceTime (float time) {
			int i = 0x00, i2;
			int ir = 0x01;
			int id = m;
			int idr = id+0x01;
			int K = N<<0x01;
			int j = K;
			int jr = j+0x01;
			int jd = j+m;
			int jdr = jd+0x01;
			int N = m*n;
			int Nh = N-m;
			float gt = time*this.g;
			float dr, dd, ddr;
			for(; i < Nh; ) {
				i2 = i+m-0x01;
				for(; i < i2; i++) {
					dr = (xnv[ir]-xnv[i]).Length-dxy;
					xnv[j] += new Vector3(0.0f,gt,0.0f);
					xnv[jr++] += new Vector3(0.0f,0.0f,0.0f);
					xnv[jd++] += new Vector3(0.0f,0.0f,0.0f);
					xnv[jdr++] += new Vector3(0.0f,0.0f,0.0f);
					ir++; id++; idr++;
				}
				i++; ir++; id++; idr++;
				j++; jr++; jd++; jdr++;
			}
			
			//apply velocity
			for(i = 0x00, j = K; i < N;) {
				xnv[i++] += time*xnv[j++];
			}
		}
		
	}
	
}*/
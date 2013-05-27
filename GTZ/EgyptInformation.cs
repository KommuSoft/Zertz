using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using OGL = OpenTK.Graphics.OpenGL;
using GTZ.Utils;

namespace GTZ.EgyptStyle {
	
	public static class EgyptInformation {
		
		public static class Brushes {
			
			public static readonly Brush EgyptGold = new SolidBrush(Colors.EgyptGold);
			public static readonly Brush EgyptPaintWhite = new SolidBrush(Colors.EgyptPaintWhite);
			public static readonly Brush EgyptPaintRed = new SolidBrush(Colors.EgyptPaintRed);
			public static readonly Brush EgyptPaintBlue = new SolidBrush(Colors.EgyptPaintBlue);
			public static readonly Brush EgyptPaintTurquoise = new SolidBrush(Colors.EgyptPaintTurquoise);
			public static readonly Brush EgyptPaintYellow = new SolidBrush(Colors.EgyptPaintYellow);
			public static readonly Brush EgyptNocturne = new SolidBrush(Colors.EgyptNocturne);
			public static readonly Brush EgyptLure = new SolidBrush(Colors.EgyptLure);
			
		}
		
		public static class Colors {
			
			public static readonly Color EgyptGold = Color.FromArgb(0xff,0xd7,0x00);
			public static readonly Color EgyptPaintWhite = Color.FromArgb(0xff,0xff,0xf0);//before: Aliceblue
			public static readonly Color EgyptPaintRed = Color.FromArgb(0xe3,0x26,0x36);//before: 0xff960018 , 0xffe23620
			public static readonly Color EgyptPaintBlue = Color.FromArgb(0x00,0x41,0x6a);//before: 0xff1668b4
			public static readonly Color EgyptPaintTurquoise = Color.FromArgb(0x40,0xe0,0xd0);
			public static readonly Color EgyptPaintYellow = Color.FromArgb(0xcc,0x77,0x22);
			public static readonly Color EgyptNocturne = Color.FromArgb(0x10,0x0f,0x0e);
			public static readonly Color EgyptLure = Color.FromArgb(0x20,0x1e,0x1c);
			public static readonly Color EgyptClay = Color.SandyBrown;
			
		}
		
		public static class Textures {
			
			private delegate Bitmap BitmapGenerator ();
			private static BitmapGenerator[] generators = new BitmapGenerator[] {texture0,texture1};
			private static Dictionary<int,ulong> hash = new Dictionary<int,ulong>();
			
			public static int GetTexture (int index) {
				lock(hash) {
					ulong val;
					if(hash.TryGetValue(index,out val)) {
						hash[index] += 0x0100000000;
					}
					else {
						val = (ulong) ((ulong) cacheTexture(generators[index]())|0x0100000000);
						hash.Add(index,val);
					}
					return (int) (val&0xffffffff);
				}
			}
			public static void UnregisterTexture (int index) {
				lock(hash) {
					ulong val;
					if(hash.TryGetValue(index,out val)) {
						if(val >= 0x0200000000) {
							hash[index] -= 0x0100000000;
						}
						else {
							OGL.GL.DeleteTexture((int) (val&0xffffffff));
						}
					}
				}
			}
			public static void UnloadAllTextures () {
				lock(hash) {
					foreach(ulong val in hash.Values) {
						OGL.GL.DeleteTexture((int) (val&0xffffffff));
					}
					hash.Clear();
				}
			}
			
			private static int cacheTexture (Bitmap bmp) {
				BitmapData bmd = bmp.LockBits(new Rectangle(0x00,0x00,bmp.Width,bmp.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				int texture = OGL.GL.GenTexture();
				OGL.GL.BindTexture(OGL.TextureTarget.Texture2D,texture);
				OGL.GL.TexParameter(OGL.TextureTarget.Texture2D, OGL.TextureParameterName.TextureMagFilter, (int)OGL.All.Linear);
				OGL.GL.TexParameter(OGL.TextureTarget.Texture2D, OGL.TextureParameterName.TextureMinFilter, (int)OGL.All.Linear);
				OGL.GL.TexImage2D(OGL.TextureTarget.Texture2D,0x00,OGL.PixelInternalFormat.Rgba,bmp.Width,bmp.Height,0x00,OGL.PixelFormat.Bgra,OGL.PixelType.UnsignedByte,bmd.Scan0);
				bmp.UnlockBits(bmd);
				return texture;
			}
			private static Bitmap texture0 () {
				int w = 0xc0;
				int wa = w/0x03;
				int h = 0x80;
				float m = 2.0f;
				float t = 3.0f;
				float mt = m+t;
				float t_2 = 0.5f*t;
				float R = w/3-m;
				float r = R-t;
				float s = r-t;
				float R2 = 2.0f*R;
				float r2 = 2.0f*r;
				Bitmap bmp = new Bitmap(0x04*w/0x03,0x02*h);
				Graphics g = Graphics.FromImage(bmp);
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				using(GraphicsPath gpLure = new GraphicsPath()) {
					gpLure.AddArc(m,m,R2,R2,180.0f,90.0f);
					gpLure.AddLine(m+R,m,w-m-R,m);
					gpLure.AddArc(w-m-R2,m,R2,R2,270.0f,90.0f);
					gpLure.AddLine(w-m,m+R,w-m,w-m-R);
					gpLure.AddArc(w-m-R2,w-m-R2,R2,R2,0.0f,90.0f);
					gpLure.AddLine(w-m-R,w-m,m+R,w-m);
					gpLure.AddArc(m,w-m-R2,R2,R2,90.0f,90.0f);
					gpLure.AddLine(m,w-m-R,m,m+R);
					gpLure.CloseFigure();
					g.FillPath(EgyptInformation.Brushes.EgyptLure,gpLure);
				}
				using(GraphicsPath gpGold = new GraphicsPath()) {
					gpGold.AddArc(m,m,R2,R2,180.0f,90.0f);
					gpGold.AddLine(m+R,m,w-m-R,m);
					gpGold.AddArc(w-m-R2,m,R2,R2,270.0f,90.0f);
					gpGold.AddLine(w-m,m+R,w-m,w-m-R);
					gpGold.AddArc(w-m-R2,w-m-R2,R2,R2,0.0f,90.0f);
					gpGold.AddLine(w-m-R,w-m,m+R,w-m);
					gpGold.AddArc(m,w-m-R2,R2,R2,90.0f,90.0f);
					gpGold.AddLine(m,w-m-R,m,m+R);
					gpGold.CloseFigure();
					gpGold.AddArc(mt,mt,r2,r2,180.0f,90.0f);
					gpGold.AddLine(m+R,mt,w-m-R,mt);
					gpGold.AddArc(w-mt-r2,mt,r2,r2,270.0f,90.0f);
					gpGold.AddLine(w-mt,m+R,w-mt,w-m-R);
					gpGold.AddArc(w-mt-r2,w-mt-r2,r2,r2,0.0f,90.0f);
					gpGold.AddLine(w-m-R,w-mt,m+R,w-mt);
					gpGold.AddArc(mt,w-mt-r2,r2,r2,90.0f,90.0f);
					gpGold.AddLine(mt,w-m-R,mt,m+R);
					gpGold.CloseFigure();
					g.FillPath(EgyptInformation.Brushes.EgyptGold,gpGold);
					GraphicsUtils.DrawGlass(g,gpGold);
				}
				g.FillEllipse(EgyptInformation.Brushes.EgyptNocturne,m,h+wa+m,R,R);
				using(GraphicsPath gpTurquoise = new GraphicsPath()) {
					gpTurquoise.AddEllipse(wa+m+t,h+wa+m+t,s,s);
					g.FillPath(EgyptInformation.Brushes.EgyptPaintTurquoise,gpTurquoise);
					GraphicsUtils.DrawGlass(g,gpTurquoise);
				}
				g.FillEllipse(EgyptInformation.Brushes.EgyptNocturne,m,h+wa+m,R,R);
				for(int i = 0x00; i < 0x02; i++) {
					using(GraphicsPath gpGold = new GraphicsPath()) {
						gpGold.AddEllipse(wa*i+m,h+m+wa,R,R);
						gpGold.AddEllipse(wa*i+m+t,h+m+wa+t,s,s);
						g.FillPath(EgyptInformation.Brushes.EgyptGold,gpGold);
						GraphicsUtils.DrawGlass(g,gpGold);
					}
				}
				g.FillRectangle(EgyptInformation.Brushes.EgyptPaintBlue,wa*2.5f-0.5f*t,h+wa,t,wa);
				bmp.Save("ti.png");
				return bmp;
			}
			private static Bitmap texture1 () {
				int h = 0x40;
				int bmpW = h<<0x01;
				int bmpH = h<<0x02;
				Bitmap bmp = new Bitmap(bmpW,bmpH);
				Graphics g = Graphics.FromImage(bmp);
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				float h_2 = 0.5f*h;
				float rightEnd = h*2.0f;
				float margin = h/48.0f;
				float margin2 = margin*2.0f;
				float margin3 = margin*3.0f;
				float r = h/36.0f;
				float r2 = 2.0f*r;
				float s = 0.25f*h;
				float s2 = s*2.0f;
				float s3 = s*3.0f;
				float t = 0.5f*s;
				float t2 = t*2.0f;
				float t3 = t*3.0f;
				float temp = (margin+s+t-h_2);
				float R = temp-4.0f*temp*(float) Math.Sqrt(0.5f);
				float xP = (2.0f-(float) Math.Sqrt(2.0f))*temp+r+h_2;
				float R2 = 2.0f*R;
				int royalN = 7;
				float gamma = 180.0f/royalN;
				float penW = h/1024.0f;
				for(int l = 0x00; l < 0x03; l++) {
					using(GraphicsPath gpNocturne = new GraphicsPath()) {
						gpNocturne.AddArc(margin,margin,h-margin2,h-margin2,90.0f,180.0f);
						gpNocturne.AddLine(h_2,margin,rightEnd,margin);
						gpNocturne.AddLine(rightEnd,margin,rightEnd,h-margin);
						gpNocturne.AddLine(rightEnd,h-margin,h_2,h-margin);
						gpNocturne.CloseFigure();
						if(l == 0x01)
							g.FillPath(EgyptInformation.Brushes.EgyptLure,gpNocturne);
						else
							g.FillPath(EgyptInformation.Brushes.EgyptNocturne,gpNocturne);
					}
					using(GraphicsPath gpGold = new GraphicsPath()) {
						for(int i = 0x00; i < royalN;) {
							using(GraphicsPath gpRoyal = new GraphicsPath()) {
								gpRoyal.AddArc(margin+r,margin+r,h-margin2-r2,h-margin2-r2,90+gamma*i++,gamma);
								gpRoyal.AddArc(margin+s,margin+s,h-margin2-s2,h-margin2-s2,90+gamma*i,-gamma);
								gpRoyal.CloseFigure();
								g.FillPath((((i&0x01) == 0x01) ? EgyptInformation.Brushes.EgyptPaintBlue : EgyptInformation.Brushes.EgyptPaintRed),gpRoyal);
							}
						}
						gpGold.AddArc(margin,margin,h-margin2,h-margin2,90.0f,180.0f);
						gpGold.AddLine(h_2,margin,rightEnd,margin);
						gpGold.AddLine(rightEnd,margin,rightEnd,margin+r);
						gpGold.AddLine(rightEnd,margin+r,h_2+r,margin+r);
						gpGold.AddLine(h_2+r,margin+r,h_2+r,s+t+margin);
						
						//flower
						gpGold.AddArc(r+margin+s+t,margin3+s3+t3-h,h-margin2-s2-t2,h-margin2-s2-t2,90.0f,-45.0f);
						gpGold.AddArc(xP-R,h_2-R,R2,R2,-45.0f,90.0f);
						gpGold.AddArc(r+margin+s+t,h-margin-s-t,h-margin2-s2-t2,h-margin2-s2-t2,-45.0f,-45.0f);
						
						gpGold.AddLine(h_2+r,h-s-t-margin,h_2+r,h-margin-r);
						gpGold.AddLine(h_2+r,h-margin-r,rightEnd,h-margin-r);
						gpGold.AddLine(rightEnd,h-margin-r,rightEnd,h-margin);
						gpGold.AddLine(rightEnd,h-margin,h_2,h-margin);
						gpGold.CloseFigure();
						gpGold.AddArc(margin+r,margin+r,h-margin2-r2,h-margin2-r2,90.0f,180.0f);
						gpGold.AddLine(h_2,margin+r,h_2,margin+s);
						gpGold.AddArc(margin+s,margin+s,h-margin2-s2,h-margin2-s2,270.0f,-180.0f);
						gpGold.AddLine(h_2,h-margin-s,h_2,h-margin-r);
						gpGold.CloseFigure();
						gpGold.AddArc(margin+r+s,margin+r+s,h-margin2-r2-s2,h-margin2-r2-s2,90.0f,180.0f);
						gpGold.AddLine(h_2,margin+r+s,h_2,margin+s+t);
						gpGold.AddArc(margin+s+t,margin+s+t,h-margin2-s2-t2,h-margin2-s2-t2,270.0f,-180.0f);
						gpGold.AddLine(h_2,h-margin-s-t,h_2,h-margin-r-s);
						gpGold.CloseFigure();
						g.FillPath(EgyptInformation.Brushes.EgyptGold,gpGold);
						GraphicsUtils.DrawGlass(g,gpGold);
						//g.DrawPath(new Pen(Color.Black,penW),gpGold);
						using(GraphicsPath gpTurquoise = new GraphicsPath()) {
							gpTurquoise.AddEllipse(margin+r+s+t,margin+r+s+t,h-margin2-r2-s2-t2,h-margin2-r2-s2-t2);
							gpTurquoise.CloseFigure();
							g.FillPath(EgyptInformation.Brushes.EgyptPaintTurquoise,gpTurquoise);
							Region oldRegion = g.Clip;
							g.Clip = new Region(gpTurquoise);
							if(l == 0x01) {
								using(GraphicsPath gpGlow = new GraphicsPath()) {
									gpGlow.AddEllipse(margin2+r2+s2+t2-h_2,margin+r+s+t,2.0f*(h-margin2-r2-s2-t2),2.0f*(h-margin2-r2-s2-t2));
									gpGlow.CloseFigure();
									PathGradientBrush pgbGlow = new PathGradientBrush(gpGlow);
									pgbGlow.CenterColor = EgyptInformation.Colors.EgyptPaintRed;
									pgbGlow.SurroundColors = new Color[] {Color.Transparent};
									g.FillPath(pgbGlow,gpGlow);
								}
							}
							LinearGradientBrush lgbGlass = new LinearGradientBrush(new PointF(0.0f,margin+r+s+t),new PointF(0.0f,h_2-margin-r-s-t),Color.FromArgb(0x50,0xff,0xff,0xff),Color.FromArgb(0x80,0xff,0xff,0xff));
							g.FillEllipse(lgbGlass,margin+r+s+t,margin2+r2+s2+t2-h_2,h-margin2-r2-s2-t2,h-margin2-r2-s2-t2);
							g.Clip = oldRegion;
							g.DrawPath(new Pen(Color.Black,penW),gpTurquoise);
						}
					}
					g.TranslateTransform(0.0f,h);
				}
				GraphicsUtils.DrawSepia(bmp,0x00,0x00,bmpW,bmpH>>0x02,0x00,0x03*bmpH/0x04);
				bmp.Save("ti.png");
				return bmp;
			}
			
		}
		
		public static class GraphicsPaths {
			
			private static readonly PointF[][] khepherData = new PointF[][] {
							new PointF[] {new PointF(0.093864589213156f,0.427258159629141f), new PointF(0.058997398053495f,0.338990371505269f), new PointF(0.094203205435588f,0.285077202775163f), new PointF(0.169253019138188f,0.268331180946390f), new PointF(0.163922937571135f,0.253033188206233f), new PointF(0.165942939380864f,0.188368046471860f), new PointF(0.216712843471404f,0.224390201489271f), new PointF(0.168854140185023f,0.169936637955179f), new PointF(0.163606789067516f,0.138416152012277f), new PointF(0.219246709710302f,0.220569704404842f), new PointF(0.216392078313234f,0.208806930275590f), new PointF(0.213615006712616f,0.200751699082763f), new PointF(0.207960651329935f,0.187271560238416f), new PointF(0.189728005180502f,0.143804680425939f), new PointF(0.204650017574065f,0.155429878249586f), new PointF(0.220613116346466f,0.183362515719709f), new PointF(0.228123859301605f,0.183024730495096f), new PointF(0.235626046056977f,0.182747177223519f), new PointF(0.243115552401184f,0.182557740498463f), new PointF(0.237428572659679f,0.142731523686823f), new PointF(0.248038629703063f,0.142149609769580f), new PointF(0.254315310119153f,0.182343589282713f), new PointF(0.259164028505072f,0.182282341665677f), new PointF(0.264006591351590f,0.182263998158263f), new PointF(0.268841952217010f,0.182296314740116f), new PointF(0.271735425067696f,0.141830014163896f), new PointF(0.278443301018803f,0.141137762202895f), new PointF(0.282087995897949f,0.182517606381571f), new PointF(0.290935906677098f,0.182758380305229f), new PointF(0.299756117528945f,0.183193022942311f), new PointF(0.308541918915542f,0.183868747280021f), new PointF(0.323904483264012f,0.147062376542963f), new PointF(0.333274937782886f,0.148746932233051f), new PointF(0.319733366655320f,0.191650610968063f), new PointF(0.319191432966484f,0.193367698683884f), new PointF(0.311527971078814f,0.213384866483273f), new PointF(0.313407934367177f,0.213572702768085f), new PointF(0.359857880906317f,0.147447990308719f), new PointF(0.370951024681866f,0.157198180053221f), new PointF(0.311879760155563f,0.223505650477393f), new PointF(0.343876253960320f,0.188643075972283f), new PointF(0.357746038448730f,0.225135144867542f), new PointF(0.356705382957651f,0.255087291704241f), new PointF(0.407685929606483f,0.252644127338413f), new PointF(0.490867949522115f,0.303694293137866f), new PointF(0.446356505451038f,0.421145585893474f), new PointF(0.458122819015446f,0.484277413540832f), new PointF(0.474660414280113f,0.600044766160292f), new PointF(0.466731710199667f,0.692136436915262f), new PointF(0.455816953955950f,0.818910817314055f), new PointF(0.384497150908588f,0.848259813622578f), new PointF(0.274185575970744f,0.844950841862346f), new PointF(0.163857504187306f,0.841641500769750f), new PointF(0.079203571690182f,0.796264587858937f), new PointF(0.072470581138319f,0.703342842615784f), new PointF(0.065266014712970f,0.603913522673622f), new PointF(0.073727111396181f,0.475977715100708f), new PointF(0.093864589213156f,0.427258159629141f)},
							new PointF[] {new PointF(0.061231797300550f,0.383959356819999f), new PointF(0.057856899713337f,0.348237838346918f), new PointF(0.033757655182687f,0.298706428783795f), new PointF(0.004322235100979f,0.272301380749043f), new PointF(0.011434468435317f,0.260241817287586f), new PointF(0.029745166824351f,0.235909708701409f), new PointF(0.000000000000000f,0.224756671527491f), new PointF(0.022523364888680f,0.199848650674986f), new PointF(0.036761681521009f,0.173965930936079f), new PointF(0.010805618530143f,0.143354433496476f), new PointF(0.040878321606005f,0.134948367335507f), new PointF(0.045548467793910f,0.112779407627821f), new PointF(0.036378868525681f,0.081402253419864f), new PointF(0.116253209344355f,0.072581365458683f), new PointF(0.136968199867902f,0.017884288785900f), new PointF(0.137591448232221f,0.000000000000000f), new PointF(0.137849549999292f,-0.007405929507994f), new PointF(0.152422050865684f,-0.001356173051910f), new PointF(0.151278536311219f,0.006483352651468f), new PointF(0.138276929099882f,0.095616040227681f), new PointF(0.091309240141750f,0.034107505264525f), new PointF(0.046824326445490f,0.174330492757085f), new PointF(0.042561184522758f,0.191448832386509f), new PointF(0.031902252496533f,0.286813126441550f), new PointF(0.074198502603485f,0.362348119759712f), new PointF(0.073215278295014f,0.377064598692687f), new PointF(0.069872451068078f,0.386472048003358f), new PointF(0.061231797300550f,0.383959356819999f)},
							new PointF[] {new PointF(0.070596650278569f,0.435826239587136f), new PointF(0.025242389752750f,0.494926804480986f), new PointF(-0.015268815483398f,0.595075337646725f), new PointF(0.015848236407184f,0.603673395081601f), new PointF(0.057133562278362f,0.615081086920525f), new PointF(0.062780407902974f,0.494536112395225f), new PointF(0.082843034360837f,0.449513389221527f), new PointF(0.089950097042079f,0.435106595475802f), new PointF(0.082336187246584f,0.425414083135484f), new PointF(0.070596650278569f,0.435826239587136f)},
							new PointF[] {new PointF(0.061231797300550f,0.692999751316208f), new PointF(0.023465593304743f,0.749632129576567f), new PointF(-0.021578366479887f,0.867945228625967f), new PointF(0.042502091344511f,0.986912353736690f), new PointF(0.054289087521306f,1.008795296306120f), new PointF(0.073024702795169f,1.002811250233140f), new PointF(0.065554032401528f,0.978267883534732f), new PointF(0.052224458050922f,0.934476022019596f), new PointF(0.031838665774524f,0.880084814099632f), new PointF(0.034382257765675f,0.824789049664739f), new PointF(0.059940242023498f,0.951003645310433f), new PointF(0.070956872444297f,0.920292797464903f), new PointF(0.068052750510448f,0.914957299023177f), new PointF(0.046430495034634f,0.872756275110754f), new PointF(0.046158543303912f,0.773133917452986f), new PointF(0.057007189054466f,0.741954140615911f), new PointF(0.059283569080233f,0.735411787119288f), new PointF(0.064175314686563f,0.729878942084377f), new PointF(0.068435501950383f,0.723975841354514f), new PointF(0.071074874134608f,0.723495955502837f), new PointF(0.081251273427214f,0.688142353619734f), new PointF(0.061231797300550f,0.692999751316208f)},
							new PointF[] {new PointF(0.477240447064516f,0.383959356819999f), new PointF(0.480615406207123f,0.348237838346918f), new PointF(0.504714589182378f,0.298706428783795f), new PointF(0.534150009264087f,0.272301380749043f), new PointF(0.527037899040536f,0.260241817287586f), new PointF(0.508727139096108f,0.235909708701409f), new PointF(0.538472305920459f,0.224756671527491f), new PointF(0.515948941031779f,0.199848650674986f), new PointF(0.501710562844056f,0.173965930936079f), new PointF(0.527666625834922f,0.143354433496476f), new PointF(0.497593984314455f,0.134948367335507f), new PointF(0.492923776571155f,0.112779407627821f), new PointF(0.502093437394779f,0.081402253419864f), new PointF(0.422219035020710f,0.072581365458683f), new PointF(0.401504044497163f,0.017884288785900f), new PointF(0.400880857688238f,0.000000000000000f), new PointF(0.400622817476561f,-0.007405929507994f), new PointF(0.386050193499381f,-0.001356173051910f), new PointF(0.387193769609240f,0.006483352651468f), new PointF(0.400195376820578f,0.095616040227681f), new PointF(0.447163004223316f,0.034107505264525f), new PointF(0.491647979474969f,0.174330492757085f), new PointF(0.495911059842307f,0.191448832386509f), new PointF(0.506569991868532f,0.286813126441550f), new PointF(0.464273803316974f,0.362348119759712f), new PointF(0.465256966070051f,0.377064598692687f), new PointF(0.468599793296987f,0.386472048003358f), new PointF(0.477240447064516f,0.383959356819999f)},
							new PointF[] {new PointF(0.469399274754348f,0.435826239587136f), new PointF(0.514753535280166f,0.494926804480986f), new PointF(0.555264740516314f,0.595075337646725f), new PointF(0.524147750181127f,0.603673395081601f), new PointF(0.482862424309949f,0.615081086920525f), new PointF(0.477215578685337f,0.494536112395225f), new PointF(0.457152952227474f,0.449513389221527f), new PointF(0.450045889546232f,0.435106595475802f), new PointF(0.457659799341727f,0.425414083135484f), new PointF(0.469399274754348f,0.435826239587136f)},
							new PointF[] {new PointF(0.479139307858839f,0.692999751316208f), new PointF(0.516905388743858f,0.749632129576567f), new PointF(0.561949410083882f,0.867945228625967f), new PointF(0.497869013814877f,0.986912353736690f), new PointF(0.486082017638083f,1.008795296306120f), new PointF(0.467346402364220f,1.002811250233140f), new PointF(0.474817011202466f,0.978267883534732f), new PointF(0.488146585553072f,0.934476022019596f), new PointF(0.508532377829471f,0.880084814099632f), new PointF(0.505988785838320f,0.824789049664739f), new PointF(0.480430863135890f,0.951003645310433f), new PointF(0.469414171159698f,0.920292797464903f), new PointF(0.472318354648940f,0.914957299023177f), new PointF(0.493940548569360f,0.872756275110754f), new PointF(0.494212500300083f,0.773133917452986f), new PointF(0.483363854549528f,0.741954140615911f), new PointF(0.481087536079155f,0.735411787119288f), new PointF(0.476195728917431f,0.729878942084377f), new PointF(0.471935480098218f,0.723975841354514f), new PointF(0.469296231024780f,0.723495955502837f), new PointF(0.459119770176781f,0.688142353619734f), new PointF(0.479139307858839f,0.692999751316208f)}
						};
			private static readonly PointF[] hedjetData = new PointF[] {
							new PointF(0f,0.669706866527198f), new PointF(0.0500673540904635f,0.562149972870286f), new PointF(0.0998908406934818f,0.487147824777547f), new PointF(0.151059161763886f,0.437075727015907f), new PointF(0.222518724417783f,0.366749791112246f), new PointF(0.336939496323099f,0.270382738230609f), new PointF(0.371605566563489f,0.248251763744943f), new PointF(0.406608282531949f,0.224958361121365f), new PointF(0.435331735304495f,0.197812241723116f), new PointF(0.460730486906538f,0.157616260784688f), new PointF(0.487857262749849f,0.109034140549121f), new PointF(0.49710115075109f,0.0536904445351527f), new PointF(0.560223110052908f,0.0169247944467177f), new PointF(0.590214738681331f,0f), new PointF(0.610823725082234f,0.00284505551978821f), new PointF(0.629252717924625f,0.0169936237875633f), new PointF(0.642651147078756f,0.0267856732406673f), new PointF(0.653074593551459f,0.0473482705212573f), new PointF(0.652575644921531f,0.0730231399350859f), new PointF(0.649515910615716f,0.108975423785672f), new PointF(0.636017148758619f,0.138444687901871f), new PointF(0.598194316586722f,0.17725395284683f), new PointF(0.580769418024076f,0.19375747259044f), new PointF(0.545083495367652f,0.248244592907643f), new PointF(0.533940368318494f,0.283739883428431f), new PointF(0.517622198488804f,0.324680937970049f), new PointF(0.50766677456582f,0.379938377369085f), new PointF(0.499218347841391f,0.438586324387921f), new PointF(0.492631328833937f,0.501890741661046f), new PointF(0.486026013863001f,0.565455522298369f), new PointF(0.472027654164368f,0.633452706656562f), new PointF(0.457321299698215f,0.699939529719688f), new PointF(0.443239840913831f,0.762590515509935f), new PointF(0.403327993337318f,0.837382525609196f), new PointF(0.364891685706019f,0.898805793230557f), new PointF(0.341191625784433f,0.946009733129811f), new PointF(0.277948883614014f,1f), new PointF(0.255284699831319f,0.973415728983553f), new PointF(0.235867695451275f,0.947111917381515f), new PointF(0.20695108747071f,0.921975683345821f), new PointF(0.213911211339916f,0.913990970766808f), new PointF(0.220197291257708f,0.904321140931856f), new PointF(0.226012692760113f,0.893474467510346f), new PointF(0.232193895493687f,0.881945472689427f), new PointF(0.238896297109553f,0.869613462129354f), new PointF(0.24425819479455f,0.856036145667758f), new PointF(0.250863539275105f,0.834583597311856f), new PointF(0.257344146598178f,0.807375315902195f), new PointF(0.23867349948973f,0.784511853665638f), new PointF(0.228806368345357f,0.773728212789285f), new PointF(0.209508877920317f,0.767186225459692f), new PointF(0.190334575397788f,0.775448269433551f), new PointF(0.163348648963671f,0.789517924370527f), new PointF(0.154617180733212f,0.800979696280459f), new PointF(0.140485024424212f,0.813213085695439f), new PointF(0.126606414757532f,0.78878555465811f), new PointF(0.103694453462937f,0.762551326736541f), new PointF(0.0800613656205816f,0.736172914818886f), new PointF(0.0539215463460616f,0.712102037030984f), new PointF(0.0297469274942541f,0.681152939320077f), new PointF(0f,0.669706866527198f)
						};
			private static readonly PointF[][] deshretData = new PointF[][] {
							new PointF[] {new PointF(0.052453279606973f,0.483646490184398f), new PointF(0.0371303741415139f,0.471576178229803f), new PointF(0.0353057264169917f,0.627972879741651f), new PointF(0.0020236914831334f,0.724514591692772f), new PointF(0f,0.73603525509949f), new PointF(0.0837422123796372f,0.767429278713381f), new PointF(0.192231772414592f,0.836245234274237f), new PointF(0.205654942429724f,0.816132595224719f), new PointF(0.222150314222681f,0.791780510637037f), new PointF(0.238843823818722f,0.77907858780505f), new PointF(0.25858741921941f,0.764964866511716f), new PointF(0.274033960968665f,0.764145030050545f), new PointF(0.291991225234507f,0.772399137572971f), new PointF(0.318012452944936f,0.784108133146472f), new PointF(0.322686597261947f,0.828739232851781f), new PointF(0.302632259115218f,0.854866990273981f), new PointF(0.288795174006878f,0.875734663318681f), new PointF(0.271483856032369f,0.893165460839859f), new PointF(0.256077815824357f,0.912062414154052f), new PointF(0.245698549799683f,0.919565644418424f), new PointF(0.34486012526274f,1.000000f), new PointF(0.349186649114578f,0.99453016027206f), new PointF(0.387962185203103f,0.954592443115852f), new PointF(0.416736638408315f,0.919113306152536f), new PointF(0.448946128580243f,0.872158590392885f), new PointF(0.494097859610787f,0.809199264278845f), new PointF(0.524408333767659f,0.761834630288609f), new PointF(0.571346555807553f,0.683280612050765f), new PointF(0.618085187850945f,0.609156290011926f), new PointF(0.654808334982705f,0.533169962905917f), new PointF(0.696349620068897f,0.45316876064964f), new PointF(0.739384172999562f,0.369957491500805f), new PointF(0.771648979750189f,0.28854118671008f), new PointF(0.804089848299346f,0.207095465010452f), new PointF(0.870596160171445f,0.0301886764298829f), new PointF(0.871444817342845f,0.0216741866304626f), new PointF(0.826759836598663f,0.00299988521010548f), new PointF(0.822711614291246f,0.00890660866845961f), new PointF(0.816164443558384f,0.0123415656979231f), new PointF(0.77346292254432f,0.100511076158992f), new PointF(0.736253403861438f,0.156550607368573f), new PointF(0.695780919809187f,0.217671270093161f), new PointF(0.671353161289181f,0.246756599273168f), new PointF(0.619144662872099f,0.306146567190618f), new PointF(0.571169681313623f,0.354919482403093f), new PointF(0.537916370499146f,0.382403615124938f), new PointF(0.490237663044017f,0.419973271114439f), new PointF(0.392281381473465f,0.487120882925073f), new PointF(0.342138315108173f,0.496657610308879f), new PointF(0.26204896870819f,0.506880412484749f), new PointF(0.221362665828498f,0.511267582065449f), new PointF(0.194460316430015f,0.512062797852864f), new PointF(0.146978654302186f,0.506258607243658f), new PointF(0.114566709715542f,0.5007719274012f), new PointF(0.0825712382141847f,0.49464969311023f), new PointF(0.052453279606973f,0.483646490184398f)},
							new PointF[] {new PointF(0.426835457905629f,0.459071220787864f), new PointF(0.412213229103683f,0.446453924860475f), new PointF(0.322077741855753f,0.346916697602383f), new PointF(0.306723980610846f,0.328540722008186f), new PointF(0.259040183817311f,0.271471279237983f), new PointF(0.245671904048864f,0.230741117452445f), new PointF(0.238816485278383f,0.187043791438806f), new PointF(0.233773776936056f,0.154900702477916f), new PointF(0.241217107552589f,0.0951237316755839f), new PointF(0.251726404841256f,0.0728928620190295f), new PointF(0.258731572731335f,0.0536259591004778f), new PointF(0.283096820319922f,0.0334784674088894f), new PointF(0.304045603030112f,0.0321246501013231f), new PointF(0.338238975933451f,0.0280864332733378f), new PointF(0.358716715060679f,0.0407800426310697f), new PointF(0.369559457275084f,0.0608371519358831f), new PointF(0.378321699265611f,0.0765166841302918f), new PointF(0.379078438588845f,0.0958978440283515f), new PointF(0.369540005876987f,0.109584274061566f), new PointF(0.35113280146283f,0.131975657721049f), new PointF(0.314403232705122f,0.125117680958488f), new PointF(0.315926623570895f,0.115919461193075f), new PointF(0.319592812425958f,0.104018296464674f), new PointF(0.349428006325488f,0.101668993906543f), new PointF(0.350929014760574f,0.0844437949986778f), new PointF(0.353276611990652f,0.0675906773552366f), new PointF(0.348415894126413f,0.0572782388736138f), new PointF(0.32646810892655f,0.051149822768453f), new PointF(0.313057888746316f,0.0483979562069625f), new PointF(0.297346008610414f,0.0520348880276272f), new PointF(0.28999673749427f,0.0592387266358066f), new PointF(0.275544295416537f,0.0733570309982809f), new PointF(0.267356642396651f,0.0883967454240453f), new PointF(0.269392644216662f,0.107545767540978f), new PointF(0.273598516108795f,0.12362765057274f), new PointF(0.282811604203165f,0.135424577123941f), new PointF(0.293853550050686f,0.144237179583514f), new PointF(0.316559033820812f,0.156919491142899f), new PointF(0.337262675623476f,0.153586853796577f), new PointF(0.357723734510692f,0.148313979458684f), new PointF(0.379256165746802f,0.140214310708f), new PointF(0.398774791073332f,0.119394706597776f), new PointF(0.402633495478056f,0.0981432281582915f), new PointF(0.410699084313444f,0.053723056216459f), new PointF(0.383407467141259f,0.0294681753277614f), new PointF(0.377318326872805f,0.0244305296780906f), new PointF(0.360961886024613f,0.0109348832207337f), new PointF(0.342253957666723f,0.000000f), new PointF(0.308911916502023f,0.0019885190920535f), new PointF(0.287628942785071f,0.00362808543139311f), new PointF(0.260597681244131f,0.0121342617565582f), new PointF(0.240854885215968f,0.0348425166847698f), new PointF(0.209667312879416f,0.0735753129889828f), new PointF(0.197523725110071f,0.142209437946256f), new PointF(0.205522513048158f,0.193838457897422f), new PointF(0.209186143911142f,0.235979139148268f), new PointF(0.234206557220844f,0.287586096417756f), new PointF(0.262487930807167f,0.329272947240667f), new PointF(0.300544511247118f,0.381060882449712f), new PointF(0.344690764870541f,0.428920701073152f), new PointF(0.392531931468408f,0.476497968154917f), new PointF(0.426835457905629f,0.459071220787864f)}
						};
			
			public static GraphicsPath Scarabee () {
				return renderBezierCurves(khepherData);
			}
			public static GraphicsPath[] ScarabeeParts () {
				int n = khepherData.Length;
				GraphicsPath[] paths = new GraphicsPath[n];
				for(int i = 0x00; i < n; i++)
					paths[i] = renderBezierCurves(khepherData[i]);
				return paths;
			}
			public static GraphicsPath Hedjet () {
				return renderBezierCurves(hedjetData);
			}
			public static GraphicsPath Deshret () {
				GraphicsPath gp = new GraphicsPath();
				PointF[] tempA = deshretData[0x00];
				PointF p1, p2, p3, p4 = tempA[0x00];
				int j = 0x01;
				for(int i = 0x00; i < 0x0b; i++) {
					p1 = p4;
					p2 = tempA[j++];
					p3 = tempA[j++];
					p4 = tempA[j++];
					gp.AddBezier(p1,p2,p3,p4);
				}
				p1 = p4;
				p4 = tempA[j++];
				gp.AddLine(p1,p4);
				for(int i = 0x00; i < 0x07; i++) {
					p1 = p4;
					p2 = tempA[j++];
					p3 = tempA[j++];
					p4 = tempA[j++];
					gp.AddBezier(p1,p2,p3,p4);
				}
				gp.CloseFigure();
				tempA = deshretData[0x01];
				p4 = tempA[0x00];
				j = 0x01;
				for(int i = 0x00; i < 0x14; i++) {
					p1 = p4;
					p2 = tempA[j++];
					p3 = tempA[j++];
					p4 = tempA[j++];
					gp.AddBezier(p1,p2,p3,p4);
				}
				gp.AddLine(p4,tempA[j++]);
				gp.CloseFigure();
				return gp;
			}
			private unsafe static GraphicsPath renderPath (PointF[] points, long[] instructions) {
				int i = 0x00;
				int iEnd = 0x00;
				byte mode = 0x00;
				int n = instructions.Length;
				PointF p1, p2, p3, p4 = PointF.Empty;
				GraphicsPath gp = new GraphicsPath();
				for(int j = 0x00; j <= n; j++) {
					if(j < n)
						iEnd = (int) (instructions[j]&0xffffffff);
					else
						iEnd = points.Length;
					for(; i < iEnd; ) {
						p1 = p4;
						switch(mode) {
							case 0x00 ://M
								p4 = points[i++];
								Console.WriteLine("M {0}",p4);
								break;
							case 0x01 ://L
								p4 = points[i++];
								Console.WriteLine("L {0}",p4);
								gp.AddLine(p1,p4);
								break;
							case 0x02 ://C
								p2 = points[i++];
								p3 = points[i++];
								p4 = points[i++];
								Console.WriteLine("C {0} {1} {2}",p2,p3,p4);
								gp.AddBezier(p1,p2,p3,p4);
								break;
						}
					}
					if(j < n)
						mode = (byte) ((instructions[j]>>0x20)&0xff);
				}
				gp.CloseFigure();
				return gp;
			}
			private static GraphicsPath renderBezierCurves (params PointF[] points) {
				return renderBezierCurves(new PointF[][] {points});
			}
			private static void ScaleCoordinates (PointF[][] data) {
				PointF[] temp;
				float minx = data[0x00][0x00].X, maxx = data[0x00][0x00].X, miny = data[0x00][0x00].Y, maxy = data[0x00][0x00].Y;
				for(int i = 0x00; i < data.Length; i++) {
					temp = data[i];
					for(int k = 0x00; k < temp.Length; k++) {
						minx = Math.Min(minx,temp[k].X);
						maxx = Math.Max(maxx,temp[k].X);
						miny = Math.Min(miny,temp[k].Y);
						maxy = Math.Max(maxy,temp[k].Y);
					}
				}
				double factor = 1.0d/Math.Max((double) maxx-minx,(double) maxy-miny);
				for(int i = 0x00; i < data.Length; i++) {
					temp = data[i];
					for(int k = 0x00; k < temp.Length; k++) {
						Console.Write(", new PointF({0}f,{1}f)",(factor*(temp[k].X-minx)).ToString("0.00000000000000000000",System.Globalization.NumberFormatInfo.InvariantInfo),(factor*(temp[k].Y-miny)).ToString("0.00000000000000000000",System.Globalization.NumberFormatInfo.InvariantInfo));
					}
					Console.WriteLine();
				}
			}
			private static GraphicsPath renderBezierCurves (PointF[][] points) {
				GraphicsPath gp = new GraphicsPath();
				int N = points.Length;
				int n, k;
				PointF[] pts;
				PointF p0, p1, p2, p3;
				for(int i = 0x00; i < N; i++) {
					pts = points[i];
					n = (pts.Length-0x01)/0x03;
					if(n > 0x00) {
						k = 0x01;
						p3 = pts[0x00];
						for(int j = 0x00; j < n; j++) {
							p0 = p3;
							p1 = pts[k++];
							p2 = pts[k++];
							p3 = pts[k++];
							gp.AddBezier(p0,p1,p2,p3);
						}
						gp.CloseFigure();
					}
				}
				return gp;
			}
			
		}
		
	}
	
}
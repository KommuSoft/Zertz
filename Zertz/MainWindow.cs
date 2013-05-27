using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Globalization;
using OpenTK;
using OpenTK.Input;
using Glu = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Zertz.Zertz;
using Zertz.Utils;
using Mono.Unix;

namespace Zertz.Rendering {
	
	public class MainWindow : GameWindow {
		
		public const float TIME_TRESHOLD = 0.01f;
		public const float TIME_IGNORANCE = 1.0f;
		public const float MOUSE_TRESHOLD = 4.0f;
		public const float LIGHT_FACTOR = 0.25f;
		
		/*private static float[] LightAmbient =  { 0.25f, 0.25f, 0.25f, 1.0f };
		private static float[] LightDiffuse =  { 1.0f, 1.0f, 1.0f, 1.0f };
		private static float[] LightPosition = { 26.30548f, 29.90608f, 0.4065362f, 0.0f };
		private static float[] LightSpecular = { 0.8f, 0.8f, 0.8f, 1.0f };*/
		private readonly float[][] l0 = new float[][] {	new float[] {-20.5f,7.0f,-7.0f},//l0P
														new float[] {0.1f,0.1f,0.1f,0.1f},//l0A
														new float[] {1.0f,1.0f,0.8627f,1.0f},//l0D//0.8f,0.8f,0.8f,0.8f
														new float[] {1.0f,1.0f,0.8627f,1.0f},//l0S
														new float[] {1.0f,1.0f,1.0f,1.0f},//l0O
														new float[] {0.0f,0.0f,0.0f,0.0f}};//l0M
		private readonly float[][] lT;
		
		private static float[] MaterialSpecular = { 0.8f, 0.8f, 0.8f, 1.0f };
		private static float[] SurfaceShininess = { 96.0f };
		
		private IScene scene;
		private Matrix4 upperMatrix = Matrix4.Identity;
		private float dt = 0.0f;
		private object renderLock = new object();
		private readonly Camera cam;
		private Matrix4 perspective;
		private int oldsel = -0x01;
		private int[] selectBuffer = new int[0x80];
		private int[] viewport = new int[0x04];
		private readonly MessageBoard messageBoard;
		private float lightTime = 0.0f;
		private readonly ComponentContainer cc = new ComponentContainer();
		private readonly SceneLoader sceneLoader;
		private readonly SubtitleRenderer subtitles;
		
		/*private float time = 0.0f;
		private BlendingFactorSrc[] srcs = new BlendingFactorSrc[] {BlendingFactorSrc.ConstantAlpha,BlendingFactorSrc.ConstantAlphaExt,BlendingFactorSrc.ConstantColor,BlendingFactorSrc.ConstantColorExt,BlendingFactorSrc.DstAlpha,BlendingFactorSrc.DstColor,BlendingFactorSrc.One,BlendingFactorSrc.OneMinusConstantAlpha,BlendingFactorSrc.OneMinusConstantAlphaExt,BlendingFactorSrc.OneMinusConstantColor,BlendingFactorSrc.OneMinusConstantColorExt,BlendingFactorSrc.OneMinusDstAlpha,BlendingFactorSrc.OneMinusDstColor,BlendingFactorSrc.OneMinusSrcAlpha,BlendingFactorSrc.SrcAlpha,BlendingFactorSrc.SrcAlphaSaturate,BlendingFactorSrc.Zero};
		private int src = 0x00;
		private BlendingFactorDest[] dsts = new BlendingFactorDest[] {BlendingFactorDest.ConstantAlpha,BlendingFactorDest.ConstantAlphaExt,BlendingFactorDest.ConstantColor,BlendingFactorDest.ConstantColorExt,BlendingFactorDest.DstAlpha,BlendingFactorDest.DstColor,BlendingFactorDest.One,BlendingFactorDest.OneMinusConstantAlpha,BlendingFactorDest.OneMinusConstantAlphaExt,BlendingFactorDest.OneMinusConstantColor,BlendingFactorDest.OneMinusConstantColorExt,BlendingFactorDest.OneMinusDstAlpha,BlendingFactorDest.OneMinusDstColor,BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorDest.SrcAlpha,BlendingFactorDest.Zero};
		private int dst = 0x00;*/
		
		public IScene CurrentScene {
			get {
				return this.scene;
			}
		}
		public SceneLoader SceneLoader {
			get {
				return this.sceneLoader;
			}
		}
		public Camera Camera {
			get {
				return this.cam;
			}
		}
		public SubtitleRenderer Subtitles {
			get {
				return this.subtitles;
			}
		}
		
		public MainWindow (MessageBoard messageBoard) {
			this.lT = new float[this.l0.Length][];
			for(int i = 0x00; i < this.l0.Length; i++) {
				this.lT[i] = new float[this.l0[i].Length];
				for(int j = 0x00; j < this.l0[i].Length; j++) {
					this.lT[i][j] = this.l0[i][j];
				}
			}
			this.Keyboard.KeyRepeat = true;
			this.Keyboard.KeyDown += HandleKeyboardhandleKeyDown;
			this.cam = new Camera();
			//this.cam.PositionTarget = new Vector3(0.0f,-5.0f,-8.5f);
			//this.cam.ThetaTarget = 30.0f;
			this.sceneLoader = new SceneLoader(this);
			this.subtitles = new SubtitleRenderer();
			float temp = (float)this.Width/(float)this.Height;
			this.perspective = Matrix4.CreatePerspectiveFieldOfView(0.25f*(float)Math.PI, temp, 1.0f, 64.0f);
			this.messageBoard = messageBoard;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void HandleMousehandleMove (object sender, MouseMoveEventArgs e) {
			if(this.cc.OnMouseMove(e)) {
				return;
			}
			this.cam.OnMouseMove(sender, e);
			/*float dx = e.X-this.oldPoint.X;
			float dy = e.Y-this.oldPoint.Y;
			if(dx*dx+dy*dy >= MOUSE_TRESHOLD*MOUSE_TRESHOLD) {
				this.oldPoint = e.Position;*/
			int sel = this.SelectPiece(e.Position.X, e.Position.Y);
			//Console.WriteLine("handling {0} @{1} resulting in {2}",e.Position,DateTime.Now.Ticks,sel);
			if(this.oldsel != sel) {
				this.selectionChanged(sel);
			}
			//}
		}
		private void selectionChanged (int sel) {
			int oldsel = this.oldsel;
			this.oldsel = sel;
			this.scene.MouseMoveSelectionChanged(oldsel, sel);
		}
		private void clickedItem (int sel) {
			this.scene.ClickedItem(sel);
		}
		private void HandleKeyboardhandleKeyDown (object sender, KeyboardKeyEventArgs e) {
			switch(e.Key) {
				case Key.F12:
					if(this.WindowState == WindowState.Fullscreen) {
						this.WindowState = WindowState.Normal;
					}
					else {
						this.WindowState = WindowState.Fullscreen;
					}
					break;
				default :
					if(!this.scene.HandleKeyDown(e.Key)) {
						this.cam.OnKeyDown(e);
					}
					break;
			}
		}
		void HandleMousehandleButtonDown (object sender, MouseButtonEventArgs e) {
			if(e.Button == MouseButton.Left) {
				int id = this.SelectPiece(e.Position.X, e.Position.Y);
				if(id != -0x01) {
					this.clickedItem(id);
				}
			}
			else if(e.Button == MouseButton.Right) {
				this.cam.OnMouseDown(sender, e);
			}
		}
		private void advanceTime (float time) {
			this.dt += time%TIME_IGNORANCE;
			this.lightTime += time;
			/*
			if(this.lightTime >= 10.0f) {
				this.lightTime = 0.0f;
				LightPosition = new float[]{ (float) (100.0d*UniversalRandom.NextSignedDouble()), (float) (100.0d*UniversalRandom.NextSignedDouble()), (float) (100.0d*UniversalRandom.NextSignedDouble()), 0.0f };
				string t = "Light: {";
				t += LightPosition[0x00].ToString(NumberFormatInfo.InvariantInfo);
				t += "f, ";
				t += LightPosition[0x01].ToString(NumberFormatInfo.InvariantInfo);
				t += "f, ";
				t += LightPosition[0x02].ToString(NumberFormatInfo.InvariantInfo);
				t += "f}";
				Console.WriteLine(t);
				GL.Light(LightName.Light0, LightParameter.Position, LightPosition);
				
			}//*/
			if(this.dt >= TIME_TRESHOLD) {
				float tim = Math.Min(this.dt, TIME_IGNORANCE);
				float abs, sgn;
				for(int i = 0x00; i < l0.Length; i++) {
					for(int j = 0x00; j < l0[i].Length; j++) {
						abs = lT[i][j]-l0[i][j];
						sgn = Math.Sign(abs);
						l0[i][j] += sgn*Math.Min(Math.Abs(abs), LIGHT_FACTOR*tim);
					}
				}
				GL.Light(LightName.Light0, LightParameter.Position, this.l0[0x00]);
				GL.Light(LightName.Light0, LightParameter.Ambient, this.l0[0x01]);
				GL.Light(LightName.Light0, LightParameter.Diffuse, this.l0[0x02]);
				GL.Light(LightName.Light0, LightParameter.Specular, this.l0[0x03]);
				GL.Light(LightName.Light0, LightParameter.SpotExponent, this.l0[0x04]);
				this.sceneLoader.AdvanceTime(tim);
				this.subtitles.AdvanceTime(tim);
				this.scene.AdvanceTime(tim);
				this.dt = 0.0f;
			}
		}
		private int SelectPiece (int x, int y) {
			
			/*int hits;
			int[] selectBuffer = new int[0x80];
			int[] viewport = new int[0x04];
			FrameEventArgs e = new FrameEventArgs();
			
			lock(renderLock) {
				///*
				GL.PushAttrib(AttribMask.ViewportBit);
				GL.Viewport(this.ClientRectangle);
				
				GL.RenderMode(RenderingMode.Select);
				
				GL.SelectBuffer(selectBuffer.Length*0x04,selectBuffer);
				
				GL.InitNames();
				GL.PushName(-0x01);
				
				GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadIdentity();
				GL.GetInteger(GetPName.Viewport,viewport);
				Glu.Glu.PickMatrix(x,this.ClientSize.Width-y,0.001f,0.001f,viewport);
				GL.MultMatrix(ref this.perspective);
				GL.MatrixMode(MatrixMode.Modelview);
				
				GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);
				
				this.scene.SelectionRender(e);
				
				GL.Flush();
				
				hits = GL.RenderMode(RenderingMode.Render);
				///*
				GL.MatrixMode(MatrixMode.Projection);
				GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Modelview);
				
				GL.PopName();
				GL.PopAttrib();
				
			}
			
			int pieceIndex = -0x01;
			uint closest = uint.MaxValue;
			for(int i = 0; i < hits; i++) {
				uint distance = (uint) selectBuffer[i*0x04+0x01];
				if(closest >= distance) {
					closest = distance;
					pieceIndex = (int) selectBuffer[i*0x04+0x03];
				}
			}*/
			
			int pieceIndex = -0x01, hits;
			FrameEventArgs e = new FrameEventArgs();
			
			lock(renderLock) {
				GL.PushAttrib(AttribMask.ViewportBit);
				GL.Viewport(this.ClientRectangle);
				
				GL.SelectBuffer(0x80*0x04, selectBuffer);
				GL.RenderMode(RenderingMode.Select);
				
				GL.Clear(ClearBufferMask.DepthBufferBit);
				
				GL.InitNames();
				GL.PushName(-0x01);
				
				GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadIdentity();
				
				GL.GetInteger(GetPName.Viewport, viewport);
				Glu.Glu.PickMatrix(x-0.0001f, this.Height-y-0.0001f, 0.0002f, 0.0002f, viewport);
				
				GL.MultMatrix(ref this.perspective);
				
				GL.MatrixMode(MatrixMode.Modelview);
				
				GL.PushMatrix();
				
				this.scene.Render(e);
				//this.scene.SelectionRender(e);
				GL.Flush();
				
				GL.PopMatrix();
				
				hits = GL.RenderMode(RenderingMode.Render);
				GL.MatrixMode(MatrixMode.Projection);
				//GL.PopName();
				GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PopAttrib();
			}
			
			int closest = int.MaxValue;
			/*Console.WriteLine("Array");
			for(int i = 0; i < hits; i++) {
				Console.Write("\t{0}",selectBuffer[i*0x04+0x00]);
				Console.Write("\t{0}",selectBuffer[i*0x04+0x01]);
				Console.Write("\t{0}",selectBuffer[i*0x04+0x02]);
				Console.WriteLine("\t{0}",selectBuffer[i*0x04+0x03]);
			}*/
			int distance, id;
			for(int i = 0; i < hits; i++) {
				distance = selectBuffer[i*0x04+0x01];
				if(distance < closest) {
					id = (int)selectBuffer[i*0x04+0x03];
					if(id != -0x01) {
						closest = distance;
						pieceIndex = id;
						//Console.WriteLine("new id: {0}",pieceIndex);
					}
				}
			}
			
			return pieceIndex;
		}
		protected override void OnLoad (EventArgs e) {
			base.OnLoad(e);
			OpenGLFont.LoadFont();
			this.WindowState = WindowState.Fullscreen;
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Enable(EnableCap.LineSmooth);
			GL.ShadeModel(ShadingModel.Smooth);
			
			this.LoadScene(new PrologueScene());

			/*GL.Light(LightName.Light0, LightParameter.Ambient, LightAmbient);
			GL.Light(LightName.Light0, LightParameter.Diffuse, LightDiffuse);
			GL.Light(LightName.Light0, LightParameter.Position, LightPosition);
			GL.Light(LightName.Light0, LightParameter.Specular, LightSpecular);*/
			GL.Light(LightName.Light0, LightParameter.Position, this.l0[0x00]);
			GL.Light(LightName.Light0, LightParameter.Ambient, this.l0[0x01]);
			GL.Light(LightName.Light0, LightParameter.Diffuse, this.l0[0x02]);
			GL.Light(LightName.Light0, LightParameter.Specular, this.l0[0x03]);
			GL.Light(LightName.Light0, LightParameter.SpotExponent, this.l0[0x04]);
			GL.LightModel(LightModelParameter.LightModelAmbient, this.l0[0x05]);
			GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
			GL.LightModel(LightModelParameter.LightModelLocalViewer, 0);
			GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
			//GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, MaterialSpecular);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, SurfaceShininess);
			
			this.Mouse.Move += HandleMousehandleMove;
			this.Mouse.ButtonDown += HandleMousehandleButtonDown;
			this.Mouse.ButtonUp += this.cam.OnMouseUp;
			this.Mouse.WheelChanged += this.cam.OnMouseWheel;
		}
		private void LoadScene (IScene scene) {
			if(this.scene != null) {
				this.scene.OnUnload(new EventArgs());
			}
			this.scene = scene;
			this.scene.MessageBoard = messageBoard;
			this.scene.MainWindow = this;
			this.scene.Camera = this.cam;
			this.cc.Clear();
			this.scene.ComponentContainer = this.cc;
			this.scene.OnLoad(new EventArgs());
		}
		protected override void OnUnload (EventArgs e) {
			this.scene.OnUnload(e);
			Unloader.CanUnload = false;
			base.OnUnload(e);
		}
		protected override void OnRenderFrame (FrameEventArgs e) {
			lock(renderLock) {
				/*this.time += (float) e.Time;
				if(this.time > 1.0f) {
					this.time -= 1.0f;
					this.dst++;
					if(this.dst >= dsts.Length) {
						this.dst = 0x00;
						this.src++;
						if(this.src >= srcs.Length) {
							this.src = 0x00;
						}
					}
					Console.WriteLine("GL.BlendFunc({0},{1});",this.srcs[this.src],this.dsts[this.dst]);
				}*/
				base.OnRenderFrame(e);
				advanceTime((float)e.Time);
				GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);
				
				/*GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadIdentity();
				
				GL.GetInteger(GetPName.Viewport,viewport);
				Glu.Glu.PickMatrix(this.Width/2-200f,this.Height/2+200f,400f,-400f,viewport);
				
				GL.MultMatrix(ref this.perspective);
				
				GL.MatrixMode(MatrixMode.Modelview);
				
				GL.PushMatrix();*/
				
				this.scene.Render(e);
				
				GL.PushAttrib(AttribMask.EnableBit);
				GL.Disable(EnableCap.DepthTest);
				GL.Disable(EnableCap.CullFace);
				GL.Disable(EnableCap.Lighting);
				GL.Normal3(0.0f, -1.0f, 0.0f);
				GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadIdentity();
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.LoadMatrix(ref this.upperMatrix);
				
				GL.Enable(EnableCap.Texture2D);
				GL.Color3(1.0f, 1.0f, 1.0f);
				this.cc.Render(e);
				this.scene.RenderDashboard(e);
				
				GL.Disable(EnableCap.Texture2D);
				GL.Color3(1.0f, 1.0f, 1.0f);
				this.messageBoard.Render(e);
				this.subtitles.Render(e);
				
				GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Projection);
				GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PopAttrib();
				
				GL.Flush();
				
				
				/*GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Projection);
				GL.PopMatrix();
				GL.MatrixMode(MatrixMode.Modelview);*/
				
				this.SwapBuffers();
			}
		}
		protected override void OnResize (EventArgs e) {
			base.OnResize(e);
			GL.Viewport(this.ClientRectangle);
			float temp = (float)this.Width/(float)this.Height;
			this.perspective = Matrix4.CreatePerspectiveFieldOfView(0.25f*(float)Math.PI, temp, 1.0f, 64.0f);
			float wi = 2.0f/this.Width;
			float hi = -2.0f/this.Height;
			this.upperMatrix = new Matrix4(wi, 0.0f, 0.0f, 0.0f,
			                               	0.0f, hi, 0.0f, 0.0f,
			                               	0.0f, 0.0f, 1.0f, 0.0f,
			                               	-1.0f, 1.0f, 0.0f, 1.0f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref perspective);
			if(this.scene != null) {
				this.scene.OnResize(e);
			}
			this.subtitles.OnResize(this.Width, this.Height);
		}
		public void LightTarget (int index, float[] target) {
			this.lT[index] = target;
		}
		
		public static int Main (string[] args) {
			//Texture t = TextureFactory.Sky(512,512,0.71828f);//TextureFactory.Wood(512,512,0.314159265f);
			//t.QuadMirror();
			//t.ToBitmap().Save("test2.png");
			//Catalog.Init("gtz","");
			//ZertzBoard.OffsetBoard().GenerateVisualRepresentation().Save("test.png");
			MessageBoard messageboard = new MessageBoard();
			using(MainWindow mw = new MainWindow(messageboard)) {
				mw.Run();
			}
			return 0x00;
		}
		
	}
	
}
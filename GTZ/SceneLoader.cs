using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace GTZ.Rendering {
	
	public class SceneLoader : ITimeSensitive {
		
		private readonly Queue<Action> actions = new Queue<Action>();
		private readonly MainWindow mw;
		private float time;
		private static readonly Regex loadRegex = new Regex(@"^\* (.*)$",RegexOptions.Compiled);
		private static readonly Regex timeRegex = new Regex(@"^([0-9]+\.[0-9]+) (.*)$",RegexOptions.Compiled);
		private static readonly Regex cameraRegex = new Regex(@"^C( ((-?[0-9]+\.[0-9]+)|N))+$");
		private static readonly Regex lightRegex = new Regex(@"^L (P|A|D|S|T)( (-?[0-9]+\.[0-9]+))+$");
		private static readonly Regex subtitRegex = new Regex(@"^S (-?[0-9]+\.[0-9]+) (.*)$");
		
		public MainWindow MainWindow {
			get {
				return this.mw;	
			}
		}
		
		public SceneLoader (MainWindow mw) {
			this.mw = mw;
		}
		
		public void AdvanceTime (float time) {
			this.time += time;
			while(this.actions.Count > 0x00 && this.actions.Peek().Time <= this.time) {
				try {
					this.actions.Dequeue().Perform();
				}
				catch (Exception e) {
					Console.Error.WriteLine(e);
				}
			}
		}
		public void LoadScene (Stream strm) {
			this.time = 0.0f;
			actions.Clear();
			StreamReader sr = new StreamReader(strm);
			string s;
			float t, span;
			Match m;
			float[] param;
			string str;
			while(!sr.EndOfStream) {
				s = sr.ReadLine();
				m = loadRegex.Match(s);
				if(m.Success) {
					this.MainWindow.CurrentScene.PerformAction(m.Groups[0x01].Value);
				}
				else {
					m = timeRegex.Match(s);
					if(m.Success) {
						t = float.Parse(m.Groups[0x01].Value,NumberFormatInfo.InvariantInfo);
						str = m.Groups[0x02].Value;
						m = cameraRegex.Match(str);
						if(m.Success) {
							param = new float[0x03];
							for(int i = 0x00; i < param.Length; i++) {
								str = m.Groups[0x01].Captures[i].Value;
								if(str == " N") {
									param[i] = float.PositiveInfinity;
								}
								else {
									param[i] = float.Parse(str,NumberFormatInfo.InvariantInfo);
								}
							}
							this.actions.Enqueue(new CameraAction(t,this,param));
						}
						else {
							m = subtitRegex.Match(str);
							if(m.Success) {
								span = float.Parse(m.Groups[0x01].Value,NumberFormatInfo.InvariantInfo);
								str = m.Groups[0x02].Value;
								this.actions.Enqueue(new SubtitleAction(t,this,span,str));
							}
							else {
								m = lightRegex.Match(str);
								if(m.Success) {
									str = m.Groups[0x01].Value;
									param = new float[m.Groups[0x03].Captures.Count];
									for(int i = 0x00; i < param.Length; i++) {
										param[i] = float.Parse(m.Groups[0x03].Captures[i].Value,NumberFormatInfo.InvariantInfo);
									}
									this.actions.Enqueue(new LightAction(t,this,str,param));
								}
								else {
									this.actions.Enqueue(new GenericAction(t,this,str));
								}
							}
						}
					}
				}
			}
			sr.Close();
		}
		
		private abstract class Action {
			
			private readonly float time;
			private readonly SceneLoader sl;
			
			public float Time {
				get {
					return this.time;	
				}
			}
			public SceneLoader SceneLoader {
				get {
					return this.sl;
				}
			}
			
			protected Action (float time, SceneLoader sl) {
				this.time = time;
				this.sl = sl;
			}
			
			public abstract void Perform ();
			
		}
		
		private class GenericAction : Action {
			
			private readonly string text;
			
			public GenericAction (float time, SceneLoader sl, string text) : base(time,sl) {
				this.text = text;
			}
			
			public override void Perform () {
				this.SceneLoader.MainWindow.CurrentScene.PerformAction(this.text);
			}
			
		}
		
		private class SubtitleAction : Action {
			
			private readonly float span;
			private readonly string text;
			
			public SubtitleAction (float time, SceneLoader sl, float span, string text) : base(time,sl) {
				this.span = span;
				this.text = text;
			}
			
			public override void Perform () {
				this.SceneLoader.MainWindow.Subtitles.SetText(this.span,this.text);
			}
			
		}
		
		private class LightAction : Action {
			
			private readonly int index;
			private readonly float[] target;
			
			public LightAction (float time, SceneLoader sl, string index, float[] target) : base(time,sl) {
				switch(index) {
					case "P" :
						this.index = 0x00;
						break;
					case "A" :
						this.index = 0x01;
						break;
					case "D" :
						this.index = 0x02;
						break;
					case "S" :
						this.index = 0x03;
						break;
					case "T" :
						this.index = 0x04;
						break;
				}
				this.target = target;
			}
			
			public override void Perform () {
				this.SceneLoader.MainWindow.LightTarget(index,target);
			}
			
		}
		
		private class CameraAction : Action {
			
			private readonly float xz, y, zoom;
			
			public CameraAction (float time, SceneLoader sl, float[] param) : base(time,sl) {
				this.xz = param[0x00];
				this.y = param[0x01];
				this.zoom = param[0x02];
			}
			
			public override void Perform () {
				if(xz < float.PositiveInfinity) {
					this.SceneLoader.MainWindow.Camera.RotateXZTarget = xz;
				}
				if(y < float.PositiveInfinity) {
					this.SceneLoader.MainWindow.Camera.RotateYTarget = y;
				}
				if(zoom < float.PositiveInfinity) {
					this.SceneLoader.MainWindow.Camera.ZoomTarget = zoom;
				}
			}
			
		}
		
	}
	
}
using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using Zertz.Utils;

namespace Zertz.Rendering {

	public class Camera : IRenderable, IKeyboardListener {

		private float zoomSpeed = 5.0f;
		private float rotateSpeed = 5.0f;
		private float zoom = 0.5f;
		private float rotateXZ = 0.0f;
		private float rotateY = 0.0f;
		private float zoomTarget = 0.5f;
		private float rotateXZTarget = 0.0f;
		private float rotateYTarget = 0.0f;
		private bool mouseDown = false;
		private Point offsetPoint = Point.Empty;
		private float rotateXYOffset, rotateZOffset;
		private float[,] savedCameraPositions;//saved cameraPositions
		private int showingView = 0x01;
		
		
		public float RotateXZ {
			get {
				return this.rotateXZ;
			}
		}
		public float RotateY {
			get {
				return this.rotateY;
			}
		}
		public float Zoom {
			get {
				return this.zoom;
			}
		}
		public float RotateXZTarget {
			get {
				return this.rotateXZTarget;
			}
			set {
				float diff = value-this.rotateXZ;
				diff -= 360.0f*(float) Math.Floor(diff/360.0f);
				if(diff >= 180.0f)
					diff -= 360.0f;
				this.rotateXZTarget = this.rotateXZ+diff;
			}
		}
		public float RotateYTarget {
			get {
				return this.rotateYTarget;
			}
			set {
				float diff = value-this.rotateY;
				diff -= 360.0f*(float) Math.Floor(diff/360.0f);
				if(diff >= 180.0f)
					diff -= 360.0f;
				this.rotateYTarget = this.rotateY+diff;
			}
		}
		public float ZoomTarget {
			get {
				return this.zoomTarget;
			}
			set {
				this.zoomTarget = value;
			}
		}
		
		public Camera () {
			savedCameraPositions = new float[,] {//RotateXZ, RotateY, Zoom
				{	60.0f,		0.0f,		0.5f},//silver saved
				{	60.0f,		180.0f,		0.5f}//red saved
			};
			this.MoveToOwner(0x00);
		}
		
		private void changeCameraView (int newView) {
			this.SaveCameraPosition(this.showingView);
			this.showingView = newView;
			this.LoadSavedCameraPosition(this.showingView);
		}
		public void Render (FrameEventArgs e) {
			GL.MatrixMode(MatrixMode.Modelview);
			float zoomFactor = Math.Min(1.0f,Math.Max(0.0f,(float) (this.zoomSpeed*e.Time)));
			float rotateFactor = Math.Min(1.0f,Math.Max(0.0f,(float) (this.rotateSpeed*e.Time)));
			this.rotateXZ = rotateFactor*this.rotateXZTarget+(1.0f-rotateFactor)*this.rotateXZ;
			this.rotateY = rotateFactor*this.rotateYTarget+(1.0f-rotateFactor)*this.rotateY;
			this.zoom = zoomFactor*this.zoomTarget+(1.0f-zoomFactor)*this.zoom;
			GL.LoadIdentity();
			GL.Translate(0.0f,0.0f,-6.0f);
			GL.Rotate(rotateXZ,1.0f,0.0f,0.0f);
			GL.Rotate(rotateY,0.0f,1.0f,0.0f);
			GL.Scale(zoom,zoom,zoom);
		}
		public void SaveCameraPosition (int index) {
			if((index&0xfc) == 0x00) {
				savedCameraPositions[index,0x00] = this.rotateXZTarget;
				savedCameraPositions[index,0x01] = this.rotateYTarget;
				savedCameraPositions[index,0x02] = this.zoomTarget;
			}
		}
		public void LoadSavedCameraPosition (int index) {
			if((index&0xfc) == 0x00) {
				this.RotateXZTarget = savedCameraPositions[index,0x00];
				this.RotateYTarget = savedCameraPositions[index,0x01];
				this.ZoomTarget = savedCameraPositions[index,0x02];
			}
		}
		public void MoveToOwner (int index) {
			this.ZoomTarget = 0.5f;
			switch (index) {
				case 0x00 :
					this.RotateXZTarget = 30.0f;
					this.RotateYTarget = 0.0f;
					break;
				case 0x01 :
					this.RotateXZTarget = 30.0f;
					this.RotateYTarget = 180.0f;
					break;
				case 0x02 :
					this.RotateXZTarget = 90.0f;
					this.RotateYTarget = 90.0f;
					break;
			}
		}
		public void MoveToPerspective () {
			this.RotateXZTarget = 45.0f;
			this.RotateYTarget = -45.0f;
			this.ZoomTarget = 0.4f;
		}
		public void OnKeyDown (KeyboardKeyEventArgs e) {
			this.OnKeyDown(null,e);
		}
		public void OnKeyDown (object s, KeyboardKeyEventArgs e) {
			switch(e.Key) {
				case Key.F1 ://player silver
					this.MoveToOwner(0x00);
					break;
				case Key.F2 ://player red
					this.MoveToOwner(0x01);
					break;
				case Key.F3 ://player all
					this.MoveToOwner(0x02);
					break;
				case Key.F4 ://perspective
					this.MoveToPerspective();
					break;
			}
		}
		public void OnMouseWheel (MouseWheelEventArgs e) {
			OnMouseWheel (null,e);
		}
		public void OnMouseWheel (object s, MouseWheelEventArgs e) {
			this.ZoomTarget *= (float) Math.Pow(1.1f,e.DeltaPrecise);
		}
		public void OnMouseDown (object s, MouseButtonEventArgs e) {
			if(e.Button == MouseButton.Right) {
				//stand still
				this.RotateXZTarget = this.rotateXZ;
				this.RotateYTarget = this.rotateY;
				this.ZoomTarget = this.zoom;
				this.offsetPoint = e.Position;
				this.rotateXYOffset = this.rotateXZ;
				this.rotateZOffset = this.rotateY;
				this.mouseDown = true;
			}
		}
		public void OnMouseMove (object s, MouseMoveEventArgs e) {
			if(mouseDown) {
				this.rotateY = this.rotateZOffset+(e.X-this.offsetPoint.X);
				this.rotateXZ = this.rotateXYOffset+(e.Y-this.offsetPoint.Y);
				this.rotateXZTarget = this.rotateXZ;
				this.rotateYTarget = this.rotateY;
			}
		}
		public void OnMouseUp (object s, MouseButtonEventArgs e) {
			if(e.Button == MouseButton.Right)
				this.mouseDown = false;
		}
		
	}
	
}
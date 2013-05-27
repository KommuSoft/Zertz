using System;
using OpenTK.Graphics.OpenGL;

namespace GTZ.Utils {

	public static class Unloader {
		
		private static bool canUnload = true;
		
		public static bool CanUnload {
			get {
				return canUnload;
			}
			internal set {
				canUnload = canUnload && value;
			}
		}
		
		public static void DeleteBuffer (ref int buffer) {
			if(canUnload) {
				GL.DeleteBuffers(0x01,ref buffer);
			}
		}
		public static void DeleteTexture (int buffer) {
			if(canUnload) {
				GL.DeleteTexture(buffer);
			}
		}
		
	}
}
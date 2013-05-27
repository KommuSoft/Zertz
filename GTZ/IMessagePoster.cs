using System;
using OpenTK.Graphics;

namespace GTZ {
	
	public interface IMessagePoster {
		
		string PosterName {
			get;
		}
		Color4 PosterColor {
			get;
		}
		
	}
	
}
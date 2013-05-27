using System;
using OpenTK.Graphics;

namespace Zertz {
	
	public interface IMessagePoster {
		
		string PosterName {
			get;
		}
		Color4 PosterColor {
			get;
		}
		
	}
	
}
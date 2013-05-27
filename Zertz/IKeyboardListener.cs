using System;
using OpenTK.Input;

namespace Zertz.Utils {
	
	public interface IKeyboardListener {
		
		void OnKeyDown (KeyboardKeyEventArgs e);
		
	}
}


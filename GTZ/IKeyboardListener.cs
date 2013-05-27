using System;
using OpenTK.Input;

namespace GTZ.Utils {
	
	public interface IKeyboardListener {
		
		void OnKeyDown (KeyboardKeyEventArgs e);
		
	}
}


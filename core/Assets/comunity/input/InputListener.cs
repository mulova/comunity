
using UnityEngine;

namespace comunity {
	public interface InputListener {
		void OnButton(KeyCode button, ButtonState state);
		/// <summary>
		/// Raises the focus event.
		/// </summary>
		/// <param name="focus">If set to <c>true</c> if this object gets focus.</param>
		/// <param name="triggerObj">object getting focus if 'focus' is false. object losing focus if 'focus' is true</param>
		void OnFocus(bool focus, object triggerObj);
	}
	
	public enum ButtonState {
		Pressed, Released, LongClick
	}
	
}

using UnityEngine;
using System;
using commons;

namespace core {
	public class AnimEventReceiver : MonoBehaviour {
        public const string END_CALLBACK = "OnClipEnd";

		private EventDispatcher<string> callback = new EventDispatcher<string>();
		private EventDispatcher<string> oneShotCallback = new EventDispatcher<string>(true);
		
        public void OnClipEnd() {
            callback.Broadcast(string.Empty);
            oneShotCallback.Broadcast(string.Empty);
        }

		public void OnClipEnd(string clipName) {
			callback.Broadcast(clipName);
			oneShotCallback.Broadcast(clipName);
		}

		public void SetCallback(Action<string> callback) {
			this.callback.SetCallback(callback);
		}

		public void AddCallback(Action<string> callback) {
			this.callback.AddCallback(callback);
		}

		public void AddOneShotCallback(Action<string> callback) {
			this.oneShotCallback.AddCallback(callback);
		}

		public void RemoveCallback(Action<string> callback) {
			this.callback.RemoveCallback(callback);
			this.oneShotCallback.RemoveCallback(callback);
		}

		public void PlayAudio(string clip) {
			AudioGroup.Broadcast(clip);
		}
	}
}

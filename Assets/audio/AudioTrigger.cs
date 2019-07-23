using UnityEngine;
using Nullable = commons.Nullable;
using commons;
using System.Text.Ex;

namespace audio {
	/// <summary>
	/// if the downloader is set, assets are downloaded and loaded automatically.
	/// else loaded by db at Start().
	/// </summary>
	public class AudioTrigger : MonoBehaviour {

		public enum Trigger
		{
			OnClick,
			OnMouseOver,
			OnMouseOut,
			OnPress,
			OnRelease,
			OnEnable,
			Custom,
		}
		
		public string clip;
		[HideInInspector] public string audioGroupGuid;
		public Trigger trigger = Trigger.OnClick;
		
		bool mIsOver = false;
		
		bool canPlay
		{
			get
			{
				return enabled;
			}
		}
		
		void OnHover (bool isOver)
		{
			if (trigger == Trigger.OnMouseOver)
			{
				if (mIsOver == isOver) return;
				mIsOver = isOver;
			}
			
			if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
				AudioGroup.Broadcast(clip);
		}
		
		void OnPress (bool isPressed)
		{
			if (trigger == Trigger.OnPress)
			{
				if (mIsOver == isPressed) return;
				mIsOver = isPressed;
			}
			
			if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease))) {
				AudioGroup.Broadcast(clip);
			}
		}
		
		void OnClick ()
		{
			if (canPlay && trigger == Trigger.OnClick) {
				AudioGroup.Broadcast(clip);
			}
		}
		
		void OnSelect (bool isSelected)
		{
			if (canPlay && (!isSelected))
				OnHover(isSelected);
		}

		void OnEnable() {
			if (canPlay && trigger == Trigger.OnEnable) {
				AudioGroup.Broadcast(clip);
			}
		}
		
        [ContextMenu("Play")]
		public void Play ()
		{
			if (clip.IsNotEmpty()) {
				AudioGroup.Broadcast(clip);
			}
		}
	}
}


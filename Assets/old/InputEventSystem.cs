#if OLD_INPUT

using UnityEngine;



/**
 * Convert InputAxis to InputEvent 
 */
using commons;


namespace comunity {
	public class InputEventSystem : SingletonBehaviour<InputEventSystem> {

		public InputAxisMapData[] inputStateData = new InputAxisMapData[0];
		private InputState state = InputState.Null;
		private InputState lockedState = InputState.Null;
		private bool locked;
		private float[] axisTime;
		private float[] axisValue;
		private EventDispatcher<InputEvent, object> dispatcher = new EventDispatcher<InputEvent, object>();
		public const float doubleClickInterval = 0.2f;

		void Start()
		{
			SetState(InputState.Null);
		}

		public void SetState(InputState state) {
			this.state = state;
			if (!locked) {
				axisValue = new float[GetData().Size];
				axisTime = new float[GetData().Size];
				// set current value
				InputAxisMapData data = GetData();
				for (int i=0; i<data.Size; i++) {
					InputAxis axis = data.GetAxis(i);
					axisValue[i] = Input.GetAxisRaw(axis.ToString());
				}
			}
		}

		public InputState GetState() {
			return this.state;
		}

		public void SetLocked(bool locked) {
			this.locked = locked;
			if (locked) {
				this.lockedState = state;
			} else {
				this.lockedState = InputState.Null;
			}
		}

		private InputAxisMapData GetData() {
			InputState s = locked? lockedState: state;
			return inputStateData[(int)s];
		}


		void Update() {
			InputAxisMapData data = GetData();
			for (int i=0; i<data.Size; i++) {
				InputAxis axis = data.GetAxis(i);
				InputAxisState axisState = GetAxisEvent(axis, ref axisValue[i], ref axisTime[i]);
				if (axisState != InputAxisState.Null) {
					if (axisState == InputAxisState.DoubleClick) {
						Broadcast(data, i, InputAxisState.Positive);
					}
					Broadcast(data, i, axisState);
				}
			}
		}

		private void Broadcast(InputAxisMapData data, int i, InputAxisState currentState) {
			InputAxis axis = data.GetAxis(i);
			BroadcastAxis(axis, currentState);
			if (data.GetTrigger(i) == currentState) {
				BroadcastEvent(data.GetEvent(i), null);
			}
		}

		/**
	 * positive axis가 눌리는 시점에 +, negative axis 가 눌리는 시점에 -를 반환한다.
	 */
		private InputAxisState GetAxisEvent(InputAxis axis, ref float button, ref float time) {
			float old = button;
			button = Input.GetAxisRaw(axis.ToString());
			if (old == 0 ^ button == 0) {
				if (button == 0) {
					return InputAxisState.Reset;
				} else {
					float oldTime = time;
					time = Time.time;
					if (time-oldTime < doubleClickInterval) {
						return InputAxisState.DoubleClick;
					}
					return button>0 ? InputAxisState.Positive: InputAxisState.Negative;
				}
			} 
			return InputAxisState.Null;
		}

		private bool IsPressed(InputAxis axis, ref bool button) {
			bool old = button;
			button = Input.GetButtonDown(axis.ToString());
			return old==false && button==true;
		}

		private bool leftButton;
		private bool IsLeftMouseButton() {
			return IsPressed(InputAxis.Fire1, ref leftButton);
		}

		private bool rightButton;
		private bool IsRightMouseButton() {
			return IsPressed(InputAxis.Fire2, ref rightButton);
		}

		public static bool IsDoubleClick(string buttonName, ref float clickTime) {
			float deltaTime = float.MaxValue;
			if (Input.GetButtonDown(buttonName)) {
				deltaTime = Time.time-clickTime;
				clickTime = Time.time;
			}
			return deltaTime < 0.2f;
		}

#region Event System
		private static readonly string AXIS_ID = typeof(InputAxisListener).FullName;
		public static void AddAxisListener(InputAxisListener l) {
			Messenger<InputAxis, InputAxisState>.AddListener(AXIS_ID, l.OnInputAxis);
		}
		public static void RemoveAxisListener(InputAxisListener l) {
			Messenger<InputAxis, InputAxisState>.RemoveListener(AXIS_ID, l.OnInputAxis);
		}
		public static void BroadcastAxis(InputAxis axis, InputAxisState state) {
			Messenger<InputAxis, InputAxisState>.Broadcast(AXIS_ID, axis, state);
		}

		public void AddEventListener(EventListener<InputEvent> l) {
			dispatcher.AddCallback(l.OnEvent);
		}
		public void RemoveEventListener(EventListener<InputEvent> l) {
			dispatcher.RemoveCallback(l.OnEvent);
		}
		public void BroadcastEvent(InputEvent evt, object data) {
			log.Debug("InputEvent.{0}", evt);
			dispatcher.Broadcast(evt, data);
		}
#endregion
	}

}

#endif
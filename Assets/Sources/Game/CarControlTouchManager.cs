#define USE_TOUCH_SCRIPT

using UnityEngine;
using System.Collections.Generic;

#if USE_TOUCH_SCRIPT
using TouchScript;
#endif

public class CarControlTouchManager : MonoBehaviour {
	public static Dictionary<int, TouchInfo> touches = new Dictionary<int, TouchInfo>();

	public class TouchInfo {
		public int id = 0;
		public Vector2 position = Vector2.zero;
		public LineDrawer drawer = null;

		public bool isTouchDown = false;
		public bool isTouch = false;
		public bool isTouchUp = false;
	}

	public static TouchInfo FindFreeTouchInfo() {
		foreach (var i in touches) {
			if (i.Value.drawer == null) {
				return i.Value;
			}
		}
		return null;
	}

	public static event System.Action OnFrameEnd;

	private void Awake() {
		Input.multiTouchEnabled = true;
	}

#if USE_TOUCH_SCRIPT
	private void OnEnable() {
		if (TouchManager.Instance != null) {
			TouchManager.Instance.TouchesBegan += TouchesBegan;
			TouchManager.Instance.TouchesCancelled += TouchesCancelled;
			TouchManager.Instance.TouchesEnded += TouchesEnded;
			TouchManager.Instance.TouchesMoved += TouchesMoved;
			TouchManager.Instance.FrameFinished += FrameFinished;
		}
	}

	private void OnDisable() {
		if (TouchManager.Instance != null) {
			TouchManager.Instance.TouchesBegan -= TouchesBegan;
			TouchManager.Instance.TouchesCancelled -= TouchesCancelled;
			TouchManager.Instance.TouchesEnded -= TouchesEnded;
			TouchManager.Instance.TouchesMoved -= TouchesMoved;
			TouchManager.Instance.FrameFinished -= FrameFinished;
		}
	}

	private void FrameFinished(object sender, System.EventArgs e) {
		foreach (var i in TouchManager.Instance.ActiveTouches) {
			if (touches.ContainsKey(i.Id)) {
				TouchInfo touchInfo = touches[i.Id];
				touchInfo.isTouch = true;
				touchInfo.position = Camera.main.ScreenToWorldPoint(i.Position);
			}
		}

		if (OnFrameEnd != null) {
			OnFrameEnd();
        }

		List<int> toRemove = new List<int>();
		foreach (var i in touches) {
			if (i.Value.isTouchUp) {
				toRemove.Add(i.Key);
				if (i.Value.drawer != null) {
					i.Value.drawer.TouchInfo = null;
				}
			}
			i.Value.isTouchDown = false;
			i.Value.isTouch = false;
			i.Value.isTouchUp = false;
		}
		foreach (var i in toRemove) {
			touches.Remove(i);
		}
	}

	private void TouchesMoved(object sender, TouchEventArgs e) {
        foreach (var i in e.Touches) {
			if (touches.ContainsKey(i.Id)) {
				TouchInfo touchInfo = touches[i.Id];
				touchInfo.isTouch = true;
				touchInfo.position = Camera.main.ScreenToWorldPoint(i.Position);
			}
		}
	}

	private void TouchesEnded(object sender, TouchEventArgs e) {
		foreach (var i in e.Touches) {
			if (touches.ContainsKey(i.Id)) {
				TouchInfo touchInfo = touches[i.Id];
				touchInfo.isTouchUp = true;
				touchInfo.position = Camera.main.ScreenToWorldPoint(i.Position);
			}
		}
	}

	private void TouchesCancelled(object sender, TouchEventArgs e) {
		TouchesEnded(sender, e);
	}

	private void TouchesBegan(object sender, TouchEventArgs e) {
		foreach (var i in e.Touches) {
			if (!touches.ContainsKey(i.Id)) {
				TouchInfo ti = new TouchInfo();
				ti.id = i.Id;
				ti.isTouchDown = true;
				touches.Add(i.Id, ti);
			}
		}
    }

	private void PrintTouches(string phase, TouchEventArgs e) {
		Debug.Log("TouchPhase: " + phase);
		foreach (var i in e.Touches) {
			Debug.LogFormat("touch id = {0} position = {1}", i.Id, i.Position);
		}
	}

#else

	private void Update() {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			int touchId = touch.fingerId;

			if (touch.phase == TouchPhase.Began) {
				if (!touches.ContainsKey(touchId)) {
					TouchInfo ti = new TouchInfo();
					ti.id = touchId;
					ti.isTouchDown = true;
					touches.Add(touchId, ti);
				}
			}

			if (touches.ContainsKey(touchId)) {
				TouchInfo touchInfo = touches[touchId];

				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
					touchInfo.isTouch = true;
				}

				if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
					touchInfo.isTouchUp = true;
				}

				touchInfo.position = Camera.main.ScreenToWorldPoint(touch.position);
			}
		}
#else
		for (int i = 0; i < 3; i++) {
			int touchId = i;

			if (Input.GetMouseButtonDown(touchId)) {
				if (!touches.ContainsKey(touchId)) {
					TouchInfo ti = new TouchInfo();
					ti.id = touchId;
					ti.isTouchDown = true;
					touches.Add(touchId, ti);
				}
			}

			if (touches.ContainsKey(touchId)) {
				TouchInfo touchInfo = touches[touchId];

				touchInfo.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				if (Input.GetMouseButton(touchId)) {
					touchInfo.isTouch = true;
				}

				if (Input.GetMouseButtonUp(touchId)) {
					touchInfo.isTouchUp = true;
				}
			}
		}
#endif

		if (OnFrameEnd != null) {
			OnFrameEnd();
        }
	}

	private void LateUpdate() {
		List<int> toRemove = new List<int>();
		foreach (var i in touches) {
			if (i.Value.isTouchUp) {
				toRemove.Add(i.Key);
				if (i.Value.drawer != null) {
					i.Value.drawer.TouchInfo = null;
				}
			}
			i.Value.isTouchDown = false;
			i.Value.isTouch = false;
			i.Value.isTouchUp = false;
		}
		foreach (var i in toRemove) {
			touches.Remove(i);
		}
	}

#endif

}

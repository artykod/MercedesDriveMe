using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour {
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

	private void Awake() {
		Input.multiTouchEnabled = true;
	}

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

				if (Input.GetMouseButton(touchId)) {
					touchInfo.isTouch = true;
				}

				if (Input.GetMouseButtonUp(touchId)) {
					touchInfo.isTouchUp = true;
				}

				touchInfo.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
		}
#endif
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
}

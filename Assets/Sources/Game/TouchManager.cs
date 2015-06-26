using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour {
	public static Dictionary<int, TouchInfo> touches = new Dictionary<int, TouchInfo>();

	public class TouchInfo {
		public int id = 0;
		public Vector2 position = Vector2.zero;
		public LineDrawer drawer = null;
	}

	[SerializeField]
	private LineDrawer[] drawers = null;

	private void Update() {
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			int touchId = touch.fingerId;

			if (touch.phase == TouchPhase.Began) {
				if (!touches.ContainsKey(touchId)) {
					TouchInfo touchInfo = new TouchInfo();
					touchInfo.id = touchId;
					touches.Add(touchId, touchInfo);
				}
			}
		}
	}
}

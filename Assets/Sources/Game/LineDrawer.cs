using UnityEngine;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {
	private LineRenderer line = null;
	private LinkedList<Vector2> points = new LinkedList<Vector2>();
	private int lineVerticesCount = 0;
	private bool canDraw = false;

	private Car controledCar = null;
	private int currentTouchIndex = -1;

	private static List<int> activeTouches = new List<int>(5);

	public bool IsUnderControl {
		get;
		private set;
	}

	public bool HasPoints {
		get {
			return points.Count > 2;
		}
	}

	public int PointsCount {
		get {
			return points.Count;
		}
	}

	public Vector2 FirstPoint {
		get {
			return points.Count > 1 ? points.First.Next.Value : Vector2.zero;
		}
		set {
			if (points.Count > 0) {
				points.First.Value = value;
				line.SetPosition(0, value);
			}
		}
	}

	public void Clear() {
		points.Clear();
		RebuildLine();

		if (controledCar != null) {
			controledCar.LineDrawer = null;
		}
	}

	public bool RemoveFirst() {
		if (points.Count < 2) {
			return false;
		}
		
        points.RemoveFirst();

		RebuildLine();

		if (points.Count < 2) {
			return false;
		}

		return true;
	}

	private void Awake() {
		line = GetComponent<LineRenderer>();
		line.sharedMaterial = Instantiate(line.sharedMaterial);
		lineVerticesCount = 0;
		line.SetVertexCount(lineVerticesCount);

		Input.multiTouchEnabled = true;
    }

	public bool isTouchDown = false;
	public bool isTouch = false;
	public bool isTouchUp = false;
	public Vector2 pointer = Vector2.zero;

	private void LateUpdate() {
        isTouchDown = false;
		isTouch = false;
		isTouchUp = false;
		pointer = Vector2.zero;
	}

	public TouchManager.TouchInfo TouchInfo {
		get;
		set;
	}

	private void Update() {
		Vector2 pointer = Vector2.zero;
		bool isTouchDown = false;
		bool isTouch = false;
		bool isTouchUp = false;

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			int touchId = touch.fingerId;

			if (!activeTouches.Contains(touchId) || currentTouchIndex == touchId) {
				if (touch.phase == TouchPhase.Began) {
					if (currentTouchIndex < 0) {
						currentTouchIndex = touchId;
						activeTouches.Add(touchId);
						isTouchDown = true;
						Debug.Log("Touch down: " + touchId);
					}
				} else if (currentTouchIndex == touchId) {
					if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
						isTouchUp = true;
						activeTouches.Remove(touchId);
						currentTouchIndex = -1;
						Debug.Log("Touch up: " + touchId);
					} else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
						isTouch = true;
						currentTouchIndex = touchId;
					}
				}

				if (currentTouchIndex == touchId && (isTouchDown || isTouch)) {
					pointer = Camera.main.ScreenToWorldPoint(touch.position);
				}
			}
		}

		if (currentTouchIndex < 0) {
			return;
		}
#else
		pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int touchId = 0;
		isTouchDown = Input.GetMouseButtonDown(touchId);

		if (currentTouchIndex >= 0) {
			isTouch = Input.GetMouseButton(touchId);
			isTouchUp = Input.GetMouseButtonUp(touchId);
			if (isTouchUp) {
				activeTouches.Remove(touchId);
				currentTouchIndex = -1;
			}
		}

		if (isTouchDown && !activeTouches.Contains(touchId)) {
			activeTouches.Add(touchId);
			currentTouchIndex = touchId;
		} else {
			if (currentTouchIndex < 0) {
				return;
			}
		}
#endif

		if (isTouchDown) {
			Clear();

			Collider2D[] colliders = Physics2D.OverlapCircleAll(pointer, 0f);

			if (colliders != null && colliders.Length > 0) {
				Car car = null;
				foreach (var c in colliders) {
					car = c.gameObject.GetComponent<Car>();
					if (car != null && car.LineDrawer == null) {
						controledCar = car;
						controledCar.LineDrawer = this;
						line.sharedMaterial.SetColor("_Color", controledCar.LineColor);
                        break;
					}
				}

				if (car != null) {
					IsUnderControl = true;
					points.AddFirst(pointer);
					points.AddLast(pointer);
				}
			}

			canDraw = true;
        }

		if (isTouch && canDraw) {
			if (points.Count > 1) {
				Vector2 lastPoint = points.Last.Previous.Value;
				Vector2 dir = pointer - lastPoint;
				float distance = dir.magnitude;

				if (distance > 0.1f) {
					points.AddBefore(points.Last, pointer);
				}
			}
		}

		RebuildLine();

		if (isTouch && canDraw) {
			if (points.Count > 0) {
				points.Last.Value = pointer;
				line.SetPosition(points.Count - 1, pointer);
			}

			Collider2D[] colliders = Physics2D.OverlapCircleAll(pointer, 0f);
			if (colliders != null && colliders.Length > 0) {
				int layer = LayerMask.NameToLayer("Level");
				foreach (var c in colliders) {
					if (c.gameObject.layer == layer) {
						isTouchUp = true;
						break;
					}
				}
			}
		}

		if (isTouchUp) {
			IsUnderControl = false;

			if (points.Count < 2) {
				points.Clear();
			}

			canDraw = false;
        }
	}

	private void RebuildLine() {
		if (lineVerticesCount == points.Count) {
			return;
		}

		lineVerticesCount = points.Count;
		line.SetVertexCount(lineVerticesCount);
		int index = 0;
		foreach (var i in points) {
			line.SetPosition(index++, i);
		}
	}
}

﻿using UnityEngine;
using System.Collections.Generic;
using TouchInfo = TouchManager.TouchInfo;

public class LineDrawer : MonoBehaviour {
	private LineRenderer line = null;
	private LinkedList<Vector2> points = new LinkedList<Vector2>();
	private int lineVerticesCount = 0;
	private bool canDraw = false;
	private Car controledCar = null;
	private TouchInfo touchInfo = null;

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

		TouchInfo = null;
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
    }

	public TouchInfo TouchInfo {
		get {
			return touchInfo;
		}
		set {
			touchInfo = value;

			/*if (touchInfo == null) {
				Debug.Log("Set touch info null of " + name);
			} else {
				Debug.LogFormat("Set touch info: {0}", touchInfo.id);
			}*/
		}
	}

	private void Update() {
		Vector2 pointer = Vector2.zero;
		bool isTouchDown = false;
		bool isTouch = false;
		bool isTouchUp = false;
		TouchInfo touchInfoLocal = TouchInfo;

		if (touchInfoLocal == null) {
			touchInfoLocal = TouchManager.FindFreeTouchInfo();
		}

		if (touchInfoLocal == null) {
			return;
		} else {
			pointer = touchInfoLocal.position;
			isTouchDown = touchInfoLocal.isTouchDown;
			isTouch = touchInfoLocal.isTouch;
			isTouchUp = touchInfoLocal.isTouchUp;
		}

		if (controledCar != null) {
			controledCar.LineDrawer = this;
		} else {
			if (points.Count > 0) {
				Clear();
				return;
			}
		}

		if (isTouchDown) {
			Clear();

			Collider2D[] colliders = Physics2D.OverlapCircleAll(pointer, 0.05f);

			if (colliders != null && colliders.Length > 0) {
				Car car = null;
				foreach (var c in colliders) {
					car = c.gameObject.GetComponent<Car>();
					if (car != null && car.LineDrawer == null) {
						controledCar = car;
						controledCar.LineDrawer = this;
						touchInfoLocal.drawer = this;
						TouchInfo = touchInfoLocal;
						line.sharedMaterial.SetColor("_Color", controledCar.LineColor);
                        break;
					}
				}

				if (controledCar != null) {
					IsUnderControl = true;
					points.AddFirst(pointer);
					points.AddLast(pointer);

					canDraw = true;
				}
			}
        }

		if (TouchInfo == null) {
			return;
		}

		if (isTouch && canDraw && controledCar != null) {
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

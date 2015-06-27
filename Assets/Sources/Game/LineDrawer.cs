using UnityEngine;
using System.Collections.Generic;
using TouchInfo = TouchManager.TouchInfo;

public class LineDrawer : MonoBehaviour {

	private static readonly bool BREAK_LINE_ON_LEVEL_COLLISION = false;
	private const float CAR_CATCH_PRECISION = 0.1f;

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

		controledCar = null;
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

			if (touchInfo == null) {
				IsUnderControl = false;
            }
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
		}

		pointer = touchInfoLocal.position;
		isTouchDown = touchInfoLocal.isTouchDown;
		isTouch = touchInfoLocal.isTouch;
		isTouchUp = touchInfoLocal.isTouchUp;

		if (controledCar != null) {
			controledCar.LineDrawer = this;

			if (controledCar != null && !controledCar.GoToTarget && controledCar.LineDrawer == null) {
				controledCar = null;
			}

		} else {
			if (points.Count > 0) {
				Clear();
				return;
			}
		}

		if (isTouchDown) {

			bool carRetrace = false;
			Car car = null;
			Collider2D[] colliders = null;

			if (controledCar != null && controledCar.LineDrawer == this) {
				colliders = Physics2D.OverlapCircleAll(pointer, CAR_CATCH_PRECISION);
				foreach (var c in colliders) {
					car = c.gameObject.GetComponent<Car>();
					if (car != null && car == controledCar) {
						carRetrace = true;
                        break;
					}
				}
			}

			if (controledCar == null || carRetrace) {
				Clear();

				bool carCatched = false;
				colliders = Physics2D.OverlapCircleAll(pointer, CAR_CATCH_PRECISION);
				if (colliders != null && colliders.Length > 0) {
					foreach (var c in colliders) {
						car = c.gameObject.GetComponent<Car>();
						if (car != null && car.LineDrawer == null) {
							if (car.LineDrawer == this) {
								Clear();
							}
							controledCar = car;
							controledCar.LineDrawer = this;
							touchInfoLocal.drawer = this;
							TouchInfo = touchInfoLocal;
							line.sharedMaterial.SetColor("_Color", controledCar.LineColor);
							carCatched = true;
							break;
						}
					}

					if (carCatched && controledCar != null && controledCar.LineDrawer == this) {
						IsUnderControl = true;
						points.AddFirst(pointer);
						points.AddLast(pointer);
						canDraw = true;
					}
				}
			}
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

			if (BREAK_LINE_ON_LEVEL_COLLISION) {
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

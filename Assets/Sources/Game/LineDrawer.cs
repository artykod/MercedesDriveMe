using UnityEngine;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {
	private LineRenderer line = null;
	private LinkedList<Vector2> points = new LinkedList<Vector2>();
	private int lineVerticesCount = 0;

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
		lineVerticesCount = 0;
		line.SetVertexCount(lineVerticesCount);
    }

	private void Update() {
		Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(0)) {
			Clear();

			Collider2D[] colliders = Physics2D.OverlapCircleAll(pointer, 0f);

			if (colliders != null && colliders.Length > 0) {
				Car car = null;
				foreach (var c in colliders) {
					car = c.gameObject.GetComponent<Car>();
					if (car != null) {
						break;
					}
				}

				if (car != null) {
					IsUnderControl = true;

					points.AddFirst(pointer);
					points.AddLast(pointer);
				}
			}
		}

		if (Input.GetMouseButton(0)) {
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

		if (Input.GetMouseButton(0)) {
			if (points.Count > 0) {
				points.Last.Value = pointer;
				line.SetPosition(points.Count - 1, pointer);
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			IsUnderControl = false;

			if (points.Count < 2) {
				points.Clear();
			}
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

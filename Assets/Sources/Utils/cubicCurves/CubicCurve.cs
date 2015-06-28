using UnityEngine;
using System.Collections.Generic;

public class CubicCurve : ScriptableObject {
	public class Evaluator {
		public enum MoveState {
			Start,
			Middle,
			End,
		}

		public class Vector2Wrapper {
			public Vector2 value = Vector2.zero;

			public Vector2Wrapper(Vector2 v) {
				value = v;
			}
		}
		
		public Vector2 StartPoint {
			get {
				return startPoint;
			}
		}
		public Vector2 EndPoint {
			get {
				return endPoint;
			}
		}

		private Vector2 startPoint = Vector2.zero;
		private Vector2 endPoint = Vector2.zero;
		private CubicSpline splineX = null;
		private CubicSpline splineY = null;

		private List<Vector2> plotted = new List<Vector2>(50);
		private int plotSegments = 0;
		
		public Evaluator(CubicSpline x, CubicSpline y, Vector2 start, Vector2 end) {
			splineX = x;
			splineY = y;
			startPoint = start;
			endPoint = end;
		}
		
		public Vector2 Evaluate(float t) {
			Vector2 result = Vector2.zero;

			if (splineX != null && splineY != null) {
				if (t > 1f) {
					t = 1f;
				}
				if (t < 0f) {
					t = 0f;
				}

				result.x = splineX.GetValue(t);
				result.y = splineY.GetValue(t);
			}

			return result;
		}

		public MoveState MoveOnPlottedPath(Vector2Wrapper point, ref int lastPoint, ref float t, float speed, int segments = 32) {
			if (segments != plotSegments) {
				Plot(segments);
			}

			if (speed == 0f) {
				return MoveState.Middle;
			}

			if (lastPoint < 0) {
				lastPoint = 0;
			}
			if (lastPoint > plotted.Count - 1) {
				lastPoint = plotted.Count - 1;
			}

			Vector2 a = plotted[lastPoint];

			if (speed < 0f && lastPoint <= 0) {
				point.value = a;
				t = 0f;
				return MoveState.Start;
			}
			if (speed > 0f && lastPoint >= plotted.Count - 1) {
				point.value = a;
				t = 1f;
				return MoveState.End;
			}

			int nextPoint = speed > 0f ? lastPoint + 1 : lastPoint - 1;
			Vector2 b = plotted[nextPoint];
			Vector2 v = (b - a).normalized;

			speed = Mathf.Abs(speed);

			v.x *= speed;
			v.y *= speed;

			Vector2 before = point.value;

			point.value.x += v.x;
			point.value.y += v.y;

			if (Vector2.Dot(point.value - b, before - b) < 0f) {
				lastPoint = nextPoint;
				point.value = b;
			}

			t = lastPoint / plotSegments;

			return MoveState.Middle;
		}

		private void Plot(int segmentsCount) {
			plotted.Clear();

			plotSegments = segmentsCount;

			for (int i = 0; i <= segmentsCount; i++) {
				float t = (float)i / (float)segmentsCount;
				plotted.Add(Evaluate(t));
			}
		}

		public void DrawGizmosPlot() {
			if (plotSegments <= 0) {
				return;
			}

			Vector3 a, b;

			Gizmos.color = Color.red;

			a = plotted[0];
			for (int i = 1; i < plotted.Count; i++) {
				b = plotted[i];
				a.z = b.z = -1f;
				Gizmos.DrawLine(a, b);
				a = b;
			}
		}
	}

	public Evaluator TestEvaluator {
		get {
			return testEvaluator;
		}
	}
	
	[SerializeField]
	private List<Vector2> dots = new List<Vector2>();
	
	private Evaluator testEvaluator = null;
	
	public List<Vector2> Dots {
		get {
			return dots;
		}
	}
	
	public Evaluator BuildEvaluatorWithStartEndPoint(Vector2 startPoint, Vector2 endPoint, Vector2 scale) {
		List<Vector2> dots = new List<Vector2>(this.dots);

		for (int i = 1; i < dots.Count; i++) {
			dots[i] = new Vector2((dots[i].x - dots[0].x) * scale.x + startPoint.x, (dots[i].y - dots[0].y) * scale.y + startPoint.y);
		}
		if (dots.Count > 0) {
			dots[0] = startPoint;
		}

		dots.Add(endPoint);

		return Build(dots);
	}
	
	public void Rebuild() {
		testEvaluator = Build(dots);
	}
	
	public Vector2 Evaluate(float t) {
		return testEvaluator != null ? testEvaluator.Evaluate(t) : Vector2.zero;
	}
	
	private Evaluator Build(List<Vector2> dots) {
		CubicSpline splineX = null;
		CubicSpline splineY = null;
		Vector2 startPoint = Vector2.zero;
		Vector2 endPoint = Vector2.zero;
		float totalDistance = 0f;

		if (dots.Count > 1) {
			List<float> keys = new List<float>();
			List<float> valuesX = new List<float>();
			List<float> valuesY = new List<float>();

			Vector2 a = dots[0];
			Vector2 b = Vector2.zero;

			for (int i = 0; i < dots.Count; i++) {
				b = dots[i];
				float distance = (b - a).magnitude;
				totalDistance += distance;
				a = b;

				valuesX.Add(b.x);
				valuesY.Add(b.y);
				keys.Add(totalDistance);
			}

			for (int i = 0; i < keys.Count; i++) {
				keys[i] /= totalDistance;
			}

			splineX = new CubicSpline(keys.ToArray(), valuesX.ToArray());
			splineY = new CubicSpline(keys.ToArray(), valuesY.ToArray());
			startPoint = dots[0];
			endPoint = dots[dots.Count - 1];
		} else {
			splineX = splineY = null;
		}

		return new Evaluator(splineX, splineY, startPoint, endPoint);
	}

	private void OnEnable() {
		Rebuild();
	}
}

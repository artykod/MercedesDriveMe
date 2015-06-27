using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

	public enum CarType {
		Red,
		Blue,
	}

	[SerializeField]
	private CarType type = CarType.Red;
	[SerializeField]
	private Color lineColor = Color.white;

	private Rigidbody2D body = null;
	private new Transform transform = null;
	private Vector2 lastTarget = Vector2.zero;
	private bool onTarget = false;
	private bool canMove = false;

	public CarType Type {
		get {
			return type;
		}
	}

	public Color LineColor {
		get {
			return lineColor;
		}
	}

	public LineDrawer LineDrawer {
		get {
			return lineDrawer;
		}
		set {
			lineDrawer = value;
		}
	}
	
	private LineDrawer lineDrawer = null;

	private void Awake() {

		if (GameCore.GameMode == GameCore.GameModes.OnePlayer && type == CarType.Blue) {
			Destroy(gameObject);
			return;
		}

		transform = base.transform;
		body = GetComponent<Rigidbody2D>();

		lastTarget = transform.position;

		canMove = false;
		StartCoroutine(WaitLevelStartPosition());
	}

	private IEnumerator WaitLevelStartPosition() {
		while (Level.StartPoint == null) {
			yield return null;
		}
		canMove = true;
		if (Level.StartPoint != null) {
			transform.position = Level.StartPoint.position;
			transform.rotation = Level.StartPoint.rotation;
		}
		lastTarget = transform.position;

		yield break;
	}

	private void Update() {
		if (!canMove) {
			return;
		}

		if (lineDrawer != null) {
			if (lineDrawer.HasPoints) {
				lastTarget = lineDrawer.FirstPoint;
				onTarget = true;
			}
		} else {
			onTarget = false;
		}

        Vector2 target = lastTarget;
		Vector2 rotation = transform.rotation * Vector2.up;
		Vector2 dir = target - new Vector2(transform.position.x, transform.position.y);
		float distance = dir.magnitude;

		if (onTarget && Mathf.Abs(distance) > 0.0001f) {
			float velocityMagnitude = Mathf.Min(10f, distance * 20f);
			float dot = Vector2.Dot(rotation, (dir / distance) * velocityMagnitude);
			Vector2 proj = rotation * dot;

			Vector2 velocity = Vector2.Lerp(body.velocity, proj * Time.deltaTime * 50f, 0.1f);
			velocity = rotation * Vector2.Dot(rotation, velocity);
			if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y)) {
				body.velocity = velocity;
			} else {
				body.velocity = Vector2.zero;
			}

			float k = Mathf.Min(Mathf.Pow(distance, 4f), 1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion2DHelper.RotationWithDirection(new Vector2(dir.y, -dir.x)), 0.2f * k);
		}

		body.velocity = rotation * Vector2.Dot(rotation, body.velocity);

		body.velocity *= 0.96f;
		body.angularVelocity = 0f;

		if (distance < 0.7f) {
			if (lineDrawer != null && lineDrawer.HasPoints) {
				if (!lineDrawer.RemoveFirst() || lineDrawer.PointsCount < 3) {
					TargetCompleted();
                }

				//lineDrawer.FirstPoint = transform.position;
			} else {
				onTarget = false;
			}
		}
	}

	private void TargetCompleted() {
		lastTarget = transform.position;
		body.velocity *= 0.75f;

		onTarget = true;

		if (lineDrawer != null && !lineDrawer.IsUnderControl) {
			lineDrawer.Clear();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (!canMove) {
			return;
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Checkpoint")) {
			Level.CheckpointCollide(collider);
		}
	}
}

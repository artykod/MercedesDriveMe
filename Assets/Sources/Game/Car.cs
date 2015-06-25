using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	private Rigidbody2D body = null;
	private new Transform transform = null;
	private Vector2 lastTarget = Vector2.zero;
	private bool onTarget = false;
	private bool canMove = false;

	[SerializeField]
	private LineDrawer lineDrawer = null;

	private void Awake() {
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
        transform.position = Level.StartPoint.position;
		transform.rotation = Level.StartPoint.rotation;
		lastTarget = transform.position;
	}

	private void Update() {
		if (!canMove) {
			return;
		}

		if (lineDrawer.HasPoints) {
			lastTarget = lineDrawer.FirstPoint;
			onTarget = true;
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
			if (lineDrawer.HasPoints) {
				if (!lineDrawer.RemoveFirst() || lineDrawer.PointsCount < 3) {
					lastTarget = transform.position;
					body.velocity *= 0.75f;

					onTarget = true;

					if (!lineDrawer.IsUnderControl) {
						lineDrawer.Clear();
					}
				}

				//lineDrawer.FirstPoint = transform.position;
			} else {
				onTarget = false;
			}
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

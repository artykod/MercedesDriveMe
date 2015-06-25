using UnityEngine;

public class Car : MonoBehaviour {

	private Rigidbody2D body = null;
	private new Transform transform = null;

	private void Awake() {
		transform = base.transform;
		body = GetComponent<Rigidbody2D>();
	}

	private void Update() {

		Vector2 mouse = Input.mousePosition;

        Vector2 target = Camera.main.ScreenToWorldPoint(mouse);

		Vector2 dir = target - new Vector2(transform.position.x, transform.position.y);
		float distance = dir.magnitude;
		Vector2 rotation = transform.rotation * Vector2.up;

		if (Input.GetMouseButton(0)) {

			float velocityMagnitude = Mathf.Min(8f, distance * 4f);
			float dot = Vector2.Dot(rotation, (dir / distance) * velocityMagnitude);
			Vector2 proj = rotation * dot;

			Vector2 velocity = Vector2.Lerp(body.velocity, proj * Time.deltaTime * 50f, 0.1f);
			body.velocity = rotation * Vector2.Dot(rotation, velocity);

			float k = Mathf.Min(Mathf.Pow(distance, 2f), 1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion2DHelper.RotationWithDirection(new Vector2(dir.y, -dir.x)), 0.1f * k);
		} else {
			body.velocity = rotation * Vector2.Dot(rotation, body.velocity);
		}

		body.velocity *= 0.96f;
		body.angularVelocity = 0f;
	}
}

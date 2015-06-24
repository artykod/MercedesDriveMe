using UnityEngine;
using System.Collections;

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

		if (Input.GetMouseButton(0)) {
			Vector2 dir = target - new Vector2(transform.position.x, transform.position.y);
			float distance = dir.magnitude;
			Vector2 rotation = transform.rotation * Vector2.up;

			Vector2 proj = rotation * Vector2.Dot(rotation, (dir / distance) * Mathf.Min(5f, distance * 4f));

			body.velocity = proj * Time.deltaTime * 50f;

			float k = Mathf.Min(distance, 1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion2DHelper.RotationWithDirection(new Vector2(dir.y, -dir.x)), 0.25f * k * k);
		}

		body.velocity *= 0.95f;
		body.angularVelocity = 0f;
	}
}

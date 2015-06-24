using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CarVisual : MonoBehaviour {
	[SerializeField]
	private Sprite[] frames = null;
	[SerializeField]
	private string spritePathInAssets = "";

	public string SpritePathInAssets {
		get {
			return spritePathInAssets;
		}
	}

	private SpriteRenderer sprite = null;
	private int currentFrame = 0;

	private void Awake() {
		sprite = GetComponent<SpriteRenderer>();
	}

	private void Update() {
		Vector3 rotation = transform.rotation.eulerAngles;
		currentFrame = (int)((1f - rotation.z / 360f) * (frames.Length - 1));
		sprite.sprite = frames[currentFrame];
	}
}

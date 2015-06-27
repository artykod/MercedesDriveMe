using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {
	[SerializeField]
	private int laps = 3;
	[SerializeField]
	private Transform startPoint1 = null;
	[SerializeField]
	private Transform startPoint2 = null;
	[SerializeField]
	private Sprite levelSplashSprite = null;
	[SerializeField]
	private Sprite levelTutorialSprite = null;

	private static Level instance = null;

	private SpriteRenderer splashRenderer = null;
	private SpriteRenderer tutorialRenderer = null;
	private CircleCollider2D[] checkpoints = null;
	private bool levelStarted = false;

	private GameObject splashObject = null;
	private GameObject tutorialObject = null;

	public static CircleCollider2D[] Checkpoints {
		get {
			return instance != null ? instance.checkpoints : null;
		}
	}

	public static bool LevelStarted {
		get {
			return instance != null ? instance.levelStarted : false;
		}
	}

	public static Transform StartPoint1 {
		get {
			return instance != null ? instance.startPoint1 : null;
		}
	}

	public static Transform StartPoint2 {
		get {
			return instance != null ? instance.startPoint2 : null;
		}
	}

	public static int TotalLaps() {
		return instance != null ? instance.laps : 0;
	}

	private IEnumerator ShowMenu() {
		levelStarted = false;

		yield return new WaitForSeconds(2f);
		Application.LoadLevel("menu");
	}

	private IEnumerator StartLevel() {
		yield return new WaitForSeconds(1f);

		tutorialRenderer.enabled = true;
		tutorialRenderer.color = Color.white;

		float time = 0.25f;
		while (time > 0f) {
			Color c = splashRenderer.color;
			c.a = time;
			splashRenderer.color = c;
			time -= Time.deltaTime;
			yield return null;
		}

		splashRenderer.enabled = false;

		time = 1f;
		while (time > 0f) {
			time -= Time.deltaTime;
			yield return null;
		}

		time = 0.25f;
		float current = time;
		while (current > 0f) {
			Color c = tutorialRenderer.color;
			c.a = current / time;
			tutorialRenderer.color = c;
			current -= Time.deltaTime;
			yield return null;
		}

		tutorialRenderer.enabled = false;

		levelStarted = true;
	}

	private void Awake() {
		instance = this;
		checkpoints = GetComponentsInChildren<CircleCollider2D>();

		splashObject = new GameObject("splash");
		Transform lt = splashObject.transform;
		lt.SetParent(transform, false);
		lt.localPosition = new Vector3(0f, 0f, -9f);

		splashRenderer = splashObject.AddComponent<SpriteRenderer>();
		splashRenderer.sprite = levelSplashSprite;
        splashRenderer.enabled = true;

		tutorialObject = new GameObject("tutorial");
		lt = tutorialObject.transform;
		lt.SetParent(transform, false);
		lt.localPosition = new Vector3(0f, 0f, -8.5f);

		tutorialRenderer = tutorialObject.AddComponent<SpriteRenderer>();
		tutorialRenderer.sprite = levelTutorialSprite;
		tutorialRenderer.enabled = false;

		StartCoroutine(StartLevel());
    }
}

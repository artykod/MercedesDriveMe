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
	private int lapsDone = 0;
	private float lapTime = 0f;
	private bool raceDone = false;
	private bool levelStarted = false;

	private GameObject splashObject = null;
	private GameObject tutorialObject = null;

	public static void CheckpointCollide(Collider2D checkpoint) {
		if (instance != null) {
			instance.CheckpointCollideCheck(checkpoint);
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

	public static string CurrentLapTime() {
		return instance != null && LevelStarted ? instance.LapTime() : "00:00:000";
	}

	public static int CurrentLap() {
		return instance != null && LevelStarted ? instance.lapsDone : 0;
	}

	public static int TotalLaps() {
		return instance != null ? instance.laps : 0;
	}

	private string LapTime() {
		float time = Time.time - lapTime;

		if (raceDone) {
			time = lapTime;
		}

		int lapMinutes = (int)(time / 60f);
		int lapSeconds = (int)(time - lapMinutes * 60f);
		int lapMillis = (int)(time * 100f - lapSeconds * 100f);

		return string.Format("{0:00}:{1:00}:{2:000}", lapMinutes, lapSeconds, lapMillis);
	}

	private void CheckpointCollideCheck(Collider2D checkpoint) {
		for (int i = 0; i < checkpoints.Length; i++) {
			if (checkpoints[i] == checkpoint) {
				if (i == 0 || !checkpoints[i - 1].enabled) {
					checkpoints[i].enabled = false;

					if (i == checkpoints.Length - 1) {
						lapsDone++;

						Debug.LogFormat("Lap done: {0} time: {1}", lapsDone, LapTime());

						foreach (var ch in checkpoints) {
							ch.enabled = true;
						}

						if (lapsDone >= 3) {
							Debug.Log("Race done");
							raceDone = true;
							lapTime = Time.time - lapTime;
							StartCoroutine(ShowMenu());

							return;
						}

						lapTime = Time.time;

						return;
					}
                }
			}
		}
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

		time = 0f;
		while (time > 0f) {
			Color c = tutorialRenderer.color;
			c.a = time;
			tutorialRenderer.color = c;
			time -= Time.deltaTime;
			yield return null;
		}

		tutorialRenderer.enabled = false;

		levelStarted = true;
	}

	private void Awake() {
		instance = this;
		checkpoints = GetComponentsInChildren<CircleCollider2D>();

		lapTime = Time.time;
		raceDone = false;

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

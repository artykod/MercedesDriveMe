using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public class PlayerLevelResult {
		public string time = "00:00:000";
		public float timeValue = 0f;
		public int laps = 0;
		public int place = 0;
		public Car.CarType carType = Car.CarType.Red;
	}

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
	[SerializeField]
	private Sprite levelTutorialHandSprite = null;
	[SerializeField]
	private LineRenderer tutorialLineRendererPrefab = null;

	[SerializeField]
	private CubicCurve tutorialCurve = null;
	[SerializeField]
	private CubicCurve botPathCurve = null;

	private static Level instance = null;
	
	private CircleCollider2D[] checkpoints = null;
	private bool levelStarted = false;

	private SpriteRenderer splashRenderer = null;
	private SpriteRenderer tutorialRenderer = null;
	private SpriteRenderer tutorialHandRenderer = null;

	private LineRenderer tutorialLineRenderer = null;

	private GameObject splashObject = null;
	private GameObject tutorialObject = null;
	private GameObject tutorialHandObject = null;

	private static List<PlayerLevelResult> playersResults = new List<PlayerLevelResult>();

	private float startTime = 3f;

	private bool raceDone = false;

	public static List<PlayerLevelResult> PlayersResults {
		get {
			return playersResults;
		}
	}

	public static bool RaceDone {
		get {
			return instance != null ? instance.raceDone : false;
		}
	}

	public static CubicCurve BotPathCurve {
		get {
			return instance != null ? instance.botPathCurve : null;
		}
	}

	public static float StartTime {
		get {
			return instance != null ? instance.startTime : 3f;
		}
	}

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

	public static bool IsTutorialDone {
		get;
		private set;
	}

	private IEnumerator ShowResults() {
		levelStarted = false;

		yield return new WaitForSeconds(Config.RaceResultsTime);
		Application.LoadLevel("result");
	}

	private IEnumerator ShowMainMenu() {
		levelStarted = false;
		yield return null;
		Application.LoadLevel("menu");
	}

	private IEnumerator StartLevel() {
		IsTutorialDone = false;

		yield return new WaitForSeconds(Config.RaceSplashTime);

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

		tutorialHandRenderer.enabled = true;
		tutorialLineRenderer.enabled = true;

		tutorialLineRenderer.sharedMaterial = Instantiate(tutorialLineRenderer.sharedMaterial);
		tutorialLineRenderer.sharedMaterial.SetColor("_Color", Config.RaceLineTutorialColor);

		time = Config.RaceTutorialTime;
		int linePointsCount = 0;
		while (time > 0f) {
			Vector2 p = tutorialCurve.Evaluate(1f - time);
			Vector3 handPosition = tutorialHandObject.transform.localPosition;
			handPosition.x = p.x;
			handPosition.y = p.y;
			tutorialHandObject.transform.localPosition = handPosition;

			linePointsCount++;
			tutorialLineRenderer.SetVertexCount(linePointsCount);
			handPosition.z += 0.1f;
            tutorialLineRenderer.SetPosition(linePointsCount - 1, handPosition);

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

		tutorialLineRenderer.enabled = false;
        tutorialHandRenderer.enabled = false;
		tutorialRenderer.enabled = false;
		IsTutorialDone = true;

		startTime = 4f;
        while (startTime > 0f) {
			startTime -= Time.deltaTime;
			yield return null;
			if (startTime < 1f) {
				startTime = 0f;
			}
		}

		levelStarted = true;

		while (startTime > -0.5f) {
			startTime -= Time.deltaTime;
			yield return null;
		}
	}

	private void Awake() {
		instance = this;
		checkpoints = GetComponentsInChildren<CircleCollider2D>();

		playersResults.Clear();

		GameCore.LastWinner = GameCore.PlayersTypes.Unknown;

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

		tutorialHandObject = new GameObject("tutorial_hand");
		lt = tutorialHandObject.transform;
		lt.SetParent(transform, false);
		lt.localPosition = new Vector3(0f, 0f, -8.8f);

		tutorialHandRenderer = tutorialHandObject.AddComponent<SpriteRenderer>();
		tutorialHandRenderer.sprite = levelTutorialHandSprite;
		tutorialHandRenderer.enabled = false;

		tutorialLineRenderer = Instantiate(tutorialLineRendererPrefab);
		tutorialLineRenderer.transform.SetParent(transform, false);

		StartCoroutine(StartLevel());
    }

	private void Update() {
		if (!raceDone) {

			// check inactive

			bool isAnyoneActive = false;
			int inactiveCount = 0;

			foreach (var i in Car.AllCars()) {
				if (i.IsBot && GameCore.LastWinner == GameCore.PlayersTypes.Unknown) {
					continue;
				}

				if (i.IsActive) {
					if (!i.RaceDone) {
						isAnyoneActive = true;
						break;
					}
				} else {
					inactiveCount++;
				}
			}

			if (
				(!isAnyoneActive && inactiveCount > 1) || 
				(!isAnyoneActive && GameCore.GameMode == GameCore.GameModes.OnePlayerWithBot) || 
				(GameCore.LastWinner != GameCore.PlayersTypes.Unknown && !isAnyoneActive)
			) {
				Debug.Log("All inactive, count = " + inactiveCount);

				StartCoroutine(inactiveCount > 1 ? ShowMainMenu() : ShowResults());
				raceDone = true;

				FillLevelResults();

				return;
			}

			// race done

			bool allDone = true;

			foreach (var i in Car.AllCars()) {
				if (!i.RaceDone) {
					allDone = false;
					break;
				}
			}

			if (allDone) {
				StartCoroutine(ShowResults());
				raceDone = true;

				FillLevelResults();

                return;
			}
		}
	}

	private void FillLevelResults() {
		playersResults.Clear();

		foreach (var i in Car.AllCars()) {
			PlayerLevelResult result = new PlayerLevelResult();

			result.carType = i.Type;
			result.laps = i.CurrentLap();
			result.time = i.CurrentLapTime();
			if (result.time == "00:00:000") {
				result.time = "--:--:---";
            }

			result.place = 
				GameCore.LastWinner == GameCore.PlayersTypes.Player1 && i.Type == Car.CarType.Red || 
				GameCore.LastWinner == GameCore.PlayersTypes.Player2 && i.Type == Car.CarType.Blue 
				? 1 : 2;

			if (GameCore.LastWinner == GameCore.PlayersTypes.Unknown) {
				result.place = 0;
			}

			playersResults.Add(result);
		}

		playersResults.Sort((PlayerLevelResult r1, PlayerLevelResult r2) => {
			return r1.place >= r2.place ? 1 : -1;
		});
    }
}

using UnityEngine;
using System.Collections.Generic;

public class LevelsManager : MonoBehaviour {

	public static readonly bool EMPTY_LEVEL = false;

	private const string PREFS_KEY_LAST_RACE_NUMBER = "lastRaceNumber";

	[SerializeField]
	private BackgroundVisual backgroundVisual = null;
	[SerializeField]
	private GameObject[] levelsPrefabs = null;

	private GameObject currentLevel = null;

	private static int currentLevelIndex = -1;
	private static int levelsCount = 0;
	private static int lastLevelIndex = 0;

	public static List<int> levelsDone = new List<int>();

	public static int LevelsCount {
		get {
			return levelsCount;
        }
	}

	public static bool IsRetryLevel {
		get;
		set;
	}

	public static int LastLevelIndex {
		get {
			return lastLevelIndex;
		}
	}

	public static void GoToNextLevel() {
		currentLevelIndex++;
		if (currentLevelIndex >= levelsCount) {
			currentLevelIndex = 0;
		}
	}

	public static void GenerateRandomLevel() {
		if (levelsCount <= 0) {
			return;
		}

		currentLevelIndex = Random.Range(0, levelsCount);
		Debug.Log("Random level = " + currentLevelIndex);
	}

	private void Start() {
		levelsCount = levelsPrefabs.Length;

		Random.seed = (int)System.DateTime.Now.Ticks;
		Debug.Log("Random.seed = " + Random.seed);

		if (currentLevelIndex < 0) {
			GenerateRandomLevel();
		}

		LoadSelectedLevel();
    }

	private void LoadSelectedLevel() {
		if (EMPTY_LEVEL) {
			return;
		}

		//LoadLevel(lastLevelIndex = 1);

		if (!IsRetryLevel) {
			int lastRace = PlayerPrefs.GetInt(PREFS_KEY_LAST_RACE_NUMBER, -1);
			if (lastRace == currentLevelIndex) {
				GoToNextLevel();
			}
		}
		PlayerPrefs.SetInt(PREFS_KEY_LAST_RACE_NUMBER, currentLevelIndex);
		PlayerPrefs.Save();

		LoadLevel(lastLevelIndex = currentLevelIndex);

		if (!levelsDone.Contains(currentLevelIndex)) {
			levelsDone.Add(currentLevelIndex);
		}
    }

	private void LoadLevel(int index) {
		if (currentLevel != null) {
			DestroyImmediate(currentLevel, false);
			currentLevel = null;
		}

		currentLevel = Instantiate(levelsPrefabs[index]);
		currentLevel.transform.localPosition = Vector3.zero;
		backgroundVisual.LoadBackground(levelsPrefabs[index].name);
	}

	private void OnGUI() {
		if (!Config.DebugMode) {
			return;
		}

		if (GUI.Button(new Rect(Screen.width - 100f, 0f, 100f, 64f), "Main menu")) {
			Application.LoadLevel("menu");
        }
	}
}

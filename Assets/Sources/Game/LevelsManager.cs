using UnityEngine;

public class LevelsManager : MonoBehaviour {

	public static readonly bool EMPTY_LEVEL = false;

	[SerializeField]
	private BackgroundVisual backgroundVisual = null;
	[SerializeField]
	private GameObject[] levelsPrefabs = null;

	private GameObject currentLevel = null;

	private static int currentLevelIndex = -1;
	private static int levelsCount = 0;
	private static int lastLevelIndex = 0;

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

	private void Start() {
		Random.seed = (int)System.DateTime.Now.Ticks;

		levelsCount = levelsPrefabs.Length;

		if (currentLevelIndex < 0) {
			currentLevelIndex = Random.Range(0, levelsPrefabs.Length);
		}

		LoadSelectedLevel();
    }

	private void LoadSelectedLevel() {
		if (EMPTY_LEVEL) {
			return;
		}

		//LoadLevel(lastLevelIndex = 1);
		LoadLevel(lastLevelIndex = currentLevelIndex);
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

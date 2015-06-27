using UnityEngine;

public class LevelsManager : MonoBehaviour {

	public static readonly bool EMPTY_LEVEL = false;

	[SerializeField]
	private BackgroundVisual backgroundVisual = null;
	[SerializeField]
	private GameObject[] levelsPrefabs = null;

	private GameObject currentLevel = null;

	private static int currentLevelIndex = -1;

	public void LoadLevel(int index) {
		if (currentLevel != null) {
			DestroyImmediate(currentLevel, false);
			currentLevel = null;
        }

		currentLevel = Instantiate(levelsPrefabs[index]);
		currentLevel.transform.localPosition = Vector3.zero;
		backgroundVisual.LoadBackground(levelsPrefabs[index].name);
	}

	private void Start() {
		Random.seed = (int)System.DateTime.Now.Ticks;

		if (currentLevelIndex < 0) {
			currentLevelIndex = Random.Range(0, levelsPrefabs.Length);
		}

		LoadRandomLevel();
    }

	private void LoadRandomLevel() {
		if (EMPTY_LEVEL) {
			return;
		}

		//LoadLevel(Random.Range(0, levelsPrefabs.Length));
		//LoadLevel(1);
		LoadLevel(currentLevelIndex++);
		if (currentLevelIndex >= levelsPrefabs.Length) {
			currentLevelIndex = 0;
		}
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

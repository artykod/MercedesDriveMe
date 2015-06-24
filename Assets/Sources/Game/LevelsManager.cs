using UnityEngine;

public class LevelsManager : MonoBehaviour {
	[SerializeField]
	private BackgroundVisual backgroundVisual = null;
	[SerializeField]
	private GameObject[] levelsPrefabs = null;

	private GameObject currentLevel = null;

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

		LoadRandomLevel();
    }

	private void LoadRandomLevel() {
		LoadLevel(Random.Range(0, levelsPrefabs.Length));
	}

	private void OnGUI() {
		if (!Config.DebugMode) {
			return;
		}

		if (GUI.Button(new Rect(Screen.width - 100f, 0f, 100f, 64f), "Next level")) {
			LoadRandomLevel();
        }
	}
}

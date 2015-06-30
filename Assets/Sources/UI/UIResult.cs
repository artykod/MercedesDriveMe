using UnityEngine;
using UnityEngine.UI;

public class UIResult : MonoBehaviour {
	[SerializeField]
	private Image background = null;
	[SerializeField]
	private Sprite gameWithBotWin = null;
	[SerializeField]
	private Sprite gameWithBotLose = null;
	[SerializeField]
	private Sprite twoPlayersBackground = null;

	[SerializeField]
	private Image buttons4Menu = null;
	[SerializeField]
	private Image buttons3Menu = null;

	public void GoToMainMenu() {
		LevelsManager.IsRetryLevel = false;
		Application.LoadLevel("menu");
	}

	public void NextLevel() {
		LevelsManager.IsRetryLevel = false;
		LevelsManager.GoToNextLevel();
		Application.LoadLevel("main");
	}

	public void RetryLevel() {
		LevelsManager.IsRetryLevel = true;
		Application.LoadLevel("main");
	}

	public void AnotherGame() {
		LevelsManager.IsRetryLevel = false;
		Debug.Log("AnotherGame stub");
	}

	private void Awake() {
		
		bool allLevelsDone = true;

		for (int i = 0; i < LevelsManager.LevelsCount; i++) {
			if (!LevelsManager.levelsDone.Contains(i)) {
				allLevelsDone = false;
				break;
			}
		}

		buttons4Menu.gameObject.SetActive(!allLevelsDone);
		buttons3Menu.gameObject.SetActive(allLevelsDone);

		if (GameCore.LastWinner == GameCore.PlayersTypes.Unknown && GameCore.GameMode == GameCore.GameModes.OnePlayerWithBot) {
			background.sprite = gameWithBotLose;
			return;
		}

		switch (GameCore.LastWinner) {
		case GameCore.PlayersTypes.Bot:
			background.sprite = gameWithBotLose;
			break;
		case GameCore.PlayersTypes.Player1:
			if (GameCore.GameMode == GameCore.GameModes.OnePlayerWithBot) {
				background.sprite = gameWithBotWin;
			} else {
				background.sprite = twoPlayersBackground;
			}
			break;
		case GameCore.PlayersTypes.Player2:
			background.sprite = twoPlayersBackground;
			break;
		default:
			//GoToMainMenu();
            break;
		}
	}
}

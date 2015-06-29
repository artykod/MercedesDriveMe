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
		Application.LoadLevel("menu");
	}

	public void NextLevel() {
		LevelsManager.GoToNextLevel();
		Application.LoadLevel("main");
	}

	public void RetryLevel() {
		Application.LoadLevel("main");
	}

	public void AnotherGame() {
		Debug.Log("AnotherGame stub");
	}

	private void Awake() {
		
		bool isNotLastLevel = LevelsManager.LastLevelIndex >= 2;

		buttons4Menu.gameObject.SetActive(!isNotLastLevel);
		buttons3Menu.gameObject.SetActive(isNotLastLevel);

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

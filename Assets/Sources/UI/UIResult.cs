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

	public void GoToMainMenu() {
		Application.LoadLevel("menu");
	}

	private void Awake() {
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

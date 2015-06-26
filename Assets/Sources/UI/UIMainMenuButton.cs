using UnityEngine;
using UnityEngine.EventSystems;

public class UIMainMenuButton : MonoBehaviour, IPointerClickHandler {

	public enum ButtonType {
		Unknown,
		OnePlayer,
		TwoPlayers,
	}

	[SerializeField]
	private ButtonType type = ButtonType.Unknown;

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		switch (type) {
		case ButtonType.OnePlayer:
			GameCore.GameMode = GameCore.GameModes.OnePlayer;
			break;
		case ButtonType.TwoPlayers:
			GameCore.GameMode = GameCore.GameModes.TwoPlayers;
			break;
		}

		Application.LoadLevel("main");
	}
}

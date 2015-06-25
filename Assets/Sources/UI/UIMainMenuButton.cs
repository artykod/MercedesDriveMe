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
			Application.LoadLevel("main");
			break;
		case ButtonType.TwoPlayers:
			//
			break;
		}
	}
}

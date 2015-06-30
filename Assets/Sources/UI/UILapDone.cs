using UnityEngine;
using UnityEngine.UI;

public class UILapDone : MonoBehaviour {
	[SerializeField]
	private Car.CarType carType = Car.CarType.Red;
	[SerializeField]
	private Image winImage = null;

	private Text text = null;

	private void Awake() {
		text = GetComponent<Text>();

		winImage.enabled = false;
    }

	private void Update() {
		if (!Level.IsTutorialDone) {
			text.text = "";
			return;
		}

		Car car = Car.CarByType(carType);

		if (car == null) {
			return;
		}

		if (
			car.RaceDone && (
				(GameCore.LastWinner == GameCore.PlayersTypes.Player1 && car.Type == Car.CarType.Red) ||
				(GameCore.LastWinner == GameCore.PlayersTypes.Player2 && car.Type == Car.CarType.Blue)
			)
		) {
			gameObject.SetActive(false);
			winImage.enabled = !car.IsBot;
		} else {
			if (car.IsBot) {
				gameObject.SetActive(false);
			} else {
				text.text = "КРУГ " + (car.CurrentLap() + 1) + " / " + Level.TotalLaps();
			}
		}
	}
}

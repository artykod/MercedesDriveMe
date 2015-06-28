using UnityEngine;
using UnityEngine.UI;

public class UILapDone : MonoBehaviour {
	[SerializeField]
	private Car.CarType carType = Car.CarType.Red;
	[SerializeField]
	private Image winImage = null;

	private Text text = null;

	private static GameCore.PlayersTypes lastWinner = GameCore.PlayersTypes.Unknown;

	private void Awake() {
		text = GetComponent<Text>();

		lastWinner = GameCore.PlayersTypes.Unknown;
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

		if (car.RaceDone && !Level.RaceDone && lastWinner == GameCore.PlayersTypes.Unknown) {
			if (car.Type == Car.CarType.Blue) {
				winImage.transform.localScale = new Vector3(-1f, -1f, 1f);
			}
			winImage.gameObject.SetActive(!car.IsBot);
			text.gameObject.SetActive(false);

			lastWinner = GameCore.LastWinner;
        } else {
			if (car.IsBot) {
				text.text = "";
			} else {
				text.text = "КРУГ " + (car.CurrentLap() + 1) + " / " + Level.TotalLaps();
			}
		}
	}
}

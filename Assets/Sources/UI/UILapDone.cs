using UnityEngine;
using UnityEngine.UI;

public class UILapDone : MonoBehaviour {
	[SerializeField]
	private Car.CarType carType = Car.CarType.Red;

	private Text text = null;

	private void Awake() {
		text = GetComponent<Text>();
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

		if (car.IsBot) {
			gameObject.SetActive(false);
			return;
		}

		text.text = "КРУГ " + (car.CurrentLap() + 1) + " / " + Level.TotalLaps();
	}
}

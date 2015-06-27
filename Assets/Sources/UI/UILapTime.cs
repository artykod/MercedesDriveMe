using UnityEngine;
using UnityEngine.UI;

public class UILapTime : MonoBehaviour {
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

		if (Level.StartTime > -0.5f) {
			int value = (int)Level.StartTime;
			if (value <= 0) {
				text.text = "ВПЕРЕД!";
			} else {
				text.text = value.ToString();
			}
			return;
		}

		Car car = Car.CarByType(carType);
		if (car == null) {
			return;
		}
		text.text = car.CurrentLapTime();
	}
}

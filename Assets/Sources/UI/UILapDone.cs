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
		Car car = Car.CarByType(carType);

		if (car == null) {
			return;
		}

		text.text = "Lap " + car.CurrentLap() + " / " + Level.TotalLaps();
	}
}

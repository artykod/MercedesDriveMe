using UnityEngine;
using UnityEngine.UI;

public class UILapTime : MonoBehaviour {
	private Text text = null;

	private void Awake() {
		text = GetComponent<Text>();
	}

	private void Update () {
		text.text = "Lap time " + Level.CurrentLapTime();
	}
}

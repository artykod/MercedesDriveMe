using UnityEngine;
using UnityEngine.UI;

public class UILapDone : MonoBehaviour {
	private Text text = null;

	private void Awake() {
		text = GetComponent<Text>();
	}

	private void Update() {
		text.text = "Lap " + Level.CurrentLap() + " / " + Level.TotalLaps();
	}
}

using UnityEngine;
using UnityEngine.UI;

public class UIResultLine : MonoBehaviour {
	[SerializeField]
	private Text[] places = null;
	[SerializeField]
	private Text[] names = null;
	[SerializeField]
	private Text[] times = null;

	private void Awake() {

		for (int i = 0; i < places.Length; i++) {
			places[i].transform.parent.gameObject.SetActive(false);
		}

		if (GameCore.GameMode != GameCore.GameModes.TwoPlayers) {
			return;
		}

		for (int i = 0; i < Level.PlayersResults.Count; i++) {

			places[i].transform.parent.gameObject.SetActive(true);

			Level.PlayerLevelResult result = Level.PlayersResults[i];

			places[i].text = result.place.ToString();
			names[i].text = result.carType == Car.CarType.Red ? "КРАСНЫЙ" : "СИНИЙ";
			Color color = result.carType == Car.CarType.Red ? Config.RaceLine1Color : Config.RaceLine2Color;
			color.a = 1f;
			names[i].color = color;
			times[i].text = result.time;
        }
	}
}

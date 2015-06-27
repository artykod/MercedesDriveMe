using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {
	[SerializeField]
	private Image intro = null;

	private static bool isFirstStart = true;

	private void Awake() {
		if (isFirstStart) {
			StartCoroutine(ShowIntro());
			isFirstStart = false;
        }
    }

	private IEnumerator ShowIntro() {
		intro.color = Color.white;
		intro.enabled = true;

		yield return new WaitForSeconds(1f);

		float time = 0.25f;
		float current = time;
		while (current > 0f) {
			Color c = intro.color;
			c.a = current / time;
			intro.color = c;
			current -= Time.deltaTime;
			yield return null;
		}

		intro.enabled = false;
	}
}

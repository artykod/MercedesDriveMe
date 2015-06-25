using UnityEngine;

public class Level : MonoBehaviour {
	[SerializeField]
	private int laps = 3;

	private static Level instance = null;
	private CircleCollider2D[] checkpoints = null;
	private int lapsDone = 0;
	private float lapTime = 0f;

	public static void CheckpointCollide(Collider2D checkpoint) {
		if (instance != null) {
			instance.CheckpointCollideCheck(checkpoint);
		}
	}

	public static string CurrentLapTime() {
		return instance != null ? instance.LapTime() : "00:00:000";
	}

	public static int CurrentLap() {
		return instance != null ? instance.lapsDone : 0;
	}

	public static int TotalLaps() {
		return instance != null ? instance.laps : 0;
	}

	private string LapTime() {
		float time = Time.time - lapTime;

		int lapMinutes = (int)(time / 60f);
		int lapSeconds = (int)(time - lapMinutes * 60f);
		int lapMillis = (int)(time * 100f - lapSeconds * 100f);

		return string.Format("{0:00}:{1:00}:{2:000}", lapMinutes, lapSeconds, lapMillis);
	}

	private void CheckpointCollideCheck(Collider2D checkpoint) {
		for (int i = 0; i < checkpoints.Length; i++) {
			if (checkpoints[i] == checkpoint) {
				if (i == 0 || !checkpoints[i - 1].enabled) {
					checkpoints[i].enabled = false;

					if (i == checkpoints.Length - 1) {
						lapsDone++;

						Debug.LogFormat("Lap done: {0} time: {1}", lapsDone, LapTime());

						lapTime = Time.time;

						foreach (var ch in checkpoints) {
							ch.enabled = true;
						}

						if (lapsDone >= 3) {
							Debug.Log("Race done");
							Application.LoadLevel("menu");
						}

						return;
					}
                }
			}
		}
	}

	private void Awake() {
		instance = this;
		checkpoints = GetComponentsInChildren<CircleCollider2D>();

		lapTime = Time.time;
    }
}

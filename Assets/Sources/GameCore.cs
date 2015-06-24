using UnityEngine;

public class GameCore : MonoBehaviour {

	private static bool initialized = false;

	[RuntimeInitializeOnLoadMethod]
	private static void Initialize() {
		if (!Application.isPlaying || initialized) {
			return;
		}

		initialized = true;

		DebugConsole.Initialize();
		Config.Load();
	}
}

using UnityEngine;

public class GameCore : MonoBehaviour {

	private static bool initialized = false;

	[RuntimeInitializeOnLoadMethod]
	private static void Initialize() {
		if (!Application.isPlaying || initialized) {
			return;
		}

		initialized = true;

		Application.targetFrameRate = 60;

		DebugConsole.Initialize();
		Config.Load();

#if !UNITY_ANDROID && !UNITY_IPHONE
		Debug.LogFormat("Set resolution: {0}x{1} fullscreen: {2}", Config.ScreenWidth, Config.ScreenHeight, Config.Fullscreen);
		Screen.SetResolution(Config.ScreenWidth, Config.ScreenHeight, Config.Fullscreen);
#endif
	}
}

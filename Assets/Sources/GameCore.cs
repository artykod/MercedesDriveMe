using UnityEngine;

public class GameCore : MonoBehaviour {

	public enum GameModes {
		OnePlayer,
		OnePlayerWithBot,
		TwoPlayers,
	}

	public static GameModes GameMode {
		get;
		set;
	}

	private static bool initialized = false;

	private void Awake() {
		Initialize();
	}

	[RuntimeInitializeOnLoadMethod]
	private static void Initialize() {
		if (!Application.isPlaying || initialized) {
			return;
		}

		GameMode = GameModes.TwoPlayers;

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

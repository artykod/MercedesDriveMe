using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public static class Config {

	private const string FILE_NAME = "config.json";

	private const string KEY_SCREEN_WIDTH = "screenWidth";
	private const string KEY_SCREEN_HEIGHT = "screenHeight";
	private const string KEY_FULLSCREEN = "fullscreen";
	private const string KEY_DEBUG_MODE = "debug";
	private const string KEY_GAME_SPLASH_TIME = "gameSplashTime";
	private const string KEY_RACE_SPLASH_TIME = "raceSplashTime";
	private const string KEY_RACE_RESULTS_TIME = "raceResultsTime";
	private const string KEY_RACE_CAR_SPEED_BOT = "carSpeedBot";
	private const string KEY_RACE_CAR_SPEED_PLAYER = "carSpeedPlayer";
	private const string KEY_RACE_TUTORIAL_TIME = "tutorialTime";
	private const string KEY_PLAYER_INACTIVE_COOLDOWN = "playerInactiveCooldown";
	private const string KEY_RACE_LINE_1_COLOR_R = "raceLine1Color_r";
	private const string KEY_RACE_LINE_1_COLOR_G = "raceLine1Color_g";
	private const string KEY_RACE_LINE_1_COLOR_B = "raceLine1Color_b";
	private const string KEY_RACE_LINE_1_COLOR_A = "raceLine1Color_a";
	private const string KEY_RACE_LINE_2_COLOR_R = "raceLine2Color_r";
	private const string KEY_RACE_LINE_2_COLOR_G = "raceLine2Color_g";
	private const string KEY_RACE_LINE_2_COLOR_B = "raceLine2Color_b";
	private const string KEY_RACE_LINE_2_COLOR_A = "raceLine2Color_a";
	private const string KEY_RACE_LINE_TUTORIAL_COLOR_R = "raceLineTutorialColor_r";
	private const string KEY_RACE_LINE_TUTORIAL_COLOR_G = "raceLineTutorialColor_g";
	private const string KEY_RACE_LINE_TUTORIAL_COLOR_B = "raceLineTutorialColor_b";
	private const string KEY_RACE_LINE_TUTORIAL_COLOR_A = "raceLineTutorialColor_a";


	public static int ScreenWidth { get; private set; }
	public static int ScreenHeight { get; private set; }
	public static bool Fullscreen { get; private set; }
	public static bool DebugMode { get; private set; }
	public static float GameSplashTime { get; private set; }
	public static float RaceSplashTime { get; private set; }
	public static float RaceResultsTime { get; private set; }
	public static float RaceCarSpeedBot { get; private set; }
	public static float RaceCarSpeedPlayer { get; private set; }
	public static float RaceTutorialTime { get; private set; }
	public static float PlayerInactiveCooldown { get; private set; }
	public static Color RaceLine1Color {
		get {
			return raceLine1Color;
		}
	}
	public static Color RaceLine2Color {
		get {
			return raceLine2Color;
		}
	}
	public static Color RaceLineTutorialColor {
		get {
			return raceLineTutorialColor;
		}
	}

	private static Color raceLine1Color = Color.white;
	private static Color raceLine2Color = Color.white;
	private static Color raceLineTutorialColor = Color.white;


	static Config() {
		DebugMode = true;

		if (Application.isPlaying) {
			Load();
		}
	}

	private static bool loaded = false;


	public static void Load() {

		if (loaded) {
			return;
		}

		loaded = true;

		ScreenWidth = 1920;
		ScreenHeight = 1080;
		Fullscreen = false;
		DebugMode = true;

		GameSplashTime = 0.5f;
		RaceSplashTime = 0.5f;
		RaceResultsTime = 2f;
		RaceTutorialTime = 1f;
		RaceCarSpeedBot = 8f;
		RaceCarSpeedPlayer = 10f;
		raceLine1Color = Color.blue;
		raceLine2Color = Color.red;
		raceLineTutorialColor = Color.white;

		try {
			string configJson = "";

#if !UNITY_IPHONE && !UNITY_ANDROID
			string configPath = Application.dataPath + "/resources/";
#if !UNITY_EDITOR
			configPath += "../../";
#endif
			Debug.Log("Config path: " + configPath + FILE_NAME);
			using (StreamReader sr = new StreamReader(configPath  + FILE_NAME)) {
				configJson = sr.ReadToEnd();
			}
#else
			TextAsset res = Resources.Load<TextAsset>(System.IO.Path.GetFileNameWithoutExtension(FILE_NAME));
			configJson = res.text;

			Application.targetFrameRate = 60;
#endif

			Dictionary<string, object> values = MiniJSON.Json.Deserialize(configJson) as Dictionary<string, object>;

			if (values == null) {
				throw new Exception("Can't parse values from config");
			}

			foreach (var i in values) {
				string key = i.Key;
				object value = i.Value;

				switch (key) {
				case KEY_SCREEN_WIDTH:
					ScreenWidth = (int)TryParseNumberValue(value, ScreenWidth);
					break;
				case KEY_SCREEN_HEIGHT:
					ScreenHeight = (int)TryParseNumberValue(value, ScreenHeight);
					break;
				case KEY_FULLSCREEN:
					Fullscreen = TryParseBooleanValue(value, Fullscreen);
					break;
				case KEY_DEBUG_MODE:
					DebugMode = TryParseBooleanValue(value, DebugMode);
					break;

				case KEY_GAME_SPLASH_TIME:
					GameSplashTime = TryParseNumberValue(value, GameSplashTime);
					break;
				case KEY_RACE_SPLASH_TIME:
					RaceSplashTime = TryParseNumberValue(value, RaceSplashTime);
					break;
				case KEY_RACE_RESULTS_TIME:
					RaceResultsTime = TryParseNumberValue(value, RaceResultsTime);
					break;
				case KEY_RACE_CAR_SPEED_BOT:
					RaceCarSpeedBot = TryParseNumberValue(value, RaceCarSpeedBot);
					break;
				case KEY_RACE_CAR_SPEED_PLAYER:
					RaceCarSpeedPlayer = TryParseNumberValue(value, RaceCarSpeedPlayer);
					break;
				case KEY_RACE_TUTORIAL_TIME:
					RaceTutorialTime = TryParseNumberValue(value, RaceTutorialTime);
					break;
				case KEY_PLAYER_INACTIVE_COOLDOWN:
					PlayerInactiveCooldown = TryParseNumberValue(value, PlayerInactiveCooldown);
					break;

				case KEY_RACE_LINE_1_COLOR_R:
					raceLine1Color.r = TryParseNumberValue(value, raceLine1Color.r);
					break;
				case KEY_RACE_LINE_1_COLOR_G:
					raceLine1Color.g = TryParseNumberValue(value, raceLine1Color.g);
					break;
				case KEY_RACE_LINE_1_COLOR_B:
					raceLine1Color.b = TryParseNumberValue(value, raceLine1Color.b);
					break;
				case KEY_RACE_LINE_1_COLOR_A:
					raceLine1Color.a = TryParseNumberValue(value, raceLine1Color.a);
					break;

				case KEY_RACE_LINE_2_COLOR_R:
					raceLine2Color.r = TryParseNumberValue(value, raceLine2Color.r);
					break;
				case KEY_RACE_LINE_2_COLOR_G:
					raceLine2Color.g = TryParseNumberValue(value, raceLine2Color.g);
					break;
				case KEY_RACE_LINE_2_COLOR_B:
					raceLine2Color.b = TryParseNumberValue(value, raceLine2Color.b);
					break;
				case KEY_RACE_LINE_2_COLOR_A:
					raceLine2Color.a = TryParseNumberValue(value, raceLine2Color.a);
					break;

				case KEY_RACE_LINE_TUTORIAL_COLOR_R:
					raceLineTutorialColor.r = TryParseNumberValue(value, raceLineTutorialColor.r);
					break;
				case KEY_RACE_LINE_TUTORIAL_COLOR_G:
					raceLineTutorialColor.g = TryParseNumberValue(value, raceLineTutorialColor.g);
					break;
				case KEY_RACE_LINE_TUTORIAL_COLOR_B:
					raceLineTutorialColor.b = TryParseNumberValue(value, raceLineTutorialColor.b);
					break;
				case KEY_RACE_LINE_TUTORIAL_COLOR_A:
					raceLineTutorialColor.a = TryParseNumberValue(value, raceLineTutorialColor.a);
					break;

				default:
					Debug.LogWarningFormat("Unknown config field: {0} with value {1}", key, value);
					break;
				}
			}
		} catch (IOException e) {
			Debug.LogErrorFormat("IO error while loading config: {0}", e.ToString());
		} catch (Exception e) {
			Debug.LogErrorFormat("Error while loading config: {0}", e.ToString());
		}
	}

	private static float TryParseNumberValue(object value, float defaultValue) {
		float parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		if (value is double || value is float || value is double) {
			parsedValue = (float)Convert.ToDouble(value);
		} else if (value is long || value is int || value is int) {
			parsedValue = Convert.ToInt32(value);
		} else if (value is string) {
			float.TryParse(value as string, out parsedValue);
		}

		return parsedValue;
	}

	private static bool TryParseBooleanValue(object value, bool defaultValue) {
		bool parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		if (value is bool || value is bool) {
			parsedValue = Convert.ToBoolean(value);
		} else if (value is string) {
			bool.TryParse(value as string, out parsedValue);
		}

		return parsedValue;
	}

	private static string TryParseStringValue(object value, string defaultValue) {
		string parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		parsedValue = value.ToString();

		return parsedValue;
	}
}
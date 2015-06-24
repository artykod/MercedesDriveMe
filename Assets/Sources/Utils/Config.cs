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


	public static int ScreenWidth { get; private set; }
	public static int ScreenHeight { get; private set; }
	public static bool Fullscreen { get; private set; }
	public static bool DebugMode { get; private set; }


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
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(CubicCurve))]
public class CubicCurveEditor : Editor {
	[MenuItem("Anim curves/Create new curve")]
	public static void CreateCurve() {
		CubicCurve asset = CreateInstance<CubicCurve>();
 
		string path = "";
		if (path == "")  {
			path = "Assets";
		} else if (Path.GetExtension(path) != "") {
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Animations/curves/" + "cubicCurve" + ".asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	private void OnEnable() {
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}

	private void OnDisable() {
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		//
	}

	private void OnSceneGUI(SceneView sceneView) {
		CubicCurve curve = target as CubicCurve;

		if (curve == null) {
			return;
		}

		bool needRebuild = false;

		// при нажатиях добавляются/удаляются контрольные точки
		if (Event.current.type == EventType.MouseDown) {
			Camera camera = SceneView.lastActiveSceneView.camera;
			Vector3 mpos = Event.current.mousePosition;
			Vector3 pos = camera.ScreenToWorldPoint(mpos);
			pos.z = 0f;
			float dif = camera.transform.position.y - pos.y;
			pos.y = camera.transform.position.y + dif;

			// поиск ближайшей точки к курсору мыши
			float minDist = float.MaxValue;
			int index = -1;
			for (int i = 0; i < curve.Dots.Count; i++) {
				Vector2 dot = curve.Dots[i];

				float dist = (pos - new Vector3(dot.x, dot.y)).magnitude;

				if (dist < minDist) {
					index = i;
					minDist = dist;
				}
			}

			if (Event.current.control) { // при зажатом ctrl добавляется точка
				if (index >= 0) { // добавление между точками, исходя из ближайшей к курсору мыши
					curve.Dots.Insert(index + 1, new Vector2(pos.x, pos.y));
				} else {
					curve.Dots.Add(new Vector2(pos.x, pos.y));
				}
			} else if (Event.current.alt) { // при зажатом альте удаляется ближайшая к мышке точка
				if (index >= 0) {
					curve.Dots.RemoveAt(index);
				}
			}

			needRebuild = true;
		}

		// отрисовка контролов для точек
		for (int i = 0; i < curve.Dots.Count; i++) {
			Vector2 dot = curve.Dots[i];
			curve.Dots[i] = Handles.PositionHandle(dot, Quaternion.identity);
			Handles.Label(curve.Dots[i], "p: " + i);

			if (dot.x != curve.Dots[i].x || dot.y != curve.Dots[i].y) {
				needRebuild = true;
			}

			if (Event.current.shift) {
				Vector2 dif = curve.Dots[i] - dot;

				for (int j = 0; j < curve.Dots.Count; j++) {
					if (i != j) {
						curve.Dots[j] += dif;
					}
				}
			}
		}

		if (needRebuild) {
			curve.Rebuild();
			EditorUtility.SetDirty(target);
		}

		// рисование самой кривой
		if (curve.Dots.Count > 0) {
			Vector3 a, b;
			a = curve.Dots[0];
			for (float key = 0f; key <= 1f; key += 0.01f) {
				b = curve.Evaluate(key);
				Handles.DrawLine(a, b);
				a = b;
			}
		}

		// блокировка нажатий в окне сцены пока выделен объект
		if (Selection.activeObject == curve) {
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		} else {
			HandleUtility.Repaint();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}

using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(CarVisual))]
public class CarVisualInspector : Editor {
	public override void OnInspectorGUI() {

		base.OnInspectorGUI();

		CarVisual carVisual = target as CarVisual;

		if (GUILayout.Button("Import frames")) {
			SerializedProperty frames = serializedObject.FindProperty("frames");
			Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(carVisual.SpritePathInAssets).OfType<Sprite>().ToArray();

			if (frames.isArray && sprites != null) {
				frames.ClearArray();
				for (int i = 0; i < sprites.Length; i++) {
					frames.InsertArrayElementAtIndex(i);
					frames.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
				}

				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
            }
		}
	}
}

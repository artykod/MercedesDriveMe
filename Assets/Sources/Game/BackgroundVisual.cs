using UnityEngine;

public class BackgroundVisual : MonoBehaviour {

	private MeshRenderer meshRenderer = null;
	private MovieTexture movie = null;

	private void Awake() {
		meshRenderer = GetComponent<MeshRenderer>();
		movie = meshRenderer.sharedMaterial != null ? meshRenderer.sharedMaterial.mainTexture as MovieTexture : null;

		movie.loop = true;
		movie.Play();
	}
}

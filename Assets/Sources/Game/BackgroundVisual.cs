using UnityEngine;
using System.Collections;

public class BackgroundVisual : MonoBehaviour {

	private class VisualAbstract {
		protected BackgroundVisual owner = null;
		protected MeshRenderer meshRenderer = null;
		protected System.Action onInitializedCallback = null;

		protected string StreamingAssetsUrl {
			get {
				string url = "";
#if UNITY_IPHONE && !UNITY_EDITOR
				url = "file:///" + Application.dataPath + "/Raw";
#elif UNITY_ANDROID && !UNITY_EDITOR
				url = "jar:file://" + Application.dataPath + "!/assets";
#else
				url = "file://" + Application.dataPath + "/StreamingAssets";
#endif
				return url;
			}
		}

		public virtual void Initialize(BackgroundVisual visualOwner, System.Action onInitialized) {
			owner = visualOwner;
			meshRenderer = visualOwner.GetComponent<MeshRenderer>();
			meshRenderer.enabled = false;
			onInitializedCallback = onInitialized;
		}

		public virtual void Play(bool loop) {
			// stub
		}

		public virtual void Destroy() {
			if (meshRenderer != null) {
				meshRenderer.enabled = false;
				Texture texture = meshRenderer.sharedMaterial.mainTexture;
				if (texture != null) {
					DestroyImmediate(texture, false);
				}
				meshRenderer.sharedMaterial.mainTexture = null;
			}
		}

		public void LoadBackground(string backgroundName) {
			Destroy();
			LoadBackgroundInternal(backgroundName);
		}

		protected virtual void LoadBackgroundInternal(string backgroundName) {

			string pathToMovies = StreamingAssetsUrl + "/Images/";

			owner.StartCoroutine(LoadMovie(pathToMovies + backgroundName + ".png", delegate (Texture2D loadedTexture, string error) {
				meshRenderer.sharedMaterial.mainTexture = loadedTexture;
				meshRenderer.enabled = true;
				onInitializedCallback();
			}));
		}

		private IEnumerator LoadMovie(string path, System.Action<Texture2D, string> receiver) {
			using (WWW loader = new WWW(path)) {
				while (!loader.isDone) {
					yield return loader;
				}

				Texture2D texture = null;
				string error = loader.error;

				if (string.IsNullOrEmpty(error)) {
					texture = loader.texture;
					texture.name = "fakeMovieTexture";
				} else {
					Debug.LogErrorFormat("WWW loading failed: file = {0} error = {1}", path, error);
				}

				receiver.Invoke(texture, error);
			}
		}
	}

#if !UNITY_ANDROID && !UNITY_IPHONE
	private class VisualVideo : VisualAbstract {
		private MovieTexture movie = null;

		protected override void LoadBackgroundInternal(string backgroundName) {

			string pathToMovies = StreamingAssetsUrl + "/Movies/";

			owner.StartCoroutine(LoadMovie(pathToMovies + backgroundName + ".ogv", delegate(MovieTexture loadedMovie, string error) {
				movie = loadedMovie;
                meshRenderer.sharedMaterial.mainTexture = movie;
				meshRenderer.enabled = true;
				onInitializedCallback();
            }));
		}

		public override void Play(bool loop) {
			base.Play(loop);

			if (movie != null) {
				movie.loop = loop;
				movie.Play();
			}
		}

		private IEnumerator LoadMovie(string path, System.Action<MovieTexture, string> receiver) {
			using (WWW loader = new WWW(path)) {
				while (!loader.isDone) {
					yield return loader;
				}

				MovieTexture movie = null;
				string error = loader.error;

				if (string.IsNullOrEmpty(error)) {
					movie = loader.movie;
					movie.name = "movieTexture";

					while (!movie.isReadyToPlay) {
						yield return null;
					}
				}

				receiver.Invoke(movie, error);
			}
		}
	}
#endif

	private VisualAbstract visual = null;

	private void Awake() {
#if !UNITY_ANDROID && !UNITY_IPHONE
		visual = new VisualVideo();
#else
		visual = new VisualAbstract();
#endif

		visual.Initialize(this, PlayLoadedBackground);
	}

	private void PlayLoadedBackground() {
		visual.Play(true);
	}

	public void LoadBackground(string backgroundName) {
		visual.LoadBackground(backgroundName);
	}
}

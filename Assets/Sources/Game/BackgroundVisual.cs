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
			onInitializedCallback = onInitialized;

			InitializeInternal();
        }

		public virtual void Play(bool loop) {
			// stub
		}

		public virtual void Destroy() {
			meshRenderer.sharedMaterial.mainTexture = null;
		}

		protected virtual void InitializeInternal() {
			string pathToMovies = StreamingAssetsUrl + "/Images/";

			owner.StartCoroutine(LoadMovie(pathToMovies + "level_1.png", delegate (Texture2D loadedTexture, string error) {
				meshRenderer.sharedMaterial.mainTexture = loadedTexture;
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

		protected override void InitializeInternal() {
			string pathToMovies = StreamingAssetsUrl + "/Movies/";

			owner.StartCoroutine(LoadMovie(pathToMovies + "level_1.ogv", delegate(MovieTexture loadedMovie, string error) {
				movie = loadedMovie;
                meshRenderer.sharedMaterial.mainTexture = movie;
				onInitializedCallback();
            }));
		}

		public override void Play(bool loop) {
			base.Play(loop);
			movie.loop = loop;
			movie.Play();
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

		visual.Initialize(this, delegate {
			visual.Play(true);
		});
	}
}

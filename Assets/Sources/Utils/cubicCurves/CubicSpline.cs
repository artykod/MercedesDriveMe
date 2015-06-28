public class CubicSpline {
	private struct SplineTuple {
		public float a;
		public float b;
		public float c;
		public float d;
		public float x;
	}

	private const float DIV6 = 1f / 6f;

	private SplineTuple[] splines = null;
	
	public CubicSpline() {
		splines = null;
	}

	public CubicSpline(float[] key, float[] value) {
		BuildSpline(key, value);
	}

	public void BuildSpline(float[] key, float[] value) {
		int n = key.Length < value.Length ? key.Length : value.Length;
		int l = n - 1;

		splines = new SplineTuple[n];

		for (int i = 0; i < n; ++i) {
			splines[i].x = key[i];
			splines[i].a = value[i];
		}

		splines[0].c = splines[l].c = 0f;

		float[] alpha = new float[l];
		float[] beta = new float[l];

		alpha[0] = beta[0] = 0f;

		for (int i = 1; i < l; ++i) {
			float h_i = key[i] - key[i - 1];
			float h_i1 = key[i + 1] - key[i];
			float A = h_i;
			float C = 2f * (h_i + h_i1);
			float B = h_i1;
			float F = 6f * ((value[i + 1] - value[i]) / h_i1 - (value[i] - value[i - 1]) / h_i);
			float z = 1f / (A * alpha[i - 1] + C);

			alpha[i] = -B * z;
			beta[i] = (F - A * beta[i - 1]) * z;
		}

		for (int i = n - 2; i > 0; --i) {
			splines[i].c = alpha[i] * splines[i + 1].c + beta[i];
		}

		beta = null;
		alpha = null;

		for (int i = l; i > 0; --i) {
			float h_i = key[i] - key[i - 1];

			splines[i].d = (splines[i].c - splines[i - 1].c) / h_i;
			splines[i].b = h_i * (2f * splines[i].c + splines[i - 1].c) / 6f + (value[i] - value[i - 1]) / h_i;
		}

		for (int i = 0; i < n; ++i) {
			splines[i].c *= 0.5f;
		}
	}

	public float GetValue(float key) {
		if  (splines == null) {
			return 0f;
		}
	
		int n = splines.Length;
		SplineTuple s;

		if  (n < 2) {
			return 0f;
		}
		
		if (key <= splines[0].x) {
			s = splines[1];
		} else if (key >= splines[n - 1].x) {
			s = splines[n - 1];
		} else {
			int i = 0, j = n - 1;

			while (i + 1 < j) {
				int k = i + (j - i) / 2;

				if (key <= splines[k].x) {
					j = k;
				} else {
					i = k;
				}
			}

			s = splines[j];
		}
		
		float dx = (key - s.x);

		return s.a + (s.b + (s.c + s.d * dx * DIV6) * dx) * dx; 
	}
}
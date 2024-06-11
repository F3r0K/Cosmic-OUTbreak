using UnityEngine;
using System.Collections;

namespace CompassNavigatorPro
{
	public class BeaconAnimator : MonoBehaviour
	{

		public float intensity = 5f;
		public float duration;
		public Color tintColor;

		float startingTime;
		Material mat;
		Color fullyTransparentColor, originalColor;

		// Use this for initialization
		void Awake ()
		{
			mat = GetComponent<Renderer>().material;
			fullyTransparentColor = new Color(0,0,0,0);
			duration = 1f;
		}

		void Start() {
			startingTime = Time.time;
			originalColor = mat.color * tintColor * intensity;
			mat.SetColor ("_EmissionColor", tintColor);
			UpdateColor ();
		}

		void OnDisable() {
			DestroyBeacon();
		}
	
		// Update is called once per frame
		void Update ()
		{
			mat.mainTextureOffset = new Vector2(Time.time * -0.25f, Time.time * -0.25f);
			mat.SetTextureOffset ("_EmissionMap", new Vector2 (Time.time * -0.15f, Time.time * -0.2f));
			UpdateColor();
		}

		void UpdateColor() {
			float elapsed = duration<=0 ? 1f: Mathf.Clamp01( (Time.time - startingTime) / duration );
			if (elapsed>=1f) {
				DestroyBeacon();
				return;
			}
			float t =  Ease(elapsed);
			mat.color = Color.Lerp(fullyTransparentColor, originalColor, t);
		}

		float Ease(float t) {
			return Mathf.Sin (t * Mathf.PI);
		}


		void DestroyBeacon() {
			if (mat!=null) {
				DestroyImmediate(mat);
				mat = null;
			}
			if (Application.isPlaying) Destroy(gameObject);
		}
	}

}
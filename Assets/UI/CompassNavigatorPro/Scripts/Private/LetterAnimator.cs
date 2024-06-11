using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CompassNavigatorPro {

    public delegate void OnAnimationEndDelegate(int poolIndex);

    public class LetterAnimator : MonoBehaviour {

		public float startTime, revealDuration, startFadeTime, fadeDuration;
		public Text text, textShadow;
        public int poolIndex;
        public OnAnimationEndDelegate OnAnimationEnds;
        public bool used;

		Vector3 localScale;

		public void Play () {
			localScale = text.transform.localScale;
			Update ();
		}

		// Update is called once per frame
		void Update () {

			float now = Time.time;
			float elapsed = now - startTime;
			if (elapsed < revealDuration) { // revealing
				float t = Mathf.Clamp01 (elapsed / revealDuration);
				UpdateTextScale (t);
				UpdateTextAlpha (t);
			} else if (now < startFadeTime) {
				UpdateTextScale (1.0f);
				UpdateTextAlpha (1.0f);
			} else if (now < startFadeTime + fadeDuration) {
				float t = Mathf.Clamp01 (1.0f - (now - startFadeTime) / fadeDuration);
				UpdateTextAlpha (t);
			} else {
                OnAnimationEnds(poolIndex);
                enabled = false;
			}
		}

		void UpdateTextScale (float t) {
			text.transform.localScale = localScale * t;
			textShadow.transform.localScale = localScale * t;
		}

		void UpdateTextAlpha (float t) {
			text.color = new Color (text.color.r, text.color.g, text.color.b, t);
			textShadow.color = new Color (0, 0, 0, t);
		}



	}

}
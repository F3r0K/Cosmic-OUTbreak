using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace CompassNavigatorPro {
	public class CompassButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public UnityAction actionHandler;
		Coroutine co;

		public void OnPointerDown (PointerEventData eventData) {
			if (co != null) {
				StopCoroutine (co);
			}
			co = StartCoroutine (ExecuteHandler ());
		}


		public void OnPointerUp (PointerEventData eventData) {
			if (co != null) {
				StopCoroutine (co);
			}
		}

		IEnumerator ExecuteHandler () {
			WaitForEndOfFrame w = new WaitForEndOfFrame ();
			while (true) {
				actionHandler ();
				yield return w;
			}
		}


	}
}
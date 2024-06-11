using UnityEngine;
using CompassNavigatorPro;

namespace CompassNavigatorProDemos {
    public class DiscoverArea : MonoBehaviour {

        void OnTriggerEnter(Collider col) {
            if (col.tag == "MainCamera") {
                gameObject.SetActive(false);
                CompassPro.instance.ShowAnimatedText("Area Discovered!");
            }
        }
    }

}
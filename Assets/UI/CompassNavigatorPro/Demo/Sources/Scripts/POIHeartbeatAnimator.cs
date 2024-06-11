using UnityEngine;
using CompassNavigatorPro;

namespace CompassNavigatorProDemos {
    public class POIHeartbeatAnimator : MonoBehaviour {

        CompassProPOI poi;
        Rigidbody rb;

        // Create random POIs
        void Start() {
            poi = GetComponent<CompassProPOI>();
            poi.OnHeartbeat += OnHeartbeatHandler;
            rb = GetComponent<Rigidbody>();
        }

        void OnHeartbeatHandler() {
            if (rb.IsSleeping()) {
                rb.AddForce(Vector3.up * (100f + Random.value * 100f));
                rb.AddTorque(Random.onUnitSphere * 10f);
            }
        }


    }
}
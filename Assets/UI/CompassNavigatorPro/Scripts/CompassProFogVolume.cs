using System;
using System.Collections;
using UnityEngine;

namespace CompassNavigatorPro {

    [ExecuteInEditMode]
    public class CompassProFogVolume : MonoBehaviour {

        [Range(0, 1), Tooltip("Transparency of the fog of war. A value of 1 means fully opaque fog.")]
        public float alpha;

        [Range(0, 1), Tooltip("Controls the hardness of the border.")]
        public float border;

        [Tooltip("Fog volumes are rendered in ascending order.")]
        public int order;

        Vector3 oldPos, oldScale;
        float oldAlpha, oldBorder;
        int oldOrder;

        void OnEnable() {
            if (!Application.isPlaying) {
                ShowFogArea(true);
            }
        }

        void Start() {
            if (Application.isPlaying) {
                ShowFogArea(true);
            }
        }

        void OnDisable() {
            ShowFogArea(false);
        }

        void Update() {
            if (order != oldOrder || alpha != oldAlpha || border != oldBorder || transform.position != oldPos || transform.localScale != oldScale) {
                NotifyChanges();
            }
        }

        void NotifyChanges() {
            oldPos = transform.position;
            oldScale = transform.localScale;
            oldAlpha = alpha;
            oldBorder = border;
            oldOrder = order;
            CompassPro compass = CompassPro.instance;
            if (compass != null) {
                compass.UpdateFogOfWar();
            }
        }

        void ShowFogArea(bool state) {
            CompassPro compass = CompassPro.instance;
            if (compass != null) {
                Bounds bounds = GetComponent<BoxCollider>().bounds;
                float alpha = state ? this.alpha : 0f;
                compass.SetFogOfWarAlpha(bounds, alpha, border);
            }
        }


    }

}
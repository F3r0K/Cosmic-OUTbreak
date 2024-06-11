using System;
using System.Collections;
using UnityEngine;

namespace CompassNavigatorPro {

    public enum POI_VISIBILITY {
        WhenInRange = 0,
        AlwaysVisible = 1,
        AlwaysHidden = 2
    }

    public enum TITLE_VISIBILITY {
        OnlyWhenVisited = 0,
        Always = 1
    }

    public delegate void OnHeartbeatEvent();

    [AddComponentMenu("Compass Navigator Pro/Compass POI")]
    [ExecuteInEditMode]
    public class CompassProPOI : MonoBehaviour {

        [Tooltip("Unique ID to be used when DontDestroyOnLoad option is set to true.")]
        public int id;

        [Tooltip("Higher priority icons get rendered on top of others")]
        public int priority;

        [Header("Compass Bar Settings")]
        [Tooltip("POI visibility in compass bar.")]
        public POI_VISIBILITY visibility = POI_VISIBILITY.WhenInRange;

        [Tooltip("A value of 0 uses the global visible distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI. Useful when you need this POI to use a different visible distance.")]
        public float visibleDistanceOverride;

        [Tooltip("A value of 0 uses the global visible min distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI.")]
        public float visibleMinDistanceOverride;

        [Tooltip("Minimum distance to show as text under the compass bar. Distance text won't be shown for objects within this distance.")]
        public float minDistanceText = 10;

        [Tooltip("Specifies if this POI is visible in the compass bar.")]
        [NonSerialized]
        public bool isVisible;

        [Tooltip("Title to be shown when this POI is in the center of the compass bar and it's a known location (isVisited = true)")]
        public string title;

        [Tooltip("Rule for title visibility.")]
        public TITLE_VISIBILITY titleVisibility = TITLE_VISIBILITY.OnlyWhenVisited;

        [Tooltip("Specifies if this POI can be marked as visited when reached.")]
        public bool canBeVisited = true;

        [Tooltip("A value of 0 uses the global visited distance property from the Compass Bar settings. A value greater than 0 will override the global value for this POI. Useful when you need this POI to use a different visited distance.")]
        public float visitedDistanceOverride;

        [Tooltip("Specifies if POI must be removed from compass bar when visited.")]
        public bool hideWhenVisited;

        [Tooltip("Specifies if this POI has been already visited.")]
        public bool isVisited;

        [Tooltip("Text to show when discovered. Leave this to null if you don't want to show any text.")]
        public string visitedText;

        [Tooltip("Sound to play when POI is visited the first time.")]
        public AudioClip visitedAudioClip;

        [Tooltip("Radius of activity of this POI. Useful for area POIs.")]
        public float radius;

        [Tooltip("User defined icon scale multiplier.")]
        public float iconScale = 1f;

        [Tooltip("The icon for the POI if has not been discovered/visited.")]
        public Sprite iconNonVisited;

        [Tooltip("The icon for the POI if has been visited.")]
        public Sprite iconVisited;

        [Tooltip("Tinting color")]
        public Color tintColor = Color.white;

        [Tooltip("If the icon will be shown in the scene during playmode. If enabled, the icon will fade in smoothly as the player approaches it.")]
        public bool showPlayModeGizmo;

        [Tooltip("If enabled, the icon will stop at the edges of the bar even if it's behind the player.")]
        public bool clampPosition;

        [Tooltip("Sound to play when beacon is shown.")]
        public AudioClip beaconAudioClip;

        [Tooltip("Preserves the state of this POI between scene changes. Note that this POI only will be visible in the scene where it was first created.")]
        public bool dontDestroyOnLoad;

        [Tooltip("Enables heartbeat effect. Plays a sound with variable speed when approaching this POI.")]
        public bool heartbeatEnabled;

        [Tooltip("Sound to play when heartbeat effect is enabled.")]
        public AudioClip heartbeatAudioClip;

        [Tooltip("Distance to start playing heartbeat effect is enabled.")]
        public float heartbeatDistance = 20f;

        [Tooltip("Interval of heartbeat rate based on distance.")]
        public AnimationCurve heartbeatInterval = AnimationCurve.Linear(0, 0.25f, 1f, 3f);

        [Header("Mini-Map Settings")]
        [Tooltip("POI visibility on the mini-map.")]
        public POI_VISIBILITY miniMapVisibility = POI_VISIBILITY.WhenInRange;

        [Tooltip("Specifies if this POI is visible on the mini-map.")]
        [NonSerialized]
        public bool miniMapIsVisible;

        [Tooltip("If enabled, the icon will stop at the edges of the mini-map even if it's behind the player.")]
        public bool miniMapClampPosition;

        [Tooltip("If enabled, the minimap icon will be rotated according to the POI rotation.")]
        public bool miniMapShowRotation;

        [Tooltip("Custom angle rotation adjustment.")]
        public float miniMapRotationAngleOffset;

        [Tooltip("Icon scale on the mini-map.")]
        public float miniMapIconScale = 1f;

        public OnHeartbeatEvent OnHeartbeat;

        [HideInInspector]
        public float distanceToCameraSQR;

        [HideInInspector]
        public Vector3 computedIconScale;

        [HideInInspector]
        public float iconAlpha;

        /// <summary>
        /// In-scene gizmo
        /// </summary>
        [NonSerialized]
        public SpriteRenderer spriteRenderer;

        /// <summary>
        /// Reference to the icon gameobject on the compass bar when it's created
        /// </summary>
        public GameObject compassIconGameObject {
            get {
                if (compassIconRectTransform != null) {
                    return compassIconRectTransform.gameObject;
                }
                return null;
            }
        }

        /// <summary>
        /// Reference to the icon gameobject on the minimap when it's created
        /// </summary>
        public GameObject miniMapIconGameObject {
            get {
                if (miniMapIconRectTransform != null) {
                    return miniMapIconRectTransform.gameObject;
                }
                return null;
            }
        }


        [NonSerialized]
        public RectTransform compassIconRectTransform, miniMapIconRectTransform;

        /// <summary>
        /// Reference to the compass script
        /// </summary>
        [NonSerialized]
        public CompassPro compass;

        /// <summary>
        /// Time when the poi appeared on the compass bar
        /// </summary>
        [NonSerialized]
        public float visibleTime;

        [NonSerialized]
        public bool heartbeatIsActive;

        Coroutine heartbeatPlayer;


        [HideInInspector]
        public Vector3 miniMapCurrentIconScale;



        void OnEnable() {
            CompassPro compass = CompassPro.instance;
            if (!Application.isPlaying && compass != null && compass.POIisRegistered(this)) {
                id = 0;
            }
            if (id == 0) {
                id = System.Guid.NewGuid().GetHashCode();
            }
            RegisterPOI();
        }

        // Use this for initialization
        void Start() {
            RegisterPOI();
        }

        public void RegisterPOI() {
            CompassPro compass = CompassPro.instance;
            if (compass == null)
                return;

            if (dontDestroyOnLoad && Application.isPlaying) {
                if (compass.POIisRegistered(this)) {
                    Destroy(gameObject);
                    return;
                }
                DontDestroyOnLoad(gameObject);
            }

            compass.POIRegister(this);
        }


        private void OnDisable() {
            CompassPro compass = CompassPro.instance;
            if (compass == null)
                return;

            compass.POIUnregister(this);
        }

        void OnDrawGizmos() {
            Gizmos.DrawIcon(transform.position, "compassIcon.png", true);
        }


        public void StartHeartbeat() {
            if (isVisited)
                return;
            heartbeatPlayer = StartCoroutine(HeartBeatPlayer());
            heartbeatIsActive = true;
        }


        public void StopHeartbeat() {
            if (heartbeatPlayer != null)
                StopCoroutine(heartbeatPlayer);
            heartbeatIsActive = false;
        }


        IEnumerator HeartBeatPlayer() {
            AudioClip heartbeatSound = heartbeatAudioClip != null ? heartbeatAudioClip : CompassPro.instance.heartbeatDefaultAudioClip;
            if (heartbeatSound == null) {
                Debug.LogWarning("Compass POI: heartbeat sound not set.");
                yield break;
            }
            heartbeatDistance = Mathf.Max(1f, heartbeatDistance);
            float minDistance = CompassPro.instance.visitedDistance;
            while (true) {
                Vector3 camPos = CompassPro.instance.cameraMain.transform.position;
                float distance = Vector3.Distance(camPos, transform.position);
                if (distance > heartbeatDistance || isVisited) {
                    heartbeatIsActive = false;
                    yield break;
                }
                if (distance < minDistance)
                    distance = minDistance;
                AudioSource.PlayClipAtPoint(heartbeatSound, camPos);
                if (OnHeartbeat != null)
                    OnHeartbeat();
                float curvePos = (distance - minDistance) / heartbeatDistance;
                float delay = heartbeatInterval.Evaluate(curvePos);
                yield return new WaitForSeconds(delay);
            }
        }

        /// <summary>
        /// Gets the screen rectangle of the icon in the compass bar
        /// </summary>
        /// <returns>The compass bar icon screen rect.</returns>
        public Rect GetCompassIconScreenRect() {
            if (isVisible && compassIconRectTransform != null && compass != null) {
                Vector3 pos = compassIconRectTransform.transform.position;
                Vector3 size = compassIconRectTransform.sizeDelta;
                return new Rect(pos.x - size.x * 0.5f, Screen.height - pos.y - size.y * 0.5f, size.x, size.y);
            }
            return new Rect(0, 0, 0, 0);
        }

        /// <summary>
        /// Gets the screen rectangle of the mini-map icon
        /// </summary>
        /// <returns>The mini map icon screen rect.</returns>
        public Rect GetMiniMapIconScreenRect() {
            if (miniMapIsVisible && miniMapIconRectTransform != null && compass != null) {
                return miniMapIconRectTransform.GetScreenRect();
            }
            return new Rect(0, 0, 0, 0);
        }

    }

}
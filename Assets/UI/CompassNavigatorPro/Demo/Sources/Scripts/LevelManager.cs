using UnityEngine;
using System.Collections;
using CompassNavigatorPro;
using System.Collections.Generic;

namespace CompassNavigatorProDemos {
	public class LevelManager : MonoBehaviour {

		public int initialPoiCount = 1;
		public Material sphereMaterial;
		public Sprite[] icons;
		public AudioClip[] soundClips;

		int poiNumber;
		CompassPro compass;
		CompassProPOI POIUnderMouse;

		void Start () {
			// Get a reference to the Compass Pro Navigator component
			compass = CompassPro.instance;

			// Add a callback when POIs are reached
			compass.OnPOIVisited += POIVisited;

			compass.OnPOIMiniMapIconMouseEnter += POIHover;
			compass.OnPOIMiniMapIconMouseExit += POIExit;
			compass.OnPOIMiniMapIconMouseClick += POIClicked;
			compass.OnMiniMapMouseClick += MiniMapClicked;

			// Populate the scene with initial POIs
			for (int k = 1; k <= initialPoiCount; k++) {
				AddRandomPOI ();
			}
		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.B)) {
				compass.POIShowBeacon (5f, 1.1f, 1f, new Color (1, 1, 0.25f));
			}
			if (Input.GetKey (KeyCode.Z)) {
				compass.miniMapZoomLevel -= Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.X)) {
				compass.miniMapZoomLevel += Time.deltaTime;
			}
			if (Input.GetKeyDown (KeyCode.M)) {
				compass.showMiniMap = !compass.showMiniMap;
			}
			if (Input.GetKeyDown (KeyCode.C)) {
				Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f));
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					compass.POIShowBeacon (hit.point, 5f, 1.1f, 1f, new Color (0, 0.5f, 1f)); 
				}
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				compass.miniMapZoomState = !compass.miniMapZoomState;
			}
		}

		void OnGUI () {
			if (POIUnderMouse != null) {
				Rect rect = POIUnderMouse.GetMiniMapIconScreenRect ();
				rect = new Rect (rect.center.x - 100, Screen.height - rect.y - 65, 200, 25);
				GUIStyle style = GUI.skin.GetStyle("Label");
				style.alignment = TextAnchor.UpperCenter;
				style.normal.textColor = Color.yellow;
				GUI.Label (rect, POIUnderMouse.title, style);
			}
		}

		void AddRandomPOI () {
			Vector3 position = new Vector3 (Random.Range (-50, 50), 1, Random.Range (-50, 50));
			AddPOI(position);
		}


        void AddPOI(Vector3 position) {
            // Create placeholder
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = position;
            obj.GetComponent<Renderer>().material = sphereMaterial;

            // Add POI info
            CompassProPOI poi = obj.AddComponent<CompassProPOI>();

            // Title name and reveal text
            poi.title = "Target " + (++poiNumber).ToString();
            poi.titleVisibility = TITLE_VISIBILITY.Always;
            poi.visitedText = "Target " + poiNumber + " acquired!";

            // Assign icons
            int j = Random.Range(0, icons.Length / 2);
            poi.iconNonVisited = icons[j * 2];
            poi.iconVisited = icons[j * 2 + 1];

            // Enable GameView gizmo
            poi.showPlayModeGizmo = true;

            // Assign random sound
            j = Random.Range(0, soundClips.Length);
            poi.visitedAudioClip = soundClips[j];
        }

        void POIVisited (CompassProPOI poi) {
			Debug.Log (poi.title + " has been reached.");
			StartCoroutine (RemovePOI (poi));
		}

		IEnumerator RemovePOI (CompassProPOI poi) {
			while (poi.transform.position.y < 5) {
				poi.transform.position += Vector3.up * Time.deltaTime;
				poi.transform.localScale *= 0.9f;
				yield return new WaitForEndOfFrame ();
			}
			Destroy (poi.gameObject);
		}

		void POIClicked (CompassProPOI poi) {
			Debug.Log (poi.title + " has been clicked on minimap.");
		}

		void POIHover (CompassProPOI poi) {
			POIUnderMouse = poi;
		}

		void POIExit (CompassProPOI poi) {
			POIUnderMouse = null;
		}

        void MiniMapClicked(Vector3 position) {
			Debug.Log("User clicked on mini-map. Creating a POI at world position: " + position);
			position.y = 1f;
			AddPOI(position);
        }

    }
}
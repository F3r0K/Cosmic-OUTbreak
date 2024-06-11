#if GAIA_PRESENT && UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using CompassNavigatorPro;
using UnityEditor;

namespace Gaia.GX.Kronnect
{
    /// <summary>
    /// Compass Navigator Pro extension for Gaia.
    /// </summary>
	public class CompassNavigatorProGaiaExtension : MonoBehaviour
    {
        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Kronnect";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Compass Navigator Pro";
        }

        #endregion

        #region Methods exposed by Gaia as buttons must be prefixed with GX_

        public static void GX_About()
        {
			EditorUtility.DisplayDialog("About Compass Navigator Pro", "Compass Navigator Pro is a professional GUI Scripting component for Unity useful in any kind of world exploration game. This is a rich and customizable compass bar (plus an art package and sound clips), usually shown on top of the screen in many RPG games, showing destinations and point of interests (POIs) as well as text indications.", "OK");
        }

		public static void GX_AddCompassBar() {
			if (CompassPro.instance != null) return;

			GameObject cnpRef = Resources.Load<GameObject> ("CNPro/Prefabs/CompassNavigatorPro");
			if (cnpRef == null) {
				Debug.LogError ("Could not load CompassNavigatorPro from Resources/Prefabs folder!");
				return;
			}
			GameObject cnp = Instantiate (cnpRef);
			cnp.name = "CompassNavigatorPro";
		}

		public static void GX_PopulateCompassBar() {
			// Wise code here to add interesting places to the compass bar.
			// To do this, we need a list of GameObjects to which attach the CompassProPOI script and set some properties.

			Sprite iconGenericBlack = Resources.Load<Sprite>("CNPro/Sprites/icon-arrow-black");
			Sprite iconGenericWhite = Resources.Load<Sprite>("CNPro/Sprites/icon-arrow-white");
			Sprite iconMonolithBlack = Resources.Load<Sprite>("CNPro/Sprites/icon-monolith-black");
			Sprite iconMonolithWhite = Resources.Load<Sprite>("CNPro/Sprites/icon-monolith-white");
			Sprite iconMineBlack = Resources.Load<Sprite>("CNPro/Sprites/icon-mine-black");
			Sprite iconMineWhite = Resources.Load<Sprite>("CNPro/Sprites/icon-mine-white");
			Sprite iconCityBlack = Resources.Load<Sprite>("CNPro/Sprites/icon-city-black");
			Sprite iconCityWhite = Resources.Load<Sprite>("CNPro/Sprites/icon-city-white");
			Sprite iconForestBlack = Resources.Load<Sprite>("CNPro/Sprites/icon-forest-black");
			Sprite iconForestWhite = Resources.Load<Sprite>("CNPro/Sprites/icon-forest-white");

			GameObject spawnedRoot = GameObject.Find ("Spawned_GameObjects");
			if (spawnedRoot==null) {
				Debug.Log("Spawned_GameObjects not found in the scene. Have you used Gaia game object spawner?");
				return;
			}

			bool refresh = EditorUtility.DisplayDialog("Populate Compass Bar", "This option will add spawned game objects created with Gaia to the Compass Bar.\nA CompassPro POI game object will be added to the center of each spawned group with an automated setup (an icon will be added to rocks, villages and forests, and the radius of the area will automatically estimated).\n\nYou may just add new spawned objects to the compass bar or refresh all of them (if this is the first time you use this option, click on any button).", "Refresh all spawned objects", "Just add new stuff to the compass bar");

			Terrain terrain = GetActiveTerrain();

			for (int k=0;k<spawnedRoot.transform.childCount;k++) {
				Transform spawned = spawnedRoot.transform.GetChild(k);
				if (spawned==null) continue;

				// Get group renderers
				Renderer[] renderers = spawned.GetComponentsInChildren<Renderer> ();
				int renderersCount = renderers.Length;
				if (renderersCount == 0) continue;

				// Check existing POI for this spawned group
				bool newPOI = false;
				Transform t = spawned.Find("CompassPOI");
				if (t==null) {
					// Create POI game object
					GameObject poiGO = new GameObject();
					poiGO.name = "CompassPOI";
					t = poiGO.transform;
					t.SetParent(spawned.transform, false);
					t.localPosition = Misc.Vector3zero;
					newPOI = true;
				}
				CompassProPOI poi = t.GetComponent<CompassProPOI>();
				if (poi==null) {
					newPOI = true;
					poi = t.gameObject.AddComponent<CompassProPOI>();
				}

				if (!newPOI && !refresh) continue;	// already has POI configured

				poi.title = spawned.name;
				string uname = spawned.name.ToUpper();
				poi.visitedText = uname + " DISCOVERED";
				// try to find a suitable icon
				Sprite iconNonVisited = null, iconVisited = null;

				if (uname.Contains("ROCK")) {
					iconNonVisited = iconMonolithBlack;
					iconVisited = iconMonolithWhite;
				} else if (uname.Contains("FARM")) {
					iconNonVisited = iconMineBlack;
					iconVisited = iconMineWhite;
				} else if (uname.Contains("VILLAGE") || uname.Contains("TOWN") || uname.Contains("CITY")) {
					iconNonVisited = iconCityBlack;
					iconVisited = iconCityWhite;
				} else if (uname.Contains("COPSE") || uname.Contains("TREE") || uname.Contains("FOREST")) {
					iconNonVisited = iconForestBlack;
					iconVisited = iconForestWhite;
				} else {
					iconNonVisited = iconGenericBlack;
					iconVisited = iconGenericWhite;
				}

				poi.iconNonVisited = iconNonVisited;
				poi.iconVisited = iconVisited; 

				// try to find a suitable radius for this group
				Vector3 poiCenter = Misc.Vector3zero;
				Bounds bounds = new Bounds(spawned.transform.position, Misc.Vector3zero);
				for (int r=0;r<renderersCount;r++) {
					Renderer renderer = renderers[k];
					if (r==0) {
						bounds = new Bounds (renderer.transform.position, renderer.bounds.size);
					} else {
						if (poiCenter==Misc.Vector3zero) {
							poiCenter = renderer.transform.position;
						} else {
							poiCenter = (poiCenter + renderer.transform.position) * 0.5f;
						}
						bounds.Encapsulate (renderer.bounds);
					}
				}

				if (terrain!=null) {
					float h = terrain.transform.position.y + terrain.terrainData.GetInterpolatedHeight( (poiCenter.x - terrain.transform.position.x) / terrain.terrainData.size.x, (poiCenter.z - terrain.transform.position.z) / terrain.terrainData.size.z);
					if (h > poiCenter.y) poiCenter.y = h;
				}
				poi.transform.position = poiCenter;
				poi.radius = Mathf.Max (bounds.size.x, bounds.size.z) * 1.15f + 5f;

				if (newPOI) {
					Debug.Log (poi.title + " added to compass bar.");
				} else {
					Debug.Log (poi.title + " has been refreshed.");
				}
			}

		}

		public static void GX_CleanCompassBar() {
			if (CompassPro.instance == null) {
				Debug.LogWarning ("CompassNavigatorPro does not exist.");
				return;
			}

			bool ok = EditorUtility.DisplayDialog("Clean Compass Bar", "This option will remove all POIs added to spawned game objects created with Gaia.\n\nContinue? (this can't be undone!)", "Remove spawned POIs", "Cancel"); 
			if (!ok) return;

			GameObject spawnedRoot = GameObject.Find ("Spawned_GameObjects");
			if (spawnedRoot==null) return;

			CompassProPOI[] pois = spawnedRoot.GetComponentsInChildren<CompassProPOI>();
			int deleted = 0;
			for (int k=0;k<pois.Length;k++) {
				if (pois[k]!=null) {
					GameObject.DestroyImmediate(pois[k].gameObject);
					deleted ++;
				}
			}

			Debug.Log(deleted + " POIs removed.");
		}


		/// <summary>
		/// Get the currently active terrain - or any terrain
		/// </summary>
		/// <returns>A terrain if there is one</returns>
		public static Terrain GetActiveTerrain()
		{
			//Grab active terrain if we can
			Terrain terrain = Terrain.activeTerrain;
			if (terrain != null && terrain.isActiveAndEnabled)
			{
				return terrain;
			}
			
			//Then check rest of terrains
			for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
			{
				terrain = Terrain.activeTerrains[idx];
				if (terrain != null && terrain.isActiveAndEnabled)
				{
					return terrain;
				}
			}
			return null;
		}


		#endregion
     
    }
}

#endif
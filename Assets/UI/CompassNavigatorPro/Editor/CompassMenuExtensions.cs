using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CompassNavigatorPro {
	public class CompassMenuExtensions : MonoBehaviour {
		
		[MenuItem("GameObject/Create Other/Compass Navigator Pro/POI")]
		static void CreateCompassPOI (MenuCommand menuCommand) {
			GameObject poi = Resources.Load<GameObject> ("CNPro/Prefabs/CompassPOI");
			if (poi == null) {
				Debug.LogError ("Could not load CompassPOI from Resources/CNPro/Prefabs folder!");
				return;
			}
			GameObject newPOI = Instantiate (poi);
			newPOI.name = "Compass POI";
			
			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign (newPOI, menuCommand.context as GameObject);
			
			// Register root object for undo.
			Undo.RegisterCreatedObjectUndo (newPOI, "Create Compass POI");
			Selection.activeObject = newPOI;
		}



		[MenuItem("GameObject/Create Other/Compass Navigator Pro/Mini-Map Fog Of War Volume")]
		static void CreateFogOfWarVolume (MenuCommand menuCommand) {
			GameObject fv = Resources.Load<GameObject> ("CNPro/Prefabs/FogOfWarVolume");
			if (fv == null) {
				Debug.LogError ("Could not load FogOfWarVolume from Resources/CNPro/Prefabs folder!");
				return;
			}
			GameObject newPOI = Instantiate (fv);
			newPOI.name = "Mini-Map Fog Of War Volume";

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign (newPOI, menuCommand.context as GameObject);

			// Register root object for undo.
			Undo.RegisterCreatedObjectUndo (newPOI, "Create Mini-Map Fog Of War Volume");
			Selection.activeObject = newPOI;

			// Enable fog of war automatically
			CompassPro compass = CompassPro.instance;
			if (compass != null) {
				compass.fogOfWarEnabled = true;
			}
		}
	
	}
	
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace CompassNavigatorPro {

	public partial class CompassPro : MonoBehaviour {

		[NonSerialized]
		public bool needFogOfWarUpdate, needFogOfWarTextureUpdate;

		const string FOG_OF_WAR_LAYER = "FogOfWarLayer";
		Texture2D fogOfWarTexture;
		Color32[] fogOfWarColorBuffer;
		Material fogOfWarMaterial;
		int fogOfWarAutoClearLastPosX, fogOfWarAutoClearLastPosZ;

		#region Fog Of War

		void UpdateFogOfWarOnLoadScene(Scene scene, LoadSceneMode loadMode) {
			if (loadMode == LoadSceneMode.Single) {
				UpdateFogOfWar ();
			}
		}


		void UpdateFogOfWarTexture () {

			if (miniMapCamera == null)
				return;
			
			Transform fogOfWarLayer = transform.Find (FOG_OF_WAR_LAYER);
			if (fogOfWarLayer != null) {
				DestroyImmediate (fogOfWarLayer.gameObject);
			}
			
			if (!_fogOfWarEnabled) return;

			if (fogOfWarTexture == null || fogOfWarTexture.width != _fogOfWarTextureSize || fogOfWarTexture.height != _fogOfWarTextureSize) {
				fogOfWarTexture = new Texture2D (_fogOfWarTextureSize, _fogOfWarTextureSize, TextureFormat.Alpha8, false);
				fogOfWarTexture.hideFlags = HideFlags.DontSave;
				fogOfWarTexture.filterMode = FilterMode.Bilinear;
				fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
			}

			// Update bounds
			ResetFogOfWar (_fogOfWarDefaultAlpha);
			CompassProFogVolume[] fv = FindObjectsOfType<CompassProFogVolume> ();
			Array.Sort (fv, VolumeComparer);
			for (int k = 0; k < fv.Length; k++) {
				Collider collider = fv [k].GetComponent<Collider> ();
				if (collider != null && collider.gameObject.activeInHierarchy) {
					SetFogOfWarAlpha (collider.bounds, fv [k].alpha, fv [k].border);
				}
			}
			needFogOfWarTextureUpdate = true;
		}

		void UpdateFogOfWarPosition () {
			
			if (!_fogOfWarEnabled)
				return;
			
			if (needFogOfWarUpdate) {
				needFogOfWarUpdate = false;
				UpdateFogOfWarTexture ();
			}

			if (_fogOfWarAutoClear && _miniMapFollow != null) {
				Vector3 pos = _miniMapFollow.transform.position;
				int x = (int)pos.x;
				int z = (int)pos.z;
				if (x != fogOfWarAutoClearLastPosX || z != fogOfWarAutoClearLastPosZ) {
					fogOfWarAutoClearLastPosX = x;
					fogOfWarAutoClearLastPosZ = z;
					SetFogOfWarAlpha (pos, _fogOfWarAutoClearRadius, 0, 1f);
				}
			}

			if (needFogOfWarTextureUpdate) {
				needFogOfWarTextureUpdate = false;
				if (fogOfWarTexture != null) {
                    fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
					fogOfWarTexture.Apply ();
                    miniMapMaterialRefresh = true;
				}
			}
		}

		int VolumeComparer (CompassProFogVolume v1, CompassProFogVolume v2) {
			if (v1.order < v2.order) {
				return -1;
			} else if (v1.order > v2.order) {
				return 1;
			} else {
				return 0;
			}
		}

		#endregion
	}


}




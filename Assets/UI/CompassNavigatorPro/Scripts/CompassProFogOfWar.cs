using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CompassNavigatorPro {


    public partial class CompassPro : MonoBehaviour {

        [SerializeField]
        bool _fogOfWarEnabled;

        public bool fogOfWarEnabled {
            get { return _fogOfWarEnabled; }
            set {
                if (value != _fogOfWarEnabled) {
                    _fogOfWarEnabled = value;
                    SetupMiniMap();
                    UpdateFogOfWar();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        Vector3 _fogOfWarCenter;

        public Vector3 fogOfWarCenter {
            get { return _fogOfWarCenter; }
            set {
                if (value != _fogOfWarCenter) {
                    _fogOfWarCenter = value;
                    UpdateFogOfWar();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Vector3 _fogOfWarSize = new Vector3(1024, 0, 1024);

        public Vector3 fogOfWarSize {
            get { return _fogOfWarSize; }
            set {
                if (value != _fogOfWarSize) {
                    if (value.x > 0 && value.z > 0) {
                        _fogOfWarSize = value;
                        UpdateFogOfWar();
                        isDirty = true;
                    }
                }
            }
        }

        [SerializeField, Range(32, 2048)]
        int _fogOfWarTextureSize = 256;

        public int fogOfWarTextureSize {
            get { return _fogOfWarTextureSize; }
            set {
                if (value != _fogOfWarTextureSize) {
                    if (value > 16) {
                        _fogOfWarTextureSize = value;
                        UpdateFogOfWar();
                        isDirty = true;
                    }
                }
            }
        }


        [SerializeField]
        Color _fogOfWarColor = new Color(47 / 255f, 47 / 255f, 47 / 255f);

        public Color fogOfWarColor {
            get { return _fogOfWarColor; }
            set {
                if (value != _fogOfWarColor) {
                    _fogOfWarColor = value;
                    miniMapMaterialRefresh = true;
                    UpdateFogOfWar();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _fogOfWarAutoClear;

        /// <summary>
        /// Clears fog automatically as player cross it
        /// </summary>
        /// <value><c>true</c> if fog of war auto clear; otherwise, <c>false</c>.</value>
        public bool fogOfWarAutoClear {
            get { return _fogOfWarAutoClear; }
            set {
                if (value != _fogOfWarAutoClear) {
                    _fogOfWarAutoClear = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _fogOfWarAutoClearRadius = 20f;

        public float fogOfWarAutoClearRadius {
            get { return _fogOfWarAutoClearRadius; }
            set {
                if (value != _fogOfWarAutoClearRadius) {
                    _fogOfWarAutoClearRadius = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField, Range(0, 1)]
        float _fogOfWarDefaultAlpha = 1f;

        public float fogOfWarDefaultAlpha {
            get { return _fogOfWarDefaultAlpha; }
            set {
                if (value != _fogOfWarDefaultAlpha) {
                    _fogOfWarDefaultAlpha = value;
                    UpdateFogOfWar();
                    isDirty = true;
                }
            }
        }


        public void UpdateFogOfWar() {
            if (!fogOfWarEnabled)
                return;
            if (Application.isPlaying) {
                needFogOfWarUpdate = true;
            } else {
                UpdateFogOfWarTexture();
            }
        }



        /// <summary>
        /// Gets or set fog of war state as a Color32 buffer. The alpha channel stores the transparency of the fog at that position (0 = no fog, 1 = opaque).
        /// </summary>
        public Color32[] fogOfWarTextureData {
            get {
                return fogOfWarColorBuffer;
            }
            set {
                fogOfWarEnabled = true;
                fogOfWarColorBuffer = value;
                if (value == null || fogOfWarTexture == null)
                    return;
                if (value.Length != fogOfWarTexture.width * fogOfWarTexture.height)
                    return;
                fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
                fogOfWarTexture.Apply();
                miniMapMaterialRefresh = true;
            }
        }


        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="border">value that determines the hardness of the border.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, float border) {
            if (fogOfWarTexture == null)
                return;

            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = fogOfWarTexture.width;
            int th = fogOfWarTexture.height;
            int px = Mathf.Clamp((int)(tx * tw), 0, tw - 1);
            int pz = Mathf.Clamp((int)(tz * th), 0, th - 1);
            int colorBufferPos = pz * tw + px;
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float tr = radius / _fogOfWarSize.z;
            int delta = (int)(th * tr);
            int deltaSqr = delta * delta;
            for (int r = pz - delta; r <= pz + delta; r++) {
                if (r >= 0 && r < th) {
                    for (int c = px - delta; c <= px + delta; c++) {
                        if (c >= 0 && c < tw) {
                            int distanceSqr = (pz - r) * (pz - r) + (px - c) * (px - c);
                            if (distanceSqr <= deltaSqr) {
                                colorBufferPos = r * tw + c;
                                Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                                float t = distanceSqr * border / deltaSqr;
                                if (t > 1f) {
                                    t = 1f;
                                }
                                byte targetAlpha = (byte)(newAlpha8 * (1.0 - t) + colorBuffer.a * t); // Mathf.Lerp (newAlpha8, colorBuffer.a, t);
                                colorBuffer.a = targetAlpha;
                                fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                needFogOfWarTextureUpdate = true;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="border">value that determines the hardness of the border.</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, float border) {
            if (fogOfWarTexture == null)
                return;

            Vector3 worldPosition = bounds.center;
            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = fogOfWarTexture.width;
            int th = fogOfWarTexture.height;
            int px = Mathf.Clamp((int)(tx * tw), 0, tw - 1);
            int pz = Mathf.Clamp((int)(tz * th), 0, th - 1);
            int colorBufferPos;
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float trx = bounds.extents.x / _fogOfWarSize.x;
            float trz = bounds.extents.z / _fogOfWarSize.z;
            int deltax = (int)(tw * trx);
            int deltaz = (int)(th * trz);
            for (int r = pz - deltaz; r <= pz + deltaz; r++) {
                if (r >= 0 && r < th) {
                    int distancez = pz - r;
                    if (distancez < 0) distancez = -distancez;
                    if (distancez > deltaz) continue;
                    float dz = (deltaz - distancez + 1) / (deltaz * border + 0.0001f);
                    for (int c = px - deltax; c <= px + deltax; c++) {
                        if (c >= 0 && c < tw) {
                            int distancex = px - c;
                            if (distancex < 0) distancex = -distancex;
                            if (distancex <= deltax) {
                                colorBufferPos = r * tw + c;
                                Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                                float dx = (deltax - distancex + 1) / (deltax * border + 0.0001f);
                                float t = dx * dz;
                                if (t > 1f) {
                                    t = 1f;
                                }
                                byte targetAlpha = (byte)(colorBuffer.a * (1f - t) + newAlpha8 * t); // Mathf.Lerp (colorBuffer.a, newAlpha8, t);
                                colorBuffer.a = targetAlpha;
                                fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                needFogOfWarTextureUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill fog of war with a value
        /// </summary>
        public void ResetFogOfWar(float alpha = 1f) {
            if (fogOfWarTexture == null)
                return;
            int h = fogOfWarTexture.height;
            int w = fogOfWarTexture.width;
            int newLength = h * w;
            if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != newLength) {
                fogOfWarColorBuffer = new Color32[newLength];
            }
            byte a8 = (byte)(alpha * 255f);
            Color32 opaque = new Color32(a8, a8, a8, a8);
            for (int k = 0; k < newLength; k++) {
                fogOfWarColorBuffer[k] = opaque;
            }
            isDirty = true;
        }



        /// <summary>
        /// Gets the current alpha value of the Fog of War at a given world position
        /// </summary>
        /// <returns>The fog of war alpha.</returns>
        /// <param name="worldPosition">World position.</param>
        public float GetFogOfWarAlpha(Vector3 worldPosition) {
            if (fogOfWarColorBuffer == null)
                return 1f;

            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return 1f;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return 1f;

            int tw = fogOfWarTexture.width;
            int th = fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            int colorBufferPos = pz * tw + px;
            if (colorBufferPos < 0 || colorBufferPos >= fogOfWarColorBuffer.Length)
                return 1f;
            return fogOfWarColorBuffer[colorBufferPos].a / 255f;
        }


        /// <summary>
        /// Activates fog of war along a path given by a list of points
        /// </summary>
        public void SetFogOfWar(List<Vector3> points, float stepSize = 1f, float alpha = 1f) {
            if (points == null) return;
            int pointCount = points.Count;
            if (pointCount < 2) return;
            for (int k = 0; k < pointCount - 1; k++) {
                Vector3 p = points[k];
                Vector3 q = points[k + 1];
                p.y = q.y = 0;
                float distance = Vector3.Distance(p, q);
                int steps = Mathf.CeilToInt(distance / stepSize);
                for (int j = 0; j <= steps; j++) {
                    float i = (float)j / steps;
                    Vector3 x = Vector3.Lerp(p, q, i);
                    SetFogOfWarAlpha(x, 1f, alpha, 0);
                }
            }
        }

    }


}




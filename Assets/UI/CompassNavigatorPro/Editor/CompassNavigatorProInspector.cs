using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CompassNavigatorPro {
    [CustomEditor(typeof(CompassPro))]
    public class CompassNavigatorProInspector : Editor {

        CompassPro _compass;
        Texture2D _headerTexture;
        static GUIStyle sectionHeaderStyle;
        bool expandCompassBarSettings, expandCompassPOISettings, expandTitleSettings, expandTextSettings, expandMiniMapSettings, expandFogOfWarSettings;

        void OnEnable() {
            _compass = (CompassPro)target;
            _headerTexture = Resources.Load<Texture2D>("CNPro/CompassNavigatorProHeader");
            expandCompassBarSettings = EditorPrefs.GetBool("CNProCompassBarSettings", true);
            expandCompassPOISettings = EditorPrefs.GetBool("CNProCompassPOISettings", false);
            expandTitleSettings = EditorPrefs.GetBool("CNProTitleSettings", false);
            expandMiniMapSettings = EditorPrefs.GetBool("CNProMiniMapSettings", false);
            expandFogOfWarSettings = EditorPrefs.GetBool("CNProFogOfWarSettings", false);
        }

        private void OnDisable() {
            EditorPrefs.SetBool("CNProCompassBarSettings", expandCompassBarSettings);
            EditorPrefs.SetBool("CNProCompassPOISettings", expandCompassPOISettings);
            EditorPrefs.SetBool("CNProTitleSettings", expandTitleSettings);
            EditorPrefs.SetBool("CNProTextSettings", expandTextSettings);
            EditorPrefs.SetBool("CNProMiniMapSettings", expandMiniMapSettings);
            EditorPrefs.SetBool("CNProFogOfWarSettings", expandFogOfWarSettings);
        }

        public override void OnInspectorGUI() {
            if (_compass == null)
                return;
            _compass.isDirty = false;
            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
                sectionHeaderStyle.SetFoldoutColor();
                sectionHeaderStyle.fontStyle = FontStyle.Bold;
            }

            EditorGUILayout.Separator();
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(_headerTexture, GUILayout.ExpandWidth(true));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            expandCompassBarSettings = DrawTitleLabel(expandCompassBarSettings, "Compass Bar Settings");
            if (GUILayout.Button("Help", GUILayout.Width(50)))
                EditorUtility.DisplayDialog("Help", "Move the mouse over each label to show a description of the parameter.", "Ok");
            if (GUILayout.Button("About", GUILayout.Width(60))) {
                CompassProAbout.ShowAboutWindow();
            }
            EditorGUILayout.EndHorizontal();

            if (expandCompassBarSettings) {
                _compass.cameraMain = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera used for distance computation."), _compass.cameraMain, typeof(Camera), true);
                _compass.style = (COMPASS_STYLE)EditorGUILayout.EnumPopup(new GUIContent("Style", "Compass bar style."), _compass.style);
                EditorGUILayout.BeginHorizontal();
                _compass.bendFactor = EditorGUILayout.Slider(new GUIContent("Bend Amount", "Bending amount. Set this to zero to disable bending effect."), _compass.bendFactor, -1f, 1f);
                if (GUILayout.Button("Disable", GUILayout.Width(80))) {
                    _compass.bendFactor = 0;
                }
                EditorGUILayout.EndHorizontal();

                _compass.width = EditorGUILayout.Slider(new GUIContent("Width", "Width of the compass bar in % of the screen width."), _compass.width, 0.05f, 1f);
                _compass.height = EditorGUILayout.Slider(new GUIContent("Height", "Vertical scale of the compass bar."), _compass.height, 0.05f, 5f);
                _compass.verticalPosition = EditorGUILayout.Slider(new GUIContent("Vertical Position", "Distance from the bottom of the screen in %."), _compass.verticalPosition, -0.2f, 1.2f);
                _compass.horizontalPosition = EditorGUILayout.Slider(new GUIContent("Horizontal Position", "Distance from the center of the screen in %."), _compass.horizontalPosition, -0.5f, 0.5f);
                _compass.endCapsWidth = EditorGUILayout.Slider(new GUIContent("End Caps Width", "Width of the end caps of the compass bar. This setting limits the usable horizontal range of the bar in the screen to prevent icons being drawn over the art of the end caps of the bar."), _compass.endCapsWidth, 0, 100f);
                _compass.edgeFadeOut = EditorGUILayout.Toggle(new GUIContent("Edge Fade Out", "Enables edge fade out effect."), _compass.edgeFadeOut);
                if (_compass.edgeFadeOut) {
                    EditorGUI.indentLevel++; ;
                    _compass.edgeFadeOutWidth = EditorGUILayout.Slider(new GUIContent("Width", "Width of the edge fade out."), _compass.edgeFadeOutWidth, 0f, 1f);
                    _compass.edgeFadeOutStart = EditorGUILayout.Slider(new GUIContent("Start", "Start of the edge fade out."), _compass.edgeFadeOutStart, 0f, 1f);
                    _compass.edgeFadeOutText = EditorGUILayout.Toggle(new GUIContent("Fade Out Text", "If edge fade out affects title and text below compass bar."), _compass.edgeFadeOutText);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.BeginHorizontal();
                _compass.alpha = EditorGUILayout.Slider(new GUIContent("Alpha", "Transparency of the compass bar."), _compass.alpha, 0f, 1f);
                if (_compass.alpha > 0) {
                    if (GUILayout.Button("Hide", GUILayout.Width(60))) {
                        _compass.alpha = 0;
                    }
                } else {
                    if (GUILayout.Button("Show", GUILayout.Width(60))) {
                        _compass.alpha = 1;
                    }
                }
                EditorGUILayout.EndHorizontal();
                _compass.alwaysVisibleInEditMode = EditorGUILayout.Toggle(new GUIContent("  Visible In Edit Mode", "Makes the bar always visible (ignored alpha property) while in Edit Mode."), _compass.alwaysVisibleInEditMode);
                _compass.autoHide = EditorGUILayout.Toggle(new GUIContent("Auto Hide If Empty", "Hides the compass bar if no POIs are below visible distance."), _compass.autoHide);
                _compass.fadeDuration = EditorGUILayout.Slider(new GUIContent("Fade Duration", "Duration of alpha changes in seconds."), _compass.fadeDuration, 0f, 8f);
                _compass.worldMappingMode = (WORLD_MAPPING_MODE)EditorGUILayout.EnumPopup(new GUIContent("World Mapping Mode", "How POIs positions are mapped to the bar. 1) Limited To Bar Width = the bar width determines the view angle, 2) Camera Frustum = the entire camera frustum is mapped to the bar width, 3) Full 180 degrees = all POIs in front of the camera will appear in the compass bar. 4) Full 360 degrees = all POIs are visible in the compass bar."), _compass.worldMappingMode);
                _compass.use3Ddistance = EditorGUILayout.Toggle(new GUIContent("Use 3D Distance", "Whether 3D distance should be computed instead of planar X/Z distance"), _compass.use3Ddistance);
                _compass.sameAltitudeThreshold = EditorGUILayout.Slider(new GUIContent("Same Altitude Diff.", "Minimum difference in altitude from camera to show 'above' or 'below'"), _compass.sameAltitudeThreshold, 1f, 50f);
                _compass.northDegrees = EditorGUILayout.Slider(new GUIContent("North Position", "The position of the North in degrees (0-360)"), _compass.northDegrees, 0, 360f);
                _compass.showCardinalPoints = EditorGUILayout.Toggle(new GUIContent("Show Cardinal Points", "Whether N, W, S, E should be visible in the compass bar."), _compass.showCardinalPoints);
                if (_compass.showCardinalPoints) {
                    _compass.cardinalScale = EditorGUILayout.Slider("   Scale", _compass.cardinalScale, 0.1f, 3f); ;
                }
                _compass.showOrdinalPoints = EditorGUILayout.Toggle(new GUIContent("Show Ordinal Points", "Whether NW, NE, SW, SE should be visible in the compass bar."), _compass.showOrdinalPoints);
                if (_compass.showOrdinalPoints) {
                    _compass.ordinalScale = EditorGUILayout.Slider("   Scale", _compass.ordinalScale, 0.1f, 3f);
                }
                if (_compass.showCardinalPoints || _compass.showOrdinalPoints) {
                    _compass.cardinalPointsVerticalOffset = EditorGUILayout.FloatField(new GUIContent("   Vertical Offset", "Optional vertical displacement for both cardinal and ordinal points."), _compass.cardinalPointsVerticalOffset);
                }

                _compass.showHalfWinds = EditorGUILayout.Toggle(new GUIContent("Show Half Winds", "Enable vertical interval marks in the compass bar."), _compass.showHalfWinds);
                if (_compass.showHalfWinds) {
                    _compass.halfWindsHeight = EditorGUILayout.Slider("  Height", _compass.halfWindsHeight, 0.1f, 1f);
                    _compass.halfWindsWidth = EditorGUILayout.Slider("  Width", _compass.halfWindsWidth, 1f, 5f);
                    _compass.halfWindsTintColor = EditorGUILayout.ColorField("  Tint Color", _compass.halfWindsTintColor);
                }
                _compass.showDistance = EditorGUILayout.Toggle(new GUIContent("Show Distance (meters)", "Whether the distance in meters should be shown in the title."), _compass.showDistance);

                if (_compass.showDistance) {
                    _compass.showDistanceFormat = EditorGUILayout.TextField(new GUIContent("  String Format", " The string format for displaying the distance. A value of F0 means 'Fixed/Decimal with 0 decimal positions'. A value of F1 includes 1 decimal position. The sintax for this string format corresponds with the available options for ToString(format) method of C#."), _compass.showDistanceFormat);
                }

                _compass.dontDestroyOnLoad = EditorGUILayout.Toggle(new GUIContent("Don't Destroy On Load", "Preserve compass bar between scene changes."), _compass.dontDestroyOnLoad);
                _compass.updateInterval = (UPDATE_INTERVAL)EditorGUILayout.EnumPopup(new GUIContent("Idle Update Mode", "Contents are always updated if camera moves or rotates. If not, this property specifies the intervel between POI change checks."), _compass.updateInterval);

                if (_compass.updateInterval == UPDATE_INTERVAL.NumberOfFrames) {
                    _compass.updateIntervalFrameCount = EditorGUILayout.IntField(new GUIContent("  Frame Count", "Frames between change check."), _compass.updateIntervalFrameCount);
                } else if (_compass.updateInterval == UPDATE_INTERVAL.Time) {
                    _compass.updateIntervalTime = EditorGUILayout.FloatField(new GUIContent("  Seconds", "Seconds between change check."), _compass.updateIntervalTime);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();

            expandCompassPOISettings = DrawTitleLabel(expandCompassPOISettings, "Compass POIs Settings");
            if (expandCompassPOISettings) {
                _compass.visibleDistance = EditorGUILayout.FloatField(new GUIContent("Visible Max Distance", "POIs beyond visible distance (meters) will not be shown in the compass bar."), _compass.visibleDistance);
                _compass.visibleMinDistance = EditorGUILayout.FloatField(new GUIContent("Visible Min Distance", "POIs nearer than this distance (meters) will not be shown in the compass bar."), _compass.visibleMinDistance);
                _compass.nearDistance = EditorGUILayout.Slider(new GUIContent("Near Distance", "Distance to a POI where the icon will start to grow as player approaches."), _compass.nearDistance, 10, 10000);
                _compass.visitedDistance = EditorGUILayout.Slider(new GUIContent("Visited Distance", "Minimum distance to a POI to be considered as explored/visited."), _compass.visitedDistance, 1, 10000);

                EditorGUILayout.BeginHorizontal();
                float minIconSize = _compass.minIconSize;
                float maxIconSize = _compass.maxIconSize;
                EditorGUILayout.MinMaxSlider(new GUIContent("Icon Size Range", "Minimum and maximum icon sizes. Icons grow/shrink depending on distance."), ref minIconSize, ref maxIconSize, 0.1f, 2f);
                _compass.minIconSize = minIconSize;
                _compass.maxIconSize = maxIconSize;
                GUILayout.Label(minIconSize.ToString("F2") + "-" + maxIconSize.ToString("F2"));
                EditorGUILayout.EndHorizontal();

                _compass.scaleInDuration = EditorGUILayout.Slider(new GUIContent("Scale In Duration", "Duration for the scale animation when the POI appears on the compass bar."), _compass.scaleInDuration, 0, 5);
                _compass.labelHotZone = EditorGUILayout.Slider(new GUIContent("Label Hot Zone", "The distance from the center of the compass bar where a POI's label is visible."), _compass.labelHotZone, 0.001f, 0.2f);
                _compass.gizmoScale = EditorGUILayout.Slider(new GUIContent("Gizmo Scale", "Scaling applied to gizmos shown during playmode."), _compass.gizmoScale, 0.01f, 1f);
                _compass.visitedDefaultAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Visited Sound", "Default audio clip to be played when a POI is visited for the first time. Note that you can specify a different audio clip in the POI script itself."), _compass.visitedDefaultAudioClip, typeof(AudioClip), false);
                _compass.beaconDefaultAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Beacon Sound", "Default audio clip to be played when a POI beacon is shown. Note that you can specify a different audio clip in the POI script itself."), _compass.beaconDefaultAudioClip, typeof(AudioClip), false);
                _compass.heartbeatDefaultAudioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Heartbeat Sound", "Default audio clip to play for the heartbeat effect. This effect is enabled on each POI and will play a custom sound with variable speed depending on distance."), _compass.heartbeatDefaultAudioClip, typeof(AudioClip), false);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();

            expandTitleSettings = DrawTitleLabel(expandTitleSettings, "Title Settings");
            if (expandTitleSettings) {
                _compass.titleFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Font", "Font for the title."), _compass.titleFont, typeof(Font), false);
                _compass.titleVerticalPosition = EditorGUILayout.Slider(new GUIContent("Vertical Offset", "Vertical offset in pixels for the title with respect to the compass bar."), _compass.titleVerticalPosition, -200, 200);
                _compass.titleScale = EditorGUILayout.Slider(new GUIContent("Scale", "Scaling applied to the title."), _compass.titleScale, 0.02f, 3);
                _compass.titleShadowEnabled = EditorGUILayout.Toggle(new GUIContent("Text Shadow", "Enable or disable text shadow."), _compass.titleShadowEnabled);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            expandTextSettings = DrawTitleLabel(expandTextSettings, "Text Settings");
            if (expandTextSettings) {
                _compass.textRevealEnabled = EditorGUILayout.Toggle(new GUIContent("Enable", "Show a revealing text effect when discovering POIs for the first time."), _compass.textRevealEnabled);
                _compass.textFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Font", "Font for the text."), _compass.textFont, typeof(Font), false);
                _compass.textVerticalPosition = EditorGUILayout.Slider(new GUIContent("Vertical Offset", "Vertical offset in pixels for the text with respect to the compass bar."), _compass.textVerticalPosition, -200, 200);
                _compass.textScale = EditorGUILayout.Slider(new GUIContent("Scale", "Scaling applied to the text."), _compass.textScale, 0.02f, 3);
                _compass.textRevealDuration = EditorGUILayout.Slider(new GUIContent("Reveal Duration", "Text reveal duration in seconds."), _compass.textRevealDuration, 0, 3);
                _compass.letterSpacing = EditorGUILayout.Slider(new GUIContent("Letter Spacing", "Controls the spacing between each letter in the reveal text."), _compass.letterSpacing, 0.02f, 3);
                _compass.textRevealLetterDelay = EditorGUILayout.Slider(new GUIContent("Letter Delay", "Delay in appearance of each letter during a text reveal."), _compass.textRevealLetterDelay, 0, 1);
                _compass.textDuration = EditorGUILayout.Slider(new GUIContent("Duration", "Text duration in screen."), _compass.textDuration, 0, 20);
                _compass.textFadeOutDuration = EditorGUILayout.Slider(new GUIContent("Fade Out Duration", "Duration of the text fade out."), _compass.textFadeOutDuration, 0, 10);
                _compass.textShadowEnabled = EditorGUILayout.Toggle(new GUIContent("Text Shadow", "Enable or disable text shadow."), _compass.textShadowEnabled);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            expandMiniMapSettings = DrawTitleLabel(expandMiniMapSettings, "Mini-Map Settings");
            if (expandMiniMapSettings) {
                _compass.showMiniMap = EditorGUILayout.Toggle(new GUIContent("Enable", "Shows the minimap."), _compass.showMiniMap);
                _compass._miniMapFollow = (Transform)EditorGUILayout.ObjectField(new GUIContent("Follow", "Center of the mini map."), _compass._miniMapFollow, typeof(Transform), true);
                _compass.miniMapKeepStraight = EditorGUILayout.Toggle(new GUIContent("Keep Straight", "Keeps the mini-map oriented to North."), _compass.miniMapKeepStraight);
                _compass.miniMapCameraMode = (MINIMAP_CAMERA_MODE)EditorGUILayout.EnumPopup(new GUIContent("Camera Projection", "Orthographic or perspective mode for the mini-map camera."), _compass.miniMapCameraMode);

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Style & Design", EditorStyles.miniBoldLabel);

                _compass.miniMapContents = (MINIMAP_CONTENTS)EditorGUILayout.EnumPopup(new GUIContent("Contents", "What to show when minimap is non full screen mode."), _compass.miniMapContents);
                if (_compass.miniMapContents == MINIMAP_CONTENTS.WorldMappedTexture) {
                    _compass.miniMapContentsTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("   Texture", "The texture to be used as background for the minimap in non full screen mode."), _compass.miniMapContentsTexture, typeof(Texture), false);
                    if (_compass.miniMapCamera != null && !_compass.miniMapCamera.orthographic) {
                        EditorGUILayout.HelpBox("Feature only available with mini camera set to orthographic mode.", MessageType.Warning);
                    }
                    _compass.miniMapWorldCenter = EditorGUILayout.Vector3Field(new GUIContent("   World Center", "Center of the world map."), _compass.miniMapWorldCenter);
                    _compass.miniMapWorldSize = EditorGUILayout.Vector3Field(new GUIContent("   World Size", "Size of the world map."), _compass.miniMapWorldSize);
                } else if (_compass.miniMapContents == MINIMAP_CONTENTS.UITexture) {
                    _compass.miniMapContentsTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("   Texture", "The texture to be used as background for the minimap in non full screen mode."), _compass.miniMapContentsTexture, typeof(Texture), false);
                }

                _compass.miniMapStyle = (MINIMAP_STYLE)EditorGUILayout.EnumPopup(new GUIContent("Style", "Mini-map style."), _compass.miniMapStyle);
                _compass.miniMapBackgroundColor = EditorGUILayout.ColorField(new GUIContent("Background Color"), _compass.miniMapBackgroundColor);

                _compass.miniMapPositionAndSize = (MINIMAP_POSITION_AND_SIZE)EditorGUILayout.EnumPopup(new GUIContent("Position & Size", "How position and size of mini-map are handled."), _compass.miniMapPositionAndSize);

                if (_compass.miniMapPositionAndSize == MINIMAP_POSITION_AND_SIZE.ControlledByCompassNavigatorPro) {
                    _compass.miniMapLocation = (MINIMAP_LOCATION)EditorGUILayout.EnumPopup(new GUIContent("Screen Location", "Location of mini-map."), _compass.miniMapLocation);
                    _compass.miniMapLocationOffset = EditorGUILayout.Vector2Field(new GUIContent("   Offset", "Location of mini-map."), _compass.miniMapLocationOffset);
                    _compass.miniMapSize = EditorGUILayout.Slider(new GUIContent("Normal Size", "Screen size of mini-map in % of screen height."), _compass.miniMapSize, 0f, 1f);
                }

                if (_compass.miniMapStyle == MINIMAP_STYLE.Custom) {
                    _compass.miniMapBorderTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Border Texture", "Texture for the border of the mini map."), _compass.miniMapBorderTexture, typeof(Texture2D), false);
                    _compass.miniMapMaskSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Mask Texture", "Mask for the border of the mini map."), _compass.miniMapMaskSprite, typeof(Sprite), false);
                }

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Icons", EditorStyles.miniBoldLabel);
                _compass.miniMapIconSize = EditorGUILayout.FloatField(new GUIContent("Icons Size", "The size for the icons on the mini-map."), _compass.miniMapIconSize);
                _compass.miniMapIconPositionShift = EditorGUILayout.Vector2Field(new GUIContent("Icon Position Shift", "Optional displacement for the icons in the mini-map."), _compass.miniMapIconPositionShift);
                _compass.miniMapPlayerIconSize = EditorGUILayout.FloatField(new GUIContent("Player Icon Size", "The size for the player icon on the mini-map."), _compass.miniMapPlayerIconSize);
                EditorGUI.indentLevel++;
                _compass.miniMapPlayerIconSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Sprite", "The sprite for the player icon."), _compass.miniMapPlayerIconSprite, typeof(Sprite), false);
                _compass.miniMapPlayerIconColor = EditorGUILayout.ColorField(new GUIContent("Color", "The color for the player icon."), _compass.miniMapPlayerIconColor);
                EditorGUI.indentLevel--;
                _compass.miniMapClampBorder = EditorGUILayout.FloatField(new GUIContent("Clamp Border", "The distance of clamped icons to the edge of the mini-map."), _compass.miniMapClampBorder);
                _compass.miniMapClampBorderCircular = EditorGUILayout.Toggle(new GUIContent("Border Is Circular", "Enable this option if the minimap uses a circular shape."), _compass.miniMapClampBorderCircular);
                if (_compass.miniMapClampBorderCircular) {
                    _compass.miniMapVignette = EditorGUILayout.Toggle(new GUIContent("   Vignette", "Enable this option if the minimap uses a circular shape and want to darken the inner border."), _compass.miniMapVignette);
                }

                _compass.miniMapIconEvents = EditorGUILayout.Toggle(new GUIContent("Icon Events", "Raise pointer click, down, up, enter and exit events on icons."), _compass.miniMapIconEvents);
                _compass.miniMapShowButtons = EditorGUILayout.Toggle(new GUIContent("Show Buttons", "Show zoom in/out/max buttons."), _compass.miniMapShowButtons);
                if (_compass.miniMapShowButtons) {
                    _compass.miniMapButtonsScale = EditorGUILayout.Slider(new GUIContent("   Buttons Scale"), _compass.miniMapButtonsScale, 0.01f, 2f);
                }

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("World Capture & Rendering", EditorStyles.miniBoldLabel);
                _compass.miniMapResolution = (MINIMAP_RESOLUTION)EditorGUILayout.EnumPopup(new GUIContent("Resolution", "Size of the render texture in non-full screen mode."), _compass.miniMapResolution);
                if (_compass.miniMapCameraMode == MINIMAP_CAMERA_MODE.Orthographic) {
                    _compass.miniMapCameraHeightVSFollow = EditorGUILayout.FloatField(new GUIContent("Height vs Follow", "The altitude of the mini-map camera relative to the main camera or followed gameobject."), _compass.miniMapCameraHeightVSFollow);
                    _compass.miniMapCaptureSize = EditorGUILayout.FloatField(new GUIContent("Captured World Size", "The orthographic size of the mini-map camera."), _compass.miniMapCaptureSize);
                    _compass.miniMapCameraSnapshotFrequency = (MINIMAP_CAMERA_SNAPSHOT_FREQUENCY)EditorGUILayout.EnumPopup(new GUIContent("Snapshot Frequency", "Frequency of camera capture."), _compass.miniMapCameraSnapshotFrequency);
                    if (_compass.miniMapCameraSnapshotFrequency != MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.Continuous) {
                        EditorGUILayout.BeginHorizontal();
                        switch (_compass.miniMapCameraSnapshotFrequency) {
                            case MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.TimeInterval:
                                _compass.miniMapSnapshotInterval = EditorGUILayout.FloatField(new GUIContent("   Time Interval (s)", "Frequency of camera capture in seconds."), _compass.miniMapSnapshotInterval);
                                break;
                            case MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.DistanceTravelled:
                                _compass.miniMapSnapshotDistance = EditorGUILayout.FloatField(new GUIContent("   Distance (m)", "Distance in meters."), _compass.miniMapSnapshotDistance);
                                break;
                        }
                        if (GUILayout.Button("Now!", GUILayout.Width(60))) {
                            _compass.UpdateMiniMapContents();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    _compass.miniMapZoomMin = EditorGUILayout.Slider("Zoom Min", _compass.miniMapZoomMin, 0f, 1f);
                    _compass.miniMapZoomMax = EditorGUILayout.Slider("Zoom Max", _compass.miniMapZoomMax, 0f, 1f);
                } else {
                    _compass.miniMapCameraMinAltitude = EditorGUILayout.FloatField(new GUIContent("Altitude Min", "The minimum altitude of the mini-map camera respect with the follow target."), _compass.miniMapCameraMinAltitude);
                    _compass.miniMapCameraMaxAltitude = EditorGUILayout.FloatField(new GUIContent("Altitude Max", "The maximum altitude of the mini-map camera respect with the follow target."), _compass.miniMapCameraMaxAltitude);
                }
                _compass.miniMapZoomLevel = EditorGUILayout.Slider(new GUIContent("Current Zoom", "The current zoom for the mini-map based on the minimum / maximum ranges."), _compass.miniMapZoomLevel, 0, 1f);
                if (_compass.miniMapCameraMode == MINIMAP_CAMERA_MODE.Perspective) {
                    _compass.miniMapCameraTilt = EditorGUILayout.Slider(new GUIContent("Camera Tilt", "Rotation of the mini-map camera."), _compass.miniMapCameraTilt, 0, 90);
                }
                _compass.miniMapLayerMask = LayerMaskField(new GUIContent("Layer Mask", "Which objects will be visible in the mini-map."), _compass.miniMapLayerMask);
                _compass.miniMapEnableShadows = EditorGUILayout.Toggle(new GUIContent("Render Shadows", "Enable to render shadows in mini-map."), _compass.miniMapEnableShadows);
                _compass.miniMapAlpha = EditorGUILayout.Slider(new GUIContent("Alpha", "Transparency of the mini-map."), _compass.miniMapAlpha, 0f, 1f);
                _compass.miniMapContrast = EditorGUILayout.Slider(new GUIContent("Contrast"), _compass.miniMapContrast, 0f, 2f);
                _compass.miniMapBrightness = EditorGUILayout.Slider(new GUIContent("Brightness"), _compass.miniMapBrightness, 0f, 2f);
                _compass.miniMapLutTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("LUT Texture"), _compass.miniMapLutTexture, typeof(Texture2D), false);
                if (_compass.miniMapLutTexture != null) {
                    CheckLUTSettings(_compass.miniMapLutTexture);
                    _compass.miniMapLutIntensity = EditorGUILayout.Slider(new GUIContent("LUT Intensity"), _compass.miniMapLutIntensity, 0f, 1f);
                }

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Full Screen Mode", EditorStyles.miniBoldLabel);
                _compass.miniMapFullScreenResolution = (MINIMAP_RESOLUTION)EditorGUILayout.EnumPopup(new GUIContent("Resolution", "Size of the render texture in full screen mode."), _compass.miniMapFullScreenResolution);
                _compass.miniMapFullScreenSize = EditorGUILayout.Slider(new GUIContent("Size", "Percentage of screen size if full-screen mode. Image resolution will increase according to screen resolution."), _compass.miniMapFullScreenSize, 0.5f, 1f);
                _compass.miniMapFullScreenPlaceholder = (RectTransform)EditorGUILayout.ObjectField(new GUIContent("UI Placeholder", "Optional UI element which serves as placeholder for exact positioning of the mini-map in fullscreen mode."), _compass.miniMapFullScreenPlaceholder, typeof(RectTransform), true);
                GUI.enabled = _compass.miniMapFullScreenPlaceholder == null;
                _compass.miniMapKeepAspectRatio = EditorGUILayout.Toggle(new GUIContent("Keep Aspect Ratio", "Keep aspect ration in full screen mode."), _compass.miniMapKeepAspectRatio);
                _compass.miniMapFullScreenWorldCenterFollows = EditorGUILayout.Toggle(new GUIContent("Center On Followed", "Forces center of the world map to be the same position of the followed object."), _compass.miniMapFullScreenWorldCenterFollows);
                GUI.enabled = !_compass.miniMapFullScreenWorldCenterFollows;
                _compass.miniMapFullScreenWorldCenter = EditorGUILayout.Vector3Field(new GUIContent("World Center", "Center of the world map."), _compass.miniMapFullScreenWorldCenter);
                GUI.enabled = true;
                _compass.miniMapFullScreenWorldSize = EditorGUILayout.Vector3Field(new GUIContent("World Size", "Size of the world map."), _compass.miniMapFullScreenWorldSize);
                _compass.miniMapFullScreenZoomLevel = EditorGUILayout.Slider(new GUIContent("Zoom Level", "The zoom level used for the mini-map in full screen mode."), _compass.miniMapFullScreenZoomLevel, 0, 1f);

                _compass.miniMapFullScreenFreezeCamera = EditorGUILayout.Toggle(new GUIContent("Freeze Camera Rotation", "Prevents camera rotation while in full screen mode."), _compass.miniMapFullScreenFreezeCamera);
                _compass.miniMapStyleFullScreenMode = (MINIMAP_STYLE)EditorGUILayout.EnumPopup(new GUIContent("Full Screen Style", "Mini-map style."), _compass.miniMapStyleFullScreenMode);
                if (_compass.miniMapStyleFullScreenMode == MINIMAP_STYLE.Custom) {
                    _compass.miniMapBorderTextureFullScreenMode = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("   Border Texture", "Texture for the border of the mini map in full screen mode."), _compass.miniMapBorderTextureFullScreenMode, typeof(Texture2D), false);
                    _compass.miniMapMaskSpriteFullScreenMode = (Sprite)EditorGUILayout.ObjectField(new GUIContent("   Mask Texture", "Mask for the border of the mini map in full screen mode."), _compass.miniMapMaskSpriteFullScreenMode, typeof(Sprite), false);
                }
                _compass.miniMapStyleFullScreenContents = (MINIMAP_CONTENTS)EditorGUILayout.EnumPopup(new GUIContent("Contents", "What to show when minimap is in full screen mode."), _compass.miniMapStyleFullScreenContents);
                if (_compass.miniMapStyleFullScreenContents == MINIMAP_CONTENTS.WorldMappedTexture) {
                    _compass.miniMapStyleFullScreenContentsTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("   Texture", "The texture to be used as background for the minimap in full screen mode."), _compass.miniMapStyleFullScreenContentsTexture, typeof(Texture), false);
                    if (_compass.miniMapCamera != null && !_compass.miniMapCamera.orthographic) {
                        EditorGUILayout.HelpBox("Feature only available with mini camera set to orthographic mode.", MessageType.Warning);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            GUI.enabled = _compass.showMiniMap;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            expandFogOfWarSettings = DrawTitleLabel(expandFogOfWarSettings, "Mini-Map Fog Of War Settings");
            if (GUILayout.Button("Help", GUILayout.Width(50))) {
                EditorUtility.DisplayDialog("Mini-Map Fog Of War", "This feature renders animated fog on top of the mini-map. To clear certain areas from fog, select option menu GameObject -> Create Other -> Compass Navigator Pro -> Mini-Map Fog of War Volume.\n\nReposition/scale fog volumes anywhere on the map.\n\nYou can also use scripting to control fog of war opacity at any position. Please check the documentation for details.", "Ok");
            }
            EditorGUILayout.EndHorizontal();

            if (expandFogOfWarSettings) {
                EditorGUILayout.BeginHorizontal();
                _compass.fogOfWarEnabled = EditorGUILayout.Toggle(new GUIContent("Enable", "Enables fog of war."), _compass.fogOfWarEnabled);
                if (_compass.fogOfWarEnabled) {
                    if (GUILayout.Button("Fit to Active Terrain")) {
                        FitFogOfWarLayerToActiveTerrain();
                    }
                    if (GUILayout.Button("Redraw")) {
                        _compass.UpdateFogOfWar();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (_compass.fogOfWarEnabled && _compass.miniMapCameraMode != MINIMAP_CAMERA_MODE.Orthographic) {
                    EditorGUILayout.HelpBox("Fog of war requires mini-map in orthographic mode.", MessageType.Warning);
                }

                _compass.fogOfWarCenter = EditorGUILayout.Vector3Field(new GUIContent("Center", "Center of the fog of war layer (only X/Z coordinates are used)"), _compass.fogOfWarCenter);
                _compass.fogOfWarSize = EditorGUILayout.Vector3Field(new GUIContent("Size", "Size of the fog of war layer (only X/Z coordinates are used)"), _compass.fogOfWarSize);
                _compass.fogOfWarTextureSize = EditorGUILayout.IntField(new GUIContent("Resolution", "Resolution for the fog of war texture effect."), _compass.fogOfWarTextureSize);
                _compass.fogOfWarDefaultAlpha = EditorGUILayout.Slider(new GUIContent("Default Opacity", "Default alpha value of fog of war in the scene."), _compass.fogOfWarDefaultAlpha, 0f, 1f);
                _compass.fogOfWarColor = EditorGUILayout.ColorField(new GUIContent("Fog Color", "Tint color."), _compass.fogOfWarColor);
                _compass.fogOfWarAutoClear = EditorGUILayout.Toggle(new GUIContent("Auto Clear", "Clears fog automatically as player crosses it."), _compass.fogOfWarAutoClear);

                if (_compass.fogOfWarAutoClear) {
                    _compass.fogOfWarAutoClearRadius = EditorGUILayout.FloatField("   Clear Radius", _compass.fogOfWarAutoClearRadius);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            GUI.enabled = true;

            if (_compass.isDirty) {
                EditorUtility.SetDirty(target);
                if (!Application.isPlaying) {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                }
            }
        }

        Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        GUIStyle titleLabelStyle;

        bool DrawTitleLabel(bool expanded, string s) {
            return EditorGUILayout.Foldout(expanded, new GUIContent(s), sectionHeaderStyle);

        }

        LayerMask LayerMaskField(GUIContent content, LayerMask layerMask) {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++) {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "") {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            if (layerMask.value == -1) {
                maskWithoutEmpty = -1;
            } else {
                for (int i = 0; i < layerNumbers.Count; i++) {
                    if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                        maskWithoutEmpty |= (1 << i);
                }
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(content, maskWithoutEmpty, layers.ToArray());
            if (maskWithoutEmpty == -1)
                return -1;

            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

        void FitFogOfWarLayerToActiveTerrain() {
            Terrain activeTerrain = Terrain.activeTerrain;
            if (activeTerrain == null) {
                EditorUtility.DisplayDialog("Fit to Terrain", "No active terrain found!", "Ok");
                return;
            }
            Vector3 size = activeTerrain.terrainData.size;
            _compass.fogOfWarCenter = new Vector3(activeTerrain.transform.position.x + size.x * 0.5f, 0, activeTerrain.transform.position.z + size.z * 0.5f);
            _compass.fogOfWarSize = new Vector3(size.x, 0, size.z);
        }

        public static void CheckLUTSettings(Texture2D tex) {
            if (Application.isPlaying || tex == null)
                return;
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path))
                return;
            TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
            if (imp == null)
                return;
            if (imp.textureType != TextureImporterType.Default || imp.sRGBTexture || imp.mipmapEnabled || imp.textureCompression != TextureImporterCompression.Uncompressed || imp.wrapMode != TextureWrapMode.Clamp || imp.filterMode != FilterMode.Bilinear) {
                EditorGUILayout.HelpBox("Texture has invalid import settings.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix Texture Import Settings", GUILayout.Width(200))) {
                    imp.textureType = TextureImporterType.Default;
                    imp.sRGBTexture = false;
                    imp.mipmapEnabled = false;
                    imp.textureCompression = TextureImporterCompression.Uncompressed;
                    imp.wrapMode = TextureWrapMode.Clamp;
                    imp.filterMode = FilterMode.Bilinear;
                    imp.anisoLevel = 0;
                    imp.SaveAndReimport();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }


    }

}

using UnityEngine;
using System;
using System.Collections.Generic;

namespace CompassNavigatorPro {

    public enum COMPASS_STYLE {
        Angled = 0,
        Rounded = 1,
        Celtic_White = 2,
        Celtic_Black = 3,
        Fantasy1 = 10,
        Fantasy2 = 11,
        Fantasy3 = 12,
        Fantasy4 = 13,
        SciFi1 = 20,
        SciFi2 = 21,
        SciFi3 = 22,
        SciFi4 = 23,
        SciFi5 = 24,
        SciFi6 = 27,
        Custom = 99
    }

    public enum WORLD_MAPPING_MODE {
        LimitedToBarWidth = 0,
        CameraFustrum = 1,
        Full180Degrees = 2,
        Full360Degrees = 3
    }

    public enum UPDATE_INTERVAL {
        NumberOfFrames,
        Time,
        Continuous,
        Scripting
    }

    public enum MINIMAP_CONTENTS {
        WorldView = 0,
        WorldMappedTexture = 1,
        UITexture = 2
    }

    public partial class CompassPro : MonoBehaviour {


        #region Public properties


        [SerializeField]
        Camera _cameraMain;

        public Camera cameraMain {
            get {
                if (_cameraMain == null) {
                    _cameraMain = Camera.main;
                    if (_cameraMain == null) {
                        _cameraMain = FindActiveObjectOfType(typeof(Camera)) as Camera;
                    }
                }
                return _cameraMain;
            }
            set {
                if (_cameraMain != value) {
                    _cameraMain = value;
                    needUpdateBarContents = true;
                }
            }
        }


        [SerializeField]
        UPDATE_INTERVAL _updateInterval = UPDATE_INTERVAL.NumberOfFrames;

        public UPDATE_INTERVAL updateInterval {
            get { return _updateInterval; }
            set {
                if (value != _updateInterval) {
                    _updateInterval = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        int _updateIntervalFrameCount = 60;

        public int updateIntervalFrameCount {
            get { return _updateIntervalFrameCount; }
            set {
                if (value != _updateIntervalFrameCount) {
                    _updateIntervalFrameCount = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _updateIntervalTime = 0.2f;

        public float updateIntervalTime {
            get { return _updateIntervalTime; }
            set {
                if (value != _updateIntervalTime) {
                    _updateIntervalTime = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        COMPASS_STYLE _style = COMPASS_STYLE.Celtic_White;

        public COMPASS_STYLE style {
            get { return _style; }
            set {
                if (value != _style) {
                    _style = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        [Range(0, 360)]
        float _northDegrees = 0f;

        /// <summary>
        /// Gets or sets the North position
        /// </summary>
        public float northDegrees {
            get { return _northDegrees; }
            set {
                if (value != _northDegrees) {
                    _northDegrees = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _visibleDistance = 500f;

        /// <summary>
        /// Gets or sets the maximum distance to a POI so it's visible in the compass bar.
        /// </summary>
        public float visibleDistance {
            get { return _visibleDistance; }
            set {
                if (value != _visibleDistance) {
                    _visibleDistance = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _visibleMinDistance;

        /// <summary>
        /// Gets or sets the minimum distance to a POI so it's visible in the compass bar.
        /// </summary>
        public float visibleMinDistance {
            get { return _visibleMinDistance; }
            set {
                if (value != _visibleMinDistance) {
                    _visibleMinDistance = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _nearDistance = 75f;

        /// <summary>
        /// Gets or sets the distance to a POI where the icon will start to grow as player approaches.
        /// </summary>
        public float nearDistance {
            get { return _nearDistance; }
            set {
                if (value != _nearDistance) {
                    _nearDistance = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _visitedDistance = 25f;

        /// <summary>
        /// Gets or sets the minimum distance required to consider a POI as visited. Once the player gets near this POI below this distance, the POI will be marked as visited.
        /// </summary>
        public float visitedDistance {
            get { return _visitedDistance; }
            set {
                if (value != _visitedDistance) {
                    _visitedDistance = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _gizmoScale = 0.25f;

        /// <summary>
        /// Gets or sets the gizmo scale during playmode.
        /// </summary>
        public float gizmoScale {
            get { return _gizmoScale; }
            set {
                if (value != _gizmoScale) {
                    _gizmoScale = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _alpha = 1.0f;

        /// <summary>
        /// The alpha (transparency) of the compass bar. Setting this value will make the bar shift smoothly from current alpha to the new value (see fadeDuration).
        /// </summary>
        public float alpha {
            get { return _alpha; }
            set {
                if (value != _alpha) {
                    _alpha = value;
                    UpdateCompassBarAlpha();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _autoHide;

        /// <summary>
        /// If no POIs are below the visible distance param, hide the compass bar
        /// </summary>
        public bool autoHide {
            get { return _autoHide; }
            set {
                if (value != _autoHide) {
                    _autoHide = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _fadeDuration = 2.0f;

        /// <summary>
        /// Sets the duration for any alpha change.
        /// </summary>
        public float fadeDuration {
            get { return _fadeDuration; }
            set {
                if (value != _fadeDuration) {
                    _fadeDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _alwaysVisibleInEditMode = true;

        /// <summary>
        /// Set this value to true to make the compass bar always visible in Edit Mode (ignores alpha property while editing).
        /// </summary>
        public bool alwaysVisibleInEditMode {
            get { return _alwaysVisibleInEditMode; }
            set {
                if (value != _alwaysVisibleInEditMode) {
                    _alwaysVisibleInEditMode = value;
                    UpdateCompassBarAlpha();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _verticalPosition = 0.97f;

        /// <summary>
        /// Distance in % of the screen from the bottom edge of the screen.
        /// </summary>
        public float verticalPosition {
            get { return _verticalPosition; }
            set {
                if (value != _verticalPosition) {
                    _verticalPosition = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _horizontalPosition;

        /// <summary>
        /// Horizontal position from the center of screen
        /// </summary>
        public float horizontalPosition {
            get { return _horizontalPosition; }
            set {
                if (value != _horizontalPosition) {
                    _horizontalPosition = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _bendAmount;

        /// <summary>
        /// Bending amount
        /// </summary>
        public float bendFactor {
            get { return _bendAmount; }
            set {
                if (value != _bendAmount) {
                    _bendAmount = value;
                    if (_bendAmount == 0) {
                        _verticalPosition = 0.94f;
                    }
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _width = 0.65f;

        /// <summary>
        /// Width of the compass bar in % of the screen width.
        /// </summary>
        public float width {
            get { return _width; }
            set {
                if (value != _width) {
                    _width = value;
                    UpdateCompassBarAppearance();
                    UpdateHalfWindsAppearance();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _height = 1f;

        /// <summary>
        /// Vertical scale of the compass bar.
        /// </summary>
        public float height {
            get { return _height; }
            set {
                if (value != _height) {
                    _height = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _edgeFadeOut;

        /// <summary>
        /// Enabled edge fade out effect
        /// </summary>
        public bool edgeFadeOut {
            get { return _edgeFadeOut; }
            set {
                if (value != _edgeFadeOut) {
                    _edgeFadeOut = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }




        [SerializeField]
        bool _edgeFadeOutText = true;

        /// <summary>
        /// If the edge fade out should affect the title and text below the compass bar
        /// </summary>
        public bool edgeFadeOutText {
            get { return _edgeFadeOutText; }
            set {
                if (value != _edgeFadeOutText) {
                    _edgeFadeOutText = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _edgeFadeOutWidth;

        /// <summary>
        /// Width of the edge fade out
        /// </summary>
        public float edgeFadeOutWidth {
            get { return _edgeFadeOutWidth; }
            set {
                if (value != _edgeFadeOutWidth) {
                    _edgeFadeOutWidth = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _edgeFadeOutStart = 0;

        /// <summary>
        /// Start of the edge fade out
        /// </summary>
        public float edgeFadeOutStart {
            get { return _edgeFadeOutStart; }
            set {
                if (value != _edgeFadeOutStart) {
                    _edgeFadeOutStart = value;
                    UpdateCompassBarAppearance();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _endCapsWidth = 54f;

        /// <summary>
        /// Width of the end caps for the compass bar.
        /// </summary>
        public float endCapsWidth {
            get { return _endCapsWidth; }
            set {
                if (value != _endCapsWidth) {
                    _endCapsWidth = value;
                    UpdateCompassBarAppearance();
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _showCardinalPoints = true;

        /// <summary>
        /// Whether cardinal points (N, W, S, E) should be visible in the compass bar
        /// </summary>
        public bool showCardinalPoints {
            get { return _showCardinalPoints; }
            set {
                if (value != _showCardinalPoints) {
                    _showCardinalPoints = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _showOrdinalPoints = false;

        /// <summary>
        /// Whether intercadinal (or ordinal) points (NE, NW, SW, SE) should be visible in the compass bar
        /// </summary>
        public bool showOrdinalPoints {
            get { return _showOrdinalPoints; }
            set {
                if (value != _showOrdinalPoints) {
                    _showOrdinalPoints = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _cardinalScale = 1f;

        /// <summary>
        /// Scaling factor for cardinal letters
        /// </summary>
        public float cardinalScale {
            get { return _cardinalScale; }
            set {
                if (value != _cardinalScale) {
                    _cardinalScale = value;
                    UpdateCompassBarContents();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _ordinalScale = 1f;

        /// <summary>
        /// Scaling factor for ordinal letters
        /// </summary>
        public float ordinalScale {
            get { return _ordinalScale; }
            set {
                if (value != _ordinalScale) {
                    _ordinalScale = value;
                    UpdateCompassBarContents();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _cardinalPointsVerticalOffset;

        /// <summary>
        /// Optional vertical offset for compass cardinal/ordinal points
        /// </summary>
        public float cardinalPointsVerticalOffset {
            get { return _cardinalPointsVerticalOffset; }
            set {
                if (value != _cardinalPointsVerticalOffset) {
                    _cardinalPointsVerticalOffset = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _showHalfWinds = false;

        /// <summary>
        /// Whether bar ticks should be visible
        /// </summary>
        public bool showHalfWinds {
            get { return _showHalfWinds; }
            set {
                if (value != _showHalfWinds) {
                    _showHalfWinds = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField, Range(0.1f, 1f)]
        float _halfWindsHeight = 0.33f;

        /// <summary>
        /// The compass bar ticks height.
        /// </summary>
        public float halfWindsHeight {
            get { return _halfWindsHeight; }
            set {
                if (value != _halfWindsHeight) {
                    _halfWindsHeight = value;
                    UpdateHalfWindsAppearance();
                    isDirty = true;
                }
            }
        }

        [SerializeField, Range(1f, 5f)]
        float _halfWindsWidth = 1f;

        /// <summary>
        /// The compass bar ticks width.
        /// </summary>
        public float halfWindsWidth {
            get { return _halfWindsWidth; }
            set {
                if (value != _halfWindsWidth) {
                    _halfWindsWidth = value;
                    UpdateHalfWindsAppearance();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Color _halfWindsTintColor = new Color(1f, 1f, 1f, 0.5f);

        /// <summary>
        /// The compass bar ticks tint color.
        /// </summary>
        public Color halfWindsTintColor {
            get { return _halfWindsTintColor; }
            set {
                if (value != _halfWindsTintColor) {
                    _halfWindsTintColor = value;
                    UpdateHalfWindsAppearance();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _labelHotZone = 0.01f;

        /// <summary>
        /// The distance from the center of the compass bar where a POI label can be shown.
        /// </summary>
        public float labelHotZone {
            get { return _labelHotZone; }
            set {
                if (value != _labelHotZone) {
                    _labelHotZone = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _maxIconSize = 1.15f;

        /// <summary>
        /// Maximum icon size. Icons grow or shrinks in the compass bar depending on distance.
        /// </summary>
        public float maxIconSize {
            get { return _maxIconSize; }
            set {
                if (value != _maxIconSize) {
                    _maxIconSize = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _minIconSize = 0.5f;

        /// <summary>
        /// Minimum icon size. Icons grow or shrinks in the compass bar depending on distance.
        /// </summary>
        public float minIconSize {
            get { return _minIconSize; }
            set {
                if (value != _minIconSize) {
                    _minIconSize = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _scaleInDuration = 0.3f;

        /// <summary>
        /// Duration for the poi's icon scaling effect when it appears on the compass bar
        /// </summary>
        public float scaleInDuration {
            get { return _scaleInDuration; }
            set {
                if (value != _scaleInDuration) {
                    _scaleInDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        WORLD_MAPPING_MODE _worldMappingMode = WORLD_MAPPING_MODE.CameraFustrum;

        /// <summary>
        /// Set this value to true to consider the width of the bar equal to the width of the viewport so the angle is not reduced.
        /// </summary>
        public WORLD_MAPPING_MODE worldMappingMode {
            get { return _worldMappingMode; }
            set {
                if (value != _worldMappingMode) {
                    _worldMappingMode = value;
                    needUpdateBarContents = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _textVerticalPosition = -30;

        /// <summary>
        /// Vertical offset for the text of POIs when visited for first time
        /// </summary>
        public float textVerticalPosition {
            get { return _textVerticalPosition; }
            set {
                if (value != _textVerticalPosition) {
                    _textVerticalPosition = value;
                    UpdateTextAppearanceEditMode();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _textScale = 0.2f;

        /// <summary>
        /// Scaling applied to text
        /// </summary>
        public float textScale {
            get { return _textScale; }
            set {
                if (value != _textScale) {
                    _textScale = value;
                    UpdateTextAppearanceEditMode();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _letterSpacing = 1f;

        /// <summary>
        /// Scaling applied to animated letters
        /// </summary>
        public float letterSpacing {
            get { return _letterSpacing; }
            set {
                if (value != _letterSpacing) {
                    _letterSpacing = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _textRevealEnabled = true;

        /// <summary>
        /// Enabled text revealing effect when discovering a POI for the first time
        /// </summary>
        public bool textRevealEnabled {
            get { return _textRevealEnabled; }
            set {
                if (value != _textRevealEnabled) {
                    _textRevealEnabled = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _textRevealDuration = 0.5f;

        /// <summary>
        /// Duration of the text reveal
        /// </summary>
        public float textRevealDuration {
            get { return _textRevealDuration; }
            set {
                if (value != _textRevealDuration) {
                    _textRevealDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _textRevealLetterDelay = 0.05f;

        /// <summary>
        /// Delay in appearance of each letter during a text reveal
        /// </summary>
        public float textRevealLetterDelay {
            get { return _textRevealLetterDelay; }
            set {
                if (value != _textRevealLetterDelay) {
                    _textRevealLetterDelay = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _textDuration = 5f;

        /// <summary>
        /// Duration of the text on screen before fading out
        /// </summary>
        public float textDuration {
            get { return _textDuration; }
            set {
                if (value != _textDuration) {
                    _textDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float _textFadeOutDuration = 2f;

        /// <summary>
        /// Duration of the text fade out
        /// </summary>
        public float textFadeOutDuration {
            get { return _textFadeOutDuration; }
            set {
                if (value != _textFadeOutDuration) {
                    _textFadeOutDuration = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _textShadowEnabled = true;

        /// <summary>
        /// Shows a drop shadow under the text
        /// </summary>
        public bool textShadowEnabled {
            get { return _textShadowEnabled; }
            set {
                if (value != _textShadowEnabled) {
                    _textShadowEnabled = value;
                    if (!Application.isPlaying) {
                        UpdateTextAppearanceEditMode();
                    }
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Font _textFont;

        /// <summary>
        /// Font for the text
        /// </summary>
        public Font textFont {
            get {
                if (_textFont == null) {
                    _textFont = Resources.Load<Font>("CNPro/Fonts/Vollkorn-Regular");
                }
                return _textFont;
            }
            set {
                if (value != _textFont) {
                    _textFont = value;
                    UpdateTextAppearanceEditMode();
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        float _titleVerticalPosition = 18f;

        /// <summary>
        /// Vertical offset for the title of the (visited/known) centered POI in the compass bar
        /// </summary>
        public float titleVerticalPosition {
            get { return _titleVerticalPosition; }
            set {
                if (value != _titleVerticalPosition) {
                    _titleVerticalPosition = value;
                    UpdateTitleAppearanceEditMode();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _titleScale = 0.1f;

        /// <summary>
        /// Scaling applied to title
        /// </summary>
        public float titleScale {
            get { return _titleScale; }
            set {
                if (value != _titleScale) {
                    _titleScale = value;
                    UpdateTitleAppearanceEditMode();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Font _titleFont;

        /// <summary>
        /// Font for the title
        /// </summary>
        public Font titleFont {
            get {
                if (_titleFont == null) {
                    _titleFont = Resources.Load<Font>("CNPro/Fonts/Actor-Regular");
                }
                return _titleFont;
            }
            set {
                if (value != _titleFont) {
                    _titleFont = value;
                    UpdateTitleAppearanceEditMode();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _titleShadowEnabled = true;

        /// <summary>
        /// Shows a drop shadow under the title
        /// </summary>
        public bool titleShadowEnabled {
            get { return _titleShadowEnabled; }
            set {
                if (value != _titleShadowEnabled) {
                    _titleShadowEnabled = value;
                    if (!Application.isPlaying) {
                        UpdateTitleAppearanceEditMode();
                    }
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _use3Ddistance = false;

        /// <summary>
        /// Check whether 3D distance should be computed instead of planar X/Z distance.
        /// </summary>
        public bool use3Ddistance {
            get { return _use3Ddistance; }
            set {
                if (value != _use3Ddistance) {
                    _use3Ddistance = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _sameAltitudeThreshold = 3f;

        /// <summary>
        /// Minimum difference in altitude from camera to show "above" or "below"
        /// </summary>
        public float sameAltitudeThreshold {
            get { return _sameAltitudeThreshold; }
            set {
                if (value != _sameAltitudeThreshold) {
                    _sameAltitudeThreshold = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _showDistance = false;

        /// <summary>
        /// Whether the distance in meters should be shown next to the title
        /// </summary>
        public bool showDistance {
            get { return _showDistance; }
            set {
                if (value != _showDistance) {
                    _showDistance = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _showDistanceFormat = "F1";

        /// <summary>
        /// The string format for displaying the distance. A value of F0 means "Fixed/Decimal with 0 decimal positions". A value of F1 includes 1 decimal position.
        /// This string format corresponds with the available options for ToString(format) method of C#.
        /// </summary>
        public string showDistanceFormat {
            get { return _showDistanceFormat; }
            set {
                if (value != _showDistanceFormat) {
                    _showDistanceFormat = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        AudioClip _visitedDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play when a POI is visited the first time. Note that you can specify a different audio clip in the POI script itself.
        /// </summary>
        public AudioClip visitedDefaultAudioClip {
            get { return _visitedDefaultAudioClip; }
            set {
                if (value != _visitedDefaultAudioClip) {
                    _visitedDefaultAudioClip = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        AudioClip _beaconDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play when a POI beacon is shown (see manual for more info about POI beacons).
        /// </summary>
        public AudioClip beaconDefaultAudioClip {
            get { return _beaconDefaultAudioClip; }
            set {
                if (value != _beaconDefaultAudioClip) {
                    _beaconDefaultAudioClip = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        AudioClip _heartbeatDefaultAudioClip;

        /// <summary>
        /// Default audio clip to play for the heartbeat effect. This effect is enabled on each POI and will play a custom sound with variable speed depending on distance.
        /// </summary>
        public AudioClip heartbeatDefaultAudioClip {
            get { return _heartbeatDefaultAudioClip; }
            set {
                if (value != _heartbeatDefaultAudioClip) {
                    _heartbeatDefaultAudioClip = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _dontDestroyOnLoad;

        /// <summary>
        /// Preserves compass bar between scene changes
        /// </summary>
        public bool dontDestroyOnLoad {
            get { return _dontDestroyOnLoad; }
            set {
                if (value != _dontDestroyOnLoad) {
                    _dontDestroyOnLoad = value;
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Keeps a reference to the nearest POI to the camera
        /// </summary>
        [NonSerialized]
        public CompassProPOI nearestPOI;

        #endregion

        #region Events

        /// <summary>
        /// Event fired when this POI is visited.
        /// </summary>
        public Action<CompassProPOI> OnPOIVisited;

        /// <summary>
        /// Event fired when the POI appears in the compass bar (gets near than the visible distance)
        /// </summary>
        public Action<CompassProPOI> OnPOIVisible;

        /// <summary>
        /// Event fired when POI disappears from the compass bar (gets farther than the visible distance)
        /// </summary>
        public Action<CompassProPOI> OnPOIHide;

        /// <summary>
        /// Event fired when mouse enters an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseEnter;

        /// <summary>
        /// Event fired when mouse exits an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseExit;

        /// <summary>
        /// Event fired when button is pressed on an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseDown;

        /// <summary>
        /// Event fired when button is released on an icon on the miniMap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseUp;

        /// <summary>
        /// Event fired when an icon is clicked on the minimap
        /// </summary>
        public Action<CompassProPOI> OnPOIMiniMapIconMouseClick;

        #endregion

        #region Public API

        /// <summary>
        /// Gets a reference to the Compass API.
        /// </summary>
        public static CompassPro instance {
            get {
                if (_instance == null) {
                    _instance = FindActiveObjectOfType(typeof(CompassPro)) as CompassPro;
                }
                return _instance;
            }
        }

        /// <summary>
        /// Call this to force a refresh of contents
        /// </summary>
        public void Refresh() {
            needUpdateBarContents = true;
        }

        /// <summary>
        /// Used to add a POI to the compass. Returns false if POI is already registered.
        /// </summary>
        public bool POIRegister(CompassProPOI newPOI) {
            if (icons == null)
                return false;
            int iconsCount = icons.Count;
            for (int k = 0; k < iconsCount; k++) {
                if (icons[k].poi == newPOI) {
                    return false;
                }
            }
            CompassActiveIcon newIcon = new CompassActiveIcon(newPOI);
            icons.Add(newIcon);
            needsIconSorting = true;
            return true;
        }

        /// <summary>
        /// Returns whether the POI is currently registered.
        /// </summary>
        public bool POIisRegistered(CompassProPOI poi) {
            if (icons == null)
                return false;
            int iconsCount = icons.Count;
            for (int k = 0; k < iconsCount; k++) {
                if (icons[k].poi.id == poi.id) {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Updates POI rendering order. Call this method if you change the priority property manually.
        /// </summary>
        public void POIResort() {
            needsIconSorting = true;
        }

        /// <summary>
        /// Call this method to remove a POI from the compass.
        /// </summary>
        public void POIUnregister(CompassProPOI newPOI) {
            if (icons == null)
                return;
            int iconsCount = icons.Count;
            for (int k = 0; k < iconsCount; k++) {
                if (icons[k].poi == newPOI) {
                    if (icons[k].rectTransform != null && icons[k].rectTransform.gameObject != null) {
                        DestroyImmediate(icons[k].rectTransform.gameObject);
                    }
                    if (icons[k].miniMapRectTransform != null && icons[k].miniMapRectTransform.gameObject != null) {
                        DestroyImmediate(icons[k].miniMapRectTransform.gameObject);
                    }
                    icons.RemoveAt(k);
                    break;
                }
            }
        }

        /// <summary>
        /// Shows given POI as gizmo in the scene and makes its icon always visible in the compass bar
        /// </summary>
        public void POIFocus(CompassProPOI existingPOI) {
            if (icons == null)
                return;
            int iconsCount = icons.Count;
            for (int k = 0; k < iconsCount; k++) {
                if (icons[k].poi == existingPOI) {
                    icons[k].poi.showPlayModeGizmo = true;
                    icons[k].poi.clampPosition = true;
                } else if (icons[k].poi != null) {
                    icons[k].poi.showPlayModeGizmo = false;
                    icons[k].poi.clampPosition = false;
                }
            }
        }

        /// <summary>
        /// Clears all gizmos and unfocus any focused POI.
        /// </summary>
        public void POIBlur() {
            if (icons == null)
                return;
            int iconsCount = icons.Count;
            for (int k = 0; k < iconsCount; k++) {
                if (icons[k].poi != null) {
                    icons[k].poi.showPlayModeGizmo = false;
                    icons[k].poi.clampPosition = false;
                }
            }
        }

        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public GameObject POIShowBeacon(CompassProPOI existingPOI, float duration, float horizontalScale = 1f) {
            return POIShowBeacon(existingPOI, duration, horizontalScale, 1f, Color.white);
        }

        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public GameObject POIShowBeacon(CompassProPOI existingPOI, float duration, float horizontalScale, float intensity, Color tintColor) {
            Transform beacon = existingPOI.transform.Find("POIBeacon");
            if (beacon != null)
                return beacon.gameObject;

            GameObject beaconObj = Instantiate(Resources.Load<GameObject>("CNPro/Prefabs/POIBeacon"));
            beaconObj.name = "POIBeacon";
            beacon = beaconObj.transform;
            beacon.localScale = new Vector3(beacon.localScale.x * horizontalScale, beacon.localScale.y, beacon.localScale.z);
            beacon.position = existingPOI.transform.position + Misc.Vector3up * beacon.transform.localScale.y * 0.5f;
            beacon.SetParent(existingPOI.transform, true);
            BeaconAnimator anim = beacon.gameObject.GetComponent<BeaconAnimator>();
            anim.duration = duration;
            anim.tintColor = tintColor;
            anim.intensity = intensity;

            if (audioSource != null) {
                if (existingPOI.beaconAudioClip != null) {
                    audioSource.PlayOneShot(existingPOI.beaconAudioClip);
                } else if (_beaconDefaultAudioClip != null) {
                    audioSource.PlayOneShot(_beaconDefaultAudioClip);
                }
            }

            return beaconObj;
        }


        /// <summary>
        /// Show a light beacon over the specified POI.
        /// </summary>
        public void POIShowBeacon(Vector3 position, float duration, float horizontalScale, float intensity, Color tintColor) {
            string beaconName = "POIBeacon " + position;
            GameObject beaconObj = GameObject.Find(beaconName);
            if (beaconObj != null)
                return;

            beaconObj = Instantiate(Resources.Load<GameObject>("CNPro/Prefabs/POIBeacon"));
            beaconObj.name = beaconName;
            Transform beacon = beaconObj.transform;
            beacon.localScale = new Vector3(beacon.localScale.x * horizontalScale, beacon.localScale.y, beacon.localScale.z);
            beacon.position = position + Misc.Vector3up * beacon.transform.localScale.y * 0.5f;
            BeaconAnimator anim = beacon.gameObject.GetComponent<BeaconAnimator>();
            anim.duration = duration;
            anim.tintColor = tintColor;
            anim.intensity = intensity;

            if (audioSource != null) {
                if (_beaconDefaultAudioClip != null) {
                    audioSource.PlayOneShot(_beaconDefaultAudioClip);
                }
            }
        }



        /// <summary>
        /// Show a light beacon over all non-visited POIs for duration in seconds and with optional custom horizontal scale for the bright cylinder.
        /// </summary>
        public void POIShowBeacon(float duration, float horizontalScale = 1f) {
            POIShowBeacon(duration, horizontalScale, 1f, Color.white);
        }

        /// <summary>
        /// Show a light beacon over all non-visited POIs for duration in seconds and with optional custom horizontal scale for the bright cylinder.
        /// </summary>
        public void POIShowBeacon(float duration, float horizontalScale, float intensity, Color tintColor) {
            for (int k = 0; k < icons.Count; k++) {
                CompassActiveIcon icon = icons[k];
                if (icon == null || icon.poi.isVisited || !icon.poi.isVisible)
                    continue;
                POIShowBeacon(icon.poi, duration, horizontalScale, intensity, tintColor);
            }
        }


        /// <summary>
        /// Initiates a fade in effect with duration in seconds.
        /// </summary>
        public void FadeIn(float duration) {
            fadeDuration = duration;
            fadeStartTime = Time.time;
            prevAlpha = canvasGroup.alpha;
            alpha = 1f;
        }

        /// <summary>
        /// Initiates a fade out effect with duration in seconds.
        /// </summary>
        public void FadeOut(float duration) {
            fadeDuration = duration;
            fadeStartTime = Time.time;
            prevAlpha = canvasGroup.alpha;
            alpha = 0f;
        }


        public void ShowAnimatedText(string text) {
            StartCoroutine(AnimateDiscoverText(text));
        }


        public Canvas canvas {
            get {
                return _canvas;
            }
        }


        /// <summary>
        /// Returns the degrees for the current compass orientation
        /// </summary>
        public float degrees {
            get {
                float angles = _cameraMain.transform.eulerAngles.y;
                return (angles + 360f + _northDegrees) % 360;
            }
        }

        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetVisited(List<CompassProPOI>pois) {
            pois.Clear();
            foreach(CompassActiveIcon icon in icons) {
                if (icon != null && icon.poi != null && icon.poi.isVisited) pois.Add(icon.poi);
            }
        }


        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetUnvisited(List<CompassProPOI> pois) {
            pois.Clear();
            foreach (CompassActiveIcon icon in icons) {
                if (icon != null && icon.poi != null && !icon.poi.isVisited) pois.Add(icon.poi);
            }
        }


        /// <summary>
        /// Returns a list of visited POIs
        /// </summary>
        /// <param name="pois"></param>
        public void POIGetAll(List<CompassProPOI> pois) {
            pois.Clear();
            foreach (CompassActiveIcon icon in icons) {
                if (icon != null && icon.poi != null) pois.Add(icon.poi);
            }
        }



        #endregion


    }

}




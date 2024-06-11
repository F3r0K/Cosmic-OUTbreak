using UnityEngine;

namespace CompassNavigatorPro {

    public partial class CompassPro : MonoBehaviour {

        static class ShaderParams {
            public static int Rotation = Shader.PropertyToID("_Rotation");
            public static int UVOffset = Shader.PropertyToID("_UVOffset");
            public static int UVFogOffset = Shader.PropertyToID("_UVFogOffset");
            public static int FoWTexture = Shader.PropertyToID("_FogOfWarTex");
            public static int FoWTintColor = Shader.PropertyToID("_FogOfWarTintColor");
            public static int Effects = Shader.PropertyToID("_Effects");
            public static int LUTTexture = Shader.PropertyToID("_LUTTex");
            public static int MiniMapTex = Shader.PropertyToID("_MiniMapTex");

            public const string SKW_COMPASS_LUT = "COMPASS_LUT";
            public const string SKW_COMPASS_FOG_OF_WAR = "COMPASS_FOG_OF_WAR";
            public const string SKW_COMPASS_ROTATED = "COMPASS_ROTATED";

        }
    }



}




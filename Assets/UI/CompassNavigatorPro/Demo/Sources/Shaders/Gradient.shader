Shader "Unlit/Gradient"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DisableBatching"="True" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 y3 : TEXCOORD0;
			};

			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float ypos = mul(unity_ObjectToWorld, v.vertex).y;
				float ycenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).y;
				float ysize = mul(unity_ObjectToWorld, float4(1,1,1,1)).y;
				o.y3 = float3(ypos, ycenter, ysize);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color;
				float height = i.y3.z - i.y3.y;
				float dark = 1.0 - (i.y3.y + height * 0.5 - i.y3.x) / height;
				col.rgb *= 0.5 + dark * 0.5;
				return col;
			}
			ENDCG
		}
	}
}

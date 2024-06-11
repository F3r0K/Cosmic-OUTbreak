Shader "CompassNavigatorPro/Beacon"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmmisionColor("Emission Color", Color) = (1,1,1,1)
		_Intensity("Intensity", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvEmission : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			sampler2D _MainTex, _EmissionMap;
			float4 _MainTex_ST, _EmissionMap_ST;
			fixed4 _Color, _EmissionColor;
			fixed _Intensity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvEmission = TRANSFORM_TEX(v.uv, _EmissionMap);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				fixed4 emi = tex2D(_EmissionMap, i.uvEmission) * _EmissionColor;
				col += emi;
				col.a *= _Color.a;
				col *= _Intensity;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

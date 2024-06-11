Shader "CompassNavigatorPro/MiniMapOverlayUnlit"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_MiniMapTex ("MiniMap Render Texture", 2D) = "black" {}
		_BorderTex ("Border Texture", 2D) = "black" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}

		_FogOfWarTex ("Fog Of War Tex", 2D) = "black" {}
		_FogOfWarTintColor ("Fog Of War Color", Color) = (1,1,1,1)

		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_UVOffset ("UV Offset", Vector) = (0,0,0,1)
		_UVFogOffset("Fog Offset", Vector) = (0,0,0)
		_Rotation("Rotation", Float) = 0
		_Effects("Effects", Vector) = (1,1,0,0)

		_LUTTexture ("LUT Texture", 2D) = "black" {}


		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile _ UNITY_UI_ALPHACLIP
			#pragma multi_compile_local _ COMPASS_FOG_OF_WAR
			#pragma multi_compile_local _ COMPASS_ROTATED
			#pragma multi_compile_local _ COMPASS_LUT
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float2 mapUV    : TEXCOORD2;
				#if COMPASS_FOG_OF_WAR
					float2 fogUV    : TEXCOORD3;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4 _UVOffset;

			sampler2D _FogOfWarTex, _NoiseTex;
			fixed4 _FogOfWarTintColor;
			float4 _UVFogOffset;
			float _Rotation;
			fixed4 _Effects;
			sampler2D _LUTTex;
			float4 _LUTTex_TexelSize;

			#define dot2(x) dot(x,x)

			#if COMPASS_ROTATED
			float2 Rotate(float2 uv) {
				uv -= 0.5;
				float s, c;
				sincos(_Rotation, s, c);
				float2x2 rotationMatrix = float2x2(c, -s, s, c);
				uv = mul(uv, rotationMatrix);
				uv += 0.5; 
				return uv;
			}
			#else
				#define Rotate(v) v
			#endif

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				float2 rotatedUV = Rotate(IN.texcoord);

				float2 uv = rotatedUV - _UVOffset.xy / _UVOffset.z;
				uv.y = (uv.y-0.5) * _UVOffset.w + 0.5;
				uv = (uv-0.5) * _UVOffset.z + 0.5;
				OUT.mapUV = uv;

				#if COMPASS_FOG_OF_WAR
				uv = rotatedUV - _UVFogOffset.xy;
				uv.y = (uv.y-0.5) * _UVOffset.w + 0.5;
				uv = (uv-0.5) * _UVFogOffset.zw + 0.5;
				OUT.fogUV = uv; 
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex, _MiniMapTex, _BorderTex, _MaskTex;

			#if COMPASS_FOG_OF_WAR
			fixed4 GetFogOfWar(float2 uv) {
				fixed fogAlpha = tex2D (_FogOfWarTex, uv).a;
                half vxy = (uv.x + uv.y);
				half wt = _Time[1] * 0.5;
				half2 waveDisp1 = half2(wt + cos(wt+uv.y * 32.0) * 0.125, 0) * 0.05;
				fixed4 fog1 = tex2D(_NoiseTex, (uv + waveDisp1) * 8);
                wt *= 1.1;
				half2 waveDisp2 = half2(wt + cos(wt+uv.y * 8.0) * 0.5, 0) * 0.05;
				fixed4 fog2 = tex2D(_NoiseTex, (uv + waveDisp2) * 2);
                fixed4 fog = (fog1 + fog2) * 0.5;
                fog.rgb *= _FogOfWarTintColor;
                fog.a = fogAlpha;
                return fog;
            }
            #endif

			half4 frag(v2f IN) : SV_Target
			{
				half4 border = (tex2D(_BorderTex, IN.texcoord)+ _TextureSampleAdd) * IN.color;
				half4 mask = tex2D(_MaskTex, IN.texcoord);

				border.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
					clip (border.a - 0.001);
				#endif

				half4 minimapTex = tex2D(_MiniMapTex, IN.mapUV);
				minimapTex.rgb *= minimapTex.a;
				minimapTex.a = 1;
				// Clip out of range area
				minimapTex *= max( abs(IN.mapUV.x - 0.5), abs(IN.mapUV.y - 0.5))< 0.499;

				#if COMPASS_LUT
					float3 lutST = float3(_LUTTex_TexelSize.x, _LUTTex_TexelSize.y, _LUTTex_TexelSize.w - 1);
					float3 lookUp = saturate(minimapTex.rgb) * lutST.zzz;
    				lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
    				float slice = floor(lookUp.z);
    				lookUp.x += slice * lutST.y;
    				float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
    				float3 lut = lerp(tex2D(_LUTTex, lookUp.xy).rgb, tex2D(_LUTTex, lookUpNextSlice).rgb, lookUp.z - slice);
		    		minimapTex.rgb = lerp(minimapTex.rgb, lut, _Effects.z);
				#endif

				// Apply fog of war?
				#if COMPASS_FOG_OF_WAR
					half4 fogOfWar = GetFogOfWar(IN.fogUV);
					minimapTex = lerp(minimapTex, fogOfWar, fogOfWar.a);
				#endif

				// Brightness & Contrast for map
				minimapTex.rgb = (minimapTex.rgb - 0.5.xxx) * _Effects.y + 0.5.xxx;
				minimapTex.rgb *= _Effects.x;

				// Vignette
				minimapTex.rgb *= 1.0 - saturate( _Effects.w * dot2(dot2( 0.5 - IN.texcoord)));

				// Mask & border
				half4 color = mask * minimapTex * _Color;
				
				color = lerp(color, border, border.a);
				//color.a = max(border.a, mask.a);

				return color;
			}
		ENDCG
		}
	}
}
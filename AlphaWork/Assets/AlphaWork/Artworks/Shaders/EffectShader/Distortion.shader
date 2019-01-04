// Upgrade NOTE: replaced 'defined _BLENDMODE_ALPHA' with 'defined (_BLENDMODE_ALPHA)'
// Upgrade NOTE: replaced 'defined _BLENDMODE_ALPHAADD' with 'defined (_BLENDMODE_ALPHAADD)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Painting/Distortion" 
{
		Properties
		{
			_MainTex("Main Tex (RGB) Trans (A)", 2D) = "white" {}
			_NoiseTex("Noise Tex", 2D) = "white" {}
			[HDR]_Color("Color", Color) = (1, 1, 1, 1)
			_HeatTime("Heat Time", range(-2,2)) = 0
			_Strength("Strength", Range(0.01, 0.3)) = 0.2
			_Transparent("Transparent", Range(0.001, 0.1)) = 0.05

			_FresnelBias("FresnelBias", Range(0,1)) = 0.0
			_FresnelPower("FresnelPower", Range(0,16)) = 4.0
			_FresnelScale("FresnelScale", Range(0,16)) = 1.0

			//[KeywordEnum(AlphaAdd, Alpha)] _BlendMode("Blend Mode", Float) = 0
			[HideInInspector] _Mode("__mode", Float) = 0.0
			[HideInInspector] _SrcBlend("__src", Float) = 5.0
			[HideInInspector] _DstBlend("__dst", Float) = 1.0
		}
		SubShader
 		{
  			Tags {"Queue"="Transparent" "RenderType"="Transparent"}

			Pass
			{
				Name "BASE"
				Tags{ "LightMode" = "Always" }

				Fog{ Mode Off }
				ZWrite Off
				Lighting Off
				Cull Off
				Blend[_SrcBlend][_DstBlend]

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				//#pragma shader_feature _BLENDMODE_ALPHAADD _BLENDMODE_ALPHA
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _NoiseTex;
				float4 _MainTex_ST;
				float4 _Color;
				float4 _NoiseTex_ST;
				float _Strength;
				float _Transparent;
				float _HeatTime;

				float _FresnelBias;
				float _FresnelPower;
				float _FresnelScale;

				struct vin
				{
					float4 color : COLOR;
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 color : COLOR;
					float4 vertex : POSITION;
					float2 uv0 : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float distortion : TEXCOORD2;
					float3 wPos : TEXCOORD3;
					float3 wNormal : TEXCOORD4;
				};

				v2f vert(vin i)
				{
					v2f o;
					o.color = i.color;
					o.vertex = UnityObjectToClipPos(i.vertex);
					o.uv1 = TRANSFORM_TEX(i.texcoord, _NoiseTex);
					//float viewAngle = dot(normalize(ObjSpaceViewDir(i.vertex)), i.normal);
					//o.distortion = viewAngle * viewAngle;
					//float depth = -mul(UNITY_MATRIX_MV, i.vertex).z;
					//o.distortion /= (1.0 + depth);
					o.distortion = 1.0;
					o.distortion *= _Strength;
					//o.uv0 = o.vertex;
					o.uv0 = i.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;

					o.wPos = mul(unity_ObjectToWorld, i.vertex).xyz;
					o.wNormal = UnityObjectToWorldNormal(i.normal);

					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					float2 uv = i.uv0.xy;
					half4 offsetColor0 = tex2D(_NoiseTex, i.uv1 + _Time.xz / 10.0 * _HeatTime);
					half4 offsetColor1 = tex2D(_NoiseTex, i.uv1 + _Time.yx / 10.0 * _HeatTime);
					//clip(overhead);
					uv.x += ((offsetColor0.r + offsetColor1.r) - 1.0) * i.distortion;
					uv.y += ((offsetColor0.g + offsetColor1.g) - 1.0) * i.distortion;
					float3 toEye = normalize(_WorldSpaceCameraPos - i.wPos);
					float fresnel = _FresnelBias + _FresnelScale * pow(1.0 - max(abs(dot(i.wNormal, toEye)), 0.0f), _FresnelPower);//fresnel
					half4 colorTex = tex2D(_MainTex, uv);
					half4 col = (colorTex + fresnel*(colorTex.a + 0.1)) * i.color;

					col = saturate(col * _Color * (i.distortion / _Transparent)* 0.3);

					return col;
				}

				ENDCG
			}
		}
		CustomEditor "DistortionShaderGUI"
}
﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mark/HOC/Effect/Distortion_BG_Add"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "blakc" {}
		_TexCol ("Texture Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (1,1,1,1)
		_TexPow ("Texture intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-2,2)) = 1

		_DisTex ("Distortion Map", 2D) = "white" {}
		_DisPow ("Distortion intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-1,1)) = 1

		_Alphatex ("Transparent Mask", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent +1" "RenderType"="Transparent"}
		LOD 100


		Blend SrcAlpha OneMinusSrcAlpha 
		// Blend one one
		// ColorMask RGB
		ZWrite Off
		Cull back


		Fog { Mode Off }


		GrabPass {       
    	Name "BASE"
    
    }
		Pass
		{
		Name "BASE"
			





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
				float4 uv1 : TEXCOORD0;
				float4 uv2 : TEXCOORD2;
				
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _DisTex;
			sampler2D _Alphatex;
			sampler2D _GrabTexture; 

			fixed4 _TexCol;
			half _TexPow;
			half _DisPow;

			float4 _MainTex_ST;
			float4 _DisTex_ST;
			float4 _Alphatex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv1.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1.zw = TRANSFORM_TEX(v.uv, _DisTex);
				o.uv2.xy = TRANSFORM_TEX(v.uv, _Alphatex);

				float4 screenUV = ComputeGrabScreenPos(o.vertex);
				o.uv2.zw = screenUV.xy/screenUV.w;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 texuv = i.uv1.xy;
				float2 disuv = i.uv1.zw;
				float2 alpuv = i.uv2.xy;
				float2 nosuv = i.uv2.zw;
				 
				fixed dis = tex2D(_DisTex, disuv)*_DisPow;
				fixed dis2 = tex2D(_DisTex, 1-disuv)*_DisPow;;
				fixed alp = tex2D(_Alphatex, alpuv);
				nosuv.x += dis-dis2;
				nosuv.y += dis2-dis;

				fixed4 tex = tex2D(_MainTex, texuv+dis)*_TexCol*_TexPow;
				
				

				fixed4 col;

				col = tex2D(_GrabTexture, UNITY_PROJ_COORD(nosuv))+tex;

				col.a = alp*_TexCol.a;



				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

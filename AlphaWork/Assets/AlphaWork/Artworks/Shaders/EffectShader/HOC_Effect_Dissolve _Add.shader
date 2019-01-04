// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mark/HOC/Effect/Dissolve_IMG_Add"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "blakc" {}
		_TexCol ("Texture Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (1,1,1,1)
		_TexPow ("Texture Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-2,2)) = 1

		_DisTex ("Dissolve Map", 2D) = "white" {}
		_DisCol ("Dissolve Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (1,1,1,1)
		_DisPow ("Dissolve Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-1,3)) = 0
		_Disrange ("Dissolve Range >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,100)) = 1
		_DisWt ("Dissolve Width >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,0.5)) = 0

		// _L ("Lift Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _R ("Right Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _F ("Front Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _B ("Back Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _T ("Top Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _U ("Under Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0

		_Alphatex ("Transparent Mask", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent +1" "RenderType"="Transparent"}
		LOD 100


		// Blend SrcAlpha one 
		// BlendOp RevSub 
		// Blend  OneMinusDstColor OneMinusDstColor
		// Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha one
		ColorMask RGBa
		ZWrite Off
		Cull off

		Fog { Mode Off }





		
		Pass
		{
		Name "BASE"
			





			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// #pragma target 3.0
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				// float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD2;
				// float3 vcolor : COLOR;
				
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _DisTex;
			sampler2D _Alphatex;
			// sampler2D _GrabTexture; 

			fixed4 _TexCol;
			half _TexPow;
			half _DisPow;
			half _Disrange;
			half _DisWt;
			fixed4 _DisCol;

			// half _L;
			// half _R;
			// half _F;
			// half _B;
			// half _T;
			// half _U;

			float4 _MainTex_ST;
			float4 _DisTex_ST;
			float4 _Alphatex_ST;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// o.vcolor = smoothstep(-1,1,mul(UNITY_MATRIX_IT_MV,v.normal));//+mul(UNITY_MATRIX_IT_MV,-v.vertex);

				o.uv1.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1.zw = TRANSFORM_TEX(v.uv, _DisTex);
				o.uv2.xy = TRANSFORM_TEX(v.uv, _Alphatex);

				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 texuv = i.uv1.xy;
				float2 disuv = i.uv1.zw;
				float2 alpuv = i.uv2.xy;

				// half L = pow((1-i.vcolor.r),_L*5)*(1-_L*0.1);
				// half R = pow((i.vcolor.r),_R*5)*(1-_R*0.1);
				// half B = pow((i.vcolor.z),_B*5)*(1-_B*0.1);
				// half F = pow((1-i.vcolor.z),_F*0.1)*(1-_F*0.1);
				// half T = pow((1-i.vcolor.y),_T*5)*(1-_T*0.1);
				// half U = pow((i.vcolor.y),_U*5)*(1-_U*0.1);
				
				 
				fixed dis = tex2D(_DisTex, disuv);
				
				fixed alp = tex2D(_Alphatex, alpuv);

				fixed diss1 = smoothstep(0.3,0.5,pow(saturate(dis+(1-_DisPow)*(1-_DisWt)),_Disrange*(1-_DisWt)));
				fixed diss2 = pow(saturate(dis+(1-_DisPow)),_Disrange);
				 

				fixed4 tex = tex2D(_MainTex, texuv)*_TexPow*_TexCol;

						 tex = (1-diss1) * _DisCol + (tex * diss1) ;
						 // tex = tex*diss2;

				
				

				fixed4 col;

				col = tex;

				// col.a = alp*pow(saturate(dis+_DisPow),_Disrange) ;
				col.a = alp*(smoothstep(0.3,0.5,pow(saturate(dis+(1-_DisPow)),_Disrange)))*((1-diss1)*_DisCol.a+diss1) *_TexCol.a;//*L*R*B*F*T*U;//+pow(saturate(dis+_DisPow),_Disrange));



				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

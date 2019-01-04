// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mark/HOC/Effect/Overlayer_3Tex_Add"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "blakc" {}
		_RotateMainAngle("Rotate Main angle",float) = 0
		[HDR]_TexCol ("Texture Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (0,0,0,1)
		_TexPow ("Texture Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-2,2)) = 1

		_MainTex2 ("Texture", 2D) = "blakc" {}
		_TexCol2("Texture Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (0,0,0,1)
		_TexPow2 ("Texture Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-2,2)) = 1

		_MainTex3 ("Texture", 2D) = "blakc" {}
		_TexCol3 ("Texture Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (0,0,0,1)
		_TexPow3 ("Texture Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-2,2)) = 1

		// _DisTex ("Dissolve Map", 2D) = "white" {}
		// _DisCol ("Dissolve Color >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",color) = (1,1,1,1)
		// _DisPow ("Dissolve Intensity >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (-1,3)) = 0
		// _Disrange ("Dissolve Range >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,100)) = 1
		// _DisWt ("Dissolve Width >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,0.5)) = 0

		// _L ("Lift Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _R ("Right Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _F ("Front Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _B ("Back Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _T ("Top Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0
		// _U ("Under Disappear>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", range (0,10)) = 0

		_Alphatex ("Transparent Mask", 2D) = "white" {}
		_RotateAngle("Mask Rotate angle",float) = 0
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


//		Fog { Mode Off }


		
		Pass
		{
		Name "BASE"
			





			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// #pragma target 3.0
			// make fog work
//			#pragma multi_compile_fog
			
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
				float4 uv2 : TEXCOORD2;
				// float3 vcolor : COLOR;
				
//				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			half _RotateMainAngle;
			// sampler2D _DisTex;
			sampler2D _Alphatex;
			half _RotateAngle;
			// sampler2D _GrabTexture; 

			sampler2D _MainTex;
			fixed4 _TexCol;
			half _TexPow;

			sampler2D _MainTex2;
			fixed4 _TexCol2;
			half _TexPow2;

			sampler2D _MainTex3;
			fixed4 _TexCol3;
			half _TexPow3;
			// half _DisPow;
			// half _Disrange;
			// half _DisWt;
			// fixed4 _DisCol;

			// half _L;
			// half _R;
			// half _F;
			// half _B;
			// half _T;
			// half _U;

			float4 _MainTex_ST;
			float4 _MainTex2_ST;
			float4 _MainTex3_ST;
			// float4 _DisTex_ST;
			float4 _Alphatex_ST;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// o.vcolor = smoothstep(-1,1,mul(UNITY_MATRIX_IT_MV,v.normal));//+mul(UNITY_MATRIX_IT_MV,-v.vertex);
				float2 tempUV = v.uv - float2(0.5,0.5);
				
				o.uv1.xy = float2(tempUV.x * cos(_RotateMainAngle) - tempUV.y * sin(_RotateMainAngle), tempUV.y * cos(_RotateMainAngle) + tempUV.x * sin(_RotateMainAngle));	
				o.uv1.xy += float2(0.5,0.5); 
				o.uv1.xy = TRANSFORM_TEX(o.uv1.xy, _MainTex);

				o.uv1.zw = TRANSFORM_TEX(v.uv, _MainTex2);
				o.uv2.xy = TRANSFORM_TEX(v.uv, _MainTex3);	
								
				o.uv2.zw = float2(tempUV.x * cos(_RotateAngle) - tempUV.y * sin(_RotateAngle), tempUV.y * cos(_RotateAngle) + tempUV.x * sin(_RotateAngle));	
				o.uv2.zw += float2(0.5,0.5);			
			    o.uv2.zw = TRANSFORM_TEX( o.uv2.zw , _Alphatex);
				
//				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 texuv = i.uv1.xy;
				float2 texuv2 = i.uv1.zw;
				float2 texuv3 = i.uv2.xy;
				float2 alpuv = i.uv2.zw;

				// half L = pow((1-i.vcolor.r),_L*5)*(1-_L*0.1);
				// half R = pow((i.vcolor.r),_R*5)*(1-_R*0.1);
				// half B = pow((i.vcolor.z),_B*5)*(1-_B*0.1);
				// half F = pow((1-i.vcolor.z),_F*0.1)*(1-_F*0.1);
				// half T = pow((1-i.vcolor.y),_T*5)*(1-_T*0.1);
				// half U = pow((i.vcolor.y),_U*5)*(1-_U*0.1);
				
				 
				// fixed dis = tex2D(_DisTex, disuv);
				
				fixed alp = tex2D(_Alphatex, alpuv);

				// fixed diss1 = smoothstep(0.3,0.5,pow(saturate(dis+(1-_DisPow)*(1-_DisWt)),_Disrange*(1-_DisWt)));
				// fixed diss2 = pow(saturate(dis+(1-_DisPow)),_Disrange);
				 

				fixed4 tex = tex2D(_MainTex, texuv)*_TexPow*_TexCol*_TexCol.a;
				fixed4 tex2 = tex2D(_MainTex2, texuv2)*_TexPow2*_TexCol2*_TexCol2.a;
				fixed4 tex3 = tex2D(_MainTex3, texuv3)*_TexPow3*_TexCol3*_TexCol3.a;

						 tex = tex + tex2 + tex3;
						 // tex = tex*diss2;

				
				

				fixed4 col;

				col = tex ;//* ((_TexCol.a+_TexCol2.a+_TexCol3.a)/3);

				// col.a = alp*pow(saturate(dis+_DisPow),_Disrange) ;
				col.a = alp;



				// apply fog
//				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

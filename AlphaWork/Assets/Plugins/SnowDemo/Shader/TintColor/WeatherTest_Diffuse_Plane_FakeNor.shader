Shader "WeatherTest/Diffuse_Plane_FakeNor"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {} 
		_TintMask("Tint Mask Single Map(A)", 2D) = "white" {}
		_MaskMapTiling("MaskMapTiling", Range(0.01, 50)) = 1  
		//_TintColor("Tint Color",Color) = (1,1,1,0)
		//_TintPower("TintPower",Range(0,2)) = 0  
		_TintPowerMaxRange("TintPowerMaxRange",Range(0,2)) = 2   
		_TintNormalEx("Change Color On Side",Range(-0.25,0.5)) = 0.05 
		_TintColorFakeNorMap("TintColor Fake Normap", 2D) = "bump" {}  
		_TintMaskNorMapTiling("TintMaskNorMapTiling", Range(0.01, 100)) = 40  
		_NormalNoiseMap("NormalNoiseMap (RG) noise Normal",2D) = "Black" {} 
		_NormalNoiseSpeed("NormalNoiseSpeed",Range(0,1)) = 0
		//_NormalNoisePower("NormalNoisePower",Range(0,1)) = 0
		_NormalNoiseTiling("NormalNoiseTiling",Range(0.1,10)) = 1 
		_NormalNoiseMapPower("NormalNoiseMapPower",Range(0.0,1.0)) = 0.3 
		_NormalNoiseSpecGloss("NormalNoiseSpecGloss",Range(1,1024)) = 500
		_NormalNoiseSpecPower("NormalNoiseSpecPower",Range(0.01,5)) = 2 
		_DecalPower("DecalPower",Range(0,5)) = 0.1
		_Decal2Tiling("Decal2Tiling",Range(1,2)) = 1.2  
	}
  
	SubShader
	{
		Tags{ "RenderType" = "Opaque" } 
		LOD 300
		Lighting on 
		Pass     
		{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			//#pragma multi_compile __ NIGHT_LIGHT_ON
			
			#ifndef VAR_TINT_COLOR_FAKENOR
			#define VAR_TINT_COLOR_FAKENOR 
			#endif
			 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"    
			#include "../Include/TintColor.cginc"
			#include "../Include/NoiseAndDecal.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			float4 _MainTex_ST;
			half _Shininess;
			
			VAR_TINT_COLOR_NEED  
			VAR_NOISE_DECAL_NEED 
			   
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0; // _MainTex
				half3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 uvLM : TEXCOORD3;
				LIGHTING_COORDS(4,5)
				UNITY_FOG_COORDS(6)
			};
			
			v2f vert (appdata_full v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				//o.uv0.zw = TRANSFORM_TEX(v.texcoord, _Illum);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = worldPos;
				o.worldNormal = worldNormal;
				o.uvLM.xy = v.texcoord1.xy;
				
				TINT_VERTEX(o.uvLM.zw ,v.texcoord1)
				
				UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			//LogLuv to FP32(RGB)
			fixed3 DecodeLogLuv(in fixed4 vLogLuv)
			{
				fixed3x3 InverseM = fixed3x3(
					6.0014, -2.7008, -1.7996,
					-1.3320, 3.1029, -5.7721,
					0.3008, -1.0882, 5.6268); 
				fixed Le = vLogLuv.z * 255 +
					vLogLuv.w;
				fixed3 Xp_Y_XYZp;
				Xp_Y_XYZp.y = exp2((Le - 127)
					/ 2);
				Xp_Y_XYZp.z = Xp_Y_XYZp.y /
					vLogLuv.y;
				Xp_Y_XYZp.x = vLogLuv.x *
					Xp_Y_XYZp.z;
				fixed3 vRGB = mul(Xp_Y_XYZp,
					InverseM);

				return max(vRGB, 0);
			}

			float4 frag (v2f i) : SV_Target 
			{
				half4 c = _Color * tex2D(_MainTex, i.uv0.xy );
				
				float3 wN = i.worldNormal;
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 halfDir = normalize(lightDir + viewDir);
				
				//TintColor
				//非虚化
				//TINT_CAL_COLOR_MASK_DOT_GRAYSCALE_AUTO_UV_CENTER_POS(i.worldPos ,wN,c.rgb) 
				//虚化     
				//TINT_CAL_COLOR_MASK_DOT_GRAYSCALE_AUTO_UV_CENTER_POS_BLEND(i.worldPos ,wN,c.rgb)   
				STRUCT_TINT tintInput; 
				tintInput.tintColor = c.rgb;
				tintInput.tintNormal = wN;
				TINT_CAL_COLOR_MASK_DOT_GRAYSCALE_AUTO_UV_CENTER_POS_BLEND_POWER_RANGE_FAKENOR(i.worldPos ,wN,tintInput)      
				c.rgb = tintInput.tintColor;
				wN = tintInput.tintNormal;
				
				//扰动法线   
				//float3 tempNor = wN;
				NOISE_NOR_RECAL_SNOWMASK(i.worldPos ,i.uvLM.zw,wN)
				//wN = lerp(tempNor,wN,tintInput.maskValue);
				//return float4(wN,1); 
				
				float diff = max(0.5, dot(wN, lightDir));
				float nh = max(0, dot(wN, halfDir)); 
				float spec = pow(nh, _Shininess * 128) * 1;
				  
				NOISE_SPEC(nh,_Shininess,spec) 

				float dotLH = max(0.8, dot(lightDir, halfDir));

				
				
			    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
				
				float4 finalcolor;				
				float3 light = _LightColor0.rgb * atten *dotLH* diff ;
				
				finalcolor.rgb = c.rgb * light.xyz;		
				
				float3 ambientLighting = ShadeSH9(half4(wN, 1.0));
				finalcolor.rgb += ambientLighting * c.rgb;
				
                UNITY_APPLY_FOG(i.fogCoord,finalcolor); 

				return  finalcolor;
			}
			ENDCG
		}
		Pass{
			Tags{ "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma multi_compile_shadowcaster  
			#include "UnityCG.cginc"  

			sampler2D _Shadow;

			struct v2f {
				V2F_SHADOW_CASTER;
				float2 uv:TEXCOORD2;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.uv = v.texcoord.xy;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				fixed alpha = tex2D(_Shadow, i.uv).a;
				clip(alpha - 0.0);
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG
		}
	}
}

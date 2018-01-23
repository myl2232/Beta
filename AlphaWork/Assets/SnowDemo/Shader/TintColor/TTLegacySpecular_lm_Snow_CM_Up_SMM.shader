Shader "TT/LegacySPecular_lm_Tint_CM_UP_SMM"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Rimcolor("rim Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_LightMap("lightmap", 2D) = "white" {}
		_Illum("Illumin (A)", 2D) = "black" {}
		_RimLev("RimLev", Range(0, 10)) = 0
		_RimPower("RimPower", Range(0, 10)) = 0
		_TintMask("Tint Mask Single Map(A)", 2D) = "white" {}
		_TintColor("Tint Color",Color) = (1,1,1,0)
		_TintPower("TintPower",Range(0,2)) = 0
		_TintNormalEx("Change Color On Side",Range(-0.25,0.5)) = 0.05
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
			//#pragma shader_feature NIGHT_LIGHT_ON
			#pragma multi_compile_fog
			#pragma multi_compile __ NIGHT_LIGHT_ON
			 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc" 
			#include "../Include/TintColor.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _Rimcolor;
			//fixed4 _TintColor;
			half _Shininess;
			float4 _MainTex_ST;
			sampler2D _LightMap;
			sampler2D _Illum;
			float4 _Illum_ST;
			half _RimPower;
			half _RimLev;  
			
			VAR_TINT_COLOR_NEED
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv0 : TEXCOORD0; // _MainTex
				half3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 uvLM : TEXCOORD3;
				LIGHTING_COORDS(4,5)
				UNITY_FOG_COORDS(6)
			/*	UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO*/
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
				o.uv0.zw = TRANSFORM_TEX(v.texcoord, _Illum);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = worldPos;
				o.worldNormal = worldNormal;
				o.uvLM.xy = v.texcoord1.xy;
				
				TINT_VERTEX(o.uvLM.zw ,v.texcoord)
				
				//o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				//UNITY_TRANSFER_SHADOW(o, v.texcoord1.xy); // pass shadow coordinates to pixel shader
				UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				//TINT_SCALE_Y_VALUE(o.pos,o.pos)
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
				float diff = max(0.5, dot(wN, lightDir));
				float nh = max(0, dot(wN, halfDir));
				float spec = pow(nh, _Shininess * 128) * 1;
				float dotLH = max(0.8, dot(lightDir, halfDir));

				float rim = 1 - saturate(dot(wN, normalize(viewDir)));  
        		rim = pow(rim, _RimPower) * 0.5 * _Rimcolor * _RimLev; 

				//float updot = saturate((dot(wN, float3(0,1,0)) + _TintNormalEx) / (1 + _TintNormalEx));  
				TINT_CAL_COLOR_MASK_UP(i.uvLM.zw ,wN,c.rgb)
				
				#if NIGHT_LIGHT_ON       
					fixed4 lm = tex2D(_LightMap, i.uvLM.xy);
					lm.xyz = DecodeLogLuv(lm);
					//fixed3 lm =  DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));					
					c.rgb *= lm.xyz;				
				#endif		
			    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)
				
				
				
				float4 finalcolor;				
				float3 light = _LightColor0.rgb * atten *dotLH* diff ;
				finalcolor.rgb = c.rgb * light.xyz + min(10,light.xyz * _SpecColor.rgb * spec ) ;			
				
				#if NIGHT_LIGHT_ON 
					finalcolor.rgb +=  c.rgb;
				#else
					float3 ambientLighting = ShadeSH9(half4(wN, 1.0));
					finalcolor.rgb += ambientLighting * c.rgb;
				#endif	
                UNITY_APPLY_FOG(i.fogCoord,finalcolor); 
				float3 emission = tex2D(_Illum, i.uv0.zw) *tex2D(_Illum, i.uv0.zw).a;
				finalcolor.rgb += emission.xyz;
				finalcolor.rgb *= 1 + rim;

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

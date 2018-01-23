Shader "Custom/TestShader" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {} 
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
			 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"  
			//#include "../Include/TintColor.cginc"
			//#include "../Include/NoiseAndDecal.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			float4 _MainTex_ST;
			half _Shininess;

			
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
				
				//TINT_VERTEX(o.uvLM.zw ,v.texcoord1)
				
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
				half4 light001 = _LightColor0;//unity_LightColor[0];
				half4 c = _Color * tex2D(_MainTex, i.uv0.xy ) * light001;
				return c;
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
	FallBack "Diffuse"
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "T4MShaders/ShaderModel2/Diffuse/T4M 4 Custom 2 Bump VF"
{
Properties {
	_Color ("Main Color", color) = (1.0,1.0,1.0,1.0)
	_Spec ("Specular Color", color) = (1.0,1.0,1.0,1.0)
	_WetSpec("WetSpecular Color", color) = (0.5, 0.5, 0.5, 1.0)
	_Splat0 ("Layer 1", 2D) = "white" {}
	_Splat1 ("Layer 2", 2D) = "white" {}
	_BMap1 ("NormalMap(layer2)", 2D) = "bump" {}
	_Splat2 ("Layer 3", 2D) = "white" {}
	_BMap2("NormalMap(layer3)", 2D) = "bump" {}
	_Splat3 ("Layer 4", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_WetFactorDiffuse("WetFactorDiffuse", Range(0, 1)) = 0.8
	_WetFactorSpecular("WetFactorSpecular", Range(0, 3)) = 1.5
	_WetControl("WetControl", Range(0, 1)) = 0
	_NormalMapFactor1("NormalMap Factor1（0-5）", Range(0, 10)) = 1
	_NormalMapFactor2("NormalMap Factor2（0-5）", Range(0, 10)) = 1
	_Shinness("Shinness", Range(0.5,3)) = 1
	_Gloss("Gloss", Range(0.0, 3)) = 0
	_RealLightDir("RealLightDir", Vector) = (1,1,1,1)
	_CustomLightColor("LightColor", Color) = (0.5, 0.5, 0.5, 1)
	_CustomLightDir("LightDir", Vector) = (1,1,1,1)
	_Noise("Noise", 2D) = "black" {}
}
                
	SubShader {
		LOD 950
		Tags {
	   "SplatCount" = "4"
	   "RenderType" = "Opaque"
	   "Queue" = "AlphaTest+1"
		}
		Fog {Mode Linear}
	Pass
        {
			
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			//#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
		
			struct v2f {
				float4 pos 		  		: SV_POSITION;
				fixed3 color	  		: Color;
				float4 uv_Control 		: TEXCOORD0;
				float4 uv_Splat01 		: TEXCOORD1;
				float4 uv_Splat23 		: TEXCOORD2;
				float3 tLightDir  		: TEXCOORD3;
				float3 tViewDir  		: TEXCOORD4;
				float3 tRealLightDir    : TEXCOORD5;
				UNITY_FOG_COORDS(6)
			};
		 
			uniform fixed4 _Color;
			uniform fixed4 _Spec;
			uniform fixed4 _WetSpec;
			uniform float4 _RealLightDir;
			uniform fixed4 _CustomLightColor;
			uniform float4 _CustomLightDir;
			uniform half _NormalMapFactor1;
			uniform half _NormalMapFactor2;
			uniform half _Shinness;
			uniform half _Gloss;
			uniform half _WetFactorDiffuse;
			uniform half _WetFactorSpecular;
			uniform half _WetControl;
			
			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
			sampler2D _BMap1,_BMap2;
			sampler2D _Noise;
		 
			float4 _Control_ST;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;
			
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
				o.uv_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv_Splat23.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv_Splat23.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
				
				o.uv_Control.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				
				TANGENT_SPACE_ROTATION;
				o.tViewDir = mul(rotation, ObjSpaceViewDir( v.vertex ));
				o.tLightDir.xyz = mul(rotation, mul(unity_WorldToObject,float4(_CustomLightDir.xyz,0)).xyz).xyz;
				o.tRealLightDir.xyz = mul(rotation, mul(unity_WorldToObject,float4(_CustomLightDir.xyz,0)).xyz).xyz;
				
				UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
				
				return o;
			}
	 
			fixed4 frag(v2f i) : COLOR
			{
				float4 splat_control = tex2D (_Control, i.uv_Control.xy).rgba;
				float splat_ctrl_sum = (splat_control.r + splat_control.g + splat_control.b + splat_control.a);
				splat_control /= splat_ctrl_sum;
				fixed3 lay1 = tex2D (_Splat0, i.uv_Splat01.xy);
				fixed4 lay2 = tex2D (_Splat1, i.uv_Splat01.zw);
				fixed4 lay3 = tex2D (_Splat2, i.uv_Splat23.xy);
				fixed3 lay4 = tex2D (_Splat3, i.uv_Splat23.zw);
				fixed3 lay2B = UnpackNormal (tex2D(_BMap1, i.uv_Splat01.zw));
				fixed3 lay3B = UnpackNormal (tex2D(_BMap2, i.uv_Splat23.xy));
				
				//增强或减弱法线效果，1默认法线，0无法线，>1增强，<1减弱。
				lay2B *= fixed3(_NormalMapFactor1, _NormalMapFactor1, 1);
				lay3B *= fixed3(_NormalMapFactor2, _NormalMapFactor2, 1);
				half3 lightDir = normalize(i.tLightDir.xyz);
				half3 realLightDir = normalize(i.tRealLightDir);
				fixed3 normal2 = normalize(lay2B);
				fixed3 normal3 = normalize(lay3B);
				half3 viewDir = normalize(i.tViewDir);
				
				//temp计算法线
				fixed3 normal = fixed3(0,0,1) * (splat_control.r + splat_control.a) + normal2 * splat_control.g + normal3 * splat_control.b;
				fixed3 noise1 = UnpackNormal(tex2D(_Noise, i.uv_Splat01.xy * 5 +  0.1 * _Time.y * 7));
				fixed3 noise2 = UnpackNormal(tex2D(_Noise, i.uv_Splat01.xy * 5 -  0.1 * _Time.y * 5));
				fixed3 tempN = normal;
				fixed3 noise = noise1 + noise2;
				tempN.z = 0;
				normal += noise * length(tempN) *  saturate(_WetControl - 0.3);
				normal += noise * 0.1 *  saturate(_WetControl - 0.3);
				normal = normalize(normal);
				
				// 模拟法线
				float at = saturate (dot (fixed3(0,0,1), realLightDir));
				float n2l = saturate(dot(normal, lightDir));
				
				//lightmap
				#ifndef LIGHTMAP_OFF
				fixed3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv_Control.zw)).rgb;
				//fixed3 diff = lightmap / (_LightColor0.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_LightColor0.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				//由于tod会更改主光颜色，导致结果不对，此处改为一个传入的光颜色，需要设置为烘焙时的主光颜色
				fixed3 diff = lightmap / (_CustomLightColor.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_CustomLightColor.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				#endif

				#ifdef LIGHTMAP_OFF
				fixed3 diff = n2l * _CustomLightColor.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;
				#endif
				
				
				//高光计算
				_Gloss += _WetFactorSpecular * _WetControl;
				half3 h = normalize(lightDir + viewDir);
				float nh = saturate(dot(normal, h));
				half scale = saturate(lay2.a * splat_control.g + lay3.a * splat_control.b);
				float spec = pow(nh, 128 * _Shinness) * _Gloss * scale;
				half3 specular = spec * _CustomLightColor.xyz * lerp(_Spec.rgb, _WetSpec, _WetControl);
				
				// 贴图混合
				fixed4 outColor = 0;
				outColor.rgb = (lay1 * splat_control.r + lay2.rgb * splat_control.g + lay3.rgb * splat_control.b + lay4 * splat_control.a) * _Color.xyz;
				
				// 积水效果 
				half wetFactor = lerp( 1.0, _WetFactorDiffuse, _WetControl);
				
				outColor.rgb = outColor.rgb * diff * wetFactor + specular;
				UNITY_APPLY_FOG(i.fogCoord, outColor); // apply fog
				return outColor;
			}
			ENDCG 
		}
	}
	
	// Fallback to T4M
	Fallback "T4MShaders/ShaderModel2/Diffuse/T4M 4 Textures"
}
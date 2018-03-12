// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Custom Bump Specular VF"
{
Properties {
	_Color ("Main Color", color) = (1.0,1.0,1.0,1.0)	
	_Spec ("Specular Color", color) = (1.0,1.0,1.0,1.0)
	_WetSpec("WetSpecular Color", color) = (0.5, 0.5, 0.5, 1.0)
	_MainTex("Base (RGB)", 2D) = "white" {}
	_BMap("NormalMap", 2D) = "bump" {}
	_MaskTex("SpecularMask", 2D) = "red"{}
	_NormalMapFactor("NormalMap Factor（0-5）", Range(0, 5)) = 1
	_Shinness("Shinness", Range(0.01,3)) = 1
	_Gloss("Gloss", Range(0.0, 3)) = 0
	_RealLightDir("RealLightDir", Vector) = (1,1,1,1)
	_CustomLightColor("LightColor", Color) = (0.5, 0.5, 0.5, 1)
	_CustomLightDir("LightDir", Vector) = (1,1,1,1)
	_LightIntensity("LightItensity", Range(0, 8)) = 1
	_WetFactorDiffuse("WetFactorDiffuse", Range(0, 1)) = 0.8
	_WetFactorSpecular("WetFactorSpecular", Range(0, 3)) = 1.5
	_WetControl("WetControl", Range(0, 1)) = 0
	_Noise("Noise", 2D) = "bump" {}
}
                
	SubShader {
		LOD 960
		Tags {"RenderType" = "Opaque"}
		Fog {Mode Linear}
	Pass
        {
			
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
 	
			struct v2f {
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float4 pack0 : TEXCOORD0;
				float4 uv_lm 	  : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				SHADOW_COORDS(6)
				UNITY_FOG_COORDS(7)
			};
	 
			uniform fixed4 _Color;
			uniform fixed4 _Spec;
			uniform fixed4 _WetSpec;
			uniform fixed4 _CustomLightColor;
			uniform float4 _CustomLightDir;
			uniform float4 _RealLightDir;
			uniform half _NormalMapFactor;
			uniform half _LightIntensity;
			uniform half _Shinness;
			uniform half _Gloss;
			uniform sampler2D _MainTex, _BMap, _MaskTex, _Noise;
			uniform float4 _MainTex_ST;
			uniform float4 _BMap_ST;
			uniform float4 _Noise_ST;
			uniform half _WetFactorDiffuse;
			uniform half _WetFactorSpecular;
			uniform half _WetControl;

			v2f vert(appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BMap);
				o.uv_lm.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.uv_lm.zw = TRANSFORM_TEX(v.texcoord, _Noise);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				o.normal = worldNormal;
				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
	 
			fixed4 frag(v2f i) : COLOR
			{
				//采样
				fixed3 mainTex = tex2D(_MainTex, i.pack0.xy);
				fixed3 normalTex = UnpackNormal(tex2D(_BMap, i.pack0.xy));
				fixed3 maskTex = tex2D(_MaskTex, i.pack0.xy);
				
				//增强或减弱法线效果，1默认法线，0无法线，>1增强，<1减弱。
				normalTex *= fixed3(_NormalMapFactor, _NormalMapFactor, 1);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				float3 worldPos = float3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				float3 lightDir = normalize(_CustomLightDir.xyz);
				float3 realLightDir = normalize(_RealLightDir.xyz);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 noise1 = UnpackNormal(tex2D(_Noise, i.pack0.xy * 15 +  0.1 * _Time.y * 7));
				fixed3 noise2 = UnpackNormal(tex2D(_Noise, i.pack0.xy * 15 -  0.1 * _Time.y * 5));
				fixed3 tempN = normalTex;
				fixed3 noise = (noise1 + noise2);
				tempN.z = 0;
				normalTex += noise * length(tempN) *  saturate(_WetControl - 0.3) * 2;
				
				//return fixed4(normalTex, 1.0);
				normalTex += noise * 0.2 *   saturate(_WetControl - 0.3);
				normalTex = normalize(normalTex);
				
				fixed3 mapNormal;
				mapNormal.x = dot(i.tSpace0.xyz, normalTex);
				mapNormal.y = dot(i.tSpace1.xyz, normalTex);
				mapNormal.z = dot(i.tSpace2.xyz, normalTex);
				//normalTex = normalize(normalTex);
				//tempN = i.normal;
				//fixed4 noise = noise1 + noise2;
				//tempN.z = 0;
				//i.normal += noise.xyz  *   _WetControl;
				
				mapNormal = normalize(mapNormal);
				//return fixed4(mapNormal,1.0);
				
				//lightmap
				#ifdef LIGHTMAP_OFF
				float3 diff = max(0, dot(mapNormal, lightDir)) * _CustomLightColor.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;
				#endif
				
				//realtime light 
				#ifndef LIGHTMAP_OFF
				fixed3 vertexNormal = normalize(i.normal);
				float at = max(0, dot(vertexNormal, realLightDir));
				float n2l = max(0, dot(mapNormal, lightDir));
				fixed3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv_lm.xy)).rgb;
				//return fixed4(min(2 * ((lightmap/ 2 - UNITY_LIGHTMODEL_AMBIENT.xyz) + UNITY_LIGHTMODEL_AMBIENT.xyz), 2) * mainTex, 1);
				//fixed3 diff = (lightmap  - UNITY_LIGHTMODEL_AMBIENT.xyz * 2) * n2l / at + UNITY_LIGHTMODEL_AMBIENT * 2;
				//fixed3 diff = ((min(lightmap, 2)  + 2 * _LightColor0.xyz * (n2l - at)));
				//fixed3 diff = lightmap / (_LightColor0.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_LightColor0.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				//由于tod会更改主光颜色，导致结果不对，此处改为一个传入的光颜色，需要设置为烘焙时的主光颜色
				fixed3 diff = lightmap / (_CustomLightColor.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_CustomLightColor.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				#endif
				
				//specular
				half3 h = normalize(lightDir + viewDir);
				half nh = max(0, dot(mapNormal, h));
				_Gloss += _WetFactorSpecular * _WetControl;
				half spec = pow(nh, 128.0 * _Shinness) * _Gloss;
				half3 specular = spec * _CustomLightColor.rgb * lerp(_Spec.rgb, _WetSpec, _WetControl) * maskTex.r;
				half3 diffuseColor = diff * mainTex * _Color.rgb;
				half wetFactor = lerp( 1.0, _WetFactorDiffuse, _WetControl);
				//return fixed4(specular, 1);
				float3 finalColor = (diffuseColor * wetFactor + specular) * atten * _LightIntensity;
				fixed4 c = fixed4(finalColor, 1.0);
				UNITY_APPLY_FOG(i.fogCoord, c); // apply fog
				return c;
			}
			ENDCG 
		}
	}
		  
	// 此SubShader用于Huawei MT7机型，语义同前一个完全一致，通过一些变化的写法解决该机型上表现效果不对的问题（推测该机型上某些优化项有问题）
	SubShader {
		LOD 950
		Tags {"RenderType" = "Opaque"}
		Fog {Mode Linear}
	Pass
        {
			
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
 	
			struct v2f {
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float4 pack0 : TEXCOORD0;
				float2 uv_lm 	  : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				SHADOW_COORDS(6)
				UNITY_FOG_COORDS(7)
			};
	 
			uniform fixed4 _Color;
			uniform fixed4 _Spec;
			uniform fixed4 _WetSpec;
			uniform fixed4 _CustomLightColor;
			uniform float4 _CustomLightDir;
			uniform float4 _RealLightDir;
			uniform half _NormalMapFactor;
			uniform half _LightIntensity;
			uniform half _Shinness;
			uniform half _Gloss;
			uniform sampler2D _MainTex, _BMap, _MaskTex;
			uniform float4 _MainTex_ST;
			uniform float4 _BMap_ST;
			uniform half _WetFactorDiffuse;
			uniform half _WetFactorSpecular;
			uniform half _WetControl;

			v2f vert(appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BMap);
				o.uv_lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				o.normal = worldNormal;
				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
	 
			fixed4 frag(v2f i) : COLOR
			{
				//采样
				fixed3 mainTex = tex2D(_MainTex, i.pack0.xy);
				fixed3 normalTex = UnpackNormal(tex2D(_BMap, i.pack0.xy));
				fixed3 maskTex = tex2D(_MaskTex, i.pack0.xy);
				
				//增强或减弱法线效果，1默认法线，0无法线，>1增强，<1减弱。
				normalTex *= fixed3(_NormalMapFactor, _NormalMapFactor, 1);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				float3 worldPos = float3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				float3 lightDir = normalize(_CustomLightDir.xyz);
				float3 realLightDir = normalize(_RealLightDir.xyz);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 mapNormal;
				mapNormal.x = dot(i.tSpace0.xyz, normalTex);
				mapNormal.y = dot(i.tSpace1.xyz, normalTex);
				mapNormal.z = dot(i.tSpace2.xyz, normalTex);
				mapNormal = normalize(mapNormal);
				
				//lightmap
				#ifdef LIGHTMAP_OFF
				float3 diff = max(0, dot(mapNormal, lightDir)) * _CustomLightColor.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;
				#endif
				
				//realtime light 
				#ifndef LIGHTMAP_OFF
				fixed3 vertexNormal = normalize(i.normal);
				float at = max(0, dot(vertexNormal, realLightDir));
				float n2l = max(0, dot(mapNormal, lightDir));
				fixed3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv_lm)).rgb;
				//return fixed4(min(2 * ((lightmap/ 2 - UNITY_LIGHTMODEL_AMBIENT.xyz) + UNITY_LIGHTMODEL_AMBIENT.xyz), 2) * mainTex, 1);
				//fixed3 diff = (lightmap  - UNITY_LIGHTMODEL_AMBIENT.xyz * 2) * n2l / at + UNITY_LIGHTMODEL_AMBIENT * 2;
				//fixed3 diff = ((min(lightmap, 2)  + 2 * _LightColor0.xyz * (n2l - at)));
				//fixed3 diff = lightmap / (_LightColor0.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_LightColor0.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				//由于tod会更改主光颜色，导致结果不对，此处改为一个传入的光颜色，需要设置为烘焙时的主光颜色
				fixed3 diff = lightmap / (_CustomLightColor.xyz * at + UNITY_LIGHTMODEL_AMBIENT.xyz) * (_CustomLightColor.xyz * n2l + UNITY_LIGHTMODEL_AMBIENT.xyz);
				#endif
				//specular
				_Gloss += _WetFactorSpecular * _WetControl;
				half3 h = normalize(lightDir + viewDir);
				half nh = max(0, dot(mapNormal, h));
				float spec = pow(nh, 128.0 * _Shinness) * _Gloss;
				half3 specular = spec * _CustomLightColor.rgb * lerp(_Spec.rgb, _WetSpec.rgb, _WetControl) * maskTex.r * atten * _LightIntensity;
				
				half wetFactor = lerp( 1.0, _WetFactorDiffuse, _WetControl);
				
				float3 finalColor = diff * mainTex * _Color.rgb * atten * _LightIntensity * wetFactor;
				finalColor.x += specular.x;
				finalColor.y += specular.y;
				finalColor.z += specular.z;
				fixed4 c = fixed4(finalColor, 1.0);
				UNITY_APPLY_FOG(i.fogCoord, c); // apply fog
				return c;
			}
			ENDCG 
		}
	}
	Fallback "Diffuse"
}
// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "TLStudio/CharSkinCutout" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Normal("Normal", 2D) = "white" {}
	_Metallic("Metallic", 2D) = "white" {}
	_FresnelStrenth("Fresnel Strenth",Range(0,50)) = 2
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_SSSWarp("SSS Warp", Range(0,1)) = 1
		_SSSScatter("SSS Scatter", Range(0,1)) = 1
		_SSSScatterColor("SSS Scatter Color", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "RenderType" = "Cutout" }
		LOD 200

		cull off

		CGPROGRAM
#include "UnityPBSLighting.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf CustomSkin fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0


		struct Input {
		float2 uv_MainTex;
		float3 worldViewDir;
		float3 worldNormal; INTERNAL_DATA
	};

	sampler2D _MainTex;
	sampler2D _Metallic;
	sampler2D _Normal;
	fixed4 _Color;
	float _FresnelStrenth;
	float _SSSWarp;
	float _SSSScatter;
	float3 _SSSScatterColor;

	

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		fixed4 metallic_roughness = tex2D(_Metallic, IN.uv_MainTex);
		o.Metallic = metallic_roughness.r;
		o.Smoothness = metallic_roughness.g;
		o.Alpha = c.a;
		float4 normalpacked = tex2D(_Normal, IN.uv_MainTex);
		float3 normal;
		normal.xy = normalpacked.wy*2.0 - 1.0;
		normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
		//o.Normal = normal;
		o.Normal = normalpacked.rgb*2.0 - 1.0;
		float3 worldnormal = WorldNormalVector(IN, o.Normal);
		float A = (saturate(dot(IN.worldViewDir, worldnormal)) + _SSSWarp) / (1.0 + _SSSWarp);
		float B = A / (_SSSScatter + 0.001);
		float3 SSS_Color = _SSSScatterColor*B*B*(3 - 2 * B)*lerp(_SSSScatter * 2, _SSSScatter, A);
		o.Emission = saturate(SSS_Color)*c.rgb*metallic_roughness.b;
		//o.Albedo = c.rgb+ saturate(SSS_Color)*c.rgb;
	}
	half4 BRDF_Custom_PBS(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
		half3 normal, half3 viewDir,
		UnityLight light, UnityIndirect gi)
	{
		half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);

		half nl = saturate(dot(normal, light.dir));
		half nh = saturate(dot(normal, halfDir));
		half nv = saturate(dot(normal, viewDir));
		half lh = saturate(dot(light.dir, halfDir));

		// Specular term
		half perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
		half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX

		// GGX Distribution multiplied by combined approximation of Visibility and Fresnel
		// See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
		// https://community.arm.com/events/1155
		half a = roughness;
		half a2 = a*a;

		half d = nh * nh * (a2 - 1.h) + 1.00001h;
#ifdef UNITY_COLORSPACE_GAMMA
		// Tighter approximation for Gamma only rendering mode!
		// DVF = sqrt(DVF);
		// DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(roughness + .5) * d);
		half specularTerm = a / (max(0.32h, lh) * (1.5h + roughness) * d);
#else
		half specularTerm = a2 / (max(0.1h, lh*lh) * (roughness + 0.5h) * (d * d) * 4);
#endif

		// on mobiles (where half actually means something) denominator have risk of overflow
		// clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
		// sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE)
		specularTerm = specularTerm - 1e-4h;
#endif

#else

		// Legacy
		half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
		// Modified with approximate Visibility function that takes roughness into account
		// Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness
		// and produced extremely bright specular at grazing angles

		half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
		half invF = lh;

		half specularTerm = ((specularPower + 1) * pow(nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
		specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
		specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
		specularTerm = 0.0;
#endif

		// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)

		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
		// 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)
#ifdef UNITY_COLORSPACE_GAMMA
		half surfaceReduction = 0.28;
#else
		half surfaceReduction = (0.6 - 0.08*perceptualRoughness);
#endif

		surfaceReduction = 1.0 - roughness*perceptualRoughness*surfaceReduction;

		half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
		half3 color = (diffColor + specularTerm * specColor) * light.color * nl
			+ gi.diffuse * diffColor;
		color += surfaceReduction * gi.specular * FresnelLerpFast(specColor, grazingTerm*_FresnelStrenth
			, nv);

		return half4(color, 1);
	}
	inline half4 LightingCustomSkin(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
	{
		s.Normal = normalize(s.Normal);

		half oneMinusReflectivity;
		half3 specColor;
		s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

		// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
		// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
		half outputAlpha;
		s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

		half4 c = BRDF_Custom_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);

		c.a = outputAlpha;
		return c;
	}
	inline void LightingCustomSkin_GI(
		SurfaceOutputStandard s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, lerp(unity_ColorSpaceDielectricSpec.rgb, s.Albedo, s.Metallic));
		gi = UnityGlobalIllumination(data, 1.0, s.Normal,g);
	}
	ENDCG
	}
	FallBack "Diffuse"
}

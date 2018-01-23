Shader "WeatherTest/WeatherTest_PBR_Forward" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}   
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0 
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0     

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0 
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0 
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0  
 
		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}   

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02 
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0 
		_OcclusionMap("Occlusion", 2D) = "white" {} 

		_EmissionColor("Color", Color) = (0,0,0)   
		_EmissionMap("Emission", 2D) = "white" {} 
		
		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}  
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0  

		_TintMask("Tint Mask Single Map(A)", 2D) = "white" {} 
		_MaskMapTiling("MaskMapTiling", Range(0.01, 50)) = 1  
		_TintMaskNorMap("Normal Map 4 TintColor", 2D) = "bump" {}       
		//_TintColor("Tint Color",Color) = (1,1,1,0)       
		//_TintPower("TintPower",Range(0,2)) = 0    
		_TintPowerMaxRange("TintPowerMaxRange",Range(0,2)) = 2   
		_TintNormalEx("Change Color On Side",Range(-0.5,0.5)) = 0.05 
		_TintMetallic("Change Metallic",Range(-1,1)) = 0.0  
		_TintSmoothness("Change Smoothness",Range(-1,1)) = 0.0
		_NormalNoiseMap("NormalNoiseMap (RG) noise Normal",2D) = "Black" {} 
		_NormalNoiseSpeed("NormalNoiseSpeed",Range(0,1)) = 0
		//_NormalNoisePower("NormalNoisePower",Range(0,1)) = 0
		_NormalNoiseTiling("NormalNoiseTiling",Range(0.1,10)) = 1  
		_NormalNoiseSpecGloss("NormalNoiseSpecGloss",Range(1,1024)) = 500
		_NormalNoiseSpecPower("NormalNoiseSpecPower",Range(0.01,5)) = 2  
		_DecalPower("DecalPower",Range(0,1)) = 0.1
		_Decal2Tiling("Decal2Tiling",Range(1,2)) = 1.2
		
		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0 
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}
	
	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT MetallicSetup  
	ENDCG
	  
	SubShader {
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	  
    
		// ------------------------------------------------------------------   
		//  Base forward pass (directional light, emission, lightmaps, ...)  
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM 
			#pragma target 3.0 
			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON                 
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2 
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A  
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF   
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF      
			#pragma shader_feature _PARALLAXMAP  

			#pragma multi_compile_fwdbase    
			#pragma multi_compile_fog
			#pragma multi_compile_instancing 			
			
			#pragma vertex vertBase      
			#pragma fragment fragBase
			#include "../Include/WeatherTest_INC.cginc"          			

			//#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _PARALLAXMAP
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
	}
	FallBack "VertexLit"
	//CustomEditor "StandardShaderGUI"
}

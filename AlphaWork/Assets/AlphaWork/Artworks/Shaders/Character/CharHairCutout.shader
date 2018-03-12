// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TLStudio/CharHairCutout" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex("Normal", 2D) = "white" {}
		_HairMask("Hair Mask", 2D) = "white" {}
		_SpecularGloss1 ("SpecularGloss1", Range(2,800)) = 5
		_ShiftValue1("ShiftValue1", Range(-1.0,1.0)) = 0.0
		_SpecularColor1("SpecularColor1", Color) = (1,1,1,1)
			_SpecularGloss2("SpecularGloss2", Range(2,800)) = 5
			_ShiftValue2("ShiftValue2", Range(-1.0,1.0)) = 0.0
			_SpecularColor2("SpecularColor2", Color) = (1,1,1,1)
		_HairTint1("HairTint1",Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.275
	}
	SubShader {
		Tags { "RenderType"="Cutout" }
		LOD 200
		
		cull off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf CustomHair fullforwardshadows alphatest:_Cutoff vertex:vert

		//#pragma vertex vert
		//#pragma fragment frag

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _HairMask;
		
		struct Input {
			float2 uv_MainTex;
			half3 tangent_input;
		};
		struct SurfaceOutputHair
		{
			fixed3 Albedo;
			fixed Alpha;
			half3 Emission;

			half3 Tangent;
			half3 Normal;
			half3 HairMask;
		};

		fixed4 _HairTint1;
		half _SpecularGloss1;
		half _ShiftValue1;
		fixed4 _SpecularColor1;
		half _SpecularGloss2;
		half _ShiftValue2;
		fixed4 _SpecularColor2;

		void vert(inout appdata_full i, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			half3 p_tangent = UnityObjectToWorldNormal(i.tangent);

			o.tangent_input = normalize(p_tangent.xyz);
		}


		void surf (Input IN, inout SurfaceOutputHair o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Tangent = IN.tangent_input;
			o.HairMask = tex2D(_HairMask, IN.uv_MainTex).rgb;
			fixed4 normalpacked = tex2D(_NormalTex, IN.uv_MainTex);
			o.Normal = normalpacked.rgb*2.0 - 1.0;
		}
		inline half4 LightingCustomHair(SurfaceOutputHair s, half3 viewDir, UnityGI gi)
		{
			UnityLight light = gi.light;
			half3 L = normalize(light.dir);
			half3 N = normalize(s.Normal);
			half NoL = saturate(dot(N, L));
			half diffusefactor = NoL*0.5 + 0.5;
			half4 diffuse = diffusefactor*_HairTint1;

			half3 T = normalize(s.Tangent);
			half3 V = normalize(viewDir);
			half3 H = normalize(V + L);

			half3 T1 = normalize(T + (s.HairMask.g - 0.5 + _ShiftValue1)*N);
			half ToH1 = dot(T1, H);
			half sinTH1 = sqrt(1- ToH1*ToH1);
			half4 specular1 = pow(sinTH1, _SpecularGloss1)*(1 + ToH1)*(1 + ToH1)*(1 - 2 * ToH1) *s.HairMask.r*_SpecularColor1;

			half3 T2 = normalize(T + (s.HairMask.g - 0.5 + _ShiftValue2)*N);
			half ToH2 = dot(T2, H);
			half sinTH2 = sqrt(1 - ToH2*ToH2);
			half4 specular2 = pow(sinTH2, _SpecularGloss2)*(1 + ToH2)*(1 + ToH2)*(1 - 2 * ToH2) *s.HairMask.r*_SpecularColor2;
			//c.a = outputAlpha;
			half4 c = (diffuse*2 + NoL*(specular1+ specular2))*half4(light.color,0.0);
			//half4 c = half4(sinTH, sinTH, sinTH, 0.0);
			return c;
		}
		inline void LightingCustomHair_GI(
			SurfaceOutputHair s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, s.Normal);
		}
		ENDCG
	}
	FallBack "Diffuse"
}

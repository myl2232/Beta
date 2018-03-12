// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CY/BlendForBloom" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Bloom1("Bloom Src 1", 2D) = "" {}
		_Bloom2("Bloom Src 2", 2D) = "" {}
		_Bloom3("Bloom Src 3", 2D) = "" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};
	struct v2f_mt {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};
			
	sampler2D _MainTex;
	sampler2D _Bloom1;
	sampler2D _Bloom2;
	sampler2D _Bloom3;
	

		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] =  v.texcoord.xy;
		
		return o;
	}

	float4 PI_psOffsets[16];
	float4 psWeights[16];

	struct vtxOutGauss
	{
		float4 pos : SV_Position;
		float4 tc0 : TEXCOORD0;
		float4 tc1 : TEXCOORD1;
		float4 tc2 : TEXCOORD2;
		float4 tc3 : TEXCOORD3;
		float4 tc4 : TEXCOORD4;
	};

	vtxOutGauss GaussBlurBilinearVS(appdata_img v)
	{
		vtxOutGauss o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.tc0.xy = v.texcoord.xy + PI_psOffsets[0].xy;
		o.tc0.zw = v.texcoord.xy + PI_psOffsets[1].xy;
		o.tc1.xy = v.texcoord.xy + PI_psOffsets[2].xy;
		o.tc1.zw = v.texcoord.xy + PI_psOffsets[3].xy;
		o.tc2.xy = v.texcoord.xy + PI_psOffsets[4].xy;
		o.tc2.zw = v.texcoord.xy + PI_psOffsets[5].xy;
		o.tc3.xy = v.texcoord.xy + PI_psOffsets[6].xy;
		o.tc3.zw = v.texcoord.xy + PI_psOffsets[7].xy;

		// Original coordinates
		o.tc4.xy = v.texcoord.xy;

		return o;
	}

	half4 GaussBlurBilinearPS(vtxOutGauss IN) : SV_Target
	{
		half4 sum = 0;

		// vanila gaussian blur

		half4 col = tex2D(_MainTex, IN.tc0.xy);
		sum += col * (half) psWeights[0].x;

		col = tex2D(_MainTex, IN.tc0.zw);
		sum += col * (half) psWeights[1].x;

		col = tex2D(_MainTex, IN.tc1.xy);
		sum += col * (half) psWeights[2].x;

		col = tex2D(_MainTex, IN.tc1.zw);
		sum += col * (half) psWeights[3].x;

		col = tex2D(_MainTex, IN.tc2.xy);
		sum += col * (half) psWeights[4].x;

		col = tex2D(_MainTex, IN.tc2.zw);
		sum += col * (half) psWeights[5].x;

		col = tex2D(_MainTex, IN.tc3.xy);
		sum += col * (half) psWeights[6].x;

		col = tex2D(_MainTex, IN.tc3.zw);
		sum += col * (half) psWeights[7].x;

		return sum;
	}

	// Using RGBK format (multiplier in alpha - filtering should work fine)
	// quality: good	
	half4 EncodeRGBK(in half4 Color, const half fMultiplier, bool bUsePPP = false)
	{
		half4 cScale = half4(half3(1.f, 1.f, 1.f) / fMultiplier, 1.f / 255.0);
		half fMax = saturate(dot(half4(Color.rgb, 1.f), cScale));   // 1 alu

		Color.a = ceil(fMax * 255.f) / 255.f;                       // 3 alu

		Color.xyz /= Color.a * fMultiplier;                         // 2alu

		if (bUsePPP)
		{
			//Color *= rsqrt( Color ); // for best quality

			Color.a = sqrt(Color.a); // encode just multiplier for performance reasons
		}

		return Color;
	}
	#define SCENE_HDR_MULTIPLIER 32.f

	half4 fragBloomBlend (v2f i) : SV_Target 
	{
		half4 outColor = 0;
		outColor += tex2D(_Bloom1, i.uv[0].xy);
		outColor += tex2D(_Bloom2, i.uv[0].xy);
		outColor += tex2D(_Bloom3, i.uv[0].xy);
		return EncodeRGBK(half4(outColor.rgb, 1), SCENE_HDR_MULTIPLIER, true);
	}

	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off

	// 0: blend 3 bloom maps
 Pass {    

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBloomBlend
      ENDCG
  }
	 // 1: Gauss Blur
 Pass {    

      CGPROGRAM
      #pragma vertex GaussBlurBilinearVS
      #pragma fragment GaussBlurBilinearPS
      ENDCG
  }
}

Fallback off
	
} // shader

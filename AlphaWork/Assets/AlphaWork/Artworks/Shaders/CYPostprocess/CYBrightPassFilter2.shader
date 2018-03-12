// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CY/BrightPassFilter"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
		_LumTex("Color", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;	
	sampler2D _LumTex;
	
	half4 _BloomParams;	// x = Threshhold	y = brightOffset	z = HDRBrightLevel w = HDRBloomMul
		
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 

	half4 fragBrightPass(v2f i) : SV_Target
	{
		half4 vSample = tex2D(_MainTex, i.uv); // fetch half res scene target and apply range scale
		half fAdaptedLum = tex2D(_LumTex, i.uv).x;

		half Level = _BloomParams.z * _BloomParams.w;

		// Determine what the pixel's value will be after tone-mapping occurs
		vSample.rgb *= Level / (fAdaptedLum + 1e-6);
		vSample.rgb -= _BloomParams.xxx;
		vSample.rgb = clamp(vSample.rgb, 0.0f, 65536.0f);
		vSample.rgb /= (_BloomParams.y + vSample.rgb); // map result to 0 to 1 range

		return vSample * 8.0; // scale range to keep some precision in darks (gets rescaled down to original value during final tone map scene)
	}
	
	inline half Max3(half3 x) { return max(x.x, max(x.y, x.z)); }
	inline half Max3(half x, half y, half z) { return max(x, max(y, z)); }
	
	// Brightness function
	half Brightness(half3 c)
	{
		return Max3(c);
	}

	half4 SampleLumOffsets0;
	half4 SampleLumOffsets1;

	half4 HDRSampleLumInitialPS(v2f i) : SV_Target
	{
		half vLumInfo = 0.0f;

		half3 s1 = tex2D(_MainTex, i.uv + SampleLumOffsets0.xy).rgb;
		half s1w = 1.0 / (Brightness(s1) + 1.0);
		
		half3 s2 = tex2D(_MainTex, i.uv + SampleLumOffsets0.zw).rgb;
		half s2w = 1.0 / (Brightness(s2) + 1.0);
		
		half3 s3 = tex2D(_MainTex, i.uv + SampleLumOffsets1.xy).rgb;
		half s3w = 1.0 / (Brightness(s3) + 1.0);
		
		half3 s4 = tex2D(_MainTex, i.uv + SampleLumOffsets1.zw).rgb;
		half s4w = 1.0 / (Brightness(s4) + 1.0);
		
		half one_div_wsum = 1.0 / (s1w + s2w + s3w + s4w);
		
		// Karis's luma weighted average (using brightness instead of luma)
		half3 sa = (s1 * s1w + s2 * s2w + s3 * s3w + s4 * s4w) * one_div_wsum;
		vLumInfo = Luminance(sa);

		// clamp to acceptable range
		vLumInfo = clamp(vLumInfo, 0.0f, 64.0f);

		return vLumInfo;
	}

	float4 SampleOffsets[16];

	half4 HDRSampleLumIterativePS(v2f i) : SV_Target
	{
		int nIter = 4;
		half nRecipIter = 1.0f / (half) nIter;
		half4 vResampleSum = 0.0f;

		for (int iter = 0; iter<nIter; iter++)
		{
			// Compute the sum of luminance throughout the sample points
			half2 vTex = tex2D(_MainTex, i.uv + SampleOffsets[iter].xy);
			vResampleSum.xy += vTex.xy;
		}

		// Divide the sum to complete the average
		//vResampleSum.x *= nRecipIter;
		vResampleSum.xy *= nRecipIter;

		return vResampleSum;
	}

	ENDCG 
	
	Subshader 
	{
		// 0
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragBrightPass

			ENDCG
		}
		// 1
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment HDRSampleLumInitialPS

			ENDCG
		}
		// 2
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment HDRSampleLumIterativePS

			ENDCG
		}
	}
	Fallback off
}

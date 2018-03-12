#ifndef NOISE_DECAL_INCLUDE  
#define NOISE_DECAL_INCLUDE  	
	
	#include "../Include/CommonCal.cginc"
	#include "TintColor.cginc"
	
	#define VAR_NOISE_DECAL_NEED
		sampler2D _NormalNoiseMap;
		float4 _NormalNoiseMap_ST;
		uniform half _NormalNoisePower;
		half _NormalNoiseSpeed;
		fixed _NormalNoiseTiling;
		uniform half _NormalNoiseMapPower;
		fixed _NormalNoiseSpecGloss;
		fixed _NormalNoiseSpecPower;
		uniform half _DecalPower;
		uniform half _Decal2Tiling;

		#define DECAL_COLOR_WORLD_CENTER(wPos,wNormal,output) output = CAL_DECAL_COLOR_WORLD_CENTER(output,wPos,wNormal);
		float3 CAL_DECAL_COLOR_WORLD_CENTER(float3 withDecalColor,float3 wPos,float3 wNormal) 
		{
			float2 uvTiling = GET_UV_WITH_POS_NOR_CENTER_POS(wPos,wNormal);
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			float3 decalColor1 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling);
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			float3 decalColor2 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling * _Decal2Tiling);

			withDecalColor.rgb += lerp(float3(0,0,0),(decalColor1 * decalColor2 * _DecalPower),_NormalNoisePower);
			return withDecalColor;
		}
		
		#define DECAL_COLOR(wPos,wNormal,output) output = CAL_DECAL_COLOR(output,wPos,wNormal);
		float3 CAL_DECAL_COLOR(float3 withDecalColor,float3 wPos,float3 wNormal) 
		{
			float2 uvTiling = GET_UV_WITH_POS_NOR_CENTER_POS(wPos,wNormal);
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			float3 decalColor1 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling);
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			float3 decalColor2 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling * _Decal2Tiling);

			withDecalColor.rgb += lerp(float3(0,0,0),(decalColor1 * decalColor2 * _DecalPower),_NormalNoisePower);
			return withDecalColor;
		}

		#define DECAL_SPEC(nhVar,shininessVar,outputSpec) outputSpec = CAL_DECAL_SPEC(outputSpec,nhVar,shininessVar);
		float CAL_DECAL_SPEC(float finalSpec,float nh,half shiVar)
		{
			finalSpec = pow(nh, shiVar * 128) * (lerp(1,_NormalNoiseSpecPower,_NormalNoisePower));
			return finalSpec;
		}

		#define NOISE_NOR_RECAL(uvNoise, outputNor) outputNor = EXC_NOISE_NOR_RECAL(outputNor,uvNoise);
		float3 EXC_NOISE_NOR_RECAL(float3 noiseFinalNor,float2 noiseUV)
		{
			float2 noiseNormal1 = tex2D(_NormalNoiseMap,noiseUV * _NormalNoiseTiling +  0.1 * _Time.y * _NormalNoiseSpeed).rg;
			float2 noiseNormal2 = tex2D(_NormalNoiseMap,noiseUV * _NormalNoiseTiling -  0.1 * _Time.y * _NormalNoiseSpeed).rg;
			
			noiseNormal1 = (noiseNormal1 - 0.5) * 2;
			noiseNormal2 = (noiseNormal2 - 0.5) * 2;
			
			float3 noiseNor1 = normalize(float3(noiseNormal1.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal1.y * _DecalPower + noiseFinalNor.z));
			float3 noiseNor2 = normalize(float3(noiseNormal2.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal2.y * _DecalPower + noiseFinalNor.z));
			
			float3 finalNormal = noiseNor1 + noiseNor2;
			noiseFinalNor = normalize(lerp(noiseFinalNor,finalNormal + noiseFinalNor,_NormalNoisePower));
			return noiseFinalNor;
		}
		
		#define NOISE_NOR_RECAL_SNOWMASK(wPos,uvNoise, outputNor) outputNor = EXC_NOISE_NOR_RECAL(outputNor,uvNoise,wPos);
		float3 EXC_NOISE_NOR_RECAL(float3 noiseFinalNor,float2 noiseUV,float3 worldPos)
		{
			float maskValue = CalMaskValue4ChannelMapwithDot(worldPos,noiseFinalNor);
			
			float2 noiseNormal1 = tex2D(_NormalNoiseMap,noiseUV * _NormalNoiseTiling +  0.1 * _Time.y * _NormalNoiseSpeed).rg;
			float2 noiseNormal2 = tex2D(_NormalNoiseMap,noiseUV * _NormalNoiseTiling -  0.1 * _Time.y * _NormalNoiseSpeed).rg;
			
			noiseNormal1 = (noiseNormal1 - 0.5) * 2;
			noiseNormal2 = (noiseNormal2 - 0.5) * 2;
			
			float3 noiseNor1 = normalize(float3(noiseNormal1.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal1.y * _DecalPower + noiseFinalNor.z));
			float3 noiseNor2 = normalize(float3(noiseNormal2.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal2.y * _DecalPower + noiseFinalNor.z));
			
			float3 finalNormal = noiseNor1 + noiseNor2;
			noiseFinalNor = normalize(lerp(noiseFinalNor,finalNormal + noiseFinalNor,min(_NormalNoisePower,saturate(maskValue - _NormalNoiseMapPower))));
			return noiseFinalNor;
			//return float3(min(_NormalNoisePower,maskValue),min(_NormalNoisePower,maskValue),min(_NormalNoisePower,maskValue));//noiseFinalNor;
		}

		#define NOISE_NOR_RECAL_UP_2_DOWN(wPos,wNor) EXC_NOISE_NOR_RECAL_UP_2_DOWN(wNor,wPos);
		float3 EXC_NOISE_NOR_RECAL_UP_2_DOWN(float3 noiseFinalNor,float3 wPos)
		{
			float2 uvTiling = GET_UV_WITH_POS_NOR_CENTER_POS(wPos,noiseFinalNor);
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			
			float2 noiseNormal1 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling).rg;
			uvTiling.y += _Time.y * _NormalNoiseSpeed;
			float2 noiseNormal2 = tex2D(_NormalNoiseMap,uvTiling * _NormalNoiseTiling * _Decal2Tiling).rg;
			
			noiseNormal1 = (noiseNormal1 - 0.5) * 2;
			noiseNormal2 = (noiseNormal2 - 0.5) * 2;

			float3 noiseNor1 = normalize(float3(noiseNormal1.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal1.y * _DecalPower + noiseFinalNor.z));
			float3 noiseNor2 = normalize(float3(noiseNormal2.x * _DecalPower + noiseFinalNor.x,noiseFinalNor.y,noiseNormal2.y * _DecalPower + noiseFinalNor.z));
			
			float3 finalNormal = noiseNor1 + noiseNor2;
			noiseFinalNor = normalize(lerp(noiseFinalNor,finalNormal + noiseFinalNor,_NormalNoisePower));
			return noiseFinalNor;
		}
		
		#define NOISE_SPEC(nhVar,shininessVar,outputSpec) outputSpec = CAL_NOISE_SPEC(outputSpec,nhVar,shininessVar);
		float CAL_NOISE_SPEC(float finalSpec,float nh,half shiVar)
		{
			float specWet = pow(nh, (shiVar * 128) + _NormalNoiseSpecGloss) * _NormalNoiseSpecPower;
			finalSpec = lerp(finalSpec,specWet,_NormalNoisePower);
			return finalSpec;
		}

#endif 
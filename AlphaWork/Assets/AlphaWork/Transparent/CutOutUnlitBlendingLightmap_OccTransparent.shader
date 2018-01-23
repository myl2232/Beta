// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Transparent/Cutout/UnlitBlendingLightmap_OccTransparent" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Float) = 0.5
		
		_LightMap1 ("lightmap1", 2D) = "white" {}
        _LightMap2 ("lightmap2", 2D) = "white" {}
        _LerpWeight("lerp weight",range(0,1)) = 0
	}
	SubShader {
		Tags {"Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass{
		LOD 200
		ZWrite On
		Lighting off
		Fog {Mode off}
		ColorMask 0
		}	// Write Depth
	 
		 Pass
         {
			 LOD 200
			 ZWrite Off
			 Offset -1, 0
             Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
                 #include "UnityCG.cginc"
                 #pragma vertex vert
                 #pragma fragment frag
                 #pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
                 #pragma multi_compile LIGHTMAFADING_ON LIGHTMAFADING_OFF
                 struct v2f{
                     float4 pos : SV_POSITION;
                     fixed2 uv[2] : TEXCOORD0;
                 };
                 
                 sampler2D _MainTex;
                 fixed4 _MainTex_ST;
                 fixed4 _Color;
                 #ifdef LIGHTMAP_ON
                 fixed4 unity_LightmapST;
                 // sampler2D unity_Lightmap;
                 sampler2D _LightMap1;
                 sampler2D _LightMap2;
                 #endif
 				 
 				 float _Cutoff;
                 fixed _LerpWeight;
                  v2f vert(appdata_full v)
                 {
                     v2f o;
                     o.pos = UnityObjectToClipPos(v.vertex);
                     o.uv[0] = TRANSFORM_TEX(v.texcoord, _MainTex);
                     
                     #ifdef LIGHTMAP_ON
                     o.uv[1] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                     #endif
  
                     return o;
                 }
                 fixed4 frag(v2f i) : COLOR
                 {
                     fixed4 c = tex2D(_MainTex, i.uv[0]) * _Color;
                     clip ( c.a - _Cutoff );

                     fixed4 outColor = c;
                     #ifdef LIGHTMAP_ON
                        outColor.rgb = DecodeLightmap(tex2D(_LightMap1, i.uv[1])).rgb * c.rgb;
                        #ifdef LIGHTMAFADING_ON
                            fixed3 t2 = DecodeLightmap(tex2D(_LightMap2, i.uv[1])).rgb * c.rgb;
                            outColor.rgb = lerp( outColor.rgb, t2, _LerpWeight);
                        #endif
                     #endif
                     return outColor;
                 }
             ENDCG
         }
         Pass
         {
             Tags { "LightMode"="ForwardAdd"}
             LOD 200
             ZWrite Off
             Blend SrcAlpha One
 
             CGPROGRAM
                 #include "UnityCG.cginc"
                 #include "Lighting.cginc"
                 #include "AutoLight.cginc"
                 #pragma multi_compile_fwdadd  
                 #pragma vertex vert
                 #pragma fragment frag
                 struct v2f{
                     float4 pos : SV_POSITION;
                     fixed2 uv: TEXCOORD0;
                     float3 normal : TEXCOORD2;  
                     float3 lightDir : TEXCOORD3;
                     LIGHTING_COORDS(4,5) 
                  };
                 sampler2D _MainTex;
                 fixed4 _MainTex_ST;
                 fixed4 _Color;
 
                 float _Cutoff;
                 v2f vert(appdata_full v)
                 {
                     v2f o;
                     o.pos = UnityObjectToClipPos(v.vertex);
                     o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                     o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                     o.lightDir =  normalize(WorldSpaceLightDir(v.vertex));
                     TRANSFER_VERTEX_TO_FRAGMENT(o); 
                     return o;
                 }

                 fixed4 frag(v2f i) : COLOR
                 {
                     fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                     clip ( c.a - _Cutoff );
                     float atten = LIGHT_ATTENUATION(i);
                     fixed diff = max (0, dot (i.normal, i.lightDir));
                     return  c * _LightColor0 * (diff * atten *2);
                 }
             ENDCG
         }
	} 
	FallBack "Transparent/Cutout/VertexLit"
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Transparent/Cutout/Diffuse_AlphaTest" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Float) = 0.5
	}
	SubShader {
		Tags {"Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass{
			LOD 200
			ZWrite On
			ColorMask 0
			Lighting on
			Fog {Mode off}
			AlphaTest Greater[_Cutoff]
			
			CGPROGRAM	
            #include "UnityCG.cginc"
			
            #pragma vertex vert
            #pragma fragment frag
            
			sampler2D _MainTex;
			fixed4 _Color;
			float _Cutoff;

            struct vertexInput {
            	float4 vertex	: POSITION;
				half2 uv 		: TEXCOORD0;
         	};
         	struct vertexOutput {
            	float4 pos 		: SV_POSITION;
				half2 uv 		: TEXCOORD0;
         	};

            vertexOutput vert(vertexInput input) {
                vertexOutput output;
                output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = input.uv;
                return output;
            }
             
            fixed4 frag(vertexOutput input):COLOR{
				fixed4 c = tex2D(_MainTex, input.uv) *_Color;
				//clip ( c.a - _Cutoff );
                return c;  
            }   
			
			ENDCG
		}	// Write Depth			

	} 
	FallBack "Transparent/Cutout/VertexLit"
}

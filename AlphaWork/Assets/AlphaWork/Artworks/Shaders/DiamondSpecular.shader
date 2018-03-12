Shader "DFM/Diamond Specular" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _CosAngle  ("CosAngle ", Float) = 0.8
      _AmbientScale  ("AmbientScale ", Float) = 0.5
      _Specular  ("Specular ", Float) = 0.8
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf SimpleSpecular
      
		  uniform float _CosAngle;
		  uniform float _Specular;
		  uniform float _AmbientScale;
		  
      half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
      
          float nh = dot (s.Normal, lightDir);
          nh = sign(nh-_CosAngle);
          nh = max(0,nh);

          float spec = nh*_Specular;

          half4 c;
          c.rgb = _AmbientScale*s.Albedo + _LightColor0.rgb * spec;
          c.a = s.Alpha;
          return c;
      }

      struct Input {
          float2 uv_MainTex;
      };
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
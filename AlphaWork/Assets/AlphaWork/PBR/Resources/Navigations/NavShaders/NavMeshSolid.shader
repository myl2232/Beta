Shader "Custom/NavMeshSolid" {
	Properties
	{
	_NavMeshColor ("NavMesh Color", Color) = (0.0, 0.0, 1.0)
	}
    SubShader {
      Tags { "RenderType" = "Transparent" }
      CGPROGRAM
      #pragma surface surf Lambert alpha
      struct Input {
          float4 color : Color;
      };
      float4 _NavMeshColor;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = IN.color.rgb;
		  o.Alpha = 0.5;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
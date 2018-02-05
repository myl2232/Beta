Shader "Custom/NavMeshWireframe" {
	Properties
	{
	_WireframeColor ("Wireframe Color", Color) = (0.1, 0.1, 0.1)
	}
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float4 color : Color;
      };
      float4 _WireframeColor;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _WireframeColor;
		  o.Alpha = 1.0;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
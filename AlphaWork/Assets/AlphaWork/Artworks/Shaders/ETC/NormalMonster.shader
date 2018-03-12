// Unlit shader. Simplest possible textured shader.
// - DFM Simplest Shader

Shader "DFM/ETC/DFM/NormalMonster" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskTex ("MaskTex", 2D) = "white" {}
	_RParam  ("R Param", Float) = 1
	_GParam  ("G Param", Float) = 0
	_BParam  ("B Param", Float) = 0
}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Transparent-10"}
	LOD 100
	
	// Non-lightmapped
	Pass {
        Name "NORMALMONSTER"
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}




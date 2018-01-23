Shader "Mobile/Transparent/Vertex Color" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	//_AlphaCutout("Alpha CutOut" , Range(0,1)) = 0.7
}

Category {
	Tags {"Queue"="Transparent-10" "IgnoreProjector"="True" "RenderType"="Transparent"}
	//ZWrite Off
	Alphatest Greater 0.5
	Blend SrcAlpha OneMinusSrcAlpha 
	SubShader {
		Pass {
		
	    	Material {
			    Diffuse [_Color]
			    Ambient [_Color]
			    Shininess [_Shininess]
			    Specular [_SpecColor]
			    Emission [_Emission]	
		    }
			//ColorMaterial AmbientAndDiffuse
			Fog { Mode Off }
			Lighting Off
			SeparateSpecular On
        	SetTexture [_MainTex] {
        	constantColor[_Color]
            Combine texture * constant DOUBLE, texture * primary
        }

		}
	} 
}
}
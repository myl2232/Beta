// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:0,x:35135,y:32442,varname:node_0,prsc:2|emission-8282-OUT,alpha-478-OUT,refract-14-OUT;n:type:ShaderForge.SFN_Multiply,id:14,x:34871,y:32718,varname:node_14,prsc:2|A-16-OUT,B-6401-A,C-4651-OUT;n:type:ShaderForge.SFN_ComponentMask,id:16,x:34622,y:32590,varname:node_16,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-25-RGB;n:type:ShaderForge.SFN_Tex2d,id:25,x:34260,y:32547,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2793-UVOUT;n:type:ShaderForge.SFN_Vector1,id:478,x:34849,y:32590,varname:node_478,prsc:2,v1:0;n:type:ShaderForge.SFN_VertexColor,id:6401,x:34379,y:32823,varname:node_6401,prsc:2;n:type:ShaderForge.SFN_Slider,id:4651,x:34252,y:33037,ptovrint:True,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:_RefractionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Clamp,id:5272,x:32650,y:33603,varname:node_5272,prsc:2|MIN-8142-OUT,MAX-4797-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4797,x:32213,y:33674,ptovrint:False,ptlb:node_7492_copy,ptin:_node_7492_copy,varname:_node_7492_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8142,x:32213,y:33590,ptovrint:False,ptlb:node_547_copy,ptin:_node_547_copy,varname:_node_547_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8282,x:34699,y:32449,varname:node_8282,prsc:2|A-25-RGB,B-6401-RGB;n:type:ShaderForge.SFN_Rotator,id:2793,x:34073,y:32548,varname:node_2793,prsc:2|UVIN-8493-UVOUT,ANG-5212-OUT;n:type:ShaderForge.SFN_TexCoord,id:8493,x:33839,y:32548,varname:node_8493,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:5212,x:33824,y:32826,ptovrint:False,ptlb:fangxiang(0~1.57~3.14~4.71),ptin:_fangxiang0157314471,varname:node_5212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:4.71;proporder:25-4651-5212;pass:END;sub:END;*/

Shader "Rhino/A3_SimpleDistortion" {
    Properties {
        _Refraction ("Refraction", 2D) = "white" {}
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0.1
        _fangxiang0157314471 ("fangxiang(0~1.57~3.14~4.71)", Range(0, 4.71)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float _RefractionIntensity;
            uniform float _fangxiang0157314471;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float node_2793_ang = _fangxiang0157314471;
                float node_2793_spd = 1.0;
                float node_2793_cos = cos(node_2793_spd*node_2793_ang);
                float node_2793_sin = sin(node_2793_spd*node_2793_ang);
                float2 node_2793_piv = float2(0.5,0.5);
                float2 node_2793 = (mul(i.uv0-node_2793_piv,float2x2( node_2793_cos, -node_2793_sin, node_2793_sin, node_2793_cos))+node_2793_piv);
                float4 _Refraction_var = tex2D(_Refraction,TRANSFORM_TEX(node_2793, _Refraction));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_Refraction_var.rgb.rg*i.vertexColor.a*_RefractionIntensity);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float3 emissive = (_Refraction_var.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

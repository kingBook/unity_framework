Shader "Custom/Skybox/Background 2D" 
{
    Properties 
    {
        _Color("  Color", color) = (1, 1, 1, 1)
        _MainTex ("  Map", 2D) = "white" {}
    }

    SubShader 
    {
        Tags{
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Background" 
        }
        LOD 200
        Cull Off ZWrite Off

        Pass 
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                sampler2D _MainTex;
            CBUFFER_END

			struct vertIn {
                float4 vertex   : POSITION;
				half3 normal    : NORMAL;
                float2 uv       : TEXCOORD0;
            };

            struct vertOut 
            {
                float4 pos : SV_POSITION;
                float4 scrPos : TEXCOORD0;
            };

            vertOut vert(vertIn v) 
            {
                vertOut o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.scrPos = ComputeScreenPos(o.pos);
                return o;
            }

            half4  frag(vertOut i) : SV_Target 
            {	
                float2 uv = (i.scrPos.xy / i.scrPos.w);
                return tex2D(_MainTex, uv) * _Color;
            }

            ENDHLSL
        }
    } 
} 
 

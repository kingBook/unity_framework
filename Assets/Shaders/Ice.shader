Shader "Custom/Ice" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("BaseTex ", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BumpAmt ("Distortion", range (0,2)) = 0.1
    }

    SubShader {
		Tags{
            "RenderPipeline" = "UniversalPipeline"
			"Queue" = "Transparent"
            "RenderType" = "Opaque" 
        }
        ZWrite off

        HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma surface surf Lambert nolightmap nodirlightmap alpha:blend

            sampler2D _BumpMap;
            sampler2D _MainTex;
            half4 _Color;
            float _BumpAmt;

            struct Input {
                float2 uv_MainTex;
                float2 uv_BumpMap;
            };

            void surf (Input IN, inout SurfaceOutput o) {
                half3 nor = UnpackNormal (tex2D(_BumpMap, IN.uv_BumpMap));
                half4 trans =tex2D(_MainTex,IN.uv_MainTex+nor.xy*_BumpAmt)*_Color;

                o.Albedo = trans.rgb;
                o.Alpha =trans.a;
                o.Emission = trans; 
            }
        ENDHLSL
    }
    FallBack "Universal Render Pipeline/Lit"
}
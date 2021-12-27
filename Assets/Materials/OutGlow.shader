// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/OutGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OffsetUV("OffsetUV", Range(0, 0.005)) = 0
        _EdgeColor("EdgeColor", Color) = (1, 0, 0, 1)
        _AlphaTreshold("Treshold", Range(0, 1)) = 0.5
        _EdgeWidth("EdgeWidth",Range(0,0.03)) = 0.003
        _EdgeColorPower("EdgeColorPower",Range(0,1))=1
        _EdgeStep("EdgeStep",Range(0,0.005)) = 0.001
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed2 uv[5] : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _OffsetUV;
            fixed4 _EdgeColor;
            fixed _AlphaTreshold;
            fixed _EdgeWidth;
            fixed _EdgeColorPower;
            fixed _EdgeStep;

            struct CheckData
            {
                fixed alpha;
                bool isCanDraw;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv[0] = v.uv;
                o.uv[1] = v.uv + fixed2(0, _OffsetUV); //up  
                o.uv[2] = v.uv + fixed2(-_OffsetUV, 0); //left  
                o.uv[3] = v.uv + fixed2(0, -_OffsetUV); //bottom  
                o.uv[4] = v.uv + fixed2(_OffsetUV, 0); //right 

                return o;
            }


            fixed check(fixed2 uv)
            {
                fixed alpha = 0;
                fixed step = _EdgeStep;
                fixed curStep = step;
                fixed4 fColor = tex2D(_MainTex, uv);
                fixed2 curUv = uv;
                if (fColor.a <= 0)
                {
                    [unroll(20)] for (; curStep <= _EdgeWidth;)
                    {
                        curUv = uv + fixed2(0, curStep);
                        fColor = tex2D(_MainTex, curUv);
                        if (fColor.a <= 0)
                        {
                            curUv = uv + fixed2(-curStep, 0);
                            fColor = tex2D(_MainTex, curUv);
                        }
                        else
                        {
                            break;
                        }
                        if (fColor.a <= 0)
                        {
                            curUv = uv + fixed2(0, -curStep);
                            fColor = tex2D(_MainTex, curUv);
                        }
                        else
                        {
                            break;
                        }
                        if (fColor.a <= 0)
                        {
                            curUv = uv + fixed2(curStep, 0);
                            fColor = tex2D(_MainTex, curUv);
                        }
                        else
                        {
                            break;
                        }
                        curStep += step;
                    }
                }


                if (fColor.a > 0)
                {
                    fixed dis = distance(uv, curUv);
                    alpha = _EdgeColor.a*(1-dis/ _EdgeWidth);
                }
                    return alpha;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv[0]);
                fixed alpha = original.a;
                fixed p1 = tex2D(_MainTex, i.uv[1]).a;
                fixed p2 = tex2D(_MainTex, i.uv[2]).a;
                fixed p3 = tex2D(_MainTex, i.uv[3]).a;
                fixed p4 = tex2D(_MainTex, i.uv[4]).a;

                alpha = p1 + p2 + p3 + p4 + alpha;
                alpha /= 5;
                if (alpha <= _AlphaTreshold)
                {
                    original.rgb = _EdgeColor.rgb+ _EdgeColorPower;
                    original.a = check(i.uv[0]);
                }
                return original;
            }
            ENDCG
        }
    }
}
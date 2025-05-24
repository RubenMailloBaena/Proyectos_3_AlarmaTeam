Shader "Custom/Bridge"
{
    Properties
    {
        _Color("Color", Color) = (1, 0, 0, 1)
        _Emission("Emission", Float) = 1
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Float) = 0.02
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }

        ZTest Always
        ZWrite Off
        Cull Off

        // --- OUTLINE PASS ---
        Pass
        {
            Name "OUTLINE"
            Cull Front   // Dibuja las caras traseras para que el borde se vea detrás del modelo
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v.vertex.xyz += v.normal * _OutlineWidth;
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // --- MAIN PASS ---
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;
            float _Emission;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color;
                col.rgb *= _Emission;
                return col;
            }
            ENDCG
        }
    }

        FallBack "Diffuse"
}

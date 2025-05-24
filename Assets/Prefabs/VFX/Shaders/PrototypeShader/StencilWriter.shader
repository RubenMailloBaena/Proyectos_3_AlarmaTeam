Shader "Custom/HeartVision"
{
    Properties
    {
        _Color("Color", Color) = (1, 0, 0, 1)
        _Emission("Emission", Float) = 1
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }

        ZTest Always      // Se dibuja aunque haya algo delante
        ZWrite Off        // No escribe en el z-buffer
        Cull Off          // Dibuja ambas caras

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha   // Blend normal (transparente), no suma el color

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

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = _Color;
                col.rgb *= _Emission;  // Aplica leve brillo si quieres usar Bloom
                return col;
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}

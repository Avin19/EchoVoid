Shader "EchoVoid/EchoPulse"
{
    Properties
    {
        _Color("Color", Color) = (0, 1, 1, 1)
        _Width("Ring Width", Range(0.001, 0.5)) = 0.1
        _Fade("Fade", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha One
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Width;
                float _Fade;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * 2.0 - 1.0;
                float dist = length(uv);

                // ring mask
                float ring = smoothstep(1.0, 1.0 - _Width, dist);
                ring = 1.0 - ring;

                // fade and glow
                float alpha = ring * _Fade;
                half4 col = half4(_Color.rgb, alpha);
                return col;
            }
            ENDHLSL
        }
    }
}

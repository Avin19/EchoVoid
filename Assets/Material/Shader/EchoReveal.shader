Shader "EchoVoid/EchoReveal"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0, 0, 0, 1)
        _GlowColor("Glow Color", Color) = (0, 1, 1, 1)
        _GlowStrength("Glow Strength", Range(0, 10)) = 3
        _ObjectPosition("Object Position", Vector) = (0, 0, 0, 0)
        _RevealRadius("Reveal Radius", Float) = 0
        _Fade("Fade", Range(0,1)) = 0
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "EchoRevealPass"
            Tags { "LightMode" = "UniversalForward" }

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
                float3 worldPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _GlowColor;
                float _GlowStrength;
                float4 _ObjectPosition;
                float _RevealRadius;
                float _Fade;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _BaseColor;

                // Distance from pulse origin
                float dist = distance(IN.worldPos, _ObjectPosition.xyz);

                // Generate glowing mask based on distance & radius
                float revealMask = saturate(1.0 - (dist / _RevealRadius));

                // Smooth edge transition for nicer look
                revealMask = smoothstep(0.0, 1.0, revealMask);

                // Apply fade
                float glowFactor = revealMask * _Fade;

                // Calculate emission
                float3 emission = _GlowColor.rgb * glowFactor * _GlowStrength;

                // Output final color (black base + cyan emission)
                half4 finalColor = half4(baseCol.rgb + emission, 1.0);
                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack Off
}

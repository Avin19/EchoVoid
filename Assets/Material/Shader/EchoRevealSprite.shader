Shader "EchoVoid/EchoRevealSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.25,0.25,0.25,1)
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _GlowStrength ("Glow Strength", Range(0,10)) = 4
        _ObjectPosition ("Object Position", Vector) = (0,0,0,0)
        _RevealRadius ("Reveal Radius", Float) = 0
        _Fade ("Fade", Range(0,1)) = 0
        [PerRendererData] _Color ("Sprite Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "Sprite"="True" }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "FORWARD"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _GlowColor;
                float _GlowStrength;
                float4 _ObjectPosition;
                float _RevealRadius;
                float _Fade;
                float4 _Color;
            CBUFFER_END

            // textures
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.texcoord;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
{
    half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
    half3 baseRGB = tex.rgb * (_BaseColor.rgb * _Color.rgb);

    float dist = distance(IN.worldPos, _ObjectPosition.xyz);

    // wave shimmer
    float wave = sin(dist * 40 - _Time.y * 15) * 0.05;
    float mask = saturate(1.0 - ((dist + wave) / _RevealRadius));
    mask = smoothstep(0.0, 1.0, mask);
    mask = pow(mask, 1.4);

    // optional noise
    #ifdef _NOISETEX
        float2 noiseUV = IN.uv * 4.0 + _Time.y * 0.05;
        float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
        mask *= lerp(0.7, 1.3, noise);
    #endif

    float glowFactor = mask * _Fade;
    float3 emission = _GlowColor.rgb * glowFactor * _GlowStrength;

    half4 outColor = half4(baseRGB + emission, tex.a * _Color.a);
    return outColor;
}

            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}

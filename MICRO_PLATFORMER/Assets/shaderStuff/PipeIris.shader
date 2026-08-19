Shader "UI/Simple Iris"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Iris ("Iris", Range(0,1)) = 0
        _HoleRadius ("Hole Radius", Range(0.01,0.6)) = 0.35
        _EdgeSoftness ("Edge Softness", Range(0.001,0.1)) = 0.01

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

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
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)

            float4 _Color;
            float _Iris;
            float _HoleRadius;
            float _EdgeSoftness;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
{
    float2 centre = float2(0.5, 0.5);

    float2 offset =
        input.uv - centre;

    float aspect =
        _ScreenParams.x /
        _ScreenParams.y;

    offset.x *= aspect;

    float distanceFromCentre =
        length(offset);

    /*
     * Iris = 0:
     * Completely transparent.
     *
     * Iris = 1:
     * Fully closed.
     */

    float radius =
        _HoleRadius * (1.0 - _Iris);

    float edge =
        max(_EdgeSoftness, 0.0001);

    float holeMask =
        smoothstep(
            radius,
            radius + edge,
            distanceFromCentre
        );

    float transitionAlpha =
        _Iris * holeMask;

    return half4(
        _Color.rgb,
        transitionAlpha *
        _Color.a *
        input.color.a
    );
}

            ENDHLSL
        }
    }
}
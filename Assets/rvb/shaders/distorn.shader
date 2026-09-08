Shader "distorn"
{
    Properties
    {
        // Property identifiers intentionally match the Cocos Effect.
        [MainTexture][NoScaleOffset] mainTexture ("Main Texture", 2D) = "white" {}
        [NoScaleOffset] detailTexture ("Detail Texture", 2D) = "black" {}
        strenghten ("Strenghten", Float) = 0.05
        speed ("Speed (XY)", Vector) = (0.05, 0.05, 0.0, 0.0)
        detailColorFactor ("Detail Color Factor", Float) = 1.0
        [MainColor] mainColor ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }

            // Cocos defaults: cullMode=back, depthFunc=less, depthWrite=true.
            Cull Back
            ZTest Less
            ZWrite On
            Blend Off

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float  fogFactor  : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(mainTexture);
            SAMPLER(sampler_mainTexture);
            TEXTURE2D(detailTexture);
            SAMPLER(sampler_detailTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 mainColor;
                float4 speed; // Cocos vec2 -> Unity Vector; only XY are used.
                float  strenghten;
                float  detailColorFactor;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // cc_time.x is elapsed seconds; Unity's elapsed seconds is _Time.y.
                float2 animUV = input.uv + _Time.y * speed.xy;
                float4 detailColor = SAMPLE_TEXTURE2D(
                    detailTexture,
                    sampler_detailTexture,
                    animUV
                );

                float gray = dot(
                    detailColor.rgb,
                    float3(0.299, 0.587, 0.114)
                );

                float2 offset =
                    (detailColor.rg - 0.5) *
                    2.0 *
                    strenghten *
                    gray;

                float4 color =
                    mainColor *
                    SAMPLE_TEXTURE2D(
                        mainTexture,
                        sampler_mainTexture,
                        input.uv + offset
                    );

                color.rgb += detailColor.rgb * detailColorFactor;
                color.rgb = MixFog(color.rgb, input.fogFactor);
                return color;
            }
            ENDHLSL
        }
    }

    Fallback Off
}

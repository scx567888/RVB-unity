// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "scx/Unlit/Transparent_Hit" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    _Color ("Color", Color) = (0,0,0)
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite On
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        Cull Off
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;
            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.texcoord);
    clip(col.a - _Cutoff);

    // --- getHitColor3 的 HLSL 实现 ---
    float r = col.r;
    float g = col.g;
    float b = col.b;

    // 亮度
    float luma = 0.299*r + 0.587*g + 0.114*b;

    // 阈值
    float midGray = 150.0/255.0;
    float highGray = 1.0;

    // 基准橙色
    float3 darkCol  = float3(230.0,120.0,10.0)/255.0;
    float3 brightCol = float3(255.0,220.0,140.0)/255.0;
    float3 whiteCol  = float3(1.0,1.0,1.0);

    float3 newCol;

    if(luma <= midGray)
    {
        float factor = sqrt(luma / midGray);
        newCol = darkCol + (brightCol - darkCol) * factor;
    }
    else
    {
        float factor = sqrt((luma - midGray)/(highGray - midGray));
        newCol = brightCol + (whiteCol - brightCol) * factor;
    }

    col.rgb = newCol;

    UNITY_APPLY_FOG(i.fogCoord, col);
    return col;
}

        ENDCG
    }
}

}

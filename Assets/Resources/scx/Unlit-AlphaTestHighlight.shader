Shader "scx/Unlit/Transparent Cutout_Highlight" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    _Color ("Tint Color", Color) = (0,0,0)
    _EdgeStrength ("Edge Strength", Range(0,5)) = 1.5
    _InnerStrength ("Inner Strength", Range(0,1)) = 0.4
    _Radius ("Edge Radius", Range(1,16)) = 4
    _Brightness ("Brightness Boost", Range(1,3)) = 1.5 // 增亮参数
    _Saturation ("Saturation Boost", Range(1,3)) = 1.5 // 饱和度参数
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 100

    Lighting Off

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

            float4 _MainTex_TexelSize;
            float  _EdgeStrength;
            float  _InnerStrength;
            float  _Radius;
            float  _Brightness;
            float  _Saturation;

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

            // --- 简单增加饱和度函数 ---
            float3 BoostSaturation(float3 col, float sat)
            {
                float grey = dot(col, float3(0.299,0.587,0.114));
                return lerp(float3(grey,grey,grey), col, sat);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                clip(col.a - _Cutoff);

                float2 px = _MainTex_TexelSize.xy;

                float nearEdge = 0;
                float2 dirs[8] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(1,-1), float2(-1,1), float2(-1,-1)
                };

                for(int k=0; k<8; k++)
                {
                    float2 uv2 = i.texcoord + dirs[k] * px * _Radius;
                    nearEdge += (tex2D(_MainTex, uv2).a < 0.1);
                }

                nearEdge = saturate(nearEdge / 8.0);

                // 内部基础染色（轻微 Tint）
                float3 tinted = lerp(col.rgb, _Color.rgb, _InnerStrength);

                // 边缘增强
                float edgeFactor = nearEdge * _EdgeStrength;
                float3 finalColor = lerp(tinted, _Color.rgb, edgeFactor);

                // 增亮
                finalColor = saturate(finalColor * _Brightness);

                // 增加饱和度
                finalColor = BoostSaturation(finalColor, _Saturation);

                return float4(finalColor, col.a);
            }
        ENDCG
    }
}

}

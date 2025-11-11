Shader "scx/Unlit/Transparent_Highlight" {
Properties {
    _MainTex ("Main Tex", 2D) = "white" {}
    _Color ("Tint Color", Color) = (1,0,0,1)
    _EdgeStrength ("Edge Strength", Range(0,5)) = 1.5
    _InnerStrength ("Inner Strength", Range(0,1)) = 0.4
    _Radius ("Edge Radius", Range(1,16)) = 4
    _Brightness ("Brightness Boost", Range(1,3)) = 1.5 // 增亮参数
    _Saturation ("Saturation Boost", Range(1,3)) = 1.5 // 饱和度参数
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
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _Color;
            float  _EdgeStrength;
            float  _InnerStrength;
            float  _Radius;
            float  _Brightness;
            float  _Saturation;

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // --- 简单增加饱和度函数 ---
            float3 BoostSaturation(float3 col, float sat)
            {
                float grey = dot(col, float3(0.299,0.587,0.114));
                return lerp(float3(grey,grey,grey), col, sat);
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                if (col.a == 0) return col;

                float2 px = _MainTex_TexelSize.xy;

                float nearEdge = 0;
                float2 dirs[8] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(1,-1), float2(-1,1), float2(-1,-1)
                };

                for(int k=0; k<8; k++)
                {
                    float2 uv2 = i.uv + dirs[k] * px * _Radius;
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

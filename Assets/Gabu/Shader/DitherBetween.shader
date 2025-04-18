Shader "CustomRenderTexture/DitherBetweenWithTexture"
{
    Properties
    {
        _PointA("Point A", Vector) = (0,0,0,0)  // camera position
        _PointB("Point B", Vector) = (1,0,0,0)  // player position
        _MainTex("Texture", 2D) = "white" {}
        _Amount("Amount", float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _PointA;
            float4 _PointB;
            float _Amount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenUV : TEXCOORD1;
                float distance : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = v.vertex.xy;

                float3 A = _PointA.xyz;
                float3 B = _PointB.xyz;
                float3 C = o.pos;

                float3 AB = B - A;
                float3 AC = C - A;

                float3 ABn = normalize(AB);
                float t = dot(AC, ABn);
                float3 P = A + ABn * t;

                o.distance = distance(C, P);
                return o;
            }

            float dither4x4(float2 uv)
            {
                int2 p = int2(fmod(uv, 4.0));
                const float thresholdMatrix[16] = {
                     0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                    12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
                     3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                    15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
                };
                int index = p.y * 4 + p.x;
                return thresholdMatrix[index];
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = i.distance;

                // 正規化距離: 近いほど 0, 遠いほど 1
                float normalized = saturate(dist - _Amount);

                // ディザマスク: 小さい距離ではcutされやすい
                float dither = dither4x4(i.screenUV * 16.0);

                clip(normalized - dither);

                fixed4 color = tex2D(_MainTex, i.uv);
                color.a *= (normalized - dither); // アルファ値を適用
                return color;
            }

            ENDCG
        }
    }
}

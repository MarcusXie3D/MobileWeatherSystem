// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/singlePixelPostProcess" {
Properties {
    _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
}

SubShader {
    Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}//overlay, draw on top of everthing
    LOD 100

    Cull Off
    ZTest Always
    ZWrite Off
    //Blend One One
    Blend One SrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = float4(v.vertex.x * 2.0, v.vertex.y * 2.0, 1.0 , 1.0);
                //o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return fixed4(0.7, 0.75, 1.0, 0.5);
                return _Color;
            }
        ENDCG
    }
}

}

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Custom/TerrainSnowCompatible" {
Properties {
    _BrickTex ("Brick Texture", 2D) = "white" {}
    _GrassTex ("Grass Texture", 2D) = "white" {}
    _TexMixer ("Texture Mixer", 2D) = "white" {}
    _SnowTex ("Snow Tex", 2D) = "white" {}
    _SnowHeavy ("Snow Heavy", Range(0.0, 1.0)) = 0.0
}
SubShader {
    Tags { "RenderType"="Opaque"}// "Queue" = "Geometry-100" 
    LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd vertex:vert

sampler2D _BrickTex;
sampler2D _GrassTex;
sampler2D _TexMixer;
sampler2D _SnowTex;
fixed _SnowHeavy;

struct Input {
    float2 uv_BrickTex;
    //float2 uv_GrassTex;
    float2 uv_TexMixer;
    fixed2 snow;
};

void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input,o);
    fixed4 wpos = mul(unity_ObjectToWorld, v.vertex);
    o.snow = wpos.xz;
}

void surf (Input IN, inout SurfaceOutput o) {
    fixed3 colorBrick = tex2D(_BrickTex, IN.uv_BrickTex).rgb;
    fixed3 colorGrass = tex2D(_GrassTex, IN.uv_BrickTex).rgb;
    half texMixer = tex2D(_TexMixer, IN.uv_TexMixer).b;
    fixed3 c = lerp(colorGrass, colorBrick, texMixer);
    fixed snowIntense = tex2D(_SnowTex, IN.snow.xy * 0.1).r * _SnowHeavy;
    fixed3 albedo = lerp(c.rgb, fixed3(1.0, 1.0, 1.0), snowIntense);
    o.Albedo = albedo;
    o.Alpha = 1.0;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}

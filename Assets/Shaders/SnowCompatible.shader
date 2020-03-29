// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Custom/Snow Compatible" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
    _SnowTex ("Snow Tex", 2D) = "white" {}
    _SnowHeavy ("Snow Heavy", Range(0.0, 1.0)) = 0.0
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf Lambert noforwardadd vertex:vert

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SnowTex;
fixed _SnowHeavy;

struct Input {
    float2 uv_MainTex;
    fixed3 snow;
};

void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input,o);

    //o.snow.z = max(v.normal.z, 0.0);
    fixed3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
    worldNormal = normalize(worldNormal);
    o.snow.z = max(worldNormal.y, 0.0);
    
    fixed4 wpos = mul(unity_ObjectToWorld, v.vertex);
    o.snow.xy = wpos.xz;
}

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    fixed snowIntense = tex2D(_SnowTex, IN.snow.xy * 0.1).r * IN.snow.z * _SnowHeavy;
    fixed3 albedo = lerp(c.rgb, fixed3(1.0, 1.0, 1.0), snowIntense);
    o.Albedo = albedo;
    o.Alpha = c.a;
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
}
ENDCG
}

FallBack "Mobile/Diffuse"
}

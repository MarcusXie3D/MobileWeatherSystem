Shader "Custom/PlantsSnowCompatible" {
Properties {
    //_Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    _SnowTex ("Snow Tex", 2D) = "white" {}
    _SnowHeavy ("Snow Heavy", Range(0.0, 1.0)) = 0.0
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200

CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert

sampler2D _MainTex;
//fixed4 _Color;
sampler2D _SnowTex;
fixed _SnowHeavy;

struct Input {
    float2 uv_MainTex;
    // integrate 2 values into 1 interpolator
    fixed3 snow;
};

void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input,o);
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
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
//Fallback "Unlit/Color"
}

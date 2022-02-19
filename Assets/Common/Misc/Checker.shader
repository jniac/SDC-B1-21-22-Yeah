Shader "Custom/Checker"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.9, 0.9, 0.9, 1)
        _ColorB ("Color B", Color) = (0.8, 0.8, 0.8, 1)
        _ColorC ("Line Color", Color) = (1.0, 1.0, 1.0, 1)
        _Line ("Line Thickness", Range(0,1)) = 0.02
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        half _Line;
        fixed4 _ColorA;
        fixed4 _ColorB;
        fixed4 _ColorC;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float sx = frac(IN.worldPos.x / 2 + 0.001) > 0.5 ? -1 : 1;
            float sy = frac(IN.worldPos.y / 2 + 0.001) > 0.5 ? -1 : 1;
            float sz = frac(IN.worldPos.z / 2 + 0.001) > 0.5 ? -1 : 1;
            float fx = abs(frac(IN.worldPos.x + _Line / 2));
            float fy = abs(frac(IN.worldPos.y + _Line / 2));
            float fz = abs(frac(IN.worldPos.z + _Line / 2));
            float d = max(max(min(fx, fz), min(fx, fy)), min(fy, fz));
            fixed4 color = d < _Line ? _ColorC : (sx * sy * sz > 0 ? _ColorB : _ColorA);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

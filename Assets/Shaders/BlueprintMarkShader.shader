// Shader for general objects on the blueprint gameboard. Main use is for Emission and Color.
Shader "Custom/BlueprintMarkShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Emission ("Emission", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        #pragma target 3.0

        sampler2D _MainTex;

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
        };

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        half _Emission;
        fixed4 _Color;
        
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // Emission is in the same color as the color.
            o.Emission = _Emission * c.rgb;

            // Remove all pixels below a certain transparency.
            if(c.a < 0.5) discard;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

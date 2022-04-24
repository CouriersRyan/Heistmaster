// Wave and oscillating grain of disappearing pixels.
Shader "Unlit/HologramShader"
{
    Properties
    {
        _MainTex ("Albedo Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _Transparency ("Transparency", Range(0.0, 0.5)) = 0.25
        _Emission ("Emission", Range(0.0, 1.0)) = 0.5
        _CutoutThresh ("Cutout  Threshold", Range(0.0, 1.0)) = 0.2
        _Distance ("Distance", Float) = 1
        _Amplitude ("Amplitude", Float) = 1
        _Speed ("Speed", Float) = 1
        _Amount ("Amount", Float) = 1
        }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Transparency;
            float _Emission;
            float _CutoutThresh;
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.x += sin(_Time.y * _Speed + v.vertex.z * _Amplitude) * _Distance * _Amount;
                v.vertex.z += sin(_Time.y * _Speed + v.vertex.x * _Amplitude) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
                col = col + (tex2D(_MainTex, i.uv) *   _Emission);
                col.a = _Transparency;
                
                if (col.r < clamp(_CutoutThresh + 1.4 + 0.6 * sin(_Time.y * 2), 0.4, 0.95)) discard;
                return col;
            }
            ENDCG
        }
    }
}

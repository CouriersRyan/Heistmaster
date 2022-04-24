//Shader for selection marker. Oscillates expanding and contracting.
Shader "Unlit/SelectMarkerUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Emission ("Emission", Range(0.0, 1.0)) = 0.5
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        
        _OscillationStrength ("Oscillation Strength", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Emission;
            float4 _TintColor;

            float _OscillationStrength;

            v2f vert (appdata v)
            {
                v2f o;
                float3 offset = normalize(v.vertex) * (_OscillationStrength * sin(_Time.y) + 1.4);
                offset.y = 0;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
                if(col.a < 0.5) discard;
                return col;
            }
            ENDCG
        }
    }
}

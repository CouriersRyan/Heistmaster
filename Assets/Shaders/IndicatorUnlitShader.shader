// Shader for the pop-up that occurs when player is interacting with something.
Shader "Unlit/IndicatorUnlitShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "black" {}
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _Color ("Color", Color) = (1, 1 , 1, 1)
        _MinWidth ("Minimum Width", Range(0, 1)) = 0.3
        _RangeWidth ("Range of Width", Range(0.01, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        // Program used to display something after culling the front from the Pass below this. 
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };
        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float _Transparency;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = _Transparency;
        }
        ENDCG
        
        // Draws the oscillating outline.
        Pass
        {
            Cull front
            
            CGPROGRAM

            #include "UnityCG.cginc"
            
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            float4 _Color;
            float _MinWidth;
            float _RangeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                float width = _MinWidth + _RangeWidth * ((sin(_Time.y) + 1) / 2);
                o.vertex = UnityObjectToClipPos(v.vertex + normalize(v.normal) * width);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}

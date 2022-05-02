// Highlights and outlines units in the game.
Shader "Custom/UnitSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 0.003)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        // First pass of the shader draws the standard texture of the mesh.
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

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
        
        
        // Draws the outline.
        Pass {
            // Removes all front pixels, so that only back pixels remain, creating an illusion of an outline.
            Cull front
            
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #include "UnityCG.cginc"

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _OutlineColor;
            float _OutlineThickness;

            // data put into vertex
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            // data outputted by vertex shader and goes into frag.
            struct v2f
            {
                float4 position : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // gets vertext positions and converts them to clip space for rendering.
                o.position = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_TARGET{
                return  _OutlineColor;
            }
            
            ENDCG
            
            }
    }
    FallBack "Standard"
}

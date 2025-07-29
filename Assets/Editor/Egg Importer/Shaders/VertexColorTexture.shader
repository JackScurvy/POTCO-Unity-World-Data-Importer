Shader "EggImporter/VertexColorTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        float4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        void vert (inout appdata_full v)
        {
            // Vertex colors are automatically passed through in Surface Shaders
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Sample texture
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            
            // Apply vertex colors and material color
            fixed4 finalColor = texColor * IN.color * _Color;
            
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    
    Fallback "Legacy Shaders/VertexLit"
}
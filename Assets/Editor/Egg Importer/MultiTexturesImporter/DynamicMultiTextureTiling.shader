Shader "EggImporter/DynamicMultiTextureTiling"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BlendTex ("Overlay Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        
        // Base texture tiling
        _BaseTileU ("Base Tile Frequency U", Float) = 1.0
        _BaseTileV ("Base Tile Frequency V", Float) = 1.0
        
        // Overlay texture tiling
        _OverlayTileU ("Overlay Tile Frequency U", Float) = 1.0
        _OverlayTileV ("Overlay Tile Frequency V", Float) = 1.0
        
        _BlendMode ("Blend Strength", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        
        sampler2D _MainTex;
        sampler2D _BlendTex;
        fixed4 _Color;
        
        float _BaseTileU;
        float _BaseTileV;
        float _OverlayTileU;
        float _OverlayTileV;
        
        half _BlendMode;
        half _Metallic;
        half _Glossiness;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv2_BlendTex;
            float4 vertexColor : COLOR;
        };
        
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexColor = v.color;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Apply dynamic tiling to base texture
            float2 baseTiledUV = IN.uv_MainTex * float2(_BaseTileU, _BaseTileV);
            fixed4 baseColor = tex2D(_MainTex, baseTiledUV);
            
            // Apply dynamic tiling to overlay texture using UV2 channel
            float2 overlayTiledUV = IN.uv2_BlendTex * float2(_OverlayTileU, _OverlayTileV);
            fixed4 overlayColor = tex2D(_BlendTex, overlayTiledUV);
            
            // FIXED: Better blending that preserves base texture visibility
            // Use overlay alpha and blend mode for mixing, but preserve more base color
            fixed4 blendedColor = baseColor;
            blendedColor.rgb = lerp(baseColor.rgb, overlayColor.rgb, _BlendMode * overlayColor.a * 0.5);
            
            // Apply color tint 
            blendedColor *= _Color;
            
            // ORIGINAL LOOK: Pure multiplicative vertex colors like before shaders
            blendedColor.rgb *= IN.vertexColor.rgb;
            
            o.Albedo = blendedColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = blendedColor.a * IN.vertexColor.a;
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}
Shader "EggImporter/MultiTextureBlend"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BlendTex ("Blend Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BlendMode ("Blend Mode", Range(0,1)) = 0.5
        _BlendScale ("Blend UV Scale", Float) = 8.0
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;   // Primary UVs for base texture
                float2 uv2 : TEXCOORD1;  // Overlay UVs for tiling texture
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 blendUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _BlendTex;
            float4 _MainTex_ST;
            float4 _BlendTex_ST;
            float4 _Color;
            float _BlendMode;
            float _BlendScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Use primary UVs for base texture (perfect ground texture mapping)
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Use overlay UVs for tiling texture (proper world-space coordinates)
                o.blendUV = TRANSFORM_TEX(v.uv2, _BlendTex) * _BlendScale;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample base texture (ground) with corrected UVs
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                
                // Sample blend texture (grass) with scaled UVs for tiling
                fixed4 blendColor = tex2D(_BlendTex, i.blendUV);
                
                // Use multiplicative blending for darker, more realistic appearance
                // This matches Panda3D's modulate blending behavior
                fixed4 finalColor = baseColor * lerp(fixed4(1,1,1,1), blendColor, _BlendMode);
                
                // Apply vertex colors and material color
                finalColor *= i.color * _Color;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
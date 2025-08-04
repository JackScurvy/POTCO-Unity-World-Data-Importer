using UnityEngine;
using System.Collections.Generic;

public class EggVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv; // Primary UV set
    public Dictionary<string, Vector2> namedUVs = new Dictionary<string, Vector2>(); // Named UV sets
    public Color color = Color.white;
    public Dictionary<string, float> boneWeights = new Dictionary<string, float>();
    
    // Helper method to get UV coordinates for a specific texture
    public Vector2 GetUVForTexture(string textureName, Dictionary<string, string> textureUVMappings)
    {
        // Check if this texture has a specific UV set mapping
        if (textureUVMappings != null && textureUVMappings.TryGetValue(textureName, out string uvSetName))
        {
            // Check for normalized overlay UV
            if (namedUVs.TryGetValue("overlay_uv", out Vector2 overlayUV))
            {
                return overlayUV;
            }
            
            // Check for original named UV
            if (namedUVs.TryGetValue(uvSetName, out Vector2 namedUV))
            {
                return namedUV;
            }
        }
        
        // Check if this texture is marked as secondary in multi-texture polygons
        bool isSecondaryTexture = SecondaryTextureRegistry.IsSecondaryTexture(textureName);
        
        if (isSecondaryTexture)
        {
            // For secondary textures, always prefer secondary UV sets
            // First check for overlay_uv (our normalized multi-texture UVs)
            if (namedUVs.TryGetValue("overlay_uv", out Vector2 overlayUV))
            {
                return overlayUV;
            }
            
            // Then try common secondary UV set names
            string[] commonSecondaryNames = { "uvNoise", "uvSet", "muti-sand", "Sand" };
            foreach (string uvName in commonSecondaryNames)
            {
                if (namedUVs.TryGetValue(uvName, out Vector2 secondaryUV))
                {
                    UnityEngine.Debug.Log($"🎯 Using secondary UV set '{uvName}' for texture '{textureName}' (detected as secondary texture)");
                    return secondaryUV;
                }
            }
            
            UnityEngine.Debug.LogWarning($"⚠️ Secondary texture '{textureName}' has no UV mapping and no secondary UV sets found");
        }
        
        // Fall back to primary UV
        return uv;
    }
}
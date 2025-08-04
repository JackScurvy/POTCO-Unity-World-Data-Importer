using System.Collections.Generic;

// Global registry to track which textures are secondary in multi-texture polygons
public static class SecondaryTextureRegistry
{
    private static HashSet<string> _secondaryTextures = new HashSet<string>();
    
    public static void AddSecondaryTexture(string textureName)
    {
        _secondaryTextures.Add(textureName);
    }
    
    public static bool IsSecondaryTexture(string textureName)
    {
        return _secondaryTextures.Contains(textureName);
    }
    
    public static void Clear()
    {
        _secondaryTextures.Clear();
    }
}
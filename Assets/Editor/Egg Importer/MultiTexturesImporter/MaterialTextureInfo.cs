using System.Collections.Generic;

// Tracks texture position information for multi-texture materials
public class MaterialTextureInfo
{
    public Dictionary<string, int> texturePositions = new Dictionary<string, int>();
    
    public void AddTexture(string textureName, int position)
    {
        texturePositions[textureName] = position;
    }
    
    public bool IsSecondaryTexture(string textureName)
    {
        return texturePositions.TryGetValue(textureName, out int position) && position > 0;
    }
}
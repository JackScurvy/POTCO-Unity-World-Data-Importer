using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using POTCO.Editor;
using System.Linq;

public class MaterialHandler
{
    // Cache frequently used shaders to avoid repeated Shader.Find calls
    private static Shader _vertexColorShader;
    private static Shader _legacyDiffuseShader;
    private static Shader _standardShader;
    
    // Cache common property IDs for better performance
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private static readonly int MetallicPropertyId = Shader.PropertyToID("_Metallic");
    private static readonly int GlossinessPropertyId = Shader.PropertyToID("_Glossiness");
    
    // Cache for default colors to avoid repeated string operations
    private static readonly Dictionary<string, Color> DefaultColorCache = new Dictionary<string, Color>();
    public List<Material> CreateMaterials(Dictionary<string, string> texturePaths, GameObject rootGO)
    {
        DebugLogger.LogEggImporter($"Creating materials from {texturePaths.Count} texture paths");
        // Pre-size the list to avoid resizing during population
        var materials = new List<Material>(texturePaths.Count + 1);
        
        foreach (var kvp in texturePaths)
        {
            string materialName = kvp.Key;
            string texturePath = kvp.Value;
            
            DebugLogger.LogEggImporter($"Creating material: {materialName} with texture: {texturePath}");
            
            Material mat = CreateVertexColorMaterial(materialName);
            
            string textureFileName = Path.GetFileName(texturePath);
            Texture2D texture = FindTextureInProject(textureFileName);
            
            if (texture != null)
            {
                mat.mainTexture = texture;
                DebugLogger.LogEggImporter($"Assigned texture {textureFileName} to material {materialName}");
            }
            else
            {
                DebugLogger.LogWarningEggImporter($"Could not find texture: {textureFileName}");
                mat.color = GetDefaultColorForMaterial(materialName);
            }
            
            // Use cached property IDs for better performance
            if (mat.HasProperty(MetallicPropertyId))
                mat.SetFloat(MetallicPropertyId, 0.0f);
            if (mat.HasProperty(GlossinessPropertyId))
                mat.SetFloat(GlossinessPropertyId, 0.1f);
            
            materials.Add(mat);
        }
        
        // Always ensure Default-Material exists for polygons that don't specify textures
        var defaultMaterial = CreateVertexColorMaterial("Default-Material");
        materials.Add(defaultMaterial);
        
        if (materials.Count == 1) // Only default material was added
        {
            DebugLogger.LogEggImporter("No textures found, using default material only");
        }
        
        return materials;
    }
    
    private Texture2D FindTextureInProject(string textureFileName)
    {
        string[] guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(textureFileName) + " t:texture2D");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string foundFileName = Path.GetFileName(path);
            
            if (foundFileName.Equals(textureFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    DebugLogger.LogEggImporter($"Found texture at: {path}");
                    return texture;
                }
            }
        }
        
        DebugLogger.LogWarningEggImporter($"Texture not found in project: {textureFileName}");
        return null;
    }
    
    private Color GetDefaultColorForMaterial(string materialName)
    {
        // Use cache to avoid repeated string operations for same material names
        if (DefaultColorCache.TryGetValue(materialName, out Color cachedColor))
            return cachedColor;
            
        string lowerName = materialName.ToLower();
        Color color;
        
        if (lowerName.Contains("skin") || lowerName.Contains("flesh"))
            color = new Color(1f, 0.8f, 0.7f);
        else if (lowerName.Contains("metal") || lowerName.Contains("steel"))
            color = new Color(0.7f, 0.7f, 0.8f);
        else if (lowerName.Contains("wood"))
            color = new Color(0.6f, 0.4f, 0.2f);
        else if (lowerName.Contains("grass") || lowerName.Contains("leaf"))
            color = new Color(0.2f, 0.8f, 0.2f);
        else if (lowerName.Contains("water"))
            color = new Color(0.2f, 0.5f, 0.8f);
        else if (lowerName.Contains("stone") || lowerName.Contains("rock"))
            color = new Color(0.5f, 0.5f, 0.5f);
        else
            color = new Color(0.7f, 0.7f, 0.7f);
            
        // Cache the result for future use
        DefaultColorCache[materialName] = color;
        return color;
    }
    
    public Dictionary<string, Material> CreateMaterialDictionary(List<Material> materials)
    {
        // Pre-size dictionary to avoid resizing
        var materialDict = new Dictionary<string, Material>(materials.Count);
        
        foreach (var mat in materials)
        {
            materialDict[mat.name] = mat;
        }
        
        if (!materialDict.ContainsKey("Default-Material") && materials.Count > 0)
        {
            // Find the Default-Material or use the first material
            var defaultMat = materials.FirstOrDefault(m => m.name == "Default-Material") ?? materials[0];
            materialDict["Default-Material"] = defaultMat;
        }
        
        return materialDict;
    }
    
    private Material CreateVertexColorMaterial(string materialName)
    {
        // Cache shaders to avoid repeated Shader.Find calls
        Shader shader = GetCachedVertexColorShader();
        
        Material mat = new Material(shader) { name = materialName };
        
        // Set material color to white so vertex colors show through properly
        if (mat.HasProperty(ColorPropertyId))
            mat.SetColor(ColorPropertyId, Color.white);
        else
            mat.color = Color.white;
        
        // Set standard shader properties if they exist using cached property IDs
        if (mat.HasProperty(MetallicPropertyId))
            mat.SetFloat(MetallicPropertyId, 0.0f);
        if (mat.HasProperty(GlossinessPropertyId))
            mat.SetFloat(GlossinessPropertyId, 0.1f);
            
        DebugLogger.LogEggImporter($"Created vertex color material '{materialName}' using shader: {shader.name}");
        
        return mat;
    }
    
    private Shader GetCachedVertexColorShader()
    {
        // Return cached shader if available
        if (_vertexColorShader != null) return _vertexColorShader;
        
        // First try to use our custom vertex color shader
        _vertexColorShader = Shader.Find("EggImporter/VertexColorTexture");
        
        if (_vertexColorShader != null) return _vertexColorShader;
        
        // Fallback to Legacy Shaders/Diffuse if custom shader not found
        if (_legacyDiffuseShader == null)
        {
            _legacyDiffuseShader = Shader.Find("Legacy Shaders/Diffuse");
            if (_legacyDiffuseShader != null)
            {
                DebugLogger.LogWarningEggImporter("Custom EggImporter/VertexColorTexture shader not found, falling back to Legacy Shaders/Diffuse");
                return _legacyDiffuseShader;
            }
        }
        else
        {
            return _legacyDiffuseShader;
        }
        
        // Last resort - Standard shader
        if (_standardShader == null)
        {
            _standardShader = Shader.Find("Standard");
            if (_standardShader != null)
            {
                DebugLogger.LogWarningEggImporter("Legacy Shaders/Diffuse not found, using Standard shader (vertex colors may not display)");
            }
        }
        
        return _standardShader;
    }
}
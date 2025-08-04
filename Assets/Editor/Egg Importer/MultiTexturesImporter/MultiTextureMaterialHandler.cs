using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using POTCO.Editor;

public class MultiTextureMaterialHandler
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
        
        // Check if this looks like a DelFuego-pattern model that needs overlay treatment
        bool isDelFuegoModel = IsDelFuegoPatternModel(texturePaths);
        
        foreach (var kvp in texturePaths)
        {
            string materialName = kvp.Key;
            string texturePath = kvp.Value;
            
            DebugLogger.LogEggImporter($"Creating material: {materialName} with texture: {texturePath}");
            
            Material mat;
            
            // Special handling for DelFuego-pattern models needing overlay treatment
            if (isDelFuegoModel && ShouldCreateOverlayMaterial(materialName, texturePaths))
            {
                mat = CreateDelFuegoOverlayMaterial(materialName, texturePaths);
                DebugLogger.LogEggImporter($"🔥 Created DelFuego overlay material: {materialName}");
            }
            else
            {
                mat = CreateVertexColorMaterial(materialName);
                
                string textureFileName = Path.GetFileName(texturePath);
                Texture2D texture = FindTextureInProject(textureFileName);
                
                if (texture != null)
                {
                    mat.mainTexture = texture;
                    
                    // Set texture wrap mode based on research findings
                    texture.wrapMode = DetermineTextureWrapMode(textureFileName, materialName);
                    DebugLogger.LogEggImporter($"Assigned texture {textureFileName} with {texture.wrapMode} wrap mode");
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
            }
            
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
    
    public List<Material> CreateMaterialsWithMultiTexture(Dictionary<string, string> texturePaths, List<string> materialNames, GameObject rootGO, Vector4 uvBounds)
    {
        DebugLogger.LogEggImporter($"🔍 Creating materials from {texturePaths.Count} texture paths and {materialNames.Count} material names");
        DebugLogger.LogEggImporter($"🔍 Texture paths: {string.Join(", ", texturePaths.Keys)}");
        DebugLogger.LogEggImporter($"🔍 Material names: {string.Join(", ", materialNames)}");
        
        // Pre-size the list to avoid resizing during population
        var materials = new List<Material>();
        
        // First create single-texture materials
        foreach (var kvp in texturePaths)
        {
            string materialName = kvp.Key;
            string texturePath = kvp.Value;
            
            DebugLogger.LogEggImporter($"Creating single-texture material: {materialName} with texture: {texturePath}");
            
            Material mat = CreateVertexColorMaterial(materialName);
            
            string textureFileName = Path.GetFileName(texturePath);
            Texture2D texture = FindTextureInProject(textureFileName);
            
            if (texture != null)
            {
                mat.mainTexture = texture;
                
                // Set texture wrap mode based on research findings
                texture.wrapMode = DetermineTextureWrapMode(textureFileName, materialName);
                DebugLogger.LogEggImporter($"Assigned texture {textureFileName} with {texture.wrapMode} wrap mode");
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
        
        // Create multi-texture materials for combined material names
        int multiTextureCount = 0;
        foreach (string materialName in materialNames)
        {
            if (materialName.Contains("||")) // Multi-texture material
            {
                multiTextureCount++;
                var textureNames = materialName.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                if (textureNames.Length >= 2)
                {
                    DebugLogger.LogEggImporter($"🎨 Creating multi-texture material #{multiTextureCount}: {materialName}");
                    DebugLogger.LogEggImporter($"🎨 Texture components: [{string.Join(", ", textureNames)}]");
                    Material multiMat = CreateMultiTextureMaterial(materialName, textureNames, texturePaths, uvBounds);
                    materials.Add(multiMat);
                }
            }
            else if (!texturePaths.ContainsKey(materialName) && materialName != "Default-Material")
            {
                // Check if this material has already been created
                bool alreadyExists = materials.Any(m => m.name == materialName);
                if (!alreadyExists)
                {
                    DebugLogger.LogEggImporter($"Creating basic material for: {materialName}");
                    Material basicMat = CreateVertexColorMaterial(materialName);
                    basicMat.color = GetDefaultColorForMaterial(materialName);
                    materials.Add(basicMat);
                }
            }
        }
        
        DebugLogger.LogEggImporter($"🎨 Created {multiTextureCount} multi-texture materials total");
        
        // Always ensure Default-Material exists for polygons that don't specify textures
        var defaultMaterial = CreateVertexColorMaterial("Default-Material");
        materials.Add(defaultMaterial);
        
        DebugLogger.LogEggImporter($"🔍 Total materials created: {materials.Count}");
        
        return materials;
    }
    
    private Texture2D FindTextureInProject(string textureFileName)
    {
        DebugLogger.LogEggImporter($"🔍 Searching for texture: '{textureFileName}'");
        string searchName = Path.GetFileNameWithoutExtension(textureFileName);
        DebugLogger.LogEggImporter($"🔍 Search name (without extension): '{searchName}'");
        
        string[] guids = AssetDatabase.FindAssets(searchName + " t:texture2D");
        DebugLogger.LogEggImporter($"🔍 Found {guids.Length} potential matches");
        
        // List ALL files found by Unity's search
        DebugLogger.LogEggImporter($"🔍 ALL FILES FOUND:");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string foundFileName = Path.GetFileName(path);
            DebugLogger.LogEggImporter($"🔍   File: '{foundFileName}' at '{path}'");
        }
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string foundFileName = Path.GetFileName(path);
            DebugLogger.LogEggImporter($"🔍 Checking: '{foundFileName}' at path: '{path}'");
            
            if (foundFileName.Equals(textureFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    DebugLogger.LogEggImporter($"✅ Found texture at: {path}");
                    return texture;
                }
                else
                {
                    DebugLogger.LogErrorEggImporter($"🚨 Texture file exists but failed to load: {path}");
                }
            }
            else
            {
                DebugLogger.LogEggImporter($"🔍 Name mismatch: expected '{textureFileName}', found '{foundFileName}'");
            }
        }
        
        // Try alternative search method - search for any texture with this exact name
        DebugLogger.LogEggImporter($"🔍 Trying alternative search for exact filename...");
        string[] allTextureGuids = AssetDatabase.FindAssets("t:texture2D");
        DebugLogger.LogEggImporter($"🔍 Searching through {allTextureGuids.Length} total textures...");
        
        foreach (string guid in allTextureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string foundFileName = Path.GetFileName(path);
            
            if (foundFileName.Equals(textureFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                DebugLogger.LogEggImporter($"🔍 ALTERNATIVE SEARCH: Found exact match '{foundFileName}' at '{path}'");
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    DebugLogger.LogEggImporter($"✅ Successfully loaded texture via alternative search: {path}");
                    return texture;
                }
            }
        }
        
        DebugLogger.LogWarningEggImporter($"🚨 Texture not found in project: {textureFileName}");
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
        // REVERTED: Back to original vertex color shader approach
        Shader shader = GetCachedVertexColorShader();
        
        Material mat = new Material(shader) { name = materialName };
        
        // REVERTED: Original white color for proper vertex color display
        if (mat.HasProperty(ColorPropertyId))
            mat.SetColor(ColorPropertyId, Color.white);
        else
            mat.color = Color.white;
        
        // REVERTED: Minimal shader properties for vertex color shader
        if (mat.HasProperty(MetallicPropertyId))
            mat.SetFloat(MetallicPropertyId, 0.0f);
        if (mat.HasProperty(GlossinessPropertyId))
            mat.SetFloat(GlossinessPropertyId, 0.1f);
            
        DebugLogger.LogEggImporter($"Created vertex color material '{materialName}' using shader: {shader.name}");
        
        return mat;
    }
    
    // REMOVED: Dynamic tiling shader methods - reverted to pre-shader state
    
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
    
    // REMOVED: Standard shader helpers - reverted to vertex color approach
    
    private Material CreateMultiTextureMaterial(string materialName, string[] textureNames, Dictionary<string, string> texturePaths, Vector4 uvBounds)
    {
        DebugLogger.LogEggImporter($"🔧 Creating multi-texture material: {materialName}");
        DebugLogger.LogEggImporter($"🔧 Texture names array: [{string.Join(", ", textureNames)}]");
        
        // NEW APPROACH: Create dual-texture material for main + tiling overlay
        Material mat = new Material(Shader.Find("Standard")) { name = materialName };
        
        // Find the base/main texture (usually the island palette)
        Texture2D mainTexture = null;
        Texture2D overlayTexture = null;
        
        foreach (string textureName in textureNames)
        {
            if (texturePaths.ContainsKey(textureName))
            {
                string texturePath = texturePaths[textureName];
                string textureFileName = Path.GetFileName(texturePath);
                Texture2D texture = FindTextureInProject(textureFileName);
                
                if (texture != null)
                {
                    // Identify main vs overlay texture by name pattern (FIXED: proper priority)
                    string lowerTexture = textureName.ToLower();
                    
                    // FIRST: Check for overlay textures (multi_ prefix is strongest indicator)
                    if (lowerTexture.Contains("multi_"))
                    {
                        overlayTexture = texture;
                        texture.wrapMode = TextureWrapMode.Repeat; // Overlay texture uses repeat for tiling
                        DebugLogger.LogEggImporter($"🔄 Assigned tiling overlay texture: {textureFileName}");
                    }
                    // SECOND: Everything else becomes main texture (palette, cliff, volcano, etc.)
                    else
                    {
                        mainTexture = texture;
                        texture.wrapMode = TextureWrapMode.Clamp; // Base texture uses clamp
                        DebugLogger.LogEggImporter($"🎨 Assigned main texture: {textureFileName}");
                    }
                }
            }
        }
        
        // Set up the material with main texture and overlay
        if (mainTexture != null)
        {
            mat.mainTexture = mainTexture;
            DebugLogger.LogEggImporter($"✅ Main texture applied: {mainTexture.name}");
        }
        
        // For overlay, we'll use detail texture or create a custom shader approach
        if (overlayTexture != null)
        {
            // Try to use detail texture for overlay if Standard shader supports it
            if (mat.HasProperty("_DetailAlbedoMap"))
            {
                mat.SetTexture("_DetailAlbedoMap", overlayTexture);
                mat.SetFloat("_DetailNormalMapScale", 1.0f);
                DebugLogger.LogEggImporter($"✅ Overlay texture applied as detail: {overlayTexture.name}");
            }
            else
            {
                // Fallback: Use overlay as main texture if no detail support
                mat.mainTexture = overlayTexture;
                DebugLogger.LogEggImporter($"⚠️ Fallback: Using overlay as main texture: {overlayTexture.name}");
            }
        }
        
        // Set material properties for better appearance
        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", 0.0f);
        if (mat.HasProperty("_Glossiness"))
            mat.SetFloat("_Glossiness", 0.1f);
        
        return mat;
    }
    
    private TextureWrapMode DetermineTextureWrapMode(string textureFileName, string materialName)
    {
        // ENHANCED APPROACH: Force Repeat for all tiling textures, Clamp only for true atlases
        string lowerTexture = textureFileName.ToLower();
        string lowerMaterial = materialName.ToLower();
        
        // FORCE REPEAT for all DelFuego-type tiling textures
        if (lowerTexture.Contains("multi") || lowerTexture.Contains("rock") || lowerTexture.Contains("grass") || 
            lowerTexture.Contains("sand") || lowerTexture.Contains("cliff") || lowerTexture.Contains("ground") ||
            lowerMaterial.Contains("delfuego") || lowerMaterial.Contains("tortuga") || lowerMaterial.Contains("cuba"))
        {
            DebugLogger.LogEggImporter($"🔄 FORCING Repeat wrap mode for tiling texture: {textureFileName}");
            return TextureWrapMode.Repeat;
        }
        
        // Only clamp true palette/atlas textures (with palette in name)
        if (lowerTexture.Contains("palette") && !lowerTexture.Contains("multi"))
        {
            DebugLogger.LogEggImporter($"🔒 Using Clamp wrap mode for true palette texture: {textureFileName}");
            return TextureWrapMode.Clamp;
        }
        
        // Default to Repeat for everything else - better safe than sorry for tiling
        DebugLogger.LogEggImporter($"🔄 Using default Repeat wrap mode for texture: {textureFileName}");
        return TextureWrapMode.Repeat;
    }
    
    // DELFUEGO OVERLAY SYSTEM: Detect and create overlay materials for padres-style islands
    
    // Detect if this is a DelFuego-pattern model that needs overlay treatment
    private bool IsDelFuegoPatternModel(Dictionary<string, string> texturePaths)
    {
        // Look for DelFuego-style texture patterns (rule-compliant detection)
        bool hasIslandPalette = texturePaths.Keys.Any(k => k.ToLower().Contains("island") && k.ToLower().Contains("palette"));
        bool hasMultiTextures = texturePaths.Keys.Any(k => k.ToLower().Contains("multi_"));
        bool hasRockTextures = texturePaths.Keys.Any(k => k.ToLower().Contains("rock") || k.ToLower().Contains("cliff"));
        
        // DelFuego pattern: has base palette + multiple overlay textures
        return hasIslandPalette && (hasMultiTextures || hasRockTextures);
    }
    
    // Determine if this specific material should get overlay treatment
    private bool ShouldCreateOverlayMaterial(string materialName, Dictionary<string, string> texturePaths)
    {
        string lowerMaterial = materialName.ToLower();
        
        // Create overlay for materials that have both base and detail textures available
        bool isBaseTexture = lowerMaterial.Contains("island") && lowerMaterial.Contains("palette");
        
        if (isBaseTexture)
        {
            // Look for matching overlay textures
            bool hasMatchingOverlay = texturePaths.Keys.Any(k => 
                k.ToLower().Contains("multi_") || 
                k.ToLower().Contains("rock") || 
                k.ToLower().Contains("cliff"));
            
            return hasMatchingOverlay;
        }
        
        return false;
    }
    
    // Create DelFuego-style overlay material with base + tiling detail
    private Material CreateDelFuegoOverlayMaterial(string materialName, Dictionary<string, string> texturePaths)
    {
        // Use Standard shader for detail mapping support
        Material mat = new Material(Shader.Find("Standard")) { name = materialName };
        
        // Find base texture (island palette)
        string baseTexturePath = texturePaths[materialName];
        string baseTextureFileName = Path.GetFileName(baseTexturePath);
        Texture2D baseTexture = FindTextureInProject(baseTextureFileName);
        
        // Find overlay texture (multi/rock/cliff)
        Texture2D overlayTexture = null;
        foreach (var kvp in texturePaths)
        {
            string overlayMaterial = kvp.Key.ToLower();
            if (overlayMaterial.Contains("multi_") || overlayMaterial.Contains("rock") || overlayMaterial.Contains("cliff"))
            {
                string overlayTextureFileName = Path.GetFileName(kvp.Value);
                overlayTexture = FindTextureInProject(overlayTextureFileName);
                if (overlayTexture != null)
                {
                    DebugLogger.LogEggImporter($"🔥 Found overlay texture for DelFuego: {overlayTextureFileName}");
                    break;
                }
            }
        }
        
        // Set up base texture
        if (baseTexture != null)
        {
            mat.mainTexture = baseTexture;
            baseTexture.wrapMode = TextureWrapMode.Clamp; // Base uses clamp
            DebugLogger.LogEggImporter($"🎨 DelFuego base texture: {baseTexture.name}");
        }
        
        // Set up overlay texture as detail with proper tiling scale
        if (overlayTexture != null)
        {
            overlayTexture.wrapMode = TextureWrapMode.Repeat; // Overlay uses repeat for tiling
            
            if (mat.HasProperty("_DetailAlbedoMap"))
            {
                mat.SetTexture("_DetailAlbedoMap", overlayTexture);
                mat.SetFloat("_DetailNormalMapScale", 1.0f);
                
                // DelFuego analysis: uvNoise range ~55 vs regular UV range ~2.6 = ~21x scaling
                // Increase tiling for more prominent repetition
                Vector2 detailTiling = new Vector2(40.0f, 40.0f); // 40x tiling for more repetition
                mat.SetTextureScale("_DetailAlbedoMap", detailTiling);
                
                // Increase detail intensity for more prominent overlay
                if (mat.HasProperty("_DetailAlbedoMapScale"))
                    mat.SetFloat("_DetailAlbedoMapScale", 2.0f);
                    
                DebugLogger.LogEggImporter($"🔄 DelFuego overlay texture: {overlayTexture.name} (20x tiling as detail map)");
            }
        }
        
        // Standard material properties
        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", 0.0f);
        if (mat.HasProperty("_Glossiness"))
            mat.SetFloat("_Glossiness", 0.1f);
            
        return mat;
    }
    
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using POTCO.Editor;
using System.IO;

public class MultiTextureEggImporter
{
    private MultiTextureParserUtilities _parserUtils;
    private MultiTextureGeometryProcessor _geometryProcessor;
    private MultiTextureAnimationProcessor _animationProcessor;
    private MultiTextureMaterialHandler _materialHandler;
    
    // Fields exactly like the reference implementation
    private List<Material> _materials;
    private Dictionary<string, Material> _materialDict;
    private Vector3[] _masterVertices;
    private Vector3[] _masterNormals;
    private Vector2[] _masterUVs;
    private Color[] _masterColors;
    private Dictionary<string, EggJoint> _joints;
    private EggJoint _rootJoint;
    private bool _hasSkeletalData = false;
    private GameObject _rootBoneObject;
    
    public MultiTextureEggImporter()
    {
        _parserUtils = new MultiTextureParserUtilities();
        _geometryProcessor = new MultiTextureGeometryProcessor();
        _animationProcessor = new MultiTextureAnimationProcessor();
        _materialHandler = new MultiTextureMaterialHandler();
    }
    
    public void ImportEggFile(string[] lines, GameObject rootGO, UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        DebugLogger.LogEggImporter($"🔥 Starting multi-texture EGG import: {rootGO.name}");
        
        try
        {
            if (lines == null || lines.Length == 0)
            {
                DebugLogger.LogErrorEggImporter($"Failed to read EGG file or file is empty");
                return;
            }
            
            // Use the exact same approach as the reference HandleGeometryFile
            HandleGeometryFile(lines, rootGO, ctx);
            
            DebugLogger.LogEggImporter($"✅ Multi-texture EGG import completed: {rootGO.name}");
        }
        catch (System.Exception e)
        {
            DebugLogger.LogErrorEggImporter($"Error during multi-texture EGG import: {e.Message}");
            DebugLogger.LogErrorEggImporter($"Stack trace: {e.StackTrace}");
            throw; // Re-throw to let the main importer handle cleanup
        }
    }
    
    // Copy the exact HandleGeometryFile method from the reference implementation
    private void HandleGeometryFile(string[] lines, GameObject rootGO, UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        DebugLogger.LogEggImporter("Processing geometry EGG file");
        // --- Pass 1: Parse all raw data into memory ---
        // Pre-size collections based on typical EGG file contents
        var vertexPool = new List<EggVertex>(1024); // Typical vertex count estimate
        var texturePaths = new Dictionary<string, string>(16); // Typical texture count
        _joints = new Dictionary<string, EggJoint>(32); // Typical joint count
        ParseAllTexturesAndVertices(lines, vertexPool, texturePaths);
        DebugLogger.LogEggImporter($"Parsed {vertexPool.Count} vertices and {texturePaths.Count} textures");
        ParseAllJoints(lines);
        DebugLogger.LogEggImporter($"Parsed {_joints.Count} joints, hasSkeletalData: {_hasSkeletalData}");
        PopulateJointWeightsFromVertices(vertexPool);
        // Calculate UV bounds for automatic texture scaling
        Vector4 uvBounds = _geometryProcessor.CalculateUVBounds(vertexPool);
        CreateMasterVertexBuffer(vertexPool);
        if (_hasSkeletalData && _rootJoint != null)
        {
            _rootBoneObject = new GameObject("Armature");
            _rootBoneObject.transform.SetParent(rootGO.transform, false);
            try
            {
                CreateBoneHierarchy(_rootBoneObject.transform, _rootJoint);
                DebugBoneHierarchy(_rootBoneObject.transform);
            }
            catch (System.Exception e)
            {
                DebugLogger.LogWarningEggImporter($"Failed to create bone hierarchy: {e.Message}. Falling back to static mesh.");
                _hasSkeletalData = false;
                if (_rootBoneObject != null)
                {
                    Object.DestroyImmediate(_rootBoneObject);
                    _rootBoneObject = null;
                }
            }
        }
        // --- Pass 2: Build Hierarchy and Map Geometry ---
        // Pre-size dictionaries based on typical EGG hierarchy complexity
        var geometryMap = new Dictionary<string, GeometryData>(64);
        var hierarchyMap = new Dictionary<string, Transform>(64);
        hierarchyMap[""] = rootGO.transform; // Root path
        BuildHierarchyAndMapGeometry(lines, 0, lines.Length, "", hierarchyMap, geometryMap);
        DebugLogger.LogEggImporter($"Built hierarchy with {hierarchyMap.Count} objects and {geometryMap.Count} geometry groups");
        
        // Collect all unique material names for multi-texture support
        var allMaterialNames = new HashSet<string>();
        foreach (var kvp in geometryMap)
        {
            DebugLogger.LogEggImporter($"Geometry group '{kvp.Key}' has {kvp.Value.subMeshes.Count} submeshes");
            foreach (string matName in kvp.Value.materialNames)
            {
                allMaterialNames.Add(matName);
            }
        }
        
        // SMART MATERIAL CREATION: Use multi-texture for DelFuego-pattern models, simple for others
        bool hasMultiTextureMaterials = allMaterialNames.Any(name => name.Contains("||"));
        
        if (hasMultiTextureMaterials)
        {
            DebugLogger.LogEggImporter("🔥 Detected multi-texture materials - using DelFuego overlay system");
            _materials = CreateMaterialsWithMultiTexture(texturePaths, allMaterialNames.ToList(), rootGO, Vector4.zero);
        }
        else
        {
            DebugLogger.LogEggImporter("📦 Using simple material creation");
            _materials = CreateMaterials(texturePaths, rootGO);
        }
        
        // Use optimized material dictionary creation from MaterialHandler
        _materialDict = _materialHandler.CreateMaterialDictionary(_materials);
        // --- Pass 3: Create Meshes from Mapped Geometry ---
        foreach (var kvp in geometryMap)
        {
            string path = kvp.Key;
            GeometryData geo = kvp.Value;
            if (hierarchyMap.TryGetValue(path, out Transform targetTransform))
            {
                CreateMeshForGameObject(targetTransform.gameObject, geo.subMeshes, geo.materialNames, ctx);
            }
        }
        // --- Pass 4: Parse and create animations ---
        ParseAnimations(lines, rootGO, ctx);
    }
    
    // Copy all the helper methods from reference implementation
    private void ParseAllTexturesAndVertices(string[] lines, List<EggVertex> vertexPool, Dictionary<string, string> texturePaths)
    {
        _geometryProcessor.ParseAllTexturesAndVertices(lines, vertexPool, texturePaths, _parserUtils);
    }

    private List<Material> CreateMaterials(Dictionary<string, string> texturePaths, GameObject rootGO)
    {
        return _materialHandler.CreateMaterialsWithMultiTexture(texturePaths, new List<string>(), rootGO, Vector4.zero);
    }
    
    private List<Material> CreateMaterialsWithMultiTexture(Dictionary<string, string> texturePaths, List<string> materialNames, GameObject rootGO, Vector4 uvBounds)
    {
        return _materialHandler.CreateMaterialsWithMultiTexture(texturePaths, materialNames, rootGO, uvBounds);
    }

    private void CreateMasterVertexBuffer(List<EggVertex> vertexPool)
    {
        _geometryProcessor.CreateMasterVertexBuffer(vertexPool, out _masterVertices, out _masterNormals, out _masterUVs, out _masterColors);
    }

    private void CreateBoneHierarchy(Transform parent, EggJoint joint)
    {
        _geometryProcessor.CreateBoneHierarchy(parent, joint);
    }

    private void BuildHierarchyAndMapGeometry(string[] lines, int start, int end, string currentPath, Dictionary<string, Transform> hierarchyMap, Dictionary<string, GeometryData> geometryMap)
    {
        _geometryProcessor.BuildHierarchyAndMapGeometry(lines, start, end, currentPath, hierarchyMap, geometryMap);
    }

    private void CreateMeshForGameObject(GameObject go, Dictionary<string, List<int>> subMeshes, List<string> materialNames, UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        _geometryProcessor.CreateMeshForGameObject(go, subMeshes, materialNames, ctx,
            _masterVertices, _masterNormals, _masterUVs, _masterColors, _materialDict,
            _hasSkeletalData, _rootJoint, _rootBoneObject, _joints);
    }

    private void ParseAnimations(string[] lines, GameObject rootGO, UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        _animationProcessor.ParseAnimations(lines, rootGO, ctx, _rootBoneObject);
    }

    private void DebugBoneHierarchy(Transform bone, string indent = "")
    {
        DebugLogger.LogEggImporter($"{indent}Bone: {bone.name}");
        for (int i = 0; i < bone.childCount; i++)
        {
            DebugBoneHierarchy(bone.GetChild(i), indent + "  ");
        }
    }

    private void ParseAllJoints(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Joint>"))
            {
                var joint = _geometryProcessor.ParseJoint(lines, ref i, _joints, _parserUtils);
                if (joint != null)
                {
                    _joints[joint.name] = joint;
                    if (joint.parent == null) _rootJoint = joint;
                    _hasSkeletalData = true;
                }
            }
        }
    }

    private void PopulateJointWeightsFromVertices(List<EggVertex> vertexPool)
    {
        for (int i = 0; i < vertexPool.Count; i++)
        {
            var vertex = vertexPool[i];
            foreach (var kvp in vertex.boneWeights)
            {
                if (_joints.TryGetValue(kvp.Key, out EggJoint joint))
                {
                    joint.vertexWeights[i] = kvp.Value;
                }
            }
        }
    }
    
    /// <summary>
    /// Gets the materials created during import for adding to AssetImportContext
    /// </summary>
    public List<Material> GetMaterials()
    {
        return _materials;
    }
    
    /// <summary>
    /// Detects if an EGG file contains multi-texture patterns that require specialized processing
    /// </summary>
    public static bool RequiresMultiTextureProcessing(string eggFilePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(eggFilePath);
            
            // Look for specific multi-texture indicator texture
            foreach (string line in lines)
            {
                if (line.Contains("pir_t_are_isl_multi_"))
                {
                    DebugLogger.LogEggImporter($"🔥 Multi-texture model detected (pir_t_are_isl_multi_): {Path.GetFileName(eggFilePath)}");
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            DebugLogger.LogWarningEggImporter($"Error checking multi-texture requirements: {e.Message}");
        }
        
        return false;
    }
}
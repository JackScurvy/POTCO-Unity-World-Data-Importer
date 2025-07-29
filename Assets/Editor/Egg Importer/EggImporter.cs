using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

[ScriptedImporter(1, "egg")]
public class EggImporter : ScriptedImporter
{
    // Cache commonly used separators to avoid repeated allocations
    private static readonly char[] SpaceSeparator = { ' ' };
    private static readonly char[] SpaceNewlineCarriageReturnSeparators = { ' ', '\n', '\r' };
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

    private AnimationProcessor _animationProcessor;
    private GeometryProcessor _geometryProcessor;
    private MaterialHandler _materialHandler;
    private ParserUtilities _parserUtils;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        Debug.Log("--- EGG IMPORTER: START ---");

        // Initialize processors
        _animationProcessor = new AnimationProcessor();
        _geometryProcessor = new GeometryProcessor();
        _materialHandler = new MaterialHandler();
        _parserUtils = new ParserUtilities();

        var rootGO = new GameObject(Path.GetFileNameWithoutExtension(ctx.assetPath));
        var lines = File.ReadAllLines(ctx.assetPath);

        bool isAnimationOnly = IsAnimationOnlyFile(lines);
        Debug.Log($"Animation-only file: {isAnimationOnly}");

        if (isAnimationOnly)
        {
            HandleAnimationOnlyFile(lines, rootGO, ctx);
        }
        else
        {
            HandleGeometryFile(lines, rootGO, ctx);
        }

        ctx.AddObjectToAsset("main", rootGO);
        ctx.SetMainObject(rootGO);

        // Add materials to context - optimized with null check
        if (_materials?.Count > 0)
        {
            foreach (var material in _materials)
            {
                ctx.AddObjectToAsset(material.name, material);
            }
        }

        Debug.Log("--- EGG IMPORTER: COMPLETE ---");
    }

    private bool IsAnimationOnlyFile(string[] lines)
    {
        bool hasBundle = false;
        bool hasVertices = false;
        bool hasPolygons = false;

        // Early termination optimization - stop when we have enough info
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Bundle>")) hasBundle = true;
            else if (line.StartsWith("<Vertex>")) hasVertices = true;
            else if (line.StartsWith("<Polygon>")) hasPolygons = true;
            
            // Early exit if we already know it's not animation-only
            if (hasVertices || hasPolygons)
            {
                Debug.Log($"File analysis - Bundle: {hasBundle}, Vertices: {hasVertices}, Polygons: {hasPolygons} (early exit)");
                return false;
            }
        }

        Debug.Log($"File analysis - Bundle: {hasBundle}, Vertices: {hasVertices}, Polygons: {hasPolygons}");
        return hasBundle && !hasVertices && !hasPolygons;
    }

    private void HandleAnimationOnlyFile(string[] lines, GameObject rootGO, AssetImportContext ctx)
    {
        Debug.Log("🎯 COMBINED: Processing animation-only file");

        // Pre-size joints dictionary based on typical animation file sizes
        _joints = new Dictionary<string, EggJoint>(32);
        GameObject armature = new GameObject("Armature");
        armature.transform.SetParent(rootGO.transform, false);
        _rootBoneObject = armature;

        ParseBoneHierarchyAndAnimations(lines, rootGO, ctx);

        if (_rootJoint != null)
        {
            Debug.Log("🎯 COMBINED: Creating bone hierarchy from parsed data");
            _geometryProcessor.CreateBoneHierarchy(armature.transform, _rootJoint);
        }
        else if (_joints.Count > 0)
        {
            Debug.Log("🎯 COMBINED: Creating bone hierarchy from joint dictionary");
            _geometryProcessor.CreateBoneHierarchyFromTables(armature.transform, _joints);
        }

        Debug.Log("🎯 COMBINED: Animation-only processing complete");
    }

    private void ParseBoneHierarchyAndAnimations(string[] lines, GameObject rootGO, AssetImportContext ctx)
    {
        Debug.Log("🎯 COMBINED: Parsing bone hierarchy AND animations in single pass");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            if (line.StartsWith("<Bundle>"))
            {
                var parts = line.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string bundleName = parts[1];
                    Debug.Log($"🎯 COMBINED: Found bundle '{bundleName}'");

                    var clip = new AnimationClip { name = bundleName + "_anim" };
                    clip.legacy = true;
                    clip.wrapMode = WrapMode.Loop;

                    int bundleEnd = _parserUtils.FindMatchingBrace(lines, i);
                    if (bundleEnd != -1)
                    {
                        _animationProcessor.ParseBundleBonesAndAnimations(lines, i + 1, bundleEnd, _rootJoint, "", clip, _joints);

                        if (_joints.Count > 0 && _rootJoint == null)
                        {
                            foreach (var joint in _joints.Values.Where(j => j.parent == null))
                            {
                                _rootJoint = joint;
                                break;
                            }
                        }

                        var curveBindings = AnimationUtility.GetCurveBindings(clip);
                        if (curveBindings.Length > 0)
                        {
                            Debug.Log($"🎯 COMBINED: Animation clip has {curveBindings.Length} curves");
                            ctx.AddObjectToAsset(clip.name, clip);

                            var animComponent = rootGO.GetComponent<Animation>();
                            if (animComponent == null)
                            {
                                animComponent = rootGO.AddComponent<Animation>();
                            }
                            animComponent.AddClip(clip, clip.name);
                            animComponent.clip = clip;
                            animComponent.playAutomatically = true;
                        }

                        i = bundleEnd;
                    }
                }
            }
        }

        Debug.Log($"🎯 COMBINED: Parsing complete. Found {_joints.Count} joints");
    }

    private void HandleGeometryFile(string[] lines, GameObject rootGO, AssetImportContext ctx)
    {
        Debug.Log("Processing geometry EGG file");
        // --- Pass 1: Parse all raw data into memory ---
        // Pre-size collections based on typical EGG file contents
        var vertexPool = new List<EggVertex>(1024); // Typical vertex count estimate
        var texturePaths = new Dictionary<string, string>(16); // Typical texture count
        _joints = new Dictionary<string, EggJoint>(32); // Typical joint count
        ParseAllTexturesAndVertices(lines, vertexPool, texturePaths);
        Debug.Log($"Parsed {vertexPool.Count} vertices and {texturePaths.Count} textures");
        ParseAllJoints(lines);
        Debug.Log($"Parsed {_joints.Count} joints, hasSkeletalData: {_hasSkeletalData}");
        PopulateJointWeightsFromVertices(vertexPool);
        _materials = CreateMaterials(texturePaths, rootGO);
        // Use optimized material dictionary creation from MaterialHandler
        _materialDict = _materialHandler.CreateMaterialDictionary(_materials);
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
                Debug.LogWarning($"Failed to create bone hierarchy: {e.Message}. Falling back to static mesh.");
                _hasSkeletalData = false;
                if (_rootBoneObject != null)
                {
                    DestroyImmediate(_rootBoneObject);
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
        Debug.Log($"Built hierarchy with {hierarchyMap.Count} objects and {geometryMap.Count} geometry groups");
        foreach (var kvp in geometryMap)
        {
            Debug.Log($"Geometry group '{kvp.Key}' has {kvp.Value.subMeshes.Count} submeshes");
        }
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

    private void ParseAllTexturesAndVertices(string[] lines, List<EggVertex> vertexPool, Dictionary<string, string> texturePaths)
    {
        _geometryProcessor.ParseAllTexturesAndVertices(lines, vertexPool, texturePaths, _parserUtils);
    }

    private List<Material> CreateMaterials(Dictionary<string, string> texturePaths, GameObject rootGO)
    {
        return _materialHandler.CreateMaterials(texturePaths, rootGO);
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

    private void CreateMeshForGameObject(GameObject go, Dictionary<string, List<int>> subMeshes, List<string> materialNames, AssetImportContext ctx)
    {
        _geometryProcessor.CreateMeshForGameObject(go, subMeshes, materialNames, ctx, 
            _masterVertices, _masterNormals, _masterUVs, _masterColors, _materialDict,
            _hasSkeletalData, _rootJoint, _rootBoneObject, _joints);
    }

    private void ParseAnimations(string[] lines, GameObject rootGO, AssetImportContext ctx)
    {
        _animationProcessor.ParseAnimations(lines, rootGO, ctx, _rootBoneObject);
    }


    private void DebugBoneHierarchy(Transform bone, string indent = "")
    {
        Debug.Log($"{indent}Bone: {bone.name} - Pos: {bone.localPosition}, Rot: {bone.localRotation.eulerAngles}, Scale: {bone.localScale}");
        for (int i = 0; i < bone.childCount; i++)
        {
            DebugBoneHierarchy(bone.GetChild(i), indent + "  ");
        }
    }

    private void ParseAllJoints(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().StartsWith("<Joint>"))
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

    private void ParseVertexRef(string fullBlock, EggJoint joint)
    {
        int openBrace = fullBlock.IndexOf('{');
        int closeBrace = fullBlock.LastIndexOf('}');
        if (openBrace == -1 || closeBrace == -1) return;
        string content = fullBlock.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
        var parts = content.Split(SpaceNewlineCarriageReturnSeparators, StringSplitOptions.RemoveEmptyEntries);
        float membership = 1.0f;
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            if (part.StartsWith("<Scalar>") && i + 2 < parts.Length && parts[i + 1] == "membership")
            {
                if (float.TryParse(parts[i + 2].TrimEnd('}'), NumberStyles.Float, CultureInfo.InvariantCulture, out float mem)) { membership = mem; }
                i += 2;
            }
            else if (part == "<Ref>") { break; }
            else if (int.TryParse(part, out int vertexIndex)) { joint.vertexWeights[vertexIndex] = membership; }
        }
    }

    private void PopulateJointWeightsFromVertices(List<EggVertex> vertexPool)
    {
        for (int i = 0; i < vertexPool.Count; i++)
        {
            var vert = vertexPool[i];
            foreach (var kvp in vert.boneWeights)
            {
                if (_joints.TryGetValue(kvp.Key, out EggJoint joint))
                {
                    joint.vertexWeights[i] = kvp.Value;
                }
            }
        }
    }
}
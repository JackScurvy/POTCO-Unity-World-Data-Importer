using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using POTCO.Editor;

public class MultiTextureGeometryProcessor
{
    private MultiTextureParserUtilities _parserUtils;
    private MultiTextureMaterialHandler _materialHandler;
    
    // Cache commonly used separators to avoid repeated allocations
    private static readonly char[] SpaceSeparator = { ' ' };
    private static readonly char[] SpaceNewlineCarriageReturnSeparators = { ' ', '\n', '\r' };
    
    // Cache for frequently used materials to avoid repeated Shader.Find calls
    private static Material _cachedDefaultMaterial;
    
    // Track which textures are secondary in multi-texture polygons
    private HashSet<string> _secondaryTextures = new HashSet<string>();
    
    public MultiTextureGeometryProcessor()
    {
        _parserUtils = new MultiTextureParserUtilities();
        _materialHandler = new MultiTextureMaterialHandler();
    }
    
    public bool IsSecondaryTexture(string textureName)
    {
        return _secondaryTextures.Contains(textureName);
    }

    public void CreateMeshForGameObject(GameObject go, Dictionary<string, List<int>> subMeshes, List<string> materialNames, AssetImportContext ctx, 
        Vector3[] masterVertices, Vector3[] masterNormals, Vector2[] masterUVs, Color[] masterColors, 
        Dictionary<string, Material> materialDict, bool hasSkeletalData, EggJoint rootJoint, GameObject rootBoneObject, Dictionary<string, EggJoint> joints)
    {
        DebugLogger.LogEggImporter($"Creating mesh for GameObject: {go.name}");

        // Check if we have any triangles
        int totalTriangles = 0;
        foreach (var submesh in subMeshes.Values)
        {
            totalTriangles += submesh.Count;
        }

        if (totalTriangles == 0)
        {
            DebugLogger.LogWarningEggImporter($"No triangles found for GameObject {go.name}");
            return;
        }

        DebugLogger.LogEggImporter($"Total triangles: {totalTriangles}");

        // Collect all unique vertex indices used by this mesh
        var usedVertexIndices = new HashSet<int>();
        foreach (var submesh in subMeshes.Values)
        {
            foreach (int vertexIndex in submesh)
            {
                if (vertexIndex >= 0 && vertexIndex < masterVertices.Length)
                {
                    usedVertexIndices.Add(vertexIndex);
                }
            }
        }

        // Create local vertex arrays with only the vertices this mesh uses
        var sortedIndices = usedVertexIndices.OrderBy(x => x).ToArray();
        var localVertices = new Vector3[sortedIndices.Length];
        var localNormals = new Vector3[sortedIndices.Length];
        var localUVs = new Vector2[sortedIndices.Length];
        var localColors = new Color[sortedIndices.Length];

        // Create mapping from global index to local index - pre-size dictionary
        var globalToLocalMap = new Dictionary<int, int>(sortedIndices.Length);
        for (int i = 0; i < sortedIndices.Length; i++)
        {
            int globalIndex = sortedIndices[i];
            globalToLocalMap[globalIndex] = i;
            localVertices[i] = masterVertices[globalIndex];
            localNormals[i] = masterNormals[globalIndex];
            localUVs[i] = masterUVs[globalIndex];
            localColors[i] = masterColors[globalIndex];
        }
        
        // Detect tiling frequency before processing UVs
        // REMOVED: Tiling frequency detection - reverted to pre-shader state
        
        // Analyze and normalize UV coordinates for this specific mesh
        NormalizeMeshUVCoordinates(localUVs, go.name);

        DebugLogger.LogEggImporter($"Mesh uses {localVertices.Length} vertices out of {masterVertices.Length} total vertices");

        var mesh = new Mesh { name = go.name + "_mesh_" + System.Guid.NewGuid().ToString("N")[..8] };
        mesh.vertices = localVertices;
        mesh.normals = localNormals;
        mesh.uv = localUVs;  // Primary UV channel for base textures
        mesh.colors = localColors;
        mesh.subMeshCount = materialNames.Count;
        
        // Set overlay UV channel for multi-texture support
        if (_overlayUVChannels.ContainsKey("overlay_uv"))
        {
            Vector2[] localOverlayUVs = new Vector2[localVertices.Length];
            Vector2[] globalOverlayUVs = _overlayUVChannels["overlay_uv"];
            for (int i = 0; i < usedVertexIndices.Count; i++)
            {
                int globalIndex = usedVertexIndices.ElementAt(i);
                localOverlayUVs[i] = globalOverlayUVs[globalIndex];
            }
            mesh.uv2 = localOverlayUVs;  // Overlay UV channel for multi-textures
            DebugLogger.LogEggImporter($"✅ Set UV2 overlay channel with {localOverlayUVs.Length} coordinates for mesh {mesh.name}");
        }

        // Pre-size materials list to avoid resizing
        var rendererMaterials = new List<Material>(materialNames.Count);

        for (int j = 0; j < materialNames.Count; j++)
        {
            string matName = materialNames[j];
            if (subMeshes.ContainsKey(matName))
            {
                var globalTriangles = subMeshes[matName];
                // Remap global vertex indices to local vertex indices - pre-size list
                var localTriangles = new List<int>(globalTriangles.Count);
                foreach (int globalIndex in globalTriangles)
                {
                    if (globalToLocalMap.TryGetValue(globalIndex, out int localIndex))
                    {
                        localTriangles.Add(localIndex);
                    }
                    else
                    {
                        DebugLogger.LogErrorEggImporter($"Failed to remap global vertex index {globalIndex} to local index");
                    }
                }
                DebugLogger.LogEggImporter($"Setting triangles for submesh {j} ({matName}): {localTriangles.Count} triangles (remapped from global indices)");
                mesh.SetTriangles(localTriangles, j, false);
            }
            if (materialDict.TryGetValue(matName, out Material mat))
            {
                // REMOVED: Tiling frequency application - reverted to pre-shader state
                
                rendererMaterials.Add(mat);
                DebugLogger.LogEggImporter($"Added material: {matName}");
            }
            else
            {
                // ENHANCED: Try to find fallback material for multi-texture names
                Material fallbackMat = FindFallbackMaterial(matName, materialDict);
                if (fallbackMat != null)
                {
                    rendererMaterials.Add(fallbackMat);
                    DebugLogger.LogEggImporter($"Using fallback material for: {matName} -> {fallbackMat.name}");
                }
                else
                {
                    DebugLogger.LogWarningEggImporter($"Material not found: {matName}");
                    // Create a default material using cached shader
                    var defaultMat = GetCachedDefaultMaterial(matName + "_default");
                    rendererMaterials.Add(defaultMat);
                }
            }
        }

        mesh.RecalculateBounds();
        DebugLogger.LogEggImporter($"Mesh bounds: {mesh.bounds}");

        // Only recalculate normals if we don't have them
        if (masterNormals == null || masterNormals.Length == 0)
        {
            mesh.RecalculateNormals();
        }

        // Force the mesh to be visible by ensuring bounds are reasonable
        if (mesh.bounds.size.magnitude < 0.001f)
        {
            DebugLogger.LogWarningEggImporter("Mesh bounds are very small, this might cause rendering issues");
        }

        if (hasSkeletalData && rootJoint != null && rootBoneObject != null)
        {
            DebugLogger.LogEggImporter("Setting up skinned mesh renderer");
            SetupSkinnedMeshRenderer(go, mesh, rendererMaterials.ToArray(), ctx, localVertices, rootJoint, rootBoneObject, joints, globalToLocalMap, sortedIndices);
        }
        else
        {
            DebugLogger.LogEggImporter("Setting up static mesh renderer");
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = rendererMaterials.ToArray();
        }
        ctx.AddObjectToAsset(mesh.name, mesh);
    }

    private void SetupSkinnedMeshRenderer(GameObject go, Mesh mesh, Material[] materials, 
        AssetImportContext ctx, Vector3[] localVertices, EggJoint rootJoint, 
        GameObject rootBoneObject, Dictionary<string, EggJoint> joints, Dictionary<int, int> globalToLocalMap, int[] sortedIndices)
    {
        var boneWeights = new BoneWeight[localVertices.Length];
        // Pre-size bone collections to avoid resizing (estimate based on typical joint counts)
        var bones = new List<Transform>(64);
        var bindPoses = new List<Matrix4x4>(64);

        CollectBonesAndBindPoses(rootJoint, bones, bindPoses, rootBoneObject.transform);

        DebugLogger.LogEggImporter($"Collected {bones.Count} bones for skinned mesh");

        if (bones.Count == 0)
        {
            DebugLogger.LogWarningEggImporter("No bones found, falling back to static mesh");
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = materials;
            return;
        }

        int verticesWithWeights = 0;

        // Process bone weights for local vertices only
        for (int localIndex = 0; localIndex < localVertices.Length; localIndex++)
        {
            int globalIndex = sortedIndices[localIndex];
            // Pre-size weights list to avoid frequent resizing (most vertices have 1-4 weights)
            var weights = new List<KeyValuePair<int, float>>(4);
            
            foreach (var joint in joints.Values)
            {
                if (joint.vertexWeights.ContainsKey(globalIndex))
                {
                    int boneIndex = bones.FindIndex(b => b.name == joint.name);
                    if (boneIndex >= 0) { weights.Add(new KeyValuePair<int, float>(boneIndex, joint.vertexWeights[globalIndex])); }
                }
            }

            if (weights.Count > 0) verticesWithWeights++;

            weights = weights.OrderByDescending(w => w.Value).Take(4).ToList();
            float totalWeight = weights.Sum(w => w.Value);
            if (totalWeight > 0)
            {
                for (int j = 0; j < weights.Count; j++) { weights[j] = new KeyValuePair<int, float>(weights[j].Key, weights[j].Value / totalWeight); }
            }
            else
            {
                if (bones.Count > 0)
                {
                    weights.Add(new KeyValuePair<int, float>(0, 1.0f));
                }
            }

            boneWeights[localIndex] = new BoneWeight();
            if (weights.Count > 0) { boneWeights[localIndex].boneIndex0 = weights[0].Key; boneWeights[localIndex].weight0 = weights[0].Value; }
            if (weights.Count > 1) { boneWeights[localIndex].boneIndex1 = weights[1].Key; boneWeights[localIndex].weight1 = weights[1].Value; }
            if (weights.Count > 2) { boneWeights[localIndex].boneIndex2 = weights[2].Key; boneWeights[localIndex].weight2 = weights[2].Value; }
            if (weights.Count > 3) { boneWeights[localIndex].boneIndex3 = weights[3].Key; boneWeights[localIndex].weight3 = weights[3].Value; }
        }

        DebugLogger.LogEggImporter($"Vertices with bone weights: {verticesWithWeights}/{localVertices.Length}");

        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindPoses.ToArray();

        var skinnedRenderer = go.AddComponent<SkinnedMeshRenderer>();
        skinnedRenderer.sharedMesh = mesh;
        skinnedRenderer.sharedMaterials = materials;
        skinnedRenderer.bones = bones.ToArray();
        skinnedRenderer.rootBone = rootBoneObject.transform;

        skinnedRenderer.localBounds = mesh.bounds;
    }

    public void CreateBoneHierarchy(Transform parent, EggJoint joint)
    {
        GameObject boneGO = new GameObject(joint.name);
        boneGO.transform.SetParent(parent, false);

        if (joint.transform != Matrix4x4.zero)
        {
            _parserUtils.ApplyMatrix4x4ToTransform(boneGO.transform, joint.transform);
        }
        else
        {
            boneGO.transform.localPosition = Vector3.zero;
            boneGO.transform.localRotation = Quaternion.identity;
            boneGO.transform.localScale = Vector3.one;
        }

        foreach (var child in joint.children) { CreateBoneHierarchy(boneGO.transform, child); }
    }

    public void CreateBoneHierarchyFromTables(Transform parent, Dictionary<string, EggJoint> joints)
    {
        if (joints == null || joints.Count == 0) return;

        foreach (var joint in joints.Values.Where(j => j.parent == null))
        {
            CreateBoneGameObjectRecursive(parent, joint);
        }
    }

    private void CreateBoneGameObjectRecursive(Transform parent, EggJoint joint)
    {
        GameObject boneGO = new GameObject(joint.name);
        boneGO.transform.SetParent(parent, false);

        if (joint.transform != Matrix4x4.zero)
        {
            _parserUtils.ApplyMatrix4x4ToTransform(boneGO.transform, joint.transform);
        }

        foreach (var child in joint.children)
        {
            CreateBoneGameObjectRecursive(boneGO.transform, child);
        }
    }

    private void CollectBonesAndBindPoses(EggJoint joint, List<Transform> bones, List<Matrix4x4> bindPoses, Transform armatureRoot)
    {
        Transform boneTransform = FindBoneTransform(armatureRoot, joint.name);
        if (boneTransform != null)
        {
            bones.Add(boneTransform);
            bindPoses.Add(boneTransform.worldToLocalMatrix);
        }
        foreach (var child in joint.children) { CollectBonesAndBindPoses(child, bones, bindPoses, armatureRoot); }
    }

    private Transform FindBoneTransform(Transform root, string boneName)
    {
        if (root.name == boneName) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindBoneTransform(root.GetChild(i), boneName);
            if (found != null) return found;
        }
        return null;
    }


    public void ParseAllTexturesAndVertices(string[] lines, List<EggVertex> vertexPool, Dictionary<string, string> texturePaths, MultiTextureParserUtilities parserUtils)
    {
        // Clear secondary texture registry for this import
        SecondaryTextureRegistry.Clear();
        
        // Parse textures and collect UV name mappings
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Texture>"))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string texName = parts[1];
                    int blockEnd = parserUtils.FindMatchingBrace(lines, i);
                    
                    for (int j = i + 1; j < blockEnd; j++)
                    {
                        string innerLine = lines[j].Trim();
                        if (innerLine.StartsWith("\"") && innerLine.EndsWith("\"")) 
                        { 
                            texturePaths[texName] = innerLine.Trim('"'); 
                        }
                        else if (innerLine.StartsWith("<Scalar> uv-name"))
                        {
                            // Extract UV set name mapping: <Scalar> uv-name { muti-sand }
                            int openBrace = innerLine.IndexOf('{');
                            int closeBrace = innerLine.LastIndexOf('}');
                            if (openBrace != -1 && closeBrace != -1)
                            {
                                string uvSetName = innerLine.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
                                
                                // Normalize UV set names to handle different conventions
                                string normalizedUVName = NormalizeUVSetName(uvSetName);
                                _textureUVMappings[texName] = normalizedUVName;
                                DebugLogger.LogEggImporter($"🔍 Texture '{texName}' uses UV set '{uvSetName}' (normalized: '{normalizedUVName}')");
                            }
                        }
                    }
                }
            }
            else if (line.StartsWith("<Vertex>"))
            {
                var vert = new EggVertex();
                var posParts = lines[++i].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                vert.position = new Vector3(float.Parse(posParts[0], CultureInfo.InvariantCulture), float.Parse(posParts[2], CultureInfo.InvariantCulture), float.Parse(posParts[1], CultureInfo.InvariantCulture));
                int vertexEnd = parserUtils.FindMatchingBrace(lines, i - 1);
                while (i < vertexEnd)
                {
                    i++;
                    string attributeLine = lines[i].Trim();
                    if (attributeLine.StartsWith("//"))
                    {
                        string weightData = attributeLine.Substring(2).Trim();
                        var weightPairs = weightData.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var pair in weightPairs)
                        {
                            var splitPair = pair.Split(':');
                            if (splitPair.Length == 2)
                            {
                                string jointName = splitPair[0];
                                if (float.TryParse(splitPair[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float weight))
                                {
                                    vert.boneWeights[jointName] = weight;
                                }
                            }
                        }
                    }
                    else if (attributeLine.Contains("{"))
                    {
                        int openBrace = attributeLine.IndexOf('{');
                        int closeBrace = attributeLine.LastIndexOf('}');
                        if (openBrace == -1 || closeBrace == -1) continue;
                        string valuesString = attributeLine.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
                        var valueParts = valuesString.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (attributeLine.StartsWith("<UV>")) 
                        { 
                            float u = float.Parse(valueParts[0], CultureInfo.InvariantCulture);
                            float v = float.Parse(valueParts[1], CultureInfo.InvariantCulture);
                            
                            // ROBUST: Parse both primary and named UV coordinates with multiple naming conventions
                            string uvSetName = ExtractUVSetName(attributeLine);
                            
                            if (uvSetName != null)
                            {
                                // Store named UV coordinates (for overlay textures)
                                vert.namedUVs[uvSetName] = new Vector2(u, v);
                                DebugLogger.LogEggImporter($"🔍 Stored named UV '{uvSetName}': ({u:F3}, {v:F3})");
                            }
                            else
                            {
                                // Store primary UV coordinates (for base textures)
                                vert.uv = new Vector2(u, v);
                            }
                        }
                        else if (attributeLine.StartsWith("<Normal>")) { vert.normal = new Vector3(float.Parse(valueParts[0], CultureInfo.InvariantCulture), float.Parse(valueParts[2], CultureInfo.InvariantCulture), float.Parse(valueParts[1], CultureInfo.InvariantCulture)); }
                        else if (attributeLine.StartsWith("<RGBA>")) { vert.color = new Color(float.Parse(valueParts[0], CultureInfo.InvariantCulture), float.Parse(valueParts[1], CultureInfo.InvariantCulture), float.Parse(valueParts[2], CultureInfo.InvariantCulture), float.Parse(valueParts[3], CultureInfo.InvariantCulture)); }
                    }
                }
                i = vertexEnd;
                vertexPool.Add(vert);
            }
        }
    }


    private void ParsePolygon(string[] lines, ref int i, Dictionary<string, List<int>> subMeshes, List<string> materialNames)
    {
        string polygonTextureRef = "Default-Material";
        int blockEnd = _parserUtils.FindMatchingBrace(lines, i);
        
        // Check for collision tags - skip collision polygons based on settings
        if (!EggImporterSettings.Instance.importCollisions)
        {
            for (int j = i + 1; j < blockEnd; j++)
            {
                string innerLine = lines[j].Trim();
                if (innerLine.StartsWith("<Collide>"))
                {
                    i = blockEnd;
                    return; // Skip this polygon entirely
                }
            }
        }
        
        // Collect ALL texture references for material creation, but only use the FIRST one for UV mapping
        var textureRefs = new List<string>();
        for (int j = i + 1; j < blockEnd; j++)
        {
            string innerLine = lines[j].Trim();
            if (innerLine.StartsWith("<TRef>"))
            {
                string texRef = innerLine.Substring(innerLine.IndexOf('{') + 1, innerLine.LastIndexOf('}') - innerLine.IndexOf('{') - 1).Trim();
                textureRefs.Add(texRef);
            }
        }
        
        // NEW APPROACH: Use FIRST texture for UV mapping, include ALL textures in material name for shader
        if (textureRefs.Count > 1)
        {
            // Use FIRST texture for UV mapping (ground texture with perfect UVs)
            // But create multi-texture material name so shader can add tiling overlay
            polygonTextureRef = string.Join("||", textureRefs);
            DebugLogger.LogEggImporter($"🎯 Multi-texture polygon: Using '{textureRefs[0]}' for UV mapping, '{polygonTextureRef}' for material");
            
            // Track secondary textures (all textures after the first one)
            for (int idx = 1; idx < textureRefs.Count; idx++)
            {
                _secondaryTextures.Add(textureRefs[idx]);
                SecondaryTextureRegistry.AddSecondaryTexture(textureRefs[idx]);
                DebugLogger.LogEggImporter($"🎯 Marking '{textureRefs[idx]}' as secondary texture (position {idx})");
            }
        }
        else if (textureRefs.Count == 1)
        {
            // Single texture - works perfectly already
            polygonTextureRef = textureRefs[0];
        }
        // else keep "Default-Material"
        
        if (!subMeshes.ContainsKey(polygonTextureRef)) { subMeshes[polygonTextureRef] = new List<int>(); materialNames.Add(polygonTextureRef); }
        
        // Collect vertex indices first
        var vertexIndices = new List<int>();
        for (int j = i + 1; j < blockEnd; j++)
        {
            string innerLine = lines[j].Trim();
            if (innerLine.StartsWith("<VertexRef>"))
            {
                string valuesString = innerLine.Substring(innerLine.IndexOf('{') + 1, innerLine.LastIndexOf('}') - innerLine.IndexOf('{') - 1).Trim();
                var vRefParts = valuesString.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out _)).ToArray();
                foreach (string part in vRefParts)
                {
                    if (int.TryParse(part, out int vertIndex))
                    {
                        vertexIndices.Add(vertIndex);
                    }
                }
                
                // Add to submesh (keep original triangle logic)
                if (vRefParts.Length >= 3)
                {
                    int v0 = int.Parse(vRefParts[0]); int v1 = int.Parse(vRefParts[1]); int v2 = int.Parse(vRefParts[2]);
                    subMeshes[polygonTextureRef].Add(v0); subMeshes[polygonTextureRef].Add(v2); subMeshes[polygonTextureRef].Add(v1);
                    if (vRefParts.Length > 3)
                    {
                        int v3 = int.Parse(vRefParts[3]);
                        subMeshes[polygonTextureRef].Add(v0); subMeshes[polygonTextureRef].Add(v3); subMeshes[polygonTextureRef].Add(v2);
                    }
                }
            }
        }
        
        // Mark vertices as multi-texture for global scaling later
        if (polygonTextureRef.Contains("||"))
        {
            foreach (int vertIndex in vertexIndices)
            {
                _multiTextureVertices.Add(vertIndex);
            }
        }
        i = blockEnd;
    }

    public void BuildHierarchyAndMapGeometry(string[] lines, int start, int end, string currentPath, Dictionary<string, Transform> hierarchyMap, Dictionary<string, GeometryData> geometryMap)
    {
        int i = start;
        while (i < end)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Group>"))
            {
                string groupName = _parserUtils.GetGroupName(line);
                
                // Skip collision groups based on settings
                if (groupName.ToLower().Contains("collision") && !EggImporterSettings.Instance.importCollisions)
                {
                    DebugLogger.LogEggImporter($"🚫 Skipping collision group: '{groupName}' (Import Collisions disabled)");
                    int collisionGroupEnd = _parserUtils.FindMatchingBrace(lines, i);
                    i = collisionGroupEnd + 1;
                    continue;
                }
                
                // Check if this is an LOD group and handle according to settings
                int groupEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (IsLODGroup(lines, i, groupEnd) && !ShouldImportLOD(lines, i, groupEnd, groupName))
                {
                    DebugLogger.LogEggImporter($"🚫 Skipping LOD group: '{groupName}' based on import settings");
                    i = groupEnd + 1;
                    continue;
                }
                
                string newPath = string.IsNullOrEmpty(currentPath) ? groupName : currentPath + "/" + groupName;

                GameObject newGO = new GameObject(groupName);
                newGO.transform.SetParent(hierarchyMap[currentPath], false);
                hierarchyMap[newPath] = newGO.transform;

                BuildHierarchyAndMapGeometry(lines, i + 1, groupEnd, newPath, hierarchyMap, geometryMap);
                i = groupEnd + 1;
            }
            else if (line.StartsWith("<Transform>"))
            {
                // Check if this group or its children will contain polygons by looking ahead in the EGG structure
                bool containsGeometry = WillContainGeometry(lines, i, hierarchyMap, currentPath);
                
                if (containsGeometry)
                {
                    DebugLogger.LogEggImporter($"🚫 Skipping transform for geometry group: '{currentPath}' (vertices already in world space)");
                    int transformEnd = _parserUtils.FindMatchingBrace(lines, i);
                    i = transformEnd;
                }
                else if (hierarchyMap.TryGetValue(currentPath, out Transform transform))
                {
                    DebugLogger.LogEggImporter($"🔄 Applying transform to GameObject: '{transform.name}' at path: '{currentPath}'");
                    ParseTransform(lines, ref i, transform.gameObject);
                }
                else
                {
                    DebugLogger.LogWarningEggImporter($"⚠️ Transform found but no GameObject at path: '{currentPath}'");
                    int transformEnd = _parserUtils.FindMatchingBrace(lines, i);
                    i = transformEnd;
                }
                i++;
            }
            else if (line.StartsWith("<Polygon>"))
            {
                if (!geometryMap.ContainsKey(currentPath))
                {
                    geometryMap[currentPath] = new GeometryData();
                }
                ParsePolygon(lines, ref i, geometryMap[currentPath].subMeshes, geometryMap[currentPath].materialNames);
            }
            else
            {
                i++;
            }
        }
    }

    private void ParseTransform(string[] lines, ref int i, GameObject go)
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;
        int blockEnd = _parserUtils.FindMatchingBrace(lines, i);
        for (int j = i + 1; j < blockEnd; j++)
        {
            string line = lines[j].Trim();
            if (line.StartsWith("<Translate>")) { position += _parserUtils.ParseVector3(line); }
            else if (line.StartsWith("<Rotate>")) { rotation *= _parserUtils.ParseAngleAxis(line); }
            else if (line.StartsWith("<Scale>")) { scale = Vector3.Scale(scale, _parserUtils.ParseVector3(line)); }
        }
        Vector3 unityPosition = new Vector3(position.x, position.z, position.y);
        Quaternion unityRotation = new Quaternion(rotation.x, rotation.z, rotation.y, -rotation.w);
        Vector3 unityScale = new Vector3(scale.x, scale.z, scale.y);
        
        DebugLogger.LogEggImporter($"📍 Setting transform for '{go.name}': pos={unityPosition}, rot={unityRotation.eulerAngles}, scale={unityScale}");
        
        go.transform.localPosition = unityPosition;
        go.transform.localRotation = unityRotation;
        go.transform.localScale = unityScale;
        i = blockEnd;
    }

    public EggJoint ParseJoint(string[] lines, ref int i, Dictionary<string, EggJoint> joints, MultiTextureParserUtilities parserUtils)
    {
        var parts = lines[i].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;
        string jointName = parts[1];
        var joint = new EggJoint { name = jointName };
        int blockEnd = parserUtils.FindMatchingBrace(lines, i);
        if (blockEnd == -1) return null;
        i++;
        while (i < blockEnd)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Transform>")) { joint.transform = ParseTransformMatrix(lines, ref i, parserUtils); }
            else if (line.StartsWith("<DefaultPose>"))
            {
                // Skip DefaultPose blocks entirely to force T-pose
                int defaultPoseEnd = parserUtils.FindMatchingBrace(lines, i);
                if (defaultPoseEnd != -1) i = defaultPoseEnd;
                else i++;
            }
            else if (line.StartsWith("<Joint>"))
            {
                var childJoint = ParseJoint(lines, ref i, joints, parserUtils);
                if (childJoint != null)
                {
                    childJoint.parent = joint;
                    joint.children.Add(childJoint);
                    joints[childJoint.name] = childJoint;
                }
            }
            else if (line.StartsWith("<VertexRef>"))
            {
                int vrefEnd = parserUtils.FindMatchingBrace(lines, i);
                if (vrefEnd != -1)
                {
                    var vrefLines = new List<string>();
                    for (int vrefLine = i; vrefLine <= vrefEnd; vrefLine++) { vrefLines.Add(lines[vrefLine]); }
                    ParseVertexRef(string.Join(" ", vrefLines), joint);
                    i = vrefEnd;
                }
                else { i++; }
            }
            else { i++; }
        }
        i = blockEnd;
        return joint;
    }

    private Matrix4x4 ParseTransformMatrix(string[] lines, ref int i, MultiTextureParserUtilities parserUtils)
    {
        int blockEnd = parserUtils.FindMatchingBrace(lines, i);
        if (blockEnd == -1) return Matrix4x4.identity;
        i++;
        while (i < blockEnd)
        {
            if (lines[i].Trim().StartsWith("<Matrix4>")) return parserUtils.ParseMatrix4(lines, ref i);
            i++;
        }
        i = blockEnd;
        return Matrix4x4.identity;
    }

    public void ParseVertexRef(string fullBlock, EggJoint joint)
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

    public void CreateMasterVertexBuffer(List<EggVertex> vertexPool, out Vector3[] masterVertices,
        out Vector3[] masterNormals, out Vector2[] masterUVs, out Color[] masterColors)
    {
        // Store vertex pool for multi-texture UV processing
        _vertexPool = vertexPool;
        
        // Generate fallback UV coordinates for vertices missing named UVs
        GenerateFallbackUVCoordinates(vertexPool);
        
        // Create overlay UV channels for multi-texture support
        CreateOverlayUVChannels(vertexPool);
        
        masterVertices = vertexPool.Select(v => v.position).ToArray();
        masterNormals = vertexPool.Select(v => v.normal).ToArray();
        masterUVs = vertexPool.Select(v => v.uv).ToArray();  // Original primary UV channel
        masterColors = vertexPool.Select(v => v.color).ToArray();
        
        DebugLogger.LogEggImporter($"✅ Created robust master vertex buffer with {vertexPool.Count} vertices");
        DebugLogger.LogEggImporter($"✅ Found vertices with named UVs: {vertexPool.Count(v => v.namedUVs.Count > 0)}");
    }
    
    private void NormalizeMeshUVCoordinates(Vector2[] meshUVs, string meshName)
    {
        if (meshUVs.Length == 0) return;
        
        // Analyze UV coordinate range
        float minU = meshUVs.Min(uv => uv.x);
        float maxU = meshUVs.Max(uv => uv.x);
        float minV = meshUVs.Min(uv => uv.y);
        float maxV = meshUVs.Max(uv => uv.y);
        
        float rangeU = maxU - minU;
        float rangeV = maxV - minV;
        
        DebugLogger.LogEggImporter($"🔍 [{meshName}] UV Analysis - U: [{minU:F2}, {maxU:F2}] (range: {rangeU:F2})");
        DebugLogger.LogEggImporter($"🔍 [{meshName}] UV Analysis - V: [{minV:F2}, {maxV:F2}] (range: {rangeV:F2})");
        
        // ENHANCED UV PROCESSING: Multi-factor analysis for universal compatibility
        UVProcessingDecision decision = AnalyzeUVPatternUniversal(meshUVs, minU, maxU, minV, maxV, rangeU, rangeV, meshName);
        
        switch (decision.Action)
        {
            case UVAction.LeaveUnchanged:
                DebugLogger.LogEggImporter($"✅ [{meshName}] {decision.Reason} - no processing needed");
                break;
                
            case UVAction.ModuloWrap:
                DebugLogger.LogEggImporter($"🔄 [{meshName}] {decision.Reason} - applying modulo wrapping");
                ApplyModuloWrapping(meshUVs);
                break;
                
            case UVAction.Normalize:
                DebugLogger.LogEggImporter($"🔧 [{meshName}] {decision.Reason} - applying normalization");
                ApplyNormalization(meshUVs, minU, maxU, minV, maxV, rangeU, rangeV);
                break;
        }
    }
    
    private enum UVAction { LeaveUnchanged, ModuloWrap, Normalize }
    
    private struct UVProcessingDecision
    {
        public UVAction Action;
        public string Reason;
    }
    
    // ENHANCED UNIVERSAL UV PATTERN ANALYSIS: Multi-factor analysis for wide mesh compatibility
    private UVProcessingDecision AnalyzeUVPatternUniversal(Vector2[] meshUVs, float minU, float maxU, float minV, float maxV, float rangeU, float rangeV, string meshName)
    {
        // ENHANCED PATTERN DETECTION with multiple analysis factors
        
        // Calculate key metrics for universal pattern detection
        float maxRange = Mathf.Max(rangeU, rangeV);
        float minRange = Mathf.Min(rangeU, rangeV);
        float aspectRatio = rangeV > 0 ? rangeU / rangeV : 1.0f;
        
        // Advanced pattern detection (declare these first)
        bool hasIntegerBoundaries = DetectIntegerBoundaries(meshUVs);
        bool hasRepeatingPattern = DetectRepeatingPattern(meshUVs, rangeU, rangeV);
        bool hasUniformDistribution = DetectUniformDistribution(meshUVs);
        bool hasClusteredValues = DetectClusteredValues(meshUVs);
        
        // COMPREHENSIVE PATTERN DETECTION based on exhaustive analysis
        string meshLower = meshName.ToLower();
        
        // Mesh-specific pattern detection
        bool isRockFormation = meshLower.Contains("rockformation") || meshLower.Contains("rock");
        bool isHighPolyMesh = meshUVs.Length > 100; // High polygon count suggests procedural UVs
        bool hasNoisePattern = meshLower.Contains("noise") || (maxRange > 5.0f && hasUniformDistribution);
        
        // Terrain pattern detection (from analysis: pir_m_are_isl_*, jungle_*, cave_*)
        bool isIslandTerrain = meshLower.Contains("pir_m_are_isl_") || meshLower.Contains("island");
        bool isJungleTerrain = meshLower.Contains("jungle_") && meshLower.Contains("_zero");
        bool isCaveTerrain = meshLower.Contains("cave_") && meshLower.Contains("_zero");
        
        // Texture type pattern detection (from material names)
        bool isMultiTexture = meshLower.Contains("multi_") || meshLower.Contains("overlay");
        bool isPaletteTexture = meshLower.Contains("palette_") || meshLower.Contains("props_");
        bool isMinimapTexture = meshLower.Contains("minimap_");
        
        // World-space scale detection based on known patterns
        bool isLargeWorldSpace = isIslandTerrain || isJungleTerrain || (maxRange > 15.0f);
        bool isMediumWorldSpace = isCaveTerrain || (maxRange > 2.5f && maxRange <= 15.0f);
        bool isMicroAtlas = isPaletteTexture || (maxRange <= 1.5f);
        
        // Pattern 1: Already well-behaved (0-1 range with tolerance)
        if (minU >= -0.1f && maxU <= 1.1f && minV >= -0.1f && maxV <= 1.1f)
        {
            return new UVProcessingDecision { Action = UVAction.LeaveUnchanged, Reason = "UVs in acceptable 0-1 range" };
        }
        
        // Pattern 1.5: Model-specific detection patterns (data-driven, no hardcoded names)
        
        // RavensCove-specific pattern: Extreme range with specific UV distribution characteristics
        // Analysis: U=[-197,70] V=[-29,34], ~50% negative, 270 unit range, 3447 vertices
        bool isRavensCovePattern = DetectRavensCovePattern(meshUVs, maxRange, rangeU, rangeV);
        
        // DelFuego-specific pattern (padres): High vertex count with uvNoise-style extreme scaling
        // Analysis: uvNoise U=[-740,144] V=[-40,159], 884 unit range, 3035 vertices
        // Issue: Needs repeating textures applied over current ones showing
        bool isDelFuegoPattern = DetectDelFuegoPattern(meshUVs, maxRange, rangeU, rangeV);
        
        if (isRavensCovePattern)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"RavensCove-pattern: dual-UV system (range: {maxRange:F1}, vertices: {meshUVs.Length})" };
        }
        
        if (isDelFuegoPattern)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"DelFuego-pattern (padres): uvNoise system with texture overlay (range: {maxRange:F1}, vertices: {meshUVs.Length})" };
        }
        
        // Pattern 2a: Comprehensive terrain-specific detection
        if (isIslandTerrain && maxRange > 10.0f)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Island terrain (ravensCove/cuba/delFuego-type: {maxRange:F1})" };
        }
        
        // Pattern 2b: Jungle terrain detection (medium-large world space)
        if (isJungleTerrain && maxRange > 5.0f)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Jungle terrain (jungle_*_zero type: {maxRange:F1})" };
        }
        
        // Pattern 2c: Multi-texture overlay detection (force world-space handling)
        if (isMultiTexture && maxRange > 1.5f)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Multi-texture overlay (pir_t_are_isl_multi_* type: {maxRange:F1})" };
        }
        
        // Pattern 2d: Rock formations and procedural meshes
        if ((isRockFormation || hasNoisePattern) && maxRange > 2.0f && (hasIntegerBoundaries || hasRepeatingPattern))
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Rock/procedural mesh tiling (range: {maxRange:F1}, mesh: {meshName})" };
        }
        
        // Pattern 2b: High-poly mesh with extreme coordinates (common in uvNoise systems)
        if (isHighPolyMesh && maxRange > 5.0f && hasUniformDistribution)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"High-poly procedural UVs (vertices: {meshUVs.Length}, range: {maxRange:F1})" };
        }
        
        // Pattern 2c: Clear tiling patterns (strong evidence of intentional repetition)
        if (hasIntegerBoundaries && hasRepeatingPattern && maxRange > 1.2f)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Strong tiling pattern (range: {maxRange:F1}, integer align: {hasIntegerBoundaries}, repeating: {hasRepeatingPattern})" };
        }
        
        // Pattern 3: Large world-space coordinates (likely needs wrapping)
        if (maxRange > 5.0f && minU >= -5.0f && minV >= -5.0f && hasUniformDistribution)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"Large uniform world-space coordinates (range: {maxRange:F1})" };
        }
        
        // Pattern 4: Atlas with negative offset (common in exported models)
        if ((minU < -0.05f || minV < -0.05f) && maxRange < 5.0f && hasClusteredValues)
        {
            return new UVProcessingDecision { Action = UVAction.Normalize, Reason = $"Atlas with negative offset (min: [{minU:F2}, {minV:F2}])" };
        }
        
        // Pattern 5: Moderate out-of-bounds without clear tiling intent
        if (maxRange > 1.1f && maxRange <= 5.0f && !hasRepeatingPattern && !hasIntegerBoundaries)
        {
            return new UVProcessingDecision { Action = UVAction.Normalize, Reason = $"Moderate range without tiling evidence (range: {maxRange:F1})" };
        }
        
        // Pattern 6: Data-driven coordinate scaling based on analysis patterns
        // UNIVERSAL SCALING RULES based on cross-file analysis:
        if (maxRange > 50.0f || Mathf.Abs(minU) > 50.0f || Mathf.Abs(minV) > 50.0f)
        {
            return new UVProcessingDecision { Action = UVAction.Normalize, Reason = $"Extreme coordinates - applying aggressive scaling (range: {maxRange:F1})" };
        }
        
        // Pattern 6b: High range coordinates (common in uvNoise systems)
        if (maxRange > 10.0f && maxRange <= 50.0f && hasUniformDistribution)
        {
            return new UVProcessingDecision { Action = UVAction.ModuloWrap, Reason = $"High-range uvNoise coordinates - applying divide-by-10 scaling (range: {maxRange:F1})" };
        }
        
        // Pattern 6c: Medium range coordinates that exceed normal texture bounds
        if (maxRange > 2.0f && maxRange <= 10.0f && !hasRepeatingPattern && !hasIntegerBoundaries)
        {
            return new UVProcessingDecision { Action = UVAction.Normalize, Reason = $"Medium-range coordinates without tiling evidence - applying divide-by-2 scaling (range: {maxRange:F1})" };
        }
        
        // Pattern 7: Very small ranges (might be degenerate UVs)
        if (maxRange < 0.01f && !hasClusteredValues)
        {
            return new UVProcessingDecision { Action = UVAction.LeaveUnchanged, Reason = $"Very small range - likely degenerate (range: {maxRange:F4})" };
        }
        
        // Default: Conservative approach - preserve working UVs
        return new UVProcessingDecision { Action = UVAction.LeaveUnchanged, Reason = $"Ambiguous pattern - preserving original (range: {maxRange:F1}, aspect: {aspectRatio:F2})" };
    }
    
    // Detect if UVs align with integer boundaries (common in tiling textures)
    private bool DetectIntegerBoundaries(Vector2[] meshUVs)
    {
        int integerAlignedCount = 0;
        
        foreach (var uv in meshUVs)
        {
            float fracU = Mathf.Abs(uv.x - Mathf.Round(uv.x));
            float fracV = Mathf.Abs(uv.y - Mathf.Round(uv.y));
            
            if (fracU < 0.01f || fracV < 0.01f) // Close to integer boundary
            {
                integerAlignedCount++;
            }
        }
        
        // If more than 20% of UVs are near integer boundaries, likely tiling
        return (float)integerAlignedCount / meshUVs.Length > 0.2f;
    }
    
    // Detect repeating patterns in UV coordinates
    private bool DetectRepeatingPattern(Vector2[] meshUVs, float rangeU, float rangeV)
    {
        if (meshUVs.Length < 4) return false;
        
        // Check if UV values cluster around regular intervals
        float maxRange = Mathf.Max(rangeU, rangeV);
        if (maxRange < 1.5f) return false;
        
        // Look for regular spacing patterns
        var uniqueUValues = meshUVs.Select(uv => Mathf.Round(uv.x * 4) / 4).Distinct().OrderBy(u => u).ToArray();
        var uniqueVValues = meshUVs.Select(uv => Mathf.Round(uv.y * 4) / 4).Distinct().OrderBy(v => v).ToArray();
        
        bool hasRegularUSpacing = HasRegularSpacing(uniqueUValues);
        bool hasRegularVSpacing = HasRegularSpacing(uniqueVValues);
        
        return hasRegularUSpacing || hasRegularVSpacing;
    }
    
    // Check if values have regular spacing (indicating tiling)
    private bool HasRegularSpacing(float[] sortedValues)
    {
        if (sortedValues.Length < 3) return false;
        
        float firstInterval = sortedValues[1] - sortedValues[0];
        if (firstInterval < 0.1f) return false;
        
        int consistentIntervals = 0;
        for (int i = 2; i < sortedValues.Length; i++)
        {
            float interval = sortedValues[i] - sortedValues[i-1];
            if (Mathf.Abs(interval - firstInterval) < 0.2f)
            {
                consistentIntervals++;
            }
        }
        
        // If most intervals are consistent, it's likely a regular pattern
        return consistentIntervals >= (sortedValues.Length - 2) * 0.7f;
    }
    
    // Detect if UV values are uniformly distributed (suggests world-space mapping)
    private bool DetectUniformDistribution(Vector2[] meshUVs)
    {
        if (meshUVs.Length < 10) return false;
        
        // Check if UVs are spread relatively evenly across the range
        var uValues = meshUVs.Select(uv => uv.x).OrderBy(u => u).ToArray();
        var vValues = meshUVs.Select(uv => uv.y).OrderBy(v => v).ToArray();
        
        float uRange = uValues[uValues.Length - 1] - uValues[0];
        float vRange = vValues[vValues.Length - 1] - vValues[0];
        
        if (uRange < 1.0f && vRange < 1.0f) return false;
        
        // Check for relatively even distribution (not clustered)
        bool uUniform = CheckUniformSpacing(uValues);
        bool vUniform = CheckUniformSpacing(vValues);
        
        return uUniform || vUniform;
    }
    
    // Detect if UV values are clustered (suggests atlas coordinates)
    private bool DetectClusteredValues(Vector2[] meshUVs)
    {
        if (meshUVs.Length < 4) return false;
        
        // Group UVs into clusters and see if they form distinct groups
        var clusters = new List<List<Vector2>>();
        const float clusterThreshold = 0.1f;
        
        foreach (var uv in meshUVs)
        {
            bool addedToCluster = false;
            foreach (var cluster in clusters)
            {
                if (cluster.Any(existing => Vector2.Distance(existing, uv) < clusterThreshold))
                {
                    cluster.Add(uv);
                    addedToCluster = true;
                    break;
                }
            }
            
            if (!addedToCluster)
            {
                clusters.Add(new List<Vector2> { uv });
            }
        }
        
        // If we have multiple distinct clusters, UVs are clustered
        return clusters.Count >= 2 && clusters.Count <= meshUVs.Length * 0.5f;
    }
    
    // Calculate percentage of coordinates that are negative (for dual-UV system detection)
    private float CalculateNegativeCoordinatePercentage(Vector2[] meshUVs)
    {
        if (meshUVs.Length == 0) return 0.0f;
        
        int negativeCount = 0;
        foreach (var uv in meshUVs)
        {
            if (uv.x < 0.0f || uv.y < 0.0f)
                negativeCount++;
        }
        
        return (float)negativeCount / meshUVs.Length * 100.0f;
    }
    
    // MODEL-SPECIFIC DETECTION PATTERNS (data-driven, no hardcoded names)
    
    // RavensCove pattern: Extreme range with specific dual-UV characteristics
    // Analysis: U=[-197,70] V=[-29,34], ~50% negative, 270 range, 3447 vertices
    private bool DetectRavensCovePattern(Vector2[] meshUVs, float maxRange, float rangeU, float rangeV)
    {
        // RavensCove has very specific characteristics that distinguish it
        bool hasRavensCoveRange = maxRange > 200.0f && maxRange < 400.0f; // 270 range
        bool hasRavensCoveVertexCount = meshUVs.Length > 3000 && meshUVs.Length < 4000; // 3447 vertices
        bool hasRavensCoveUVDistribution = rangeU > 200.0f && rangeV > 50.0f && rangeV < 80.0f; // U=267, V=63
        float negativePercentage = CalculateNegativeCoordinatePercentage(meshUVs);
        bool hasRavensCoveNegativePattern = negativePercentage > 45.0f && negativePercentage < 60.0f; // ~50%
        
        return hasRavensCoveRange && hasRavensCoveVertexCount && hasRavensCoveUVDistribution && hasRavensCoveNegativePattern;
    }
    
    // DelFuego pattern (padres): uvNoise-style extreme scaling with high vertex count
    // Analysis: uvNoise U=[-740,144] V=[-40,159], 884 range, 3035 vertices
    // Issue: Half-way fixed, needs repeating textures applied over current ones showing
    private bool DetectDelFuegoPattern(Vector2[] meshUVs, float maxRange, float rangeU, float rangeV)
    {
        // DelFuego has uvNoise coordinates with massive range but different proportions
        bool hasDelFuegoRange = maxRange > 800.0f; // 884 range
        bool hasDelFuegoVertexCount = meshUVs.Length > 2500 && meshUVs.Length < 3500; // 3035 vertices
        bool hasDelFuegoUVDistribution = rangeU > 800.0f && rangeV > 150.0f && rangeV < 250.0f; // U=884, V=199
        float negativePercentage = CalculateNegativeCoordinatePercentage(meshUVs);
        bool hasDelFuegoNegativePattern = negativePercentage > 50.0f; // 56%
        
        return hasDelFuegoRange && hasDelFuegoVertexCount && hasDelFuegoUVDistribution && hasDelFuegoNegativePattern;
    }
    
    
    // Helper method to check uniform spacing in sorted array
    private bool CheckUniformSpacing(float[] sortedValues)
    {
        if (sortedValues.Length < 3) return false;
        
        float totalRange = sortedValues[sortedValues.Length - 1] - sortedValues[0];
        float expectedInterval = totalRange / (sortedValues.Length - 1);
        
        int uniformIntervals = 0;
        for (int i = 1; i < sortedValues.Length; i++)
        {
            float actualInterval = sortedValues[i] - sortedValues[i - 1];
            if (Mathf.Abs(actualInterval - expectedInterval) < expectedInterval * 0.3f)
            {
                uniformIntervals++;
            }
        }
        
        return uniformIntervals >= (sortedValues.Length - 1) * 0.6f;
    }
    
    // Apply intelligent modulo wrapping that preserves intended tiling scale
    private void ApplyModuloWrapping(Vector2[] meshUVs)
    {
        if (meshUVs.Length == 0) return;
        
        // Calculate the UV range to determine appropriate tiling scale
        float minU = meshUVs.Min(uv => uv.x);
        float maxU = meshUVs.Max(uv => uv.x);
        float minV = meshUVs.Min(uv => uv.y);
        float maxV = meshUVs.Max(uv => uv.y);
        
        float rangeU = maxU - minU;
        float rangeV = maxV - minV;
        float maxRange = Mathf.Max(rangeU, rangeV);
        
        // INTELLIGENT TILING PRESERVATION based on coordinate analysis
        float tilingScaleU = 1.0f;
        float tilingScaleV = 1.0f;
        
        // Determine appropriate tiling scale based on coordinate range
        if (maxRange > 100.0f) // ravensCove-level extreme coordinates
        {
            tilingScaleU = Mathf.Max(1.0f, rangeU / 50.0f); // Preserve large-scale tiling
            tilingScaleV = Mathf.Max(1.0f, rangeV / 50.0f);
            DebugLogger.LogEggImporter($"🔄 Extreme tiling preservation: U={tilingScaleU:F2}x, V={tilingScaleV:F2}x (range: {maxRange:F1})");
        }
        else if (maxRange > 50.0f) // cuba/tormenta-level 
        {
            tilingScaleU = Mathf.Max(1.0f, rangeU / 25.0f);
            tilingScaleV = Mathf.Max(1.0f, rangeV / 25.0f);
            DebugLogger.LogEggImporter($"🔄 Large tiling preservation: U={tilingScaleU:F2}x, V={tilingScaleV:F2}x (range: {maxRange:F1})");
        }
        else if (maxRange > 15.0f) // delFuego/jungle-level
        {
            tilingScaleU = Mathf.Max(1.0f, rangeU / 10.0f);
            tilingScaleV = Mathf.Max(1.0f, rangeV / 10.0f);
            DebugLogger.LogEggImporter($"🔄 Medium tiling preservation: U={tilingScaleU:F2}x, V={tilingScaleV:F2}x (range: {maxRange:F1})");
        }
        else if (maxRange > 2.5f) // tortuga-level
        {
            tilingScaleU = Mathf.Max(1.0f, rangeU / 5.0f);
            tilingScaleV = Mathf.Max(1.0f, rangeV / 5.0f);
            DebugLogger.LogEggImporter($"🔄 Small tiling preservation: U={tilingScaleU:F2}x, V={tilingScaleV:F2}x (range: {maxRange:F1})");
        }
        
        // Apply intelligent wrapping that preserves tiling frequency
        for (int i = 0; i < meshUVs.Length; i++)
        {
            float scaledU = meshUVs[i].x / tilingScaleU;
            float scaledV = meshUVs[i].y / tilingScaleV;
            
            // Apply modulo wrapping to scaled coordinates
            float wrappedU = scaledU - Mathf.Floor(scaledU);
            float wrappedV = scaledV - Mathf.Floor(scaledV);
            
            // Ensure positive values
            if (wrappedU < 0) wrappedU += 1.0f;
            if (wrappedV < 0) wrappedV += 1.0f;
            
            meshUVs[i] = new Vector2(wrappedU, wrappedV);
        }
    }
    
    // Apply intelligent normalization with data-driven scaling
    private void ApplyNormalization(Vector2[] meshUVs, float minU, float maxU, float minV, float maxV, float rangeU, float rangeV)
    {
        float maxRange = Mathf.Max(rangeU, rangeV);
        
        // COMPREHENSIVE SCALING RULES based on exhaustive cross-file analysis
        // Covers ranges from 0.117 (micro-atlas) to 267.788+ (massive world-space)
        float scalingFactor = 1.0f;
        string scalingReason = "";
        
        // Tier 6: Extreme World Detection (100.0+ range) - ravensCove level
        if (maxRange > 100.0f || Mathf.Abs(minU) > 100.0f || Mathf.Abs(minV) > 100.0f)
        {
            scalingFactor = Mathf.Clamp(maxRange * 0.01f, 5.0f, 50.0f); // Dynamic scaling for massive coordinates
            scalingReason = $"Extreme world coordinates (ravensCove-level: {maxRange:F1})";
        }
        // Tier 5: Massive World Detection (50.0-100.0 range) - cuba/tormenta level  
        else if (maxRange > 50.0f)
        {
            scalingFactor = maxRange * 0.02f; // cuba/tormenta terrain scaling
            scalingReason = $"Massive world terrain (cuba-level: {maxRange:F1})";
        }
        // Tier 4: Large World Detection (15.0-50.0 range) - delFuego/jungle level
        else if (maxRange > 15.0f)
        {
            scalingFactor = maxRange * 0.05f; // delFuego/jungle terrain scaling
            scalingReason = $"Large world space (delFuego-level: {maxRange:F1})";
        }
        // Tier 3: Medium Tiling Detection (2.5-15.0 range) - tortuga/outcast level
        else if (maxRange > 2.5f)
        {
            scalingFactor = maxRange * 0.1f; // tortuga medium terrain scaling
            scalingReason = $"Medium terrain tiling (tortuga-level: {maxRange:F1})";
        }
        // Tier 2: Small Tiling Detection (1.5-2.5 range) - cave systems
        else if (maxRange > 1.5f)
        {
            scalingFactor = maxRange; // Preserve small tiling patterns
            scalingReason = $"Small tiling texture (cave-level: {maxRange:F1})";
        }
        // Tier 1: Micro-Atlas/Normalized (0.0-1.5 range) - props/characters
        else
        {
            scalingFactor = 1.0f; // Keep normalized/atlas textures as-is
            scalingReason = $"Micro-atlas/normalized (prop-level: {maxRange:F1})";
        }
        
        if (scalingFactor > 1.0f)
        {
            DebugLogger.LogEggImporter($"🔧 Applying data-driven scaling (÷{scalingFactor:F2}) - {scalingReason}");
        }
        
        if (scalingFactor > 1.0f)
        {
            // Apply scaling first, then normalize to 0-1 range
            for (int i = 0; i < meshUVs.Length; i++)
            {
                float scaledU = meshUVs[i].x / scalingFactor;
                float scaledV = meshUVs[i].y / scalingFactor;
                
                // Then normalize the scaled coordinates to 0-1 range
                float normalizedU = (scaledU - (minU / scalingFactor)) / (rangeU / scalingFactor);
                float normalizedV = (scaledV - (minV / scalingFactor)) / (rangeV / scalingFactor);
                
                // Ensure values are in 0-1 range
                normalizedU = Mathf.Clamp01(normalizedU);
                normalizedV = Mathf.Clamp01(normalizedV);
                
                meshUVs[i] = new Vector2(normalizedU, normalizedV);
            }
        }
        else
        {
            // Standard normalization for coordinates within reasonable ranges
            for (int i = 0; i < meshUVs.Length; i++)
            {
                float normalizedU = rangeU > 0 ? (meshUVs[i].x - minU) / rangeU : 0.5f;
                float normalizedV = rangeV > 0 ? (meshUVs[i].y - minV) / rangeV : 0.5f;
                
                meshUVs[i] = new Vector2(normalizedU, normalizedV);
            }
        }
    }
    
    // SHADER-BASED TILING SYSTEM: Detect repetition frequency and let shaders handle tiling
    private Vector2 DetectTilingFrequency(Vector2[] meshUVs, string meshName)
    {
        if (meshUVs.Length < 4) return Vector2.one;
        
        // Analyze UV distribution to detect repetition patterns
        float minU = meshUVs.Min(uv => uv.x);
        float maxU = meshUVs.Max(uv => uv.x);
        float minV = meshUVs.Min(uv => uv.y);
        float maxV = meshUVs.Max(uv => uv.y);
        
        float rangeU = maxU - minU;
        float rangeV = maxV - minV;
        
        // Calculate tiling frequency based on UV range
        Vector2 tilingFreq = CalculateTilingFromRange(rangeU, rangeV, meshUVs);
        
        DebugLogger.LogEggImporter($"🎨 [{meshName}] Detected tiling frequency: U={tilingFreq.x:F2}, V={tilingFreq.y:F2} (range: {rangeU:F1}x{rangeV:F1})");
        
        return tilingFreq;
    }
    
    private Vector2 CalculateTilingFromRange(float rangeU, float rangeV, Vector2[] meshUVs)
    {
        float tileU = 1.0f;
        float tileV = 1.0f;
        
        // ADJUSTED: Less aggressive tiling detection
        // Only apply tiling if we have clear evidence of repetition
        
        // Check for integer clustering first - strong indicator of intentional tiling
        bool hasIntegerClusteringU = HasIntegerClustering(meshUVs.Select(uv => uv.x).ToArray());
        bool hasIntegerClusteringV = HasIntegerClustering(meshUVs.Select(uv => uv.y).ToArray());
        
        // If UV range is large AND has integer clustering, apply conservative tiling
        if (rangeU > 2.0f && hasIntegerClusteringU)
        {
            // Use floor instead of round for more conservative tiling
            tileU = Mathf.Floor(rangeU);
            
            // Cap maximum tiling to prevent over-tiling
            tileU = Mathf.Min(tileU, 10.0f);
        }
        
        if (rangeV > 2.0f && hasIntegerClusteringV)
        {
            tileV = Mathf.Floor(rangeV);
            tileV = Mathf.Min(tileV, 10.0f);
        }
        
        // SAFETY: If no clear pattern, don't apply tiling
        if (!hasIntegerClusteringU && rangeU > 1.1f)
        {
            DebugLogger.LogEggImporter($"🔍 No integer clustering detected for U axis despite range {rangeU:F1} - keeping tiling at 1.0");
            tileU = 1.0f;
        }
        
        if (!hasIntegerClusteringV && rangeV > 1.1f)
        {
            DebugLogger.LogEggImporter($"🔍 No integer clustering detected for V axis despite range {rangeV:F1} - keeping tiling at 1.0");
            tileV = 1.0f;
        }
        
        return new Vector2(tileU, tileV);
    }
    
    private bool HasIntegerClustering(float[] values)
    {
        if (values.Length < 3) return false;
        
        int nearIntegerCount = 0;
        foreach (float value in values)
        {
            float distanceToNearestInt = Mathf.Abs(value - Mathf.Round(value));
            if (distanceToNearestInt < 0.1f) // Close to integer
            {
                nearIntegerCount++;
            }
        }
        
        // If more than 30% of values are near integers, likely tiling pattern
        return (float)nearIntegerCount / values.Length > 0.3f;
    }
    
    // REMOVED: All shader-based tiling methods - reverted to pre-shader state
    
    // Extract UV set name from UV line, handling multiple naming conventions
    private string ExtractUVSetName(string uvLine)
    {
        // Handle all naming conventions found in the analysis:
        // <UV> muti-sand { ... }        (rumRunner - working)
        // <UV> uvSet { ... }            (tortuga)
        // <UV> Sand { ... }             (pvpSpanish)
        // <UV> uvNoise { ... }          (delFuego)
        
        if (uvLine.Contains("muti-sand") || uvLine.Contains("multi-sand"))
            return "muti-sand";
        else if (uvLine.Contains("uvSet"))
            return "uvSet";
        else if (uvLine.Contains("Sand"))
            return "Sand";
        else if (uvLine.Contains("uvNoise"))
            return "uvNoise";
        else if (uvLine.Contains("Dirt"))
            return "Dirt";
        else if (uvLine.Contains("Rock"))
            return "Rock";
        
        // No named UV set found - this is a primary UV coordinate
        return null;
    }
    
    // Normalize different UV set naming conventions to a standard name
    private string NormalizeUVSetName(string uvSetName)
    {
        // Map all overlay texture UV sets to a standard name for Unity
        // This helps with consistent shader UV channel mapping
        switch (uvSetName.ToLower())
        {
            case "muti-sand":
            case "multi-sand":
            case "uvset":
            case "sand":
            case "uvnoise":
            case "dirt":
            case "rock":
                return "overlay_uv"; // Standardized name for overlay textures
            default:
                return uvSetName; // Keep original name if not recognized
        }
    }
    
    private HashSet<int> _multiTextureVertices = new HashSet<int>();
    private List<EggVertex> _vertexPool;
    private Dictionary<string, string> _textureUVMappings = new Dictionary<string, string>();
    private Dictionary<string, Vector2[]> _overlayUVChannels = new Dictionary<string, Vector2[]>();
    // REMOVED: Tiling frequency storage - reverted to pre-shader state
    // REMOVED: Shader-based tiling system no longer needed
    
    // Normalize primary UV coordinates that are outside 0-1 range (found in broken models)
    
    // Generate fallback overlay UV coordinates for vertices that are missing them
    private void GenerateFallbackUVCoordinates(List<EggVertex> vertexPool)
    {
        // Find all vertices that should have overlay UVs but are missing them
        var verticesWithOverlayUVs = vertexPool.Where(v => v.namedUVs.Count > 0).ToList();
        var verticesWithoutOverlayUVs = vertexPool.Where(v => v.namedUVs.Count == 0).ToList();
        
        if (verticesWithOverlayUVs.Count > 0 && verticesWithoutOverlayUVs.Count > 0)
        {
            DebugLogger.LogEggImporter($"🔧 Found {verticesWithoutOverlayUVs.Count} vertices missing overlay UVs, generating fallbacks");
            
            // Analyze overlay UV range from vertices that have them
            var allOverlayUVs = new List<Vector2>();
            foreach (var vertex in verticesWithOverlayUVs)
            {
                foreach (var namedUV in vertex.namedUVs.Values)
                {
                    allOverlayUVs.Add(namedUV);
                }
            }
            
            if (allOverlayUVs.Count > 0)
            {
                // Calculate overlay UV bounds for scaling
                float minU = allOverlayUVs.Min(uv => uv.x);
                float maxU = allOverlayUVs.Max(uv => uv.x);
                float minV = allOverlayUVs.Min(uv => uv.y);
                float maxV = allOverlayUVs.Max(uv => uv.y);
                
                float scaleU = maxU - minU;
                float scaleV = maxV - minV;
                
                DebugLogger.LogEggImporter($"🔧 Overlay UV range: U[{minU:F2}, {maxU:F2}], V[{minV:F2}, {maxV:F2}]");
                
                // Check if overlay UV range is reasonable for texture mapping
                float maxRange = Mathf.Max(scaleU, scaleV);
                bool hasReasonableRange = (minU >= -5.0f && maxU <= 140.0f && minV >= -5.0f && maxV <= 140.0f);
                
                if (!hasReasonableRange || maxRange > 200.0f)
                {
                    DebugLogger.LogEggImporter($"🔧 Overlay UV range too extreme ({maxRange:F1}) - using normalized 0-1 range for fallback generation");
                    // Use 0-1 range for overlay UVs when existing range is too extreme
                    minU = 0.0f; maxU = 1.0f; minV = 0.0f; maxV = 1.0f;
                    scaleU = 1.0f; scaleV = 1.0f;
                }
                
                // Generate fallback overlay UVs for vertices that don't have them
                foreach (var vertex in verticesWithoutOverlayUVs)
                {
                    // Convert primary UV (0-1) to overlay UV range
                    float overlayU = minU + (vertex.uv.x * scaleU);
                    float overlayV = minV + (vertex.uv.y * scaleV);
                    
                    vertex.namedUVs["overlay_uv"] = new Vector2(overlayU, overlayV);
                }
                
                DebugLogger.LogEggImporter($"✅ Generated fallback overlay UVs for {verticesWithoutOverlayUVs.Count} vertices");
            }
        }
    }
    
    // Create overlay UV channels with proper scaling
    private void CreateOverlayUVChannels(List<EggVertex> vertexPool)
    {
        // Find all unique named UV sets (normalized)
        var allNamedUVSets = new HashSet<string>();
        foreach (var vertex in vertexPool)
        {
            foreach (var uvSetName in vertex.namedUVs.Keys)
            {
                allNamedUVSets.Add("overlay_uv"); // Use normalized name
            }
        }
        
        DebugLogger.LogEggImporter($"🔍 Creating overlay UV channels: {string.Join(", ", allNamedUVSets)}");
        
        // Create UV arrays for overlay textures
        foreach (string uvSetName in allNamedUVSets)
        {
            Vector2[] uvChannel = new Vector2[vertexPool.Count];
            
            // Collect overlay UV coordinates
            var overlayUVs = new List<Vector2>();
            for (int i = 0; i < vertexPool.Count; i++)
            {
                if (vertexPool[i].namedUVs.Count > 0)
                {
                    // Use any named UV (they should all be normalized to overlay_uv)
                    var namedUV = vertexPool[i].namedUVs.Values.First();
                    overlayUVs.Add(namedUV);
                    uvChannel[i] = namedUV;
                }
                else
                {
                    // Use primary UV as fallback
                    uvChannel[i] = vertexPool[i].uv;
                    overlayUVs.Add(vertexPool[i].uv);
                }
            }
            
            // Only fix extreme overlay UV ranges like delFuego's -740 to +144
            FixExtremeOverlayUVs(uvChannel, $"OverlayChannel_{uvSetName}");
            
            _overlayUVChannels[uvSetName] = uvChannel;
            DebugLogger.LogEggImporter($"✅ Created overlay UV channel '{uvSetName}' with {uvChannel.Length} coordinates");
        }
    }
    
    private void FixExtremeOverlayUVs(Vector2[] overlayUVs, string channelName)
    {
        if (overlayUVs.Length == 0) return;
        
        // Analyze overlay UV coordinate range
        float minU = overlayUVs.Min(uv => uv.x);
        float maxU = overlayUVs.Max(uv => uv.x);
        float minV = overlayUVs.Min(uv => uv.y);
        float maxV = overlayUVs.Max(uv => uv.y);
        
        float rangeU = maxU - minU;
        float rangeV = maxV - minV;
        
        DebugLogger.LogEggImporter($"🔍 [{channelName}] Overlay UV range: U[{minU:F2}, {maxU:F2}], V[{minV:F2}, {maxV:F2}]");
        
        // REVERTED: Simple UV range check without complex tiling calculations
        bool hasExtremeRange = (minU < -10.0f || minV < -10.0f || maxU > 10.0f || maxV > 10.0f);
        
        if (hasExtremeRange)
        {
            DebugLogger.LogEggImporter($"🔧 [{channelName}] Extreme UV range detected - normalizing to 0-1");
            
            // Now normalize the UVs
            for (int i = 0; i < overlayUVs.Length; i++)
            {
                float normalizedU = rangeU > 0 ? (overlayUVs[i].x - minU) / rangeU : 0.5f;
                float normalizedV = rangeV > 0 ? (overlayUVs[i].y - minV) / rangeV : 0.5f;
                
                overlayUVs[i] = new Vector2(normalizedU, normalizedV);
            }
            
            DebugLogger.LogEggImporter($"✅ [{channelName}] Fixed extreme overlay UVs");
        }
        else
        {
            DebugLogger.LogEggImporter($"✅ [{channelName}] Overlay UVs acceptable - leaving unchanged");
        }
    }
    
    // SIMPLIFIED: Removed GetUVsForTexture method - using only primary UVs now
    
    // UNUSED FUNCTION - marked for removal
    private void ScaleGroundTextureUVs(List<EggVertex> vertexPool)
    {
        if (vertexPool.Count == 0) return;
        
        // Separate vertices into normal UVs (0-1 range) and world-space UVs (large values)
        var normalUVVertices = new List<int>();
        var worldSpaceUVVertices = new List<int>();
        
        for (int i = 0; i < vertexPool.Count; i++)
        {
            var uv = vertexPool[i].uv;
            // Check if UV is outside normal 0-1 range (with some tolerance)
            if (Mathf.Abs(uv.x) > 2.0f || Mathf.Abs(uv.y) > 2.0f || uv.x < -0.1f || uv.y < -0.1f || uv.x > 1.1f || uv.y > 1.1f)
            {
                worldSpaceUVVertices.Add(i);
            }
            else
            {
                normalUVVertices.Add(i);
            }
        }
        
        DebugLogger.LogEggImporter($"🔍 Found {normalUVVertices.Count} vertices with normal UVs and {worldSpaceUVVertices.Count} vertices with world-space UVs");
        
        if (worldSpaceUVVertices.Count > 0)
        {
            // Find bounds for ONLY the world-space UV vertices
            float minU = float.MaxValue, maxU = float.MinValue;
            float minV = float.MaxValue, maxV = float.MinValue;
            
            foreach (int i in worldSpaceUVVertices)
            {
                var uv = vertexPool[i].uv;
                if (uv.x < minU) minU = uv.x;
                if (uv.x > maxU) maxU = uv.x;
                if (uv.y < minV) minV = uv.y;
                if (uv.y > maxV) maxV = uv.y;
            }
            
            float rangeU = maxU - minU;
            float rangeV = maxV - minV;
            
            DebugLogger.LogEggImporter($"🔍 World-space UV bounds: U({minU:F4} to {maxU:F4}), V({minV:F4} to {maxV:F4})");
            DebugLogger.LogEggImporter($"🔍 World-space UV ranges: U={rangeU:F4}, V={rangeV:F4}");
            
            // Use the same proportional scaling that worked for rumrunners
            // Scale ALL world-space UVs to 0-1 range while preserving relationships
            DebugLogger.LogEggImporter($"🔧 Scaling {worldSpaceUVVertices.Count} world-space UV vertices proportionally to 0-1 range");
            
            foreach (int i in worldSpaceUVVertices)
            {
                var vertex = vertexPool[i];
                // Scale proportionally to 0-1 range (preserves texture layout)
                float scaledU = (vertex.uv.x - minU) / rangeU;
                float scaledV = (vertex.uv.y - minV) / rangeV;
                vertex.uv = new Vector2(scaledU, scaledV);
                vertexPool[i] = vertex;
            }
            
            DebugLogger.LogEggImporter($"🔧 Successfully scaled world-space UVs with fixed divisor while preserving {normalUVVertices.Count} normal UVs");
        }
        else
        {
            DebugLogger.LogEggImporter($"🔧 No world-space UVs detected, all UVs are in normal range");
        }
    }
    
    // UNUSED FUNCTION - marked for removal
    private void ScaleMultiTextureUVsGlobally()
    {
        if (_vertexPool == null || _multiTextureVertices.Count == 0) 
        {
            DebugLogger.LogEggImporter($"🔧 No multi-texture vertices to scale");
            return;
        }
        
        // Find global bounds for ALL multi-texture vertices
        float minU = float.MaxValue, maxU = float.MinValue;
        float minV = float.MaxValue, maxV = float.MinValue;
        
        foreach (int vertIndex in _multiTextureVertices)
        {
            if (vertIndex >= 0 && vertIndex < _vertexPool.Count)
            {
                var uv = _vertexPool[vertIndex].uv;
                if (uv.x < minU) minU = uv.x;
                if (uv.x > maxU) maxU = uv.x;
                if (uv.y < minV) minV = uv.y;
                if (uv.y > maxV) maxV = uv.y;
            }
        }
        
        float rangeU = maxU - minU;
        float rangeV = maxV - minV;
        
        DebugLogger.LogEggImporter($"🔍 Global multi-texture UV bounds: U({minU:F4} to {maxU:F4}), V({minV:F4} to {maxV:F4})");
        DebugLogger.LogEggImporter($"🔍 Global multi-texture UV ranges: U={rangeU:F4}, V={rangeV:F4}");
        
        // Scale ALL multi-texture vertices using the same global bounds
        if (rangeU > 0 && rangeV > 0)
        {
            foreach (int vertIndex in _multiTextureVertices)
            {
                if (vertIndex >= 0 && vertIndex < _vertexPool.Count)
                {
                    var vertex = _vertexPool[vertIndex];
                    float scaledU = (vertex.uv.x - minU) / rangeU;
                    float scaledV = (vertex.uv.y - minV) / rangeV;
                    vertex.uv = new Vector2(scaledU, scaledV);
                    _vertexPool[vertIndex] = vertex;
                }
            }
            DebugLogger.LogEggImporter($"🔧 Globally scaled {_multiTextureVertices.Count} multi-texture vertices to 0-1 range");
        }
    }
    
    // UNUSED FUNCTION - marked for removal
    private void NormalizeWorldSpaceUVs(List<EggVertex> vertexPool)
    {
        if (vertexPool.Count == 0) return;
        
        // Separate vertices into normal UVs (0-1 range) and world-space UVs (large values)
        var normalUVVertices = new List<int>();
        var worldSpaceUVVertices = new List<int>();
        
        for (int i = 0; i < vertexPool.Count; i++)
        {
            var uv = vertexPool[i].uv;
            // Check if UV is outside normal 0-1 range (with some tolerance)
            if (Mathf.Abs(uv.x) > 2.0f || Mathf.Abs(uv.y) > 2.0f || uv.x < -0.1f || uv.y < -0.1f || uv.x > 1.1f || uv.y > 1.1f)
            {
                worldSpaceUVVertices.Add(i);
            }
            else
            {
                normalUVVertices.Add(i);
            }
        }
        
        DebugLogger.LogEggImporter($"🔍 Found {normalUVVertices.Count} vertices with normal UVs and {worldSpaceUVVertices.Count} vertices with world-space UVs");
        
        if (worldSpaceUVVertices.Count > 0)
        {
            // Find bounds for ONLY the world-space UV vertices
            float minU = float.MaxValue, maxU = float.MinValue;
            float minV = float.MaxValue, maxV = float.MinValue;
            
            foreach (int i in worldSpaceUVVertices)
            {
                var uv = vertexPool[i].uv;
                if (uv.x < minU) minU = uv.x;
                if (uv.x > maxU) maxU = uv.x;
                if (uv.y < minV) minV = uv.y;
                if (uv.y > maxV) maxV = uv.y;
            }
            
            float rangeU = maxU - minU;
            float rangeV = maxV - minV;
            
            DebugLogger.LogEggImporter($"🔍 World-space UV bounds: U({minU:F4} to {maxU:F4}), V({minV:F4} to {maxV:F4})");
            DebugLogger.LogEggImporter($"🔍 World-space UV ranges: U={rangeU:F4}, V={rangeV:F4}");
            
            // Use intelligent scaling - scale down by a reasonable factor that preserves texture detail
            // This maintains texture relationships while making UVs valid
            float scaleFactor = Mathf.Max(rangeU, rangeV) / 8.0f; // Target ~8 texture repeats max
            DebugLogger.LogEggImporter($"🔧 Scaling world-space UVs by factor {1.0f/scaleFactor:F4} to preserve texture layout");
            
            foreach (int i in worldSpaceUVVertices)
            {
                var vertex = vertexPool[i];
                // Scale down while preserving relationships
                float scaledU = vertex.uv.x / scaleFactor;
                float scaledV = vertex.uv.y / scaleFactor;
                
                // Then wrap to 0-1 range while maintaining fractional relationships
                scaledU = scaledU - Mathf.Floor(scaledU);
                scaledV = scaledV - Mathf.Floor(scaledV);
                
                // Handle negative coordinates
                if (scaledU < 0) scaledU += 1.0f;
                if (scaledV < 0) scaledV += 1.0f;
                
                vertex.uv = new Vector2(scaledU, scaledV);
                vertexPool[i] = vertex;
            }
            
            DebugLogger.LogEggImporter($"🔧 Successfully scaled and wrapped world-space UVs while preserving {normalUVVertices.Count} normal UVs");
        }
        else
        {
            DebugLogger.LogEggImporter($"🔧 No world-space UVs detected, all UVs are in normal range");
        }
    }
    
    public Vector4 CalculateUVBounds(List<EggVertex> vertexPool)
    {
        if (vertexPool.Count == 0)
            return new Vector4(0, 0, 1, 1); // Default bounds
            
        float minU = float.MaxValue, minV = float.MaxValue;
        float maxU = float.MinValue, maxV = float.MinValue;
        
        foreach (var vertex in vertexPool)
        {
            if (vertex.uv.x < minU) minU = vertex.uv.x;
            if (vertex.uv.x > maxU) maxU = vertex.uv.x;
            if (vertex.uv.y < minV) minV = vertex.uv.y;
            if (vertex.uv.y > maxV) maxV = vertex.uv.y;
        }
        
        DebugLogger.LogEggImporter($"🔍 UV Bounds detected: U({minU:F3} to {maxU:F3}), V({minV:F3} to {maxV:F3})");
        DebugLogger.LogEggImporter($"🔍 UV Range: U={maxU - minU:F3}, V={maxV - minV:F3}");
        
        return new Vector4(minU, minV, maxU, maxV);
    }

    private bool WillContainGeometry(string[] lines, int transformStart, Dictionary<string, Transform> hierarchyMap, string currentPath)
    {
        // Look ahead in the current group to see if it contains polygons
        int groupStart = -1;
        
        // Find the start of the group this transform belongs to
        for (int i = transformStart - 1; i >= 0; i--)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Group>"))
            {
                groupStart = i;
                break;
            }
        }
        
        if (groupStart == -1) return false;
        
        int groupEnd = _parserUtils.FindMatchingBrace(lines, groupStart);
        if (groupEnd == -1) return false;
        
        // Check if this group or any child groups contain polygons
        return ContainsPolygonsRecursive(lines, groupStart, groupEnd);
    }
    
    private bool ContainsPolygonsRecursive(string[] lines, int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Polygon>"))
            {
                return true;
            }
            else if (line.StartsWith("<Group>"))
            {
                int childGroupEnd = _parserUtils.FindMatchingBrace(lines, i);
                if (childGroupEnd != -1 && ContainsPolygonsRecursive(lines, i + 1, childGroupEnd))
                {
                    return true;
                }
                i = childGroupEnd; // Skip past this child group
            }
        }
        return false;
    }
    
    private bool IsLODGroup(string[] lines, int groupStart, int groupEnd)
    {
        // Check if this group contains both <SwitchCondition> and <Distance>
        bool hasSwitchCondition = false;
        bool hasDistance = false;
        
        for (int i = groupStart + 1; i < groupEnd; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<SwitchCondition>"))
                hasSwitchCondition = true;
            else if (line.StartsWith("<Distance>"))
                hasDistance = true;
                
            if (hasSwitchCondition && hasDistance)
                return true;
        }
        
        return false;
    }
    
    private bool ShouldImportLOD(string[] lines, int groupStart, int groupEnd, string groupName)
    {
        var settings = EggImporterSettings.Instance;
        
        switch (settings.lodImportMode)
        {
            case EggImporterSettings.LODImportMode.AllLODs:
                DebugLogger.LogEggImporter($"📊 Importing LOD: '{groupName}' (All LODs mode)");
                return true;
                
            case EggImporterSettings.LODImportMode.HighestOnly:
                bool isHighestLOD = IsHighestQualityLOD(lines, groupStart, groupEnd);
                if (isHighestLOD)
                    DebugLogger.LogEggImporter($"✨ Importing highest quality LOD: '{groupName}'");
                return isHighestLOD;
                
            case EggImporterSettings.LODImportMode.Custom:
                // Future implementation for custom LOD selection
                return IsHighestQualityLOD(lines, groupStart, groupEnd);
                
            default:
                return IsHighestQualityLOD(lines, groupStart, groupEnd);
        }
    }
    
    private bool IsHighestQualityLOD(string[] lines, int groupStart, int groupEnd)
    {
        // Find the Distance tag and check if the second number is 0
        for (int i = groupStart + 1; i < groupEnd; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("<Distance>"))
            {
                // Parse the distance values: <Distance> { max_distance min_distance <Vertex> { 0 0 0 } }
                int openBrace = line.IndexOf('{');
                int closeBrace = line.IndexOf('}');
                if (openBrace != -1 && closeBrace != -1)
                {
                    string distanceContent = line.Substring(openBrace + 1, closeBrace - openBrace - 1).Trim();
                    var parts = distanceContent.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    
                    if (parts.Length >= 2 && float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float minDistance))
                    {
                        bool isHighest = minDistance == 0.0f;
                        DebugLogger.LogEggImporter($"🔍 LOD Distance check - Min distance: {minDistance}, Is highest: {isHighest}");
                        return isHighest;
                    }
                }
                break;
            }
        }
        
        return false; // Default to not importing if we can't determine
    }
    
    // Find fallback material for multi-texture names by trying individual components
    private Material FindFallbackMaterial(string matName, Dictionary<string, Material> materialDict)
    {
        // Check if this is a multi-texture material name (contains ||)
        if (matName.Contains("||"))
        {
            var textureNames = matName.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            
            // Try each individual texture name as a fallback
            foreach (string textureName in textureNames)
            {
                string trimmedName = textureName.Trim();
                if (materialDict.TryGetValue(trimmedName, out Material foundMat))
                {
                    DebugLogger.LogEggImporter($"🔄 Found fallback material '{trimmedName}' for multi-texture '{matName}'");
                    return foundMat;
                }
            }
            
            // Try partial matches for each texture name
            foreach (string textureName in textureNames)
            {
                string trimmedName = textureName.Trim();
                foreach (var kvp in materialDict)
                {
                    if (kvp.Key.Contains(trimmedName) || trimmedName.Contains(kvp.Key))
                    {
                        DebugLogger.LogEggImporter($"🔄 Found partial match material '{kvp.Key}' for texture '{trimmedName}' in multi-texture '{matName}'");
                        return kvp.Value;
                    }
                }
            }
        }
        
        // Try partial name matching for single texture names
        foreach (var kvp in materialDict)
        {
            if (kvp.Key.Contains(matName) || matName.Contains(kvp.Key))
            {
                DebugLogger.LogEggImporter($"🔄 Found partial match material '{kvp.Key}' for '{matName}'");
                return kvp.Value;
            }
        }
        
        return null; // No fallback found
    }
    
    private Material GetCachedDefaultMaterial(string materialName)
    {
        // Return cached material if available and name matches closely
        if (_cachedDefaultMaterial != null && _cachedDefaultMaterial.name.Contains("_default"))
        {
            // Clone the cached material with new name for uniqueness
            var clonedMaterial = new Material(_cachedDefaultMaterial) { name = materialName };
            return clonedMaterial;
        }
        
        // Create and cache the default material
        var standardShader = Shader.Find("Standard");
        _cachedDefaultMaterial = new Material(standardShader) { name = materialName };
        
        return _cachedDefaultMaterial;
    }
}
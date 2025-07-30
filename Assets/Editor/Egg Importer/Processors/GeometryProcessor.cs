using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using POTCO.Editor;

public class GeometryProcessor
{
    private ParserUtilities _parserUtils;
    private MaterialHandler _materialHandler;
    
    // Cache commonly used separators to avoid repeated allocations
    private static readonly char[] SpaceSeparator = { ' ' };
    private static readonly char[] SpaceNewlineCarriageReturnSeparators = { ' ', '\n', '\r' };
    
    // Cache for frequently used materials to avoid repeated Shader.Find calls
    private static Material _cachedDefaultMaterial;
    
    public GeometryProcessor()
    {
        _parserUtils = new ParserUtilities();
        _materialHandler = new MaterialHandler();
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

        // Use different approaches for skinned vs static meshes
        Vector3[] meshVertices;
        Vector3[] meshNormals;
        Vector2[] meshUVs;
        Color[] meshColors;
        Dictionary<int, int> globalToLocalMap = null;
        int[] sortedIndices = null;

        if (hasSkeletalData && rootJoint != null && rootBoneObject != null)
        {
            DebugLogger.LogEggImporter($"Using all {masterVertices.Length} master vertices for SKINNED mesh (matching working version approach)");
            meshVertices = masterVertices;
            meshNormals = masterNormals;
            meshUVs = masterUVs;
            meshColors = masterColors;
        }
        else
        {
            DebugLogger.LogEggImporter($"Using optimized local vertices for STATIC mesh");
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
            sortedIndices = usedVertexIndices.OrderBy(x => x).ToArray();
            var localVertices = new Vector3[sortedIndices.Length];
            var localNormals = new Vector3[sortedIndices.Length];
            var localUVs = new Vector2[sortedIndices.Length];
            var localColors = new Color[sortedIndices.Length];

            // Create mapping from global index to local index
            globalToLocalMap = new Dictionary<int, int>(sortedIndices.Length);
            for (int i = 0; i < sortedIndices.Length; i++)
            {
                int globalIndex = sortedIndices[i];
                globalToLocalMap[globalIndex] = i;
                localVertices[i] = masterVertices[globalIndex];
                localNormals[i] = masterNormals[globalIndex];
                localUVs[i] = masterUVs[globalIndex];
                localColors[i] = masterColors[globalIndex];
            }

            meshVertices = localVertices;
            meshNormals = localNormals;
            meshUVs = localUVs;
            meshColors = localColors;
            DebugLogger.LogEggImporter($"Static mesh uses {meshVertices.Length} vertices out of {masterVertices.Length} total vertices");
        }

        // Calculate bounds to fix pivot point based on settings
        var settings = EggImporterSettings.Instance;
        if (meshVertices.Length > 0 && settings.pivotMode != EggImporterSettings.PivotMode.Original)
        {
            Vector3 min = meshVertices[0];
            Vector3 max = meshVertices[0];
            
            foreach (var vertex in meshVertices)
            {
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }
            
            // Calculate pivot offset based on selected mode
            Vector3 pivotOffset = Vector3.zero;
            switch (settings.pivotMode)
            {
                case EggImporterSettings.PivotMode.Center:
                    pivotOffset = (min + max) * 0.5f;
                    break;
                case EggImporterSettings.PivotMode.BottomCenter:
                    pivotOffset = new Vector3(
                        (min.x + max.x) * 0.5f,  // Center X
                        min.y,                    // Bottom Y
                        (min.z + max.z) * 0.5f   // Center Z
                    );
                    break;
                case EggImporterSettings.PivotMode.TopCenter:
                    pivotOffset = new Vector3(
                        (min.x + max.x) * 0.5f,  // Center X
                        max.y,                    // Top Y
                        (min.z + max.z) * 0.5f   // Center Z
                    );
                    break;
            }
            
            // Offset all vertices to place pivot at origin
            for (int i = 0; i < meshVertices.Length; i++)
            {
                meshVertices[i] -= pivotOffset;
            }
            
            // Adjust the GameObject position to compensate
            go.transform.localPosition += pivotOffset;
            
            DebugLogger.LogEggImporter($"Pivot adjustment ({settings.pivotMode}): {pivotOffset}, Bounds: min={min}, max={max}");
        }

        var mesh = new Mesh { name = go.name + "_mesh_" + System.Guid.NewGuid().ToString("N")[..8] };
        mesh.vertices = meshVertices;
        mesh.normals = meshNormals;
        mesh.uv = meshUVs;
        mesh.colors = meshColors;
        mesh.subMeshCount = materialNames.Count;

        // Pre-size materials list to avoid resizing
        var rendererMaterials = new List<Material>(materialNames.Count);

        for (int j = 0; j < materialNames.Count; j++)
        {
            string matName = materialNames[j];
            if (subMeshes.ContainsKey(matName))
            {
                var globalTriangles = subMeshes[matName];
                
                if (hasSkeletalData && rootJoint != null && rootBoneObject != null)
                {
                    // Skinned mesh: use global indices directly
                    DebugLogger.LogEggImporter($"Setting triangles for SKINNED submesh {j} ({matName}): {globalTriangles.Count} triangles (global indices)");
                    mesh.SetTriangles(globalTriangles, j, false);
                }
                else
                {
                    // Static mesh: remap global indices to local indices
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
                    DebugLogger.LogEggImporter($"Setting triangles for STATIC submesh {j} ({matName}): {localTriangles.Count} triangles (remapped from global indices)");
                    mesh.SetTriangles(localTriangles, j, false);
                }
            }
            if (materialDict.TryGetValue(matName, out Material mat))
            {
                rendererMaterials.Add(mat);
                DebugLogger.LogEggImporter($"Added material: {matName}");
            }
            else
            {
                DebugLogger.LogWarningEggImporter($"Material not found: {matName}");
                // Create a default material using cached shader
                var defaultMat = GetCachedDefaultMaterial(matName + "_default");
                rendererMaterials.Add(defaultMat);
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
            SetupSkinnedMeshRenderer(go, mesh, rendererMaterials.ToArray(), ctx, meshVertices, rootJoint, rootBoneObject, joints);
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
        AssetImportContext ctx, Vector3[] masterVertices, EggJoint rootJoint, 
        GameObject rootBoneObject, Dictionary<string, EggJoint> joints)
    {
        var boneWeights = new BoneWeight[masterVertices.Length];
        var bones = new List<Transform>();
        var bindPoses = new List<Matrix4x4>();

        CollectBonesAndBindPoses(rootJoint, bones, bindPoses, rootBoneObject.transform);

        DebugLogger.LogEggImporter($"Collected {bones.Count} bones for skinned mesh");

        if (bones.Count == 0)
        {
            DebugLogger.LogWarningEggImporter("No bones found, falling back to static mesh");
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = materials;
            return;
        }

        // Check for valid bone weights
        int verticesWithWeights = 0;

        for (int i = 0; i < masterVertices.Length; i++)
        {
            var weights = new List<KeyValuePair<int, float>>();
            foreach (var joint in joints.Values)
            {
                if (joint.vertexWeights.ContainsKey(i))
                {
                    int boneIndex = bones.FindIndex(b => b.name == joint.name);
                    if (boneIndex >= 0) { weights.Add(new KeyValuePair<int, float>(boneIndex, joint.vertexWeights[i])); }
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
                // If no weights, bind to root bone
                if (bones.Count > 0)
                {
                    weights.Add(new KeyValuePair<int, float>(0, 1.0f));
                }
            }

            boneWeights[i] = new BoneWeight();
            if (weights.Count > 0) { boneWeights[i].boneIndex0 = weights[0].Key; boneWeights[i].weight0 = weights[0].Value; }
            if (weights.Count > 1) { boneWeights[i].boneIndex1 = weights[1].Key; boneWeights[i].weight1 = weights[1].Value; }
            if (weights.Count > 2) { boneWeights[i].boneIndex2 = weights[2].Key; boneWeights[i].weight2 = weights[2].Value; }
            if (weights.Count > 3) { boneWeights[i].boneIndex3 = weights[3].Key; boneWeights[i].weight3 = weights[3].Value; }
        }

        DebugLogger.LogEggImporter($"Vertices with bone weights: {verticesWithWeights}/{masterVertices.Length}");

        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindPoses.ToArray();

        var skinnedRenderer = go.AddComponent<SkinnedMeshRenderer>();
        skinnedRenderer.sharedMesh = mesh;
        skinnedRenderer.sharedMaterials = materials;
        skinnedRenderer.bones = bones.ToArray();
        skinnedRenderer.rootBone = rootBoneObject.transform;

        // Force update bounds
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


    public void ParseAllTexturesAndVertices(string[] lines, List<EggVertex> vertexPool, Dictionary<string, string> texturePaths, ParserUtilities parserUtils)
    {
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
                        if (innerLine.StartsWith("\"") && innerLine.EndsWith("\"")) { texturePaths[texName] = innerLine.Trim('"'); break; }
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
                        if (attributeLine.StartsWith("<UV>")) { vert.uv = new Vector2(float.Parse(valueParts[0], CultureInfo.InvariantCulture), float.Parse(valueParts[1], CultureInfo.InvariantCulture)); }
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
        
        for (int j = i + 1; j < blockEnd; j++)
        {
            string innerLine = lines[j].Trim();
            if (innerLine.StartsWith("<TRef>")) { polygonTextureRef = innerLine.Substring(innerLine.IndexOf('{') + 1, innerLine.LastIndexOf('}') - innerLine.IndexOf('{') - 1).Trim(); break; }
        }
        if (!subMeshes.ContainsKey(polygonTextureRef)) { subMeshes[polygonTextureRef] = new List<int>(); materialNames.Add(polygonTextureRef); }
        for (int j = i + 1; j < blockEnd; j++)
        {
            string innerLine = lines[j].Trim();
            if (innerLine.StartsWith("<VertexRef>"))
            {
                string valuesString = innerLine.Substring(innerLine.IndexOf('{') + 1, innerLine.LastIndexOf('}') - innerLine.IndexOf('{') - 1).Trim();
                var vRefParts = valuesString.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries).Where(s => int.TryParse(s, out _)).ToArray();
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

    public EggJoint ParseJoint(string[] lines, ref int i, Dictionary<string, EggJoint> joints, ParserUtilities parserUtils)
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

    private Matrix4x4 ParseTransformMatrix(string[] lines, ref int i, ParserUtilities parserUtils)
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
        masterVertices = vertexPool.Select(v => v.position).ToArray();
        masterNormals = vertexPool.Select(v => v.normal).ToArray();
        masterUVs = vertexPool.Select(v => v.uv).ToArray();
        masterColors = vertexPool.Select(v => v.color).ToArray();
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
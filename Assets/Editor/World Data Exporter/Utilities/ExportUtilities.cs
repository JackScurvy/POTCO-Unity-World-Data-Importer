using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldDataExporter.Data;
using POTCO;
using POTCO.Editor;

namespace WorldDataExporter.Utilities
{
    public static class ExportUtilities
    {
        public static ExportStatistics ExportWorldData(ExportSettings settings)
        {
            var startTime = System.DateTime.Now;
            var stats = new ExportStatistics();
            
            DebugLogger.LogWorldExporter($"🚀 Starting world data export to: {settings.outputPath}");
            
            // Collect objects to export based on settings
            List<GameObject> objectsToExport = CollectObjectsToExport(settings);
            DebugLogger.LogWorldExporter($"📊 Found {objectsToExport.Count} objects to export");
            
            // Convert Unity objects to export data structure
            List<ExportedObject> exportedObjects = ConvertUnityObjectsToExportData(objectsToExport, settings, stats);
            
            // Generate Python file
            bool success = PythonFileGenerator.GeneratePythonFile(exportedObjects, settings, stats);
            
            // Clean up any temporary objects that were created from assets
            CleanupTemporaryObjects(objectsToExport);
            
            if (success)
            {
                stats.exportTime = (float)(System.DateTime.Now - startTime).TotalSeconds;
                stats.totalObjectsExported = exportedObjects.Count;
                
                // Calculate file size
                if (System.IO.File.Exists(settings.outputPath))
                {
                    var fileInfo = new System.IO.FileInfo(settings.outputPath);
                    stats.fileSizeKB = fileInfo.Length / 1024f;
                }
                
                DebugLogger.LogWorldExporter($"✅ Export completed successfully in {stats.exportTime:F2} seconds");
                DebugLogger.LogWorldExporter($"📄 Exported {stats.totalObjectsExported} objects to {System.IO.Path.GetFileName(settings.outputPath)}");
            }
            else
            {
                DebugLogger.LogErrorWorldExporter("❌ Export failed!");
                stats.AddWarning("Export failed - check console for details");
            }
            
            return stats;
        }
        
        private static void CleanupTemporaryObjects(List<GameObject> objects)
        {
            foreach (var obj in objects)
            {
                if (obj != null && (obj.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
                {
                    DebugLogger.LogWorldExporter($"🧹 Cleaning up temporary object: '{obj.name}'");
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }
        
        private static List<GameObject> CollectObjectsToExport(ExportSettings settings)
        {
            List<GameObject> objects = new List<GameObject>();
            
            switch (settings.exportSource)
            {
                case ExportSource.EntireScene:
                    // Get all root objects in the scene
                    var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                    foreach (var root in rootObjects)
                    {
                        CollectChildObjects(root, objects);
                    }
                    break;
                    
                case ExportSource.SelectedObjects:
                    // Handle both GameObjects and Project assets
                    var selectedGameObjects = Selection.gameObjects;
                    var selectedAssets = Selection.objects;
                    
                    if (selectedGameObjects.Length > 0)
                    {
                        // Selected GameObjects from scene - export all selected
                        objects.AddRange(selectedGameObjects);
                        DebugLogger.LogWorldExporter($"📌 Exporting {selectedGameObjects.Length} GameObjects from scene");
                    }
                    else if (selectedAssets.Length > 0)
                    {
                        // Selected assets from Project window - create temporary GameObjects for all
                        DebugLogger.LogWorldExporter($"📦 Processing {selectedAssets.Length} selected assets");
                        
                        foreach (var asset in selectedAssets)
                        {
                            GameObject tempObj = CreateTempGameObjectFromAsset(asset);
                            if (tempObj != null)
                            {
                                objects.Add(tempObj);
                                DebugLogger.LogWorldExporter($"✅ Created temporary GameObject from asset: '{asset.name}'");
                            }
                            else
                            {
                                DebugLogger.LogErrorWorldExporter($"❌ Could not create GameObject from asset: '{asset.name}'");
                            }
                        }
                    }
                    break;
                    
                case ExportSource.RootObject:
                    // Export only root level objects (no children)
                    var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                    objects.AddRange(roots);
                    break;
            }
            
            return objects;
        }
        
        private static void CollectChildObjects(GameObject parent, List<GameObject> collection)
        {
            // SIMPLE RULE: Only collect objects with ObjectListInfo that have original IDs
            var potcoInfo = parent.GetComponent<ObjectListInfo>();
            if (potcoInfo != null && !string.IsNullOrEmpty(potcoInfo.objectId))
            {
                // Only collect if ID doesn't contain "export" (generated IDs)
                if (!potcoInfo.objectId.Contains("export"))
                {
                    collection.Add(parent);
                    DebugLogger.LogWorldExporter($"📊 Collected POTCO object: '{parent.name}' (ID: {potcoInfo.objectId})");
                }
                else
                {
                    DebugLogger.LogWorldExporter($"📊 Skipped generated ID: '{parent.name}' (ID: {potcoInfo.objectId})");
                }
            }
            
            // Always recurse to children
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                CollectChildObjects(parent.transform.GetChild(i).gameObject, collection);
            }
        }
        
        private static GameObject CreateTempGameObjectFromAsset(UnityEngine.Object asset)
        {
            try
            {
                // Check if it's a prefab
                if (asset is GameObject prefab)
                {
                    DebugLogger.LogWorldExporter($"🎯 Creating temporary GameObject from prefab: '{asset.name}'");
                    
                    // Create a temporary instance (don't add to scene)
                    GameObject tempObj = UnityEngine.Object.Instantiate(prefab);
                    tempObj.name = asset.name; // Keep original name
                    
                    // Set it as temporary so we can clean it up later
                    tempObj.hideFlags = HideFlags.DontSave;
                    
                    return tempObj;
                }
                
                // Check if it's a mesh asset that we can turn into a GameObject
                if (asset is Mesh mesh)
                {
                    DebugLogger.LogWorldExporter($"🎯 Creating temporary GameObject from mesh: '{asset.name}'");
                    
                    GameObject tempObj = new GameObject(asset.name);
                    tempObj.hideFlags = HideFlags.DontSave;
                    
                    // Add mesh components
                    var meshFilter = tempObj.AddComponent<MeshFilter>();
                    var meshRenderer = tempObj.AddComponent<MeshRenderer>();
                    
                    meshFilter.sharedMesh = mesh;
                    
                    // Try to find a material with the same name
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    string dir = System.IO.Path.GetDirectoryName(assetPath);
                    string materialPath = System.IO.Path.Combine(dir, asset.name + ".mat");
                    
                    var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if (material != null)
                    {
                        meshRenderer.sharedMaterial = material;
                        DebugLogger.LogWorldExporter($"📦 Found and assigned material: '{materialPath}'");
                    }
                    else
                    {
                        // Create a default material
                        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                    }
                    
                    return tempObj;
                }
                
                DebugLogger.LogWarningWorldExporter($"⚠️ Unsupported asset type for '{asset.name}': {asset.GetType()}");
                return null;
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogErrorWorldExporter($"❌ Error creating GameObject from asset '{asset.name}': {ex.Message}");
                return null;
            }
        }
        
        private static List<ExportedObject> ConvertUnityObjectsToExportData(List<GameObject> unityObjects, ExportSettings settings, ExportStatistics stats)
        {
            var exportedObjects = new List<ExportedObject>();
            var objectMap = new Dictionary<GameObject, ExportedObject>();
            
            // First pass: Create ExportedObject for each Unity GameObject
            foreach (var unityObj in unityObjects)
            {
                DebugLogger.LogWorldExporter($"🔄 Processing Unity object: '{unityObj.name}'");
                
                var exportedObj = ConvertUnityObject(unityObj, settings, stats);
                if (exportedObj != null)
                {
                    DebugLogger.LogWorldExporter($"✅ Created ExportedObject for: '{unityObj.name}' (type: {exportedObj.objectType})");
                    
                    if (ShouldExportObject(exportedObj, settings))
                    {
                        exportedObjects.Add(exportedObj);
                        objectMap[unityObj] = exportedObj;
                        
                        // Update statistics
                        stats.AddObjectType(exportedObj.objectType ?? "Unknown");
                        stats.exportedObjectIds.Add(exportedObj.id);
                        
                        if (exportedObj.IsLightObject()) stats.lightingObjectsExported++;
                        if (exportedObj.IsCollisionObject()) stats.collisionObjectsExported++;
                        if (exportedObj.IsNodeObject()) stats.nodeObjectsExported++;
                        
                        DebugLogger.LogWorldExporter($"🎯 Final export: '{unityObj.name}' -> '{exportedObj.objectType}'");
                    }
                    else
                    {
                        DebugLogger.LogWarningWorldExporter($"❌ Object '{unityObj.name}' created but filtered out by ShouldExportObject");
                    }
                }
                else
                {
                    DebugLogger.LogWarningWorldExporter($"❌ Failed to create ExportedObject for: '{unityObj.name}'");
                }
            }
            
            // Second pass: Establish parent-child relationships
            if (settings.preserveHierarchy)
            {
                foreach (var kvp in objectMap)
                {
                    var unityObj = kvp.Key;
                    var exportedObj = kvp.Value;
                    
                    // Set parent relationship
                    if (unityObj.transform.parent != null && objectMap.ContainsKey(unityObj.transform.parent.gameObject))
                    {
                        var parentExported = objectMap[unityObj.transform.parent.gameObject];
                        parentExported.AddChild(exportedObj);
                    }
                }
            }
            
            return exportedObjects;
        }
        
        private static ExportedObject ConvertUnityObject(GameObject unityObj, ExportSettings settings, ExportStatistics stats)
        {
            // Simple check: Only convert objects with ObjectListInfo
            var potcoInfo = unityObj.GetComponent<ObjectListInfo>();
            DebugLogger.LogWorldExporter($"🔍 Checking '{unityObj.name}': ObjectListInfo component = {(potcoInfo != null ? "FOUND" : "NOT FOUND")}");
            
            if (potcoInfo != null)
            {
                DebugLogger.LogWorldExporter($"🔍 ObjectListInfo details: objectId='{potcoInfo.objectId}', objectType='{potcoInfo.objectType}', modelPath='{potcoInfo.modelPath}'");
            }
            
            if (potcoInfo == null || string.IsNullOrEmpty(potcoInfo.objectId))
            {
                DebugLogger.LogWorldExporter($"⏭️ Skipping '{unityObj.name}' - {(potcoInfo == null ? "no ObjectListInfo component" : "empty objectId")}");
                return null;
            }
            
            // Use existing POTCO ID (we already verified it exists)
            string objectId = potcoInfo.objectId;
            DebugLogger.LogWorldExporter($"📋 Using POTCO ID: {objectId}");
                
            var exportedObj = new ExportedObject(objectId);
            
            // Basic properties - Name should be empty for Building Interior objects
            if (potcoInfo.objectType == "Building Interior")
            {
                exportedObj.name = ""; // Building Interior objects have empty Name in POTCO files
            }
            else
            {
                exportedObj.name = unityObj.name;
            }
            
            // Transform data - always convert coordinates to Panda3D format
            var transform = unityObj.transform;
            exportedObj.position = CoordinateConverter.UnityToPanda3DPosition(transform.localPosition);
            exportedObj.rotation = CoordinateConverter.UnityToPanda3DHPR(transform.localEulerAngles);
            exportedObj.scale = CoordinateConverter.UnityToPanda3DScale(transform.localScale);
            
            // Use data from ObjectListInfo (we already verified it exists)
            exportedObj.objectType = potcoInfo.objectType;
            exportedObj.modelPath = potcoInfo.modelPath;
            exportedObj.visualColor = potcoInfo.visualColor;
            exportedObj.disableCollision = potcoInfo.disableCollision;
            exportedObj.instanced = potcoInfo.instanced;
            exportedObj.holiday = potcoInfo.holiday;
            exportedObj.visSize = potcoInfo.visSize;
            
            DebugLogger.LogWorldExporter($"📋 Using ObjectListInfo data: Type='{potcoInfo.objectType}', Model='{potcoInfo.modelPath}'");
            
            // Model path should already be set from ObjectListInfo
            if (string.IsNullOrEmpty(exportedObj.modelPath))
            {
                DebugLogger.LogWarningWorldExporter($"⚠️ No model path in ObjectListInfo for '{unityObj.name}' - this may cause export issues");
            }
            
            // Visual color should already be set from ObjectListInfo (if needed)
            
            // Extract lighting properties if this is a light object
            if (exportedObj.IsLightObject())
            {
                ExtractLightingProperties(unityObj, exportedObj);
            }
            
            // All POTCO properties already extracted from ObjectListInfo
            
            return exportedObj;
        }
        
        // Removed bloated ShouldExportAsPOTCOObject method - now using simple collection logic
        
        private static bool ShouldHaveModel(string objectType, GameObject unityObj)
        {
            // Container/organizational objects that don't have models
            if (string.IsNullOrEmpty(objectType))
                return false;
                
            // These object types are containers and don't have Visual.Model properties
            var containerTypes = new[] {
                "Ship Part",
                "Connector Tunnel",
                "Connector Door"
            };
            
            foreach (var containerType in containerTypes)
            {
                if (objectType.Equals(containerType, StringComparison.OrdinalIgnoreCase))
                {
                    DebugLogger.LogWorldExporter($"📦 Container object '{objectType}' - no model needed");
                    return false;
                }
            }
            
            // Node objects typically don't have visual models (they use editor representations)
            if (objectType.Contains("Node"))
            {
                DebugLogger.LogWorldExporter($"📍 Node object '{objectType}' - no model needed");
                return false;
            }
            
            // Objects without mesh renderers might still need models (they could be parent containers)
            // Only skip if they have no children AND no mesh components
            if (unityObj.GetComponent<MeshRenderer>() == null && 
                unityObj.GetComponent<MeshFilter>() == null &&
                unityObj.transform.childCount == 0)
            {
                DebugLogger.LogWorldExporter($"🚫 No mesh components and no children on '{objectType}' - no model needed");
                return false;
            }
            
            return true;
        }
        
        private static string DetermineObjectType(GameObject unityObj)
        {
            DebugLogger.LogWorldExporter($"🎯 Determining object type for: '{unityObj.name}'");
            
            // Get the model name from the GameObject or its children
            string modelName = FindModelNameFromGameObject(unityObj);
            DebugLogger.LogWorldExporter($"🔍 Extracted model name: '{modelName}' from GameObject: '{unityObj.name}'");
            
            if (!string.IsNullOrEmpty(modelName))
            {
                DebugLogger.LogWorldExporter($"🔍 Looking up type for model: '{modelName}'");
                
                // Look up the model in ObjectList.py
                string objectType = FindObjectTypeByExactModelName(modelName);
                if (!string.IsNullOrEmpty(objectType))
                {
                    DebugLogger.LogWorldExporter($"✅ Found exact type: '{modelName}' -> '{objectType}'");
                    return objectType;
                }
                else
                {
                    DebugLogger.LogWarningWorldExporter($"⚠️ No ObjectList.py match found for model: '{modelName}'");
                }
            }
            else
            {
                DebugLogger.LogWarningWorldExporter($"⚠️ Could not extract model name from GameObject: '{unityObj.name}'");
            }
            
            // Check for Light component
            if (unityObj.GetComponent<Light>() != null)
            {
                DebugLogger.LogWorldExporter($"💡 '{unityObj.name}' identified as Light by component");
                return "Light - Dynamic";
            }
            
            // Basic name pattern fallbacks for non-model objects
            string name = unityObj.name.ToLower();
            
            if (name.Contains("collision") && name.Contains("barrier"))
            {
                DebugLogger.LogWorldExporter($"🚧 '{unityObj.name}' identified as Collision Barrier by name pattern");
                return "Collision Barrier";
            }
            
            if (name.Contains("spawn") && name.Contains("node"))
            {
                DebugLogger.LogWorldExporter($"📍 '{unityObj.name}' identified as Spawn Node by name pattern");
                return "Spawn Node";
            }
            
            if (name.Contains("node"))
            {
                DebugLogger.LogWorldExporter($"📍 '{unityObj.name}' identified as Locator Node by name pattern");
                return "Locator Node";
            }
                
            if (name.Contains("townsperson"))
            {
                DebugLogger.LogWorldExporter($"👤 '{unityObj.name}' identified as Townsperson by name pattern");
                return "Townsperson";
            }
            
            // Default fallback
            DebugLogger.LogWarningWorldExporter($"⚠️ No POTCO definition found for '{unityObj.name}', defaulting to Prop");
            return "Prop";
        }
        
        private static string FindObjectTypeByExactModelName(string modelName)
        {
            DebugLogger.LogWorldExporter($"🔍 FindObjectTypeByExactModelName called with: '{modelName}'");
            
            // Simple lookup using the model-to-type map
            string objectType = ObjectListParser.GetObjectTypeByModelName(modelName);
            
            if (!string.IsNullOrEmpty(objectType))
            {
                DebugLogger.LogWorldExporter($"✅ Found exact match: '{modelName}' -> '{objectType}'");
                return objectType;
            }
            
            DebugLogger.LogWorldExporter($"❌ No match found for model: '{modelName}'");
            return null;
        }
        
        private static string FindObjectTypeByModelName(string modelName)
        {
            var objectDefinitions = ObjectListParser.GetObjectDefinitions();
            
            // First, clean the model name - extract just the base name
            string cleanModelName = ExtractBaseModelName(modelName);
            DebugLogger.LogWorldExporter($"🧹 Searching for object type with cleaned model name: '{cleanModelName}' (original: '{modelName}')");
            
            // Search through all object definitions for this model name
            foreach (var kvp in objectDefinitions)
            {
                var objectType = kvp.Key;
                var definition = kvp.Value;
                
                // Check each model in the Visual.Models array
                foreach (var modelPath in definition.visual.models)
                {
                    // Extract just the model filename from the path
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(modelPath);
                    
                    // Check for exact match with cleaned name
                    if (cleanModelName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        DebugLogger.LogWorldExporter($"🔍 Exact model match: '{cleanModelName}' found in '{objectType}' models list");
                        return objectType;
                    }
                    
                    // Check for partial match with cleaned name
                    if (cleanModelName.ToLower().Contains(fileName.ToLower()) || 
                        fileName.ToLower().Contains(cleanModelName.ToLower()))
                    {
                        DebugLogger.LogWorldExporter($"🔍 Partial model match: '{cleanModelName}' ~= '{fileName}' in '{objectType}' models list");
                        return objectType;
                    }
                }
            }
            
            return null; // No match found
        }
        
        private static string ExtractBaseModelName(string fullName)
        {
            // Only extract base name if it looks like there's an object ID suffix
            // Pattern: "ModelName_1165269209.69kmuller" -> "ModelName"
            if (fullName.Contains("_"))
            {
                string[] parts = fullName.Split('_');
                if (parts.Length >= 2)
                {
                    // Check if the last part looks like an object ID (starts with digits)
                    string lastPart = parts[parts.Length - 1];
                    if (System.Text.RegularExpressions.Regex.IsMatch(lastPart, @"^\d+\.\d+[a-zA-Z]+\d*$"))
                    {
                        // It's an object ID suffix, remove it
                        string baseName = string.Join("_", parts.Take(parts.Length - 1));
                        DebugLogger.LogWorldExporter($"🔍 Extracted base model name: '{baseName}' from '{fullName}' (removed object ID suffix)");
                        return baseName;
                    }
                }
            }
            
            // Handle names with dots like "bottle.red" -> "bottle" (but not object IDs)
            if (fullName.Contains(".") && !IsObjectId(fullName))
            {
                string baseName = fullName.Split('.')[0];
                DebugLogger.LogWorldExporter($"🔍 Extracted base model name: '{baseName}' from '{fullName}' (removed extension)");
                return baseName;
            }
            
            // Return full name if no extraction needed
            return fullName;
        }
        
        private static string ExtractModelPath(GameObject unityObj)
        {
            DebugLogger.LogWorldExporter($"🔍 Extracting model for '{unityObj.name}'");
            
            // For temporary assets from Project window, prioritize object name-based paths
            bool isTemporaryAsset = (unityObj.hideFlags & HideFlags.DontSave) == HideFlags.DontSave;
            
            // First, try to find the actual model name from the GameObject or its children
            string modelName = FindModelNameFromGameObject(unityObj);
            if (!string.IsNullOrEmpty(modelName))
            {
                DebugLogger.LogWorldExporter($"🎯 Found model name from GameObject: '{modelName}'");
                
                // Try to find the exact model path that matches the model name
                var potcoDefinition = ObjectListParser.FindBestMatchingType(modelName, unityObj);
                if (potcoDefinition != null)
                {
                    // Look for the specific model that matches our name
                    var availableModels = potcoDefinition.GetAvailableModels();
                    foreach (var model in availableModels)
                    {
                        string modelFileName = System.IO.Path.GetFileNameWithoutExtension(model);
                        if (modelFileName.Equals(modelName, StringComparison.OrdinalIgnoreCase))
                        {
                            DebugLogger.LogWorldExporter($"📦 Found exact model match: {model}");
                            return model;
                        }
                    }
                    
                    // Fallback to default model if no exact match
                    string defaultModel = potcoDefinition.GetDefaultModel();
                    if (!string.IsNullOrEmpty(defaultModel))
                    {
                        DebugLogger.LogWorldExporter($"📦 Using POTCO default model as fallback: {defaultModel}");
                        return defaultModel;
                    }
                }
                
                // Create model path from found name
                return $"models/props/{modelName}";
            }
            
            // Fallback: try to get the POTCO definition using GameObject name
            var potcoDefFallback = ObjectListParser.FindBestMatchingType(unityObj.name, unityObj);
            DebugLogger.LogWorldExporter($"🔍 POTCO definition found: {potcoDefFallback?.objectType ?? "null"}");
            
            if (potcoDefFallback != null)
            {
                string defaultModel = potcoDefFallback.GetDefaultModel();
                if (!string.IsNullOrEmpty(defaultModel))
                {
                    DebugLogger.LogWorldExporter($"📦 Using POTCO default model for '{unityObj.name}': {defaultModel}");
                    return defaultModel;
                }
            }
            
            // For temporary assets, create model path from object name
            if (isTemporaryAsset && !IsObjectId(unityObj.name))
            {
                // Extract the base model name (e.g., "Crate" from "Crate_1165269209.69kmuller")
                string baseModelName = ExtractBaseModelName(unityObj.name);
                string modelPath = $"models/props/{baseModelName}";
                DebugLogger.LogWorldExporter($"🎯 Created model path for temporary asset: {modelPath} (from '{unityObj.name}')");
                return modelPath;
            }
            
            // Try to get model path from Unity asset path
            var meshRenderer = unityObj.GetComponent<MeshRenderer>();
            var meshFilter = unityObj.GetComponent<MeshFilter>();
            
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    // Convert Unity asset path to POTCO model path format
                    // Remove "Assets/Resources/" and file extension
                    assetPath = assetPath.Replace("Assets/Resources/", "");
                    assetPath = assetPath.Replace(".fbx", "").Replace(".prefab", "").Replace(".asset", "").Replace(".egg", "");
                    
                    // Don't use object IDs as model paths - they should be actual model names
                    string fileName = System.IO.Path.GetFileName(assetPath);
                    if (IsObjectId(fileName))
                    {
                        DebugLogger.LogWarningWorldExporter($"⚠️ Skipping object ID as model path: {fileName}");
                    }
                    else
                    {
                        return assetPath;
                    }
                }
            }
            
            // For Building Interior objects, try to infer the correct building model
            if (potcoDefFallback != null && potcoDefFallback.objectType == "Building Interior")
            {
                // Building Interior objects should have specific building models
                string name = unityObj.name.ToLower();
                if (name.Contains("tavern"))
                    return "models/buildings/interior_tavern";
                if (name.Contains("jail"))
                    return "models/buildings/interior_jail";
                if (name.Contains("blacksmith"))
                    return "models/buildings/interior_blacksmith";
                // Add more building types as needed
                
                // Default to generic interior if no specific match
                return "models/buildings/interior_generic";
            }
            
            // Fallback: try to infer from object name (but avoid object IDs)
            string objName = unityObj.name;
            if (!IsObjectId(objName))
            {
                // Extract base model name for fallback
                string baseModelName = ExtractBaseModelName(objName);
                
                // For objects classified as "Prop", try to create reasonable model paths
                if (potcoDefFallback?.objectType == "Prop" || string.IsNullOrEmpty(potcoDefFallback?.objectType))
                {
                    // Check if this object has any visual components that suggest it should have a model
                    bool hasVisualComponents = unityObj.GetComponent<MeshRenderer>() != null || 
                                             unityObj.GetComponent<MeshFilter>() != null ||
                                             unityObj.transform.childCount == 0; // Leaf objects often have models
                    
                    if (hasVisualComponents)
                    {
                        DebugLogger.LogWorldExporter($"🎯 Creating fallback model path for '{objName}' -> models/props/{baseModelName}");
                        return $"models/props/{baseModelName}";
                    }
                }
                else
                {
                    // For non-Prop objects, still try to create model paths
                    DebugLogger.LogWorldExporter($"🎯 Creating fallback model path for non-Prop '{objName}' -> models/props/{baseModelName}");
                    return $"models/props/{baseModelName}";
                }
            }
            
            // If we can't determine a proper model path, return null to avoid fake paths
            DebugLogger.LogWarningWorldExporter($"⚠️ Cannot determine model path for '{unityObj.name}' - no model will be exported");
            return null;
        }
        
        private static bool IsObjectId(string name)
        {
            // POTCO object IDs are typically in format: timestamp.sequenceusername
            // Like: 1153419689.81dzlu00
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^\d+\.\d+[a-zA-Z]+\d*$");
        }
        
        private static bool IsValidModelName(string name)
        {
            DebugLogger.LogWorldExporter($"🔍 Validating model name: '{name}'");
            
            // Filter out generic or system names that aren't actual POTCO models
            string[] invalidNames = {
                "Holiday", "Christmas", "Halloween", "Event",
                "GameObject", "Model", "Mesh", "Prefab",
                "Group", "Container", "Root", "Parent", "Child",
                "Transform", "Empty", "Null", "Node",
                "Test", "Debug", "Temp", "tmp", "WIP",
                "LOD", "Collider", "Trigger", "Zone"
            };
            
            foreach (string invalid in invalidNames)
            {
                if (name.Equals(invalid, StringComparison.OrdinalIgnoreCase))
                {
                    DebugLogger.LogWorldExporter($"❌ Rejecting invalid model name: '{name}'");
                    return false;
                }
            }
            
            // Simple lookup - if it's in ObjectList.py, it's valid
            string objectType = ObjectListParser.GetObjectTypeByModelName(name);
            bool isValid = !string.IsNullOrEmpty(objectType);
            
            if (isValid)
            {
                DebugLogger.LogWorldExporter($"✅ Valid POTCO model name: '{name}' -> '{objectType}'");
            }
            else
            {
                DebugLogger.LogWorldExporter($"❌ Model name '{name}' not found in ObjectList.py - rejecting");
            }
            
            return isValid;
        }
        
        private static string FindModelNameFromGameObject(GameObject obj)
        {
            DebugLogger.LogWorldExporter($"🔍 FindModelNameFromGameObject called for: '{obj.name}'");
            
            // First, check the GameObject's own name
            string cleanName = ExtractBaseModelName(obj.name);
            DebugLogger.LogWorldExporter($"🔍 Extracted clean name: '{cleanName}' from '{obj.name}'");
            if (!IsObjectId(cleanName) && !string.IsNullOrEmpty(cleanName) && cleanName != "GameObject" && IsValidModelName(cleanName))
            {
                DebugLogger.LogWorldExporter($"📍 Using GameObject's own name: '{cleanName}'");
                return cleanName;
            }
            
            // Check if this object has a prefab connection
            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            if (prefabAsset != null)
            {
                string prefabName = ExtractBaseModelName(prefabAsset.name);
                if (IsValidModelName(prefabName))
                {
                    DebugLogger.LogWorldExporter($"🎯 Found prefab source name: '{prefabName}'");
                    return prefabName;
                }
            }
            
            // Search through children for the first child with a mesh
            MeshFilter[] childMeshFilters = obj.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in childMeshFilters)
            {
                if (meshFilter.sharedMesh != null)
                {
                    // Try to get name from mesh
                    string meshName = ExtractBaseModelName(meshFilter.sharedMesh.name);
                    if (!IsObjectId(meshName) && !string.IsNullOrEmpty(meshName) && IsValidModelName(meshName))
                    {
                        DebugLogger.LogWorldExporter($"🎯 Found mesh name in child: '{meshName}'");
                        return meshName;
                    }
                    
                    // Try to get name from the child GameObject that has the mesh
                    string childName = ExtractBaseModelName(meshFilter.gameObject.name);
                    if (!IsObjectId(childName) && !string.IsNullOrEmpty(childName) && childName != "GameObject" && IsValidModelName(childName))
                    {
                        DebugLogger.LogWorldExporter($"🎯 Found child GameObject with mesh: '{childName}'");
                        return childName;
                    }
                }
            }
            
            // Last resort: look for any child with a recognizable name
            Transform[] allChildren = obj.GetComponentsInChildren<Transform>();
            foreach (var child in allChildren)
            {
                if (child == obj.transform) continue; // Skip self
                
                string childName = ExtractBaseModelName(child.name);
                if (!IsObjectId(childName) && !string.IsNullOrEmpty(childName) && 
                    childName != "GameObject" && !childName.StartsWith("unity") && IsValidModelName(childName))
                {
                    DebugLogger.LogWorldExporter($"🎯 Found recognizable child name: '{childName}'");
                    return childName;
                }
            }
            
            DebugLogger.LogWarningWorldExporter($"⚠️ Could not find model name for GameObject '{obj.name}'");
            return null;
        }
        
        private static Color? ExtractVisualColor(GameObject unityObj)
        {
            // First, try to get color from Unity material
            var renderer = unityObj.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                var material = renderer.sharedMaterial;
                if (material.HasProperty("_Color"))
                {
                    return material.color;
                }
            }
            
            // Use POTCO default color if available
            var potcoDefinition = ObjectListParser.FindBestMatchingType(unityObj.name, unityObj);
            if (potcoDefinition != null && potcoDefinition.visual.color.HasValue)
            {
                DebugLogger.LogWorldExporter($"🎨 Using POTCO default color for '{unityObj.name}': {potcoDefinition.visual.color.Value}");
                return potcoDefinition.visual.color.Value;
            }
            
            return null;
        }
        
        private static void ExtractLightingProperties(GameObject unityObj, ExportedObject exportedObj)
        {
            var light = unityObj.GetComponent<Light>();
            if (light == null) return;
            
            // Get POTCO light definition for defaults
            var potcoDefinition = ObjectListParser.GetObjectDefinition("Light - Dynamic");
            
            // Map Unity light type to POTCO light type
            switch (light.type)
            {
                case LightType.Point:
                    exportedObj.lightType = "POINT";
                    break;
                case LightType.Spot:
                    exportedObj.lightType = "SPOT";
                    exportedObj.coneAngle = light.spotAngle;
                    break;
                case LightType.Directional:
                    exportedObj.lightType = "DIRECTIONAL";
                    break;
                default:
                    exportedObj.lightType = "POINT";
                    break;
            }
            
            // Use Unity values if available, otherwise use POTCO defaults
            exportedObj.intensity = light.intensity / 2f; // Convert back from Unity range to POTCO range
            exportedObj.attenuation = 1f / (light.range / 10f); // Reverse the attenuation calculation
            exportedObj.visualColor = light.color;
            
            // Check for LightFlicker component
            var flicker = unityObj.GetComponent<LightFlicker>();
            if (flicker != null)
            {
                exportedObj.flickering = true;
                exportedObj.flickRate = flicker.flickRate;
            }
            else
            {
                // Use POTCO defaults for flickering
                if (potcoDefinition != null)
                {
                    var flickeringDefault = potcoDefinition.GetDefaultValue("Flickering");
                    var flickRateDefault = potcoDefinition.GetDefaultValue("FlickRate");
                    
                    exportedObj.flickering = flickeringDefault is bool b ? b : false;
                    exportedObj.flickRate = flickRateDefault is float f ? f : 0.5f;
                    
                    DebugLogger.LogWorldExporter($"💡 Applied POTCO lighting defaults to '{unityObj.name}': Flickering={exportedObj.flickering}, FlickRate={exportedObj.flickRate}");
                }
                else
                {
                    exportedObj.flickering = false;
                    exportedObj.flickRate = 0.5f;
                }
            }
            
            // Apply other POTCO defaults if not set
            if (potcoDefinition != null)
            {
                if (!exportedObj.coneAngle.HasValue)
                {
                    var coneAngleDefault = potcoDefinition.GetDefaultValue("ConeAngle");
                    if (coneAngleDefault is string coneStr && float.TryParse(coneStr, out float cone))
                    {
                        exportedObj.coneAngle = cone;
                    }
                }
                
                if (!exportedObj.dropOff.HasValue)
                {
                    var dropOffDefault = potcoDefinition.GetDefaultValue("DropOff");
                    if (dropOffDefault is string dropStr && float.TryParse(dropStr, out float drop))
                    {
                        exportedObj.dropOff = drop;
                    }
                }
                
                if (!exportedObj.attenuation.HasValue)
                {
                    var attenuationDefault = potcoDefinition.GetDefaultValue("Attenuation");
                    if (attenuationDefault is string attStr && float.TryParse(attStr, out float att))
                    {
                        exportedObj.attenuation = att;
                    }
                }
            }
        }
        
        private static void ExtractPOTCOProperties(GameObject unityObj, ExportedObject exportedObj)
        {
            string name = unityObj.name;
            
            // Check for holiday indicators in name
            if (name.Contains("holiday") || name.Contains("_hol_"))
            {
                // Extract holiday name if possible
                if (name.Contains("christmas")) exportedObj.holiday = "Christmas";
                else if (name.Contains("halloween")) exportedObj.holiday = "Halloween";
                else exportedObj.holiday = "Holiday";
            }
            
            // Check for instanced indicator
            if (name.Contains("instanced") || name.ToLower().Contains("_inst"))
            {
                exportedObj.instanced = true;
            }
            
            // Set DisableCollision property (most POTCO objects have this)
            var colliders = unityObj.GetComponents<Collider>();
            if (colliders.Length > 0)
            {
                // If any collider is disabled, mark as disabled
                exportedObj.disableCollision = colliders.Any(c => !c.enabled);
            }
            else
            {
                // Default to False if no colliders (matching POTCO pattern)
                exportedObj.disableCollision = false;
            }
        }
        
        private static bool ShouldExportObject(ExportedObject obj, ExportSettings settings)
        {
            // Get POTCO definition for additional filtering info
            var potcoDefinition = ObjectListParser.GetObjectDefinition(obj.objectType);
            
            // Filter based on settings using POTCO definition data
            if (!settings.exportLighting && obj.IsLightObject())
                return false;
                
            if (!settings.exportCollisions && obj.IsCollisionObject())
                return false;
                
            if (!settings.exportNodes && (obj.IsNodeObject() || (potcoDefinition?.nonRpmNode == true)))
                return false;
                
            if (!settings.exportHolidayObjects && obj.IsHolidayObject())
                return false;
            
            // Check exclude/include lists
            if (settings.excludeObjectTypes.Count > 0 && 
                settings.excludeObjectTypes.Contains(obj.objectType))
                return false;
                
            if (settings.includeObjectTypes.Count > 0 && 
                !settings.includeObjectTypes.Contains(obj.objectType))
                return false;
            
            DebugLogger.LogWorldExporter($"✅ Exporting object: '{obj.objectType}' - '{obj.name}'");
            return true;
        }
    }
}
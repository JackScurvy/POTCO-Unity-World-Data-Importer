using UnityEngine;
using System.Linq;
using POTCO;
using WorldDataExporter.Utilities;

namespace POTCO.Editor
{
    /// <summary>
    /// Editor-only integration with ObjectListParser for ObjectListInfo
    /// </summary>
    public static class ObjectListIntegration
    {
        /// <summary>
        /// Detect object type using ObjectListParser
        /// </summary>
        public static string DetectObjectTypeFromObjectList(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                DebugLogger.LogAutoObjectList("❌ Model name is empty");
                return "MISC_OBJ";
            }
            
            DebugLogger.LogAutoObjectList($"🔍 Looking up model '{modelName}' in ObjectList.py");
            
            try
            {
                // Try exact match first
                string result = ObjectListParser.GetObjectTypeByModelName(modelName);
                DebugLogger.LogAutoObjectList($"🔍 ObjectListParser.GetObjectTypeByModelName('{modelName}') returned: '{result}'");
                if (!string.IsNullOrEmpty(result) && result != "MISC_OBJ")
                {
                    DebugLogger.LogAutoObjectList($"✅ Exact match found: '{modelName}' -> '{result}'");
                    return result;
                }
                
                // Try without prefixes (pir_m_prp_ etc.)
                string cleanName = CleanModelName(modelName);
                if (cleanName != modelName)
                {
                    DebugLogger.LogAutoObjectList($"🧹 Trying cleaned name: '{cleanName}'");
                    result = ObjectListParser.GetObjectTypeByModelName(cleanName);
                    DebugLogger.LogAutoObjectList($"🔍 ObjectListParser.GetObjectTypeByModelName('{cleanName}') returned: '{result}'");
                    if (!string.IsNullOrEmpty(result) && result != "MISC_OBJ")
                    {
                        DebugLogger.LogAutoObjectList($"✅ Cleaned match found: '{cleanName}' -> '{result}'");
                        return result;
                    }
                }
                
                // Try searching through all definitions manually
                DebugLogger.LogAutoObjectList($"🔍 Trying manual search through all object definitions...");
                var definitions = ObjectListParser.GetObjectDefinitions();
                DebugLogger.LogAutoObjectList($"📊 Searching through {definitions.Count} object type definitions");
                
                foreach (var kvp in definitions)
                {
                    foreach (var model in kvp.Value.visual.models)
                    {
                        string modelFileName = System.IO.Path.GetFileNameWithoutExtension(model);
                        if (modelFileName.Equals(modelName, System.StringComparison.OrdinalIgnoreCase) ||
                            modelFileName.Equals(cleanName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            DebugLogger.LogAutoObjectList($"✅ Manual search found: '{modelFileName}' in '{kvp.Key}' models list");
                            return kvp.Key;
                        }
                    }
                }
                
                DebugLogger.LogAutoObjectList($"❌ No match found for '{modelName}' in ObjectList.py");
                return "MISC_OBJ";
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogErrorAutoObjectList($"❌ Error accessing ObjectList: {ex.Message}");
                return "MISC_OBJ";
            }
        }
        
        /// <summary>
        /// Clean model name by removing common prefixes and suffixes
        /// </summary>
        private static string CleanModelName(string modelName)
        {
            if (string.IsNullOrEmpty(modelName)) return "";
            
            // Remove common POTCO prefixes
            if (modelName.StartsWith("pir_m_prp_")) modelName = modelName.Substring(10);
            if (modelName.StartsWith("pir_m_")) modelName = modelName.Substring(6);
            if (modelName.StartsWith("pir_")) modelName = modelName.Substring(4);
            
            // Remove common suffixes
            if (modelName.EndsWith("_lod")) modelName = modelName.Substring(0, modelName.Length - 4);
            if (modelName.EndsWith("_low")) modelName = modelName.Substring(0, modelName.Length - 4);
            if (modelName.EndsWith("_high")) modelName = modelName.Substring(0, modelName.Length - 5);
            
            return modelName;
        }
        
        /// <summary>
        /// Auto-detect all properties for a ObjectListInfo component
        /// </summary>
        public static void AutoDetectAllProperties(ObjectListInfo objectListInfo)
        {
            if (objectListInfo == null) 
            {
                DebugLogger.LogErrorAutoObjectList("❌ ObjectListInfo is null in AutoDetectAllProperties");
                return;
            }
            
            DebugLogger.LogAutoObjectList($"🎯 Auto-detecting properties for '{objectListInfo.gameObject.name}'");
            
            // Generate object ID if missing
            if (objectListInfo.autoGenerateId && string.IsNullOrEmpty(objectListInfo.objectId))
            {
                objectListInfo.GenerateObjectId();
                DebugLogger.LogAutoObjectList($"🆔 Generated Object ID: {objectListInfo.objectId}");
            }
            
            // Auto-detect model path if missing or always refresh it
            string detectedPath = DetectModelPath(objectListInfo.gameObject);
            if (!string.IsNullOrEmpty(detectedPath))
            {
                objectListInfo.modelPath = detectedPath;
                DebugLogger.LogAutoObjectList($"📁 Detected model path: {detectedPath}");
            }
            else
            {
                DebugLogger.LogWarningAutoObjectList($"⚠️ Could not detect model path for '{objectListInfo.gameObject.name}'");
            }
            
            // Auto-detect object type from model path
            if (!string.IsNullOrEmpty(objectListInfo.modelPath))
            {
                string modelName = System.IO.Path.GetFileNameWithoutExtension(objectListInfo.modelPath);
                DebugLogger.LogAutoObjectList($"🔍 Extracted model name '{modelName}' from path '{objectListInfo.modelPath}'");
                
                string detectedType = DetectObjectTypeFromObjectList(modelName);
                objectListInfo.objectType = detectedType;
                DebugLogger.LogAutoObjectList($"🏷️ Detected object type: {detectedType}");
            }
            else
            {
                DebugLogger.LogWarningAutoObjectList($"⚠️ No model path available for object type detection on '{objectListInfo.gameObject.name}'");
                objectListInfo.objectType = "MISC_OBJ"; // fallback
            }
            
            DebugLogger.LogAutoObjectList($"✅ Auto-detection complete for '{objectListInfo.gameObject.name}' - Type: '{objectListInfo.objectType}', Path: '{objectListInfo.modelPath}'");
        }
        
        /// <summary>
        /// Detect model path from GameObject by searching Resources folder
        /// </summary>
        private static string DetectModelPath(GameObject gameObject)
        {
            string modelName = ExtractModelNameFromGameObject(gameObject);
            if (string.IsNullOrEmpty(modelName))
            {
                return "";
            }
            
            // Search through Resources/phase_* folders to find the actual model location
            string foundPath = SearchForModelInResources(modelName);
            if (!string.IsNullOrEmpty(foundPath))
            {
                DebugLogger.LogAutoObjectList($"📁 Found model in Resources: '{foundPath}'");
                return foundPath;
            }
            
            // Fallback to pattern-based detection if not found in Resources
            string category = DetectModelCategory(modelName);
            string fallbackPath = $"models/{category}/{modelName}";
            DebugLogger.LogAutoObjectList($"📁 Model not found in Resources, using fallback: '{fallbackPath}'");
            return fallbackPath;
        }
        
        /// <summary>
        /// Search for model file in Resources/phase_* folders (fast, limited search)
        /// </summary>
        private static string SearchForModelInResources(string modelName)
        {
            try
            {
                // Quick check - if this is taking too long, skip it
                var startTime = System.DateTime.Now;
                
                // Get the path to Resources folder
                string resourcesPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                if (!System.IO.Directory.Exists(resourcesPath))
                {
                    return "";
                }
                
                // Common model categories to check first (most likely locations)
                string[] commonCategories = { "caves", "props", "buildings", "char", "effects", "gui" };
                string[] extensions = { ".fbx", ".prefab", ".egg" };
                
                // Search through phase_* folders, but limit search scope
                string[] phaseFolders = System.IO.Directory.GetDirectories(resourcesPath, "phase_*");
                foreach (string phaseFolder in phaseFolders)
                {
                    // Check if we're taking too long (timeout after 100ms)
                    if ((System.DateTime.Now - startTime).TotalMilliseconds > 100)
                    {
                        DebugLogger.LogAutoObjectList($"⏱️ Model search timeout for '{modelName}', using fallback");
                        break;
                    }
                    
                    string modelsPath = System.IO.Path.Combine(phaseFolder, "models");
                    if (!System.IO.Directory.Exists(modelsPath)) continue;
                    
                    // First try common categories
                    foreach (string category in commonCategories)
                    {
                        string categoryPath = System.IO.Path.Combine(modelsPath, category);
                        if (System.IO.Directory.Exists(categoryPath))
                        {
                            foreach (string extension in extensions)
                            {
                                string filePath = System.IO.Path.Combine(categoryPath, modelName + extension);
                                if (System.IO.File.Exists(filePath))
                                {
                                    return $"models/{category}/{modelName}";
                                }
                            }
                        }
                    }
                    
                    // If not found in common categories, do a limited recursive search
                    // but only check direct subdirectories of models folder
                    try
                    {
                        string[] subDirs = System.IO.Directory.GetDirectories(modelsPath);
                        foreach (string subDir in subDirs)
                        {
                            string dirName = System.IO.Path.GetFileName(subDir);
                            if (System.Array.IndexOf(commonCategories, dirName) >= 0) continue; // Already checked
                            
                            foreach (string extension in extensions)
                            {
                                string filePath = System.IO.Path.Combine(subDir, modelName + extension);
                                if (System.IO.File.Exists(filePath))
                                {
                                    return $"models/{dirName}/{modelName}";
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        DebugLogger.LogWarningAutoObjectList($"Error searching {modelsPath}: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogWarningAutoObjectList($"Error in model search: {ex.Message}");
            }
            
            return "";
        }
        
        /// <summary>
        /// Extract clean model name from GameObject
        /// </summary>
        private static string ExtractModelNameFromGameObject(GameObject gameObject)
        {
            // Try GameObject name first
            string cleanName = CleanUnityName(gameObject.name);
            if (IsValidModelName(cleanName))
            {
                return cleanName;
            }
            
            // Try mesh names
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh != null)
                {
                    string meshName = CleanUnityName(meshFilter.sharedMesh.name);
                    if (IsValidModelName(meshName))
                    {
                        return meshName;
                    }
                }
            }
            
            return cleanName;
        }
        
        /// <summary>
        /// Clean Unity GameObject/mesh names
        /// </summary>
        private static string CleanUnityName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            
            // Remove Unity suffixes
            name = name.Replace("(Clone)", "").Replace(" (Clone)", "");
            name = name.Replace("Instance", "").Replace(" Instance", "");
            
            // Remove ObjectListInfo display names like "barrel_grey (Barrel)" -> "barrel_grey"
            if (name.Contains(" (") && name.EndsWith(")"))
            {
                int parenIndex = name.LastIndexOf(" (");
                if (parenIndex > 0)
                {
                    name = name.Substring(0, parenIndex);
                }
            }
            
            // Remove object ID suffixes
            if (name.Contains("_"))
            {
                string[] parts = name.Split('_');
                if (parts.Length >= 2)
                {
                    string lastPart = parts[parts.Length - 1];
                    if (System.Text.RegularExpressions.Regex.IsMatch(lastPart, @"^\d+\.\d+[a-zA-Z]+\d*$"))
                    {
                        return string.Join("_", parts.Take(parts.Length - 1));
                    }
                }
            }
            
            // Remove file extensions
            if (name.Contains("."))
            {
                string extension = System.IO.Path.GetExtension(name);
                if (extension == ".fbx" || extension == ".prefab" || extension == ".egg")
                {
                    return System.IO.Path.GetFileNameWithoutExtension(name);
                }
            }
            
            return name.Trim();
        }
        
        /// <summary>
        /// Check if model name is valid
        /// </summary>
        private static bool IsValidModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            
            string[] invalidNames = {
                "GameObject", "Model", "Mesh", "Prefab", "Cube", "Sphere", "Capsule", "Cylinder",
                "Plane", "Quad", "Empty", "Group", "Container", "Root", "Parent", "Child"
            };
            
            return !System.Array.Exists(invalidNames, invalid => name.Equals(invalid, System.StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Detect model category for path
        /// </summary>
        private static string DetectModelCategory(string modelName)
        {
            modelName = modelName.ToLower();
            
            if (modelName.Contains("building") || modelName.Contains("interior")) return "buildings";
            if (modelName.Contains("cav") || modelName.Contains("cave")) return "caves";
            if (modelName.Contains("char") || modelName.Contains("avatar")) return "char";
            if (modelName.Contains("effect") || modelName.Contains("particle")) return "effects";
            if (modelName.Contains("gui") || modelName.Contains("interface")) return "gui";
            if (modelName.Contains("weapon") || modelName.Contains("sword")) return "weapons";
            if (modelName.Contains("ship") || modelName.Contains("boat")) return "ships";
            if (modelName.Contains("environment") || modelName.Contains("terrain")) return "environment";
            
            return "props"; // Default category for most objects
        }
    }
}
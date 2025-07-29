using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace POTCO
{
    /// <summary>
    /// Enhanced component that stores POTCO object type information with auto-detection
    /// </summary>
    public class POTCOTypeInfo : MonoBehaviour
    {
        [Header("POTCO Object Information")]
        [Tooltip("Select from available POTCO object types")]
        public string objectType = "MISC_OBJ";
        
        [Tooltip("Auto-generated unique ID (generated on export)")]
        public string objectId;
        
        [Tooltip("Auto-detected model path based on GameObject")]
        public string modelPath;
        
        [Header("Visual Properties")]
        public bool hasVisualBlock = true;
        public Color? visualColor;
        
        [Header("Object Properties")]
        public bool disableCollision = false;
        public bool instanced = false;
        public string holiday = "";
        public string visSize = "";
        
        [Header("Auto-Detection")]
        [Tooltip("Auto-detect properties from GameObject and ObjectList")]
        public bool autoDetectOnStart = true;
        
        [Tooltip("Generate new object ID automatically")]
        public bool autoGenerateId = true;
        
        private void Awake()
        {
            // Check for duplicate object IDs when component is first created/loaded
            if (autoGenerateId && !string.IsNullOrEmpty(objectId))
            {
                CheckAndFixDuplicateObjectId();
            }
        }
        
        private void Start()
        {
            if (autoDetectOnStart)
            {
                AutoDetectProperties();
            }
        }
        
        private void OnValidate()
        {
            // Auto-detect when component is added or modified in editor
            if (Application.isEditor && autoDetectOnStart)
            {
                AutoDetectProperties();
            }
        }
        
        /// <summary>
        /// Auto-detect POTCO properties from the GameObject
        /// </summary>
        public void AutoDetectProperties()
        {
            if (autoGenerateId)
            {
                if (string.IsNullOrEmpty(objectId))
                {
                    GenerateObjectId();
                }
                else
                {
                    // Check for duplicates even if we have an ID
                    CheckAndFixDuplicateObjectId();
                }
            }
            
            // Auto-detect model path from GameObject name or mesh
            if (string.IsNullOrEmpty(modelPath))
            {
                modelPath = AutoDetectModelPath();
            }
            
            // Auto-detect object type from model path
            if (objectType == "MISC_OBJ" && !string.IsNullOrEmpty(modelPath))
            {
                string detectedType = AutoDetectObjectType();
                if (!string.IsNullOrEmpty(detectedType))
                {
                    objectType = detectedType;
                }
            }
        }
        
        /// <summary>
        /// Check if the current object ID is duplicated in the scene and fix it
        /// </summary>
        private void CheckAndFixDuplicateObjectId()
        {
            if (string.IsNullOrEmpty(objectId)) return;
            
            // Find all POTCOTypeInfo components in the scene
            POTCOTypeInfo[] allPOTCOComponents = FindObjectsByType<POTCOTypeInfo>(FindObjectsSortMode.None);
            
            // Count how many objects have the same ID (excluding this one)
            int duplicateCount = 0;
            foreach (POTCOTypeInfo other in allPOTCOComponents)
            {
                if (other != this && other.objectId == this.objectId)
                {
                    duplicateCount++;
                }
            }
            
            // If we found duplicates, generate a new ID
            if (duplicateCount > 0)
            {
                string oldId = objectId;
                GenerateObjectId();
                Debug.Log($"🔄 Fixed duplicate object ID on '{gameObject.name}': '{oldId}' -> '{objectId}' ({duplicateCount} duplicates found)");
            }
        }
        
        /// <summary>
        /// Generate a unique POTCO-style object ID
        /// </summary>
        public void GenerateObjectId()
        {
            // Generate timestamp similar to POTCO format (Unix timestamp with milliseconds)
            double timestamp = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
            
            // Add some randomness for uniqueness
            int sequence = Random.Range(10, 99);
            
            // Use a default username that won't conflict
            string username = "unity";
            
            objectId = $"{timestamp:F2}{username}{sequence:D2}";
        }
        
        /// <summary>
        /// Auto-detect model path from GameObject by searching Resources folder
        /// </summary>
        private string AutoDetectModelPath()
        {
            string modelName = ExtractModelNameFromGameObject();
            if (string.IsNullOrEmpty(modelName))
            {
                return "";
            }
            
            // Search through Resources/phase_* folders to find the actual model location
            string foundPath = SearchForModelInResources(modelName);
            if (!string.IsNullOrEmpty(foundPath))
            {
                Debug.Log($"📁 Found model in Resources: '{foundPath}'");
                return foundPath;
            }
            
            // Fallback to pattern-based detection if not found in Resources
            string category = DetectModelCategory(modelName);
            string fallbackPath = $"models/{category}/{modelName}";
            Debug.Log($"📁 Model not found in Resources, using fallback: '{fallbackPath}'");
            return fallbackPath;
        }
        
        /// <summary>
        /// Search for model file in Resources/phase_* folders (fast, limited search)
        /// </summary>
        private string SearchForModelInResources(string modelName)
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
                        Debug.Log($"⏱️ Model search timeout for '{modelName}', using fallback");
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
                        Debug.LogWarning($"Error searching {modelsPath}: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error in model search: {ex.Message}");
            }
            
            return "";
        }
        
        /// <summary>
        /// Extract model name from GameObject and its components
        /// </summary>
        private string ExtractModelNameFromGameObject()
        {
            // First try the GameObject name
            string cleanName = CleanModelName(gameObject.name);
            if (IsValidModelName(cleanName))
            {
                return cleanName;
            }
            
            // Try mesh names from children
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh != null)
                {
                    string meshName = CleanModelName(meshFilter.sharedMesh.name);
                    if (IsValidModelName(meshName))
                    {
                        return meshName;
                    }
                }
            }
            
            return cleanName; // Return cleaned name even if not validated
        }
        
        /// <summary>
        /// Clean model name by removing Unity suffixes and object IDs
        /// </summary>
        private string CleanModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            
            // Remove common Unity suffixes
            name = name.Replace("(Clone)", "").Replace(" (Clone)", "");
            name = name.Replace("Instance", "").Replace(" Instance", "");
            
            // Remove object ID suffixes (pattern: ModelName_1165269209.69kmuller)
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
        /// Check if a model name seems valid (not generic Unity names)
        /// </summary>
        private bool IsValidModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            
            string[] invalidNames = {
                "GameObject", "Model", "Mesh", "Prefab", "Cube", "Sphere", "Capsule", "Cylinder",
                "Plane", "Quad", "Empty", "Group", "Container", "Root", "Parent", "Child"
            };
            
            return !invalidNames.Any(invalid => name.Equals(invalid, System.StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Detect model category from name patterns
        /// </summary>
        private string DetectModelCategory(string modelName)
        {
            modelName = modelName.ToLower();
            
            if (modelName.Contains("building") || modelName.Contains("interior")) return "buildings";
            if (modelName.Contains("cav") || modelName.Contains("cave")) return "caves";
            if (modelName.Contains("prop") || modelName.Contains("furniture")) return "props";
            if (modelName.Contains("char") || modelName.Contains("avatar")) return "char";
            if (modelName.Contains("effect") || modelName.Contains("particle")) return "effects";
            if (modelName.Contains("gui") || modelName.Contains("interface")) return "gui";
            if (modelName.Contains("weapon") || modelName.Contains("sword")) return "weapons";
            if (modelName.Contains("ship") || modelName.Contains("boat")) return "ships";
            if (modelName.Contains("environment") || modelName.Contains("terrain")) return "environment";
            
            return "props"; // Default category
        }
        
        /// <summary>
        /// Auto-detect object type using ObjectList.py data only
        /// </summary>
        private string AutoDetectObjectType()
        {
            if (string.IsNullOrEmpty(modelPath)) return "MISC_OBJ";
            
            string modelName = System.IO.Path.GetFileNameWithoutExtension(modelPath);
            Debug.Log($"🔍 Attempting to detect object type for model: '{modelName}' from path: '{modelPath}'");
            
            // Try to use ObjectListParser first (editor only)
            try
            {
                // This will only work in editor context, but that's fine since auto-detection happens in editor
                var objectListType = System.Type.GetType("WorldDataExporter.Utilities.ObjectListParser, Assembly-CSharp-Editor");
                if (objectListType != null)
                {
                    var method = objectListType.GetMethod("GetObjectTypeByModelName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        string result = (string)method.Invoke(null, new object[] { modelName });
                        if (!string.IsNullOrEmpty(result) && result != "Unknown")
                        {
                            Debug.Log($"✅ ObjectList found: '{modelName}' -> '{result}'");
                            return result;
                        }
                        else
                        {
                            Debug.Log($"❌ ObjectList lookup failed for '{modelName}' - not found in database");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Could not access ObjectListParser: {ex.Message}");
            }
            
            // Special case fallback: Cave pieces should be Cave_Pieces type (MODULAR_OBJ mapping)
            if (modelName.ToLower().Contains("cav") || modelPath.ToLower().Contains("caves"))
            {
                Debug.Log($"🏔️ Cave piece detected (fallback): '{modelName}' -> 'Cave_Pieces'");
                return "Cave_Pieces";
            }
            
            // Fallback: return MISC_OBJ if ObjectList lookup fails
            Debug.Log($"🔄 Defaulting to 'MISC_OBJ' for '{modelName}'");
            return "MISC_OBJ";
        }
        
        /// <summary>
        /// Get a clean display name for the hierarchy
        /// </summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(modelPath))
            {
                string modelName = System.IO.Path.GetFileNameWithoutExtension(modelPath);
                return $"{modelName} ({objectType})";
            }
            
            return $"{objectType} - {objectId}";
        }
        
        /// <summary>
        /// Manually trigger property detection (for editor button)
        /// </summary>
        [ContextMenu("Auto-Detect Properties")]
        public void ManualAutoDetect()
        {
            // Use the proper POTCOObjectListIntegration for detection
            try
            {
                var integrationType = System.Type.GetType("POTCO.Editor.POTCOObjectListIntegration, Assembly-CSharp-Editor");
                if (integrationType != null)
                {
                    var method = integrationType.GetMethod("AutoDetectAllProperties", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { this });
                        Debug.Log($"✅ Used POTCOObjectListIntegration for auto-detection on '{gameObject.name}'");
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Could not use POTCOObjectListIntegration: {ex.Message}");
            }
            
            // Fallback to old method
            AutoDetectProperties();
            Debug.Log($"Auto-detected properties for '{gameObject.name}': Type='{objectType}', Model='{modelPath}', ID='{objectId}'");
        }
        
        /// <summary>
        /// Generate new object ID (for editor button)
        /// </summary>
        [ContextMenu("Generate New Object ID")]
        public void ManualGenerateId()
        {
            GenerateObjectId();
            Debug.Log($"Generated new object ID for '{gameObject.name}': {objectId}");
        }
        
        /// <summary>
        /// Check for and fix duplicate object IDs (for editor button)
        /// </summary>
        [ContextMenu("Check and Fix Duplicate ID")]
        public void ManualCheckDuplicates()
        {
            CheckAndFixDuplicateObjectId();
            Debug.Log($"Checked for duplicate IDs on '{gameObject.name}' - Current ID: {objectId}");
        }
    }
}
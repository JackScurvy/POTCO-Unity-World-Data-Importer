using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using POTCO;

namespace POTCO.Editor
{
    /// <summary>
    /// Automatically adds POTCOTypeInfo to objects dragged into the scene
    /// </summary>
    [InitializeOnLoad]
    public class AutoPOTCODetection
    {
        private static double lastAutoDetectionTime = 0;
        
        static AutoPOTCODetection()
        {
            // Subscribe to hierarchy change events
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            
            // Subscribe to scene save events to clean up any missed objects
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }
        
        private static void OnHierarchyChanged()
        {
            // Only run in play mode or when not playing
            if (Application.isPlaying) return;
            
            // Moderate throttling to prevent freezing but allow responsive detection - 0.5 second cooldown
            if (EditorApplication.timeSinceStartup - lastAutoDetectionTime < 0.5f) 
            {
                Debug.Log($"⏱️ AutoPOTCODetection throttled - last run {EditorApplication.timeSinceStartup - lastAutoDetectionTime:F2}s ago");
                return;
            }
            lastAutoDetectionTime = EditorApplication.timeSinceStartup;
            
            Debug.Log($"🔄 AutoPOTCODetection running...");
            
            // Use a timeout to prevent long operations
            var startTime = System.DateTime.Now;
            const int maxProcessingTimeMs = 50; // Only spend 50ms max
            
            // Only check recently added objects instead of all objects
            // Get objects that likely need checking (recently created/imported)
            GameObject[] recentObjects = GetRecentlyAddedObjects();
            
            int processedCount = 0;
            foreach (GameObject obj in recentObjects)
            {
                // Timeout check - don't freeze Unity
                if ((System.DateTime.Now - startTime).TotalMilliseconds > maxProcessingTimeMs)
                {
                    Debug.Log($"⏰ Auto-detection timeout after processing {processedCount} objects");
                    break;
                }
                
                // Skip if it already has POTCOTypeInfo
                if (obj.GetComponent<POTCOTypeInfo>() != null) continue;
                
                // Skip UI objects, cameras, lights that shouldn't be POTCO objects
                if (ShouldSkipObject(obj)) continue;
                
                // IMPORTANT: Only apply to root/parent objects, not child mesh objects
                if (IsChildMeshObject(obj)) 
                {
                    // If this is a child mesh object, check if the parent needs POTCOTypeInfo
                    GameObject parent = obj.transform.parent?.gameObject;
                    if (parent != null)
                    {
                        Debug.Log($"🔍 Child '{obj.name}' skipped, checking parent '{parent.name}'");
                        
                        bool hasComponent = parent.GetComponent<POTCOTypeInfo>() != null;
                        bool looksLikePOTCO = QuickLooksLikePOTCOModel(parent);
                        
                        Debug.Log($"  📋 Parent '{parent.name}' - HasComponent: {hasComponent}, LooksLikePOTCO: {looksLikePOTCO}");
                        
                        if (!hasComponent && looksLikePOTCO)
                        {
                            Debug.Log($"🔄 Adding POTCOTypeInfo to parent '{parent.name}'");
                            AddPOTCOTypeInfoToObject(parent);
                        }
                        else if (hasComponent)
                        {
                            Debug.Log($"⏭️ Parent '{parent.name}' already has POTCOTypeInfo");
                        }
                        else if (!looksLikePOTCO)
                        {
                            Debug.Log($"⏭️ Parent '{parent.name}' doesn't look like POTCO model");
                        }
                    }
                    else
                    {
                        Debug.Log($"⚠️ Child '{obj.name}' has no parent - this shouldn't happen");
                    }
                    continue;
                }
                
                // Quick check if this looks like a POTCO model (fast pattern matching only)
                if (QuickLooksLikePOTCOModel(obj))
                {
                    AddPOTCOTypeInfoToObject(obj);
                }
                
                processedCount++;
            }
        }
        
        /// <summary>
        /// Get objects that were likely recently added (more efficient than checking all objects)
        /// </summary>
        private static GameObject[] GetRecentlyAddedObjects()
        {
            // Instead of checking ALL objects, only check root objects and their immediate children
            // This is much faster than the full scene scan
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            var candidates = new System.Collections.Generic.List<GameObject>();
            
            foreach (GameObject root in rootObjects)
            {
                candidates.Add(root);
                
                // Only check first level children to avoid deep recursion
                for (int i = 0; i < root.transform.childCount && i < 10; i++) // Limit to 10 children max
                {
                    candidates.Add(root.transform.GetChild(i).gameObject);
                }
            }
            
            return candidates.ToArray();
        }
        
        /// <summary>
        /// Quick POTCO model detection by checking if model exists in Resources/phase_*/models/
        /// </summary>
        private static bool QuickLooksLikePOTCOModel(GameObject obj)
        {
            string name = obj.name;
            
            // Skip obvious mesh part names
            if (name.Contains("mesh") || name.Contains("geometry") || name.Contains("_geo") || 
                name.Contains("_mesh") || name.Equals("unnamed") || name.StartsWith("polysurface"))
            {
                return false;
            }
            
            // Extract clean model name (remove Unity suffixes)
            string cleanName = CleanModelName(name);
            if (string.IsNullOrEmpty(cleanName)) return false;
            
            // Check if this model exists in Resources/phase_*/models/ folders
            return ModelExistsInResources(cleanName);
        }
        
        /// <summary>
        /// Check if a model file exists in any Resources/phase_*/models/ folder
        /// </summary>
        private static bool ModelExistsInResources(string modelName)
        {
            try
            {
                // Get the path to Resources folder
                string resourcesPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                if (!System.IO.Directory.Exists(resourcesPath)) return false;
                
                string[] extensions = { ".fbx", ".prefab", ".egg" };
                
                // Search through phase_* folders quickly
                string[] phaseFolders = System.IO.Directory.GetDirectories(resourcesPath, "phase_*");
                foreach (string phaseFolder in phaseFolders)
                {
                    string modelsPath = System.IO.Path.Combine(phaseFolder, "models");
                    if (!System.IO.Directory.Exists(modelsPath)) continue;
                    
                    // Check all subdirectories in models folder
                    string[] modelSubDirs = System.IO.Directory.GetDirectories(modelsPath);
                    foreach (string subDir in modelSubDirs)
                    {
                        // Check for model file with any supported extension
                        foreach (string extension in extensions)
                        {
                            string filePath = System.IO.Path.Combine(subDir, modelName + extension);
                            if (System.IO.File.Exists(filePath))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error checking model existence: {ex.Message}");
            }
            
            return false;
        }
        
        /// <summary>
        /// Clean model name by removing Unity suffixes
        /// </summary>
        private static string CleanModelName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            
            // Remove Unity suffixes
            name = name.Replace("(Clone)", "").Replace(" (Clone)", "");
            name = name.Replace("Instance", "").Replace(" Instance", "");
            
            // Remove POTCOTypeInfo display names like "barrel_grey (Barrel)" -> "barrel_grey"
            if (name.Contains(" (") && name.EndsWith(")"))
            {
                int parenIndex = name.LastIndexOf(" (");
                if (parenIndex > 0)
                {
                    name = name.Substring(0, parenIndex);
                }
            }
            
            return name.Trim();
        }
        
        private static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
        {
            // Run detection again on scene save to catch any missed objects
            OnHierarchyChanged();
        }
        
        /// <summary>
        /// Check if this object should be skipped for POTCOTypeInfo based on POTCO hierarchy rules
        /// </summary>
        private static bool IsChildMeshObject(GameObject obj)
        {
            return IsChildMeshObjectPublic(obj);
        }
        
        /// <summary>
        /// Public version of IsChildMeshObject for external cleanup tools
        /// </summary>
        public static bool IsChildMeshObjectPublic(GameObject obj)
        {
            // If this object has no parent, it could be a root POTCO object
            if (obj.transform.parent == null) return false;
            
            GameObject parent = obj.transform.parent.gameObject;
            
            // RULE 1: If parent has POTCOTypeInfo, this object should NOT get POTCOTypeInfo
            // This prevents nested POTCOTypeInfo components
            if (parent.GetComponent<POTCOTypeInfo>() != null)
            {
                Debug.Log($"⏭️ Skipping '{obj.name}' - parent '{parent.name}' already has POTCOTypeInfo (hierarchy rule)");
                return true;
            }
            
            // RULE 2: Check if any ancestor has POTCOTypeInfo (multi-level hierarchy)
            Transform ancestor = parent.transform.parent;
            while (ancestor != null)
            {
                if (ancestor.GetComponent<POTCOTypeInfo>() != null)
                {
                    Debug.Log($"⏭️ Skipping '{obj.name}' - ancestor '{ancestor.name}' has POTCOTypeInfo (nested hierarchy rule)");
                    return true;
                }
                ancestor = ancestor.parent;
            }
            
            // RULE 3: Skip obvious mesh/geometry children
            string name = obj.name.ToLower();
            if (name.Contains("mesh") || name.Contains("geometry") || name.Contains("model") || 
                name.Contains("_geo") || name.Contains("_mesh") || name.Contains("lod") ||
                name.Equals("unnamed") || name.StartsWith("polysurface"))
            {
                Debug.Log($"⏭️ Skipping child mesh object '{obj.name}' - appears to be geometry");
                return true;
            }
            
            // RULE 4: Special handling for interior models - their children are always mesh parts
            string parentName = parent.name.ToLower();
            if (parentName.Contains("interior_") || parentName.StartsWith("pir_m_bld_int_") || 
                parentName.Contains("_interior") || parentName.Contains("building_interior"))
            {
                Debug.Log($"⏭️ Skipping '{obj.name}' - parent '{parent.name}' is an interior model, children are mesh parts");
                return true;
            }
            
            // RULE 5: If this object has mesh components and parent looks like POTCO model
            // This prevents mesh children from getting POTCOTypeInfo when parent should have it
            if ((obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<MeshFilter>() != null))
            {
                if (LooksLikePOTCOModel(parent))
                {
                    Debug.Log($"⏭️ Skipping child mesh object '{obj.name}' - parent '{parent.name}' should have POTCOTypeInfo");
                    return true;
                }
            }
            
            // RULE 6: Check if this object is a simple mesh child of a complex model
            // If parent has multiple mesh children, this is likely a mesh part
            if (obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<MeshFilter>() != null)
            {
                MeshRenderer[] siblingMeshes = parent.GetComponentsInChildren<MeshRenderer>();
                if (siblingMeshes.Length > 1)
                {
                    Debug.Log($"⏭️ Skipping '{obj.name}' - one of {siblingMeshes.Length} mesh parts in '{parent.name}'");
                    return true;
                }
            }
            
            // RULE 7: Skip objects with default transform (position 0,0,0, rotation 0,0,0, scale 1,1,1)
            // These are typically organizational or mesh parts
            Transform transform = obj.transform;
            Vector3 defaultPos = Vector3.zero;
            Vector3 defaultRot = Vector3.zero;
            Vector3 defaultScale = Vector3.one;
            
            if (Vector3.Distance(transform.localPosition, defaultPos) < 0.001f &&
                Vector3.Distance(transform.localEulerAngles, defaultRot) < 0.001f &&
                Vector3.Distance(transform.localScale, defaultScale) < 0.001f)
            {
                Debug.Log($"⏭️ Skipping '{obj.name}' - has default transform (0,0,0 pos/rot, 1,1,1 scale), likely mesh part");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if we should skip adding POTCOTypeInfo to this object
        /// </summary>
        private static bool ShouldSkipObject(GameObject obj)
        {
            // Skip Unity built-in objects
            if (obj.name.StartsWith("Main Camera") || obj.name.StartsWith("Directional Light")) return true;
            
            // Skip objects that are children of Canvas (UI objects)
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                if (parent.GetComponent<Canvas>() != null) return true;
                parent = parent.parent;
            }
            
            // Skip objects with certain components that indicate they're not POTCO models
            if (obj.GetComponent<Camera>() != null) return true;
            if (obj.GetComponent<Light>() != null && !LooksLikePOTCOLight(obj)) return true;
            if (obj.GetComponent<Canvas>() != null) return true;
            if (obj.GetComponent<UnityEngine.EventSystems.EventSystem>() != null) return true;
            
            // Skip objects that are prefab instances of Unity built-ins
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                if (prefabRoot != null && prefabRoot.name.Contains("Unity")) return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this looks like a POTCO model that should have POTCOTypeInfo
        /// Uses ObjectList.py data for detection instead of hardcoded patterns
        /// </summary>
        private static bool LooksLikePOTCOModel(GameObject obj)
        {
            string name = obj.name.ToLower();
            
            // Skip obvious mesh part names
            if (name.Contains("mesh") || name.Contains("geometry") || name.Contains("_geo") || 
                name.Contains("_mesh") || name.Equals("unnamed") || name.StartsWith("polysurface"))
            {
                return false;
            }
            
            // POTCO models often have specific naming patterns
            if (name.StartsWith("pir_")) return true;
            if (name.Contains("_m_")) return true; // Model indicator
            if (name.Contains("_prp_")) return true; // Prop indicator
            if (name.Contains("_chr_")) return true; // Character indicator
            if (name.Contains("_bld_")) return true; // Building indicator
            if (name.Contains("_env_")) return true; // Environment indicator
            
            // Interior models are always root POTCO objects
            if (name.Contains("interior_") || name.StartsWith("interior") || name.Contains("_interior"))
            {
                return true;
            }
            
            // Try to detect using ObjectList.py data
            try
            {
                // Extract clean model name and check if it exists in ObjectList.py
                string cleanName = obj.name;
                
                // Remove POTCOTypeInfo display names like "bottle_red (Jug)" -> "bottle_red"
                if (cleanName.Contains(" (") && cleanName.EndsWith(")"))
                {
                    int parenIndex = cleanName.LastIndexOf(" (");
                    if (parenIndex > 0)
                    {
                        cleanName = cleanName.Substring(0, parenIndex);
                    }
                }
                
                // Try ObjectList lookup
                string objectType = WorldDataExporter.Utilities.ObjectListParser.GetObjectTypeByModelName(cleanName);
                if (!string.IsNullOrEmpty(objectType) && objectType != "Unknown")
                {
                    return true;
                }
                
                // Try cleaned name patterns
                string[] cleanVariants = {
                    cleanName,
                    cleanName.Replace("_high", "").Replace("_low", "").Replace("_med", ""),
                    cleanName.StartsWith("pir_m_prp_") ? cleanName.Substring(10) : cleanName,
                    cleanName.StartsWith("pir_m_") ? cleanName.Substring(6) : cleanName,
                    cleanName.StartsWith("pir_") ? cleanName.Substring(4) : cleanName
                };
                
                foreach (string variant in cleanVariants)
                {
                    objectType = WorldDataExporter.Utilities.ObjectListParser.GetObjectTypeByModelName(variant);
                    if (!string.IsNullOrEmpty(objectType) && objectType != "Unknown")
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to check ObjectList for '{obj.name}': {ex.Message}");
            }
            
            // Check if it has mesh components (visual objects)
            if (obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<MeshFilter>() != null) 
            {
                // If it has a mesh and doesn't look like a Unity primitive, it's probably a POTCO model
                if (!IsUnityPrimitive(obj)) return true;
            }
            
            // Check if any children have mesh components
            MeshRenderer[] childMeshes = obj.GetComponentsInChildren<MeshRenderer>();
            if (childMeshes.Length > 0)
            {
                // If it has child meshes and doesn't look like Unity built-ins, it might be a POTCO model
                if (!IsUnityPrimitive(obj)) return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this is a Unity primitive (Cube, Sphere, etc.)
        /// </summary>
        private static bool IsUnityPrimitive(GameObject obj)
        {
            string name = obj.name.ToLower();
            string[] primitives = { "cube", "sphere", "capsule", "cylinder", "plane", "quad" };
            
            foreach (string primitive in primitives)
            {
                if (name.Equals(primitive) || name.StartsWith(primitive + " ")) return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this light looks like a POTCO light (not a Unity scene light)
        /// </summary>
        private static bool LooksLikePOTCOLight(GameObject obj)
        {
            string name = obj.name.ToLower();
            
            // POTCO lights usually have specific names
            if (name.Contains("torch") || name.Contains("lantern") || name.Contains("candle") ||
                name.Contains("fire") || name.Contains("flame") || name.Contains("pir_")) return true;
            
            // Unity default lights should be skipped
            if (name.Contains("directional light") || name.Contains("point light") || 
                name.Contains("spot light")) return false;
            
            return false;
        }
        
        /// <summary>
        /// Add POTCOTypeInfo component and auto-detect properties
        /// </summary>
        private static void AddPOTCOTypeInfoToObject(GameObject obj)
        {
            // Add the component (using Undo for proper editor integration)
            POTCOTypeInfo potcoInfo = Undo.AddComponent<POTCOTypeInfo>(obj);
            
            // Auto-detect properties using our ObjectList integration
            POTCOObjectListIntegration.AutoDetectAllProperties(potcoInfo);
            
            // Mark the object as dirty so changes are saved
            EditorUtility.SetDirty(obj);
            
            Debug.Log($"✅ Auto-added POTCOTypeInfo to '{obj.name}' - Type: '{potcoInfo.objectType}', Model: '{potcoInfo.modelPath}'");
        }
        
        /// <summary>
        /// Manually apply POTCOTypeInfo to selected objects
        /// </summary>
        public static void AddPOTCOTypeInfoToSelected()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            
            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No objects selected. Please select one or more GameObjects in the scene.");
                return;
            }
            
            int addedCount = 0;
            int skippedCount = 0;
            
            foreach (GameObject obj in selectedObjects)
            {
                if (obj.GetComponent<POTCOTypeInfo>() != null)
                {
                    Debug.Log($"⏭️ Skipped '{obj.name}' - already has POTCOTypeInfo");
                    skippedCount++;
                    continue;
                }
                
                AddPOTCOTypeInfoToObject(obj);
                addedCount++;
            }
            
            Debug.Log($"✅ Added POTCOTypeInfo to {addedCount} objects, skipped {skippedCount} objects");
        }
        
        /// <summary>
        /// Refresh all POTCOTypeInfo components in the scene
        /// </summary>
        public static void RefreshAllPOTCOTypeInfo()
        {
            POTCOTypeInfo[] allPOTCOComponents = GameObject.FindObjectsByType<POTCOTypeInfo>(FindObjectsSortMode.None);
            
            Debug.Log($"🔄 Refreshing {allPOTCOComponents.Length} POTCOTypeInfo components...");
            
            foreach (POTCOTypeInfo potcoInfo in allPOTCOComponents)
            {
                POTCOObjectListIntegration.AutoDetectAllProperties(potcoInfo);
                EditorUtility.SetDirty(potcoInfo.gameObject);
            }
            
            Debug.Log($"✅ Refreshed all POTCOTypeInfo components in scene");
        }
        
    }
}
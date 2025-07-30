using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using WorldDataExporter.Utilities;
using WorldDataExporter.Data;
using POTCO;
using POTCO.Editor;

namespace WorldDataExporter
{
    public class WorldDataExporter : EditorWindow
    {
        private ExportSettings settings = new ExportSettings();
        private ExportStatistics lastExportStats;
        private Vector2 scrollPosition;
        private bool showStatistics = false;
        
        // Mesh visibility settings
        private static bool hideMeshObjects = false;

        [MenuItem("POTCO/World Data Exporter")]
        public static void ShowWindow()
        {
            GetWindow<WorldDataExporter>("World Data Exporter");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("POTCO World Data Exporter", EditorStyles.boldLabel);
            GUILayout.Space(10);

            DrawBasicSettings();
            GUILayout.Space(10);

            DrawFilteringOptions();
            GUILayout.Space(10);

            DrawExportActions();
            GUILayout.Space(10);

            DrawDebuggingTools();
            GUILayout.Space(10);

            DrawStatistics();

            EditorGUILayout.EndScrollView();
        }

        private void DrawBasicSettings()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Export Settings", EditorStyles.boldLabel);

            // Export source selection
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Export Source:", GUILayout.Width(100));
            settings.exportSource = (ExportSource)EditorGUILayout.EnumPopup(settings.exportSource);
            EditorGUILayout.EndHorizontal();

            if (settings.exportSource == ExportSource.SelectedObjects)
            {
                EditorGUILayout.HelpBox("Select GameObjects in the scene hierarchy to export", MessageType.Info);
            }

            GUILayout.Space(5);

            // Output file selection
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Output File", GUILayout.Width(120)))
            {
                string path = EditorUtility.SaveFilePanel("Export World Data", 
                    "Assets/Editor/World Data Exporter/", 
                    "exported_world.py", 
                    "py");
                if (!string.IsNullOrEmpty(path))
                {
                    settings.outputPath = path;
                    DebugLogger.LogWorldExporter($"📤 Export path set: {settings.outputPath}");
                }
            }
            if (!string.IsNullOrEmpty(settings.outputPath))
            {
                EditorGUILayout.LabelField("Output:", System.IO.Path.GetFileName(settings.outputPath));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawFilteringOptions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Object Filtering", EditorStyles.boldLabel);

            settings.exportLighting = EditorGUILayout.Toggle("Export Lighting", settings.exportLighting);
            EditorGUILayout.LabelField("   Include Light - Dynamic objects with all lighting properties", EditorStyles.miniLabel);

            settings.exportCollisions = EditorGUILayout.Toggle("Export Collisions", settings.exportCollisions);
            EditorGUILayout.LabelField("   Include Collision Barrier objects", EditorStyles.miniLabel);

            settings.exportNodes = EditorGUILayout.Toggle("Export Nodes", settings.exportNodes);
            EditorGUILayout.LabelField("   Include spawn points, locators, and other node objects", EditorStyles.miniLabel);

            settings.exportHolidayObjects = EditorGUILayout.Toggle("Export Holiday Objects", settings.exportHolidayObjects);
            EditorGUILayout.LabelField("   Include objects with Holiday properties", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();
        }

        private void DrawExportActions()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Export Actions", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(settings.outputPath));

            if (GUILayout.Button("🚀 Export World Data", GUILayout.Height(30)))
            {
                DebugLogger.LogWorldExporter($"🚀 Starting world data export...");
                lastExportStats = ExportUtilities.ExportWorldData(settings);
                showStatistics = true;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        private void DrawDebuggingTools()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Debugging Tools", EditorStyles.boldLabel);
            
            if (GUILayout.Button("➕ Add POTCOTypeInfo to Selected Objects"))
            {
                POTCO.Editor.AutoPOTCODetection.AddPOTCOTypeInfoToSelected();
            }
            
            if (GUILayout.Button("🔄 Refresh All POTCOTypeInfo in Scene"))
            {
                POTCO.Editor.AutoPOTCODetection.RefreshAllPOTCOTypeInfo();
            }
            
            if (GUILayout.Button("🔍 Check for Duplicate Object IDs"))
            {
                CheckForDuplicateObjectIds();
            }
            
            EditorGUILayout.Space(5);
            
            // Auto-detection toggle
            EditorGUI.BeginChangeCheck();
            bool autoDetectionEnabled = POTCO.Editor.AutoPOTCODetection.IsAutoDetectionEnabled();
            autoDetectionEnabled = EditorGUILayout.Toggle("🔄 Auto-Add POTCOTypeInfo to New Objects", autoDetectionEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                POTCO.Editor.AutoPOTCODetection.SetAutoDetectionEnabled(autoDetectionEnabled);
            }
            
            EditorGUILayout.HelpBox("When enabled, POTCOTypeInfo components will be automatically added to objects dragged into the scene. Disable this to prevent background processing.", MessageType.Info);
            
            EditorGUILayout.Space(5);
            
            // Primary objects only toggle
            EditorGUI.BeginChangeCheck();
            hideMeshObjects = EditorGUILayout.Toggle("🎯 Show Only Primary Objects", hideMeshObjects);
            if (EditorGUI.EndChangeCheck())
            {
                SetMeshObjectsVisibility(!hideMeshObjects);
            }
            
            EditorGUILayout.HelpBox("Hides all child objects and non-POTCO objects, showing only root GameObjects with POTCOTypeInfo and important Unity objects (Camera, Lights, etc.)", MessageType.Info);
            
            // Manual hide/show buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🙈 Hide Child Objects"))
            {
                hideMeshObjects = true;
                SetMeshObjectsVisibility(false);
            }
            if (GUILayout.Button("👁️ Show All Objects"))
            {
                hideMeshObjects = false;
                SetMeshObjectsVisibility(true);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Check for duplicate object IDs across all POTCOTypeInfo components
        /// </summary>
        public static void CheckForDuplicateObjectIds()
        {
            var allPOTCOComponents = GameObject.FindObjectsByType<POTCO.POTCOTypeInfo>(FindObjectsSortMode.None);
            
            if (allPOTCOComponents.Length == 0)
            {
                DebugLogger.LogWorldExporter("🔍 No POTCOTypeInfo components found in scene");
                return;
            }
            
            var idGroups = allPOTCOComponents
                .Where(p => !string.IsNullOrEmpty(p.objectId))
                .GroupBy(p => p.objectId)
                .Where(g => g.Count() > 1)
                .ToList();
            
            if (idGroups.Count == 0)
            {
                DebugLogger.LogWorldExporter($"✅ No duplicate object IDs found ({allPOTCOComponents.Length} objects checked)");
                return;
            }
            
            DebugLogger.LogWorldExporter($"⚠️ Found {idGroups.Count} duplicate object ID groups:");
            
            int totalDuplicates = 0;
            int fixedCount = 0;
            
            foreach (var group in idGroups)
            {
                var duplicates = group.ToList();
                totalDuplicates += duplicates.Count;
                
                DebugLogger.LogWorldExporter($"🔄 Duplicate ID '{group.Key}' found on {duplicates.Count} objects:");
                
                // Keep the first object, fix the rest
                for (int i = 0; i < duplicates.Count; i++)
                {
                    var obj = duplicates[i];
                    DebugLogger.LogWorldExporter($"   {i + 1}. '{obj.gameObject.name}' at {obj.transform.position}");
                    
                    if (i > 0) // Fix all except the first one
                    {
                        string oldId = obj.objectId;
                        obj.GenerateObjectId();
                        EditorUtility.SetDirty(obj);
                        DebugLogger.LogWorldExporter($"   ✅ Fixed: '{oldId}' -> '{obj.objectId}'");
                        fixedCount++;
                    }
                }
            }
            
            DebugLogger.LogWorldExporter($"✅ Fixed {fixedCount} duplicate IDs out of {totalDuplicates} total duplicates");
        }

        /// <summary>
        /// Clean up POTCOTypeInfo components that were incorrectly added to mesh parts
        /// </summary>
        public static void CleanUpMeshPartComponents()
        {
            var allPOTCOComponents = GameObject.FindObjectsByType<POTCO.POTCOTypeInfo>(FindObjectsSortMode.None);
            
            if (allPOTCOComponents.Length == 0)
            {
                DebugLogger.LogWorldExporter("🧹 No POTCOTypeInfo components found in scene");
                return;
            }
            
            DebugLogger.LogWorldExporter($"🧹 Checking {allPOTCOComponents.Length} POTCOTypeInfo components for incorrect placement...");
            
            int removedCount = 0;
            int movedCount = 0;
            
            foreach (var potcoInfo in allPOTCOComponents)
            {
                GameObject obj = potcoInfo.gameObject;
                
                // Check if this should be a child mesh object using the same logic as auto-detection
                if (POTCO.Editor.AutoPOTCODetection.IsChildMeshObjectPublic(obj))
                {
                    GameObject parent = obj.transform.parent?.gameObject;
                    
                    // If parent exists and doesn't have POTCOTypeInfo, move it there
                    if (parent != null && parent.GetComponent<POTCO.POTCOTypeInfo>() == null)
                    {
                        DebugLogger.LogWorldExporter($"🔄 Moving POTCOTypeInfo from mesh part '{obj.name}' to parent '{parent.name}'");
                        
                        // Copy the component data to parent
                        var newComponent = parent.AddComponent<POTCO.POTCOTypeInfo>();
                        newComponent.objectType = potcoInfo.objectType;
                        newComponent.objectId = potcoInfo.objectId;
                        newComponent.modelPath = potcoInfo.modelPath;
                        newComponent.hasVisualBlock = potcoInfo.hasVisualBlock;
                        newComponent.visualColor = potcoInfo.visualColor;
                        newComponent.disableCollision = potcoInfo.disableCollision;
                        newComponent.instanced = potcoInfo.instanced;
                        newComponent.holiday = potcoInfo.holiday;
                        newComponent.visSize = potcoInfo.visSize;
                        newComponent.autoDetectOnStart = potcoInfo.autoDetectOnStart;
                        newComponent.autoGenerateId = potcoInfo.autoGenerateId;
                        
                        EditorUtility.SetDirty(parent);
                        movedCount++;
                    }
                    
                    // Remove from the mesh part
                    DebugLogger.LogWorldExporter($"🗑️ Removing POTCOTypeInfo from mesh part '{obj.name}'");
                    UnityEngine.Object.DestroyImmediate(potcoInfo);
                    EditorUtility.SetDirty(obj);
                    removedCount++;
                }
            }
            
            DebugLogger.LogWorldExporter($"✅ Cleanup complete: Moved {movedCount} components to parents, removed {removedCount} from mesh parts");
        }

        /// <summary>
        /// Debug interior model detection to understand why components are being added to mesh parts
        /// </summary>
        public static void DebugInteriorModelDetection()
        {
            var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            // Find all interior-related objects
            var interiorObjects = allObjects.Where(obj => obj.name.ToLower().Contains("interior_")).ToList();
            
            if (interiorObjects.Count == 0)
            {
                DebugLogger.LogWorldExporter("🔍 No interior models found in scene");
                return;
            }
            
            DebugLogger.LogWorldExporter($"🏗️ Found {interiorObjects.Count} interior-related objects:");
            
            foreach (var obj in interiorObjects)
            {
                DebugLogger.LogWorldExporter($"\n📋 Analyzing '{obj.name}':");
                DebugLogger.LogWorldExporter($"   🔹 Position: {obj.transform.position}");
                DebugLogger.LogWorldExporter($"   🔹 Local Position: {obj.transform.localPosition}");
                DebugLogger.LogWorldExporter($"   🔹 Local Rotation: {obj.transform.localEulerAngles}");
                DebugLogger.LogWorldExporter($"   🔹 Local Scale: {obj.transform.localScale}");
                DebugLogger.LogWorldExporter($"   🔹 Parent: {(obj.transform.parent ? obj.transform.parent.name : "None")}");
                DebugLogger.LogWorldExporter($"   🔹 Children: {obj.transform.childCount}");
                
                // Check if it has POTCOTypeInfo
                var potcoInfo = obj.GetComponent<POTCO.POTCOTypeInfo>();
                DebugLogger.LogWorldExporter($"   🔹 Has POTCOTypeInfo: {potcoInfo != null}");
                
                // Check if it would be skipped by the rules
                bool wouldBeSkipped = POTCO.Editor.AutoPOTCODetection.IsChildMeshObjectPublic(obj);
                DebugLogger.LogWorldExporter($"   🔹 Would be skipped by rules: {wouldBeSkipped}");
                
                // Check if it looks like a POTCO model
                bool looksLikePOTCO = obj.name.ToLower().Contains("interior_"); // simplified check
                DebugLogger.LogWorldExporter($"   🔹 Looks like POTCO model: {looksLikePOTCO}");
                
                // Check mesh components
                var meshRenderer = obj.GetComponent<MeshRenderer>();
                var meshFilter = obj.GetComponent<MeshFilter>();
                DebugLogger.LogWorldExporter($"   🔹 Has MeshRenderer: {meshRenderer != null}");
                DebugLogger.LogWorldExporter($"   🔹 Has MeshFilter: {meshFilter != null}");
                
                // List children
                if (obj.transform.childCount > 0)
                {
                    DebugLogger.LogWorldExporter($"   🔹 Children:");
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        var child = obj.transform.GetChild(i);
                        var childPOTCO = child.GetComponent<POTCO.POTCOTypeInfo>();
                        DebugLogger.LogWorldExporter($"      - '{child.name}' (POTCOTypeInfo: {childPOTCO != null})");
                    }
                }
            }
            
            DebugLogger.LogWorldExporter("\n🔧 Use this information to understand why POTCOTypeInfo is being added incorrectly");
        }
        
        /// <summary>
        /// Set visibility of mesh objects in the hierarchy
        /// </summary>
        public static void SetMeshObjectsVisibility(bool visible)
        {
            var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int hiddenCount = 0;
            int shownCount = 0;
            
            foreach (var obj in allObjects)
            {
                if (IsMeshObject(obj))
                {
                    // Unity's HideFlags control hierarchy visibility
                    if (visible)
                    {
                        // Show the object
                        obj.hideFlags &= ~HideFlags.HideInHierarchy;
                        shownCount++;
                    }
                    else
                    {
                        // Hide the object
                        obj.hideFlags |= HideFlags.HideInHierarchy;
                        hiddenCount++;
                    }
                }
            }
            
            // Refresh the hierarchy window
            EditorApplication.RepaintHierarchyWindow();
            
            if (visible)
            {
                DebugLogger.LogWorldExporter($"👁️ Showed {shownCount} child/secondary objects in hierarchy");
            }
            else
            {
                DebugLogger.LogWorldExporter($"🎯 Hidden {hiddenCount} child/secondary objects - showing only primary objects");
            }
        }
        
        /// <summary>
        /// Check if a GameObject should be hidden (only showing primary GameObjects)
        /// </summary>
        private static bool IsMeshObject(GameObject obj)
        {
            // NEVER hide objects with POTCOTypeInfo (these are primary objects)
            if (obj.GetComponent<POTCOTypeInfo>() != null) return false;
            
            // NEVER hide important Unity objects
            if (IsImportantUnityObject(obj)) return false;
            
            // HIDE: Any object that has a parent (not a root object)
            if (obj.transform.parent != null)
            {
                return true;
            }
            
            // HIDE: Objects that look like imported model containers without POTCOTypeInfo
            string name = obj.name.ToLower();
            if (name.Contains("interior_") || name.Contains("exterior_") || 
                name.StartsWith("pir_") || name.Contains("_m_") ||
                name.Contains("building") || name.Contains("prop"))
            {
                // Only show if it has POTCOTypeInfo, otherwise hide
                return obj.GetComponent<POTCOTypeInfo>() == null;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this is an important Unity object that should never be hidden
        /// </summary>
        private static bool IsImportantUnityObject(GameObject obj)
        {
            // Never hide cameras, lights, audio sources, etc.
            if (obj.GetComponent<Camera>() != null) return true;
            if (obj.GetComponent<Light>() != null) return true;
            if (obj.GetComponent<AudioSource>() != null) return true;
            if (obj.GetComponent<Canvas>() != null) return true;
            if (obj.GetComponent<UnityEngine.EventSystems.EventSystem>() != null) return true;
            
            // Never hide objects with certain names
            string name = obj.name.ToLower();
            if (name.StartsWith("main camera") || name.StartsWith("directional light") ||
                name.StartsWith("canvas") || name.StartsWith("eventsystem")) return true;
            
            return false;
        }

        private void DrawStatistics()
        {
            if (lastExportStats == null) return;

            EditorGUILayout.BeginVertical("box");

            showStatistics = EditorGUILayout.Foldout(showStatistics, $"📊 Last Export Statistics ({lastExportStats.exportTime:F2}s)", true);
            if (showStatistics)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Total Objects Exported:", lastExportStats.totalObjectsExported.ToString());
                EditorGUILayout.LabelField("Lighting Objects:", lastExportStats.lightingObjectsExported.ToString());
                EditorGUILayout.LabelField("Collision Objects:", lastExportStats.collisionObjectsExported.ToString());
                EditorGUILayout.LabelField("Node Objects:", lastExportStats.nodeObjectsExported.ToString());
                EditorGUILayout.LabelField("File Size:", $"{lastExportStats.fileSizeKB:F1} KB");

                if (lastExportStats.objectTypeCount.Count > 0)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Exported Object Types:", EditorStyles.boldLabel);
                    foreach (var kvp in lastExportStats.objectTypeCount)
                    {
                        EditorGUILayout.LabelField($"  {kvp.Key}:", kvp.Value.ToString());
                    }
                }

                if (lastExportStats.warnings.Count > 0)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Warnings:", EditorStyles.boldLabel);
                    foreach (string warning in lastExportStats.warnings)
                    {
                        EditorGUILayout.LabelField($"  ⚠️ {warning}");
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DebugSceneHierarchy()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string fileName = $"POTCOAutoDetection_Debug_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = System.IO.Path.Combine("Assets/Editor/World Data Exporter/", fileName);
            
            var output = new System.Text.StringBuilder();
            output.AppendLine("=== 🔍 POTCO AUTO-DETECTION DEBUG ANALYSIS ===");
            output.AppendLine($"Scene: {sceneName}");
            output.AppendLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            output.AppendLine();
            output.AppendLine("This debug analyzes why POTCOTypeInfo components are being applied to certain objects.");
            output.AppendLine("Focus: Understanding parent-child relationships and auto-detection logic.");
            output.AppendLine();
            
            // Get all objects in the scene
            var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int totalObjects = 0;
            int objectsWithPOTCOInfo = 0;
            int potentialTargets = 0;
            int incorrectPlacements = 0;
            
            output.AppendLine("=== 📋 OBJECTS ANALYSIS ===");
            
            foreach (var obj in allObjects)
            {
                totalObjects++;
                bool hasPOTCOInfo = obj.GetComponent<POTCOTypeInfo>() != null;
                if (hasPOTCOInfo) objectsWithPOTCOInfo++;
                
                // Analyze this object using the same logic as auto-detection
                bool shouldSkip = ShouldSkipObjectDebug(obj, output);
                bool isChildMesh = IsChildMeshObjectDebug(obj, output);
                bool looksLikePOTCO = LooksLikePOTCOModelDebug(obj, output);
                bool wouldGetPOTCO = !shouldSkip && !isChildMesh && looksLikePOTCO;
                
                if (wouldGetPOTCO) potentialTargets++;
                
                // Check for incorrect placements
                if (hasPOTCOInfo && isChildMesh)
                {
                    incorrectPlacements++;
                    output.AppendLine($"❌ INCORRECT PLACEMENT: '{GetObjectPath(obj)}'");
                    output.AppendLine($"   └─ Has POTCOTypeInfo but is identified as child mesh object");
                }
                
                // Log detailed analysis for objects that have POTCOInfo or would get it
                if (hasPOTCOInfo || wouldGetPOTCO || isChildMesh)
                {
                    string status = hasPOTCOInfo ? "HAS_POTCO" : (wouldGetPOTCO ? "WOULD_GET" : "CHILD_MESH");
                    string icon = hasPOTCOInfo ? "✅" : (wouldGetPOTCO ? "🎯" : "🔧");
                    
                    output.AppendLine($"{icon} {status}: '{GetObjectPath(obj)}'");
                    output.AppendLine($"   ├─ ShouldSkip: {shouldSkip}");
                    output.AppendLine($"   ├─ IsChildMesh: {isChildMesh}");
                    output.AppendLine($"   ├─ LooksLikePOTCO: {looksLikePOTCO}");
                    output.AppendLine($"   ├─ HasMeshRenderer: {obj.GetComponent<MeshRenderer>() != null}");
                    output.AppendLine($"   ├─ HasMeshFilter: {obj.GetComponent<MeshFilter>() != null}");
                    output.AppendLine($"   ├─ Parent: {(obj.transform.parent?.name ?? "None")}");
                    output.AppendLine($"   ├─ Children: {obj.transform.childCount}");
                    
                    if (hasPOTCOInfo)
                    {
                        var potcoInfo = obj.GetComponent<POTCOTypeInfo>();
                        output.AppendLine($"   ├─ ObjectType: '{potcoInfo.objectType}'");
                        output.AppendLine($"   ├─ ObjectId: '{potcoInfo.objectId}'");
                        output.AppendLine($"   └─ ModelPath: '{potcoInfo.modelPath}'");
                    }
                    else
                    {
                        output.AppendLine($"   └─ WouldApplyPOTCO: {wouldGetPOTCO}");
                    }
                    output.AppendLine();
                }
            }
            
            // Hierarchy analysis
            output.AppendLine("=== 🌳 HIERARCHY ANALYSIS ===");
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootObj in rootObjects)
            {
                AnalyzeHierarchyRecursive(rootObj, 0, output);
            }
            
            output.AppendLine();
            output.AppendLine("=== 📊 SUMMARY ===");
            output.AppendLine($"Total Objects: {totalObjects}");
            output.AppendLine($"Objects with POTCOTypeInfo: {objectsWithPOTCOInfo}");
            output.AppendLine($"Objects that would get POTCOTypeInfo: {potentialTargets}");
            output.AppendLine($"Incorrectly placed POTCOTypeInfo: {incorrectPlacements}");
            
            if (incorrectPlacements > 0)
            {
                output.AppendLine();
                output.AppendLine("🛠️ RECOMMENDATIONS:");
                output.AppendLine("- Run 'POTCO > Clean Up Incorrectly Placed POTCOTypeInfo' to fix issues");
                output.AppendLine("- Check parent-child relationships for objects with meshes");
            }
            
            output.AppendLine();
            output.AppendLine("=== 🔍 DEBUG ANALYSIS COMPLETE ===");
            
            // Write to file
            try
            {
                System.IO.File.WriteAllText(filePath, output.ToString());
                DebugLogger.LogWorldExporter($"✅ POTCO Auto-Detection debug exported to: {filePath}");
                
                // Also log key findings to console
                DebugLogger.LogWorldExporter($"📊 POTCO Debug Summary: {objectsWithPOTCOInfo} objects have POTCOTypeInfo, {incorrectPlacements} incorrectly placed");
                
                // Refresh the asset database so the file appears in Unity
                AssetDatabase.Refresh();
                
                // Ping the file in the project window
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
                if (asset != null)
                {
                    EditorGUIUtility.PingObject(asset);
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogErrorWorldExporter($"❌ Failed to export POTCO debug: {ex.Message}");
            }
        }
        
        private bool ShouldSkipObjectDebug(GameObject obj, System.Text.StringBuilder output)
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
            if (obj.GetComponent<Light>() != null && !LooksLikePOTCOLightDebug(obj)) return true;
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
        
        private bool IsChildMeshObjectDebug(GameObject obj, System.Text.StringBuilder output)
        {
            // If this object has a parent, check if it's likely a child mesh
            if (obj.transform.parent != null)
            {
                // If this object has mesh components but the parent looks like the main POTCO object
                if ((obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<MeshFilter>() != null))
                {
                    GameObject parent = obj.transform.parent.gameObject;
                    
                    // If parent has a POTCO-like name or already has POTCOTypeInfo
                    if (LooksLikePOTCOModelDebug(parent, output) || parent.GetComponent<POTCOTypeInfo>() != null)
                    {
                        return true;
                    }
                    
                    // If this object's name suggests it's a child mesh (common patterns)
                    string name = obj.name.ToLower();
                    if (name.Contains("mesh") || name.Contains("geometry") || name.Contains("model") || 
                        name.Contains("_geo") || name.Contains("_mesh") || name.Contains("lod"))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        private bool LooksLikePOTCOModelDebug(GameObject obj, System.Text.StringBuilder output)
        {
            string name = obj.name.ToLower();
            
            // POTCO models often have specific naming patterns
            if (name.StartsWith("pir_")) return true;
            if (name.Contains("_m_")) return true; // Model indicator
            if (name.Contains("_prp_")) return true; // Prop indicator
            if (name.Contains("_chr_")) return true; // Character indicator
            if (name.Contains("_bld_")) return true; // Building indicator
            if (name.Contains("_env_")) return true; // Environment indicator
            
            // Check if it has mesh components (visual objects)
            if (obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<MeshFilter>() != null) 
            {
                // If it has a mesh and doesn't look like a Unity primitive, it's probably a POTCO model
                if (!IsUnityPrimitiveDebug(obj)) return true;
            }
            
            // Check if any children have mesh components
            MeshRenderer[] childMeshes = obj.GetComponentsInChildren<MeshRenderer>();
            if (childMeshes.Length > 0)
            {
                // If it has child meshes and the name suggests it's a POTCO object
                if (name.Contains("crate") || name.Contains("barrel") || name.Contains("chest") ||
                    name.Contains("table") || name.Contains("chair") || name.Contains("torch") ||
                    name.Contains("tree") || name.Contains("rock") || name.Contains("building") ||
                    name.Contains("ship") || name.Contains("weapon")) return true;
            }
            
            return false;
        }
        
        private bool LooksLikePOTCOLightDebug(GameObject obj)
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
        
        private bool IsUnityPrimitiveDebug(GameObject obj)
        {
            string name = obj.name.ToLower();
            string[] primitives = { "cube", "sphere", "capsule", "cylinder", "plane", "quad" };
            
            foreach (string primitive in primitives)
            {
                if (name.Equals(primitive) || name.StartsWith(primitive + " ")) return true;
            }
            
            return false;
        }
        
        private string GetObjectPath(GameObject obj)
        {
            if (obj == null) return "null";
            
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        private void AnalyzeHierarchyRecursive(GameObject obj, int depth, System.Text.StringBuilder output)
        {
            string indent = new string(' ', depth * 2);
            
            // Get POTCOTypeInfo status
            bool hasPOTCO = obj.GetComponent<POTCOTypeInfo>() != null;
            bool shouldSkip = ShouldSkipObjectDebug(obj, output);
            bool isChildMesh = IsChildMeshObjectDebug(obj, output);
            bool looksLikePOTCO = LooksLikePOTCOModelDebug(obj, output);
            bool wouldGetPOTCO = !shouldSkip && !isChildMesh && looksLikePOTCO;
            
            // Choose icon and status
            string icon = "📦";
            string status = "";
            
            if (hasPOTCO)
            {
                icon = isChildMesh ? "❌" : "✅";
                status = isChildMesh ? " (INCORRECT - CHILD MESH)" : " (HAS POTCO)";
            }
            else if (wouldGetPOTCO)
            {
                icon = "🎯";
                status = " (WOULD GET POTCO)";
            }
            else if (isChildMesh)
            {
                icon = "🔧";
                status = " (CHILD MESH)";
            }
            
            output.AppendLine($"{indent}{icon} {obj.name}{status}");
            
            // Show detailed analysis for problematic objects
            if ((hasPOTCO && isChildMesh) || wouldGetPOTCO)
            {
                output.AppendLine($"{indent}   ├─ Components: {string.Join(", ", obj.GetComponents<Component>().Where(c => !(c is Transform)).Select(c => c.GetType().Name))}");
                output.AppendLine($"{indent}   ├─ Parent: {(obj.transform.parent?.name ?? "None")}");
                output.AppendLine($"{indent}   └─ Children: {obj.transform.childCount}");
            }
            
            // Recurse through children
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                AnalyzeHierarchyRecursive(obj.transform.GetChild(i).gameObject, depth + 1, output);
            }
        }
    }
}
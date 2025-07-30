using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace POTCO.Editor
{
    /// <summary>
    /// Advanced surface placement tool with raycasting for precise object placement
    /// </summary>
    public static class SurfacePlacementTool
    {
        private static GameObject previewObject;
        private static Material previewMaterial;
        private static bool isDragging = false;
        private static bool isEnabled = false;

        // Placement settings
        public static bool snapToSurface = true;
        public static bool alignToSurfaceNormal = true;
        public static bool preventOverlap = true;
        public static bool autoAddColliders = true; // Automatically add MeshColliders to objects without them
        public static float overlapCheckRadius = 0.5f;
        public static LayerMask surfaceLayerMask = -1; // All layers by default
        public static LayerMask excludeLayerMask = 0; // No layers excluded by default

        /// <summary>
        /// Check if the surface placement tool is currently enabled
        /// </summary>
        public static bool IsEnabled => isEnabled;

        /// <summary>
        /// Enable the surface placement tool
        /// </summary>
        public static void Enable()
        {
            if (!isEnabled)
            {
                isEnabled = true;
                SceneView.duringSceneGui += OnSceneGUI;
                CreatePreviewMaterial();
                
                // Subscribe to hierarchy changes to detect new objects
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
                
                // Automatically add colliders to objects without them if enabled
                if (autoAddColliders)
                {
                    EnsureSceneObjectsHaveColliders();
                }
                
                DebugLogger.LogAlways("🎯 Surface Placement Tool ENABLED");
            }
        }

        /// <summary>
        /// Disable the surface placement tool
        /// </summary>
        public static void Disable()
        {
            if (isEnabled)
            {
                isEnabled = false;
                SceneView.duringSceneGui -= OnSceneGUI;
                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                ClearPreview();
                DebugLogger.LogAlways("🎯 Surface Placement Tool DISABLED");
            }
        }

        /// <summary>
        /// Start placing an object with surface snapping
        /// </summary>
        public static void StartPlacement(GameObject prefab)
        {
            if (prefab == null) return;
            
            DebugLogger.LogAlways($"🎯 Starting surface placement for: {prefab.name}");
            Enable();
            CreatePreview(prefab);
            isDragging = true;
        }

        /// <summary>
        /// Place object at world position with surface snapping
        /// </summary>
        public static GameObject PlaceAtPosition(GameObject prefab, Vector3 worldPosition, bool useSnapping = true)
        {
            if (prefab == null) return null;

            Vector3 finalPosition = worldPosition;
            Quaternion finalRotation = prefab.transform.rotation;

            if (useSnapping && snapToSurface)
            {
                var hitInfo = GetSurfaceAtPosition(worldPosition);
                if (hitInfo.HasValue)
                {
                    finalPosition = hitInfo.Value.point;
                    
                    if (alignToSurfaceNormal)
                    {
                        finalRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.Value.normal) * prefab.transform.rotation;
                    }
                }
            }

            // Check for overlaps if enabled
            if (preventOverlap && CheckForOverlap(finalPosition, prefab))
            {
                Debug.LogWarning($"Cannot place {prefab.name} - overlaps with existing object");
                return null;
            }

            // Create the object
            GameObject newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            newObject.transform.position = finalPosition;
            newObject.transform.rotation = finalRotation;

            // Add ObjectListInfo if it doesn't exist
            if (newObject.GetComponent<ObjectListInfo>() == null)
            {
                var objectInfo = newObject.AddComponent<ObjectListInfo>();
                objectInfo.modelPath = $"models/props/{prefab.name}";
                objectInfo.objectType = "MISC_OBJ";
            }

            // Add colliders if needed for surface detection
            if (autoAddColliders)
            {
                EnsureObjectHasCollider(newObject);
            }

            // Record undo
            Undo.RegisterCreatedObjectUndo(newObject, $"Place {prefab.name}");
            
            // Select the new object
            Selection.activeGameObject = newObject;

            return newObject;
        }

        /// <summary>
        /// Get surface information at a world position using raycasting
        /// </summary>
        public static RaycastHit? GetSurfaceAtPosition(Vector3 worldPosition)
        {
            // Simple downward raycast from above the position
            Vector3 rayStart = worldPosition + Vector3.up * 1000f;
            Ray ray = new Ray(rayStart, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 2000f, surfaceLayerMask & ~excludeLayerMask))
            {
                return hit;
            }

            return null;
        }

        /// <summary>
        /// Get surface at mouse position (optimized for interactive placement)
        /// </summary>
        public static RaycastHit? GetSurfaceAtMousePosition(Vector2 mousePos)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);

            // Simple Physics.Raycast - works reliably since we ensure all objects have colliders
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, surfaceLayerMask & ~excludeLayerMask))
            {
                return hit;
            }

            return null;
        }

        /// <summary>
        /// Check for overlapping objects at position
        /// </summary>
        private static bool CheckForOverlap(Vector3 position, GameObject prefab)
        {
            if (!preventOverlap) return false;

            // Get bounds of the prefab
            Bounds prefabBounds = GetObjectBounds(prefab);
            Bounds checkBounds = new Bounds(position + prefabBounds.center, prefabBounds.size);

            // Check for overlapping colliders
            Collider[] overlapping = Physics.OverlapBox(
                checkBounds.center,
                checkBounds.extents,
                prefab.transform.rotation,
                surfaceLayerMask & ~excludeLayerMask
            );

            // Filter out the surface we're placing on (if it's a ground plane, etc.)
            foreach (var collider in overlapping)
            {
                // Skip if it's a large ground plane (likely terrain)
                if (collider.bounds.size.x > 100f || collider.bounds.size.z > 100f)
                    continue;

                // Skip if it's significantly below our placement position
                if (collider.bounds.max.y < position.y - 0.1f)
                    continue;

                return true; // Found overlap
            }

            return false;
        }

        /// <summary>
        /// Get bounds of an object including all child renderers
        /// </summary>
        private static Bounds GetObjectBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.one);

            Bounds bounds = renderers[0].bounds;
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Scene GUI handler for interactive placement
        /// </summary>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!isEnabled) return;

            Event evt = Event.current;

            if (isDragging && previewObject != null)
            {
                // Update preview position based on mouse
                Vector2 mousePos = evt.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);

                // Raycast to find surface
                Vector3 targetPosition = ray.origin + ray.direction * 10f;
                Quaternion targetRotation = previewObject.transform.rotation;

                if (snapToSurface)
                {
                    var hitInfo = GetSurfaceAtMousePosition(mousePos);
                    if (hitInfo.HasValue)
                    {
                        targetPosition = hitInfo.Value.point;
                        
                        if (alignToSurfaceNormal)
                        {
                            targetRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.Value.normal) * previewObject.transform.rotation;
                        }
                    }
                }

                // Update preview object
                previewObject.transform.position = targetPosition;
                previewObject.transform.rotation = targetRotation;

                // Check for overlaps and change preview color
                bool hasOverlap = preventOverlap && CheckForOverlap(targetPosition, previewObject);
                UpdatePreviewMaterial(hasOverlap);

                // Handle placement
                if (evt.type == EventType.MouseDown && evt.button == 0)
                {
                    if (!hasOverlap)
                    {
                        // Place the object
                        var originalPrefab = GetOriginalPrefab(previewObject);
                        if (originalPrefab != null)
                        {
                            PlaceAtPosition(originalPrefab, targetPosition, false); // Already calculated position
                        }
                    }
                    
                    StopPlacement();
                    evt.Use();
                }
                else if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Escape)
                {
                    StopPlacement();
                    evt.Use();
                }

                // Force scene view repaint
                sceneView.Repaint();
            }
        }

        /// <summary>
        /// Create preview object for placement
        /// </summary>
        private static void CreatePreview(GameObject prefab)
        {
            ClearPreview();
            
            previewObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            previewObject.name = "[SURFACE_PREVIEW] " + previewObject.name;
            previewObject.hideFlags = HideFlags.HideAndDontSave;

            // Disable colliders and make semi-transparent
            var renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }
                renderer.sharedMaterials = materials;
            }

            var colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }

        /// <summary>
        /// Update preview material based on placement validity
        /// </summary>
        private static void UpdatePreviewMaterial(bool hasError)
        {
            if (previewMaterial == null) CreatePreviewMaterial();

            Color targetColor = hasError ? new Color(1f, 0.3f, 0.3f, 0.6f) : new Color(0.3f, 1f, 0.3f, 0.6f);
            previewMaterial.color = targetColor;
        }

        /// <summary>
        /// Create semi-transparent preview material
        /// </summary>
        private static void CreatePreviewMaterial()
        {
            if (previewMaterial == null)
            {
                previewMaterial = new Material(Shader.Find("Standard"));
                previewMaterial.color = new Color(0.3f, 1f, 0.3f, 0.6f);
                previewMaterial.SetFloat("_Mode", 3); // Transparent mode
                previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                previewMaterial.SetInt("_ZWrite", 0);
                previewMaterial.DisableKeyword("_ALPHATEST_ON");
                previewMaterial.EnableKeyword("_ALPHABLEND_ON");
                previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                previewMaterial.renderQueue = 3000;
            }
        }

        /// <summary>
        /// Get the original prefab from a preview object
        /// </summary>
        private static GameObject GetOriginalPrefab(GameObject previewObj)
        {
            if (previewObj == null) return null;
            
            string originalName = previewObj.name.Replace("[SURFACE_PREVIEW] ", "");
            return PrefabUtility.GetCorrespondingObjectFromSource(previewObj);
        }

        /// <summary>
        /// Stop current placement operation
        /// </summary>
        private static void StopPlacement()
        {
            isDragging = false;
            ClearPreview();
        }

        /// <summary>
        /// Clear preview object
        /// </summary>
        private static void ClearPreview()
        {
            if (previewObject != null)
            {
                Object.DestroyImmediate(previewObject);
                previewObject = null;
            }
        }

        /// <summary>
        /// Called when hierarchy changes - detect new objects and add colliders
        /// </summary>
        private static void OnHierarchyChanged()
        {
            if (!isEnabled || !autoAddColliders) return;

            // Delay the collider check to avoid issues during object creation
            EditorApplication.delayCall += () =>
            {
                if (isEnabled && autoAddColliders)
                {
                    // Check all objects in the scene for missing colliders
                    var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                    int collidersAdded = 0;

                    foreach (var obj in allObjects)
                    {
                        // Skip preview objects and objects that are part of prefabs in project
                        if (obj.name.Contains("[PREVIEW]") || obj.scene.name == null) continue;

                        if (EnsureObjectHasCollider(obj))
                        {
                            collidersAdded++;
                        }
                    }

                    if (collidersAdded > 0)
                    {
                        DebugLogger.LogAlways($"🎯 Auto-added {collidersAdded} MeshColliders to new scene objects");
                    }
                }
            };
        }

        /// <summary>
        /// Automatically add MeshColliders to objects that don't have colliders
        /// </summary>
        private static void EnsureSceneObjectsHaveColliders()
        {
            var meshRenderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
            int collidersAdded = 0;

            foreach (var renderer in meshRenderers)
            {
                if (EnsureObjectHasCollider(renderer.gameObject))
                {
                    collidersAdded++;
                }
            }

            if (collidersAdded > 0)
            {
                DebugLogger.LogAlways($"🎯 Added {collidersAdded} MeshColliders to scene objects for surface detection");
            }
        }

        /// <summary>
        /// Add MeshCollider to a specific object if it doesn't have one
        /// </summary>
        private static bool EnsureObjectHasCollider(GameObject obj)
        {
            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            int collidersAdded = 0;

            foreach (var renderer in meshRenderers)
            {
                // Skip if object already has a collider
                if (renderer.GetComponent<Collider>() != null) continue;

                // Skip if no mesh filter or mesh
                var meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null) continue;

                // Skip very small or simple meshes (likely UI elements)
                if (meshFilter.sharedMesh.vertexCount < 4) continue;

                // Add a MeshCollider
                var meshCollider = renderer.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                meshCollider.convex = false; // Non-convex for better surface detection
                
                collidersAdded++;
                DebugLogger.LogAlways($"🎯 Added MeshCollider to '{renderer.gameObject.name}' for surface detection");
            }

            return collidersAdded > 0;
        }

        /// <summary>
        /// Settings GUI for editor windows
        /// </summary>
        public static void DrawSettingsGUI()
        {
            EditorGUILayout.LabelField("Surface Placement Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            snapToSurface = EditorGUILayout.Toggle("Snap to Surface", snapToSurface);
            
            if (snapToSurface)
            {
                EditorGUI.indentLevel++;
                alignToSurfaceNormal = EditorGUILayout.Toggle("Align to Surface Normal", alignToSurfaceNormal);
                autoAddColliders = EditorGUILayout.Toggle("Auto Add Colliders", autoAddColliders);
                EditorGUI.indentLevel--; 
            }

            preventOverlap = EditorGUILayout.Toggle("Prevent Overlap", preventOverlap);
            
            if (preventOverlap)
            {
                EditorGUI.indentLevel++;
                overlapCheckRadius = EditorGUILayout.FloatField("Overlap Check Radius", overlapCheckRadius);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Layer Settings", EditorStyles.miniLabel);
            surfaceLayerMask = EditorGUILayout.MaskField("Surface Layers", surfaceLayerMask, UnityEditorInternal.InternalEditorUtility.layers);
            excludeLayerMask = EditorGUILayout.MaskField("Exclude Layers", excludeLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button(isEnabled ? "Disable Surface Tool" : "Enable Surface Tool"))
            {
                if (isEnabled) Disable();
                else Enable();
            }
        }
    }
}
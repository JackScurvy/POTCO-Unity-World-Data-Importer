using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using POTCO;

namespace POTCO.Editor
{
    /// <summary>
    /// Quick placement tool for rapid level editing with hotkeys and smart placement
    /// </summary>
    public class QuickPlaceTool : EditorWindow
    {
        private class QuickSlot
        {
            public GameObject prefab;
            public string name;
            public KeyCode hotkey;
            public int useCount;
        }

        // Quick slots (1-9 keys)
        private QuickSlot[] quickSlots = new QuickSlot[9];
        private Vector2 scrollPosition;
        private bool isPlacementMode = false;
        private GameObject previewObject;
        private Material previewMaterial;

        // Placement settings
        private bool snapToGrid = true;
        private float gridSize = 1.0f;
        private bool alignToSurface = true;
        private bool randomRotation = false;
        private Vector2 rotationRange = new Vector2(0, 360);

        [MenuItem("POTCO/Quick Place Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<QuickPlaceTool>("Quick Place");
            window.minSize = new Vector2(300, 200);
            window.Show();
        }

        private void OnEnable()
        {
            LoadQuickSlots();
            CreatePreviewMaterial();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SaveQuickSlots();
            SceneView.duringSceneGui -= OnSceneGUI;
            
            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawQuickSlots();
            EditorGUILayout.Space(10);
            
            DrawPlacementSettings();
            EditorGUILayout.Space(10);
            
            DrawInstructions();

            EditorGUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("⚡ Quick Place Tool", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Assign props to slots 1-9 for instant placement", EditorStyles.miniLabel);
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button(isPlacementMode ? "🔒 Exit Placement Mode" : "🎯 Enter Placement Mode"))
            {
                TogglePlacementMode();
            }
            
            if (GUILayout.Button("🔄 Clear All Slots"))
            {
                if (EditorUtility.DisplayDialog("Clear Quick Slots", "Are you sure you want to clear all quick slots?", "Clear", "Cancel"))
                {
                    ClearAllSlots();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawQuickSlots()
        {
            EditorGUILayout.LabelField("Quick Slots", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            for (int i = 0; i < quickSlots.Length; i++)
            {
                DrawQuickSlot(i);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawQuickSlot(int index)
        {
            if (quickSlots[index] == null)
            {
                quickSlots[index] = new QuickSlot
                {
                    hotkey = (KeyCode)((int)KeyCode.Alpha1 + index),
                    useCount = 0
                };
            }

            var slot = quickSlots[index];
            
            EditorGUILayout.BeginHorizontal("box");
            
            // Slot number and hotkey
            GUILayout.Label($"{index + 1}", EditorStyles.boldLabel, GUILayout.Width(20));
            GUILayout.Label($"[{slot.hotkey}]", EditorStyles.miniLabel, GUILayout.Width(35));
            
            // Prefab field
            EditorGUI.BeginChangeCheck();
            GameObject newPrefab = EditorGUILayout.ObjectField(slot.prefab, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                slot.prefab = newPrefab;
                slot.name = newPrefab != null ? newPrefab.name : "";
                SaveQuickSlots();
            }
            
            // Usage count
            if (slot.useCount > 0)
            {
                GUILayout.Label($"({slot.useCount})", EditorStyles.miniLabel, GUILayout.Width(30));
            }
            
            // Quick place button
            EditorGUI.BeginDisabledGroup(slot.prefab == null);
            if (GUILayout.Button("Place", GUILayout.Width(50)))
            {
                PlaceObjectAtCursor(slot);
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPlacementSettings()
        {
            EditorGUILayout.LabelField("Placement Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid);
            if (snapToGrid)
            {
                EditorGUI.indentLevel++;
                gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
                EditorGUI.indentLevel--;
            }
            
            alignToSurface = EditorGUILayout.Toggle("Align to Surface", alignToSurface);
            
            randomRotation = EditorGUILayout.Toggle("Random Rotation", randomRotation);
            if (randomRotation)
            {
                EditorGUI.indentLevel++;
                rotationRange = EditorGUILayout.Vector2Field("Y Rotation Range (degrees)", rotationRange);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawInstructions()
        {
            EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField("• Drag prefabs into slots 1-9", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Press keys 1-9 to place objects", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Hold Shift + key for placement mode", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Mouse position determines placement", EditorStyles.miniLabel);
            
            if (isPlacementMode)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("🎯 PLACEMENT MODE ACTIVE", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("• Move mouse in Scene view", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("• Click to place object", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("• Press Escape to exit", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event evt = Event.current;
            
            // Handle hotkeys
            if (evt.type == EventType.KeyDown && !evt.control && !evt.alt)
            {
                for (int i = 0; i < quickSlots.Length; i++)
                {
                    if (evt.keyCode == quickSlots[i].hotkey)
                    {
                        if (evt.shift)
                        {
                            // Shift + key enters placement mode for that slot
                            EnterPlacementModeForSlot(i);
                        }
                        else if (!isPlacementMode)
                        {
                            // Direct placement
                            PlaceObjectAtCursor(quickSlots[i]);
                        }
                        evt.Use();
                        break;
                    }
                }
                
                // Escape to exit placement mode
                if (evt.keyCode == KeyCode.Escape && isPlacementMode)
                {
                    ExitPlacementMode();
                    evt.Use();
                }
            }
            
            // Handle placement mode
            if (isPlacementMode)
            {
                HandlePlacementMode(evt);
            }
        }

        private void HandlePlacementMode(Event evt)
        {
            // Update preview object position
            Vector3 mousePosition = evt.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            
            Vector3 targetPosition = ray.origin + ray.direction * 10f;
            
            // Raycast to find surface
            if (alignToSurface && Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
            }
            
            // Apply grid snapping
            if (snapToGrid)
            {
                targetPosition = SnapToGrid(targetPosition);
            }
            
            // Update preview
            if (previewObject != null)
            {
                previewObject.transform.position = targetPosition;
                
                if (randomRotation)
                {
                    // Show potential random rotation
                    float randomY = Random.Range(rotationRange.x, rotationRange.y);
                    previewObject.transform.rotation = Quaternion.Euler(0, randomY, 0);
                }
            }
            
            // Handle placement click
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                PlaceObjectAtPosition(targetPosition);
                evt.Use();
            }
            
            // Keep scene view updated
            SceneView.RepaintAll();
        }

        private void TogglePlacementMode()
        {
            if (isPlacementMode)
            {
                ExitPlacementMode();
            }
            else
            {
                // Enter placement mode with first available slot
                for (int i = 0; i < quickSlots.Length; i++)
                {
                    if (quickSlots[i] != null && quickSlots[i].prefab != null)
                    {
                        EnterPlacementModeForSlot(i);
                        break;
                    }
                }
            }
        }

        private void EnterPlacementModeForSlot(int slotIndex)
        {
            if (quickSlots[slotIndex]?.prefab == null) return;
            
            isPlacementMode = true;
            
            // Create preview object
            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
            }
            
            previewObject = Instantiate(quickSlots[slotIndex].prefab);
            previewObject.name = "[PREVIEW] " + previewObject.name;
            previewObject.hideFlags = HideFlags.HideAndDontSave;
            
            // Make it semi-transparent
            ApplyPreviewMaterial(previewObject);
            
            // Disable colliders
            var colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            
            DebugLogger.LogAlways($"🎯 Entered placement mode for '{quickSlots[slotIndex].name}'");
        }

        private void ExitPlacementMode()
        {
            isPlacementMode = false;
            
            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
                previewObject = null;
            }
            
            DebugLogger.LogAlways("🔒 Exited placement mode");
        }

        private void PlaceObjectAtCursor(QuickSlot slot)
        {
            if (slot?.prefab == null) return;
            
            // Get mouse position in scene
            Vector2 mousePos = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            
            Vector3 position = ray.origin + ray.direction * 10f;
            
            // Try to place on surface
            if (alignToSurface && Physics.Raycast(ray, out RaycastHit hit))
            {
                position = hit.point;
            }
            
            PlaceObjectAtPosition(position, slot);
        }

        private void PlaceObjectAtPosition(Vector3 position, QuickSlot slot = null)
        {
            // Find the current slot if not provided
            if (slot == null && previewObject != null)
            {
                string prefabName = previewObject.name.Replace("[PREVIEW] ", "");
                slot = quickSlots.FirstOrDefault(s => s?.prefab?.name == prefabName);
            }
            
            if (slot?.prefab == null) return;
            
            // Apply grid snapping
            if (snapToGrid)
            {
                position = SnapToGrid(position);
            }
            
            // Create the object
            GameObject newObj = PrefabUtility.InstantiatePrefab(slot.prefab) as GameObject;
            newObj.transform.position = position;
            
            // Apply random rotation
            if (randomRotation)
            {
                float randomY = Random.Range(rotationRange.x, rotationRange.y);
                newObj.transform.rotation = Quaternion.Euler(0, randomY, 0);
            }
            
            // Add ObjectListInfo if needed
            if (newObj.GetComponent<ObjectListInfo>() == null)
            {
                var objectListInfo = newObj.AddComponent<ObjectListInfo>();
                objectListInfo.modelPath = $"models/props/{slot.prefab.name}";
                objectListInfo.objectType = "MISC_OBJ";
            }
            
            // Register for undo (IMPORTANT: This enables Ctrl+Z)
            Undo.RegisterCreatedObjectUndo(newObj, $"Quick Place {slot.prefab.name}");
            
            // Select the new object
            Selection.activeGameObject = newObj;
            
            // Update usage count
            slot.useCount++;
            SaveQuickSlots();
            
            DebugLogger.LogAlways($"⚡ Quick placed '{slot.name}' at {position}");
        }

        private Vector3 SnapToGrid(Vector3 position)
        {
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                Mathf.Round(position.y / gridSize) * gridSize,
                Mathf.Round(position.z / gridSize) * gridSize
            );
        }

        private void CreatePreviewMaterial()
        {
            if (previewMaterial == null)
            {
                previewMaterial = new Material(Shader.Find("Standard"));
                previewMaterial.color = new Color(0.5f, 1f, 0.5f, 0.5f);
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

        private void ApplyPreviewMaterial(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }
                renderer.sharedMaterials = materials;
            }
        }

        private void ClearAllSlots()
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                quickSlots[i] = new QuickSlot
                {
                    hotkey = (KeyCode)((int)KeyCode.Alpha1 + i),
                    useCount = 0
                };
            }
            SaveQuickSlots();
        }

        /// <summary>
        /// Public method to reload quick slots from EditorPrefs (called by PropBrowser)
        /// </summary>
        public void ReloadQuickSlots()
        {
            LoadQuickSlots();
            Repaint();
        }

        private void LoadQuickSlots()
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                string prefabPath = EditorPrefs.GetString($"QuickPlace_Slot{i}_Prefab", "");
                int useCount = EditorPrefs.GetInt($"QuickPlace_Slot{i}_UseCount", 0);
                
                DebugLogger.LogAlways($"⚡ Loading Quick Slot {i + 1}: key='QuickPlace_Slot{i}_Prefab', path='{prefabPath}'");
                
                quickSlots[i] = new QuickSlot
                {
                    prefab = !string.IsNullOrEmpty(prefabPath) ? AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) : null,
                    hotkey = (KeyCode)((int)KeyCode.Alpha1 + i),
                    useCount = useCount
                };
                
                if (quickSlots[i].prefab != null)
                {
                    quickSlots[i].name = quickSlots[i].prefab.name;
                    DebugLogger.LogAlways($"⚡ Successfully loaded '{quickSlots[i].name}' into Quick Slot {i + 1}");
                }
                else if (!string.IsNullOrEmpty(prefabPath))
                {
                    DebugLogger.LogAlways($"⚡ Failed to load prefab from path '{prefabPath}' for Quick Slot {i + 1}");
                }
            }
        }

        private void SaveQuickSlots()
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                if (quickSlots[i] != null)
                {
                    string prefabPath = quickSlots[i].prefab != null ? AssetDatabase.GetAssetPath(quickSlots[i].prefab) : "";
                    EditorPrefs.SetString($"QuickPlace_Slot{i}_Prefab", prefabPath);
                    EditorPrefs.SetInt($"QuickPlace_Slot{i}_UseCount", quickSlots[i].useCount);
                }
            }
        }
    }
}
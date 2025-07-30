using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using POTCO;
using WorldDataExporter.Utilities;

namespace POTCO.Editor
{
    [CustomEditor(typeof(POTCOTypeInfo))]
    public class POTCOTypeInfoEditor : UnityEditor.Editor
    {
        private SerializedProperty objectTypeProp;
        private SerializedProperty objectIdProp;
        private SerializedProperty modelPathProp;
        private SerializedProperty hasVisualBlockProp;
        private SerializedProperty disableCollisionProp;
        private SerializedProperty instancedProp;
        private SerializedProperty holidayProp;
        private SerializedProperty visSizeProp;
        private SerializedProperty autoDetectOnStartProp;
        private SerializedProperty autoGenerateIdProp;
        
        private List<string> availableObjectTypes;
        private int selectedTypeIndex = 0;
        
        private void OnEnable()
        {
            objectTypeProp = serializedObject.FindProperty("objectType");
            objectIdProp = serializedObject.FindProperty("objectId");
            modelPathProp = serializedObject.FindProperty("modelPath");
            hasVisualBlockProp = serializedObject.FindProperty("hasVisualBlock");
            disableCollisionProp = serializedObject.FindProperty("disableCollision");
            instancedProp = serializedObject.FindProperty("instanced");
            holidayProp = serializedObject.FindProperty("holiday");
            visSizeProp = serializedObject.FindProperty("visSize");
            autoDetectOnStartProp = serializedObject.FindProperty("autoDetectOnStart");
            autoGenerateIdProp = serializedObject.FindProperty("autoGenerateId");
            
            LoadAvailableObjectTypes();
        }
        
        private void LoadAvailableObjectTypes()
        {
            try
            {
                // Try to get object types from ObjectListParser first
                var rawObjectTypes = ObjectListParser.GetAllObjectTypes();
                if (rawObjectTypes == null || rawObjectTypes.Count == 0)
                {
                    throw new System.Exception("ObjectListParser returned empty list");
                }
                
                // Apply UI display mapping for user-friendly names
                availableObjectTypes = new List<string>();
                foreach (string type in rawObjectTypes)
                {
                    if (type == "MODULAR_OBJ")
                    {
                        availableObjectTypes.Add("Cave_Pieces");
                    }
                    else
                    {
                        availableObjectTypes.Add(type);
                    }
                }
                
                availableObjectTypes.Sort();
                
                // Find current selection index
                string currentType = objectTypeProp.stringValue;
                DebugLogger.LogAutoPOTCO($"🔍 Looking for current type '{currentType}' in dropdown with {availableObjectTypes.Count} options");
                selectedTypeIndex = availableObjectTypes.IndexOf(currentType);
                
                // If not found, try to find MISC_OBJ as a fallback
                if (selectedTypeIndex < 0)
                {
                    DebugLogger.LogAutoPOTCO($"⚠️ Current type '{currentType}' not found in dropdown, trying MISC_OBJ as fallback");
                    selectedTypeIndex = availableObjectTypes.IndexOf("MISC_OBJ");
                    if (selectedTypeIndex < 0) 
                    {
                        DebugLogger.LogAutoPOTCO($"⚠️ MISC_OBJ not found either, defaulting to index 0");
                        selectedTypeIndex = 0;
                    }
                    else
                    {
                        DebugLogger.LogAutoPOTCO($"✅ Found MISC_OBJ at index {selectedTypeIndex}");
                    }
                }
                else
                {
                    DebugLogger.LogAutoPOTCO($"✅ Found current type '{currentType}' at index {selectedTypeIndex}");
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogWarningAutoPOTCO($"Could not load object types from ObjectListParser: {ex.Message}");
                // Fall back to basic types from the runtime detector
                availableObjectTypes = POTCOObjectTypeDetector.GetBasicObjectTypes();
                availableObjectTypes.Sort();
                
                string currentType = objectTypeProp.stringValue;
                selectedTypeIndex = availableObjectTypes.IndexOf(currentType);
                if (selectedTypeIndex < 0) selectedTypeIndex = 0;
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            POTCOTypeInfo potcoInfo = (POTCOTypeInfo)target;
            
            EditorGUILayout.LabelField("POTCO Type Info", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Auto-Detection Settings
            EditorGUILayout.LabelField("Auto-Detection", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoDetectOnStartProp, new GUIContent("Auto-Detect on Start", "Automatically detect properties when component starts"));
            EditorGUILayout.PropertyField(autoGenerateIdProp, new GUIContent("Auto-Generate ID", "Automatically generate object ID when needed"));
            
            // Auto-detect button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 Auto-Detect All Properties", GUILayout.Height(25)))
            {
                POTCOObjectListIntegration.AutoDetectAllProperties(potcoInfo);
                EditorUtility.SetDirty(potcoInfo);
                serializedObject.Update();
                LoadAvailableObjectTypes(); // Refresh the dropdown
            }
            if (GUILayout.Button("🆔 Generate New ID", GUILayout.Height(25)))
            {
                potcoInfo.GenerateObjectId();
                EditorUtility.SetDirty(potcoInfo);
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Object Information
            EditorGUILayout.LabelField("Object Information", EditorStyles.boldLabel);
            
            // Object Type Dropdown
            if (availableObjectTypes != null && availableObjectTypes.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                selectedTypeIndex = EditorGUILayout.Popup("Object Type", selectedTypeIndex, availableObjectTypes.ToArray());
                if (EditorGUI.EndChangeCheck() && selectedTypeIndex >= 0 && selectedTypeIndex < availableObjectTypes.Count)
                {
                    objectTypeProp.stringValue = availableObjectTypes[selectedTypeIndex];
                }
            }
            else
            {
                EditorGUILayout.PropertyField(objectTypeProp, new GUIContent("Object Type"));
            }
            
            // Object ID with status indicator
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(objectIdProp, new GUIContent("Object ID"));
            if (string.IsNullOrEmpty(objectIdProp.stringValue))
            {
                EditorGUILayout.LabelField("❌", GUILayout.Width(20));
            }
            else
            {
                EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
            
            // Model Path with status indicator
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(modelPathProp, new GUIContent("Model Path"));
            if (string.IsNullOrEmpty(modelPathProp.stringValue))
            {
                EditorGUILayout.LabelField("❌", GUILayout.Width(20));
            }
            else
            {
                EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Visual Properties
            EditorGUILayout.LabelField("Visual Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(hasVisualBlockProp, new GUIContent("Has Visual Block"));
            
            // Handle nullable Color manually
            EditorGUILayout.BeginHorizontal();
            bool hasColor = potcoInfo.visualColor.HasValue;
            bool newHasColor = EditorGUILayout.Toggle("Use Visual Color", hasColor);
            
            if (newHasColor != hasColor)
            {
                if (newHasColor)
                {
                    potcoInfo.visualColor = Color.white;
                }
                else
                {
                    potcoInfo.visualColor = null;
                }
                EditorUtility.SetDirty(potcoInfo);
            }
            
            if (newHasColor)
            {
                Color currentColor = potcoInfo.visualColor ?? Color.white;
                Color newColor = EditorGUILayout.ColorField(currentColor);
                if (newColor != currentColor)
                {
                    potcoInfo.visualColor = newColor;
                    EditorUtility.SetDirty(potcoInfo);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Object Properties
            EditorGUILayout.LabelField("Object Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(disableCollisionProp, new GUIContent("Disable Collision"));
            EditorGUILayout.PropertyField(instancedProp, new GUIContent("Instanced"));
            EditorGUILayout.PropertyField(holidayProp, new GUIContent("Holiday"));
            EditorGUILayout.PropertyField(visSizeProp, new GUIContent("Vis Size"));
            
            EditorGUILayout.Space();
            
            // Export Status
            EditorGUILayout.LabelField("Export Status", EditorStyles.boldLabel);
            bool readyToExport = !string.IsNullOrEmpty(objectIdProp.stringValue) && !string.IsNullOrEmpty(objectTypeProp.stringValue);
            
            if (readyToExport)
            {
                EditorGUILayout.HelpBox("✅ Ready to export!", MessageType.Info);
            }
            else
            {
                string issues = "";
                if (string.IsNullOrEmpty(objectIdProp.stringValue)) issues += "• Missing Object ID\n";
                if (string.IsNullOrEmpty(objectTypeProp.stringValue)) issues += "• Missing Object Type\n";
                
                EditorGUILayout.HelpBox($"❌ Cannot export:\n{issues}", MessageType.Warning);
            }
            
            // Display detected info
            if (!string.IsNullOrEmpty(modelPathProp.stringValue))
            {
                string modelName = System.IO.Path.GetFileNameWithoutExtension(modelPathProp.stringValue);
                EditorGUILayout.HelpBox($"📋 Detected Model: {modelName}", MessageType.None);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
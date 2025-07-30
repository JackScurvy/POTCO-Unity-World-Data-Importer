using UnityEditor;
using UnityEngine;
using WorldDataImporter.Data;
using WorldDataImporter.Algorithms;
using Unity.EditorCoroutines.Editor;
using POTCO.Editor;

public class WorldSceneBuilderEditor : EditorWindow
{
    private ImportSettings settings = new ImportSettings();
    private ImportStatistics lastImportStats;
    private Vector2 scrollPosition;
    private bool showAdvancedSettings = false;
    private bool showStatistics = false;

    [MenuItem("POTCO/World Data Importer")]
    public static void ShowWindow()
    {
        GetWindow<WorldSceneBuilderEditor>("World Scene Importer");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Enhanced World Data Importer", EditorStyles.boldLabel);
        GUILayout.Space(10);

        DrawBasicSettings();
        GUILayout.Space(10);
        
        DrawAdvancedSettings();
        GUILayout.Space(10);
        
        DrawImportActions();
        GUILayout.Space(10);
        
        DrawStatistics();

        EditorGUILayout.EndScrollView();
    }

    private void DrawBasicSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Basic Import Settings", EditorStyles.boldLabel);
        
        // ObjectList data toggle - at the very top
        EditorGUILayout.BeginHorizontal();
        settings.importObjectListData = EditorGUILayout.Toggle("Import ObjectList Data", settings.importObjectListData);
        if (settings.importObjectListData) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("   Adds ObjectListInfo components for World Data Exporter compatibility (2x slower import)", EditorStyles.miniLabel);
        
        GUILayout.Space(5);
        
        // Model source toggle
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Model Source:", GUILayout.Width(100));
        settings.useEggFiles = EditorGUILayout.Toggle(settings.useEggFiles, GUILayout.Width(20));
        GUILayout.Label(settings.useEggFiles ? "Use .egg files (direct import)" : "Use .prefab files", EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // File selection
        if (GUILayout.Button("Select World .py File"))
        {
            string selected = EditorUtility.OpenFilePanel("Select .py File", "Assets/Editor/World Data Importer/WorldData", "py");
            if (!string.IsNullOrEmpty(selected))
            {
                settings.filePath = selected;
                DebugLogger.LogWorldImporter($"📄 Selected file: {settings.filePath}");
            }
        }

        if (!string.IsNullOrEmpty(settings.filePath))
        {
            EditorGUILayout.LabelField("Selected File:", System.IO.Path.GetFileName(settings.filePath));
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawAdvancedSettings()
    {
        EditorGUILayout.BeginVertical("box");
        
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;
            
            GUILayout.Label("Import Options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            settings.applyColorOverrides = EditorGUILayout.Toggle("Apply Color Overrides", settings.applyColorOverrides);
            if (settings.applyColorOverrides) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Applies Visual.Color properties to object materials", EditorStyles.miniLabel);
            
            EditorGUILayout.BeginHorizontal();
            settings.importCollisions = EditorGUILayout.Toggle("Import Collisions", settings.importCollisions);
            if (settings.importCollisions) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Controls whether collision objects are created during import", EditorStyles.miniLabel);
            
            EditorGUILayout.BeginHorizontal();
            settings.addLighting = EditorGUILayout.Toggle("Add Lighting", settings.addLighting);
            if (settings.addLighting) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Creates Unity Light components from 'Light - Dynamic' objects", EditorStyles.miniLabel);
            
            EditorGUILayout.BeginHorizontal();
            settings.importNodes = EditorGUILayout.Toggle("Import Nodes", settings.importNodes);
            if (settings.importNodes) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Import spawn points, locators, and other node objects (usually not needed for visuals)", EditorStyles.miniLabel);
            
            GUILayout.Space(5);
            GUILayout.Label("Filtering Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            settings.importHolidayObjects = EditorGUILayout.Toggle("Import Holiday Objects", settings.importHolidayObjects);
            if (settings.importHolidayObjects) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Include objects with Holiday properties (uncheck to skip holiday decorations)", EditorStyles.miniLabel);
            
            GUILayout.Space(5);
            GUILayout.Label("Performance Options", EditorStyles.boldLabel);
            settings.showImportStatistics = EditorGUILayout.Toggle("Show Import Statistics", settings.showImportStatistics);
            settings.logDetailedInfo = EditorGUILayout.Toggle("Log Detailed Info", settings.logDetailedInfo);
            
            GUILayout.Space(5);
            GUILayout.Label("Generation Delay", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            settings.useGenerationDelay = EditorGUILayout.Toggle("Use Generation Delay", settings.useGenerationDelay);
            if (settings.useGenerationDelay) EditorGUILayout.LabelField("✅", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("   Adds delay between object creation to prevent Unity freezing", EditorStyles.miniLabel);
            
            if (settings.useGenerationDelay)
            {
                EditorGUI.indentLevel++;
                settings.delayBetweenObjects = EditorGUILayout.Slider("Delay (seconds)", settings.delayBetweenObjects, 0.001f, 0.1f);
                EditorGUILayout.LabelField($"   {settings.delayBetweenObjects:F3} seconds between objects", EditorStyles.miniLabel);
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawImportActions()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Import Actions", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(settings.filePath));
        
        if (GUILayout.Button("🚧 Build Scene", GUILayout.Height(30)))
        {
            DebugLogger.LogWorldImporter($"🚧 Starting enhanced world build... (Using {(settings.useEggFiles ? ".egg files" : ".prefab files")})");
            
            // Only disable AutoObjectListDetection if user wants ObjectList data
            if (settings.importObjectListData)
            {
                DebugLogger.LogWorldImporter("📋 ObjectList data import enabled - disabling AutoObjectListDetection during import for speed");
                AutoObjectListDetection.SetEnabled(false);
            }
            else
            {
                DebugLogger.LogWorldImporter("⚡ ObjectList data import disabled - maximum speed import (no ObjectListInfo components)");
            }
            
            if (settings.useGenerationDelay)
            {
                DebugLogger.LogWorldImporter($"⏱️ Using generation delay: {settings.delayBetweenObjects:F3} seconds between objects");
                EditorCoroutineUtility.StartCoroutine(SceneBuildingAlgorithm.BuildSceneFromPythonCoroutine(settings.filePath, settings.useEggFiles, settings, (stats) => {
                    lastImportStats = stats;
                    showStatistics = true;
                    
                    // Only process ObjectList data if enabled
                    if (settings.importObjectListData)
                    {
                        AutoObjectListDetection.SetEnabled(true);
                        AutoObjectListDetection.ProcessAllObjectsInScene();
                    }
                    
                    Repaint(); // Refresh the UI when done
                }), this);
            }
            else
            {
                lastImportStats = SceneBuildingAlgorithm.BuildSceneFromPython(settings.filePath, settings.useEggFiles, settings);
                showStatistics = true;
                
                // Only process ObjectList data if enabled
                if (settings.importObjectListData)
                {
                    AutoObjectListDetection.SetEnabled(true);
                    AutoObjectListDetection.ProcessAllObjectsInScene();
                }
            }
        }
        
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }

    private void DrawStatistics()
    {
        if (lastImportStats == null) return;
        
        EditorGUILayout.BeginVertical("box");
        
        showStatistics = EditorGUILayout.Foldout(showStatistics, $"📊 Last Import Statistics ({lastImportStats.importTime:F2}s)", true);
        if (showStatistics)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Total Objects:", lastImportStats.totalObjects.ToString());
            EditorGUILayout.LabelField("Successful Imports:", lastImportStats.successfulImports.ToString());
            EditorGUILayout.LabelField("Missing Models:", lastImportStats.missingModels.ToString());
            EditorGUILayout.LabelField("Color Overrides:", lastImportStats.colorOverrides.ToString());
            EditorGUILayout.LabelField("Collision Disabled:", lastImportStats.collisionDisabled.ToString());
            EditorGUILayout.LabelField("Collisions Removed:", lastImportStats.collisionRemoved.ToString());
            EditorGUILayout.LabelField("Lights Created:", lastImportStats.lightsCreated.ToString());
            
            if (lastImportStats.objectTypeCount.Count > 0)
            {
                GUILayout.Space(5);
                GUILayout.Label("Object Types:", EditorStyles.boldLabel);
                foreach (var kvp in lastImportStats.objectTypeCount)
                {
                    EditorGUILayout.LabelField($"  {kvp.Key}:", kvp.Value.ToString());
                }
            }
            
            if (lastImportStats.missingModelPaths.Count > 0)
            {
                GUILayout.Space(5);
                GUILayout.Label("Missing Models:", EditorStyles.boldLabel);
                foreach (string path in lastImportStats.missingModelPaths)
                {
                    EditorGUILayout.LabelField($"  ❌ {path}");
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
    }
}
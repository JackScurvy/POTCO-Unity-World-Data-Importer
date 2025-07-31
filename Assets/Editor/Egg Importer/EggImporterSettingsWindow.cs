using UnityEngine;
using UnityEditor;
using System.IO;
using POTCO.Editor;

public class EggImporterSettingsWindow : EditorWindow
{
    private EggImporterSettings settings;
    private SerializedObject serializedSettings;
    private Vector2 scrollPosition;
    private int selectedTab = 0;
    private readonly string[] tabs = { "⚙️ Settings", "📁 Manual Import", "📊 Statistics", "ℹ️ Info" };
    
    // UI Styles
    private GUIStyle headerStyle;
    private GUIStyle sectionStyle;
    private GUIStyle buttonStyle;
    private GUIStyle tabButtonStyle;
    private GUIStyle activeTabButtonStyle;
    
    // Cached statistics data to prevent lag
    private bool statisticsCached = false;
    private int cachedEggFileCount = 0;
    private string cachedTotalSize = "";
    private int cachedGeneratedPrefabs = 0;
    private float lastStatisticsRefresh = 0f;
    
    [MenuItem("POTCO/EGG Importer Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<EggImporterSettingsWindow>("EGG Importer Manager");
        window.minSize = new Vector2(500, 400);
        window.Show();
    }
    
    private void OnEnable()
    {
        settings = EggImporterSettings.Instance;
        serializedSettings = new SerializedObject(settings);
    }
    
    private void InitializeStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };
        }
        
        if (sectionStyle == null)
        {
            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(15, 15, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
        }
        
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 35
            };
        }
        
        if (tabButtonStyle == null)
        {
            tabButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30
            };
        }
        
        if (activeTabButtonStyle == null)
        {
            activeTabButtonStyle = new GUIStyle(tabButtonStyle);
            activeTabButtonStyle.normal.background = activeTabButtonStyle.active.background;
        }
    }
    
    private void OnGUI()
    {
        if (settings == null || serializedSettings == null)
        {
            OnEnable();
            return;
        }
        
        serializedSettings.Update();
        InitializeStyles();
        
        // Header
        DrawHeader();
        
        // Tab Navigation
        DrawTabNavigation();
        
        // Content Area
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        switch (selectedTab)
        {
            case 0:
                DrawSettingsTab();
                break;
            case 1:
                DrawManualImportTab();
                break;
            case 2:
                DrawStatisticsTab();
                break;
            case 3:
                DrawInfoTab();
                break;
        }
        
        EditorGUILayout.EndScrollView();
        
        // Footer
        DrawFooter();
    }
    
    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        GUILayout.Label("🥚 EGG Importer Manager", headerStyle);
        GUILayout.Space(5);
        
        bool autoImportEnabled = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        string statusText = autoImportEnabled ? "✅ Auto-Import: ENABLED" : "⚠️ Auto-Import: DISABLED";
        var statusStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        statusStyle.normal.textColor = autoImportEnabled ? Color.green : Color.yellow;
        GUILayout.Label(statusText, statusStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawTabNavigation()
    {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < tabs.Length; i++)
        {
            var style = selectedTab == i ? activeTabButtonStyle : tabButtonStyle;
            if (GUILayout.Button(tabs[i], style))
            {
                selectedTab = i;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
    
    private void DrawSettingsTab()
    {
        EditorGUI.BeginChangeCheck();
        
        // Auto-Import Settings
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🔄 Auto-Import Control", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        bool autoImportEnabled = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        bool newAutoImportEnabled = EditorGUILayout.Toggle("Enable Auto-Import", autoImportEnabled);
        if (newAutoImportEnabled != autoImportEnabled)
        {
            EditorPrefs.SetBool("EggImporter_AutoImportEnabled", newAutoImportEnabled);
        }
        
        if (autoImportEnabled)
        {
            EditorGUILayout.HelpBox("✅ EGG files will be automatically processed when Unity starts or when files are added.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Auto-import is DISABLED. EGG files will not be processed automatically. Use the Manual Import tab when needed.", MessageType.Warning);
        }
        
        GUILayout.Space(10);
        
        // Startup Prompt Settings
        bool skipStartupPrompt = EditorPrefs.GetBool("EggImporter_SkipStartupPrompt", false);
        bool newSkipStartupPrompt = EditorGUILayout.Toggle("Disable Startup Import Prompt", skipStartupPrompt);
        if (newSkipStartupPrompt != skipStartupPrompt)
        {
            EditorPrefs.SetBool("EggImporter_SkipStartupPrompt", newSkipStartupPrompt);
        }
        
        if (newSkipStartupPrompt)
        {
            EditorGUILayout.HelpBox("🔕 Startup prompt is DISABLED. No import dialog will appear when opening the project.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("💬 Startup prompt is ENABLED. Will ask to import EGG files when opening the project (if auto-import is disabled).", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
        
        // LOD Import Settings
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("📊 LOD Import Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        var lodModeProperty = serializedSettings.FindProperty("lodImportMode");
        EditorGUILayout.PropertyField(lodModeProperty, new GUIContent("LOD Import Mode"));
        
        switch ((EggImporterSettings.LODImportMode)lodModeProperty.enumValueIndex)
        {
            case EggImporterSettings.LODImportMode.HighestOnly:
                EditorGUILayout.HelpBox("🎯 Only imports the highest quality LOD (Distance ending with 0). Recommended for most use cases.", MessageType.Info);
                break;
            case EggImporterSettings.LODImportMode.AllLODs:
                EditorGUILayout.HelpBox("📈 Imports all LOD levels as separate GameObjects. Useful for analyzing LOD differences.", MessageType.Info);
                break;
            case EggImporterSettings.LODImportMode.Custom:
                EditorGUILayout.HelpBox("🔧 Custom LOD selection (Not yet implemented).", MessageType.Warning);
                break;
        }
        EditorGUILayout.EndVertical();
        
        // Collision Import Settings
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("💥 Collision Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        var collisionProperty = serializedSettings.FindProperty("importCollisions");
        EditorGUILayout.PropertyField(collisionProperty, new GUIContent("Import Collisions"));
        EditorGUILayout.HelpBox("🔒 Enable to import collision geometry. Disabled by default as collision meshes are usually not needed for visual purposes.", MessageType.Info);
        EditorGUILayout.EndVertical();
        
        // Debug Settings
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🐛 Debug Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        var debugProperty = serializedSettings.FindProperty("enableDebugLogging");
        EditorGUILayout.PropertyField(debugProperty, new GUIContent("Enable Debug Logging"));
        EditorGUILayout.HelpBox("📝 Enable detailed logging during EGG import process for troubleshooting.", MessageType.None);
        EditorGUILayout.EndVertical();
        
        if (EditorGUI.EndChangeCheck())
        {
            serializedSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
        }
    }
    
    private void DrawManualImportTab()
    {
        bool autoImportEnabled = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        
        // Status Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("📋 Import Status", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if (autoImportEnabled)
        {
            EditorGUILayout.HelpBox("✅ Auto-import is currently ENABLED. EGG files should import automatically.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Auto-import is DISABLED. Use the manual import buttons below to process EGG files.", MessageType.Warning);
        }
        EditorGUILayout.EndVertical();
        
        // Quick Actions
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚡ Quick Actions", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔄 Enable Auto-Import", buttonStyle))
        {
            EditorPrefs.SetBool("EggImporter_AutoImportEnabled", true);
            DebugLogger.LogEggImporter("Auto-import enabled via EGG Importer Manager.");
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("⏸️ Disable Auto-Import", buttonStyle))
        {
            EditorPrefs.SetBool("EggImporter_AutoImportEnabled", false);
            DebugLogger.LogEggImporter("Auto-import disabled via EGG Importer Manager.");
        }
        EditorGUILayout.EndVertical();
        
        // Manual Import Options
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("📁 Manual Import Options", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("📂 Import Selected EGG Files", buttonStyle))
        {
            ImportSelectedEggFiles();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🗂️ Import All EGG Files in Project", buttonStyle))
        {
            ImportAllEggFiles();
        }
        
        EditorGUILayout.HelpBox("💡 Tip: Select .egg files in the Project window before using 'Import Selected EGG Files'.", MessageType.Info);
        EditorGUILayout.EndVertical();
        
        // Startup Prompt Testing
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🧪 Testing & Utilities", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔔 Test Startup Prompt", buttonStyle))
        {
            // Reset the prompt flag and show it
            var promptType = System.Type.GetType("EggImportStartupPrompt");
            if (promptType != null)
            {
                var hasPromptedField = promptType.GetField("hasPrompted", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (hasPromptedField != null)
                {
                    hasPromptedField.SetValue(null, false);
                }
                
                var method = promptType.GetMethod("ShowStartupPrompt", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method?.Invoke(null, null);
            }
        }
        
        EditorGUILayout.HelpBox("🧪 Use 'Test Startup Prompt' to preview the startup dialog that appears when opening the project.", MessageType.Info);
        EditorGUILayout.EndVertical();
    }
    
    private void DrawStatisticsTab()
    {
        // Only refresh when first opening tab or manually requested
        if (!statisticsCached)
        {
            RefreshStatisticsCache();
        }
        
        // Project Overview Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("📊 Project Overview", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("Total EGG Files:", cachedEggFileCount.ToString());
        EditorGUILayout.LabelField("Total EGG File Size:", cachedTotalSize);
        EditorGUILayout.LabelField("Generated Prefabs:", cachedGeneratedPrefabs.ToString());
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("🔄 Refresh", GUILayout.Width(80)))
        {
            RefreshStatisticsCache();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        // Import Performance Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⏱️ Import Performance", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        // Get statistics from EditorPrefs
        int totalImports = EditorPrefs.GetInt("EggImporter_TotalImports", 0);
        float totalImportTime = EditorPrefs.GetFloat("EggImporter_TotalImportTime", 0f);
        int failedImports = EditorPrefs.GetInt("EggImporter_FailedImports", 0);
        
        EditorGUILayout.LabelField("Total Imports This Session:", totalImports.ToString());
        EditorGUILayout.LabelField("Failed Imports:", failedImports.ToString());
        EditorGUILayout.LabelField("Total Import Time:", $"{totalImportTime:F2} seconds");
        
        if (totalImports > 0)
        {
            float avgTime = totalImportTime / totalImports;
            EditorGUILayout.LabelField("Average Import Time:", $"{avgTime:F2} seconds");
            EditorGUILayout.LabelField("Success Rate:", $"{((totalImports - failedImports) / (float)totalImports * 100):F1}%");
        }
        else
        {
            EditorGUILayout.LabelField("Average Import Time:", "No imports yet");
            EditorGUILayout.LabelField("Success Rate:", "No data");
        }
        
        EditorGUILayout.EndVertical();
        
        // Recent Activity Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🕒 Recent Activity", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        string lastImportTime = EditorPrefs.GetString("EggImporter_LastImportTime", "Never");
        string lastImportFile = EditorPrefs.GetString("EggImporter_LastImportFile", "None");
        
        EditorGUILayout.LabelField("Last Import:", lastImportTime);
        EditorGUILayout.LabelField("Last File:", lastImportFile);
        
        EditorGUILayout.EndVertical();
        
        // Material Statistics Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🎨 Material Statistics", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        int createdMaterials = EditorPrefs.GetInt("EggImporter_CreatedMaterials", 0);
        int texturesFound = EditorPrefs.GetInt("EggImporter_TexturesFound", 0);
        int texturesMissing = EditorPrefs.GetInt("EggImporter_TexturesMissing", 0);
        
        EditorGUILayout.LabelField("Materials Created:", createdMaterials.ToString());
        EditorGUILayout.LabelField("Textures Found:", texturesFound.ToString());
        EditorGUILayout.LabelField("Textures Missing:", texturesMissing.ToString());
        
        EditorGUILayout.EndVertical();
        
        // System Information Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("💻 System Information", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("Unity Version:", Application.unityVersion);
        EditorGUILayout.LabelField("Platform:", Application.platform.ToString());
        EditorGUILayout.LabelField("System Memory:", $"{SystemInfo.systemMemorySize} MB");
        EditorGUILayout.LabelField("Graphics Memory:", $"{SystemInfo.graphicsMemorySize} MB");
        
        EditorGUILayout.EndVertical();
        
        // Actions Section
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🔧 Statistics Actions", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔄 Reset Import Statistics", buttonStyle))
        {
            bool confirmed = EditorUtility.DisplayDialog("Reset Statistics", 
                "Are you sure you want to reset all import statistics? This cannot be undone.", "Reset", "Cancel");
            if (confirmed)
            {
                EditorPrefs.DeleteKey("EggImporter_TotalImports");
                EditorPrefs.DeleteKey("EggImporter_TotalImportTime");
                EditorPrefs.DeleteKey("EggImporter_FailedImports");
                EditorPrefs.DeleteKey("EggImporter_LastImportTime");
                EditorPrefs.DeleteKey("EggImporter_LastImportFile");
                EditorPrefs.DeleteKey("EggImporter_CreatedMaterials");
                EditorPrefs.DeleteKey("EggImporter_TexturesFound");
                EditorPrefs.DeleteKey("EggImporter_TexturesMissing");
                DebugLogger.LogEggImporter("Import statistics have been reset.");
            }
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📝 Export Statistics Report", buttonStyle))
        {
            string reportPath = EditorUtility.SaveFilePanel("Save Statistics Report", "", "EggImporter_Statistics", "txt");
            if (!string.IsNullOrEmpty(reportPath))
            {
                ExportStatisticsReport(reportPath, cachedEggFileCount, cachedTotalSize, cachedGeneratedPrefabs, totalImports, 
                    totalImportTime, failedImports, lastImportTime, lastImportFile, createdMaterials, 
                    texturesFound, texturesMissing);
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void RefreshStatisticsCache()
    {
        // Count EGG files in project
        string[] eggFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.egg", System.IO.SearchOption.AllDirectories);
        cachedEggFileCount = eggFiles.Length;
        
        // Calculate total file size
        long totalSize = 0;
        foreach (string file in eggFiles)
        {
            totalSize += new System.IO.FileInfo(file).Length;
        }
        cachedTotalSize = totalSize < 1024 * 1024 ? $"{totalSize / 1024} KB" : $"{totalSize / (1024 * 1024)} MB";
        
        // Count imported prefabs (only in Resources folders)
        string[] prefabFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        cachedGeneratedPrefabs = 0;
        foreach (string prefab in prefabFiles)
        {
            string relativePath = "Assets" + prefab.Substring(Application.dataPath.Length).Replace('\\', '/');
            if (relativePath.Contains("/Resources/"))
                cachedGeneratedPrefabs++;
        }
        
        statisticsCached = true;
        lastStatisticsRefresh = (float)EditorApplication.timeSinceStartup;
    }
    
    private void ExportStatisticsReport(string filePath, int totalEggFiles, string totalSize, int generatedPrefabs,
        int totalImports, float totalImportTime, int failedImports, string lastImportTime, string lastImportFile,
        int createdMaterials, int texturesFound, int texturesMissing)
    {
        try
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== EGG Importer Statistics Report ===");
            report.AppendLine($"Generated: {System.DateTime.Now}");
            report.AppendLine($"Unity Version: {Application.unityVersion}");
            report.AppendLine();
            
            report.AppendLine("PROJECT OVERVIEW:");
            report.AppendLine($"  Total EGG Files: {totalEggFiles}");
            report.AppendLine($"  Total File Size: {totalSize}");
            report.AppendLine($"  Generated Prefabs: {generatedPrefabs}");
            report.AppendLine();
            
            report.AppendLine("IMPORT PERFORMANCE:");
            report.AppendLine($"  Total Imports: {totalImports}");
            report.AppendLine($"  Failed Imports: {failedImports}");
            if (totalImports > 0)
            {
                report.AppendLine($"  Average Import Time: {(totalImportTime / totalImports):F2} seconds");
                report.AppendLine($"  Success Rate: {((totalImports - failedImports) / (float)totalImports * 100):F1}%");
            }
            report.AppendLine();
            
            report.AppendLine("RECENT ACTIVITY:");
            report.AppendLine($"  Last Import: {lastImportTime}");
            report.AppendLine($"  Last File: {lastImportFile}");
            report.AppendLine();
            
            report.AppendLine("MATERIAL STATISTICS:");
            report.AppendLine($"  Materials Created: {createdMaterials}");
            report.AppendLine($"  Textures Found: {texturesFound}");
            report.AppendLine($"  Textures Missing: {texturesMissing}");
            report.AppendLine();
            
            report.AppendLine("SYSTEM INFORMATION:");
            report.AppendLine($"  Platform: {Application.platform}");
            report.AppendLine($"  System Memory: {SystemInfo.systemMemorySize} MB");
            report.AppendLine($"  Graphics Memory: {SystemInfo.graphicsMemorySize} MB");
            
            System.IO.File.WriteAllText(filePath, report.ToString());
            EditorUtility.DisplayDialog("Export Complete", $"Statistics report saved to:\n{filePath}", "OK");
            DebugLogger.LogEggImporter($"Statistics report exported to: {filePath}");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Export Failed", $"Failed to export statistics report:\n{e.Message}", "OK");
            DebugLogger.LogErrorEggImporter($"Failed to export statistics report: {e.Message}");
        }
    }
    
    private void DrawInfoTab()
    {
        // Version Info
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("ℹ️ System Information", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("EGG Importer Version:", "2.0");
        EditorGUILayout.LabelField("Unity Version:", Application.unityVersion);
        EditorGUILayout.LabelField("Auto-Import Status:", EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false) ? "Enabled" : "Disabled");
        EditorGUILayout.EndVertical();
        
        // LOD Information
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("📊 LOD Detection", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("🔍 LOD Detection: Groups with <SwitchCondition> and <Distance> tags are considered LODs. The LOD with Distance ending in 0 is considered the highest quality.", MessageType.Info);
        EditorGUILayout.EndVertical();
        
        // Supported Features
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("✨ Supported Features", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("• Multi-texture DelFuego patterns");
        EditorGUILayout.LabelField("• Ground UV coordinate fixes");
        EditorGUILayout.LabelField("• Skeletal animations and bones");
        EditorGUILayout.LabelField("• LOD level management");
        EditorGUILayout.LabelField("• Collision geometry import");
        EditorGUILayout.LabelField("• Smart texture detection");
        EditorGUILayout.LabelField("• Auto-import control");
        
        EditorGUILayout.EndVertical();
        
        // Help
        EditorGUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("❓ Need Help?", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("📖 Visit the GitHub main page for detailed documentation, usage instructions, and support.", MessageType.Info);
        EditorGUILayout.EndVertical();
    }
    
    private void DrawFooter()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔄 Reset All Settings", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset all EGG importer settings to defaults?", "Yes", "Cancel"))
            {
                settings.lodImportMode = EggImporterSettings.LODImportMode.HighestOnly;
                settings.importCollisions = false;
                settings.enableDebugLogging = true;
                EditorPrefs.SetBool("EggImporter_AutoImportEnabled", false);
                EditorPrefs.SetBool("EggImporter_SkipStartupPrompt", false); // Enable startup prompt by default
                EditorUtility.SetDirty(settings);
                DebugLogger.LogEggImporter("All EGG importer settings reset to defaults.");
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void ImportSelectedEggFiles()
    {
        var selectedGuids = Selection.assetGUIDs;
        int importedCount = 0;
        
        foreach (string guid in selectedGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath.EndsWith(".egg", System.StringComparison.OrdinalIgnoreCase))
            {
                ForceImportEggFile(assetPath);
                importedCount++;
            }
        }
        
        if (importedCount == 0)
        {
            DebugLogger.LogEggImporter("No EGG files selected. Please select EGG files in the Project window.");
            EditorUtility.DisplayDialog("No EGG Files", "No EGG files were selected. Please select EGG files in the Project window and try again.", "OK");
        }
        else
        {
            DebugLogger.LogEggImporter($"Successfully imported {importedCount} EGG files manually.");
            EditorUtility.DisplayDialog("Import Complete", $"Successfully imported {importedCount} EGG files.", "OK");
            
            // Refresh statistics after import
            statisticsCached = false;
        }
    }
    
    private void ImportAllEggFiles()
    {
        string[] eggFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.egg", SearchOption.AllDirectories);
        int importedCount = 0;
        
        if (eggFiles.Length == 0)
        {
            DebugLogger.LogEggImporter("No EGG files found in the project.");
            EditorUtility.DisplayDialog("No EGG Files", "No EGG files were found in the project.", "OK");
            return;
        }
        
        bool proceed = EditorUtility.DisplayDialog("Import All EGG Files", 
            $"Found {eggFiles.Length} EGG files in the project. This may take some time. Continue?", "Yes", "Cancel");
            
        if (!proceed) return;
        
        foreach (string fullPath in eggFiles)
        {
            string relativePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
            relativePath = relativePath.Replace('\\', '/');
            
            ForceImportEggFile(relativePath);
            importedCount++;
            
            // Show progress
            EditorUtility.DisplayProgressBar("Importing EGG Files", 
                $"Importing {Path.GetFileName(relativePath)}...", (float)importedCount / eggFiles.Length);
        }
        
        EditorUtility.ClearProgressBar();
        DebugLogger.LogEggImporter($"Successfully imported {importedCount} EGG files manually.");
        EditorUtility.DisplayDialog("Import Complete", $"Successfully imported {importedCount} EGG files.", "OK");
        
        // Refresh statistics after import
        statisticsCached = false;
    }
    
    private void ForceImportEggFile(string assetPath)
    {
        DebugLogger.LogEggImporter($"Force importing: {assetPath}");
        
        // Temporarily enable auto-import for this specific import
        bool originalSetting = EditorPrefs.GetBool("EggImporter_AutoImportEnabled", false);
        EditorPrefs.SetBool("EggImporter_AutoImportEnabled", true);
        
        try
        {
            // Force reimport the asset
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
        finally
        {
            // Restore original setting
            EditorPrefs.SetBool("EggImporter_AutoImportEnabled", originalSetting);
        }
    }
}